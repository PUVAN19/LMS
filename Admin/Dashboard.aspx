<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Admin.Dashboard" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>LMS Admin Dashboard</title>
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Outfit:wght@400;500;600;700&display=swap" rel="stylesheet" />

    <style>
        /* ── BASE ── */
        body {
            font-family: 'Outfit', sans-serif;
            background: #f0f4f8;
        }
        .card{
            letter-spacing:0px !important;
        }
        /* ── SUMMARY CARDS ── */
        .summary-card {
            border: none;
            border-radius: 16px;
            color: #fff;
            min-height: 115px;
            cursor: pointer;
            transition: transform 0.25s ease, box-shadow 0.25s ease;
            position: relative;
            overflow: hidden;
        }

            .summary-card::before {
                content: '';
                position: absolute;
                top: -30px;
                right: -30px;
                width: 110px;
                height: 110px;
                border-radius: 50%;
                background: rgba(255,255,255,0.12);
            }

            .summary-card:hover {
                transform: translateY(-5px);
                box-shadow: 0 14px 28px rgba(0,0,0,0.18);
            }

            .summary-card h5 {
                font-size: 0.85rem;
                font-weight: 500;
                opacity: 0.9;
                margin-bottom: 4px;
            }

            .summary-card h2 {
                font-size: 2rem;
                font-weight: 700;
                margin: 0;
            }

        .icon {
            width: 58px;
            height: 58px;
            object-fit: contain;
            filter: brightness(0) invert(1);
            opacity: 0.85;
        }

        .bg-NewAddBooks {
            background: linear-gradient(135deg, #2563eb, #60a5fa);
        }

        .bg-issued {
            background: linear-gradient(135deg, #059669, #34d399);
        }

        .bg-due {
            background: linear-gradient(135deg, #d97706, #fbbf24);
        }

        .bg-returned {
            background: linear-gradient(135deg, #db2777, #f472b6);
        }

        /* ── CHART CARDS ── */
        .chart-card {
            border: none;
            border-radius: 16px;
            box-shadow: 0 2px 16px rgba(0,0,0,0.07);
            background: #fff;
            overflow: hidden;
            display: flex;
            flex-direction: column;
            height: 100%;
            width: 100%;
        }

            .chart-card .card-header {
                background: #fff;
                border-bottom: 1px solid #f1f5f9;
                padding: 14px 20px;
                font-size: 0.95rem;
                font-weight: 600;
                color: #1e293b;
                display: flex;
                align-items: center;
                gap: 8px;
                flex-shrink: 0;
            }

                .chart-card .card-header .chart-icon {
                    width: 22px;
                    height: 22px;
                    border-radius: 6px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 12px;
                }

            .chart-card .card-body {
                flex: 1;
                display: flex;
                flex-direction: column;
                padding: 16px 20px;
            }

        /* ── BADGE PILLS ON CARD HEADER ── */
        .chart-badge {
            margin-left: auto;
            font-size: 0.7rem;
            font-weight: 500;
            padding: 2px 10px;
            border-radius: 20px;
            background: #f1f5f9;
            color: #64748b;
        }

        /* ── SECTION TITLE ── */
        .section-title {
            font-size: 1.5rem;
            font-weight: 700;
            color: #0f172a;
            letter-spacing: -0.3px;
        }

        .section-sub {
            font-size: 0.82rem;
            color: #94a3b8;
            margin-top: 2px;
        }

        .chart-canvas-wrap {
            height: 320px;
            position: relative;
        }

            .chart-canvas-wrap canvas {
                width: 100% !important;
                height: 100% !important;
            }
        /* ── MODAL ── */
        .modal-header {
            padding: 12px 20px;
        }

            .modal-header .btn-close {
                top: 15px !important;
            }

        .csv-download-btn {
            position: absolute;
            right: 55px;
            top: 50%;
            transform: translateY(-50%);
        }

        /* ── FINE STAT STRIP ── */
        .fine-stat {
            background: linear-gradient(135deg, #7c3aed, #a78bfa);
            border-radius: 12px;
            padding: 14px 20px;
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 16px;
        }

            .fine-stat .label {
                font-size: 0.78rem;
                opacity: 0.85;
            }

            .fine-stat .value {
                font-size: 1.6rem;
                font-weight: 700;
            }

        /* ── RESPONSIVE CHART HEIGHT ── */
        @media (max-width: 768px) {
            .chart-canvas-wrap {
                height: 260px !important;
            }
        }

        .small-chart {
            height: 220px;
        }



        /* ✅ Single consistent chart wrapper */
        .chart-box {
            position: relative;
            width: 100%;
            height: 300px;
            min-height: 300px;
            flex-shrink: 0;
        }

            .chart-box canvas {
                width: 100% !important;
                height: 100% !important;
            }

            /* ✅ Pie & Doughnut */
            .chart-box.chart-small {
                height: 260px;
                min-height: 260px;
            }

            /* ✅ Fine Trend — grows to fill all remaining card space */
            .chart-box.chart-line {
                flex: 1 1 0;
                height: 0; /* required — lets flex-grow take over */
                min-height: 150px; /* safety floor so chart never collapses */
            }

        /* ✅ Perfect centering for pie/doughnut */
        .chart-center {
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* ✅ Mobiles */
        @media (max-width: 768px) {
            .chart-box {
                height: 240px;
            }

                .chart-box.chart-small {
                    height: 220px;
                }
        }
        /* Desktop input size */
.date-input {
    width: 100%;
}

/* Mobile view */
@media (max-width: 768px) {

    /* Hide arrow */
    .arrow {
        display: none;
    }

    /* Stack nicely */
    .filter-row > div {
        width: 100%;
    }
}
   

       
    </style>
</head>

<body>
    <uc1:header runat="server" id="Header" />

    <form id="form1" runat="server">
        <div class="container-fluid pt-3 pb-4 px-4">

            <!-- ── HEADER ROW ── -->
            <!-- Font Awesome (only if not already in your template) -->
            <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

           <div class="row mb-4 align-items-center">

    <!-- Title -->
    <div class="col-md-7 col-12 mb-2 mb-md-0">
        <div class="section-title">📚 Library Dashboard</div>
        <div class="section-sub">Overview of library activity for the selected period</div>
    </div>

    <!-- Filters -->
    <div class="col-md-5 col-12">
        <div class="row align-items-center g-2 filter-row ">

            <!-- From Date -->
            <div class="col-md-4 col-12">
                <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date"
                    CssClass="form-control date-input"
                    onkeydown="return false;" onpaste="return false;" />
            </div>

            <!-- Arrow -->
            <div class="col-md-auto col-12 text-center arrow">
                <span class="text-muted">→</span>
            </div>

            <!-- To Date -->
            <div class="col-md-4 col-12">
                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date"
                    CssClass="form-control date-input"
                    onkeydown="return false;" onpaste="return false;" />
            </div>

            <!-- Search -->
            <div class="col-md-3 col-12">
                <asp:LinkButton ID="btnSearch" runat="server"
                    CssClass="btn btn-primary fw-semibold w-100"
                    OnClick="btnSearch_Click">
                    Search
                </asp:LinkButton>
            </div>

        </div>
    </div>

</div>


            <!-- ── SUMMARY CARDS ── -->
            <div class="row g-3 mb-2">
                <div class="col-lg-3 col-sm-6">
                    <asp:LinkButton runat="server" CssClass="text-decoration-none d-block"
                        CommandArgument="Added" OnCommand="Card_Click">
                        <div class="card summary-card bg-NewAddBooks">
                            <div class="card-body d-flex justify-content-between align-items-center">
                                <div>
                                    <h5>Added Books</h5>
                                    <h2>
                                        <asp:Label ID="lblTotalBooks" runat="server" Text="0" /></h2>
                                </div>
                                <img src="../assets/images/icons/NewBook.png" class="icon" />
                            </div>
                        </div>
                    </asp:LinkButton>
                </div>
                <div class="col-lg-3 col-sm-6">
                    <asp:LinkButton runat="server" CssClass="text-decoration-none d-block"
                        CommandArgument="Issued" OnCommand="Card_Click">
                        <div class="card summary-card bg-issued">
                            <div class="card-body d-flex justify-content-between align-items-center">
                                <div>
                                    <h5>Total Issued </h5>
                                    <h2>
                                        <asp:Label ID="lblTotalIssued" runat="server" Text="0" /></h2>
                                </div>
                                <img src="../assets/images/icons/issuebooks.png" class="icon" />
                            </div>
                        </div>
                    </asp:LinkButton>
                </div>
                <div class="col-lg-3 col-sm-6">
                    <asp:LinkButton runat="server" CssClass="text-decoration-none d-block"
                        CommandArgument="Due" OnCommand="Card_Click">
                        <div class="card summary-card bg-due">
                            <div class="card-body d-flex justify-content-between align-items-center">
                                <div>
                                    <h5>Due Books</h5>
                                    <h2>
                                        <asp:Label ID="lblDueBooks" runat="server" Text="0" /></h2>
                                </div>
                                <img src="../assets/images/icons/deadline.png" class="icon" />
                            </div>
                        </div>
                    </asp:LinkButton>
                </div>
                <div class="col-lg-3 col-sm-6">
                    <asp:LinkButton runat="server" CssClass="text-decoration-none d-block"
                        CommandArgument="Returned" OnCommand="Card_Click">
                        <div class="card summary-card bg-returned">
                            <div class="card-body d-flex justify-content-between align-items-center">
                                <div>
                                    <h5>Returned Books</h5>
                                    <h2>
                                        <asp:Label ID="lblReturnedBooks" runat="server" Text="0" /></h2>
                                </div>
                                <img src="../assets/images/icons/book.png" class="icon" />
                            </div>
                        </div>
                    </asp:LinkButton>
                </div>
            </div>

            <!-- ── ROW 1: Issued vs Returned (bar) + Category Popularity (horizontal bar) ── -->
            <div class="row g-3 mb-3 ">
                <!-- Issued vs Returned by Member Type — Google Bar Chart (KEPT) -->
                <div class="col-12 col-md-6">
                    <div class="card chart-card h-100">
                        <div class="card-header">
                            <span class="chart-icon" style="background: #dbeafe;">📊</span>
                            Issued vs Returned — Student &amp; Staff
                           
                            <span class="chart-badge">Bar</span>
                        </div>

                        <div class="card-body">
                            <div class="chart-box">
                                <div id="barChart"></div>
                            </div>
                        </div>

                    </div>
                </div>

                <!-- NEW: Book Category Popularity -->
                <div class="col-12 col-md-6">
                    <div class="card chart-card h-100">
                        <div class="card-header">
                            <span class="chart-icon" style="background: #fef9c3;">📖</span>
                            Top Book Categories Borrowed
                           
                            <span class="chart-badge">Horizontal Bar</span>
                        </div>

                        <div class="card-body">
                            <div class="chart-box">
                                <canvas id="categoryChart"></canvas>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="row g-3 align-items-stretch">

                <!-- Day Wise -->
                <div class="col-12 col-md-6 d-flex flex-column">
                    <div class="card chart-card h-100 w-100">
                        <div class="card-header">
                            <span class="chart-icon" style="background: #dcfce7;">📅</span>
                            Issue vs Returned — Day Wise
                <span class="chart-badge">Stacked Bar</span>
                        </div>
                        <div class="card-body d-flex flex-column pb-0">
                            <div class="chart-box">
                                <canvas id="dayWiseChart"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Fine Collection Trend -->
                <div class="col-12 col-md-6 d-flex flex-column">
                    <div class="card chart-card h-100 w-100">
                        <div class="card-header">
                            <span class="chart-icon" style="background: #ede9fe;">💰</span>
                            Fine Collection Trend
                <span class="chart-badge">Line</span>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <div class="fine-stat">
                                <div>
                                    <div class="label">Total Fine Collected</div>
                                    <div class="value">₹<asp:Label ID="lblTotalFine" runat="server" Text="0" /></div>
                                </div>
                                <div>🏷️</div>
                            </div>
                            <!-- ✅ flex:1 makes this grow to fill remaining card height -->
                            <div class="chart-box chart-line">
                                <canvas id="fineTrendChart"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="row g-3 mb-2">
                <div class="col-12 col-md-6">
                    <div class="card chart-card h-100">
                        <div class="card-header">
                            <span class="chart-icon" style="background: #fee2e2;">📊</span>
                            Book Status
           
                            <span class="chart-badge">Pie</span>
                        </div>
                        <div class="card-body chart-center">
                            <div class="chart-box chart-small">
                                <canvas id="pieStatusChart"></canvas>
                            </div>
                        </div>

                    </div>
                </div>

                <!-- ── DOUGHNUT CHART: RETURN PERFORMANCE ── -->
                <div class="col-12 col-md-6">
                    <div class="card chart-card h-100">
                        <div class="card-header">
                            <span class="chart-icon" style="background: #e0f2fe;">🎯</span>
                            Return Performance
           
                            <span class="chart-badge">Doughnut</span>
                        </div>

                        <div class="card-body chart-center">
                            <div class="chart-box chart-small">
                                <canvas id="doughnutChart"></canvas>
                            </div>
                        </div>

                    </div>
                </div>

            </div>




            <!-- ── MODAL GRID ── -->
            <div class="modal fade" id="dataModal" tabindex="-1">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header bg-primary text-white p-2 position-relative">
                            <h5 class="modal-title mb-0">
                                <asp:Label ID="lblModalTitle" runat="server" />
                            </h5>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btnDownload_Click"
                                ToolTip="Download CSV" CssClass="csv-download-btn">
                                <asp:Image ID="Image1" runat="server"
                                    ImageUrl="~/assets/images/icons/csvdownload.png"
                                    AlternateText="Download" Width="35" Height="35" />
                            </asp:LinkButton>
                            <button type="button" class="btn-close btn-close-white ms-2"
                                data-bs-dismiss="modal" aria-label="Close">
                            </button>
                        </div>
                        <div class="modal-body pt-0">
                            <div class="col-12 col-lg-auto ms-lg-auto text-lg-end pt-0">
                                <asp:Label ID="lblRecordCount" runat="server" CssClass="fw-bold text-primary" />
                            </div>
                            <asp:GridView ID="gvData" runat="server"
                                CssClass="table table-bordered table-striped table-hover fixed-header"
                                AutoGenerateColumns="true" EmptyDataText="No records found"
                                AllowPaging="True" PageSize="5" PagerSettings-Visible="false" />
                            <div class="pager-fixed d-flex justify-content-end">
                                <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPage" runat="server"
                                            CssClass='<%# (bool)Eval("IsActive") ? "page-btn active" : "page-btn" %>'
                                            CommandName='<%# Eval("Command") %>'
                                            CommandArgument='<%# Eval("PageIndex") %>'
                                            Enabled='<%# Eval("Enabled") %>'
                                            Text='<%# Eval("Text") %>' />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ── HIDDEN FIELDS ── -->
            <%-- Existing --%>
            <asp:HiddenField ID="hfStudentIssued" runat="server" />
            <asp:HiddenField ID="hfStudentReturned" runat="server" />
            <asp:HiddenField ID="hfStaffIssued" runat="server" />
            <asp:HiddenField ID="hfStaffReturned" runat="server" />
            <asp:HiddenField ID="hfDayLabels" runat="server" />
            <asp:HiddenField ID="hfDayIssued" runat="server" />
            <asp:HiddenField ID="hfDayReturned" runat="server" />

            <%-- NEW --%>
            <asp:HiddenField ID="hfCategoryLabels" runat="server" />
            <asp:HiddenField ID="hfCategoryValues" runat="server" />
            <asp:HiddenField ID="hfFineLabels" runat="server" />
            <asp:HiddenField ID="hfFineValues" runat="server" />
            <asp:HiddenField ID="hfOverdueLabels" runat="server" />
            <asp:HiddenField ID="hfOverdueValues" runat="server" />

            <%-- (keep old DD fields if anything else references them) --%>
            <asp:HiddenField ID="hfStudentIssuedDD" runat="server" />
            <asp:HiddenField ID="hfStudentReturnedDD" runat="server" />
            <asp:HiddenField ID="hfStaffIssuedDD" runat="server" />
            <asp:HiddenField ID="hfStaffReturnedDD" runat="server" />
            <asp:HiddenField ID="hfStudentTotal" runat="server" />
            <asp:HiddenField ID="hfStaffTotal" runat="server" />

            <asp:HiddenField ID="hfPieLabels" runat="server" />
            <asp:HiddenField ID="hfPieValues" runat="server" />

            <asp:HiddenField ID="hfDoughnutLabels" runat="server" />
            <asp:HiddenField ID="hfDoughnutValues" runat="server" />
        </div>
    </form>

    <uc1:footer runat="server" id="Footer" />

    <!-- ── SCRIPTS ── -->
    <script src="../assets/js/chart/google/google-chart-loader.js"></script>
    <script src="../assets/js/customcharts/chartjs.js"></script>
    <script src="../assets/js/customcharts/chartjsplugin.js"></script>

    <script>
        /* ── helpers ── */
        function hv(id) { return document.getElementById(id).value; }
        function hp(id) { var v = hv(id); return v ? parseInt(v) : 0; }
        function hArr(id) { var v = hv(id); return v ? v.split(',') : []; }
        function hNums(id) { return hArr(id).map(Number); }

        /* ══ GOOGLE: Issued vs Returned bar ══ */
        google.charts.load('current', { packages: ['corechart', 'bar'] });
        google.charts.setOnLoadCallback(function () {
            var data = google.visualization.arrayToDataTable([
                ['Category', 'Issued', 'Returned'],
                ['Student', hp('<%=hfStudentIssued.ClientID%>'), hp('<%=hfStudentReturned.ClientID%>')],
                ['Staff', hp('<%=hfStaffIssued.ClientID%>'), hp('<%=hfStaffReturned.ClientID%>')]
            ]);
            var options = {
                bars: 'vertical',
                height: 320,
                legend: { position: 'top' },
                chartArea: { width: '80%', height: '68%' },
                colors: ['#2563eb', '#db2777'],
                bar: { groupWidth: '50%' },
                animation: { startup: true, duration: 700 }
            };
            var chart = new google.charts.Bar(document.getElementById('barChart'));
            chart.draw(data, google.charts.Bar.convertOptions(options));
        });

        document.addEventListener("DOMContentLoaded", function () {

            /* ── palette helpers ── */
            var palette = [
                '#2563eb', '#059669', '#d97706', '#db2777',
                '#7c3aed', '#0891b2', '#65a30d', '#ea580c'
            ];

            /* ══ 1. CATEGORY POPULARITY — horizontal bar ══ */
            var catLabels = hArr('<%=hfCategoryLabels.ClientID%>');
            var catValues = hNums('<%=hfCategoryValues.ClientID%>');

            new Chart(document.getElementById('categoryChart'), {
                type: 'bar',
                data: {
                    labels: catLabels,
                    datasets: [{
                        label: 'Times Borrowed',
                        data: catValues,
                        backgroundColor: catLabels.map(function (_, i) { return palette[i % palette.length]; }),
                        borderRadius: 6,
                        borderSkipped: false
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false },

                        datalabels: {
                            anchor: 'end', align: 'end',
                            color: '#374151', font: { size: 11, weight: '600' },
                            formatter: function (v) { return v > 0 ? v : ''; }
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true,
                            grid: { color: '#f1f5f9' },
                            ticks: { color: '#64748b', font: { size: 11 } }
                        },
                        y: {
                            grid: { display: false },
                            ticks: { color: '#374151', font: { size: 12 } }
                        }
                    }
                },
                plugins: [ChartDataLabels]
            });

            /* ══ 2. DAY-WISE STACKED BAR (kept) ══ */
            var dayLabels = hArr('<%=hfDayLabels.ClientID%>');
            var dayIssued = hNums('<%=hfDayIssued.ClientID%>');
            var dayReturned = hNums('<%=hfDayReturned.ClientID%>');

            new Chart(document.getElementById('dayWiseChart'), {
                type: 'bar',
                data: {
                    labels: dayLabels,
                    datasets: [
                        { label: 'Issued', data: dayIssued, backgroundColor: '#2563eb', borderRadius: 4 },
                        { label: 'Returned', data: dayReturned, backgroundColor: '#db2777', borderRadius: 4 }
                    ]
                },
                options: {
                    layout: {
                        padding: { top: 0, bottom: 0, left: 0, right: 0 }
                    },
                    scales: {
                        x: {
                            stacked: true,
                            grid: { display: false },
                            ticks: { color: '#64748b', font: { size: 10 } }
                        },
                        y: {
                            stacked: true,
                            beginAtZero: true,
                            grid: { color: '#f1f5f9' },
                            ticks: { color: '#64748b' }
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                pointStyle: 'circle',
                                padding: 8          /* ✅ tightens legend gap */
                            }
                        }
                    }
                }
            });

            /* ══ 3. FINE TREND — line chart ══ */
            var fineLabels = hArr('<%=hfFineLabels.ClientID%>');
            var fineValues = hNums('<%=hfFineValues.ClientID%>');

            new Chart(document.getElementById('fineTrendChart'), {
                type: 'line',
                data: {
                    labels: fineLabels,
                    datasets: [{
                        label: 'Fine (₹)',
                        data: fineValues,
                        borderColor: '#7c3aed',
                        backgroundColor: 'rgba(124,58,237,0.1)',
                        fill: true,
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false },
                        datalabels: { display: false }
                    },
                    scales: {
                        x: { grid: { display: false } },
                        y: { beginAtZero: true }
                    }
                }
            });
            const pieRawLabels = document.getElementById('<%= hfPieLabels.ClientID %>').value;
            const pieRawValues = document.getElementById('<%= hfPieValues.ClientID %>').value;

            if (pieRawLabels && pieRawValues) {
                const pieLabels = pieRawLabels.split(',');
                const pieValues = pieRawValues.split(',');

                new Chart(document.getElementById('pieStatusChart'), {
                    type: 'pie',
                    data: {
                        labels: pieLabels,
                        datasets: [{
                            data: pieValues,
                            backgroundColor: ['#2563eb', '#059669', '#d97706', '#db2777', '#7c3aed', '#0891b2'],
                            borderWidth: 2,
                            borderColor: '#fff'
                        }]
                    },

                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                position: 'bottom'
                            }
                        }
                    }

                });
            }

            // DOUGHNUT CHART
            const dRawLabels = document.getElementById('<%= hfDoughnutLabels.ClientID %>').value;
            const dRawValues = document.getElementById('<%= hfDoughnutValues.ClientID %>').value;

            if (dRawLabels && dRawValues) {
                const dLabels = dRawLabels.split(',');
                const dValues = dRawValues.split(',');

                new Chart(document.getElementById('doughnutChart'), {
                    type: 'doughnut',
                    data: {
                        labels: dLabels,
                        datasets: [{
                            data: dValues,
                            backgroundColor: ['#2563eb', '#db2777', '#059669', '#d97706', '#7c3aed'],
                            borderWidth: 2,
                            borderColor: '#fff'
                        }]
                    },

                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                position: 'bottom'
                            }
                        }
                    }

                });
            }

        });
    </script>
</body>
</html>

