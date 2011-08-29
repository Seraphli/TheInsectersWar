using System;
using System.IO;

namespace zz
{
    namespace Net
    {
        public class UpdateDownloader
        {
            public string tempDir = "";
            public string fileName;
            public int bufferSize = 1000000;
            public string[] downloadList
            {
                set
                {
                    downloadUriList = new System.Uri[value.Length];
                    uriErrorCount = new int[value.Length];
                    for(int i=0;i<value.Length;++i)
                    {
                        downloadUriList[i] = new System.Uri(value[i]);
                    }
                }
            }
            
            int maxRetry = 3;
            int errorCount = 0;
            System.Uri[] downloadUriList;

            //记录每个Uri的出错次数
            int[] uriErrorCount;

            void outError(string pInfo)
            {
                Console.WriteLine(pInfo);
                Console.ReadLine();
            }

            public bool download()
            {
                if (downloadUriList == null || downloadUriList.Length < 0)
                    outError("更新失败:没有可用的更新地址");
                while (errorCount < downloadUriList.Length * maxRetry)
                {
                    for (int i = 0; i < downloadUriList.Length; ++i)
                    {
                        if (uriErrorCount[i] >= maxRetry)
                            continue;
                        try
                        {
                            Console.WriteLine(downloadUriList[i]);
                            if (download(downloadUriList[i]))
                                return true;
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e);
                            outError("更新失败:文件访问错误");
                        }
                        catch (System.Exception e)
                        {
                            Console.WriteLine(e);
                            ++uriErrorCount[i];
                            ++errorCount;
                        }
                    }
                }
                return false;
            }

            public string filePath
            {
                get 
                { 
                    return Path.Combine(tempDir,fileName);
                }
            }

            public bool download(System.Uri lUri)
            {
                var lDownloadInfo = BreakpointDownload.getDownloadInfo(lUri);
                lDownloadInfo.Close();

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                using (var lFile = new FileStream(filePath,
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite,bufferSize))
                {
                    if (lFile.Length != lDownloadInfo.ContentLength)
                        continueDownload(lFile, lUri, lDownloadInfo.ContentLength);
                    if (lFile.Length == lDownloadInfo.ContentLength)
                        return true;
                }
                //BreakpointDownload.download(lUri, lMemoryStream);
                //Console.Write(Encoding.ASCII.GetString(lMemoryStream.GetBuffer()));
                return false;
            }

            public class DownloadInfoPrinter
            {
                public string info = string.Empty;
                public float rate;
                public float speed;
                public float size;
                public void refresh()
                {
                    var lCursorLeft = Console.CursorLeft;
                    var lCursorTop = Console.CursorTop;
                    Console.SetCursorPosition(0, Console.WindowHeight - 1);
                    Console.Write("{0} {1:f2}m({2:f1}%) {3:f1}k/s          ",info, size, rate * 100f, speed);
                    Console.CursorLeft = lCursorLeft;
                    Console.CursorTop = lCursorTop;
                }
                public void clear()
                {
                    var lCursorLeft = Console.CursorLeft;
                    var lCursorTop = Console.CursorTop;
                    Console.SetCursorPosition(0, Console.WindowHeight - 1);
                    Console.Write("                                                   ");
                    Console.CursorLeft = lCursorLeft;
                    Console.CursorTop = lCursorTop;
                }
            }

            class OutDownloadInfo : Stream
            {
                public DateTime lastPrintTime = DateTime.Now;
                long _fileLength;
                public long fileLength
                {
                    set
                    {
                        _fileLength = value;
                        infoPrinter.size = (float)value / 1024f / 1024f;
                    }
                }
                public string fileName
                {
                    set
                    {
                        infoPrinter.info = value;
                    }
                }
                public int size = 0;
                public Stream wrapedStream;
                public DownloadInfoPrinter infoPrinter = new DownloadInfoPrinter();
                public override bool CanRead { get { return wrapedStream.CanRead; } }
                public override bool CanSeek { get { return wrapedStream.CanSeek; } }
                public override bool CanWrite { get { return wrapedStream.CanWrite; } }
                public override long Length { get { return wrapedStream.Length; } }
                public override long Position
                {
                    get { return wrapedStream.Position; }
                    set { wrapedStream.Position = value; }
                }

                public override void Flush() { wrapedStream.Flush(); }


                public override int Read(byte[] buffer, int offset, int count)
                {
                    return wrapedStream.Read(buffer, offset, count);
                }

                public override long Seek(long offset, SeekOrigin origin)
                {
                    return wrapedStream.Seek(offset, origin);
                }


                public override void SetLength(long value)
                {
                    wrapedStream.SetLength(value);
                }

                public void refreshShow(TimeSpan pDeltaTime)
                {
                    infoPrinter.rate = (float)Length / (float)_fileLength;
                    infoPrinter.speed = (float)size / 1024f / (float)pDeltaTime.TotalSeconds;
                    infoPrinter.refresh();
                }

                public void clearShow()
                {
                    infoPrinter.clear();
                }

                public override void Write(byte[] buffer, int offset, int count)
                {
                    var lNow = DateTime.Now;
                    var lDelta = lNow - lastPrintTime;
                    if (lDelta.Seconds >= 1)
                    {
                        refreshShow(lDelta);
                        size = 0;
                        lastPrintTime = lNow;
                    }
                    wrapedStream.Write(buffer, offset, count);
                    size += count;
                }
            }

            void continueDownload(FileStream pFileStream, System.Uri pUri, long pDataLength)
            {
                pFileStream.Position = pFileStream.Length;
                using (var lOutDownloadInfo = new OutDownloadInfo()
                {
                    wrapedStream = pFileStream,
                    fileLength = pDataLength,
                    fileName = fileName,
                })
                {
                    BreakpointDownload.download(pUri, (int)pFileStream.Length, lOutDownloadInfo);
                    lOutDownloadInfo.clearShow();
                }

            }
        }
    }
}