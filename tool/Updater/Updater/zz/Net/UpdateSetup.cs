using System;
using System.IO;

namespace zz
{
    namespace Net
    {
        public class UpdateSetup
        {
            public string tempPath = "temp";
            public string setupFilePath;
            public string extractFolderName;
            public string _7zPath;

            //将压缩包中srcPathInArchive路径下的东西,覆盖到updateDir下
            public string updatePath;
            public string srcPathInArchive;

            public void setup()
            {
                //创建解压用的临时文件夹
                var lExtractFolderDir = Path.Combine(tempPath, extractFolderName);
                if (Directory.Exists(lExtractFolderDir))
                {
                    Directory.Delete(lExtractFolderDir, true);
                }
                Directory.CreateDirectory(lExtractFolderDir);
                var lExtractorArguments = string.Format("x \"{0}\" -o\"{1}\" -y",
                            setupFilePath, lExtractFolderDir);
                Console.WriteLine(lExtractorArguments);
                var lExtractorProcess = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = _7zPath,
                        UseShellExecute = false,
                        Arguments = lExtractorArguments,
                        //WorkingDirectory = appl
                        //RedirectStandardError = true,
                    },
                };
                lExtractorProcess.Start();
                lExtractorProcess.WaitForExit();
                //File.Move()
                MoveCover(Path.Combine(lExtractFolderDir, srcPathInArchive), updatePath);
                Console.WriteLine("更新完毕");
                if (Directory.Exists(lExtractFolderDir))
                    Directory.Delete(lExtractFolderDir, true);
            }

            static void MoveCover(DirectoryInfo pSrcDirInfo, string pDestDirName)
            {
                var lDestDirInfo = new DirectoryInfo(pDestDirName);
                if (!lDestDirInfo.Exists)
                {
                    pSrcDirInfo.MoveTo(pDestDirName);
                    return;
                }
                foreach (var lFiles in pSrcDirInfo.GetFiles())
                {
                    var lDestFilePath = Path.Combine(pDestDirName, lFiles.Name);
                    File.Delete(lDestFilePath);
                    lFiles.MoveTo(lDestFilePath);
                }
                foreach (var lDirectories in pSrcDirInfo.GetDirectories())
                {
                    var lDestDirPath = Path.Combine(pDestDirName, lDirectories.Name);
                    MoveCover(lDirectories, lDestDirPath);
                }

            }

            static void MoveCover(string pSourceDirName, string pDestDirName)
            {
                MoveCover(new DirectoryInfo(pSourceDirName), pDestDirName);
                Directory.Delete(pSourceDirName, true);
            }
        }
    }
}