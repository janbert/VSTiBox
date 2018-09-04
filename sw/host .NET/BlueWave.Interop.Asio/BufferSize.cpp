//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

#include "BufferSize.h"

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
			BufferSize::BufferSize(IAsio* pAsio)
			{
				long p1, p2, p3, p4;

				// ask the driver for the four bits of info
				pAsio->getBufferSize(&p1, &p2, &p3, &p4);

				// and set them
				m_nMinSize = p1;
				m_nMaxSize = p2;
				m_nPreferredSize = p3;
				m_nGranularity = p4;
			}

			int BufferSize::MinSize::get()
			{
				return m_nMinSize;
			}

			int BufferSize::MaxSize::get()
			{
				return m_nMaxSize;
			}

			int BufferSize::PreferredSize::get()
			{
				return m_nPreferredSize;
			}

			int BufferSize::Granularity::get()
			{
				return m_nGranularity;
			}
		}
	}
}