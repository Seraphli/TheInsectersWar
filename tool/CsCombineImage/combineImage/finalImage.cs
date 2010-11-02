using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace combineImage
{
	public class finalImage
	{

		public finalImage(int lFactorWidth,int lFactorHeight,int lImageNum)
			{
				mFinalImageDataPtr = new finalImageData();
				mFinalImageDataPtr.FactorWidth(lFactorWidth);
				mFinalImageDataPtr.FactorHeight(lFactorHeight);
				mFinalImageDataPtr.ImageNum(lImageNum);

				if(lImageNum>15)
					mFinalImageDataPtr.setNumOfPicInRow((int)Math.Sqrt((float)lImageNum));

				int lNumOfPicInRow = mFinalImageDataPtr.getNumOfPicInRow();
				int lWidth = lFactorWidth*lNumOfPicInRow;
				int lHeight = lFactorHeight*(lImageNum/lNumOfPicInRow);
				if( lImageNum%lNumOfPicInRow != 0 )
					lHeight+=lFactorHeight;
				mFinalImageDataPtr.setImage(
			            new Bitmap(lWidth,lHeight,PixelFormat.Format32bppArgb)
					);

			}
		
			//lIndex 从0开始
			public void drawImage(int lIndex,Bitmap	lFactorDib)
			{
				Bitmap	lDib=lFactorDib;
//				if( lFactorDib.PixelFormat==PixelFormat.Format32bppArgb)
//					lDib = lFactorDib;
//				else
//					lDib = FreeImage_ConvertTo32Bits(lFactorDib);

				Graphics 	lTargetImg = mFinalImageDataPtr.getGraphics();
			
				if(lIndex>=mFinalImageDataPtr.ImageNum())
				{
					logError("Index of image is out of ");
					return;
				}
				int lWidth = lDib.Width;
				if(lWidth!=mFinalImageDataPtr.FactorWidth())
				{
					logError("Width Error");
					return;
				}
				int lHeight = lDib.Height;
				if(lHeight!=mFinalImageDataPtr.FactorHeight())
				{
					logError("Height Error");
					return;
				}
			
				int posU = lIndex%mFinalImageDataPtr.getNumOfPicInRow() * lWidth;
				int posV = lIndex/mFinalImageDataPtr.getNumOfPicInRow() * lHeight;
			
				lTargetImg.DrawImage(lDib, posU, posV);


//				long lPixelNum = lWidth*lHeight;
//				//FreeImage_GetPixelIndex
//				int lPixelSize = mFinalImageDataPtr->getPixelSize();
//
//				void* srcData = FreeImage_GetBits(lDib);
//				void* targetData = FreeImage_GetBits(lTargetImg);
//
//				int lPixelBitSize = lPixelSize/8;
//				long posU = lIndex%mFinalImageDataPtr->getNumOfPicInRow() * lWidth;
//				long posV = lIndex/mFinalImageDataPtr->getNumOfPicInRow() * lHeight;
//
//				//long lTargetLine = lHeight*(lIndex/mFinalImageDataPtr->getNumOfPicInRow());
//				posV = FreeImage_GetHeight(lTargetImg) - lHeight - posV;
//				long lTargetLine = posV;
//
//				//TSS::imageUtil::drawPicture(posU,posV,srcData,lWidth,lHeight,targetData,
//				//	FreeImage_GetWidth(lTargetImg),
//				//	FreeImage_GetHeight(lTargetImg),
//				//	lPixelBitSize);
//
//
//				for(long lSourceLine=0;lSourceLine<lHeight;++lSourceLine)
//				{
//					BYTE* lTargetData = FreeImage_GetScanLine(lTargetImg,lTargetLine);
//					BYTE* lSourceData = FreeImage_GetScanLine(lDib,lSourceLine);
//					memcpy(
//						lTargetData+posU*lPixelBitSize, 
//						lSourceData, 
//						lPixelBitSize*lWidth
//					);
//					++lTargetLine;
//
//				}
//
//
//				//若有转换过图,则销毁转换的拷贝
//				if(lDib!=lFactorDib)
//					FreeImage_Unload(lDib);

			}

			public void saveToFile(string fileName)
			{
				mFinalImageDataPtr.getImage().Save(fileName,ImageFormat.Png);
			}

			void logError(string lInfo)
			{
				Console.WriteLine(lInfo);
			}

			finalImageData	mFinalImageDataPtr;
	}
}

