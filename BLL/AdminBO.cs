using DAL;
using System;
using System.Data;
namespace BLL
{
    public class AdminBO
    {
        private AdminDAO objAdminDAO;

        public AdminBO()
        {
            objAdminDAO = new AdminDAO();
        }
        public void ReleaseResources()
        {
            objAdminDAO.ReleaseResources();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        public DataSet GetRoleMaster(
          string action,
          int roleID = 0,
          string roleName = "",
          string DefaultPage = "",
          bool active = true,
          int adminUserID = 0
      )
        {
            return objAdminDAO.RoleMasterCRUD(action, roleID, roleName, DefaultPage, active, adminUserID);
        }

        public DataSet UserMaster(string Action, int UserID = 0, string LoginId = "", string EmpCode = "",
          string UserName = "", string Gender = "", string Email = "", string AltEmail = "",
          string Phone = "", string AltPhone = "", string Department = "", string Designation = "",
          int RoleType = 0, string Password = "", bool Active = true, int AdminUserID = 0, string searchBy = "",
          string searchValue = "")

        {
            return objAdminDAO.UserMaster(Action, UserID, LoginId, EmpCode, UserName, Gender,
                Email, AltEmail, Phone, AltPhone, Department, Designation, RoleType, Password,
                Active, AdminUserID, searchBy, searchValue
            );
        }

        public DataSet GetRolesForDropdown()
        {
            return objAdminDAO.GetRolesForDropdown();
        }

        public DataSet RoleType(string action, int roleId = 0, string strRoleName = "",
                          bool active = true)
        {
            return objAdminDAO.RoleType(action, roleId, strRoleName, active);
        }
        public DataSet PageName(string action)
        {
            return objAdminDAO.PageName(action);
        }

        public DataSet BookDueMaster(
            string action,
            string memberType = null,
            string memberID = null,
            int? issueId = null,
            DateTime? dueDate = null,
            DateTime? returnDate = null,
            int adminUserID = 0
        )
        {
            return objAdminDAO.GetBookDues(action, memberType, memberID, issueId, dueDate, returnDate, adminUserID);
        }

        // SELECT_USER
        public DataSet GetBookDues(string memberType, string memberID)
        {
            return BookDueMaster(
                "SELECT_USER",
                memberType,
                memberID
            );
        }

        public DataSet MarkBookReturned(int issueId,DateTime returnDate,int adminUserID)
        {
            return BookDueMaster(
                "UPDATE_RETURN",
                null,
                null,
                issueId,
                null,             // DueDate = null
                returnDate,     // ReturnDate
                adminUserID
            );
        }

        // UPDATE_RENEW
        public DataSet RenewBook(int issueId, DateTime dueDate, int adminUserID)
        {
            return BookDueMaster(
                "UPDATE_RENEW",
                null,
                null,
                issueId,
                dueDate,          // DueDate
                null,             // ReturnDate = null
                adminUserID
            );
        }
        

        // ROLE TYPES
        //public DataSet GetRoleType()
        //{
        //    return objAdminDAO.GetRoleType();
        //}

        // ROLE MENU SEARCH
        public DataSet SearchRoleMenu(int RoleID)
        {
            return objAdminDAO.SearchRoleMenu(RoleID);
        }

        // CHECKBOX SAVE
        public DataSet SaveUpdateRoleMenu(int RoleID, int MenuID, int SequenceNo, int IsChecked, int AUserID)
        {
            return objAdminDAO.SaveUpdateRoleMenu(RoleID, MenuID, SequenceNo, IsChecked, AUserID);
        }



        public DataSet MenuMaster(
       string action,
       int menuID = 0,
       string menuName = "",
       string pageName = "",
       int parentMenuID = 0,
       bool isChildMenu = false,
       bool IsDefaultPage= false,
       int sequenceNo = 0,
       bool isActive = true,
       int adminUserID = 0,
       string searchBy = "",
       string searchValue = ""
       )
        {
            return objAdminDAO.MenuMaster(
                action,
                menuID,
                menuName,
                pageName,
                parentMenuID,
                isChildMenu,
                IsDefaultPage,
                sequenceNo,
                isActive,
                adminUserID,
                searchBy,
                searchValue
            );
        }
        // LOAD GRID
        public DataSet GetMenuMasterGrid()
        {
            return MenuMaster("SELECT");
        }

        // SEARCH GRID
        public DataSet SearchMenuMaster(string searchBy, string searchValue)
        {
            return MenuMaster(
                "SEARCH",
                searchBy: searchBy,
                searchValue: searchValue
            );
        }

        // GET BY ID
        public DataSet GetMenuByID(int menuID)
        {
            return MenuMaster("SELECT_BY_ID", menuID);
        }

        // GRID
        //public DataTable GetMenuMasterGrid()
        //{
        //    DataSet ds = objAdminDAO.MenuMaster("SELECT");
        //    return ds.Tables[0];
        //}

        //public DataTable SearchMenuMaster(string searchBy, string searchValue)
        //{
        //    DataSet ds = objAdminDAO.MenuMaster("SEARCH", searchBy: searchBy, searchValue: searchValue);
        //    return ds.Tables[0];
        //}
        //public DataTable GetMenuByID(int menuId)
        //{
        //    DataSet ds = objAdminDAO.MenuMaster("SELECT_BY_ID", menuId);
        //    return ds.Tables[0];
        //}

        public DataSet GetMenusByRole(int RoleID)
        {
            return objAdminDAO.GetMenusByRole(RoleID);
        }

        public DataSet GetDashboardData(DateTime fromDate, DateTime toDate)
        {
            return objAdminDAO.GetDashboardData(fromDate, toDate);
        }

        public DataSet GetDashboardGrid(string actionType, DateTime fromDate, DateTime toDate)
        {
            return objAdminDAO.GetDashboardGrid(actionType, fromDate, toDate);
        }

    }
}

