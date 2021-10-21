using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Microsoft.Win32;
using Convert = System.Convert;

#pragma warning disable 1573

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedVariable
// ReSharper disable RedundantAssignment
// ReSharper disable once CheckNamespace
namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts Source to Byte[]
        /// </summary>
        /// <returns>Byte[]</returns>
        public static byte[] SourceToByte(this Image img)
        {
            try
            {
                byte[] bytes;
                var encoder = new PngBitmapEncoder();

                if (!(img.Source is BitmapSource bitmapSource)) return null;

                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }

                return bytes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Convert ImageSource to Byte[]
        /// </summary>
        public static byte[] ImageSourceToByte(this ImageSource img)
        {
            try
            {
                byte[] bytes;
                var encoder = new PngBitmapEncoder();

                if (!(img is BitmapSource bitmapSource)) return null;

                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }

                return bytes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Convert BitmapImage to Byte[]
        /// </summary>
        public static byte[] BitmapImageToBytes(this BitmapImage img)
        {
            byte[] bytes;
            var encoder = new PngBitmapEncoder();

            if (!(img is BitmapSource bitmapSource)) return null;

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                bytes = stream.ToArray();
            }

            return bytes;
        }

        /// <summary>
        /// Converts Byte[] to Source
        /// </summary>
        public static void ByteToSource(this Image img, byte[] bytes)
        {
            try
            {
                var biImg = new BitmapImage();
                var ms = new MemoryStream(bytes);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();

                img.Source = biImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Converts Byte[] to ImageSource
        /// </summary>
        public static void ByteToImageSource(this ImageSource img, byte[] bytes)
        {
            try
            {
                var biImg = new BitmapImage();
                var ms = new MemoryStream(bytes);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();

                img = biImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Converts Byte[] to BitmapImage
        /// </summary>
        public static void BytesToBitmapImage(this BitmapImage img, byte[] bytes)
        {
            try
            {
                var biImg = new BitmapImage();
                var ms = new MemoryStream(bytes);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();

                img = biImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Sets string as ImageSource
        /// </summary>
        public static void Source(this Image img, string source, bool useUri = false, UriKind uri = UriKind.Relative)
        {
            try
            {
                img.Source = useUri ? new BitmapImage(new Uri(source, uri)) : new BitmapImage(new Uri(source));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Checks if string Starts-With or Contains another string.
        /// </summary>
        /// <param name="check">String to compare against source</param>
        /// <param name="toLower">Convert all chars to lower case (boolean, default: false)</param>
        /// <returns>boolean</returns>
        public static bool StartsContains(this string str, string check, bool toLower = false)
        {
            return toLower
                ? str.ToLower().StartsWith(check.ToLower()) || str.ToLower().Contains(check.ToLower())
                : str.StartsWith(check) || str.Contains(check);
        }

        /// <summary>
        /// Replaces text at given index, splits on desired index & glues string back with inserted text
        /// </summary>
        /// <param name="str">source string</param>
        /// <param name="index">where to replce</param>
        /// <param name="txt">text to put on said index</param>
        /// <param name="spaces">Add spaces between streing start, inserted text & end or no. Default: False = No Spaces</param>
        /// <returns>New string with inserted text</returns>
        public static string ReplaceAt(this string str, int index, string txt, bool spaces = false)
        {
            if (string.IsNullOrEmpty(str) || index < 0 && index > str.Length || string.IsNullOrEmpty(txt))
                return string.Empty;

            var start = string.Empty;

            var end = string.Empty;

            for(var i = 0; i < index - 1; i++)
                start += str[i];

            for (var j = start.Length + 1; j < str.Length; j++)
                end += str[j];

            return !spaces ? $"{start}{txt}{end}" : $"{start} {txt} {end}";
        }

        /// <summary>
        /// Get day suffix (i.e. 31st, 2nd, 3rd, 4th)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetDaySuffix(this DateTime date)
        {
            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        /// <summary>
        /// <para>Returns Month Day-suffix, Year Time</para>
        /// <para>Example: 09:11 (UTC), September 13th, 2021</para>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static string GetUSTimeDate(this DateTime date, string zone = "UTC")
        {
            return $"{date:MMMM} {date.Day}{date.GetDaySuffix()}, {date:yyyy, HH:mm} ({zone})";
        }

        #region StringCypher

        private const string CypherKey =
            @"kL88g!nio0EhZe@tiSZcRMVjOMa5YHyyBZ7*lca3Qr77az#SzTK2Zt6^c4kHk0@E$GvR@VnhvMxg@@e4aK^s0y5%VXgaHMW0$yK";

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        /// <summary>
        /// Encrypts a string according to sent Key
        /// </summary>
        /// <param name="passPhrase">Encryption Key</param>
        /// <returns>Encrypted string according to sent Key</returns>
        public static string Encrypt(this string plainText, string passPhrase = CypherKey)
        {
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(passPhrase))
                return string.Empty;

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dencrypts a string according to sent Key
        /// </summary>
        /// <param name="passPhrase">Encryption Key</param>
        /// <returns>Decrypted string according to sent Key</returns>
        public static string Decrypt(this string cipherText, string passPhrase = CypherKey)
        {
            if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(passPhrase))
                return string.Empty;

            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2)
                .Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        #endregion

        /// <summary>
        /// Get DateTime of Start of Week
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// <para>Sets OpenFileDialog Filter to accept only Images</para>
        /// <para>File Types: jpg, jpeg, jpe, jfif, png, bmp, dib, gif, tif, tiff</para>
        /// </summary>
        public static void ImageTypes(this OpenFileDialog diag)
        {
            try
            {
                if (diag == null)
                    return;

                var imageFileTypes = new[]
                {
                    "jpg",
                    "jpeg",
                    "jpe",
                    "jfif",
                    "png",
                    "bmp",
                    "dib",
                    "gif",
                    "tif",
                    "tiff",
                    "webp"
                };

                diag.Title = "Please select an Image";
                diag.Filter =
                    $"Image files ({imageFileTypes.Aggregate(string.Empty, (str, i) => str + $"*.{i},")}) | {imageFileTypes.Aggregate(string.Empty, (str, i) => str + $"*.{i};")}";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                    action(item);
            }
        }

        public static void ForEach<T>(this ObservableCollection<T> source, List<Action<T>> actions)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (actions == null) throw new ArgumentNullException(nameof(actions));

            foreach (var item in source)
            {
                foreach (var action in actions)
                {
                    action(item);
                }
            }
        }
    }

    #region XML

    public static class Xml
    {
        public static void Serialize<T>(string path, object model)
        {
            //create XmlSerializer, assign sent Type
            var xs = new XmlSerializer(typeof(T));

            using (var wr = new StreamWriter($@"{path}"))
            {
                //serialize sent model into xml file
                xs.Serialize(wr, (T)model);
            }
        }

        public static T Deserialize<T>(string path) where T : class
        {
            //create XmlSerializer, assign sent Type
            var xs = new XmlSerializer(typeof(T));

            using (var rd = new StreamReader($@"{path}"))
            {
                //return deserialized file
                return (T)xs.Deserialize(rd);
            }
        }
    }

    #endregion

    #region IAsyncCommand

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly IErrorHandler _errorHandler;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            IErrorHandler errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion
    }

    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly IErrorHandler _errorHandler;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion
    }

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

    public static class TaskUtilities
    {
        #pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
        #pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }

    #endregion

    #region OnPropertyChanged

    public class OnPropertyChange : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public RelayCommand(Action<object> execute) : this(execute, DefaultCanExecute)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute != null && _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        private event EventHandler CanExecuteChangedInternal;

        // ReSharper disable once UnusedMember.Global
        public void OnCanExecuteChanged()
        {
            var handler = CanExecuteChangedInternal;
            handler?.Invoke(this, EventArgs.Empty);
        }

        // ReSharper disable once UnusedMember.Global
        public void Destroy()
        {
            _canExecute = _ => false;
            _execute = _ => { };
        }

        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }

    #endregion

    #region Message Logging

    public enum MessageShow
    {
        Yes = 1,
        No = 2
    }

    public static class Message
    {
        /// <summary>
        ///     Logs every Exception into [MachineName].log file, showing error message to end-user.
        /// </summary>
        public static void Log(Exception e, MessageShow opt = MessageShow.Yes)
        {
            try
            {
                // ReSharper disable once CommentTypo
                //deafult file name
                var logFilename = $@"{Environment.MachineName}.log";
                var logFolder = logFilename;

                //write error into the logfile
                using (var fs = new FileStream(logFolder, FileMode.Append, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine("====================");
                        sw.WriteLine($"[Date] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        sw.WriteLine($"[PC Name] {Environment.MachineName}");
                        sw.WriteLine($"[Error Message] {e.Message}");
                        sw.WriteLine("[Stack Trace]");
                        sw.WriteLine($"{e.StackTrace}\n\n");
                        sw.WriteLine("[InnerException]");
                        sw.WriteLine($"{e.InnerException}");
                        sw.WriteLine("====================");
                        sw.WriteLine(" ");
                    }
                }

                if (opt == MessageShow.Yes)
                {
                    Show(e.Message, "An Error occured", MessageBoxButton.OK, MessageBoxImage.Error);

                    //if (win != null)
                    //{
                    //    MetroShow("Something went wrong", e.Message);
                    //}
                    //else
                    //{
                    //    Show(e.Message, "An Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                }
            }
            catch (Exception ex)
            {
                //show this message in case there is an issue with the logging
                MessageBox.Show(ex.Message, "Error Logging Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool Show(string text, string caption = "",
            MessageBoxButton btn = MessageBoxButton.OK,
            MessageBoxImage img = MessageBoxImage.None,
            MessageBoxResult result = MessageBoxResult.None,
            MessageBoxOptions optn = MessageBoxOptions.DefaultDesktopOnly)
        {
            return MessageBox.Show(text, caption, btn, img, result, optn) == MessageBoxResult.Yes;
        }

        public static bool MetroShow(string text, string caption = "",
            MahApps.Metro.Controls.MetroWindow win = null,
            MessageDialogStyle btn = MessageDialogStyle.Affirmative)
        {
            return win.ShowMessageAsync(caption, text, btn).Result == MessageDialogResult.Affirmative;
        }
    }

    #endregion

    #region Check Another Instance

    public static class DuplicateProgramInstance
    {
        /// <summary>
        /// Checks for open duplicate instances of program,
        /// notifies user if so.
        /// </summary>
        public static void Check(string applicationName = "")
        {
#if !DEBUG
            var currentProcess = Process.GetCurrentProcess();

            var runningProcess =
            (
                from process in Process.GetProcesses()
                where process.Id != currentProcess.Id &&
                      process.ProcessName.Equals(currentProcess.ProcessName, StringComparison.Ordinal)
                select process
            ).FirstOrDefault();

            if (runningProcess == null) return;

            MessageBox.Show(
                $"Another instance of {(string.IsNullOrEmpty(applicationName) ? "the application" : $"'{applicationName}'")} is already running, please close it before opening another.",
                "Duplicate instance found...", MessageBoxButton.OK, MessageBoxImage.Information);

            Environment.Exit(0);
#endif
        }
    }

    #endregion

    #region Converters

    /// <summary>
    /// Converts Byte[] to ImageSource in XAML
    /// </summary>
    public class ByteToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is byte[] bytes))
                return null;

            var biImg = new BitmapImage();
            var ms = new MemoryStream(bytes);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            return biImg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts file string to ImageSource in XAML
    /// </summary>
    public class StringToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var image = value as string;

            try
            {
                if (image == null)
                    return null;

                return image.Contains("/") 
                    ? new BitmapImage(new Uri(image))
                    : new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}/{image}"));
            }
            catch
            {
                try
                {
                    return image != null && image.Contains("/")
                        ? new BitmapImage(new Uri(image, UriKind.Relative))
                        : new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}/{image}", UriKind.Relative));
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    #endregion
}
