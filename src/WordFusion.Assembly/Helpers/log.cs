using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFusion.Assembly.MailMerge.Helpers
{
    public static class logger
    {

        /// <summary>
        /// Log the error and return
        /// </summary>
        /// <param name="ee">The ee.</param>
        /// <param name="errormessage">The user friendly error.</param>
        /// <returns></returns>
        public static void LogMessage(string errormessage, string userfriendlymessage, Exception ex)
        {

            using (System.Security.Principal.WindowsIdentity.Impersonate(IntPtr.Zero))
            {

                try
                {
                    bool log = false;
                    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Logging"])) log = true;

                    if (log)
                    {
                        if (log && !errormessage.Contains("deadlocked"))
                        {
                            //if (log) {
                            String logpath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];

                            // check if directory exists
                            if (Directory.Exists(logpath))
                            {

                                logpath = logpath + "\\ZumeDocs_" + DateTime.Today.ToString("yyyy-MM-dd") + ".log";
                                // check if file exist
                                if (!File.Exists(logpath))
                                {
                                    File.Create(logpath).Dispose();
                                }

                                // log the error now
                                using (StreamWriter writer = File.AppendText(logpath))
                                {
                                    string error = "\r\nLog written at : " + DateTime.Now.ToString() +
                                    "\r\n\r\nMessage :\n" + errormessage;
                                    writer.WriteLine(error);
                                    writer.WriteLine("==========================================");
                                    writer.Flush();
                                    writer.Close();
                                }

                                // Purge Old Log Files
                                //LogFilePurge();

                            }
                        }
                    }

                    if (errormessage == "WordFusion.Server.User.Logon User already logged on at different workstation")
                    {
                        throw new Exception(Environment.NewLine + Environment.NewLine + "User already logged on at different workstation.");
                    }

                    if (errormessage.Trim() == "WordFusion.Server.User.UserInsert User is already registered. If you have forgotten your password use the recover password link.")
                    {
                        throw new Exception(Environment.NewLine + Environment.NewLine + "This Email address is already registered. If you have forgotten your password use the recover password link.");
                    }

                    if (ex != null)
                    {
                        if (!ex.Message.Contains("deadlocked"))
                        {
                            if (ex.Message.Trim() == "Security.UserIsValid: Not authorised or session expired.")
                            {
                                throw new Exception(Environment.NewLine + Environment.NewLine + "Not authorised or session expired.");
                            }
                            else
                            {
                                throw new Exception(userfriendlymessage);
                            }
                        }
                    }
                }
                catch
                {

                }

            }

            //public static  void LogFilePurge()
            //{

            //    String logpath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];

            //    int cnt = 6;

            //    if (System.Configuration.ConfigurationManager.AppSettings["LogPurge"] != null)
            //    {
            //        cnt = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["LogPurge"]);
            //    }

            //    string[] files = Directory.GetFiles(logpath);
            //    logpath = logpath + "\\ZumeDocs_" + DateTime.Today.ToString("yyyy-MM-dd") + ".log";

            //    foreach (string file in files)
            //    {
            //        FileInfo fi = new FileInfo(file);
            //        if (fi.LastWriteTime < DateTime.Now.AddMonths(-cnt))
            //        {

            //            try
            //            {

            //                fi.Delete();

            //                // Log deletion in current log file
            //                // Dont call log message function as will start a recursion

            //                // check if file exist
            //                if (!File.Exists(logpath))
            //                {
            //                    File.Create(logpath).Dispose();
            //                }

            //                // log the error now
            //                using (StreamWriter writer = File.AppendText(logpath))
            //                {
            //                    string error = "\r\nLog written at : " + DateTime.Now.ToString() +
            //                    "\r\n\r\nMessage :\n" + "Purging old log file " + fi.Name;
            //                    writer.WriteLine(error);
            //                    writer.WriteLine("==========================================");
            //                    writer.Flush();
            //                    writer.Close();
            //                }
            //            }
            //            catch
            //            {
            //                // Continue regardless as file locks are possible. 
            //                // Log files can be purged next time if reqiured
            //            }

            //        }
            //    }

            //}

        }
    }
}
