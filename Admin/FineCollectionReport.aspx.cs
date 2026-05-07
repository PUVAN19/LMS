using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class FineCollectionReport : System.Web.UI.Page
    {
        CommonBO objCommonBO = new CommonBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCounts();
                BindGrid("UNPAID", true);
                lblGridTitle.InnerText = "UnPaid Fine Details";
            }
        }

        private void LoadCounts()
        {
            using (DataSet ds = objCommonBO.FineAmount("COUNT_UNPAID"))
            {
                lblUnpaidCount.Text =
                    (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    ? ds.Tables[0].Rows[0][0].ToString()
                    : "0";
            }

            using (DataSet ds = objCommonBO.FineAmount("COUNT_PAID"))
            {
                lblPaidCount.Text =
                    (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    ? ds.Tables[0].Rows[0][0].ToString()
                    : "0";
            }
        }

        private void BindGrid(string action, bool resetPageIndex = false)
        {
            using (DataSet ds = objCommonBO.FineAmount(action))
            {
                if (resetPageIndex)
                    gvFine.PageIndex = 0;

                if (ds != null && ds.Tables.Count > 0)
                {
                    gvFine.DataSource = ds.Tables[0];

                    if (action == "UNPAID")
                        gvFine.Columns[5].Visible = false;  
                    else
                        gvFine.Columns[5].Visible = true;

                    SetCSVHiddenColumns(action);
                }
                else
                {
                    gvFine.DataSource = null;
                }

                gvFine.DataBind();
                divGrid.Visible = true;

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > gvFine.PageSize)
                {
                    CommonFunction.BuildPager(rptPager, gvFine.PageCount, gvFine.PageIndex);
                    rptPager.Visible = true;
                }
                else
                {
                    rptPager.Visible = false;
                }
                if (ds != null && ds.Tables.Count > 0)
                    ViewState["GridData"] = ds.Tables[0];
            }
        }

        protected void CardUnpaid_Click(object sender, EventArgs e)
        {
            lblGridTitle.InnerText = "UnPaid Fine Details";
            BindGrid("UNPAID", true);
        }

        protected void CardPaid_Click(object sender, EventArgs e)
        {
            lblGridTitle.InnerText = "Paid Fine Details";
            BindGrid("PAID", true);
        }
        private void SetCSVHiddenColumns(string action)
        {
            action = action.ToUpper();

            if (action == "UNPAID")
            {
                // ❌ In UNPAID we don't want PaidDate column
                hfRemoveColumnsCSV.Value = "PaidDate";
            }
            else if (action == "PAID")
            {
                // ✅ In PAID we show everything
                hfRemoveColumnsCSV.Value = "";
            }
            else
            {
                hfRemoveColumnsCSV.Value = "";
            }
        }

        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable gridData = ViewState["GridData"] as DataTable;

                if (gridData == null || gridData.Rows.Count == 0)
                {
                    ShowToastr("No data available for export.", "warning");
                    return;
                }

                DataTable csvTable = gridData.Copy();

                if (!string.IsNullOrWhiteSpace(hfRemoveColumnsCSV.Value))
                {
                    foreach (string col in hfRemoveColumnsCSV.Value.Split(','))
                    {
                        string colName = col.Trim();
                        if (csvTable.Columns.Contains(colName))
                            csvTable.Columns.Remove(colName);
                    }
                }

                DataColumn snoCol = new DataColumn("S.No", typeof(int));
                csvTable.Columns.Add(snoCol);
                snoCol.SetOrdinal(0);

                int index = 1;
                foreach (DataRow row in csvTable.Rows)
                {
                    row["S.No"] = index++;
                }

                // ✅ Prevent Excel scientific format issue (if ISBN exists)
                if (csvTable.Columns.Contains("ISBN"))
                {
                    foreach (DataRow row in csvTable.Rows)
                        row["ISBN"] = "\t" + row["ISBN"];
                }

                // ✅ Generate CSV
                StringBuilder sb = CommonFunction.CSVFileGeneration(csvTable, "");

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition",
                    "attachment;filename=FineCollectionReport.csv");
                Response.ContentType = "text/csv";
                Response.Write(sb.ToString());

                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();

                csvTable.Dispose();
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowToastr("Failed to download CSV.", "error");
            }
        }

      


        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int newIndex = Convert.ToInt32(e.CommandArgument);

            // ✅ FIX: prevent invalid index
            if (newIndex < 0)
                newIndex = 0;

            if (newIndex >= gvFine.PageCount)
                newIndex = gvFine.PageCount - 1;

            gvFine.PageIndex = newIndex;

            string action = lblGridTitle.InnerText.Contains("UnPaid")
                ? "UNPAID"
                : "PAID";

            BindGrid(action, false);
        }

        protected void gvFine_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int newIndex = e.NewPageIndex;

            if (newIndex < 0)
                newIndex = 0;

            if (newIndex >= gvFine.PageCount)
                newIndex = gvFine.PageCount - 1;

            gvFine.PageIndex = newIndex;

            string action = lblGridTitle.InnerText.Contains("UnPaid")
                ? "UNPAID"
                : "PAID";

            BindGrid(action, false);
        }

        private void ShowToastr(string message, string alertType = "error")
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), Guid.NewGuid().ToString(),
                $"$(function(){{ AlertMessage('{message.Replace("'", "\\'")}', '{alertType.ToLower()}'); }});",
                true
            );
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (objCommonBO != null)
            {
                objCommonBO.ReleaseResources();
                objCommonBO = null;
            }
        }
    }
}