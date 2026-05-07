using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LMS
{
    public partial class ErrorView : System.Web.UI.Page
    {
        string strFileName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.HasKeys())
            {
                strFileName = Convert.ToString(Request.QueryString["Log"]);
                litFileContent.Text = GetErrorLog(strFileName);
            }
        }

        private string GetErrorLog(string strFileName)
        {
            StreamReader objStreamReader;
            string strText = "";
            if (File.Exists(Server.MapPath("ErrorLog/" + strFileName + ".log")))
            {
                objStreamReader = File.OpenText(Server.MapPath("ErrorLog/" + strFileName + ".log"));
                strText = objStreamReader.ReadToEnd();
                objStreamReader.Close();

                return strText.Replace("\n", "</br>");
            }
            else
            {
                return strText = "File Not Found";
            }
        }

        // 🔐 Restrict access (IMPORTANT)
        //    if (Session["AdminUserID"] == null)
        //    {
        //        Response.Redirect("Login.aspx");
        //        return;
        //    }

        //    if (!IsPostBack)
        //    {
        //        string fileName = Request.QueryString["Log"];

        //        if (!IsValidFileName(fileName))
        //        {
        //            lblMessage.Text = "Invalid file name.";
        //            return;
        //        }

        //        litFileContent.Text = GetErrorLog(fileName);
        //    }
        //}

        //// ✅ Allow only safe file names
        //private bool IsValidFileName(string fileName)
        //{
        //    return !string.IsNullOrEmpty(fileName) &&
        //           Regex.IsMatch(fileName, @"^[a-zA-Z0-9_-]+$");
        //}

        //private string GetErrorLog(string fileName)
        //{
        //    try
        //    {
        //        string folderPath = Server.MapPath("/ErrorLog/");
        //        string filePath = Path.Combine(folderPath, fileName + ".log");

        //        // 🔐 Prevent path traversal
        //        if (!filePath.StartsWith(folderPath))
        //        {
        //            return "Access denied.";
        //        }

        //        if (!File.Exists(filePath))
        //        {
        //            return "File not found.";
        //        }

        //        using (StreamReader reader = new StreamReader(filePath))
        //        {
        //            string content = reader.ReadToEnd();

        //            // 🔐 Prevent XSS
        //            return Server.HtmlEncode(content).Replace("\n", "<br/>");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return "Error reading file.";
        //    }
        //}
    }
    }
    
