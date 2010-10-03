#ifndef __TSS_prerequisites_H__
#define __TSS_prerequisites_H__

//#define TSS_API

#if defined( TS_NONCLIENT_BUILD ) || defined( TSS_NONCLIENT_BUILD )
	#define TSS_API __declspec( dllexport )
#else
	#define TSS_API __declspec( dllimport )
#endif

namespace TSS
{
	//! 8 bit unsigned variable.
	/** This is a typedef for unsigned char, it ensures portability of the engine. */
	#ifdef _MSC_VER
	typedef unsigned __int8		u8;
	#else
	typedef unsigned char		u8;
	#endif

	typedef	u8					byte;

	//! 8 bit signed variable.
	/** This is a typedef for signed char, it ensures portability of the engine. */
	#ifdef _MSC_VER
	typedef __int8			s8;
	#else
	typedef signed char		s8;
	#endif

	//! 8 bit character variable.
	/** This is a typedef for char, it ensures portability of the engine. */
	typedef char			c8;



	//! 16 bit unsigned variable.
	/** This is a typedef for unsigned short, it ensures portability of the engine. */
	#ifdef _MSC_VER
	typedef unsigned __int16	u16;
	#else
	typedef unsigned short		u16;
	#endif

	//! 16 bit signed variable.
	/** This is a typedef for signed short, it ensures portability of the engine. */
	#ifdef _MSC_VER
	typedef __int16			s16;
	#else
	typedef signed short		s16;
	#endif



	//! 32 bit unsigned variable.
	/** This is a typedef for unsigned int, it ensures portability of the engine. */
	#ifdef _MSC_VER
	typedef unsigned __int32	u32;
	#else
	typedef unsigned int		u32;
	#endif

	//! 32 bit signed variable.
	/** This is a typedef for signed int, it ensures portability of the engine. */
	#ifdef _MSC_VER
	typedef __int32			s32;
	#else
	typedef signed int		s32;
	#endif


	//! 32 bit floating point variable.
	/** This is a typedef for float, it ensures portability of the engine. */
	typedef float				f32;

	//! 64 bit floating point variable.
	/** This is a typedef for double, it ensures portability of the engine. */
	typedef double				f64;
}

#endif