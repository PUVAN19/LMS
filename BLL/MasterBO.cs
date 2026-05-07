using DAL;
using System;
using System.Data;

namespace BLL
{
    public class MasterBO
    {
        private MasterDAO objMasterDAO;
        public MasterBO()
        {
            objMasterDAO = new MasterDAO();
        }
        public void ReleaseResources()
        {
            objMasterDAO.ReleaseResources();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        public DataSet AuthorMaster(string action, int authorID = 0, string authorName = "",
                            string authorType = "", bool active = true, int adminUserID = 0)
        {
            return objMasterDAO.AuthorMaster(action, authorID, authorName, authorType, active, adminUserID);
        }
        public DataSet CategoryMaster(string action, int categoryID = 0, string categoryName = "",
                           string description = "", bool active = true, int adminUserID = 0)
        {
            return objMasterDAO.CategoryMaster(action, categoryID, categoryName, description, active, adminUserID);
        }


        public DataSet BookMaster(
     string Action,
     int BookID = 0,
     string ISBN = null,
     int CategoryID = 0,
     string BookTitle = null,
     string Language = null,
     string PublisherName = null,
     int? YearPublished = null,
     string Edition = null,
     decimal? Price = null,
     int? TotalCopies = null,
     decimal? TotalPrice = null,
     bool? TaxCheck = null,
     decimal? TaxPercent = null,
     decimal? TaxAmount = null,
     decimal? FinalAmount = null,
     string UploadReceipt = null,
     string eBook = null,
     string ShelfLocation = null,
     bool? Active = null,
     string AuthorIDs = null,
     int AdminUserID = 0,
      string UploadedFileName = null,
     string searchBy = null,
     string searchValue = null,
     int? ReceiptID = null
    // ✅ add this
 )
        {
            return objMasterDAO.BookMaster(
     Action, BookID, ISBN, CategoryID, BookTitle, Language,
     PublisherName, YearPublished, Edition, Price,
     TotalCopies, TotalPrice, TaxCheck,
     TaxPercent, TaxAmount, FinalAmount,
     UploadReceipt, eBook, ShelfLocation, Active,
     AuthorIDs, AdminUserID, UploadedFileName,searchBy, searchValue, ReceiptID );
        }
        public int DeleteReceipt(int receiptId, int adminUserId)
        {
            return objMasterDAO.DeleteReceipt(receiptId, adminUserId);
        }

        public DataSet BookAuthor(string action, int bookId, int? authorId = null)
        {
            return objMasterDAO.BookAuthor(action, bookId, authorId);
        }

        public DataSet BookPurchaseLog_Insert(int BookID, decimal Price, int Quantity, bool TaxIncluded, decimal TaxPercentage, decimal TaxAmount,
     decimal FinalAmount, string Receipt, int AdminUserID,string UploadedFileName)
        {
            return objMasterDAO.BookPurchaseLog_Insert(BookID, Price, Quantity, TaxIncluded, TaxPercentage, TaxAmount, FinalAmount, Receipt, AdminUserID , UploadedFileName
            );
        }

        public DataTable BulkInsertBooks(DataTable bulkDt, int adminUserID)
        {
            return objMasterDAO.BulkInsertBooks(bulkDt, adminUserID);
        }

        public DataTable GetConfigValues(string action, string configName)
        {
            return objMasterDAO.GetConfigValues(action, configName);
        }
        public DataSet BulkReceiptUpload(DataTable bulkReceiptDt, int adminUserID)
        {
            return objMasterDAO.BulkReceiptUpload(bulkReceiptDt, adminUserID);
        }
    }
}

