using System.Text;

namespace FCStore.Common
{
    public static class PubFunction
    {
        public static string BuildResult(object content, bool successTag = true, int errCode = 0, string errStr = "")
        {
            return string.Format("{{\"content\":{0},\"successTag\":{1},\"errCode\":{2},\"errStr\":\"{3}\"}}", content != null ? Newtonsoft.Json.JsonConvert.SerializeObject(content) : "null", successTag ? "true" : "false", errCode, errStr); 
        }
    }
}