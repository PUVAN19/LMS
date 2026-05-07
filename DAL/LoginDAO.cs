using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Library;
using Microsoft.ApplicationBlocks.Data;
using System.Security.Cryptography;

namespace DAL
{
    public class LoginDAO
    {
        private SqlConnection objSqlConnection;

        public LoginDAO()
        {
            objSqlConnection = new SqlConnection(Security.CryptTripleDES(false, ConfigurationManager.AppSettings["ConnectionString"].ToString()));
        }

        public void ReleaseResources()
        {
            if (objSqlConnection.State != ConnectionState.Closed)
                objSqlConnection.Close();
            objSqlConnection.Dispose();
        }
        public DataSet CheckLogin(int userType, string username, string Password)
        {
            SqlParameter[] objSqlParam = new SqlParameter[3];
            objSqlParam[0] = new SqlParameter("@userType", userType);
            objSqlParam[1] = new SqlParameter("@LoginID", SqlDbType.VarChar, 200);
            objSqlParam[1].Value = username;
            objSqlParam[2] = new SqlParameter("@Password", Password);
            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure, "ValidateLogin", objSqlParam);
            //string query = $"SELECT COUNT(*) TotalCount FROM UserMaster WHERE username='{username}' AND password='{Password}'";
            //return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.Text, query);
        }
        public DataSet ChangePassword(int userId, string oldPwd, string newPwd)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@OldPassword", oldPwd),
                new SqlParameter("@NewPassword", newPwd)
            };

            return SqlHelper.ExecuteDataset(
                        objSqlConnection,
                        CommandType.StoredProcedure,
                        "sp_ChangePassword",
                        param);
        }


    }
}
