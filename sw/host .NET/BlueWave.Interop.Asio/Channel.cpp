//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

//#include "AsioRedirect.h"
#include "Channel.h"

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
			Channel::Channel(IAsio* pAsio, bool IsInput, int channelNumber, void* pTheirBuffer0, void* pTheirBuffer1, int bufferSize)
			{
				// remember the two buffers (one plays the other updates)
				_pTheirBuffer0 = (DWORD*)pTheirBuffer0;
				_pTheirBuffer1 = (DWORD*)pTheirBuffer1;

				// and the size
				_bufferSize = bufferSize;

				// we need one of these to query the driver
				ASIOChannelInfo* pChannelInfo = new ASIOChannelInfo();

				// populated with this
				pChannelInfo->channel = channelNumber;
				pChannelInfo->isInput = IsInput;

				// now we can get the data
				pAsio->getChannelInfo(pChannelInfo);

				// get channelinfo
				_isInput = pChannelInfo->isInput != 0;
				_name = gcnew String(pChannelInfo->name);
				_sampleType = pChannelInfo->type;
			}

			String^ Channel::Name::get()
			{
				return _name;
			}

			int Channel::BufferSize::get()
			{
				return _bufferSize;
			}

			double Channel::SampleType::get()
			{
				return _sampleType;
			}

			void Channel::SetDoubleBufferIndex(long doubleBufferIndex)
			{
				if (doubleBufferIndex == 0)
				{
					_pTheirCurrentBuffer = _pTheirBuffer0;
				}
				else
				{
					_pTheirCurrentBuffer = _pTheirBuffer1;
				}
			}

			void Channel::default::set(int sample, float value)
			{
				// clip value to avoid problems with conversion.
				if (value > __maxSampleValue)
				{
					value = __maxSampleValue;
				}
				else if (value < -1.0f)
				{
					value = -1.0f;
				}
			    
				// convert float between -1.0 and 1.0 to signed integer
				_pTheirCurrentBuffer[sample] = (DWORD)(value * 2147483648.0f);
			}

			float Channel::default::get(int sample)
			{
				// convert signed integer to float between -1.0 and 1.0
				return (float)((int)_pTheirCurrentBuffer[sample] / 2147483648.0f);
			}
		}
	}
}