using Library;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminDAO
    {
        private SqlConnection objSqlConnection;

        public AdminDAO()
        {
            objSqlConnection = new SqlConnection(Security.CryptTripleDES(false, ConfigurationManager.AppSettings["ConnectionString"].ToString()));
        }
        public void ReleaseResources()
        {
            if (objSqlConnection.State != ConnectionState.Closed)
                objSqlConnection.Close();
            objSqlConnection.Dispose();
        }
        public DataSet RoleMasterCRUD(
           string action,
           int roleID = 0,
           string roleName = "",
           string DefaultPage = "",
           bool active = true,
           int adminUserID = 0
       )
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@RoleID", roleID),
                new SqlParameter("@RoleName", roleName),
                new SqlParameter("@DefaultPage", DefaultPage),
                new SqlParameter("@Active", active),
                new SqlParameter("@AdminUserID", adminUserID)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "RoleMaster_CRUD",
                param
            );
        }

        public DataSet UserMaster(string Action, int UserID, string LoginId, string EmpCode, string UserName,
      string Gender, string Email, string AltEmail, string Phone, string AltPhone, string Department,
      string Designation, int RoleType, string Password, bool Active, int AdminUserID, string searchBy,
      string searchValue)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", Action),
        new SqlParameter("@UserID", UserID),
        new SqlParameter("@LoginId", (object)LoginId ?? DBNull.Value),
        new SqlParameter("@EmpCode", (object)EmpCode ?? DBNull.Value),
        new SqlParameter("@UserName", (object)UserName ?? DBNull.Value),
        new SqlParameter("@Gender", (object)Gender ?? DBNull.Value),
        new SqlParameter("@Email", (object)Email ?? DBNull.Value),
        new SqlParameter("@AltEmail", (object)AltEmail ?? DBNull.Value),
        new SqlParameter("@Phone", (object)Phone ?? DBNull.Value),
        new SqlParameter("@AltPhone", (object)AltPhone ?? DBNull.Value),
        new SqlParameter("@Department", (object)Department ?? DBNull.Value),
        new SqlParameter("@Designation", (object)Designation ?? DBNull.Value),
        new SqlParameter("@RoleType", RoleType),
        new SqlParameter("@Password", (object)Password ?? DBNull.Value),
        new SqlParameter("@Active", Active),
        new SqlParameter("@AdminUserID", AdminUserID),
        new SqlParameter("@SearchBy", (object)searchBy ?? DBNull.Value),
        new SqlParameter("@SearchValue", (object)searchValue ?? DBNull.Value)

            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "dbo.UserMaster",
                objParam
            );
        }


        public DataSet RoleType(string action, int roleId = 0, string strRoleName = "",
                          bool active = true)
        {
            SqlParameter[] objparam = new SqlParameter[]
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@RoleID", roleId),
                new SqlParameter("@RoleName", (object)strRoleName ?? DBNull.Value),

                new SqlParameter("@Active", active),

            };

            return SqlHelper.ExecuteDataset(objSqlConnection,
                    CommandType.StoredProcedure, "dbo.RoleMaster_CRUD", objparam);
        }
        public DataSet PageName(string action)
        {
            SqlParameter[] objparam = new SqlParameter[]
            {
                new SqlParameter("@Action", action),



            };

            return SqlHelper.ExecuteDataset(objSqlConnection,
                    CommandType.StoredProcedure, "sp_MenuMaster", objparam);
        }
        public DataSet GetRolesForDropdown()
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", "SELECT")
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "dbo.RoleMaster_CRUD",
                objParam
            );
        }


        public DataSet GetBookDues(
             string action,
             string memberType = null,
             string memberID = null,
             int? issueId = null,
             DateTime? dueDate = null,
             DateTime? returnDate = null,
             int adminUserID = 0
         )
        {
            SqlParameter[] prms = new SqlParameter[]
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@MemberType", (object)memberType ?? DBNull.Value),
                new SqlParameter("@MemberID", (object)memberID ?? DBNull.Value),
                new SqlParameter("@IssueID", (object)issueId ?? DBNull.Value),
                new SqlParameter("@DueDate", (object)dueDate ?? DBNull.Value),
                new SqlParameter("@ReturnDate", (object)returnDate ?? DBNull.Value),
                new SqlParameter("@AdminUserID", adminUserID)
            };

            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure, "sp_GetBookdues", prms);
        }
        public DataSet GetRoleType()
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Action", "GETROLE")
            };

            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure,
                "sp_RoleMenuMapping", p);
        }

        public DataSet SearchRoleMenu(int RoleID)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@Action", "GET"),
            new SqlParameter("@RoleID", RoleID)
            };

            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure,
                "sp_RoleMenuMapping", p);
        }

        // SAVE UPDATE
        public DataSet SaveUpdateRoleMenu(int RoleID, int MenuID, int SequenceNo, int IsChecked, int AUserID)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Action", "SAVEUPDATE"),
                new SqlParameter("@RoleID", RoleID),
                new SqlParameter("@MenuID", MenuID),
                new SqlParameter("@SequenceNo", SequenceNo),
                new SqlParameter("@IsChecked", IsChecked),
                new SqlParameter("@AUserID", AUserID)
            };

            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure,
                "sp_RoleMenuMapping", p);
        }

        public DataSet MenuMaster(
         string action,
         int? menuID = null,
         string menuName = "",
         string pageName = "",
         int? parentMenuID = null,
         bool? isChildMenu = null,
         bool? IsDefaultPage = null,
         int? sequenceNo = null,
         bool isActive = true,
         int adminUserID = 0,
         string searchBy = "",
         string searchValue = "")
        {
            SqlParameter[] objparam = new SqlParameter[]
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@MenuID", (object)menuID ?? DBNull.Value),
                new SqlParameter("@MenuName", (object)menuName ?? DBNull.Value),
                new SqlParameter("@PageName", (object)pageName ?? DBNull.Value),
                new SqlParameter("@ParentMenuID", (object)parentMenuID ?? DBNull.Value),
                new SqlParameter("@IsChildMenu", (object)isChildMenu ?? DBNull.Value),
                new SqlParameter("@IsDefaultPage", (object)IsDefaultPage ?? DBNull.Value),
                new SqlParameter("@SequenceNo", (object)sequenceNo ?? DBNull.Value),
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@AdminUserID", adminUserID),
                new SqlParameter("@SearchBy", (object)searchBy ?? DBNull.Value),
                new SqlParameter("@SearchValue", (object)searchValue ?? DBNull.Value)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "dbo.sp_MenuMaster",
                objparam
            );
        }

        public DataSet GetMenusByRole(int RoleID)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@RoleID", RoleID)
            };

            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure, "sp_GetMenusByRole", param);
        }


        public DataSet GetDashboardData(DateTime fromDate, DateTime toDate)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@FromDate", SqlDbType.Date)
                {
                    Value = fromDate.Date
                },
                new SqlParameter("@ToDate", SqlDbType.Date)
                {
                    Value = toDate.Date
                }
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "Dashboard",
                parameters
            );
        }

        /*=========================================
          DASHBOARD GRID (MODAL + DOWNLOAD)
        =========================================*/
        public DataSet GetDashboardGrid(string actionType, DateTime fromDate, DateTime toDate)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@ActionType", SqlDbType.VarChar, 20) { Value = actionType },
        new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
        new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDate.Date }
    };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "DashboardGrid",
                parameters
            );
        }

    }
}