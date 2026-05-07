
using BLL;
using Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        AdminBO objAdminBO = new AdminBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFromDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadDashboard();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        //LOAD DASHBOARD

        private void LoadDashboard()
        {
            ClearChartValues();
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(txtFromDate.Text, out fromDate)) return;
            if (!DateTime.TryParse(txtToDate.Text, out toDate)) return;

            using (DataSet ds = objAdminBO.GetDashboardData(fromDate, toDate))
            {
                if (ds == null || ds.Tables.Count < 8) return;

                /* ── SUMMARY CARDS ── */
                lblTotalBooks.Text = ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["TotalBooks"].ToString() : "0";
                lblTotalIssued.Text = ds.Tables[1].Rows.Count > 0 ? ds.Tables[1].Rows[0]["TotalIssued"].ToString() : "0";
                lblDueBooks.Text = ds.Tables[1].Rows.Count > 0 ? ds.Tables[1].Rows[0]["DueBooks"].ToString() : "0";
                lblReturnedBooks.Text = ds.Tables[1].Rows.Count > 0 ? ds.Tables[1].Rows[0]["ReturnedBooks"].ToString() : "0";

                /* ── BAR CHART ── */
                int studentIssued = 0, studentReturned = 0;
                int staffIssued = 0, staffReturned = 0;

                if (ds.Tables.Count > 2)
                {
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        bool isStudent = row["IssueType"].ToString().Equals("Student", StringComparison.OrdinalIgnoreCase);
                        bool isIssued = row["Status"].ToString().Equals("Issued", StringComparison.OrdinalIgnoreCase);
                        int total = row["Total"] != DBNull.Value ? Convert.ToInt32(row["Total"]) : 0;

                        if (isStudent)
                        {
                            if (isIssued) studentIssued = total;
                            else studentReturned = total;
                        }
                        else
                        {
                            if (isIssued) staffIssued = total;
                            else staffReturned = total;
                        }
                    }
                }

                hfStudentIssued.Value = studentIssued.ToString();
                hfStudentReturned.Value = studentReturned.ToString();
                hfStaffIssued.Value = staffIssued.ToString();
                hfStaffReturned.Value = staffReturned.ToString();

                /* ── DAY-WISE CHART ── */
                if (ds.Tables.Count > 3)
                    BuildDayWiseChart(ds.Tables[3], fromDate, toDate);

                /* ── CATEGORY ── */
                if (ds.Tables.Count > 4)
                    BuildCategoryChart(ds.Tables[4]);

                /* ── FINE TREND ── */
                if (ds.Tables.Count > 5)
                    BuildFineTrendChart(ds.Tables[5], fromDate, toDate);

                /* ── PIE CHART ── */
                if (ds.Tables.Count > 6)
                    BuildPieChart(ds.Tables[6]);

                if (ds.Tables.Count > 7)
                    BuildDoughnutChart(ds.Tables[7]);
            }
        }
        /* ── Day-wise stacked bar (unchanged logic) ── */
        private void BuildDayWiseChart(DataTable dtDayWise, DateTime fromDate, DateTime toDate)
        {
            int totalDays = (toDate - fromDate).Days + 1;
            int barCount = totalDays <= 7 ? totalDays : 6;
            int daysPerBar = (int)Math.Ceiling((double)totalDays / barCount);

            var labels = new List<string>();
            var issuedList = new List<int>();
            var returnedList = new List<int>();

            DateTime current = fromDate;
            while (current <= toDate)
            {
                DateTime end = current.AddDays(daysPerBar - 1);
                if (end > toDate) end = toDate;

                labels.Add($"{current:dd MMM} - {end:dd MMM}");

                int issued = dtDayWise.AsEnumerable()
                    .Where(r => r.Field<string>("Status") == "Issued"
                             && r.Field<DateTime>("ActionDate") >= current
                             && r.Field<DateTime>("ActionDate") <= end)
                    .Sum(r => r.Field<int>("Total"));

                int returned = dtDayWise.AsEnumerable()
                    .Where(r => r.Field<string>("Status") == "Returned"
                             && r.Field<DateTime>("ActionDate") >= current
                             && r.Field<DateTime>("ActionDate") <= end)
                    .Sum(r => r.Field<int>("Total"));

                issuedList.Add(issued);
                returnedList.Add(returned);
                current = end.AddDays(1);
            }

            hfDayLabels.Value = string.Join(",", labels);
            hfDayIssued.Value = string.Join(",", issuedList);
            hfDayReturned.Value = string.Join(",", returnedList);
        }

        private void BuildCategoryChart(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                hfCategoryLabels.Value = "";
                hfCategoryValues.Value = "";
                return;
            }

                
                // Take top 8 categories sorted by count descending
                var rows = dt.AsEnumerable()
                         .OrderByDescending(r => Convert.ToInt32(r["BorrowCount"]))
                         .Take(8)
                         .ToList();

            hfCategoryLabels.Value = string.Join(",", rows.Select(r => r["CategoryName"].ToString()));
            hfCategoryValues.Value = string.Join(",", rows.Select(r => r["BorrowCount"].ToString()));
        }

        private void BuildFineTrendChart(DataTable dt, DateTime fromDate, DateTime toDate)
        {
            if (dt == null)
            {
                hfFineValues.Value = "";
                hfFineLabels.Value = "";
                return;
            }
            int totalDays = (toDate - fromDate).Days + 1;
            int barCount = totalDays <= 7 ? totalDays : 6;
            int daysPerBar = (int)Math.Ceiling((double)totalDays / barCount);

            var labels = new List<string>();
            var values = new List<decimal>();
            decimal grandTotal = 0;

            DateTime current = fromDate;
            while (current <= toDate)
            {
                DateTime end = current.AddDays(daysPerBar - 1);
                if (end > toDate) end = toDate;

                labels.Add($"{current:dd MMM}");

                decimal fineSum = dt.AsEnumerable()
                    .Where(r => r.Field<DateTime>("ActionDate") >= current
                             && r.Field<DateTime>("ActionDate") <= end)
                    .Sum(r => r.Field<decimal>("FineAmount"));

                values.Add(fineSum);
                grandTotal += fineSum;
                current = end.AddDays(1);
            }

            hfFineLabels.Value = string.Join(",", labels);
            hfFineValues.Value = string.Join(",", values.Select(v => v.ToString("F0")));
            lblTotalFine.Text = grandTotal.ToString("N0");
        }

        private void BuildPieChart(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                hfPieLabels.Value = "";
                hfPieValues.Value = "";
                return;
            }
            

            hfPieLabels.Value = string.Join(",", dt.AsEnumerable()
                .Select(r => r["Status"].ToString()));

            hfPieValues.Value = string.Join(",", dt.AsEnumerable()
                .Select(r => r["Total"].ToString()));
        }

        private void BuildDoughnutChart(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                hfDoughnutLabels.Value = "";
                hfDoughnutValues.Value = "";
                return;
            }


            hfDoughnutLabels.Value = string.Join(",", dt.AsEnumerable()
                .Select(r => r["ReturnStatus"].ToString()));

            hfDoughnutValues.Value = string.Join(",", dt.AsEnumerable()
                .Select(r => r["Total"].ToString()));
        }

        private void ClearChartValues()
        {
            hfStudentIssued.Value = "0";
            hfStudentReturned.Value = "0";
            hfStaffIssued.Value = "0";
            hfStaffReturned.Value = "0";

            hfDayLabels.Value = "";
            hfDayIssued.Value = "";
            hfDayReturned.Value = "";

            hfCategoryLabels.Value = "";
            hfCategoryValues.Value = "";

            hfFineLabels.Value = "";
            hfFineValues.Value = "";

            hfPieLabels.Value = "";
            hfPieValues.Value = "";

            hfDoughnutLabels.Value = "";
            hfDoughnutValues.Value = "";

            lblTotalFine.Text = "0";
        }
      

        /* ═══════════════════════════════════════
           CARD CLICK → LOAD GRID
        ═══════════════════════════════════════ */
        protected void Card_Click(object sender, CommandEventArgs e)
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(txtFromDate.Text, out fromDate)) return;
            if (!DateTime.TryParse(txtToDate.Text, out toDate)) return;

            string actionType = e.CommandArgument.ToString();
            using (DataSet ds = objAdminBO.GetDashboardGrid(actionType, fromDate, toDate))
            {
                DataTable dt = (ds != null && ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();

                lblModalTitle.Text = $"{actionType} Books";
                gvData.PageIndex = 0;
                gvData.DataSource = dt;
                gvData.DataBind();
                lblRecordCount.Text = "No. of Records: " + dt.Rows.Count;
                ViewState["GridData"] = dt;
                ViewState["ReportName"] = actionType;

                rptPager.DataSource = null;
                rptPager.DataBind();

                if (dt.Rows.Count > gvData.PageSize)
                {
                    rptPager.Visible = true;
                    CommonFunction.BuildPager(rptPager,gvData.PageCount, gvData.PageIndex);
                }
                else
                {
                    rptPager.Visible = false;
                }
                ShowModal();
            }
        }

        /* ═══════════════════════════════════════
           PAGER
        ═══════════════════════════════════════ */
        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Page") return;
            DataTable dt = ViewState["GridData"] as DataTable;
            if (dt == null) return;

            int newIndex = Convert.ToInt32(e.CommandArgument);
            if (newIndex < 0) newIndex = 0;
            if (newIndex >= gvData.PageCount) newIndex = gvData.PageCount - 1;

            gvData.PageIndex = newIndex;
            gvData.DataSource = dt;
            gvData.DataBind();

            rptPager.DataSource = null;
            rptPager.DataBind();

            if (dt.Rows.Count > gvData.PageSize)
            {
                rptPager.Visible = true;
                CommonFunction.BuildPager(rptPager, gvData.PageCount, gvData.PageIndex);
            }
            else
            {
                rptPager.Visible = false;
            }
            ShowModal();
        }

      

        /* ═══════════════════════════════════════
           DOWNLOAD CSV
        ═══════════════════════════════════════ */
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ViewState["GridData"] as DataTable;
                string reportName = ViewState["ReportName"]?.ToString() ?? "Report";

                if (dt == null || dt.Rows.Count == 0)
                {
                    ShowAlert("No data available for export.", "warning");
                    return;
                }

                StringBuilder sb = CommonFunction.CSVFileGenerationWithoutHeader(dt, reportName);
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("Content-Disposition", $"attachment;filename={reportName}.csv");
                Response.ContentType = "text/csv";
                Response.Write(sb.ToString());
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                ViewState.Remove("GridData");
            }
            catch (Exception ex)
            {
                MyExceptionLogger.Publish(ex);
                ShowAlert("Failed to download CSV.", "error");
            }
        }

        private void ShowModal()
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShowModal",
                "$(document).ready(function(){ $('#dataModal').modal('show'); initPagination(); });", true);
        }

        private void ShowAlert(string msg, string type)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                $"AlertMessage('{msg}','{type}');", true);
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            try { if (objAdminBO != null) { objAdminBO.ReleaseResources(); objAdminBO = null; } }
            catch (Exception ex) { MyExceptionLogger.Publish(ex); }
        }
    }
}

