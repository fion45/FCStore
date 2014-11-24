using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FCStore.Models;
using System.Data.OleDb;
using OfficeOpenXml;
using OfficeOpenXml.Table;


namespace FCStore.Common
{
    public static class PubFunction
    {
        public const string LONGDATETIMEFORMAT = "yyyy-MM-dd hh:mm:ss";

        public static string BuildResult(object content, string customJsonStr = null, bool successTag = true, int errCode = 0, string errStr = "")
        {
            return string.Format("{{\"content\":{0},\"custom\":{1},\"successTag\":{2},\"errCode\":{3},\"errStr\":\"{4}\"}}", content != null ? Newtonsoft.Json.JsonConvert.SerializeObject(content) : "null", string.IsNullOrEmpty(customJsonStr) ? "null" : customJsonStr, successTag ? "true" : "false", errCode, errStr);
        }

        public static string CHPriceFormat(decimal price)
        {
            string result = price.ToString();
            int index = result.IndexOf('.');
            if (index > 0)
            {
                result = "￥" + result.Substring(0, index) + result.Substring(index, 3);
            }
            else
            {
                result = "￥" + result + ".00";
            }
            return result;
        }

        public static void SaveImg(Bitmap originBmp, int width, int height, string saveFP)
        {
            if (width != originBmp.Width || height != originBmp.Height)
            {
                Bitmap resizedBmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(resizedBmp);
                //设置高质量插值法   
                g.InterpolationMode = InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度   
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                //消除锯齿 
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(originBmp, new Rectangle(0, 0, width, height), new Rectangle(0, 0, originBmp.Width, originBmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                resizedBmp.Save(saveFP);
            }
            else
            {
                originBmp.Save(saveFP);
            }
        }

        public static void NotNullObj<T>(ref T obj)
        {
            Type type = typeof(T);
            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    object tmpObj = type.InvokeMember(pi.Name, BindingFlags.GetProperty, null, obj, null);
                    if (tmpObj == null)
                        type.InvokeMember(pi.Name, BindingFlags.SetProperty, null, obj, new object[] { "" });
                }
            }
        }

        public static void CopyObj<T>(T source, ref T destination)
        {
            Type type = typeof(T);
            foreach (PropertyInfo pi in type.GetProperties())
            {
                object tmpObj = type.InvokeMember(pi.Name, BindingFlags.GetProperty, null, source, null);
                type.InvokeMember(pi.Name, BindingFlags.SetProperty, null, destination, new object[] { tmpObj });
            }
        }

        public static string GetUploadFilePathUsingDate()
        {
            string serverFP = "";
            serverFP = "~/Uploads/" + (DateTime.Now.Year % 100).ToString() + "/";
            string numStr = "0" + DateTime.Now.Month.ToString();
            numStr = numStr.Substring(numStr.Length - 2, 2);
            serverFP += numStr + "/";
            numStr = "0" + DateTime.Now.Day.ToString();
            numStr = numStr.Substring(numStr.Length - 2, 2);
            serverFP += numStr + "/";
            return serverFP;
        }

        /// <summary>
        /// 保存数据列表到Excel（泛型）
        /// </summary>
        /// <typeparam name="T">集合数据类型</typeparam>
        /// <param name="data">数据列表</param>
        /// <param name="dict">生成字符串时的回调函数</param>
        public static byte[] SaveToExcel<T>(IEnumerable<T> data, Dictionary<string, MemberToStringDG> dict = null)
        {
            try
            {
                using(ExcelPackage ep = new ExcelPackage())
                {
                    ExcelWorkbook wb = ep.Workbook;
                    ExcelWorksheet ws = wb.Worksheets.Add("MySheet");
                    Type objType = typeof(T);
                    PropertyInfo[] piArray = objType.GetProperties();
                    List<PropertyInfo> piLST = new List<PropertyInfo>();
                    int rowIndex = 0;
                    int colIndex = 0;
                    foreach (PropertyInfo pi in piArray)
                    {
                        bool RecordTag = true;
                        ReadOnlyCollection<CustomAttributeData> CACollection = pi.CustomAttributes as ReadOnlyCollection<CustomAttributeData>;
                        if (CACollection != null)
                        {
                            foreach (CustomAttributeData attr in CACollection)
                            {
                                if (attr.AttributeType.FullName.IndexOf(".NotMappedAttribute") > -1 || attr.AttributeType.FullName.IndexOf(".JsonIgnoreAttribute") > -1)
                                {
                                    RecordTag = false;
                                    break;
                                }
                            }
                        }
                        if (RecordTag)
                        {
                            ws.Cells[rowIndex + 1, colIndex + 1].Value = pi.Name;
                            piLST.Add(pi);
                            ++colIndex;
                        }
                    }
                    ++rowIndex;
                    foreach (T item in data)
                    {
                        colIndex = 0;
                        foreach (PropertyInfo pi in piLST)
                        {
                            try
                            {
                                string tmpStr = "";
                                object tmpObj = pi.GetValue(item);
                                if (dict.ContainsKey(pi.Name))
                                {
                                    tmpStr = dict[pi.Name](tmpObj);
                                }
                                else
                                {
                                    tmpStr = tmpObj.ToString();
                                }
                                ws.Cells[rowIndex + 1, colIndex + 1].Value = tmpStr;
                            }
                            catch (Exception ex)
                            {

                            }
                            ++colIndex;
                        }
                        ++rowIndex;
                    }
                    return ep.GetAsByteArray();
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 从Excel中加载数据（泛型）
        /// </summary>
        /// <typeparam name="T">每行数据的类型</typeparam>
        /// <param name="FileName">Excel文件名</param>
        /// <returns>泛型列表</returns>
        public static IEnumerable<T> LoadFromExcel<T>(string FileName, Dictionary<string, StringToMemberDG> dict = null) where T : new()
        {
            FileInfo existingFile = new FileInfo(FileName);
            List<T> resultList = new List<T>();
            Dictionary<string, int> dictHeader = new Dictionary<string, int>();

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int colStart = worksheet.Dimension.Start.Column;  //工作区开始列
                int colEnd = worksheet.Dimension.End.Column;       //工作区结束列
                int rowStart = worksheet.Dimension.Start.Row;       //工作区开始行号
                int rowEnd = worksheet.Dimension.End.Row;       //工作区结束行号

                //将每列标题添加到字典中
                for (int i = colStart; i <= colEnd; i++)
                {
                    dictHeader[worksheet.Cells[rowStart, i].Value.ToString()] = i;
                }

                List<PropertyInfo> propertyInfoList = new List<PropertyInfo>(typeof(T).GetProperties());

                for (int row = rowStart + 1; row <= rowEnd; row++)
                {
                    T result = new T();

                    //为对象T的各属性赋值
                    foreach (PropertyInfo p in propertyInfoList)
                    {
                        try
                        {
                            ExcelRange cell = worksheet.Cells[row, dictHeader[p.Name]]; //与属性名对应的单元格

                            if (cell.Value == null)
                                continue;
                            switch (p.PropertyType.Name.ToLower())
                            {
                                case "string":
                                    p.SetValue(result, cell.Value);
                                    break;
                                case "int16":
                                    p.SetValue(result, short.Parse(cell.Value.ToString()));
                                    break;
                                case "int32":
                                    p.SetValue(result, int.Parse(cell.Value.ToString()));
                                    break;
                                case "int64":
                                    p.SetValue(result, Int64.Parse(cell.Value.ToString()));
                                    break;
                                case "decimal":
                                    p.SetValue(result, decimal.Parse(cell.Value.ToString()));
                                    break;
                                case "double":
                                    p.SetValue(result, double.Parse(cell.Value.ToString()));
                                    break;
                                case "datetime":
                                    p.SetValue(result, DateTime.Parse(cell.Value.ToString()));
                                    break;
                                case "boolean":
                                    p.SetValue(result, Boolean.Parse(cell.Value.ToString()));
                                    break;
                                case "byte":
                                    p.SetValue(result, byte.Parse(cell.Value.ToString()));
                                    break;
                                case "char":
                                    p.SetValue(result, cell.Value.ToString()[0]);
                                    break;
                                case "single":
                                    p.SetValue(result, Single.Parse(cell.Value.ToString()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (KeyNotFoundException ex)
                        { }
                    }
                    resultList.Add(result);
                }
            }
            return resultList;
        } 
    }
}





        //public static T[] LoadExcelFileToObjectArr<T>(string ExcelFP, Dictionary<string, StringToMemberDG> dict) where T : new()
        //{
        //    if (!File.Exists(ExcelFP))
        //    {
        //        return null;
        //    }
        //    FileInfo tmpFI = new FileInfo(ExcelFP);
        //    OfficeOpenXml.ExcelPackage ep = new OfficeOpenXml.ExcelPackage(tmpFI);
        //    OfficeOpenXml.ExcelWorkbook wb = ep.Workbook;
        //    OfficeOpenXml.ExcelWorksheet ws = wb.Worksheets["MySheet"];
        //    int colStart = ws.Dimension.Start.Column;   //工作区开始列
        //    int colEnd = ws.Dimension.End.Column;       //工作区结束列
        //    int rowStart = ws.Dimension.Start.Row;      //工作区开始行号
        //    int rowEnd = ws.Dimension.End.Row;          //工作区结束行号
        //    T[] result = new T[rowEnd - rowStart - 1];
        //    //将每列标题添加到字典中
        //    Dictionary<string, int> dictHeader = new Dictionary<string, int>();
        //    for (int i = colStart; i <= colEnd; i++)
        //    {
        //        dictHeader[ws.Cells[rowStart, i].Value.ToString()] = i;
        //    }
        //    Type objType = typeof(T);
        //    MemberInfo[] miArray = objType.GetMembers();
        //    List<MemberInfo> miLST = new List<MemberInfo>();
        //    foreach (MemberInfo mi in miArray)
        //    {
        //        if (mi.MemberType == MemberTypes.Property)
        //        {
        //            bool RecordTag = true;
        //            ReadOnlyCollection<CustomAttributeData> CACollection = mi.CustomAttributes as ReadOnlyCollection<CustomAttributeData>;
        //            if (CACollection != null)
        //            {
        //                foreach (CustomAttributeData attr in CACollection)
        //                {
        //                    if (attr.AttributeType.FullName.IndexOf(".NotMappedAttribute") > -1 || attr.AttributeType.FullName.IndexOf(".JsonIgnoreAttribute") > -1)
        //                    {
        //                        RecordTag = false;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (RecordTag)
        //            {
        //                miLST.Add(mi);
        //            }
        //        }
        //    }

        //    int index = 0;
        //    for (int row = rowStart + 1; row < rowEnd; row++)
        //    {
        //        T item = new T();
        //        //为对象T的各属性赋值
        //        foreach (MemberInfo mi in miLST)
        //        {
        //            try
        //            {
        //                OfficeOpenXml.ExcelRange cell = ws.Cells[row, dictHeader[mi.Name]]; //与属性名对应的单元格
        //                object tmpVal = null;
        //                if (cell.Value == null)
        //                    continue;
        //                PropertyInfo pi = mi as PropertyInfo;
        //                if (pi == null)
        //                    continue;
        //                switch (pi.PropertyType.Name.ToLower())
        //                {
        //                    case "string":
        //                        tmpVal = cell.GetValue<String>();
        //                        break;
        //                    case "int16":
        //                        tmpVal = cell.GetValue<Int16>();
        //                        break;
        //                    case "int32":
        //                        tmpVal = cell.GetValue<Int32>();
        //                        break;
        //                    case "int64":
        //                        tmpVal = cell.GetValue<Int64>();
        //                        break;
        //                    case "decimal":
        //                        tmpVal = cell.GetValue<Decimal>();
        //                        break;
        //                    case "double":
        //                        tmpVal = cell.GetValue<Double>();
        //                        break;
        //                    case "datetime":
        //                        tmpVal = cell.GetValue<DateTime>();
        //                        break;
        //                    case "boolean":
        //                        tmpVal = cell.GetValue<Boolean>();
        //                        break;
        //                    case "byte":
        //                        tmpVal = cell.GetValue<Byte>();
        //                        break;
        //                    case "char":
        //                        tmpVal = cell.GetValue<Char>();
        //                        break;
        //                    case "single":
        //                        tmpVal = cell.GetValue<Single>();
        //                        break;
        //                    case "list":
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                objType.InvokeMember(mi.Name, BindingFlags.SetProperty, null, item, new object[] { tmpVal });
        //            }
        //            catch (KeyNotFoundException ex)
        //            {
        //            }
        //        }
        //        result[index++] = item;
        //    }
        //    return result;
        //}

        //public static byte[] ObjectArrSaveToExcelFile<T>(T[] objArr, string ExcelFP, Dictionary<string, MemberToStringDG> dict)
        //{
        //    if (File.Exists(ExcelFP))
        //        File.Delete(ExcelFP);
        //    OfficeOpenXml.ExcelPackage ep = new OfficeOpenXml.ExcelPackage();
        //    OfficeOpenXml.ExcelWorkbook wb = ep.Workbook;
        //    OfficeOpenXml.ExcelWorksheet ws = wb.Worksheets.Add("MySheet");
        //    //配置文件属性
        //    wb.Properties.Category = "商品数据";
        //    wb.Properties.Author = "RightGO";
        //    wb.Properties.Comments = "导出的商品数据";
        //    wb.Properties.Company = "RightGO";
        //    wb.Properties.Keywords = "Product";
        //    wb.Properties.Manager = "Fionhuo";
        //    wb.Properties.Status = "内容状态";
        //    wb.Properties.Subject = "主题";
        //    wb.Properties.Title = "标题";
        //    wb.Properties.LastModifiedBy = "最后一次保存者";
        //    //写数据
        //    Type objType = typeof(T);
        //    MemberInfo[] miArray = objType.GetMembers();
        //    List<MemberInfo> miLST = new List<MemberInfo>();
        //    int rowIndex = 0;
        //    int colIndex = 0;
        //    foreach (MemberInfo mi in miArray)
        //    {
        //        if (mi.MemberType == MemberTypes.Property)
        //        {
        //            bool RecordTag = true;
        //            ReadOnlyCollection<CustomAttributeData> CACollection = mi.CustomAttributes as ReadOnlyCollection<CustomAttributeData>;
        //            if (CACollection != null)
        //            {
        //                foreach (CustomAttributeData attr in CACollection)
        //                {
        //                    if (attr.AttributeType.FullName.IndexOf(".NotMappedAttribute") > -1 || attr.AttributeType.FullName.IndexOf(".JsonIgnoreAttribute") > -1)
        //                    {
        //                        RecordTag = false;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (RecordTag)
        //            {
        //                ws.Cells[1, colIndex + 1].Value = mi.Name;
        //                miLST.Add(mi);
        //                ++colIndex;
        //            }
        //        }
        //    }
        //    ++rowIndex;
        //    foreach (T obj in objArr)
        //    {
        //        colIndex = 0;
        //        foreach (MemberInfo mi in miLST)
        //        {
        //            try
        //            {
        //                object tmpObj = objType.InvokeMember(mi.Name, BindingFlags.GetProperty, null, obj, null);
        //                string tmpStr = "";
        //                if (dict.ContainsKey(mi.Name))
        //                {
        //                    tmpStr = dict[mi.Name](tmpObj);
        //                }
        //                else
        //                {
        //                    Type memberType = tmpObj.GetType();
        //                    if (memberType.FullName.Contains("List"))
        //                    {
        //                        IEnumerable enumera = tmpObj as IEnumerable;
        //                        StringBuilder tmpSB = new StringBuilder(1024);
        //                        foreach (object item in enumera)
        //                        {
        //                            tmpSB.Append(item.ToString() + ",");
        //                        }
        //                        tmpStr = tmpSB.ToString();
        //                        tmpStr = tmpStr.TrimEnd(new char[] { ',' });
        //                    }
        //                    else
        //                    {
        //                        tmpStr = tmpObj.ToString();
        //                    }
        //                }
        //                ws.Cells[rowIndex + 1, colIndex + 1].Value = tmpStr;
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            ++colIndex;
        //        }
        //        ++rowIndex;
        //    }
        //    return ep.GetAsByteArray();
        //}