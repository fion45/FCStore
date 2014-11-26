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
using System.Net;


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

        public static string GetWebPageByGet(string geturl)
        {
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(geturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }

        public static string GetWebPageByPost(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }
    }
}