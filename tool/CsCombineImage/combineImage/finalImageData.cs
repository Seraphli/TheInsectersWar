using System;
using System.Drawing;


namespace combineImage
{
	public class finalImageData
	{
		
			public finalImageData()
			{
			}

			~finalImageData()
			{
//				if(mDib)
//					FreeImage_Unload(mDib);
			}

			public Bitmap getImage() { return mDib; }
		
			public Graphics getGraphics(){return mGraphics;}
		
			public void setImage(Bitmap val) 
			{ 
				//assert(!mDib);
				mDib = val; 
				mGraphics = Graphics.FromImage(mDib);
			}

			public int FactorWidth() { return mFactorWidth; }
			public void FactorWidth(int val) { mFactorWidth = val; }

			public int FactorHeight()  { return mFactorHeight; }
			public void FactorHeight(int val) { mFactorHeight = val; }

			public int ImageNum() { return mImageNum; }
			public void ImageNum(int val) { mImageNum = val; }

			public  int getNumOfPicInRow()
			{
				return mNumOfPicInRow;
			}

			public void setNumOfPicInRow(int lValue)
			{
				mNumOfPicInRow = lValue;
			}

			public int getPixelSize()
			{
				return 32;
			}

			Bitmap 	mDib;
			Graphics mGraphics ;

			//被合并的图,单幅的长宽
			int	mFactorWidth=0;
			int	mFactorHeight=0;
			//被合并的图片数
			int		mImageNum=0;
			int	mNumOfPicInRow=0;
	}
}

