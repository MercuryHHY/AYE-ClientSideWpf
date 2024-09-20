using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using System.Data;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI;
using SqlSugar;

namespace AYE_Commom.Helper
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 将Excel转化成DataTable
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataTable ReadExcelToTable(string fileName, Stream stream)
        {
            using (stream)
            {
                IWorkbook workbook = null;
                if (fileName.EndsWith(".xlsx"))// 2007版本
                {
                    workbook = new XSSFWorkbook(stream);
                }
                else// 2003版本
                {
                    workbook = new HSSFWorkbook(stream);
                }
                //var workbook = new XSSFWorkbook(tempFile);
                var sheet = workbook.GetSheetAt(0);
                var dataTable = new DataTable();
                var tableHeadRow = sheet.GetRow(0);
                for (int i = 0; i < tableHeadRow.PhysicalNumberOfCells; i++)
                {
                    var headCell = tableHeadRow.Cells[i];
                    dataTable.Columns.Add(new DataColumn(headCell.StringCellValue));
                }
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    var newRow = dataTable.NewRow();
                    // for (int j = 0; j < row.PhysicalNumberOfCells; j++)
                    for (int j = 0; j < tableHeadRow.PhysicalNumberOfCells; j++)
                    {
                        if (row.GetCell(j) == null)
                        {
                            continue;
                        }
                        //var cell = row.Cells[j];
                        var cell = row.GetCell(j);
                        if (cell.CellType == CellType.String)
                        {
                            newRow[j] = cell.StringCellValue.Trim();
                        }
                        else if (cell.CellType == CellType.Numeric)
                        {
                            newRow[j] = cell.NumericCellValue;
                        }
                        else if (cell.CellType == CellType.Formula)
                        {
                            try
                            {
                                newRow[j] = cell.StringCellValue.ToString().Trim();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            newRow[j] = cell.StringCellValue;
                        }
                    }
                    dataTable.Rows.Add(newRow);
                }
                // workbook.Clear();
                workbook.Close();
                return dataTable;
            }
        }

        /// <summary>
        /// 将Excel转化成DataTable
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataTable ReadExcelToTable(string fileName, Stream stream, string sheetName)
        {
            using (stream)
            {
                IWorkbook workbook = null;
                if (fileName.EndsWith(".xlsx"))// 2007版本
                {
                    workbook = new XSSFWorkbook(stream);
                }
                else// 2003版本
                {
                    workbook = new HSSFWorkbook(stream);
                }
                //var workbook = new XSSFWorkbook(tempFile);
                //var sheet = workbook.GetSheetAt(0);
                var sheet = workbook.GetSheet(sheetName);
                var dataTable = new DataTable();
                if (sheet == null)
                {
                    return dataTable;
                }

                var tableHeadRow = sheet.GetRow(0);
                for (int i = 0; i < tableHeadRow.PhysicalNumberOfCells; i++)
                {
                    var headCell = tableHeadRow.Cells[i];
                    dataTable.Columns.Add(new DataColumn(headCell.StringCellValue));
                }
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    var newRow = dataTable.NewRow();
                    // for (int j = 0; j < row.PhysicalNumberOfCells; j++)
                    for (int j = 0; j < tableHeadRow.PhysicalNumberOfCells; j++)
                    {
                        if (row.GetCell(j) == null)
                        {
                            continue;
                        }
                        //var cell = row.Cells[j];
                        var cell = row.GetCell(j);
                        if (cell.CellType == CellType.String)
                        {
                            newRow[j] = cell.StringCellValue.Trim();
                        }
                        else if (cell.CellType == CellType.Numeric)
                        {
                            newRow[j] = cell.NumericCellValue;
                        }
                        else if (cell.CellType == CellType.Formula)
                        {
                            try
                            {
                                newRow[j] = cell.StringCellValue.ToString().Trim();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            newRow[j] = cell.StringCellValue;
                        }
                    }
                    dataTable.Rows.Add(newRow);
                }
                // workbook.Clear();
                workbook.Close();
                return dataTable;
            }
        }

        /// <summary>
        /// 将datatable转化为Excel
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string DataTableToExcel(DataTable dataTable, string filePath)
        {
            var dt = dataTable;
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                var workBook = new XSSFWorkbook();
                var sheet = workBook.CreateSheet();

                IRow headRow = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = headRow.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i] == null ? "" : dt.Columns[i].ToString());
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        IRow newRow = sheet.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ICell cell = newRow.CreateCell(j);
                            cell.SetCellValue(dt.Rows[i][j] == null ? "" : dt.Rows[i][j].ToString());
                        }
                    }
                }
                workBook.Write(fileStream);
                workBook.Clear();
                workBook.Close();
            }
            return filePath;
        }

        /// <summary>
        /// 创建Excel文件，并返回IWorkbook
        /// 一般与下面的方法配合使用，
        /// 先创建，然后用下面的函数读取Excel数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IWorkbook CreateWorkbook(Stream stream)
        {
            return WorkbookFactory.Create(stream);
        }


        /// <summary>
        /// 将Excel转化成DataTable
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataTable ReadExcelToTable(IWorkbook workbook, string sheetName)
        {
            var sheet = workbook.GetSheet(sheetName);
            var dataTable = new DataTable();
            if (sheet == null)
            {
                return dataTable;
            }

            var tableHeadRow = sheet.GetRow(0);
            for (int i = 0; i < tableHeadRow.PhysicalNumberOfCells; i++)
            {
                var headCell = tableHeadRow.Cells[i];
                dataTable.Columns.Add(new DataColumn(headCell.StringCellValue));
            }

            for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                var newRow = dataTable.NewRow();
                for (int j = 0; j < tableHeadRow.PhysicalNumberOfCells; j++)
                {
                    if (row.GetCell(j) == null)
                    {
                        continue;
                    }
                    var cell = row.GetCell(j);
                    if (cell.CellType == CellType.String)
                    {
                        newRow[j] = cell.StringCellValue.Trim();
                    }
                    else if (cell.CellType == CellType.Numeric)
                    {
                        newRow[j] = cell.NumericCellValue;
                    }
                    else if (cell.CellType == CellType.Formula)
                    {
                        try
                        {
                            newRow[j] = cell.StringCellValue.ToString().Trim();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        newRow[j] = cell.StringCellValue;
                    }
                }
                dataTable.Rows.Add(newRow);
            }
            return dataTable;
        }
   
    
    }

}