using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinSCP;

namespace SvsFileProcessor {
	class FileProcessor {

		public FileProcessor() {

			if (!Directory.Exists(AppSettings.WorkingDirectoryPath)) {
				Logger.Write("Creating directory: " + AppSettings.WorkingDirectoryPath);
				Directory.CreateDirectory(AppSettings.WorkingDirectoryPath);
			}

		}
		//---------------------------------------------------------------------------------------------------
		public void ProcessDailyFiles() {

			CkeSftp ckeSftp = new CkeSftp();
			try {
				Stopwatch sw1 = new Stopwatch();
				sw1.Start();
				Logger.Write("ProcessDailyFiles starting...");

				AppUtility.ClearWorkingDirectory();

				int fileCount = GetFiles(FileType.Daily, Concept.CarlsJr);
				fileCount += GetFiles(FileType.Daily, Concept.Hardees);

				if (fileCount > 0) {
					AppUtility.ArchiveFiles();
				}

				Logger.Write("ProcessDailyFiles has completed.  Elapsed time: " + sw1.Elapsed.ToString());
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in ProcessDailyFiles.  Please see error log for details.");
				Logger.WriteError(ex);
			}
			finally {
				ckeSftp.Close();
			}
		}
		//---------------------------------------------------------------------------------------------------
		public void ProcessPeriodFiles() {

			try {
				Stopwatch sw1 = new Stopwatch();
				sw1.Start();
				Logger.Write("ProcessPeriodFiles starting...");

				AppUtility.ClearWorkingDirectory();

				int fileCount = GetFiles(FileType.Period, Concept.CarlsJr);
				fileCount += GetFiles(FileType.Period, Concept.Hardees);

				if (fileCount > 0) {

					FileTransfer fileTrans = new FileTransfer();
					fileTrans.ZipAndMailFiles();

					AppUtility.ArchiveFiles();
				}

				Logger.Write("ProcessPeriodFiles has completed.  Elapsed time: " + sw1.Elapsed.ToString());
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in ProcessPeriodFiles.  Please see error log for details.");
				Logger.WriteError(ex);
			}
		}
		//---------------------------------------------------------------------------------------------------
		public void ProcessWeeklyFiles() {

			try {
				Stopwatch sw1 = new Stopwatch();
				sw1.Start();
				Logger.Write("ProcessWeeklyFiles starting...");

				AppUtility.ClearWorkingDirectory();

				int fileCount = GetFiles(FileType.Weekly, Concept.CarlsJr);
				fileCount += GetFiles(FileType.Weekly, Concept.Hardees);

				if (fileCount > 0) {

					FileTransfer fileTrans = new FileTransfer();
					fileTrans.ZipAndMailFiles();

					AppUtility.ArchiveFiles();
				}

				Logger.Write("ProcessWeeklyFiles has completed.  Elapsed time: " + sw1.Elapsed.ToString());
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in ProcessWeeklyFiles.  Please see error log for details.");
				Logger.WriteError(ex);
			}
		}
		//---------------------------------------------------------------------------------------------------
		//-- Private
		//---------------------------------------------------------------------------------------------------

		private void AddDateToFileName(string fileName) {

			string srcFilePath = Path.Combine(AppSettings.WorkingDirectoryPath, fileName);

			FileInfo theFile = new FileInfo(srcFilePath);

			if (theFile.Exists) {
				theFile.CopyTo(srcFilePath + "." + DateTime.Today.ToString("yyyyMMdd"), true);
				theFile.Delete();

			}
		}

		//---------------------------------------------------------------------------------------------------
		private int GetFiles(FileType fileType, Concept concept) {

			int fileCount = 0;
			CkeSftp ckeSftp = new CkeSftp();

			try {
				Stopwatch sw1 = new Stopwatch();
				sw1.Start();
				Logger.Write("GetFiles starting...");

				SearchPatternManifest manifest = new SearchPatternManifest();

				List<FileSearchPattern> fileSearchPatternList = manifest.GetSearchPatternList(fileType, concept);

				if (concept == Concept.CarlsJr) {
					ckeSftp.Open(AppSettings.SvsCarlsSftpUserName, AppSettings.SvsCarlsSftpPassword);
				}
				else {
					ckeSftp.Open(AppSettings.SvsHardeesSftpUserName, AppSettings.SvsHardeesSftpPassword);
				}

				foreach (FileSearchPattern fsp in fileSearchPatternList) {

					Logger.Write("Checking for files using search pattern: " + fsp.SearchPattern);

					List<RemoteFileInfo> remoteFiles = ckeSftp.CheckForFiles("/", fsp.SearchPattern);

					Logger.Write(remoteFiles.Count().ToString() + " file(s) found.");
					fileCount += remoteFiles.Count();

					foreach (RemoteFileInfo remoteFile in remoteFiles) {
						ckeSftp.GetFile("/", remoteFile.Name);

						if (fsp.AddDateToFileName) {
							AddDateToFileName(remoteFile.Name);
						}
					}
				}

				Logger.Write("GetFiles has completed.  Elapsed time: " + sw1.Elapsed.ToString());

			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in GetFiles.  Please see error log for details.");
				Logger.WriteError(ex);
			}

			return fileCount;
		}
	}
}

