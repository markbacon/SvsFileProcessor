using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvsFileProcessor {
	class Controller {

		public void Run(Action action) {

			try {
				Stopwatch sw1 = new Stopwatch();
				sw1.Start();
				Logger.Write("Controller.Run starting...");

				FileProcessor fp = new FileProcessor();

				switch (action) {

					case Action.ProcessDailyFiles:
						fp.ProcessDailyFiles();
						break;

					case Action.ProcessPeriodFiles:
						fp.ProcessPeriodFiles();
						break;

					case Action.ProcessWeeklyFiles:
						fp.ProcessWeeklyFiles();
						break;
				}


				Logger.Write("Controller.Run has completed.  Elapsed time: " + sw1.Elapsed.ToString());
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in Controller.Run.  Please see error log for details.");
				Logger.WriteError(ex);
			}
		}
	}
}
