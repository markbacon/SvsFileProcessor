using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvsFileProcessor {


    public enum Action {
        ProcessDailyFiles,
        ProcessWeeklyFiles,
		ProcessPeriodFiles
    }

    public enum Concept {
        CarlsJr,
        Hardees
    }


    public enum FileType {
        Daily,
        Period,
        Weekly
    }


	class AppSettings {
        //---------------------------------------------------------------------------------------------------
        public static string ArchiveDirectory {

            get {
                return ConfigurationManager.AppSettings["ArchiveDirectory"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static bool DeleteFilesAfterTransfer {

            get {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["DeleteFilesAfterTransfer"]);
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string EmailPassword {

            get {
                return ConfigurationManager.AppSettings["EmailPassword"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static int EmailPort {

            get {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string EmailServer {

            get {
                return ConfigurationManager.AppSettings["EmailServer"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string EmailUserName {

            get {
                return ConfigurationManager.AppSettings["EmailUserName"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static bool ErrorEmailEnabled {

            get {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ErrorEmailEnabled"]);
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string ErrorEmailSubject {

            get {
                return ConfigurationManager.AppSettings["ErrorEmailSubject"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string ErrorEmailToAddress {

            get {
                return ConfigurationManager.AppSettings["ErrorEmailToAddress"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static bool SessionLoggingEnabled {

            get {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["SessionLoggingEnabled"]);
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsCarlsSftpPassword {

            get {
                return ConfigurationManager.AppSettings["SvsCarlsSftpPassword"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsCarlsSftpUserName {

            get {
                return ConfigurationManager.AppSettings["SvsCarlsSftpUserName"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsHardeesSftpPassword {

            get {
                return ConfigurationManager.AppSettings["SvsHardeesSftpPassword"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsHardeesSftpUserName {

            get {
                return ConfigurationManager.AppSettings["SvsHardeesSftpUserName"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsSftpFingerprint {

            get {
                return ConfigurationManager.AppSettings["SvsSftpFingerprint"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsSftpHostName {

            get {
                return ConfigurationManager.AppSettings["SvsSftpHostName"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsZipFileEmail {

            get {
                return ConfigurationManager.AppSettings["SvsZipFileEmail"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string SvsZipFileName {

            get {
                return ConfigurationManager.AppSettings["SvsZipFileName"];
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static string WorkingDirectoryPath {

            get {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["WorkingDirectory"]);
            }
        }
    }
}
