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

        
        public static bool ObjectArrSaveToXMLFile<T>(T[] objArr,string XMLFP)
        {
            try
            {
                StringBuilder resultStr = new StringBuilder("<table style='background-color:#0066CC;'><tr style='background-color:Black;color:white;'>");
                Type objType = typeof(T);
                MemberInfo[] miArray = objType.GetMembers();
                List<MemberInfo> miLST = new List<MemberInfo>();
                foreach (MemberInfo mi in miArray)
                {
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        bool RecordTag = true;
                        ReadOnlyCollection<CustomAttributeData> CACollection = mi.CustomAttributes as ReadOnlyCollection<CustomAttributeData>;
                        if (CACollection != null)
                        {
                            foreach(CustomAttributeData attr in CACollection)
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
                            resultStr.Append("<th>" + mi.Name + "</th>");
                            miLST.Add(mi);
                        }
                    }
                }
                foreach (T obj in objArr)
                {
                    resultStr.Append("<tr>");
                    foreach (MemberInfo mi in miLST)
                    {
                        try
                        {
                            resultStr.Append("<td>" + resultStr.Append(objType.InvokeMember(mi.Name, BindingFlags.GetProperty, null, obj, null).ToString()) + "</td>");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    resultStr.Append("</tr>");
                }
                resultStr.Append("</table>");
                byte[] tmpBuffer = UnicodeEncoding.Unicode.GetBytes(resultStr.ToString());
                FileStream tmpFS = new FileStream(XMLFP, FileMode.CreateNew);
                tmpFS.Write(tmpBuffer, 0, tmpBuffer.Length);
                tmpFS.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}