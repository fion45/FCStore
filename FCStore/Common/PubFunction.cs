using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
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
    }
}