using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using zz.Net;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            var lProgram = new Program();
            lProgram.DoMain(new string[]{
                "DownloadUrl=http://50.19.80.93/u/6264102/test/%5B%E5%8F%AF%E7%88%B1%E6%B4%8B%E8%91%B1%E5%A4%B4%E5%BA%9E%E5%85%8B%E9%A3%8E%E6%A1%8C%E5%B8%83%5D.Sweet.ONION.TOU.rar",
                "FileName=game.rar",
                "TempPath=temp",
                @"7zPath=E:\Program Files\7-Zip\7z.exe",
                @"UpdatePath=E:\develop\team\microcosmic_war\tool\Updater\Updater\bin\Debug\game",
                "SrcPathInArchive=",
            });
        }

        void parse(string pKey,string pValue)
        {
            switch(pKey)
            {
                case "DownloadUrl":
                    downloadUrls.Add(pValue);
                    break;
                case "FileName":
                    fileName = pValue;
                    break;
                case "TempPath":
                    tempPath = pValue;
                    break;
                case "7zPath":
                    _7zPath = pValue;
                    break;
                case "UpdatePath":
                    updatePath = pValue;
                    break;
                case "SrcPathInArchive":
                    srcPathInArchive = pValue;
                    break;
                default:
                    Console.WriteLine("no the command: key {0},value {1}",
                        pKey, pValue);
                    break;
            }
        }

        string fileName;
        string tempPath;
        string _7zPath;

        //将压缩包中lArchivePath路径下的东西,覆盖到lUpdateDir下
        string updatePath;
        string srcPathInArchive = "";
        List<string> downloadUrls = new List<string>();

        bool availableCheck()
        {
            if (fileName != null
                && tempPath != null
                && _7zPath != null
                && updatePath != null
                )
                return true;
            return false;
        }

        void parse(string pCommand)
        {
            int lSeparatorPos = pCommand.IndexOf('=');
            if(lSeparatorPos>0)
            {
                var lKey = pCommand.Substring(0, lSeparatorPos);
                var lValue = pCommand.Substring(lSeparatorPos+1);
                if(lKey.Trim().Length>0
                    && lValue.Trim().Length>0)
                {
                    parse(lKey, lValue);
                }
            }
        }

        void OutError(string pInfo)
        {
            Console.WriteLine(pInfo);
            Console.ReadLine();
        }

        void DoMain(string[] args)
        {
            foreach (var lArg in args)
            {
                parse(lArg);
            }
            if(!availableCheck())
            {
                OutError("失败:参数不足");
                return;
            }
            //string lUrl = "http://50.19.80.93/u/6264102/test/collisionLayer.cs";
            //string lUrl1 = "http://50.19.80.93/u/6264102/test/%5B%E5%8F%AF%E7%88%B1%E6%B4%8B%E8%91%B1%E5%A4%B4%E5%BA%9E%E5%85%8B%E9%A3%8E%E6%A1%8C%E5%B8%83%5D.Sweet.ONION.TOU.rar";
            //string lUrl = "http://183.60.157.27/down_group100/M00/17/9A/tzydG05Rdm8AAAAAASpNZA7G0085153133/TheInsectersWar0.5%212011082203.7z?k=ChQuPMBC-p0VERWBUESqMg&t=1314463966&u=222.76.19.54@4352405@bhdy0f5d&file=TheInsectersWar0.5%212011082203.7z";
            //string lFileName = "game.rar";
            //string lTempPath = "temp";
            //string l7zPath = @"E:\Program Files\7-Zip\7z.exe";

            ////将压缩包中lArchivePath路径下的东西,覆盖到lUpdateDir下
            //string lUpdatePath = @"E:\develop\team\microcosmic_war\tool\Updater\Updater\bin\Debug\game";
            //string lSrcPathInArchive = ""; 
            var lDownloader = new zz.Net.UpdateDownloader()
            {
                downloadList = downloadUrls.ToArray(),
                fileName = fileName,
                tempDir = tempPath,
            };
            Console.WriteLine("下载更新中...");
            if(!lDownloader.download())
            {
                OutError("失败:获取安装文件失败");
                return;
            }
            string lExtractFolder = Path.GetFileNameWithoutExtension(fileName);
            string lDownFilePath = lDownloader.filePath;
            //string lUpdatePackageDir = "../updateDest";


            Console.WriteLine("安装更新中...");
            var lUpdateSetup = new UpdateSetup()
            {
                _7zPath = _7zPath,
                extractFolderName = lExtractFolder,
                setupFilePath = lDownloader.filePath,
                tempPath = tempPath,
                updatePath = updatePath,
                srcPathInArchive = srcPathInArchive,
            };
            lUpdateSetup.setup();
            ////string lExtractorName = "7z.exe";
            ////byte[] lExtractorData = Updater.Properties.Resources._7z;
            ////using(var lExtractor = new FileStream(lExtractorName,FileMode.OpenOrCreate, FileAccess.ReadWrite))
            ////{
            ////    lExtractor.Write(lExtractorData, 0, lExtractorData.Length);
            ////}
            ////创建解压用的临时文件夹
            //var lExtractFolderDir = lTempDir + Path.DirectorySeparatorChar + lExtractFolder;
            //if (Directory.Exists(lExtractFolderDir))
            //{
            //    Directory.Delete(lExtractFolderDir,true);
            //}
            //Directory.CreateDirectory(lExtractFolderDir);
            //var lExtractorArguments = string.Format("x \"{0}\" -o\"{1}\" -y",
            //            lDownFilePath, lExtractFolderDir);
            //Console.WriteLine(lExtractorArguments);
            //var lExtractorProcess = new System.Diagnostics.Process()
            //{
            //    StartInfo = new System.Diagnostics.ProcessStartInfo()
            //    {
            //        FileName = _7zPath,
            //        UseShellExecute = false,
            //        Arguments = lExtractorArguments,
            //        //WorkingDirectory = appl
            //        //RedirectStandardError = true,
            //    },
            //};
            //lExtractorProcess.Start();
            //lExtractorProcess.WaitForExit();
            ////File.Move()
            //MoveCover(lExtractFolderDir + Path.DirectorySeparatorChar + lSrcPathInArchive, lUpdateDir);
            //Console.WriteLine("更新完毕");
            //Directory.Delete(lExtractFolderDir,true);
            Console.ReadLine();
        }
    }

    //    static void Main(string[] args)
    //    {
    //        //string lUrl = "http://50.16.230.95/u/6264102/test/collisionLayer.cs";
    //        string lUrl = "http://50.16.230.95/u/6264102/test/%5B%E5%8F%AF%E7%88%B1%E6%B4%8B%E8%91%B1%E5%A4%B4%E5%BA%9E%E5%85%8B%E9%A3%8E%E6%A1%8C%E5%B8%83%5D.Sweet.ONION.TOU.rar";

    //        //string lUrl = "http://183.60.157.27/down_group100/M00/17/9A/tzydG05Rdm8AAAAAASpNZA7G0085153133/TheInsectersWar0.5%212011082203.7z?k=ChQuPMBC-p0VERWBUESqMg&t=1314463966&u=222.76.19.54@4352405@bhdy0f5d&file=TheInsectersWar0.5%212011082203.7z";
    //        string lTempDir = "temp";
    //        string lFileName = "test.rar";
    //        //var lMemoryStream = new MemoryStream();
    //        var lUri = new System.Uri(lUrl);
    //        var lDownloadInfo = BreakpointDownload.getDownloadInfo(lUri);
    //        var lResponseHeaders = lDownloadInfo.Headers;
    //        var lKeys = lResponseHeaders.Keys;
    //        for(int i=0;i<lResponseHeaders.Count;++i)
    //        {
    //            Console.WriteLine(lKeys[i].ToString() + ":" + lResponseHeaders[i]);
    //        }

    //        if (lTempDir.Length>0 && !Directory.Exists(lTempDir))
    //        {
    //            Directory.CreateDirectory(lTempDir);
    //        }

    //        using (var lFile = new FileStream(lTempDir+Path.DirectorySeparatorChar+lFileName,
    //            FileMode.OpenOrCreate,FileAccess.ReadWrite, FileShare.ReadWrite))
    //        {
    //            if (lFile.Length != lDownloadInfo.ContentLength)
    //                continueDownload(lFile, lUri, lDownloadInfo.ContentLength);
    //        }
    //        //BreakpointDownload.download(lUri, lMemoryStream);
    //        //Console.Write(Encoding.ASCII.GetString(lMemoryStream.GetBuffer()));
    //        Console.WriteLine("finish");
    //        Console.ReadLine();
    //    }

    //    public class DownloadInfoPrinter
    //    {
    //        public float rate;
    //        public float speed;
    //        public float size;
    //        public void refresh()
    //        {
    //            var lCursorLeft = Console.CursorLeft;
    //            var lCursorTop = Console.CursorTop;
    //            Console.SetCursorPosition(0, Console.WindowHeight - 1);
    //            Console.Write("{0:f2}m({1:f1}%) {2:f1}k/s          ", size, rate * 100f, speed);
    //            Console.CursorLeft = lCursorLeft;
    //            Console.CursorTop = lCursorTop;
    //        }
    //    }

    //    class OutDownloadInfo:Stream
    //    {
    //        public DateTime lastPrintTime = DateTime.Now;
    //        long _fileLength;
    //        public long fileLength
    //        {
    //            set 
    //            {
    //                _fileLength = value;
    //                infoPrinter.size = (float)value / 1024f / 1024f; 
    //            }
    //        }
    //        public int size = 0;
    //        public Stream wrapedStream;
    //        public DownloadInfoPrinter infoPrinter = new DownloadInfoPrinter(); 
    //        public override bool CanRead { get { return wrapedStream.CanRead; } }
    //        public override bool CanSeek { get { return wrapedStream.CanSeek; } }
    //        public override bool CanWrite { get { return wrapedStream.CanWrite; } }
    //        public override long Length { get { return wrapedStream.Length; } }
    //        public override long Position 
    //        {
    //            get { return wrapedStream.Position; }
    //            set { wrapedStream.Position = value; }
    //        }

    //        public override void Flush() { wrapedStream.Flush(); }


    //        public override int Read(byte[] buffer, int offset, int count)
    //        {
    //            return wrapedStream.Read(buffer, offset, count);
    //        }

    //        public override long Seek(long offset, SeekOrigin origin)
    //        {
    //            return wrapedStream.Seek(offset, origin);
    //        }


    //        public override void SetLength(long value)
    //        {
    //            wrapedStream.SetLength(value);
    //        }

    //        public void refreshShow(TimeSpan pDeltaTime)
    //        {
    //            infoPrinter.rate = (float)Length / (float)_fileLength;
    //            infoPrinter.speed = (float)size / 1024f / (float)pDeltaTime.TotalSeconds;
    //            infoPrinter.refresh();
    //        }

    //        public void refreshShow()
    //        {
    //            infoPrinter.rate = (float)Length / (float)_fileLength;
    //            infoPrinter.speed = 0f;
    //            infoPrinter.refresh();
    //        }

    //        public override void Write(byte[] buffer, int offset, int count)
    //        {
    //            var lNow = DateTime.Now;
    //            var lDelta = lNow - lastPrintTime;
    //            if (lDelta.Seconds>=1)
    //            {
    //                refreshShow(lDelta);
    //                size = 0;
    //                lastPrintTime = lNow;
    //            }
    //            wrapedStream.Write(buffer, offset, count);
    //            size += count;
    //        }
    //    }

    //    static void continueDownload(FileStream pFileStream, System.Uri pUri, long pDataLength)
    //    {
    //        pFileStream.Position = pFileStream.Length;
    //        using (var lOutDownloadInfo = new OutDownloadInfo() {
    //            wrapedStream = pFileStream , fileLength = pDataLength})
    //        {
    //            BreakpointDownload.download(pUri, (int)pFileStream.Length, lOutDownloadInfo);
    //            lOutDownloadInfo.refreshShow();
    //        }

    //    }
    //}
}
