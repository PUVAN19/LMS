using BLL;
using Library;
using System;
using System.Data;

namespace Controls
{
    public partial class Header : System.Web.UI.UserControl
    {
        AdminBO objBO = new AdminBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserName();
                LoadSidebarMenu();
           
            }
        }

        private void LoadUserName()
        {
            if (Session["AdminUserName"] != null)
            {
                ltrUserName.Text = Session["AdminUserName"].ToString();
            }
            else
            {
                ltrUserName.Text = "User";
            }
        }

        private void LoadSidebarMenu()
        {
            try
            {
                int roleId = Convert.ToInt32(Session["AdminUserRoleID"]);
                DataSet ds = objBO.GetMenusByRole(roleId);

                if (ds != null && ds.Tables.Count >= 3)
                {
                    DataTable dtParents = ds.Tables[0];
                    DataTable dtChildren = ds.Tables[1];
                    DataTable dtSingleMenus = ds.Tables[2];
                    ltrMenu.Text =
                       GenerateMenuHtml(dtParents, dtChildren) +
                       GenerateSingleMenus(dtSingleMenus);
                    }
                else
                {
                    ltrMenu.Text = "<li>No menus assigned</li>";
                }
            }
            catch (Exception)
            {
                ltrMenu.Text = "<li>Error loading menu</li>";
            }
        }


        private string GenerateMenuHtml(DataTable dtParents, DataTable dtChildren)
        {
            string html = "";

            foreach (DataRow row in dtParents.Rows)
            {
                int menuId = Convert.ToInt32(row["MenuID"]);
                string menuName = row["MenuName"].ToString();
                string pageUrl =(row["PageName"]).ToString(); 

                //string pageUrl = row["PageName"] == DBNull.Value ? "" : row["PageName"].ToString();

                bool hasChild = dtChildren.Select("ParentMenuID=" + menuId).Length > 0;

                // ✅ CASE 1: Parent menu with child menus
                if (hasChild)
                {
                    html += $@"
<li class='sidebar-list'>
    <a class='sidebar-link sidebar-title' href='javascript:void(0)'>
        <svg class='stroke-icon'>
            <use href='../assets/svg/iconly-sprite.svg#Category'></use>
        </svg>
        <span>{menuName}</span><i class=""iconly-Arrow-Right-2 icli""></i>
    </a>";

                    html += GenerateChildMenu(dtChildren, menuId);

                    html += "</li>";
                }
                // ✅ CASE 2: Single menu (no child, but has PageName)
                else if (!string.IsNullOrWhiteSpace(pageUrl))
                {
                    html += $@"
<li class='sidebar-list'>
    <a class='sidebar-link' href='{pageUrl}'>
        <svg class='stroke-icon'>
            <use href='../assets/svg/iconly-sprite.svg#Category'></use>
        </svg>
        <span>{menuName}</span>
    </a>
</li>";
                }
                // ❌ CASE 3: No child + no PageName → DO NOT RENDER
            }

            return html;
        }


        private string GenerateSingleMenus(DataTable dtSingle)
        {
            string html = "";

            foreach (DataRow row in dtSingle.Rows)
            {
                html += $@"
<li class='sidebar-list'>
    <a class='sidebar-link' href='{(row["PageName"])}'>
        <svg class='stroke-icon'>
            <use href='../assets/svg/iconly-sprite.svg#Category'></use>
        </svg>
        <span>{row["MenuName"]}</span>
    </a>
</li>";
            }

            return html;
        }



        private string GenerateChildMenu(DataTable dtChildren, int parentId)
        {
            string submenu = "";

            DataRow[] rows = dtChildren.Select("ParentMenuID=" + parentId);

            if (rows.Length > 0)
            {
                submenu += "<ul class='sidebar-submenu'>";

                foreach (DataRow row in rows)
                {
                    submenu += $@"
<li>
    <a href='{row["PageName"]}'>
        {row["MenuName"]}
    </a>
</li>";
                }

                submenu += "</ul>";
            }

            return submenu;
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (objBO != null)
                {
                    objBO.ReleaseResources();
                    objBO = null;
                }
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
            }
        }
    }
}
