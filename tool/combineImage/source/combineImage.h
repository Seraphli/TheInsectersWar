#include <FreeImage.h>
#include <vector>
#include <string>
#include <exception>
#include <boost/shared_ptr.hpp>
#include <boost/lexical_cast.hpp>
#include <assert.h>
#include <math.h>

//#include "TSSimageUtil.h"

namespace zz
{
	class combineImage
	{
		class finalImageData
		{
		public:
			finalImageData():mDib(NULL),mFactorWidth(0),mFactorHeight(0),
				mImageNum(0),mNumOfPicInRow(0)
			{
			}

			~finalImageData()
			{
				if(mDib)
					FreeImage_Unload(mDib);
			}

			FIBITMAP * getImage() const { return mDib; }
			void setImage(FIBITMAP * val) 
			{ 
				assert(!mDib);
				mDib = val; 
			}

			long FactorWidth() const { return mFactorWidth; }
			void FactorWidth(long val) { mFactorWidth = val; }

			long FactorHeight() const { return mFactorHeight; }
			void FactorHeight(long val) { mFactorHeight = val; }

			int ImageNum() const { return mImageNum; }
			void ImageNum(int val) { mImageNum = val; }

			inline int getNumOfPicInRow()const
			{
				return mNumOfPicInRow;
			}

			void setNumOfPicInRow(int lValue)
			{
				mNumOfPicInRow = lValue;
			}

			inline int getPixelSize()const
			{
				return 32;
			}

		protected:
			FIBITMAP *	mDib;
			//被合并的图,单幅的长宽
			long	mFactorWidth;
			long	mFactorHeight;
			//被合并的图片数
			int		mImageNum;
			long	mNumOfPicInRow;
		};

		typedef boost::shared_ptr<finalImageData>	finalImageDataPtr;

		class finalImage
		{
		public:
			finalImage(long lFactorWidth,long lFactorHeight,int lImageNum)
				:mFinalImageDataPtr(new finalImageData())
			{
				mFinalImageDataPtr->FactorWidth(lFactorWidth);
				mFinalImageDataPtr->FactorHeight(lFactorWidth);
				mFinalImageDataPtr->ImageNum(lImageNum);

				if(lImageNum>15)
					mFinalImageDataPtr->setNumOfPicInRow(sqrt((float)lImageNum));

				int lNumOfPicInRow = mFinalImageDataPtr->getNumOfPicInRow();
				long lWidth = lFactorWidth*lNumOfPicInRow;
				long lHeight = lFactorHeight*(lImageNum/lNumOfPicInRow);
				if(lImageNum%lNumOfPicInRow)
					lHeight+=lFactorHeight;
				mFinalImageDataPtr->setImage(
						FreeImage_Allocate(lWidth, lHeight, 32)
					);

			}

			//lIndex 从0开始
			void drawImage(unsigned lIndex,FIBITMAP *	lFactorDib)
			{
				FIBITMAP *	lDib;
				if( FreeImage_GetBPP ( lFactorDib )==32)
					lDib = lFactorDib;
				else
					lDib = FreeImage_ConvertTo32Bits(lFactorDib);

				FIBITMAP *	lTargetImg = mFinalImageDataPtr->getImage();
				if(lIndex>=mFinalImageDataPtr->ImageNum())
				{
					logError("Index of image is out of ");
					return;
				}
				long lWidth = FreeImage_GetWidth(lDib);
				if(lWidth!=mFinalImageDataPtr->FactorWidth())
				{
					logError("Width Error");
					return;
				}
				long lHeight = FreeImage_GetHeight(lDib);
				if(lHeight!=mFinalImageDataPtr->FactorHeight())
				{
					logError("Height Error");
					return;
				}

				long lPixelNum = lWidth*lHeight;
				//FreeImage_GetPixelIndex
				int lPixelSize = mFinalImageDataPtr->getPixelSize();

				void* srcData = FreeImage_GetBits(lDib);
				void* targetData = FreeImage_GetBits(lTargetImg);

				int lPixelBitSize = lPixelSize/8;
				long posU = lIndex%mFinalImageDataPtr->getNumOfPicInRow() * lWidth;
				long posV = lIndex/mFinalImageDataPtr->getNumOfPicInRow() * lHeight;

				//long lTargetLine = lHeight*(lIndex/mFinalImageDataPtr->getNumOfPicInRow());
				posV = FreeImage_GetHeight(lTargetImg) - lHeight - posV;
				long lTargetLine = posV;

				//TSS::imageUtil::drawPicture(posU,posV,srcData,lWidth,lHeight,targetData,
				//	FreeImage_GetWidth(lTargetImg),
				//	FreeImage_GetHeight(lTargetImg),
				//	lPixelBitSize);


				for(long lSourceLine=0;lSourceLine<lHeight;++lSourceLine)
				{
					BYTE* lTargetData = FreeImage_GetScanLine(lTargetImg,lTargetLine);
					BYTE* lSourceData = FreeImage_GetScanLine(lDib,lSourceLine);
					memcpy(
						lTargetData+posU*lPixelBitSize, 
						lSourceData, 
						lPixelBitSize*lWidth
					);
					++lTargetLine;

				}


				//若有转换过图,则销毁转换的拷贝
				if(lDib!=lFactorDib)
					FreeImage_Unload(lDib);

			}

			void saveToFile(const std::string& fileName)
			{
				FreeImage_Save( FIF_PNG, mFinalImageDataPtr->getImage(),
					fileName.c_str() );
			}

		protected:

			void logError(const std::string& lInfo)
			{
				std::cout<<lInfo<<std::endl;
			}

			finalImageDataPtr	mFinalImageDataPtr;

		};

		public:

		static void combine(const std::string& path, const std::string& extensionName,int imgNum)
		{
			std::string lSrcFile = path+"1"+"."+extensionName;

			FIBITMAP * srcImg = Load(lSrcFile);
			long lWidth = FreeImage_GetWidth(srcImg);
			long lHeight = FreeImage_GetHeight(srcImg);
			finalImage	lFinalImage(lWidth,lHeight,imgNum);
			lFinalImage.drawImage(0,srcImg);
			FreeImage_Unload(srcImg);

			for(int i = 1;i<imgNum; ++i)
			{
				lSrcFile= path+boost::lexical_cast<std::string>(i+1)+"."+extensionName;
				FIBITMAP * srcImg = Load(lSrcFile);
				std::cout<<lSrcFile<<std::endl;
				lFinalImage.drawImage(i,srcImg);
				FreeImage_Unload(srcImg);
			}
			lFinalImage.saveToFile(path+"final.png");

		}

		static void combine(const std::string& path, const std::string& extensionName,long imgBeginNum,long imgEndNum)
		{
			if(imgBeginNum>=imgEndNum)
				return;
			long lImgIndex = imgBeginNum;
			std::string lSrcFile = path+boost::lexical_cast<std::string>(lImgIndex)+"."+extensionName;

			FIBITMAP * srcImg = Load(lSrcFile);
			long lWidth = FreeImage_GetWidth(srcImg);
			long lHeight = FreeImage_GetHeight(srcImg);
			finalImage	lFinalImage(lWidth,lHeight,imgEndNum - imgBeginNum +1);
			lFinalImage.drawImage(0,srcImg);
			FreeImage_Unload(srcImg);

			int i=1;
			for(++lImgIndex;lImgIndex<=imgEndNum; ++lImgIndex)
			{
				lSrcFile= path+boost::lexical_cast<std::string>(lImgIndex)+"."+extensionName;
				FIBITMAP * srcImg = Load(lSrcFile);
				std::cout<<lSrcFile<<std::endl;
				lFinalImage.drawImage(i,srcImg);
				FreeImage_Unload(srcImg);
				++i;
			}
			lFinalImage.saveToFile(path+"final.png");

		}

		static FIBITMAP * Load(const std::string& file)
		{
			FREE_IMAGE_FORMAT lFormat;
			lFormat =  FreeImage_GetFileType(file.c_str());
			if(lFormat==FIF_UNKNOWN)
				lFormat =  FreeImage_GetFIFFromFilename(file.c_str());

			assert(lFormat!=FIF_UNKNOWN);

			FIBITMAP * srcImg = FreeImage_Load(lFormat,file.c_str());
			return srcImg;

		}

		static void test()
		{
			FREE_IMAGE_FORMAT lFormat;
			std::string lSrcFile = "D:/test/1.png";
			lFormat =  FreeImage_GetFileType(lSrcFile.c_str());
			if(lFormat==FIF_UNKNOWN)
				lFormat =  FreeImage_GetFIFFromFilename(lSrcFile.c_str());

			FIBITMAP * srcImg = FreeImage_Load(lFormat,lSrcFile.c_str());

			{
				FREE_IMAGE_TYPE  image_type  =  FreeImage_GetImageType ( srcImg ) ;
				unsigned bpp = FreeImage_GetBPP ( srcImg ) ;
				++bpp;

			}


			long lWidth = FreeImage_GetWidth(srcImg);
			long lHeight = FreeImage_GetHeight(srcImg);
			finalImage	lFinalImage(lWidth,lHeight,7);
			lFinalImage.drawImage(0,srcImg);

			lFinalImage.saveToFile("D:/test/final.png");
		}
	};
	
}