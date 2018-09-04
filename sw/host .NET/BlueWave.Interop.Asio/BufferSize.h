//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

#include "AsioRedirect.h"

#pragma once
#pragma managed
using namespace System;

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
			// represents buffer size info specified by the driver
			public ref class BufferSize
			{
			internal:

				// internal construction only
				BufferSize(IAsio* pAsio);

				// these four things constitute a buffer size
				long m_nMinSize;
				long m_nMaxSize;
				long m_nPreferredSize;
				long m_nGranularity;

			public:

				// and this is where you can retrieve them
				property int MinSize { int get(); }
				property int MaxSize { int get(); }
				property int PreferredSize { int get(); }
				property int Granularity { int get(); }
			};
		}
	}
}