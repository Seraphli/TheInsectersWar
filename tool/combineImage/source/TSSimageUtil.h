#ifndef __TSS_imageUtil_H__
#define __TSS_imageUtil_H__

#include"TSSprerequisites.h"
#include"iostream"

namespace TSS
{
	namespace imageUtil
	{
		
		struct cutOutResult
		{
			unsigned srcBegin,destBegin,length;
		};

		///裁剪Src使其覆盖在Dest线上,起始点为0;
		TSS_API cutOutResult	lineCutOut(int beginPos,unsigned SrcLength,unsigned DestLength);


		struct cutOut2dResult
		{
			cutOutResult	widthCut,heightCut;
		};

		TSS_API cutOut2dResult cutOut2D(int widthBeginPos,int heightBeginPos
			,unsigned srcWidth,unsigned srcHeight
			,unsigned destWidth,unsigned destHeight);

		struct cutOut2DImgResult
		{
			///源图开始读入处指针;
			void *srcBegin;
			///目标图开始写入处指针;
			void *destBegin;
			///剪裁后图片宽高
			unsigned	width,height;
		};

		///剪裁2D图片使其能覆盖到目标上
		TSS_API cutOut2DImgResult cutOut2DImg(int posU,int posV
			,void* src,unsigned srcWidth,unsigned srcHeight
			,void* dest,unsigned destWidth,unsigned destHeight,unsigned bytesPerPixel);

		///将src图片画在dest图片上的posU posV 位置上,
		TSS_API void drawPicture(int posU,int posV
			,void* src,unsigned srcWidth,unsigned srcHeight
			,void* dest,unsigned destWidth,unsigned destHeight,unsigned bytesPerPixel
			,void* backgroundColor=NULL//底色,将不会覆盖
			);
		

		struct imageData
		{
			imageData
				(void* Data
				,unsigned Width
				,unsigned Height
				,unsigned BytesPerPixel
				)
				:data(Data),width(Width),height(Height),bytesPerPixel(BytesPerPixel){}

			void* data;
			unsigned width;
			unsigned height;
			unsigned bytesPerPixel;
		};
		
		TSS_API void drawPicture(int posU,int posV
			,imageData&	src
			,imageData& dest
			,void* backgroundColor=NULL//底色,将被忽略
			);
	}
}

#endif