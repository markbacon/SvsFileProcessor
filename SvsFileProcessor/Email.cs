using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SvsFileProcessor {

    class Email {

        //---------------------------------------------------------------------------------------------------
        public List<string> AttachmentFileNames {

            get {
                return _attachmentFileNames;
            }
        }
        //---------------------------------------------------------------------------------------------------
        public string CCAddress {

            get {
                return _ccAddress;
            }
            set {
                _ccAddress = value;
            }
        }
        //---------------------------------------------------------------------------------------------------
        public string Message {

            get {
                return _message;
            }
            set {
                _message = value;
            }
        }
        //---------------------------------------------------------------------------------------------------
        public void Send() {


            try {
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                Logger.Write("Email.Send starting...");

                MailMessage message = new MailMessage();

                message.From = new MailAddress(AppSettings.EmailUserName);

                string[] toAddresses = _toAddress.Split(new char[] { ';' });

                for (int i = 0; i < toAddresses.Length; i++) {

                    message.To.Add(toAddresses[i].Trim());
                }

                message.Subject = _subject;
                message.Body = _message;
                if (!String.IsNullOrEmpty(_ccAddress)) {
                    message.CC.Add(_ccAddress);
                }

                foreach (string attachmentFileName in _attachmentFileNames) {

                    if (attachmentFileName.Length > 0) {
                        Attachment attachment = new Attachment(attachmentFileName);
                        message.Attachments.Add(attachment);
                    }
                }

                SmtpClient c = new SmtpClient() {
                    UseDefaultCredentials = false,
                    Host = AppSettings.EmailServer,
                    Credentials = new NetworkCredential(AppSettings.EmailUserName, AppSettings.EmailPassword),
                    Port = AppSettings.EmailPort,
                    EnableSsl = true
                };

                c.Send(message);

                Logger.Write("Email.Send has completed. Elapsed time: " + sw1.Elapsed.ToString());
            }

            catch (Exception ex) {
                Logger.Write("An exception has occurred in Email.Send. Please see error log for details.");
                Logger.WriteError(ex);
            }
        }
        //---------------------------------------------------------------------------------------------------
        public string Subject {

            get {
                return _subject;
            }
            set {
                _subject = value;
            }
        }
        //---------------------------------------------------------------------------------------------------
        public string ToAddress {

            get {
                return _toAddress;
            }
            set {
                _toAddress = value;
            }
        }
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //-- Private Variables
        //---------------------------------------------------------------------------------------------------
        private string _ccAddress = "";
        private string _message = "";
        private string _toAddress = "";
        private string _subject = "";

        private List<string> _attachmentFileNames = new List<string>();
    }
}
