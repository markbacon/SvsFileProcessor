using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SvsFileProcessor {

	public delegate void LoggerWrite(object sender, LoggerEventArgs e);

	class Logger {

		public static event LoggerWrite LoggerWrite;

		//---------------------------------------------------------------------------------------------------
		public static string ErrorBuffer {
			get {
				return _sbErrorBuffer.ToString();
			}
		}
		//---------------------------------------------------------------------------------------------------
		public static int ErrorCount {
			get {
				return _errorCount;
			}
		}
		//---------------------------------------------------------------------------------------------------
		public static void StartLogSession() {

			//PurgeLogs();

			StringBuilder sb = new StringBuilder();

			sb.Append("//");
			sb.Append(new string('-', 110));
			sb.Append("\r\n");

			sb.Append("//-- Start Application. Date/Time: ");
			sb.Append(DateTime.Now.ToLongDateString());
			sb.Append("  ");
			sb.Append(DateTime.Now.ToLongTimeString());
			sb.Append("\r\n");

			sb.Append("//");
			sb.Append(new string('-', 110));
			sb.Append("\r\n");

			string logFilePath = Path.Combine(GetLogFilePath(), GetLogFileName());

			using (StreamWriter sw = File.AppendText(logFilePath)) {
				sw.Write(sb.ToString());
			}
		}
		//---------------------------------------------------------------------------------------------------
		public static void Write(string logEntry) {

			if (logEntry.Length > 0) {

				logEntry = FormatLogEntry(logEntry);

				string logFilePath = Path.Combine(GetLogFilePath(), GetLogFileName());

				using (StreamWriter sw = File.AppendText(logFilePath)) {
					sw.WriteLine(logEntry);
				}
			}

			OnLoggerWrite(logEntry);

		}
		//---------------------------------------------------------------------------------------------------
		public static void Write(string logEntry, bool echoLogEntry) {

			if (logEntry.Length > 0) {

				logEntry = FormatLogEntry(logEntry);

				string logFilePath = Path.Combine(GetLogFilePath(), GetLogFileName());

				using (StreamWriter sw = File.AppendText(logFilePath)) {
					sw.WriteLine(logEntry);
				}
			}

			if (echoLogEntry) {
				OnLoggerWrite(logEntry);
			}
		}
		//---------------------------------------------------------------------------------------------------
		public static void WriteError(string message) {

			string logEntry = FormatLogEntry(message);

			string logFilePath = Path.Combine(GetLogFilePath(), GetErrorLogFileName());

			using (StreamWriter sw = File.AppendText(logFilePath)) {
				sw.WriteLine(logEntry);
			}

			_errorCount++;
		}
		//---------------------------------------------------------------------------------------------------
		public static void WriteError(Exception ex) {

			try {

				string logEntry = FormatErrorLogEntry(ex);

				string logFilePath = Path.Combine(GetLogFilePath(), GetErrorLogFileName());

				using (StreamWriter sw = File.AppendText(logFilePath)) {
					sw.WriteLine(logEntry);
				}

				_errorCount++;

				_sbErrorBuffer.Append(logEntry);
				_sbErrorBuffer.Append("\r\n\r\n");
			}
			catch { } //If error log fails do nothing
		}
		//---------------------------------------------------------------------------------------------------
		//-- Protected Methods
		//---------------------------------------------------------------------------------------------------
		protected static void OnLoggerWrite(string message) {

			if (LoggerWrite != null) {
				LoggerEventArgs e = new LoggerEventArgs();
				e.Message = message;
				LoggerWrite(null, e);
			}
		}
		//---------------------------------------------------------------------------------------------------
		//-- Private Methods
		//---------------------------------------------------------------------------------------------------
		private const int MAX_LOG_SIZE = 5000000;



		private static int _errorCount = 0;
		private static string _errorLogFileName = "";
		private static string _logFileName = "";

		private static StringBuilder _sbErrorBuffer = new StringBuilder();
		//---------------------------------------------------------------------------------------------------
		private static string FormatErrorLogEntry(Exception ex) {
			StringBuilder sb = new StringBuilder();

			sb.Append(new string('-', 110));
			sb.Append("\r\n  Exception Summary\r\n");
			sb.Append(new string('-', 110));
			sb.Append("\r\nDate/Time: ");
			sb.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
			sb.Append("\r\nError Message: ");
			sb.Append(ex.Message);
			sb.Append("\r\nSource: ");
			sb.Append(ex.Source);
			sb.Append("\r\n\r\nStack Trace: ");
			sb.Append(ex.StackTrace);
			sb.Append("\r\n\r\n");

			while (ex.InnerException != null) {
				ex = ex.InnerException;

				sb.Append("Inner Exception:\r\nError Message: ");
				sb.Append(ex.Message);
				sb.Append("\r\nSource: ");
				sb.Append(ex.Source);
				sb.Append("\r\n\r\nStack Trace: ");
				sb.Append(ex.StackTrace);
				sb.Append("\r\n\r\n");
			}

			return sb.ToString();

		}
		//---------------------------------------------------------------------------------------------------
		private static string FormatLogEntry(string message) {

			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t:" + message;
		}
		//---------------------------------------------------------------------------------------------------
		private static string GetErrorLogFileName() {

			if (_errorLogFileName == "") {

				_errorLogFileName = Assembly.GetExecutingAssembly().GetName().Name + ".Error.log";
			}

			return _errorLogFileName;
		}
		//---------------------------------------------------------------------------------------------------
		private static string GetLogFileName() {

			if (_logFileName == "") {

				_logFileName = Assembly.GetExecutingAssembly().GetName().Name + "." + DateTime.Now.ToString("yyyyMMdd_HHmmss-fff") + ".log";
			}

			return _logFileName;
		}
		//---------------------------------------------------------------------------------------------------
		private static string GetLogFilePath() {

			string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

			//Create directory if it doesn't exist
			if (!Directory.Exists(logFilePath)) {
				Directory.CreateDirectory(logFilePath);
			}

			return logFilePath;
		}
	}
}
