using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Data.OleDb;

namespace FCStore.Common
{
    public static class PubFunction
    {
        public const string LONGDATETIMEFORMAT = "yyyy-MM-dd hh:mm:ss";

        public static string BuildResult(object content,string customJsonStr = null, bool successTag = true, int errCode = 0, string errStr = "")
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
            if(width != originBmp.Width || height != originBmp.Height)
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
                        type.InvokeMember(pi.Name,BindingFlags.SetProperty,null, obj, new object[] {""});
                }
            }
        }

        public static void CopyObj<T>(T source,ref T destination)
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

        
        public static byte[] ObjectArrSaveToXMLFile<T>(T[] objArr,string XMLFP)
        {
            if(File.Exists(XMLFP))
                File.Delete(XMLFP);
            OfficeOpenXml.ExcelPackage ep = new OfficeOpenXml.ExcelPackage();
            OfficeOpenXml.ExcelWorkbook wb = ep.Workbook;  
            OfficeOpenXml.ExcelWorksheet ws = wb.Worksheets.Add("MySheet");
            //配置文件属性
            wb.Properties.Category = "商品数据";
            wb.Properties.Author = "RightGO";
            wb.Properties.Comments = "导出的商品数据";
            wb.Properties.Company = "RightGO";
            wb.Properties.Keywords = "Product";
            wb.Properties.Manager = "Fionhuo";
            wb.Properties.Status = "内容状态";
            wb.Properties.Subject = "主题";
            wb.Properties.Title = "标题";
            wb.Properties.LastModifiedBy = "最后一次保存者";
            //写数据
            ws.Cells[1, 1].Value = "Hello";
            ws.Column(1).Width = 40;//修改列宽
            ws.Cells["B1"].Value = "World";
            ws.Cells[3, 3, 3, 5].Merge = true;
            ws.Cells[3, 3].Value = "Cells[3, 3, 3, 5]合并";
            ws.Cells["A4:D5"].Merge = true;
            ws.Cells["A4"].Value = "Cells[\"A4:D5\"]合并";
            ////写到客户端（下载）
            //Response.Clear();
            //Response.AddHeader("content-disposition", "attachment; filename=FileFlow.xls");
            //Response.ContentType ="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.BinaryWrite(ep.GetAsByteArray());
            ////ep.SaveAs(Response.OutputStream); 第二种方式
            //Response.Flush();
            //Response.End();
            return ep.GetAsByteArray();
        }
    }
}