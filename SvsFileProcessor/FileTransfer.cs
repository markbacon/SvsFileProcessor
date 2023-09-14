using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ionic.Zip;

namespace SvsFileProcessor {
	class FileTransfer {
		public void ZipAndMailFiles() {

			try {
				Stopwatch sw1 = new Stopwatch();
				sw1.Start();
				Logger.Write("ZipAndMailFiles starting...");

				string zipFileName = AppSettings.SvsZipFileName.Replace("!YYYY-MM-DD", DateTime.Today.ToString("yyyy-MM-dd"));

				string zipFilePath = Path.Combine(AppSettings.WorkingDirectoryPath, zipFileName);


				DirectoryInfo di = new DirectoryInfo(AppSettings.WorkingDirectoryPath);
				FileInfo[] svsFiles =  di.GetFiles();

				using (ZipFile zippy = new ZipFile(zipFilePath)) {

					foreach (FileInfo svsFile in svsFiles) {
						zippy.AddFile(svsFile.FullName, "");
					}

					zippy.Save();
				}

				EmailZipFile(zipFilePath);

				Logger.Write("ZipAndMailFiles has completed. Elapsed time: " + sw1.Elapsed.ToString());
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in ZipAndMailFiles. Please see error log for details.");
				Logger.WriteError(ex);
			}
		}
		//---------------------------------------------------------------------------------------------------
		//-- Private
		//---------------------------------------------------------------------------------------------------

		private void EmailZipFile(string zipFilePath) {

			Email mail = new Email();

			mail.ToAddress = AppSettings.SvsZipFileEmail;
			mail.Subject = "SVS Zip File Attached.";

			mail.AttachmentFileNames.Add(zipFilePath);

			mail.Send();

		}
	}
}
