using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using WinSCP;

namespace SvsFileProcessor {

	class CkeSftp : IDisposable {

		public void Close() {

			if (_session != null) {
				if (_session.Opened) {
					_session.Close();
				}
			}
		}
		//---------------------------------------------------------------------------------------------------
		// Public implementation of Dispose pattern callable by consumers.
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		//---------------------------------------------------------------------------------------------------
		public List<RemoteFileInfo> CheckForFiles(string remoteDirectoryName, string searchPattern) {

			List<RemoteFileInfo> remoteFiles = null;

			try {
				remoteFiles = _session.EnumerateRemoteFiles(remoteDirectoryName, searchPattern, EnumerationOptions.None).ToList();
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in CheckForFiles.  Please see error log for details.");
				Logger.WriteError(ex);
			}
			
			return remoteFiles;
		}
		//---------------------------------------------------------------------------------------------------
		public bool GetFile(string remoteDirectoryName, string fileName) {

			bool isSuccessful = true;

			try {
				TransferOptions transferOptions = new TransferOptions();
				transferOptions.TransferMode = TransferMode.Ascii;
				

				string remoteFilePath = RemotePath.Combine(remoteDirectoryName, fileName);

				TransferEventArgs transferResult = _session.GetFileToDirectory(remoteFilePath, AppSettings.WorkingDirectoryPath, AppSettings.DeleteFilesAfterTransfer, transferOptions);


				if (transferResult.Error == null) {
					Logger.Write("SFTP file transfer succeeded for: " + fileName);
				}
				else {
					Logger.Write("SFTP file transfer failed for: " + fileName + ".  Please see error log for details.");
					Logger.WriteError(transferResult.Error.Message);
				}
			}
			catch (Exception ex) {
				Logger.Write("An exception occurred in GetFile. Please see error log for details.");
				Logger.WriteError(ex);

				isSuccessful = false;
			}

			return isSuccessful;
		}
		//---------------------------------------------------------------------------------------------------
		public void GetFiles(string remoteDirectoryName, string fileName) {

			TransferOptions transferOptions = new TransferOptions();
			transferOptions.TransferMode = TransferMode.Binary;

			string remoteFilePath = RemotePath.Combine(remoteDirectoryName, fileName);

			TransferOperationResult transferResult = _session.GetFiles(remoteFilePath, AppSettings.WorkingDirectoryPath, AppSettings.DeleteFilesAfterTransfer, transferOptions);

			// Throw on any error
			transferResult.Check();

			// Print results
			foreach (TransferEventArgs transfer in transferResult.Transfers) {
				Logger.Write("SFTP file transfer succeeded for: " + transfer.FileName);
			}
		}
		//---------------------------------------------------------------------------------------------------
		public bool IsOpened {

			get {
				bool isSessionOpened = false;

				if (_session != null) {
					isSessionOpened = _session.Opened;
				}
				return isSessionOpened;
			}
		}
		//---------------------------------------------------------------------------------------------------
		public List<string> ListFiles() {

			List<string> fileNameList = new List<string>();

			RemoteDirectoryInfo remoteDirectory = _session.ListDirectory("*");

			foreach (RemoteFileInfo rfi in remoteDirectory.Files) {

				if (!rfi.IsDirectory) {
					fileNameList.Add(rfi.Name);
				}
			}

			return fileNameList;
		}
		//---------------------------------------------------------------------------------------------------
		public List<string> ListFiles(string remoteDirectoryName) {

			List<string> fileNameList = new List<string>();

			string remoteFilePath = remoteDirectoryName; //_session.CombinePaths(remoteDirectoryName, "*");

			RemoteDirectoryInfo remoteDirectory = _session.ListDirectory(remoteFilePath);

			foreach (RemoteFileInfo rfi in remoteDirectory.Files) {

				if (!rfi.IsDirectory) {
					fileNameList.Add(rfi.Name);
				}
			}

			return fileNameList;
		}
		//---------------------------------------------------------------------------------------------------
		public List<string> ListFiles(string remoteDirectoryName, string searchPattern) {

			List<string> fileNameList = new List<string>();

			string remoteFilePath = "";

			if (!string.IsNullOrEmpty(remoteDirectoryName)) {

				remoteFilePath = remoteDirectoryName;// _session.CombinePaths(remoteDirectoryName, searchPattern);
			}
			else {
				remoteFilePath = ".";
			}


			RemoteDirectoryInfo remoteDirectory = _session.ListDirectory(remoteFilePath);


			foreach (RemoteFileInfo rfi in remoteDirectory.Files) {
				if (!rfi.IsDirectory) {

					if (searchPattern == "*") {
						fileNameList.Add(rfi.Name);
					}

					else if (rfi.Name.IndexOf(searchPattern, StringComparison.CurrentCultureIgnoreCase) > -1) {
						fileNameList.Add(rfi.Name);
					}
				}
			}

			return fileNameList;
		}
		//---------------------------------------------------------------------------------------------------
		public void Open(string userName, string password) {

			SessionOptions sessionOptions = new SessionOptions {
				Protocol = Protocol.Sftp,
				HostName = AppSettings.SvsSftpHostName,
				UserName = userName,
				Password = password,
				PortNumber = 22,
				SshHostKeyFingerprint = AppSettings.SvsSftpFingerprint
			};

			if (_session != null) {
				if (_session.Opened) {
					_session.Close();
				}
			}

			_session = new Session();

			if (AppSettings.SessionLoggingEnabled) {
				_session.SessionLogPath = GetSessionLogFilePath();
			}

			sessionOptions.FtpMode = FtpMode.Active;

			_session.Open(sessionOptions);
		}
		//---------------------------------------------------------------------------------------------------
		public void SendSSH(string hostName, string userName, string sftpFingerprint, string privateKeyPassphrase, string privateKeyFilePath, string sourceFilePath, string remoteDirectoryName, bool deleteFilesAfterTransfer) {

			Session sshSession = null;

			try {

				SessionOptions sessionOptions = new SessionOptions {
					Protocol = Protocol.Sftp,
					HostName = hostName,
					UserName = userName,
					SshHostKeyFingerprint = sftpFingerprint,
					SshPrivateKeyPath = privateKeyFilePath,
					PrivateKeyPassphrase = privateKeyPassphrase
				};

				sshSession = new Session();

				if (AppSettings.SessionLoggingEnabled) {
					sshSession.SessionLogPath = GetSessionLogFilePath();
				}

				sessionOptions.FtpMode = FtpMode.Active;

				sshSession.Open(sessionOptions);


				TransferOptions transferOptions = new TransferOptions();
				transferOptions.TransferMode = TransferMode.Binary;

				string remoteFilePath = RemotePath.Combine(remoteDirectoryName, Path.GetFileName(sourceFilePath));


				TransferOperationResult transferResult = sshSession.PutFiles(sourceFilePath, remoteFilePath, deleteFilesAfterTransfer, transferOptions);

				// Throw on any error
				transferResult.Check();

				// Print results
				foreach (TransferEventArgs transfer in transferResult.Transfers) {
					Logger.Write("Send SSH SFTP file transfer succeeded for: " + transfer.FileName);
				}
			}

			catch (Exception ex) {
				Logger.Write("An exception occurred in SendSSH. Please see error logs for details.");
				Logger.WriteError(ex);
			}
			finally {
				if (sshSession != null) {
					if (sshSession.Opened) {
						sshSession.Close();
					}
				}
			}
		}
		//---------------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------------
		//-- Protected
		//---------------------------------------------------------------------------------------------------
		// Protected implementation of Dispose pattern.
		protected virtual void Dispose(bool disposing) {

			if (_disposed)
				return;

			if (disposing) {
				if (_session != null) {
					if (_session.Opened) {
						_session.Close();
					}
				}
			}

			_disposed = true;
		}
		//---------------------------------------------------------------------------------------------------
		//-- Private
		//---------------------------------------------------------------------------------------------------
		private Session _session = null;
		private bool _disposed = false;
		//---------------------------------------------------------------------------------------------------
		private string GetSessionLogFilePath() {

			string sessionDirectoryName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SessionLogs");

			if (!Directory.Exists(sessionDirectoryName)) {
				Directory.CreateDirectory(sessionDirectoryName);
			}

			string fileName = "SFTPSessions." + DateTime.Today.ToString("yyyyMMdd") + ".log";

			return Path.Combine(sessionDirectoryName, fileName);
		}
	}
}
