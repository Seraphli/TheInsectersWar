#include <iostream>
#include "combineImage.h"

void  FreeImageErrorHandler (FREE_IMAGE_FORMAT  fif  ,   const char* meg ) 
{

	std::cout<<meg<<std::endl;
}

void main()
{
	FreeImage_SetOutputMessage( FreeImageErrorHandler ) ;
	std::string path;
	std::string extensionName;
	//int num = 0;

	//std::cout<<"path"<<std::endl;
	//std::cin>>path;

	//std::cout<<"extensionName"<<std::endl;
	//std::cin>>extensionName;

	//std::cout<<"file number"<<std::endl;
	//std::cin>>num;

	//zz::combineImage::combine(path,extensionName,num);

	long imgBeginNum = 0;
	long imgEndNum = 0;

	std::cout<<"path"<<std::endl;
	std::cin>>path;

	std::cout<<"extensionName"<<std::endl;
	std::cin>>extensionName;

	std::cout<<"imgBeginNum"<<std::endl;
	std::cin>>imgBeginNum;

	std::cout<<"imgEndNum"<<std::endl;
	std::cin>>imgEndNum;

	zz::combineImage::combine(path,extensionName,imgBeginNum,imgEndNum);


	getchar();
	getchar();
}