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

		///�ü�Srcʹ�串����Dest����,��ʼ��Ϊ0;
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
			///Դͼ��ʼ���봦ָ��;
			void *srcBegin;
			///Ŀ��ͼ��ʼд�봦ָ��;
			void *destBegin;
			///���ú�ͼƬ���
			unsigned	width,height;
		};

		///����2DͼƬʹ���ܸ��ǵ�Ŀ����
		TSS_API cutOut2DImgResult cutOut2DImg(int posU,int posV
			,void* src,unsigned srcWidth,unsigned srcHeight
			,void* dest,unsigned destWidth,unsigned destHeight,unsigned bytesPerPixel);

		///��srcͼƬ����destͼƬ�ϵ�posU posV λ����,
		TSS_API void drawPicture(int posU,int posV
			,void* src,unsigned srcWidth,unsigned srcHeight
			,void* dest,unsigned destWidth,unsigned destHeight,unsigned bytesPerPixel
			,void* backgroundColor=NULL//��ɫ,�����Ḳ��
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
			,void* backgroundColor=NULL//��ɫ,��������
			);
	}
}

#endif