using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;


namespace FionPushFilm.HtmlHelper
{
    class HtmlReader
    {
        public delegate void DealWithResponse(string html, object par);

        private class RequestState
        {
            public HttpWebRequest request;
            public HttpWebResponse response;
            public Stream responseStream;
            public Stream buffer;
            public byte[] tmpBuf;
            public DealWithResponse dealwithfun;
            public object dealwithpar;
            public bool otherTag;
        }

        public HtmlReader()
        {
        }

        public static string OpenSync(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(url));
                request.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
                request.Accept = "text/html,application/xhtml+xml;q=0.9,image/webp,*/*;q=0.8";
                request.UserAgent = "Mozilla/5.0(window NT 6.1) Applewebkit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 safari/537.36";
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn,zh;q=0.8");

                WebResponse response = request.GetResponse();

                string encodingStr = response.Headers["Content-Encoding"];
                Stream tmpStream = response.GetResponseStream();
                //tmpStream.Seek(0, SeekOrigin.Begin);
                //结束
                if (encodingStr == "gzip")
                {
                    tmpStream = new GZipStream(tmpStream, CompressionMode.Decompress);
                }
                else if (encodingStr == "deflate")
                {
                    tmpStream = new DeflateStream(tmpStream, CompressionMode.Decompress);
                }
                StreamReader reader = new StreamReader(tmpStream);
                string htmlStr = reader.ReadToEnd();
                reader.Close();
                return htmlStr;
            }
            catch
            {
                return "";
            }
        }

        public void Open(string url,DealWithResponse dealwithFun,object dealwithPar )
        {
            HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(url));
            DealWithResponse dealwithSearch = new DealWithResponse(dealwithFun);
            SendRequest(request, dealwithSearch, dealwithPar);
        }

        private void SendRequest(HttpWebRequest request, DealWithResponse dealwithCB, object dealwithPar)
        {
            RequestState state = new RequestState();
            state.request = request;
            state.dealwithfun = dealwithCB;
            state.dealwithpar = dealwithPar;
            state.buffer = new MemoryStream();
            state.tmpBuf = new byte[1024];
            request.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
            request.Accept = "text/html,application/xhtml+xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0(window NT 6.1) Applewebkit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 safari/537.36";
            //request.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml;q=0.9,image/webp,*/*;q=0.8");
            //request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0(window NT 6.1) Applewebkit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 safari/537.36");
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn,zh;q=0.8");
            //request.CookieContainer = new CookieContainer();
            //request.CookieContainer.Add(new Cookie("__test", "11"));
            //request.CookieContainer.Add(new Cookie("x__utmvc", "1"));
            //request.CookieContainer.Add(new Cookie("PHPSESSID", "7b9d72107ad3af399e964a2442dac7d9"));
            //request.CookieContainer.Add(new Cookie("visid_incap_146743", "V8w1iWmWQk2aN23QPhVdmwm6yVMAAAAAQUIPAAAAAAAap46eSzXwuSOLQTnA+QWR"));
            //request.CookieContainer.Add(new Cookie("incap_ses_200_146743", "z9uNSToNaykTAJvzOIvGAudr81MAAAAAE2ZWW3YIC/vbCTJw7mE+YA=="));
            //request.CookieContainer.Add(new Cookie("__utma", "124867215.161693485.1404718479.1408459005.1408461452.16"));
            //request.CookieContainer.Add(new Cookie("__utmz", "124867215.1404718479.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)"));
            //request.CookieContainer.Add(new Cookie("__atuvc", "49%7C30%2C10%7C31%2C43%7C32%2C11%7C33%2C10%7C34"));

            request.BeginGetResponse(new AsyncCallback(RespCallback), state);
        }

        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            RequestState state = (RequestState)(asynchronousResult.AsyncState);
            HttpWebRequest request = state.request;
            //WebResponse tmpResponse = request.EndGetResponse(asynchronousResult);
            state.response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            state.responseStream = state.response.GetResponseStream();
            state.responseStream.BeginRead(state.tmpBuf, 0, 1024, ReadCallback, state);
            state.otherTag = true;
        }

        private static void ReadCallback(IAsyncResult asynchronousResult)
        {
            RequestState state = (RequestState)(asynchronousResult.AsyncState);
            int RLen = state.responseStream.EndRead(asynchronousResult);
            if (state.otherTag && RLen == 0)
            {
                string encodingStr = state.response.Headers["Content-Encoding"];
                Stream tmpStream = null;
                //结束
                if (encodingStr == "gzip")
                {
                    state.buffer.Seek(0, SeekOrigin.Begin);
                    tmpStream = new GZipStream(state.buffer, CompressionMode.Decompress);
                }
                else if (encodingStr == "deflate")
                {
                    state.buffer.Seek(0, SeekOrigin.Begin);
                    tmpStream = new DeflateStream(state.buffer, CompressionMode.Decompress);
                }
                else
                {
                    tmpStream = state.buffer;
                    tmpStream.Seek(0, SeekOrigin.Begin);
                }
                StreamReader reader = new StreamReader(tmpStream);
                string htmlStr = reader.ReadToEnd();
                reader.Close();
                state.dealwithfun(htmlStr, state.dealwithpar);

            }
            else
            {
                if (RLen == 1024)
                {
                    state.otherTag = false;
                    state.buffer.Write(state.tmpBuf, 0, RLen);
                    state.responseStream.BeginRead(state.tmpBuf, 0, 1024, ReadCallback, state);
                }
                else
                {
                    state.buffer.Write(state.tmpBuf, 0, RLen);
                    state.request.BeginGetResponse(new AsyncCallback(RespCallback), state);
                }
            }
        }
    }
}
