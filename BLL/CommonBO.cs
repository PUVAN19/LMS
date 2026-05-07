using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CommonBO
    {
        private CommonDAO objCommonDAO;

        public CommonBO()
        {
            objCommonDAO = new CommonDAO();
        }
        public void ReleaseResources()
        {
            objCommonDAO.ReleaseResources();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }
        public DataSet BookIssue(string Action, int IssueID = 0, string MemberID = "", string IssueType = "",
    DateTime? IssueDate = null, DateTime? DueDate = null, string BookIDList = "",      // comma-separated book IDs
    int AdminUserID = 0, bool Active = true)
        {
            return objCommonDAO.BookIssue(
                Action,
                IssueID,
                MemberID,
                IssueType,
                IssueDate,
                DueDate,
                BookIDList,

                AdminUserID,
                Active
            );
        }

        public DataSet ValidateMember(string Action, string MemberID ="", string IssueType ="")

        {

            return objCommonDAO.ValidateMember(
                Action,
                MemberID,
                IssueType
               );
        }

        public DataSet GetMemberDashboard(string MemberID)
        {
            return objCommonDAO.GetMemberDashboard(MemberID);
        }

        public DataSet BookRenewalRequest(string Action,
    string memberId,
    int bookIssueId,
    int noOfDays
)
        {
            return objCommonDAO.BookRenewalRequest(
                 Action,
                 memberId,
                bookIssueId,
                noOfDays
            );
        }

        public DataSet RenewalRequestAction(
             string action,               // APPROVE / REJECT
             int renewalRequestId,
             DateTime? approvedDueDate,   // Used for APPROVE
             string rejectReason,         // Used for REJECT
             int adminId
         )
        {
            return objCommonDAO.RenewalRequestAction(
                action,
                renewalRequestId,
                approvedDueDate,
                rejectReason,
                adminId
            );
        }
        public DataSet GetBookIssueDashboard(string action)
        {
            return objCommonDAO.GetBookIssueDashboard(action);
        }
        public DataSet UpdateFineAmount(string action, int issueId, decimal fineAmount, int AdminUserID)
        {
            return objCommonDAO.UpdateFineAmount(action, issueId, fineAmount, AdminUserID);
        }


        public DataSet GetBookAvailability(string categoryNames, string isbn, string bookTitle, string authorName, int? yearPublished, string publisherName)
        {
            return objCommonDAO.GetBookAvailability(categoryNames, isbn, bookTitle, authorName, yearPublished, publisherName);
        }

        public DataSet FineAmount(string action)
        {
            return objCommonDAO.FineAmount(action);
        }

    }
}
