using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace FCStore.Common
{
    public static class LogHelper
    {
        public static string CurrentPath = "";

        private static Dictionary<string, FileStream> m_fsdict = new Dictionary<string, FileStream>();



        public static void Log(string message,string type,bool includeDT = true)
        {
            if(!m_fsdict.ContainsKey(type))
            {
                FileStream tmpFS = File.Open(CurrentPath + type + ".fclog",FileMode.OpenOrCreate);
                m_fsdict.Add(type, tmpFS);

            }
            byte[] tmpBuf = Encoding.Unicode.GetBytes((includeDT ? DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ":" : "") + message);
            m_fsdict[type].Write(tmpBuf, 0, tmpBuf.Length);
        }

        public static void Dispose()
        {
            foreach(KeyValuePair<string,FileStream> kvItem in m_fsdict)
            {
                kvItem.Value.Close();
            }
        }
    }
}