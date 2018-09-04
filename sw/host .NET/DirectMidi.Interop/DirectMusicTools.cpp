/*
 ____   _  ____  _____ _____ ______  __  __  _  ____   _       __   _   _____ ______
|  _ \ | ||  _ \|  __//  __//_   _/ |  \/  || ||  _ \ | |     |   \| | |  __//_   _/
| |_| || || |> /|  _| | <__   | |   | |\/| || || |_| || |  _  | |\   | |  _|   | |
|____/ |_||_|\_\|____\\____/  |_|   |_|  |_||_||____/ |_| |_| |_| \__| |____\  |_|

////////////////////////////////////////////////////////////////////////////////////
  
  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.
 
  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.
 
  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 
 
Copyright (c) 2007 by Jason Hendrickson
Based on the DirectMidi library by Carlos Jiménez de Parga

All rights reserved.
For any suggestion, please contact me by:
e-mail: jivinstev@hotmail.com

////////////////////////////////////////////////////////////////////////////////////

Version: 2.3b_0.9.1
Module : DirectMusicTools.cpp
Purpose: Code implementation of wrappers around tool graph items in the DirectX headers
Created: JH / 07-19-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

#include "DirectMusicTools.h"


namespace DirectMidi {


//
//	IDirectMusicGraph wrapper
//
IDirectMusicGraph::IDirectMusicGraph()
{
	::IDirectMusicGraph* graph;
	HRESULT hr = CoCreateInstance(CLSID_DirectMusicGraph, NULL, CLSCTX_INPROC, IID_IDirectMusicGraph, reinterpret_cast<void**>(&graph));
	CDMusicException::ASSERT_HR((HR_DMUS)hr);
	base = graph;
}

HR_DMUS IDirectMusicGraph::GetTool(UInt32 dwIndex, IDirectMusicTool^% ppTool)
{
	checkbase();
	ppTool = nullptr;

	::IDirectMusicTool* pTool = NULL;
	HRESULT hr = base->GetTool(dwIndex, &pTool);
	if (SUCCEEDED(hr) && pTool) { ppTool = gcnew IDirectMusicTool(pTool, false); }	//GetTool added reference
	return (HR_DMUS)hr;
}

HR_DMUS IDirectMusicGraph::InsertTool(IDirectMusicTool^ pTool, array<UInt32>^ pdwChannels, UInt32 cPChannels, Int32 lIndex)
{
	checkbase();

	if (!pdwChannels) {
		return (HR_DMUS)base->InsertTool(&pTool->baseref(), NULL, 0, lIndex);
	} else {
		pin_ptr<UInt32> chan = &pdwChannels[0];
		return (HR_DMUS)base->InsertTool(&pTool->baseref(), reinterpret_cast<DWORD*>(chan), cPChannels, lIndex);
	}
}

HR_DMUS IDirectMusicGraph::RemoveTool(IDirectMusicTool^ pTool)
{
	checkbase();
	return (HR_DMUS)base->RemoveTool(&pTool->baseref());
}

HR_DMUS IDirectMusicGraph::StampPMsg(DMUS_PMSG^ pPMSG)
{
	checkbase();
	return (HR_DMUS)base->StampPMsg(&pPMSG->baseref());
}

//
//	IDirectMusicTool wrapper
//
HR_DMUS IDirectMusicTool::Flush(IDirectMusicPerformance^ pPerf, DMUS_PMSG^ pMSG, Int64 rtTime)
{
	checkbase();
	return (HR_DMUS)base->Flush(&pPerf->baseref(), &pMSG->baseref(), rtTime);
}

HR_DMUS IDirectMusicTool::GetMediaTypeArraySize(UInt32% pdwNumElements)
{
	checkbase();
	DWORD dw;
	HRESULT hr = base->GetMediaTypeArraySize(&dw);
	pdwNumElements = dw;
	return (HR_DMUS)hr;
}

HR_DMUS IDirectMusicTool::GetMediaTypes(array<DMUS_PMSGT>^% psdwMediaTypes, UInt32 dwNumElements)
{
	checkbase();

	psdwMediaTypes = gcnew array<DMUS_PMSGT>(dwNumElements);
	pin_ptr<DMUS_PMSGT> mt = &psdwMediaTypes[0];
	DWORD* pmt = reinterpret_cast<DWORD*>(mt);
	return (HR_DMUS)base->GetMediaTypes(&pmt, dwNumElements);
}

HR_DMUS IDirectMusicTool::GetMsgDeliveryType(DMUS_PMSGF_TOOL% pdwDeliveryType)
{
	checkbase();
	DWORD dw;
	HRESULT hr = base->GetMsgDeliveryType(&dw);
	if (SUCCEEDED(hr)) {
		pdwDeliveryType = static_cast<DMUS_PMSGF_TOOL>(dw);
	}
	return (HR_DMUS)hr;
}

HR_DMUS IDirectMusicTool::Init(IDirectMusicGraph^ pGraph)
{
	checkbase();
	return (HR_DMUS)base->Init(&pGraph->baseref());
}

HR_DMUS IDirectMusicTool::ProcessPMsg(IDirectMusicPerformance^ pPerf, DMUS_PMSG^ pMsg)
{
	checkbase();
	return (HR_DMUS)base->ProcessPMsg(&pPerf->baseref(), &pMsg->baseref());
}

//
//
//		Custom DirectMusicTool implementations
//
//


DMUS_PMSG^ DMUS_PMSG::FromPMSG(::DMUS_PMSG* msg)
{
	if (!msg) { return nullptr; }

	switch (msg->dwType) {
		case DMUS_PMSGT_MIDI:				return gcnew DMUS_MIDI_PMSG(reinterpret_cast<::DMUS_MIDI_PMSG*>(msg));
		case DMUS_PMSGT_NOTE:				return gcnew DMUS_NOTE_PMSG(reinterpret_cast<::DMUS_NOTE_PMSG*>(msg));
		case DMUS_PMSGT_SYSEX:				return gcnew DMUS_SYSEX_PMSG(reinterpret_cast<::DMUS_SYSEX_PMSG*>(msg));
		case DMUS_PMSGT_NOTIFICATION:		return gcnew DMUS_NOTIFICATION_PMSG(reinterpret_cast<::DMUS_NOTIFICATION_PMSG*>(msg)); 
		case DMUS_PMSGT_TEMPO:				return gcnew DMUS_TEMPO_PMSG(reinterpret_cast<::DMUS_TEMPO_PMSG*>(msg));
		case DMUS_PMSGT_CURVE:				return gcnew DMUS_CURVE_PMSG(reinterpret_cast<::DMUS_CURVE_PMSG*>(msg));
		case DMUS_PMSGT_TIMESIG:			return gcnew DMUS_TIMESIG_PMSG(reinterpret_cast<::DMUS_TIMESIG_PMSG*>(msg));
		case DMUS_PMSGT_PATCH:				return gcnew DMUS_PATCH_PMSG(reinterpret_cast<::DMUS_PATCH_PMSG*>(msg));
		case DMUS_PMSGT_TRANSPOSE:			return gcnew DMUS_TRANSPOSE_PMSG(reinterpret_cast<::DMUS_TRANSPOSE_PMSG*>(msg));
		case DMUS_PMSGT_CHANNEL_PRIORITY:	return gcnew DMUS_CHANNEL_PRIORITY_PMSG(reinterpret_cast<::DMUS_CHANNEL_PRIORITY_PMSG*>(msg));
		case DMUS_PMSGT_STOP:				return gcnew DMUS_PMSG(msg);
		case DMUS_PMSGT_DIRTY:				return gcnew DMUS_PMSG(msg);
		case DMUS_PMSGT_WAVE:				return gcnew DMUS_WAVE_PMSG(reinterpret_cast<::DMUS_WAVE_PMSG*>(msg));
		case DMUS_PMSGT_LYRIC:				return gcnew DMUS_LYRIC_PMSG(reinterpret_cast<::DMUS_LYRIC_PMSG*>(msg));
		case DMUS_PMSGT_SCRIPTLYRIC:		return gcnew DMUS_LYRIC_PMSG(reinterpret_cast<::DMUS_LYRIC_PMSG*>(msg));
		default:							return gcnew DMUS_PMSG(msg);
	}
}


//
//	COM wrapper
//
STDMETHODIMP CDirectMusicToolWrapper::Init(::IDirectMusicGraph* pGraph)
{
	try { return (HRESULT)m_iface->Init(gcnew IDirectMusicGraph(pGraph, true)); }
	catch (Exception^) { return E_UNEXPECTED; }
}

STDMETHODIMP CDirectMusicToolWrapper::GetMsgDeliveryType(DWORD* pdwDeliveryType)
{
	try
	{ 
		DMUS_PMSGF_TOOL t;
		HRESULT hr = (HRESULT)m_iface->GetMsgDeliveryType(t);
		*pdwDeliveryType = static_cast<DWORD>(t);
		return hr;
	}
	catch (Exception^) { return E_UNEXPECTED; }
}

STDMETHODIMP CDirectMusicToolWrapper::GetMediaTypeArraySize(DWORD* pdwNumElements)
{
	try
	{ 
		UInt32 n;
		HRESULT hr = (HRESULT)m_iface->GetMediaTypeArraySize(n);
		*pdwNumElements = n;
		return hr;
	}
	catch (Exception^) { return E_UNEXPECTED; }
}

STDMETHODIMP CDirectMusicToolWrapper::GetMediaTypes(DWORD** padwMediaTypes, DWORD dwNumElements)
{
	try
	{
		array<DMUS_PMSGT>^ types = nullptr;
		if (padwMediaTypes)
		{
			types = gcnew array<DMUS_PMSGT>(dwNumElements);
		}

		HRESULT hr = (HRESULT)m_iface->GetMediaTypes(types, dwNumElements);
		if (padwMediaTypes)
		{
			pin_ptr<DMUS_PMSGT> pTypes = &types[0];
			memcpy(*padwMediaTypes, pTypes, sizeof(DWORD) * dwNumElements);
		}
		return hr;
	}
	catch (Exception^) { return E_UNEXPECTED; }
}

STDMETHODIMP CDirectMusicToolWrapper::ProcessPMsg(::IDirectMusicPerformance* pPerf, ::DMUS_PMSG* pPMSG)
{
	try { return (HRESULT)m_iface->ProcessPMsg(gcnew IDirectMusicPerformance(pPerf, true), DMUS_PMSG::FromPMSG(pPMSG)); }
	catch (Exception^) { return E_UNEXPECTED; }
}

STDMETHODIMP CDirectMusicToolWrapper::Flush(::IDirectMusicPerformance* pPerf, ::DMUS_PMSG* pPMSG, REFERENCE_TIME rtTime)
{
	try { return (HRESULT)m_iface->Flush(gcnew IDirectMusicPerformance(pPerf, true), DMUS_PMSG::FromPMSG(pPMSG), rtTime); }
	catch (Exception^) { return E_UNEXPECTED; }
}

IDirectMusicTool^ CDirectMusicTool::QueryInterface()
{
	return gcnew IDirectMusicTool(new CDirectMusicToolWrapper(this), false);	//by calling 'new' we already added a reference
}



}	//namespace DirectMidi
