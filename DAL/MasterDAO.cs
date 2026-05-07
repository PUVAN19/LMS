using Library;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace DAL
{
    public class MasterDAO
    {
        private SqlConnection objSqlConnection;

        public MasterDAO()
        {
            objSqlConnection = new SqlConnection(Security.CryptTripleDES(false, ConfigurationManager.AppSettings["ConnectionString"].ToString()));
        }
        public void ReleaseResources()
        {
            if (objSqlConnection.State != ConnectionState.Closed)
                objSqlConnection.Close();
            objSqlConnection.Dispose();
        }
       

        public DataSet AuthorMaster(string action, int authorID = 0, string authorName = "",
                            string authorType = "", bool active = true, int adminUserID = 0)
        {
            SqlParameter[] objparam = new SqlParameter[]
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@AuthorID", authorID),
                new SqlParameter("@AuthorName", (object)authorName ?? DBNull.Value),
                new SqlParameter("@AuthorType", (object)authorType ?? DBNull.Value),
                new SqlParameter("@Active", active),
                new SqlParameter("@AdminUserID", adminUserID),

            };

            return SqlHelper.ExecuteDataset(objSqlConnection,
                    CommandType.StoredProcedure, "dbo.AuthorMaster", objparam);
        }

        public DataSet CategoryMaster(string action, int categoryID = 0, string categoryName = "",
                           string description = "", bool active = true, int adminUserID = 0)
        {
            SqlParameter[] objparam = new SqlParameter[]
            {
                new SqlParameter("@Action", action),
                new SqlParameter("@CategoryID", categoryID),
                new SqlParameter("@CategoryName", (object)categoryName ?? DBNull.Value),
                new SqlParameter("@Description", (object)description ?? DBNull.Value),
                new SqlParameter("@Active", active),
                new SqlParameter("@AdminUserID", adminUserID),

            };

            return SqlHelper.ExecuteDataset(objSqlConnection,
                    CommandType.StoredProcedure, "dbo.CategoryMaster", objparam);
        }


        public DataSet BookMaster(string Action,  int BookID = 0, string ISBN = null, int CategoryID = 0, string BookTitle = null,
     string Language = null,  string PublisherName = null,  int? YearPublished = null, string Edition = null, decimal? Price = null,
     int? TotalCopies = null,  decimal? TotalPrice = null,  bool? TaxCheck = null, decimal? TaxPercent = null,
     decimal? TaxAmount = null, decimal? FinalAmount = null, string UploadReceipt = null,  string eBook = null,  string ShelfLocation = null,
     bool? Active = null, string AuthorIDs = null,  int AdminUserID = 0, string UploadedFileName = null ,string searchBy = null, string searchValue = null, int? ReceiptID = null // ✅ added
 )
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", Action),
        new SqlParameter("@BookID", BookID),
        new SqlParameter("@ISBN", (object)ISBN ?? DBNull.Value),
        new SqlParameter("@CategoryID", CategoryID),
        new SqlParameter("@BookTitle", (object)BookTitle ?? DBNull.Value),
        new SqlParameter("@Language", (object)Language ?? DBNull.Value),
        new SqlParameter("@PublisherName", (object)PublisherName ?? DBNull.Value),
        new SqlParameter("@YearPublished", (object)YearPublished ?? DBNull.Value),
        new SqlParameter("@Edition", (object)Edition ?? DBNull.Value),
        new SqlParameter("@Price", (object)Price ?? DBNull.Value),
        new SqlParameter("@TotalCopies", (object)TotalCopies ?? DBNull.Value),
        new SqlParameter("@TotalPrice", (object)TotalPrice ?? DBNull.Value),
        new SqlParameter("@TaxCheck", (object)(TaxCheck ?? false)),
        new SqlParameter("@TaxPercentage", (object)TaxPercent ?? DBNull.Value),
        new SqlParameter("@TaxAmount", (object)TaxAmount ?? DBNull.Value),
        new SqlParameter("@FinalAmount", (object)FinalAmount ?? DBNull.Value),
        new SqlParameter("@Receipt", (object)UploadReceipt ?? DBNull.Value), // ✅ FIX
        new SqlParameter("@eBook", (object)eBook ?? DBNull.Value),
        new SqlParameter("@ShelfLocation", (object)ShelfLocation ?? DBNull.Value),
        new SqlParameter("@Active", (object)Active ?? DBNull.Value),
        new SqlParameter("@AuthorIDs", (object)AuthorIDs ?? DBNull.Value),
        new SqlParameter("@AdminUserID", AdminUserID),
       new SqlParameter("@UploadedFileName", (object)UploadedFileName ?? DBNull.Value), // ✅ FIX
        new SqlParameter("@SearchBy", (object)searchBy ?? DBNull.Value),
        new SqlParameter("@SearchValue", (object)searchValue ?? DBNull.Value),
        new SqlParameter("@ReceiptID", ReceiptID ?? (object)DBNull.Value),
      
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "dbo.BookMasterPage",
                objParam
            );
        }

        public DataSet BookAuthor(string action, int bookId, int? authorId = null)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
                    new SqlParameter("@Action", action),
                    new SqlParameter("@BookID", bookId),
                    new SqlParameter("@AuthorID", authorId),

            };
            return SqlHelper.ExecuteDataset(objSqlConnection,
                       CommandType.StoredProcedure, "dbo.BookAuthorMapping", objParam);
        }
        public DataSet BookPurchaseLog_Insert(int BookID, decimal Price, int Quantity,bool TaxIncluded,decimal TaxPercentage,
            decimal TaxAmount,  decimal FinalAmount, string Receipt,  int AdminUserID,string UploadedFileName)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@BookID", BookID),
        new SqlParameter("@Price", Price),
        new SqlParameter("@Quantity", Quantity),
        new SqlParameter("@TaxIncluded", TaxIncluded),
        new SqlParameter("@TaxPercentage", TaxPercentage),
        new SqlParameter("@TaxAmount", TaxAmount),
        new SqlParameter("@FinalAmount", FinalAmount),
       new SqlParameter("@Receipt", (object)Receipt ?? DBNull.Value),
        new SqlParameter("@AdminUserID", AdminUserID),
        new SqlParameter("@UploadedFileName",UploadedFileName)
            };

            return SqlHelper.ExecuteDataset( objSqlConnection,  CommandType.StoredProcedure, "BookPurchaseLog_Insert",objParam );
        }
       

        public DataTable BulkInsertBooks(DataTable bulkDt, int adminUserID)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", "BULKINSERT"),
        new SqlParameter("@AdminUserID", adminUserID),

        new SqlParameter
        {
            ParameterName = "@Books",
            SqlDbType = SqlDbType.Structured,
            TypeName = "dbo.BookMasterBulkType",
            Value = bulkDt
        }
            };

            DataTable dtResult = new DataTable();

            using (SqlCommand cmd = new SqlCommand("dbo.BookMasterPage", objSqlConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(objParam);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtResult); // ✅ works without opening connection manually
                }
            }

            return dtResult;
        }

        public DataTable GetConfigValues(string action, string configName)
        {
            SqlParameter[] objparam = new SqlParameter[]
            {
        new SqlParameter("@ActionType", action),
        new SqlParameter("@ConfigName", configName)
            };

            DataSet ds = SqlHelper.ExecuteDataset(objSqlConnection,
                    CommandType.StoredProcedure, "ConfigMaster", objparam);

            return ds.Tables[0];
        }

        public int DeleteReceipt(int receiptId, int adminUserId)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", "DELETERECEIPT"),
        new SqlParameter("@ReceiptID", receiptId),
        new SqlParameter("@AdminUserID", adminUserId)
            };

            DataSet ds = SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "dbo.BookMasterPage",
                objParam
            );

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return Convert.ToInt32(ds.Tables[0].Rows[0]["MsgCode"]);

            return 0;
        }
        public DataSet BulkReceiptUpload(DataTable dtBulkReceipts, int adminUserID)
        {
            SqlParameter[] objParam = new SqlParameter[]
            {
        new SqlParameter("@Action", "BULKRECEIPTUPLOAD"),
        new SqlParameter("@AdminUserID", adminUserID),

        new SqlParameter
        {
            ParameterName = "@BulkReceipts",
            SqlDbType = SqlDbType.Structured,
            TypeName = "dbo.BookReceiptBulkType",
            Value = dtBulkReceipts
        }
            };

            return SqlHelper.ExecuteDataset(
                objSqlConnection,
                CommandType.StoredProcedure,
                "BookMasterPage",
                objParam
            );
        }
    }
}