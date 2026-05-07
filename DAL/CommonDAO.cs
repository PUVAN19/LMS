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
    public class CommonDAO
    {
        private SqlConnection objSqlConnection;

        public CommonDAO()
        {
            objSqlConnection = new SqlConnection(Security.CryptTripleDES(false, ConfigurationManager.AppSettings["ConnectionString"].ToString()));
        }
        public void ReleaseResources()
        {
            if (objSqlConnection.State != ConnectionState.Closed)
                objSqlConnection.Close();
            objSqlConnection.Dispose();
        }

        public DataSet BookIssue(string Action, int IssueID, string MemberID, string IssueType,
             DateTime? IssueDate, DateTime? DueDate, string BookIDList, int AdminUserID, bool Active)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", Action),
        new SqlParameter("@IssueID", IssueID),
        new SqlParameter("@MemberID", MemberID),
        new SqlParameter("@IssueType", (object)IssueType ?? DBNull.Value),
        new SqlParameter("@IssueDate", (object)IssueDate ?? DBNull.Value),
        new SqlParameter("@DueDate", (object)DueDate ?? DBNull.Value),
        
        // list of multiple book ids
        new SqlParameter("@BookIDList", (object)BookIDList ?? DBNull.Value),
        
        new SqlParameter("@AdminUserID", AdminUserID),
        new SqlParameter("@Active", Active)
            };

            return SqlHelper.ExecuteDataset(
                        objSqlConnection,
                        CommandType.StoredProcedure,
                        "Issuebook",
                        objParam
                   );
        }

        public DataSet ValidateMember(string Action, string MemberID, string IssueType)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
            new SqlParameter("@Action", Action),
            new SqlParameter("@MemberID", MemberID),
            new SqlParameter("@IssueType", (object)IssueType ?? DBNull.Value),
           
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "ValidateMember",
                objParam
            );
        }

        public DataSet GetMemberDashboard(string MemberID)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@MemberID", MemberID)
            };

            return SqlHelper.ExecuteDataset(
                        objSqlConnection,
                        CommandType.StoredProcedure,
                        "GetMemberDashboard",
                        objParam
                   );
        }
        public DataSet BookRenewalRequest(
    string Action,
    string memberId,
    int bookIssueId,
    int noOfDays
)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", Action),
        new SqlParameter("@MemberID", memberId),
        new SqlParameter("@BookIssueID", bookIssueId),
        new SqlParameter("@NoOfDays", noOfDays)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "BookRenewalRequest",
                objParam
            );
        }

        public DataSet RenewalRequestAction(
    string action,                 // APPROVE / REJECT
    int renewalRequestId,
    DateTime? approvedDueDate,     // Only for APPROVE
    string rejectReason,           // Only for REJECT
    int adminId
)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", action),
        new SqlParameter("@RenewalRequestID", renewalRequestId),
        new SqlParameter("@ApprovedDueDate", approvedDueDate ?? (object)DBNull.Value),
        new SqlParameter("@RejectReason", rejectReason ?? (object)DBNull.Value),
        new SqlParameter("@AdminID", adminId)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "RenewalRequestAction",
                objParam
            );
        }
        public DataSet GetBookIssueDashboard(string action)
        {
            SqlParameter[] param =
            {
                 new SqlParameter("@Action", action)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "AdminDashboard",
                param
            );
        }
        public DataSet UpdateFineAmount(string action, int issueId, decimal fineAmount, int AdminUserID)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@IssueID", issueId),
                new SqlParameter("@FineAmount", (object)fineAmount ?? DBNull.Value),
                new SqlParameter("@AdminUserID", AdminUserID)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "AdminDashboard",
                param
            );
        }

        public DataSet GetBookAvailability(
         string categoryNames,
         string isbn,
         string bookTitle,
         string authorName,
         int? yearPublished,
         string publisherName)
        {
            SqlParameter[] param =
            {
                    new SqlParameter("@CategoryNames", (object)categoryNames ?? DBNull.Value),
                    new SqlParameter("@ISBN", (object)isbn ?? DBNull.Value),
                    new SqlParameter("@BookTitle", (object)bookTitle ?? DBNull.Value),
                    new SqlParameter("@AuthorName", (object)authorName ?? DBNull.Value),
                    new SqlParameter("@YearPublished", (object)yearPublished ?? DBNull.Value),
                    new SqlParameter("@PublisherName", (object)publisherName ?? DBNull.Value)
                };

            return SqlHelper.ExecuteDataset(objSqlConnection, CommandType.StoredProcedure, "CheckBookAvailability", param);
        }

        public DataSet FineAmount(string action)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Action", action)
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "FineCollectionReport",
                param
            );
        }
    }
}
