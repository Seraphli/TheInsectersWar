using System.Net;

namespace zz
{
    namespace Net
    {
        public class BreakpointDownload
        {
            public static HttpWebResponse getDownloadInfo(System.Uri pUri)
            {
                HttpWebRequest lRequest = (HttpWebRequest)HttpWebRequest.Create(pUri);
                lRequest.Accept = "*/*";
                return (HttpWebResponse)lRequest.GetResponse();
            }

            static WebHeaderCollection getHeader(System.Uri pUri)
            {
                var lHeader = new WebHeaderCollection();
                //lHeader.Add("Host", pUri.Host);
                //lHeader.Add("Get", pUri.PathAndQuery);
                //lHeader.Add("Accept", "*/*");
                lHeader.Add("Cache-Control", "no-cache");
                //lHeader.Add("Connection", "Keep-Alive");
                lHeader.Add("Pragma", "no-cache");
                //lHeader.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                return lHeader;
            }

            public static bool download(System.Uri pUri, System.IO.Stream pWriterStream)
            {
                return download(pUri, 0, 0, pWriterStream);
            }

            public static bool download(System.Uri pUri, int pBegin, System.IO.Stream pWriterStream)
            {
                return download(pUri, pBegin, 0, pWriterStream);
            }

            public static bool download(System.Uri pUri, int pBegin, int pEnd, System.IO.Stream pWriterStream)
            {
                HttpWebRequest lRequest = (HttpWebRequest)HttpWebRequest.Create(pUri);
                lRequest.Timeout = 30000;
                lRequest.ReadWriteTimeout = 60000;
                lRequest.Headers = getHeader(pUri);
                if (pEnd > 0)
                    lRequest.AddRange(pBegin, pEnd);
                else
                    lRequest.AddRange(pBegin);
                lRequest.Accept = "*/*";
                lRequest.KeepAlive = true;
                lRequest.Method = WebRequestMethods.Http.Get;
                //对发送的数据不使用缓存
                lRequest.AllowWriteStreamBuffering = false;
                var lResponse = (HttpWebResponse)lRequest.GetResponse();
                using(var lResponseStream = lResponse.GetResponseStream())
                {
                    byte[] lBuffer = new byte[1024];
                    int lSize = lResponseStream.Read(lBuffer, 0, lBuffer.Length);
                    while (lSize > 0)
                    {
                        pWriterStream.Write(lBuffer, 0, lSize);
                        lSize = lResponseStream.Read(lBuffer, 0, lBuffer.Length);
                    }
                }
                lRequest.Abort();
                lResponse.Close();
                return true;

            }
        }
    }
}