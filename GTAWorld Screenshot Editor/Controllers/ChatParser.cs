using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ExtensionMethods;

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
        /// <param name="linesToParse"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public static string ParseChatLog(string directoryPath, int linesToParse = 100, bool showError = false)
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
                
                //remove timestamps from log
                log = Regex.Replace(log, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty);

                //split log into lines
                var lines = log.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //remvoe paid date if you were paid
                lines.ForEach(fe =>
                    Regex.Replace(fe, @"( \(\d{2}/[A-z]{3}/\d{4} - \d{2}:\d{2}:\d{2}\))", string.Empty));

                //remove '[!] ' highlight on chat lines if any.
                lines.ForEach(fe => fe = fe.Replace("[!] ", ""));

                //pull only n > 100  or n < 1000 of last lines
                if (lines.Count > 100)
                {
                    //get last n amount of lines
                    return lines.Skip(Math.Max(0, lines.Count - linesToParse)).ToList()
                        .Aggregate(string.Empty, (cur, i) => cur += $"{i}\n");
                }
                
                return lines.Aggregate(string.Empty, (cur, i) => cur += $"{i}\n");
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
                ChatLog.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => !string.IsNullOrEmpty(w)).ToList();

            var filteredResult = string.Empty;

            foreach (var line in lines)
            {
                var str = line;

                //remove datetime from payment if random strangler passes chat-parse
                if (Regex.IsMatch(line, @"( \(\d{2}/[A-z]{3}/\d{4} - \d{2}:\d{2}:\d{2}\))"))
                {
                    str = Regex.Replace(line, @"( \(\d{2}/[A-z]{3}/\d{4} - \d{2}:\d{2}:\d{2}\))", string.Empty);
                }

                var isCriterionEnabled = false;
                var matchedRegularCriterion = false;

                foreach (var criteria in
                    args.Where(keyValuePair => !string.IsNullOrEmpty(keyValuePair.Name) &&
                                               !string.IsNullOrWhiteSpace(keyValuePair.Filter))
                        .Where(keyValuePair =>
                            Regex.IsMatch(Regex.Replace(str, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty),
                                keyValuePair.Filter, RegexOptions.IgnoreCase)))
                {
                    matchedRegularCriterion = Regex.IsMatch(
                        Regex.Replace(str, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty), $@"{criteria.Filter}",
                        RegexOptions.IgnoreCase);

                    if (criteria.Selected != true) continue;

                    isCriterionEnabled = true;
                    break;
                }

                if (isCriterionEnabled || !matchedRegularCriterion && includeOther)
                    filteredResult += $"{str}\n";
            }

            return filteredResult;
        }
    }
}