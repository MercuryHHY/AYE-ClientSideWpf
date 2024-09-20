using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Commom.Helper;

public class CsvHelper
{
    /// <summary>
    /// 数据保存在CSV文件中，如果存在就续写，不存在则创建
    /// </summary>
    /// <param name="key">CSV文件头</param>
    /// <param name="value">要写入的字符串，数据之间用英文逗号隔开</param>
    /// <param name="strDirPath"> 要写入的文件路径</param>
    /// <param name="strFileName">文件名</param>
    /// <param name="bHasTitle">是否写入标题（也就是CSV文件头）</param>
    /// <returns></returns>
    public static bool CsvWriteFile(string key, string value, string strDirPath, string strFileName, bool bHasTitle)
    {
        try
        {
            if (!Directory.Exists(strDirPath))
            {
                Directory.CreateDirectory(strDirPath);
            }

            string path = Path.Combine(strDirPath, strFileName);
            bool flag = !File.Exists(path);
            using (StreamWriter streamWriter = new StreamWriter(path, append: true, Encoding.UTF8))
            {
                if (bHasTitle && flag)
                {
                    streamWriter.WriteLine(key);
                }

                streamWriter.WriteLine(value);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static DataTable OpenCSV(string filePath, out string strError)
    {
        strError = "";
        try
        {
            DataTable dataTable = new DataTable();
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
            string text = "";
            string[] array = null;
            string[] array2 = null;
            int num = 0;
            bool flag = true;
            while ((text = streamReader.ReadLine()) != null)
            {
                if (flag)
                {
                    array2 = text.Split(',');
                    flag = false;
                    num = array2.Length;
                    for (int i = 0; i < num; i++)
                    {
                        DataColumn column = new DataColumn(array2[i]);
                        dataTable.Columns.Add(column);
                    }
                }
                else
                {
                    array = text.Split(',');
                    DataRow dataRow = dataTable.NewRow();
                    for (int j = 0; j < num; j++)
                    {
                        dataRow[j] = array[j].Trim();
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            streamReader.Close();
            fileStream.Close();
            return dataTable;
        }
        catch (Exception ex)
        {
            strError = ex.ToString();
            return null;
        }
    }

    public static bool DataTableToCSV(string filePath, DataTable dt, out string strError)
    {
        strError = "";
        try
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            using (StreamWriter streamWriter = new StreamWriter(filePath, append: false, Encoding.UTF8))
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i != 0)
                    {
                        stringBuilder.Append(",");
                    }

                    stringBuilder.Append(dt.Columns[i].ColumnName);
                }

                streamWriter.WriteLine(stringBuilder);
                stringBuilder.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j != 0)
                        {
                            stringBuilder.Append(",");
                        }

                        if (dt.Columns[j].DataType == typeof(string) && row[dt.Columns[j]].ToString().Contains(","))
                        {
                            stringBuilder.Append("\"" + row[dt.Columns[j]].ToString().Replace("\"", "\"\"") + "\"");
                        }
                        else
                        {
                            stringBuilder.Append(row[dt.Columns[j]].ToString());
                        }
                    }

                    streamWriter.WriteLine(stringBuilder);
                    stringBuilder.Clear();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            strError = ex.Message;
            return false;
        }
    }
}
