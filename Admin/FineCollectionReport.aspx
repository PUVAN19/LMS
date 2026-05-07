<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FineCollectionReport.aspx.cs" Inherits="Admin.FineCollectionReport" %>

<%@ Register Src="../Controls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="../Controls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fine Collection Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="../assets/css/CustomPagination.css" rel="stylesheet" />

    <style>
        .summary-card {
            border-radius: 12px;
            min-height: 120px;
            cursor: pointer;
            transition: transform 0.25s ease, box-shadow 0.25s ease;
        }

            .summary-card:hover {
                transform: scale(1.04);
                box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15);
            }

        .bg-unpaid {
            background: linear-gradient(135deg, #ef6c00, #ffe0b2);
        }

        .bg-paid {
            background: linear-gradient(135deg, #43a047, #dcedc8);
        }

        .icon {
            width: 85px;
            height: 80px;
            filter: brightness(0);
            display: flex;
            align-items: center;
            justify-content: center;
            transition: transform 0.3s ease;
        }

        #gvFine th, #gvFine td {
            text-align: center;
            vertical-align: middle;
        }
    </style>
</head>

<body>
    <uc1:header runat="server" id="Header" />

    <form id="form1" runat="server">
        <asp:HiddenField ID="hfRemoveColumnsCSV" runat="server" />
        <div class="page-body">
            <div class="container-fluid pt-3">

                <!-- Summary Cards -->
                <div class="row g-3">

                    <!-- Yet To Be Paid -->
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardUnpaid_Click">
                            <div class="card summary-card bg-unpaid shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div>
                                        <h4 class="mb-1">Yet To Be Paid</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblUnpaidCount" runat="server" Text="0" />
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/penalty.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>

                    <!-- Paid -->
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <asp:LinkButton runat="server" OnClick="CardPaid_Click">
                            <div class="card summary-card bg-paid shadow-sm">
                                <div class="card-body d-flex align-items-center justify-content-between">
                                    <div>
                                        <h4 class="mb-1">Paid</h4>
                                        <h2 class="fw-bold mb-0">
                                            <asp:Label ID="lblPaidCount" runat="server" Text="0" />
                                        </h2>
                                    </div>
                                    <div class="icon-box">
                                        <img src="../assets/images/icons/approve.png" class="icon" />
                                    </div>
                                </div>
                            </div>
                        </asp:LinkButton>
                    </div>

                </div>

                <!-- Grid -->
                <div class="card" runat="server" id="divGrid" visible="false">
                    <div class="card-header bg-primary text-white p-2 d-flex justify-content-between align-items-center">
                        <h4 class="mb-0" id="lblGridTitle" runat="server">Fine Report</h4>


                        <asp:LinkButton
                            ID="lnkDownloadCSV"
                            runat="server"
                            OnClick="btnDownloadCSV_Click"
                            ToolTip="Download CSV">

                            <asp:Image
                                ID="imgDownload"
                                runat="server"
                                ImageUrl="~/assets/images/icons/csvdownload.png"
                                AlternateText="Download"
                                CssClass="img-fluid"
                                Width="35" Height="35" />
                        </asp:LinkButton>
                    </div>

                    <div class="card-body">
                     <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">

                        <asp:GridView ID="gvFine"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-striped"
                            EmptyDataText="No records found"
                            AllowPaging="true"
                            PageSize="5"
                            OnPageIndexChanging="gvFine_PageIndexChanging"
                            PagerSettings-Visible="false">

                            <Columns>

                                <asp:BoundField DataField="MemberID" HeaderText="Member ID" />

                                <asp:BoundField DataField="BookTitle" HeaderText="Book Title" />

                                <asp:BoundField DataField="BookPrice"
                                    HeaderText="Book Price"
                                    DataFormatString="{0:N2}" />

                                <asp:BoundField DataField="FineAmount"
                                    HeaderText="Fine Amount"
                                    DataFormatString="{0:N2}" />

                                <asp:BoundField DataField="AllottedOn"
                                    HeaderText="Allotted On"
                                    DataFormatString="{0:dd-MMM-yyyy}" />

                                <asp:BoundField DataField="PaidDate"
                                    HeaderText="Paid Date"
                                    DataFormatString="{0:dd-MMM-yyyy}" />

                            </Columns>

                        </asp:GridView>
                      </div>

                        <div class="pager-fixed d-flex justify-content-end" runat="server" id="divPager">
                            <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkPage" runat="server"
                                        CssClass='<%# (bool)Eval("IsActive") ? "page-btn active" : "page-btn" %>'
                                        CommandName='<%# Eval("Command") %>'
                                        CommandArgument='<%# Eval("PageIndex") %>'
                                        Enabled='<%# Eval("Enabled") %>'
                                        Text='<%# Eval("Text") %>'>
                              </asp:LinkButton>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
                </div>

            </div>

    </form>

    <uc1:footer runat="server" id="Footer" />
</body>
</html>
