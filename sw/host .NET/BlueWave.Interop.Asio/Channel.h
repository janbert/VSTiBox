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
			// represents a single audio channel (input or output) on the soundcard
			public ref class Channel
			{
			internal:

				// pointer to owner
				IAsio* _pDriver;

				// true is this is an input channel
				bool _isInput;

				// the channel name
				String^ _name;

				// the number of samples in the buffer
				int _bufferSize;

				// the range for floats for bitperfect reproduction
				const static float __maxSampleValue = 8388607.0f / 8388608.0f;

				// sample format
				ASIOSampleType _sampleType;

				// pointer to their buffer
				DWORD* _pTheirBuffer0;
				DWORD* _pTheirBuffer1;

				// what buffer should be affected by our indexer
				DWORD* _pTheirCurrentBuffer;

				// internal construction only
				Channel(IAsio* pAsio, bool IsInput, int channelNumber, void* pTheirBuffer0, void* pTheirBuffer1, int bufferSize);

				// what buffer should be affected by our indexer
				void SetDoubleBufferIndex(long doubleBufferIndex);

			public:

				// the channel name
				property String^ Name { String^ get(); }

				// the buffer size in samples
				property int BufferSize { int get(); }

				// the sample type
				property double SampleType { double get(); }

				// indexer for setting the value of sample in the buffer
				property float default[int] { void set(int sample, float value); float get(int sample); }

			};
		};
	}
}