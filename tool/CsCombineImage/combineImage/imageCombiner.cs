using System;
using System.Drawing;


namespace combineImage
{
	public class imageCombiner
	{
		public imageCombiner ()
		{
		}
		
		public static void combine(string path, string extensionName,int imgNum)
		{
			path+="/";
			string lSrcFile = path+"1"+"."+extensionName;

			Bitmap lFirstImg = Load(lSrcFile);
			int lWidth = lFirstImg.Width;
			int lHeight = lFirstImg.Height;
			finalImage	lFinalImage = new finalImage(lWidth,lHeight,imgNum);
			lFinalImage.drawImage(0,lFirstImg);
			Console.WriteLine(lSrcFile);
//			FreeImage_Unload(srcImg);

			for(int i = 1;i<imgNum; ++i)
			{
				lSrcFile= ""+ (i+1)+"."+extensionName;
				Bitmap lSrcImg = Load(lSrcFile);
				//std::cout<<lSrcFile<<std::endl;
				Console.WriteLine(lSrcFile);
				lFinalImage.drawImage(i,lSrcImg);
				//FreeImage_Unload(srcImg);
			}
			lFinalImage.saveToFile(path+"final.png");

		}

		public static void combine(string path, string extensionName,int imgBeginNum,int imgEndNum)
		{
			path+="/";
			if(imgBeginNum>=imgEndNum)
				return;
			int lImgIndex = imgBeginNum;
			string lSrcFile = path+ (lImgIndex)+"."+extensionName;

			Bitmap lFirstImg = Load(lSrcFile);
			int lWidth = lFirstImg.Width;
			int lHeight = lFirstImg.Height;
			finalImage	lFinalImage = new finalImage(lWidth,lHeight,imgEndNum - imgBeginNum +1);
			lFinalImage.drawImage(0,lFirstImg);
			//FreeImage_Unload(srcImg);
			Console.WriteLine(lSrcFile);

			int i=1;
			for(++lImgIndex;lImgIndex<=imgEndNum; ++lImgIndex)
			{
				lSrcFile= path+ (lImgIndex)+"."+extensionName;
				Bitmap lSrcImg = Load(lSrcFile);
				Console.WriteLine(lSrcFile);
				lFinalImage.drawImage(i,lSrcImg);
				//FreeImage_Unload(srcImg);
				++i;
			}
			lFinalImage.saveToFile(path+"final.png");

		}

		static Bitmap  Load(string file)
		{
//			FREE_IMAGE_FORMAT lFormat;
//			lFormat =  FreeImage_GetFileType(file.c_str());
//			if(lFormat==FIF_UNKNOWN)
//				lFormat =  FreeImage_GetFIFFromFilename(file.c_str());
//
//			assert(lFormat!=FIF_UNKNOWN);
//
//			FIBITMAP * srcImg = FreeImage_Load(lFormat,file.c_str());
//			return srcImg;
			return new Bitmap(file);
		}

		
		public static void test()
		{
			combine("D:/czl/ttt", "png",100001,100046);
		}
	}
}

