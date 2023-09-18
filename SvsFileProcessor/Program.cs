using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SvsFileProcessor {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {

			try {

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				if (args.Length > 0) {
					ParseCommandLineArgs(args);
				}

				Application.Run(new frmResponse(_action));
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in Main. Please see error log for details.");
				Logger.WriteError(ex);
			}
			finally {

				if (AppSettings.ErrorEmailEnabled) {
					if (Logger.ErrorCount > 0) {
						Email mail = new Email();
						mail.Message = Logger.ErrorBuffer;
						mail.Subject = AppSettings.ErrorEmailSubject;
						mail.ToAddress = AppSettings.ErrorEmailToAddress;

						mail.Send();
					}
				}
			}
		}
		//---------------------------------------------------------------------------------------------------
		//-- Private
		//---------------------------------------------------------------------------------------------------
		private static Action _action = Action.ProcessDailyFiles;
		private static void ParseCommandLineArgs(string[] args) {

			foreach (string arg in args) {

				string[] items = arg.Split('=');

				switch (items[0].ToLower()) {

					case "/action":
						_action = (Action)Enum.Parse(typeof(Action), items[1]);
						break;

				}
			}
		}
	}
}
