using System.Text;

namespace FCStore.Common
{
    public static class PubFunction
    {
        public static string BuildResult(object content,string customJsonStr = null, bool successTag = true, int errCode = 0, string errStr = "")
        {
            return string.Format("{{\"content\":{0},{1}\"successTag\":{2},\"errCode\":{3},\"errStr\":\"{4}\"}}", content != null ? Newtonsoft.Json.JsonConvert.SerializeObject(content) : "null", customJsonStr == null ? "" : customJsonStr + ",", successTag ? "true" : "false", errCode, errStr); 
        }
    }
}