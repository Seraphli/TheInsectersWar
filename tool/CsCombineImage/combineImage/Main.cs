using System;

namespace combineImage
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try
			{
				string path;
				string extensionName;
			
				int imgBeginNum = 0;
				int imgEndNum = 0;
			
				Console.Write("路径:");
				path = Console.ReadLine();
				//std::cin>>path;
			
				Console.Write("扩展名:");
				extensionName = Console.ReadLine();
				//std::cout<<"extensionName"<<std::endl;
				//std::cin>>extensionName;
			
				Console.Write("图片起始序号:");
				imgBeginNum = Int32.Parse( Console.ReadLine() );
				//std::cout<<"imgBeginNum"<<std::endl;
				//std::cin>>imgBeginNum;
			
				Console.Write("图片结束序号:");
				imgEndNum = Int32.Parse( Console.ReadLine() );
				//std::cout<<"imgEndNum"<<std::endl;
				//std::cin>>imgEndNum;
			
				combineImage.imageCombiner.combine(path,extensionName,imgBeginNum,imgEndNum);
				
				//自动测试
//				combineImage.imageCombiner.test();
			
				
			}
			catch (System.Exception ex)
			{
			    System.Console.WriteLine("{0} exception caught here.", ex.GetType().ToString());
			    System.Console.WriteLine(ex.Message);
			}
			finally
			{
			    System.Console.WriteLine("Clean-up code executes here...");
			}

			
			Console.Read();
			Console.Read();

		}
	}
}

