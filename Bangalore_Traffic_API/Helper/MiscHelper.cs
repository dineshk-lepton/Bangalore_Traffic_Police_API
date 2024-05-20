using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class MiscHelper
{
    public static string FormatDate(string date)
    {
        var result = string.Empty;
        if (!string.IsNullOrEmpty(date))
        {
            result = Convert.ToDateTime(date).ToString("dd-MMM-yyyy");
        }
        return result;
    }

    public static string FormatDateTime(string date)
    {
        var result = string.Empty;
        if (!string.IsNullOrEmpty(date))
        {
            result = Convert.ToDateTime(date).ToString("dd-MMM-yy hh:mm tt");
        }
        return result;
    }
      

    public static DataTable ListToDataTable<T>(IList<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);

        //Get all the properties
        if (items != null && items.Count > 0)
        {
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name.ToUpper(), type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }

   

    public static void GetExcelRowValuesData(StringBuilder sb, DataColumnCollection dtColumnNames, DataRow row)
    {
        foreach (DataColumn col in dtColumnNames)
        {
            if (sb.ToString() == string.Empty)
                sb.Append("'" + row[col].ToString().Trim().Replace("'", "''") + "'");
            else
                sb.Append(",'" + row[col].ToString().Trim().Replace("'", "''") + "'");
        }
    }

    public static List<T> ConvertDataTableToList<T>(DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem<T>(row);
            data.Add(item);
        }
        return data;
    }
    private static T GetItem<T>(DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                if (pro.Name == column.ColumnName)
                    pro.SetValue(obj, dr[column.ColumnName], null);
                else
                    continue;
            }
        }
        return obj;
    }


    public static string EncodeTo64(string strText)
    {

        byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(strText);

        string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

        return returnValue;


    }

    public static string DecodeTo64(string strText)
    {
        byte[] toDecodeAsBytes = Convert.FromBase64String(strText);
        string returnValue = Encoding.UTF8.GetString(toDecodeAsBytes);
        return returnValue;
    }



    public static void CopyMatchingProperties(object source, object destination)
    {
        FieldInfo[] fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Concat(source.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            .Concat(source.GetType().BaseType.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();

        foreach (FieldInfo fi in fields)
        {
            fi.SetValue(destination, fi.GetValue(source));
        }
    }
    public static void CopyMatchingBaseProperties(object source, object destination)
    {


        FieldInfo[] fields = source.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToArray();

        foreach (FieldInfo fi in fields)
        {
            fi.SetValue(destination, fi.GetValue(source));
        }
    }


    public static DataTable GetDataTableFromDictionaries<T>(List<Dictionary<string, T>> list)
    {
        DataTable dataTable = new DataTable();

        if (list == null || !list.Any()) return dataTable;

        foreach (var column in list.First().Select(c => new DataColumn(c.Key, typeof(T))))
        {
            dataTable.Columns.Add(column);
        }

        foreach (var row in list.Select(
            r =>
            {
                var dataRow = dataTable.NewRow();
                r.ToList().ForEach(c => dataRow.SetField(c.Key, c.Value));
                return dataRow;
            }))
        {
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    /// <summary>
    ///  Clean the directory before Upload the new shape file
    /// </summary>
    public static void CleanDirectorypath(string Dirpath, int user_id)
    {
        try
        {
            string[] filePaths = Directory.GetFiles(Dirpath);
            string fileName = string.Empty;

            foreach (string filePath in filePaths)
            {
                fileName = Path.GetFileName(filePath);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                if (fileName.Substring(fileName.LastIndexOf('_') + 1) == Convert.ToString(user_id))
                {
                    System.IO.File.Delete(filePath);
                }

            }
        }
        catch { }
    }


    public void CopyFile(string srcDirPath, string destDirPath, string srcFileName, string destFileName)
    {
        try
        {
            if (!Directory.Exists(destDirPath)) { Directory.CreateDirectory(destDirPath); }
            File.Copy(string.Concat(srcDirPath, "\\", srcFileName), string.Concat(destDirPath, "\\", destFileName));
        }
        catch { throw; }
    }
    //public void BindPortDetails(dynamic objMaster, string enType, string ddlType)
    //{
    //    var layerdetails = new BLLayer().getLayer(enType);
    //    if (layerdetails != null)
    //    {
    //        objMaster.unit_input_type = layerdetails.unit_input_type;
    //        if (layerdetails.unit_input_type == UnitInputType.iopddl.ToString())
    //        {
    //            var objResp = new BLMisc().GetDropDownList(enType, ddlType);
    //            objMaster.lstIOPDetails = objResp;
    //        }
    //    }
    //}

   
    public static string Encrypt(string clearText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }
    public static string Decrypt(string cipherText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    public static string getTimeStamp()
    {
        return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
    }
    public static string getPortName(int portNo)
    {
        return portNo.ToString().Substring(0, 1) == "-" ? portNo.ToString().Replace("-", "") + " IN" : portNo + " OUT";
    }
  
    public static String BytesToString(long byteCount)
    {
        string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0 " + suf[1];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
    }

    // Convert list to datatable
    public DataTable ToDataTable<T>(List<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);

        // Get all properties of the class
        var properties = typeof(T).GetProperties();

        // Create columns in DataTable for each property
        foreach (var property in properties)
        {
            dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        // Populate DataTable with data from the list
        foreach (var item in items)
        {
            DataRow row = dataTable.NewRow();
            foreach (var property in properties)
            {
                row[property.Name] = property.GetValue(item) ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
    public static void RemoveAllNullRowsFromDataTable(ref DataTable dt)
    {
        if (dt != null)
        {
            int columnCount = dt.Columns.Count;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                bool allNull = true;
                for (int j = 0; j < columnCount; j++)
                {
                    if (dt.Rows[i][j] != DBNull.Value)
                        allNull = false;
                }
                if (allNull)
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }
    }
  
}
