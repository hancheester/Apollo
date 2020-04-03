using System.Data;
using System.Data.OleDb;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class ExcelUtility
    {
        private const string EXCEL9703CONNSTRING = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties='Excel 8.0;HDR={1}'";
        private const string EXCEL07CONNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 8.0;HDR={1}'";

        public DataTable PrepareDataTable(string filePath, string tableName, string extension, bool firstRowIsColumnNames)
        {
            string conStr = string.Empty;
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    //conStr = EXCEL9703CONNSTRING;
                    conStr = EXCEL07CONNSTRING;
                    break;
                case ".xlsx": //Excel 07
                    conStr = EXCEL07CONNSTRING;
                    break;
            }

            conStr = string.Format(conStr, filePath, firstRowIsColumnNames ? "YES" : "NO");
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            dt.TableName = tableName;
            cmdExcel.Connection = connExcel;

            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            connExcel.Close();

            //Read Data from First Sheet
            connExcel.Open();
            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
            oda.SelectCommand = cmdExcel;
            oda.Fill(dt);
            connExcel.Close();
            return dt;
        }
    }
}