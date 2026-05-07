using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mail;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Xml;


namespace Library
{
    public class CommonFunction
    {
        public CommonFunction()
        {
            //
            // TODO: Add constructor logic here
            //

        }
        public static void LoadHash()
        {
            if (HttpContext.Current.Cache["ErrMessage"] == null)
            {
                string strFilePath = System.Web.HttpContext.Current.Server.MapPath(Convert.ToString(ConfigurationSettings.AppSettings["ErrorXmlPath"]));

                Hashtable Htable = new Hashtable();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(strFilePath);
                string strNode = "Error/Message";
                XmlNodeList xmlnodelist = xmldoc.SelectNodes(strNode);
                string strMessage = "";

                foreach (XmlNode xmlnode in xmlnodelist)
                {
                    string pstrErrCode = xmlnode.SelectSingleNode("@id").InnerText.Trim();
                    strMessage = xmlnode.InnerText.Trim();
                    Htable.Add(pstrErrCode, strMessage);
                }

                HttpContext.Current.Cache["ErrMessage"] = Htable;
            }

        }


        public static StringBuilder CSVFileGeneration(DataTable dt, string strTableHeading)
        {
            StringBuilder sbCSV = new StringBuilder();
            string strData = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                sbCSV.Append(strTableHeading);
                sbCSV.Append(Environment.NewLine);
                sbCSV.Append(Environment.NewLine);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbCSV.Append(Convert.ToString(dt.Columns[i].ColumnName) + ",");
                }
                sbCSV.Append(Environment.NewLine);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strData = Convert.ToString(dt.Rows[i][j]);
                        if (strData.Contains(","))
                            strData = "\"" + strData + "\"";
                        if (strData.Contains("\r\n"))
                            strData = strData.Replace("\r\n", " ");
                        sbCSV.Append(strData + ",");
                    }
                    sbCSV.Append(Environment.NewLine);
                }

            }
            return sbCSV;
        }

        public static StringBuilder DowloadCSVFile(DataTable dt, string strTableHeading)
        {
            StringBuilder sbCSV = new StringBuilder();

            if (dt != null && dt.Rows.Count > 0)
            {
                sbCSV.AppendLine(strTableHeading);
                sbCSV.AppendLine();

                // Header
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbCSV.Append(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                        sbCSV.Append(",");
                }
                sbCSV.AppendLine();

                // Rows
                foreach (DataRow row in dt.Rows)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string strData = Convert.ToString(row[j]);

                        if (strData.Contains(",") || strData.Contains("\""))
                            strData = "\"" + strData.Replace("\"", "\"\"") + "\"";

                        if (strData.Contains("\r\n"))
                            strData = strData.Replace("\r\n", " ");

                        sbCSV.Append(strData);

                        if (j < dt.Columns.Count - 1)
                            sbCSV.Append(",");
                    }
                    sbCSV.AppendLine();
                }
            }

            return sbCSV;
        }

        public static StringBuilder CSVFileGenerationWithoutHeader(DataTable dt, string strTableHeading)
        {
            StringBuilder sbCSV = new StringBuilder();
            string strData = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                //sbCSV.Append(strTableHeading);
                //sbCSV.Append(Environment.NewLine);
                //sbCSV.Append(Environment.NewLine);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbCSV.Append(Convert.ToString(dt.Columns[i].ColumnName) + ",");
                }
                sbCSV.Append(Environment.NewLine);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strData = Convert.ToString(dt.Rows[i][j]);
                        if (strData.Contains(","))
                            strData = "\"" + strData + "\"";
                        if (strData.Contains("\r\n"))
                            strData = strData.Replace("\r\n", " ");
                        sbCSV.Append(strData + ",");
                    }
                    sbCSV.Append(Environment.NewLine);
                }

            }
            return sbCSV;
        }
        public static void LoadHashRoleMenu()
        {
            if (HttpContext.Current.Cache["AdminErrMessage"] == null)
            {
                string strFilePath = System.Web.HttpContext.Current.Server.MapPath("..") + "\\Config\\AdminMessages.xml";
                Hashtable Htable = new Hashtable();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(strFilePath);
                string strNode = "Error/Message";
                XmlNodeList xmlnodelist = xmldoc.SelectNodes(strNode);
                string strMessage = "";

                foreach (XmlNode xmlnode in xmlnodelist)
                {
                    string pstrErrCode = xmlnode.SelectSingleNode("@id").InnerText.Trim();
                    strMessage = xmlnode.InnerText.Trim();
                    Htable.Add(pstrErrCode, strMessage);
                }
                HttpContext.Current.Cache["AdminErrMessage"] = Htable;
            }
        }

        public static string GetErrorMessage(string pstrFilePath, string pstrErrCode)
        {
            LoadHash();
            Hashtable ht = (Hashtable)HttpContext.Current.Cache["ErrMessage"];
            string str = Convert.ToString(ht[pstrErrCode]);
            return str;

        }

        public static string GetErrorMessageRoleMenu(string pstrFilePath, string pstrErrCode)
        {
            LoadHashRoleMenu();
            Hashtable ht = (Hashtable)HttpContext.Current.Cache["AdminErrMessage"];
            string str = Convert.ToString(ht[pstrErrCode]);
            return str;
        }


        //To check for valid Email
        public static bool CheckEmail(string strEmail)
        {
            Regex objEmailPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,15})+)$");
            return objEmailPattern.IsMatch(strEmail);
        }

        public static bool CheckDate(string strDate)
        {
            //Regex objEmailPattern=new Regex("\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
            Regex objEmailPattern = new Regex("(0[1-9]|1[012])[-/.](0[1-9]|[12][0-9]|3[01])[-/.](19|20)\\d\\d");
            return objEmailPattern.IsMatch(strDate);
        }

        public static string TrimString(string strToTrim, int intLength)
        {
            if (strToTrim == null) return "";
            if (strToTrim.Trim().Length > intLength) strToTrim = strToTrim.Substring(0, intLength);
            return strToTrim.Trim();
        }

        //To Checking whether any Number exists in the input
        public static bool checkAlphaNumOccurence(string FieldValue)
        {
            //Password should have atleast one alphabet and atleast one number
            bool bFlag = true;
            //Checking whether any Number exists in the input
            string NumberSet = "0123456789";
            string NumberFlag = "false";
            for (int i = 0; i < FieldValue.Length; i++)
            {
                string FieldChar = FieldValue.Substring(i, 1);
                if (NumberSet.IndexOf(FieldChar) >= 0)
                {
                    NumberFlag = "true";
                    break;
                }
            }

            //Checking whether any Alphabet exists in the input
            string AlphabetSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string AlphabetFlag = "false";
            for (int j = 0; j < FieldValue.Length; j++)
            {
                string FieldChar = FieldValue.Substring(j, 1);
                if (AlphabetSet.IndexOf(FieldChar) >= 0)
                {
                    AlphabetFlag = "true";
                    break;
                }
            }

            if (NumberFlag == "false" || AlphabetFlag == "false")
            {
                bFlag = false;
            }
            else if (NumberFlag == "true" && AlphabetFlag == "true")
            {
                bFlag = true;
            }
            return bFlag;
        }

        public static string EscapeSingleQuote(string val)
        {
            if (val == null)
                return null;
            return val.Replace("'", "''");
            //return val;
        }

        public static string GetSearchString(string val)
        {
            if (val == null)
                return null;

            return "'%" + val + "%'";
        }

        public static string FillString(char cFillChar, int iCount)
        {
            int i;
            string cStr = "";
            for (i = 1; i <= iCount; i++)
            {
                cStr += cFillChar;
            }

            return cStr;
        }
        public static string ProperCase(string s)
        {
            try
            {
                s = s.ToLower();
                s = s.Trim();
                string sProper = "";

                char[] seps = new char[] { ' ' };
                foreach (string ss in s.Split(seps))
                {
                    sProper += char.ToUpper(ss[0]);
                    sProper +=
                        (ss.Substring(1, ss.Length - 1) + ' ');
                }
                return sProper;
            }
            catch
            {
                return " ";
            }
        }

        public static string MultilineTextBox(string StrMessage)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < StrMessage.Length; i++)
            {
                if (StrMessage[i] == '\n')
                    sb.Append("<br>");
                else
                    sb.Append(StrMessage[i]);
            }
            return sb.ToString();

        }
        public static string GetMACAddress()
        {
            string MACAddress = String.Empty;
            try
            {
                ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection objMOC = objMOS.Get();

                foreach (ManagementObject objMO in objMOC)
                {
                    if (MACAddress == String.Empty) // only return MAC Address from first card   
                    {
                        MACAddress = objMO["MacAddress"].ToString();
                    }
                    objMO.Dispose();
                }
                MACAddress = MACAddress.Replace(":", "");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return MACAddress;
        }
        public static string GetMacAddress2()
        {
            System.Net.NetworkInformation.NetworkInterface[] nic = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            String macAddress = string.Empty;
            foreach (System.Net.NetworkInformation.NetworkInterface ni in nic)
            {
                System.Net.NetworkInformation.IPInterfaceProperties properties = ni.GetIPProperties();
                macAddress = ni.GetPhysicalAddress().ToString();
                if (macAddress != String.Empty)
                {
                    return macAddress;
                }
            }
            return macAddress;
        }
        public static string GetIPAddress()
        {
            string strClientIPAddress = "";
            strClientIPAddress = Convert.ToString(HttpContext.Current.Request.UserHostAddress);
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (strClientIPAddress != null && strClientIPAddress.Trim() != "" && result != null && result.Trim() != "")
            {
                if (strClientIPAddress.Trim() != result.Trim())
                    strClientIPAddress = result;
            }
            return strClientIPAddress;
        }
        public static string GetMACAddress4()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();

                }
            }
            return sMacAddress;
        }

        public static bool CheckUserName(string UserName)
        {
            //Password should contain only the characters given below
            string PwdSet = " '.-abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (allowChar(UserName, PwdSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool allowChar(string FieldValue, string Chars)
        {
            int FieldLen = FieldValue.Length;
            for (int i = 0; i < FieldLen; i++)
            {
                char FieldChar = FieldValue[i];
                if (Chars.IndexOf(FieldChar) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckNumeric(string strNumeric)
        {
            string NumericSet = "0123456789";
            if (allowChar(strNumeric, NumericSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool CheckDecimal(string strNumeric)
        {
            string NumericSet = "0123456789.";
            if (allowChar(strNumeric, NumericSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool CheckDecimalNumber(string strNumeric)
        {
            string NumericSet = "0123456789.-";
            if (allowChar(strNumeric, NumericSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool CheckPassword(string PasswordString)
        {
            string PwdSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            if (allowChar(PasswordString, PwdSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckTextValue(string TextString)
        {
            string PwdSet = "_-. abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            if (allowChar(TextString, PwdSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool CheckAlpha(string TextString)
        {
            string PwdSet = "  abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ .";
            if (allowChar(TextString, PwdSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckEmailId(string EmailId)
        {
            Regex objEmailPattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,15})+)$");
            return objEmailPattern.IsMatch(EmailId);
        }
        public static DateTime DateConvert(string date, bool IsCultureInfoApply)
        {
            CultureInfo cInfo = new CultureInfo("en-GB");
            CultureInfo usInfo = new CultureInfo("en-US");
            try
            {
                return Convert.ToDateTime(date, cInfo);
            }
            catch
            {
                return Convert.ToDateTime(date, usInfo);
            }
        }


        public static bool IsCheckBoxChecked(CheckBoxList chkSelect)
        {
            bool chkFlag = false;
            for (int i = 0; i < chkSelect.Items.Count; i++)
            {
                if (chkSelect.Items[i].Selected)
                    chkFlag = true;
            }
            return chkFlag;
        }


        /// <summary>
        /// IsInteger
        /// </summary>
        /// <param name="objNumber">Number</param>
        /// <returns>bool</returns>
        public static bool IsInteger(object objNumber)
        {
            //if(Convert.ToString(objNumber).Trim().Length == 0)return false;
            try
            {
                if (Convert.ToInt64(objNumber, CultureInfo.InvariantCulture) > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check valid date
        /// </summary>
        /// <param name="strDate">strDate</param>
        /// <returns>bool</returns>
        public static bool CheckValidDate(string strDate)
        {
            try
            {
                if (!Regex.IsMatch(strDate, @"^(\d)+-(\w){3}-(\d){4}$", RegexOptions.IgnoreCase))
                {
                    return false;
                }

                DateTime dtFormat = Convert.ToDateTime(strDate, CultureInfo.InvariantCulture);
                string strDateCheck = dtFormat.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                /*
                int strYearCheck = System.Convert.ToInt32(strDateCheck.Substring(strDateCheck.LastIndexOf('-')+1,4));
                if(strYearCheck <  1900)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                */
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckValidDateTime(string strDate)
        {
            try
            {
                if (!Regex.IsMatch(strDate, @"^(\d)+-(\w){3}-(\d){4} (\d){2}:(\d){2}$", RegexOptions.IgnoreCase))
                {
                    return false;
                }

                DateTime dtFormat = Convert.ToDateTime(strDate, CultureInfo.InvariantCulture);
                string strDateCheck = dtFormat.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                /*
                int strYearCheck = System.Convert.ToInt32(strDateCheck.Substring(strDateCheck.LastIndexOf('-')+1,4));
                if(strYearCheck <  1900)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                */
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string KillChars(string pstrText)
        {
            string strValidText = "";
            string[] arrKillChars = new string[7] { "select", "drop", ";", "--", "insert", "delete", "xp_" };

            try
            {
                strValidText = pstrText;
                for (int intI = 0; intI < arrKillChars.Length; intI++)
                {
                    strValidText = strValidText.Replace(arrKillChars[intI], "");
                }

                strValidText = strValidText.Replace("'", "''");

            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
            return strValidText;
        }

        public static double DateDiff(DateTime sDt, DateTime eDt)
        {
            double i = 0;
            while (DateTime.Compare(sDt, eDt) <= 0)
            {
                i++;
                sDt = sDt.AddDays(1);
            }
            return i;
        }

        public static int CompareDates(DateTime sDt, DateTime eDt)
        {
            int i = 0;
            i = DateTime.Compare(sDt, eDt);
            return i;
        }
        /// <summary>
        /// Sends a mail in HTML format
        /// </summary>
        /// <param name="pstrFrom">From EmailId</param>
        /// <param name="pstrTo">To EmailId</param>
        /// <param name="pstrSubject">Subject of the Email</param>
        /// <param name="pstrBody">Body of the Email</param>
        public static void SendMail(string pstrFrom, string pstrTo, string pstrSubject, string pstrBody)
        {
            try
            {
                MailMessage objMail = new MailMessage();
                objMail.From = pstrFrom;
                objMail.To = pstrTo;
                objMail.Subject = pstrSubject;
                objMail.Body = pstrBody;
                objMail.BodyFormat = MailFormat.Html;
                SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["EmailServer"];
                SmtpMail.Send(objMail);
            }
            catch { }
        }

        public static string GetPanelMenuFileName(int intIndex, string filestring)
        {
            string strVirPath = ConfigurationSettings.AppSettings["VirtualPath"];
            string strTmp = filestring.Substring(0, intIndex);
            string strLastFileName = filestring.Substring(intIndex); //,filestring.Length);
            if (strTmp.ToUpper() == "COMMON")
            {
                filestring = strVirPath + "/" + filestring;
            }
            else if (strTmp.ToUpper() == "REQCRE")
            {
                filestring = strVirPath + "/Employee/RequirementCreator" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "PROFILE")
            {
                filestring = strVirPath + "/Profile" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "RECOM")
            {
                filestring = strVirPath + "/Employee/RequirementRecommendor" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "APPR")
            {
                filestring = strVirPath + "/Employee/RequirementApprover" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "HAPPR")
            {
                filestring = strVirPath + "/Employee/HireApprover" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "RECRU")
            {
                filestring = strVirPath + "/Employee/Recruiter" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "IVPANEL")
            {
                filestring = strVirPath + "/Employee/InterviewPanelist" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "HR")
            {
                filestring = strVirPath + "/Employee/HR" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "ADMIN")
            {
                filestring = strVirPath + "/Admin" + strLastFileName;
            }
            else if (strTmp.ToUpper() == "EMPRE")
            {
                filestring = strVirPath + "/EmpReferral" + strLastFileName;
            }

            return filestring;

        }

        public static int GetValidPageIndex(int currentPageIndex, int rowcount, int pagesize)
        {

            if (currentPageIndex < 1)
                return 0;

            int limit = (int)Math.Ceiling((double)rowcount / pagesize) - 1;



            if (currentPageIndex > limit)
                return limit;

            return currentPageIndex;


        }

        public static void CheckForcefulSession()
        {

            if (HttpContext.Current.Request.ServerVariables["HTTP_REFERER"] == null && Convert.ToString(ConfigurationSettings.AppSettings["IsForcefullSession"]) == "1")
            {
                HttpSessionState session = HttpContext.Current.Session;

                string strUser = Convert.ToString(session["USERID"]);
                HttpContext.Current.Cache.Remove(strUser);

                session.Clear();
                session.Abandon();
                session = null;

                System.Web.Security.FormsAuthentication.SignOut();

                HttpContext.Current.Response.Redirect("~/ForcefulBrowsing.html", true);
            }

        }

        /// <summary>
        /// Checks for the allowable characters.
        /// returns false if pstrFieldValue does not exists in pstrChars and vice versa
        /// </summary>
        /// <param name="pstrFieldValue">String to be checked</param>
        /// <param name="pstrChars">Characters to be disallowed</param>
        /// <returns>bool</returns>
        public static bool CheckAllowChars(string pstrFieldValue, string pstrChars)
        {
            int intFieldLen = pstrFieldValue.Length;

            for (int i = 0; i < intFieldLen; i++)
            {
                string strFieldChar = pstrFieldValue.Substring(i, 1);
                if (pstrChars.IndexOf(strFieldChar) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetFormatOutput(params string[] strParam)
        {
            if (strParam.Length == 0) return "";
            StringBuilder sb = new StringBuilder();
            string strTmp = "";
            foreach (string strValue in strParam)
            {
                strTmp = strValue.Replace(" ", "&nbsp;");
                sb.Append(strTmp);
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        // Function to Check for AlphaNumeric.
        public static bool IsAlphaNumeric(string strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9 \\s]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }

        public static bool CheckAlphaNumPunctuations(string field, string punctuations)
        {

            string alphaNumPunct = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" + punctuations;

            return CommonFunction.CheckAllowChars(field, alphaNumPunct);
        }
        public static string RemoveHTMLTags(string input)
        {
            input = input.Replace("&lt;", "<");
            input = input.Replace("&gt;", ">");
            input = input.Replace("&nbsp;", " ");
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static bool LeapYear(int intYear)
        {
            try
            {
                if (intYear % 100 == 0)
                {
                    if (intYear % 400 == 0) { return true; }
                }
                else
                {
                    if ((intYear % 4) == 0) { return true; }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string SendSMS(string MobileNumber, string strMessage)
        {
            string strStatus = string.Empty;

            try
            {
                string strMobileNumber = MobileNumber;
                string FullUri = string.Empty;
                FullUri = GetAppSettings("SecondarySmsUrl") + "?username=" + GetAppSettings("SecondarySmsUsername") + "&password=" + GetAppSettings("SecondarySmsPassword") + "&to=" + strMobileNumber + "&from=" + GetAppSettings("SecondarySmsFrom") + "&message=" + strMessage;
                string StrMessageId = GetPageContent(FullUri);
                FullUri = GetAppSettings("SecondarySmsStatusUrl") + "?username=" + GetAppSettings("SecondarySmsUsername") + "&password=" + GetAppSettings("SecondarySmsPassword") + "&msgid=" + StrMessageId;
                string StrFullStatus = GetPageContent(FullUri);

                if (StrFullStatus.IndexOf("-") > -1)
                {
                    strStatus = StrFullStatus.Split('-')[1];
                }
                if (strStatus == "Delivered;" || strStatus == "Submitted;")
                {
                    strStatus = "Message Send";
                    // SaveSMSHistory(strStudentid, strMobileNumber, strOTP, strMessage, strStatus, 1);
                }
                else
                {
                    strStatus = "Message Failed";
                    //SaveSMSHistory(strStudentid, strMobileNumber, strOTP, strMessage, strStatus, 0);
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
            return strStatus;


        }
        public static string GetUserBrowserType()
        {
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string strBrowserType = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + "Name = " + browser.Browser + "\n"
                + "Version = " + browser.Version + "\n"
                + "Major Version = " + browser.MajorVersion + "\n"
                + "Minor Version = " + browser.MinorVersion + "\n"
                + "Platform = " + browser.Platform + "\n"
                + "Is Beta = " + browser.Beta + "\n"
                + "Is Crawler = " + browser.Crawler + "\n"
                + "Is AOL = " + browser.AOL + "\n"
                + "Is Win16 = " + browser.Win16 + "\n"
                + "Is Win32 = " + browser.Win32 + "\n"
                + "Supports Frames = " + browser.Frames + "\n"
                + "Supports Tables = " + browser.Tables + "\n"
                + "Supports Cookies = " + browser.Cookies + "\n"
                + "Supports VBScript = " + browser.VBScript + "\n"
                + "Supports JavaScript = " +
                    browser.EcmaScriptVersion.ToString() + "\n"
                + "Supports Java Applets = " + browser.JavaApplets + "\n"
                + "Supports ActiveX Controls = " + browser.ActiveXControls
                      + "\n"
                + "Supports JavaScript Version = " +
                    browser["JavaScriptVersion"] + "\n";

            strBrowserType = TrimString(strBrowserType, 8000);
            return strBrowserType;
        }
        private static string GetPageContent(string FullUri)
        {
            HttpWebRequest Request;
            StreamReader ResponseReader;
            Request = ((HttpWebRequest)(WebRequest.Create(FullUri)));
            ResponseReader = new StreamReader(Request.GetResponse().GetResponseStream());
            return ResponseReader.ReadToEnd();
        }

        [Obsolete]
        public static string GetAppSettings(string Context)
        {
            return Convert.ToString(ConfigurationSettings.AppSettings[Context]);
        }

        public static int CheckBirthDay(string strDate)
        {
            try
            {

                DateTime dtInpDate = Convert.ToDateTime(strDate, CultureInfo.InvariantCulture);
                DateTime dtCurDate = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);

                int intTotYrs = (dtCurDate.Year - 1) - dtInpDate.Year;

                int intIntYrDays;
                int intCurYrDays;

                if (LeapYear(dtInpDate.Year) && dtInpDate.DayOfYear >= 60)
                    intIntYrDays = dtInpDate.DayOfYear - 1;
                else
                    intIntYrDays = dtInpDate.DayOfYear;

                if (LeapYear(dtCurDate.Year) && dtCurDate.DayOfYear >= 60)
                    intCurYrDays = dtCurDate.DayOfYear - 1;
                else
                    intCurYrDays = dtCurDate.DayOfYear;

                if (intCurYrDays >= intIntYrDays)
                    intTotYrs = intTotYrs + 1;

                if (intTotYrs < 18)
                {
                    return 1;
                }
                else if (intTotYrs > 100)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }

            }
            catch
            {
                return 0;
            }
        }
        public static bool CheckAlphaNumeric(string UserName)
        {
            //Password should contain only the characters given below
            string PwdSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (allowChar(UserName, PwdSet) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static string GetUniqueKey(int maxSize, string strPasswordChar)
        {
            char[] chars = new char[62];
            chars = strPasswordChar.ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static bool SendMail(string strFromMailId, string strToMailId, string strSubject, string strTemplate, string strCCMailId = "")
        {
            SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["Emaillist_SMTPSERVER"];
            MailMessage objMailMessage = new MailMessage();
            objMailMessage.From = strFromMailId;
            objMailMessage.To = strToMailId;
            if (strCCMailId.Trim() != "") { objMailMessage.Cc = strCCMailId; }
            objMailMessage.Subject = strSubject;
            objMailMessage.Body = strTemplate;
            objMailMessage.BodyFormat = MailFormat.Html;

            try
            {
                SmtpMail.Send(objMailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInternalUser(string userIp)
        {
            string allowedIps = GetAppSettings("InternalIpAddress");

            if (string.IsNullOrEmpty(allowedIps))
                return false;

            string[] ipList = allowedIps.Split(',');

            foreach (string ip in ipList)
            {
                if (userIp.Trim() == ip.Trim())
                    return true;
            }

            return false;
        }



        public static bool IsValidEdition(string edition)
        {
            var match = Regex.Match(edition ?? "", @"^(\d+)(st|nd|rd|th)$");
            if (!match.Success) return false;

            int n = int.Parse(match.Groups[1].Value);
            string expected = (n % 100 >= 11 && n % 100 <= 13) ? "th" :
                              (n % 10 == 1) ? "st" :
                              (n % 10 == 2) ? "nd" :
                              (n % 10 == 3) ? "rd" : "th";

            return match.Groups[2].Value == expected;
        }

        public static void BuildPager(Repeater rptPager,int totalPages, int currentPage)
        {
            var pages = new List<object>();
            int maxPagesToShow = 3;

            int startPage = Math.Max(0, currentPage - 1);
            int endPage = Math.Min(totalPages - 1, startPage + maxPagesToShow - 1);

            if (endPage - startPage < maxPagesToShow - 1)
                startPage = Math.Max(0, endPage - maxPagesToShow + 1);

            // Previous
            // 🔹 First
            pages.Add(new
            {
                PageIndex = 0,
                Text = "« First",
                Command = "Page",
                Enabled = currentPage > 0,
                IsActive = false
            });

            // 🔹 Previous
            pages.Add(new
            {
                PageIndex = currentPage - 1,
                Text = "‹ Prev",
                Command = "Page",
                Enabled = currentPage > 0,
                IsActive = false
            });

            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(new
                {
                    PageIndex = i,
                    Text = (i + 1).ToString(),
                    Command = "Page",
                    Enabled = true,
                    IsActive = (i == currentPage)
                });
            }

            // 🔹 Next
            pages.Add(new
            {
                PageIndex = currentPage + 1,
                Text = "Next ›",
                Command = "Page",
                Enabled = currentPage < totalPages - 1,
                IsActive = false
            });

            // 🔹 Last
            pages.Add(new
            {
                PageIndex = totalPages - 1,
                Text = "Last »",
                Command = "Page",
                Enabled = currentPage < totalPages - 1,
                IsActive = false
            });
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }
    }
}
