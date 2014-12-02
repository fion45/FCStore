using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Threading;

namespace FCStore.Common
{
    public static class LogHelper
    {
        public struct LogItem
        {
            public string logtype;
            public string filepath;
            public List<string> messagelst;
            public Thread thread;
            public AutoResetEvent trigger;
            public Mutex mutex;
        }

        public static string CurrentPath = "";

        private static Dictionary<string, LogItem> StructDict = new Dictionary<string, LogItem>();

        private static bool RunningTag = false;

        public static void Log(string message,string type = "debug",bool includeDT = true)
        {
            if(!RunningTag)
                RunningTag = true;
            if (!StructDict.ContainsKey(type))
            {
                LogItem logitem = new LogItem();
                logitem.logtype = type;
                logitem.filepath = CurrentPath + type + ".fclog";
                logitem.messagelst = new List<string>();
                logitem.thread = new Thread(Running);
                logitem.trigger = new AutoResetEvent(false);
                logitem.mutex = new Mutex();
                StructDict.Add(type, logitem);
                logitem.thread.Start(logitem);
            }
            //byte[] tmpBuf = Encoding.Unicode.GetBytes((includeDT ? DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ":" : "") + message);
            if (StructDict[type].mutex.WaitOne())
            {
                StructDict[type].messagelst.Add((includeDT ? DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ":" : "") + message);
                StructDict[type].mutex.ReleaseMutex();
            }
            StructDict[type].trigger.Set();
        }

        public static void Running(object obj)
        {
            LogItem logitem = (LogItem)obj;
            while(RunningTag)
            {
                if(logitem.trigger.WaitOne(30000))
                {
                    FileStream tmpFS = null;
                    try
                    {
                        tmpFS = File.Open(logitem.filepath, FileMode.OpenOrCreate);
                        tmpFS.Seek(0,SeekOrigin.End);
                        if (logitem.mutex.WaitOne())
                        {
                            foreach (string message in logitem.messagelst)
                            {
                                byte[] tmpBuf = Encoding.Unicode.GetBytes(message + "\r\n");
                                tmpFS.Write(tmpBuf, 0, tmpBuf.Length);
                            }
                            logitem.messagelst.Clear();
                            logitem.mutex.ReleaseMutex();
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (tmpFS != null)
                        {
                            tmpFS.Close();
                        }
                    }
                }
            }
        }

        public static void Dispose()
        {
            RunningTag = false;
            foreach (KeyValuePair<string, LogItem> kvItem in StructDict)
            {
                kvItem.Value.trigger.Set();
            }
        }
    }
}