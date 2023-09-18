using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvsFileProcessor {
	class AppUtility {

		public static void ArchiveFile(string filePath) {

			try {
				string archiveDirName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppSettings.ArchiveDirectory, DateTime.Today.ToString("yyyy-MM-dd"));

				if (!Directory.Exists(archiveDirName)) {
					Directory.CreateDirectory(archiveDirName);
				}

				string fileName = Path.GetFileName(filePath);

				string archiveFilePath = Path.Combine(archiveDirName, fileName);

				if (File.Exists(archiveFilePath)) {

					int counter = 1;

					do {
						fileName = Path.GetFileNameWithoutExtension(filePath)  + "(" + counter.ToString() + ").csv"; ;
						archiveFilePath = Path.Combine(archiveDirName, fileName);

						counter++;

					} while (File.Exists(archiveFilePath));
				}

				File.Copy(filePath, archiveFilePath);
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in ArchiveFile. Please see error log for details.");
				Logger.WriteError(ex);
			}
		}
		//---------------------------------------------------------------------------------------------------------
		public static void ArchiveFiles() {

			string[] rptFilePaths = Directory.GetFiles(AppSettings.WorkingDirectoryPath);

			foreach (string rptFilePath in rptFilePaths) {
				ArchiveFile(rptFilePath);
			}
		}
		//---------------------------------------------------------------------------------------------------------
		public static void ClearWorkingDirectory() {

			try {
				if (Directory.Exists(AppSettings.WorkingDirectoryPath)) {

					string[] rptFileNames = Directory.GetFiles(AppSettings.WorkingDirectoryPath);

					foreach (string rptFileName in rptFileNames) {

						File.Delete(rptFileName);
					}
				}
				else {
					Directory.CreateDirectory(AppSettings.WorkingDirectoryPath);
				}
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in ClearWorkingDirectory. Please see error log for details.");
				Logger.WriteError(ex);

			}
		}
		//---------------------------------------------------------------------------------------------------------
		//-- Private
		//---------------------------------------------------------------------------------------------------------
		private static string SemFilePath {

			get {
				string fileName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name) + ".sem";

				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
			}
		}
	}
}
