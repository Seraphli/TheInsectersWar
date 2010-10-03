
#include<TSSimageUtil.h>
#include<assert.h>

namespace TSS
{
	namespace imageUtil
	{

		///裁剪Src使其覆盖在Dest线上,起始点为0;
		cutOutResult	lineCutOut(int beginPos,unsigned SrcLength,unsigned DestLength)
		{
			int srcLength=(int)SrcLength;
			int destLength=(int)DestLength;
			cutOutResult	out={0,0,0};
			if(beginPos>destLength||-beginPos>srcLength)
				return out;
			if(beginPos<0)
			{
				out.srcBegin=-beginPos;
			}
			else
				out.destBegin=beginPos;

			unsigned destEnd=beginPos+srcLength;
			if(destEnd>DestLength)
				destEnd=DestLength;
			out.length=destEnd-out.destBegin;//没有+1,因为0为第一位
			return out;
		}

		TSS_API cutOut2dResult cutOut2D(int widthBeginPos,int heightBeginPos
			,unsigned srcWidth,unsigned srcHeight
			,unsigned destWidth,unsigned destHeight)
		{
			cutOut2dResult out={{0,0,0},{0,0,0}};
			out.widthCut=lineCutOut(widthBeginPos,srcWidth,destWidth);
			out.heightCut=lineCutOut(heightBeginPos,srcHeight,destHeight);
			return out;
		}

		///剪裁2D图片使其能覆盖到目标上
		cutOut2DImgResult cutOut2DImg(int posU,int posV
			,void* src,unsigned srcWidth,unsigned srcHeight
			,void* dest,unsigned destWidth,unsigned destHeight,unsigned bytesPerPixel)
		{
			cutOut2DImgResult out={0,0,0,0};
			//裁剪
			cutOut2dResult	cut2d=cutOut2D(posU,posV,srcWidth,srcHeight,destWidth,destHeight);

			cutOutResult&	widthCut=cut2d.widthCut;
			if(widthCut.length==0)
				return out;
			cutOutResult&	heightCut=cut2d.heightCut;
			if(heightCut.length==0)
				return out;

			out.width=widthCut.length;
			out.height=heightCut.length;

			out.srcBegin=(byte*)src
				+(heightCut.srcBegin*srcWidth+widthCut.srcBegin)*bytesPerPixel;

			out.destBegin=(byte*)dest
				+(heightCut.destBegin*destWidth+widthCut.destBegin)*bytesPerPixel;
			return out;
		}

		///将一个图片画在另一个图片上,
		void drawPicture(int posU,int posV
			,void* src,unsigned srcWidth,unsigned srcHeight
			,void* dest,unsigned destWidth,unsigned destHeight,unsigned bytesPerPixel
			,void* backgroundColor
			)
		{
			cutOut2DImgResult cut
				=cutOut2DImg(posU,posV,src,srcWidth,srcHeight,dest,destWidth,destHeight,bytesPerPixel);
			if(!cut.destBegin)
				return;

			byte* srcImg=(byte*)cut.srcBegin;
			byte* p=(byte*)cut.destBegin;
			if(backgroundColor==NULL)//不需去除背景色
				for(int h=0;h<cut.height;h++)
				{
					memcpy(p,srcImg,bytesPerPixel*cut.width);
					srcImg+=srcWidth*bytesPerPixel;
					p+=destWidth*bytesPerPixel;
				}
			else		
				for(int h=0;h<cut.height;h++)
				{
					byte* write=p;
					byte* read=srcImg;
					for(int w=0;w<cut.width;w++)
					{
						if(memcmp(backgroundColor,read,bytesPerPixel) )
							memcpy(write,read,bytesPerPixel);

						write+=bytesPerPixel;
						read+=bytesPerPixel;
					}
					srcImg+=srcWidth*bytesPerPixel;
					p+=destWidth*bytesPerPixel;
				}

		}
		
		void drawPicture(int posU,int posV
			,imageData&	src
			,imageData& dest
			,void* backgroundColor
			)
		{
			if(src.bytesPerPixel!=dest.bytesPerPixel)
				{
					//std::cout<<"原图与目标图像素大小不一致"<<std::endl;
					assert(!"原图与目标图像素大小不一致");
					//throw std::exception( "原图与目标图像素大小不一致" );
				}
			drawPicture(posU,posV
				,src.data,src.width,src.height
				,dest.data,dest.width,dest.height
				,dest.bytesPerPixel,backgroundColor);
		}
	}
}