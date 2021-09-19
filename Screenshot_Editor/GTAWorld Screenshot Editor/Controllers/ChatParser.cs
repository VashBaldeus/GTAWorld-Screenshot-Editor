using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace GTAWorld_Screenshot_Editor
{
    /// <summary>
    /// Full credits for this class are going to Maple (https://github.com/MapleToo/GTAW-Log-Parser)
    /// </summary>
    public static class ChatParser
    {
        public static string ResourceDirectory;
        public static string LogLocation;

        public static string PreviousLog = string.Empty;

        /// <summary>
        /// Initializes the server IPs matching with the
        /// current server depending on the chosen locale
        /// and determines the newest log file if multiple
        /// server IPs are used to connect to the server
        /// </summary>
        public static void InitializeServerIp()
        {
            try
            {
                ResourceDirectory = "Not Found";
                LogLocation = $"client_resources\\{@"play.gta.world_22005"}\\.storage";

                // Return if the user has not picked
                // a RAGEMP directory path yet
                string directoryPath = Properties.Settings.Default.DirectoryPath;
                if (string.IsNullOrWhiteSpace(directoryPath)) return;

                // Get every directory in the client_resources directory found inside directoryPath
                string[] resourceDirectories = Directory.GetDirectories(directoryPath + @"\client_resources");

                // Store each GTA W .storage file path in a List (found by a tag in the .storage file)
                List<string> potentialLogs = new List<string>();
                foreach (string resourceDirectory in resourceDirectories)
                {
                    if (!File.Exists(resourceDirectory + @"\.storage"))
                        continue;

                    string log;
                    using (StreamReader sr = new StreamReader(resourceDirectory + @"\.storage"))
                    {
                        log = sr.ReadToEnd();
                    }

                    if (!Regex.IsMatch(log, "\\\"server_version\\\":\\\"GTA World[^\"]*\""))
                        continue;

                    potentialLogs.Add(resourceDirectory);
                }

                if (potentialLogs.Count == 0) return;

                // Compare the last write time on all .storage files in the List to find the latest one
                foreach (var file in potentialLogs.Select(log => new FileInfo(log + @"\.storage")))
                {
                    file.Refresh();
                }

                while (potentialLogs.Count > 1)
                {
                    potentialLogs.Remove(DateTime.Compare(File.GetLastWriteTimeUtc(potentialLogs[0] + @"\.storage"), File.GetLastWriteTimeUtc(potentialLogs[1] + @"\.storage")) > 0 ? potentialLogs[1] : potentialLogs[0]);
                }

                // Save the directory name that houses the latest .storage file
                int finalSeparator = potentialLogs[0].LastIndexOf(@"\", StringComparison.Ordinal);
                if (finalSeparator == -1) return;

                // Finally, set the log location
                ResourceDirectory = potentialLogs[0].Substring(finalSeparator + 1, potentialLogs[0].Length - finalSeparator - 1);
                LogLocation = $"client_resources\\{ResourceDirectory}\\.storage";
            }
            catch
            {
                // Silent exception
            }
        }

        /// <summary>
        /// Parses the most recent chat log found at the
        /// selected RAGEMP directory path and returns it.
        /// Displays an error if a chat log does not
        /// exist or if it has an incorrect format
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="removeTimestamps"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public static string ParseChatLog(string directoryPath, bool removeTimestamps, bool showError = false)
        {
            try
            {
                // Read the chat log
                string log;
                using (StreamReader sr = new StreamReader(directoryPath + ChatParser.LogLocation))
                {
                    log = sr.ReadToEnd();
                }

                // Grab the chat log part from the .storage file
                log = Regex.Match(log, "\\\"chat_log\\\":\\\".+\\\\n\\\"").Value;
                if (string.IsNullOrWhiteSpace(log))
                    throw new IndexOutOfRangeException();

                // Q: Why REGEX? A: Way faster than parsing the JSON object
                log = log.Replace("\"chat_log\":\"", string.Empty); // Remove the chat log indicator
                log = log.Replace("\\n", "\n");                     // Change all occurrences of `\n` into new lines
                log = log.Remove(log.Length - 1, 1);                // Remove the `"` character from the end

                log = System.Net.WebUtility.HtmlDecode(log);    // Decode HTML symbols (example: `&apos;` into `'`)
                log = log.TrimEnd('\r', '\n');                  // Remove the `new line` characters from the end

                PreviousLog = log;
                if (removeTimestamps)
                    log = Regex.Replace(log, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty);

                //remvoe paid date if you were paid
                log = Regex.Replace(log,
                    @"(\(\d{2}/[A-z]{3}/\d{4} - \d{2}:\d{2}:\d{2}\)\.)", string.Empty);

                //remove paid amount and replace with $xxxx
                log = Regex.Replace(log,
                    @"(?<SYMBOL>[$]){1}\s*(?<AMOUNT>[\d.,]+)", "$xxxx");

                //remove amount of given item
                log = Regex.Replace(log, @"\((?<AMOUNT>[\d.,]+)\)\s(to)", "(x) to");

                //remove amount of received item
                log = Regex.Replace(log, @"(received).\s*(?<AMOUNT>[\d.,]+)", "received (x) of");
                
                log = log.Replace("Character | Money: $xxxx / Bank: $xxxx / Debt: $xxxx / Total Assets: $xxxx", string.Empty);
                log = log.Replace("You were taxed $xxxx for your paycheck income", string.Empty);

                return log;
            }
            catch
            {
                if (showError)
                    MessageBox.Show("An error occurred while parsing the chat log.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return string.Empty;
            }
        }

        public static string TryToFilter(string ChatLog, List<Criteria> args, bool includeOther)
        {
            var lines =
                ChatLog.Split('\n').Where(w => !string.IsNullOrEmpty(w));//.Reverse().Take(100).Reverse();

            var filteredResult = string.Empty;

            //KeyValuePair<string, Tuple<string, bool>> keyValuePair in _filterCriteria
            //    .Where(keyValuePair => !string.IsNullOrWhiteSpace(keyValuePair.Key) &&
            //                           !string.IsNullOrWhiteSpace(keyValuePair.Value.Item1)).Where(keyValuePair =>
            //        Regex.IsMatch(Regex.Replace(line, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty),
            //            keyValuePair.Value.Item1, RegexOptions.IgnoreCase));

            foreach (var line in lines)
            {
                var isCriterionEnabled = false;
                var matchedRegularCriterion = false;

                foreach (var criteria in
                    args.Where(keyValuePair => !string.IsNullOrEmpty(keyValuePair.Name) &&
                                               !string.IsNullOrWhiteSpace(keyValuePair.Filter))
                        .Where(keyValuePair =>
                            Regex.IsMatch(Regex.Replace(line, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty),
                                keyValuePair.Filter, RegexOptions.IgnoreCase)))
                {
                    matchedRegularCriterion = Regex.IsMatch(
                        Regex.Replace(line, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty), $@"{criteria.Filter}",
                        RegexOptions.IgnoreCase);

                    if (criteria.Selected != true) continue;

                    isCriterionEnabled = true;
                    break;
                }

                if (isCriterionEnabled || !matchedRegularCriterion && includeOther)
                    filteredResult += $"{line}\n";
            }

            return filteredResult;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fastFilter"></param>
        //public static Dictionary<int, string> TryToFilter(string ChatLog, List<Criteria> regs)
        //{
        //    string logToCheck = ChatLog;
        //    string[] lines = logToCheck.Split('\n');
        //    List<string> filtered = new List<string>();
        //    Dictionary<int, string> filteredResult = new Dictionary<int, string>();
        //    var count = 0;

        //    // Loop through every line in the
        //    // loaded chat log
        //    foreach (string line in lines)
        //    {
        //        if (string.IsNullOrEmpty(line))
        //            continue;

        //        bool matchedRegularCriterion = false;
        //        bool otherSelected = false;

        //        foreach (var reg in regs)
        //        {
        //            if (reg.Name == "Other")
        //            {
        //                otherSelected = true;
        //                break;
        //            }

        //            matchedRegularCriterion = Regex.IsMatch(
        //                Regex.Replace(line, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty), reg.Filter, RegexOptions.IgnoreCase);

        //            //Console.WriteLine($"reg:{reg.Name}\nline: {line}\nmatchedRegularCriterion: {matchedRegularCriterion}\notherSelected: {otherSelected}");

        //            if (matchedRegularCriterion)
        //                break;
        //        }

        //        // Add the line to the filtered chat log if the criterion is
        //        // enabled or if it didn't match any criterion and Other is enabled
        //        if (matchedRegularCriterion || otherSelected)
        //            filtered.Add($"{line}\n");
        //    }

        //    // Filter successful
        //    if (filtered.Count > 0)
        //    {
        //        foreach (var s in filtered)
        //        {
        //            var str = s.TrimEnd('\r', '\n');

        //            filteredResult.Add(count++, Regex.Replace(str, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty));
        //        }
        //    }
        //    else // Nothing found
        //    {
        //        foreach (var s in filtered)
        //        {
        //            filteredResult.Add(count++, Regex.Replace(s, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty));
        //        }
        //    }

        //    return filteredResult;
        //}
    }
}