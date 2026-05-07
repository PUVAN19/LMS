using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace Library
{
    public class ErrorLogger : IExceptionPublisher
    {
        private string strLogName = @"C:\ErrorLog.txt";
        //private string strLogName = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +"\\ErrorLog\\ErrorLog.txt";
        private bool boolDisplayExceptionSource = false;
        private bool boolDisplayExceptionStack = false;

        void IExceptionPublisher.Publish(
            Exception exception,
            NameValueCollection AdditionalInfo,
            NameValueCollection ConfigSettings)
        {
            if (ConfigSettings != null)
            {
                if (ConfigSettings["logFileName"] != null &&
                    ConfigSettings["logFileName"].Length > 0)
                    strLogName = System.Web.HttpContext.Current.Server.MapPath(ConfigSettings["logFileName"]);

                if (ConfigSettings["logExceptionSource"] != null &&
                    ConfigSettings["logExceptionSource"].Length > 0)
                {
                    if (ConfigSettings["logExceptionSource"].ToLower() == "true") boolDisplayExceptionSource = true;
                }

                if (ConfigSettings["logExceptionStack"] != null &&
                    ConfigSettings["logExceptionStack"].Length > 0)
                {
                    if (ConfigSettings["logExceptionStack"].ToLower() == "true") boolDisplayExceptionStack = true;
                }
            }

            StringBuilder strInfo = new StringBuilder();
            /*		if (AdditionalInfo != null)
					{
						// Record General information.
						strInfo.AppendFormat("{0}------- Exception Log Entry ------- {0}",Environment.NewLine);
						strInfo.AppendFormat("{0}General Information {0}",Environment.NewLine);
						strInfo.AppendFormat("{0}Additonal Info:", Environment.NewLine);
						foreach (string i in AdditionalInfo)
						{
							strInfo.AppendFormat("{0}{1}: {2}", Environment.NewLine, i,AdditionalInfo.Get(i));
						}
					}
					*/
            // Append the exception text
            if (exception != null)
            {
                strInfo.AppendFormat("{0}------- Exception Log Entry ({1}) ------- ", Environment.NewLine, System.DateTime.Now);
                strInfo.AppendFormat("{0}Exception Information : {1}",
                    Environment.NewLine, exception.Message.ToString());
                if (boolDisplayExceptionSource == true)
                {
                    strInfo.AppendFormat("{0}Source Information : {1}",
                        Environment.NewLine, exception.Source.ToString());
                }
                if (boolDisplayExceptionStack == true)
                {
                    strInfo.AppendFormat("{0}Stack Information :{0}{1}",
                        Environment.NewLine, exception.StackTrace.ToString());
                }
            }
            else
            {
                strInfo.AppendFormat("{0}{0}No Exception.{0}", Environment.NewLine);
            }
            // Write the entry to the event log.   
            strLogName = strLogName + System.DateTime.Now.ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture) + ".log";
            FileStream fs = File.Open(strLogName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(strInfo.ToString());
            sw.Close();
            fs.Close();
        }
    }

    public class MyExceptionLogger
    {
        public static void Publish(Exception exthis)
        {
            ExceptionManager.Publish(exthis);
            if (exthis.GetType().ToString() != "System.Threading.ThreadAbortException") System.Web.HttpContext.Current.Response.Redirect(ConfigurationManager.AppSettings["VirtualPath"] + "/Error.aspx", true);
           
        }
    }
}
