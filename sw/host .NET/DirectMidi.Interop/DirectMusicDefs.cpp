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
Module : DirectMusicDefs.cpp
Purpose: Code implementation of wrappers around DirectX interfaces
Created: JH / 07-01-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

#include "DirectMusicDefs.h"
#include "DirectBase.h"
#include "DirectMusicTools.h"

//guid definitions
#pragma comment (lib,"dxguid.lib")
#pragma comment (lib,"winmm.lib")
#pragma comment (lib,"dsound.lib")
#pragma comment (lib,"dxerr9.lib")

namespace DirectMidi {

//
//	IReferenceClock wrapper
//
HR_DMUS IReferenceClock::GetTime(Int64% pTime)
{
	checkbase();
	REFERENCE_TIME time;
	HR_DMUS ret = static_cast<HR_DMUS>(base->GetTime(&time));
	pTime = time;
	return ret;
}

HR_DMUS IReferenceClock::AdviseTime(Int64 rtBaseTime, Int64 rtStreamTime, IntPtr hEvent, UInt32% pdwAdviseCookie)
{
	checkbase();
	DWORD cookie;
	HR_DMUS ret = static_cast<HR_DMUS>(base->AdviseTime(rtBaseTime, rtStreamTime, static_cast<HANDLE>(hEvent.ToPointer()), &cookie));
	pdwAdviseCookie = cookie;
	return ret;
}

HR_DMUS IReferenceClock::AdvisePeriodic(Int64 rtStartTime, Int64 rtPeriodTime, IntPtr hSemaphore, UInt32% pdwAdviseCookie)
{
	checkbase();
	DWORD cookie;
	HR_DMUS ret = static_cast<HR_DMUS>(base->AdvisePeriodic(rtStartTime, rtPeriodTime, static_cast<HANDLE>(hSemaphore.ToPointer()), &cookie));
	pdwAdviseCookie = cookie;
	return ret;
}

HR_DMUS IReferenceClock::Unadvise(UInt32 dwAdviseCookie)
{
	checkbase();
	return static_cast<HR_DMUS>(base->Unadvise(dwAdviseCookie));
}

//
//	IDirectMusicSegment wrapper
//
static Type^ GetParamObjectRequired(Guid rguidType)
{
//	if (rguidType.Equals(ParamGUID::BandParam)) { return DMUS_BAND_PARAM::typeid; }	//NOTE: not wrapped - contains COM interface
//TODO: implement		if (rguidType.Equals(ParamGUID::ChordParam)) { return DMUS_CHORD_PARAM::typeid; }	//based on DMUS_CHORD_PARAM, typedef'd from DMUS_CHORD_KEY
	if (rguidType.Equals(ParamGUID::Clear_All_Bands)) { return Void::typeid; }
	if (rguidType.Equals(ParamGUID::CommandParam)) { return DMUS_COMMAND_PARAM::typeid; }
	if (rguidType.Equals(ParamGUID::CommandParam2)) { return DMUS_COMMAND_PARAM_2::typeid; }
	if (rguidType.Equals(ParamGUID::CommandParamNext)) { return DMUS_COMMAND_PARAM_2::typeid; }
//	if (rguidType.Equals(ParamGUID::ConnectToDLSCollection)) { return IDirectMusicCollection8::typeid; }	//NOTE: not wrapped - contains COM interface
	if (rguidType.Equals(ParamGUID::Disable_Auto_Download)) { return Void::typeid; }
	if (rguidType.Equals(ParamGUID::DisableTempo)) { return Void::typeid; }
	if (rguidType.Equals(ParamGUID::DisableTimeSig)) { return Void::typeid; }
//	if (rguidType.Equals(ParamGUID::Download)) { return IDirectMusicPerformance8::typeid; }	//NOTE: not wrapped - contains COM interface
//	if (rguidType.Equals(ParamGUID::DownloadToAudioPath)) { return IDirectMusicAudioPath8::typeid; }	//NOTE: not wrapped - contains COM interface
	if (rguidType.Equals(ParamGUID::Enable_Auto_Download)) { return Void::typeid; }
	if (rguidType.Equals(ParamGUID::EnableTempo)) { return Void::typeid; }
	if (rguidType.Equals(ParamGUID::EnableTimeSig)) { return Void::typeid; }
//	if (rguidType.Equals(ParamGUID::IDirectMusicBand)) { return IDirectMusicBand8::typeid; }	//NOTE: not wrapped - contains COM interface
//	if (rguidType.Equals(ParamGUID::IDirectMusicChordMap)) { return IDirectMusicChordMap8::typeid; }	//NOTE: not wrapped - contains COM interface
//	if (rguidType.Equals(ParamGUID::IDirectMusicStyle)) { return IDirectMusicStyle::typeid; }	//NOTE: not wrapped - contains COM interface
	if (rguidType.Equals(ParamGUID::MuteParam)) { return DMUS_MUTE_PARAM::typeid; }
	if (rguidType.Equals(ParamGUID::Play_Marker)) { return DMUS_PLAY_MARKER_PARAM::typeid; }
	if (rguidType.Equals(ParamGUID::RhythmParam)) { return DMUS_RHYTHM_PARAM::typeid; }
	if (rguidType.Equals(ParamGUID::SeedVariations)) { return Int32::typeid; }
	if (rguidType.Equals(ParamGUID::StandardMIDIFile)) { return Void::typeid; }
	if (rguidType.Equals(ParamGUID::TempoParam)) { return DMUS_TEMPO_PARAM::typeid; }
	if (rguidType.Equals(ParamGUID::TimeSignature)) { return DMUS_TIMESIGNATURE::typeid; }
//	if (rguidType.Equals(ParamGUID::Unload)) { return IDirectMusicPerformance8::typeid; }	//NOTE: not wrapped - contains COM interface
//	if (rguidType.Equals(ParamGUID::UnloadFromAudioPath)) { return IDirectMusicAudioPath8::typeid; }	//NOTE: not wrapped - contains COM interface
	if (rguidType.Equals(ParamGUID::Valid_Start_Time)) { return DMUS_VALID_START_PARAM::typeid; }
//TODO: implement		if (rguidType.Equals(ParamGUID::Variations)) { return DMUS_VARIATIONS_PARAM::typeid; }

	throw gcnew ArgumentException(String::Format("Guid {0} is not currently supported", rguidType));
}

static void CheckParamType(Type^ paramType, Object^ param)
{
	//handle void type
	if (paramType->Equals(Void::typeid))
	{
		if (!param) { return; }
		throw gcnew ArgumentException(String::Format("Void parameter expected; null needs to be passed"));
	}
	
	//handle null ptr
	if (!param)
	{
		throw gcnew ArgumentException(String::Format("Parameter type {0} expected; null passed", paramType->FullName));
	}

	//ensure types are as expected
	if (!param->GetType()->Equals(paramType))
	{
		throw gcnew ArgumentException(String::Format("Passed parameter type {0}; expected {1}", param->GetType()->FullName, paramType->FullName));
	}
}

static void* GetRefParamPtr(Type^ paramType, Object^ param)
{
//TODO: implement	if (paramType->Equals(DMUS_CHORD_PARAM::typeid)) { return &((DMUS_CHORD_PARAM^)param)->baseref(); }
//TODO: implement	if (paramType->Equals(DMUS_VARIATIONS_PARAM::typeid)) { return &((DMUS_VARIATIONS_PARAM^)param)->baseref(); }

	throw gcnew ArgumentException(String::Format("Type {0} is not currently supported", paramType->FullName));
}

struct ValueParamsToPass
{
	::IDirectMusicSegment* iface;
	GUID rguidType;
	DWORD dwGroupBits;
	DWORD dwIndex;
	MUSIC_TIME mtTime;
	MUSIC_TIME* pmtNext;

	ValueParamsToPass(::IDirectMusicSegment* iface, REFGUID rguidType, DWORD dwGroupBits, DWORD dwIndex, MUSIC_TIME mtTime, MUSIC_TIME* pmtNext = NULL)
	{
		this->iface = iface;
		this->rguidType = rguidType;
		this->dwGroupBits = dwGroupBits;
		this->dwIndex = dwIndex;
		this->mtTime = mtTime;
		this->pmtNext = pmtNext;
	}
};

template<typename T>
static HR_DMUS GetOrSetValueParam(ValueParamsToPass& pp, T% param)
{
	pin_ptr<T> pParam = &param;
	if (pp.pmtNext)
	{
		return static_cast<HR_DMUS>(pp.iface->GetParam(pp.rguidType, pp.dwGroupBits, pp.dwIndex, pp.mtTime, pp.pmtNext, pParam));
	}
	else
	{
		return static_cast<HR_DMUS>(pp.iface->SetParam(pp.rguidType, pp.dwGroupBits, pp.dwIndex, pp.mtTime, pParam));
	}
}

static HR_DMUS GetOrSetTypeValueParam(ValueParamsToPass& pp, Object^ value)
{
	if (value->GetType()->Equals(DMUS_COMMAND_PARAM::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_COMMAND_PARAM^)value); }
	if (value->GetType()->Equals(DMUS_COMMAND_PARAM_2::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_COMMAND_PARAM_2^)value); }
	if (value->GetType()->Equals(DMUS_MUTE_PARAM::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_MUTE_PARAM^)value); }
	if (value->GetType()->Equals(DMUS_PLAY_MARKER_PARAM::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_PLAY_MARKER_PARAM^)value); }
	if (value->GetType()->Equals(DMUS_RHYTHM_PARAM::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_RHYTHM_PARAM^)value); }
	if (value->GetType()->Equals(Int32::typeid))	{ return GetOrSetValueParam(pp, *(Int32^)value); }
	if (value->GetType()->Equals(DMUS_TEMPO_PARAM::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_TEMPO_PARAM^)value); }
	if (value->GetType()->Equals(DMUS_TIMESIGNATURE::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_TIMESIGNATURE^)value); }
	if (value->GetType()->Equals(DMUS_VALID_START_PARAM::typeid))	{ return GetOrSetValueParam(pp, *(DMUS_VALID_START_PARAM^)value); }

	throw gcnew ArgumentException(String::Format("Type {0} is not currently supported", value->GetType()->FullName));
}

HR_DMUS IDirectMusicSegment::GetParam(Guid rguidType, UInt32 dwGroupBits, UInt32 dwIndex, Int32 mtTime, Int32% pmtNext, Object^% pParam)
{
	checkbase();

	MUSIC_TIME mtNext;
	HR_DMUS ret;
	GUID gtype = fromGuid(rguidType);

	//
	//	if the type is void, return empty;
	//	if null is passed, create the return object;
	//	otherwise, check that the type passed in is appropriate
	//
	Type^ paramType = GetParamObjectRequired(rguidType);
	if (paramType->Equals(Void::typeid))
	{
		pParam = nullptr;
	}
	else if (!pParam)
	{
		pParam = Activator::CreateInstance(paramType);
		if (!pParam)
		{
			throw gcnew ArgumentException(String::Format("Type {0} could not be created by Activator::CreateInstance", paramType->FullName));
		}
	}
	else if (!paramType->Equals(pParam->GetType()))
	{
		throw gcnew ArgumentException(String::Format("Type {0} was passed in; expected NULL or type {1}", pParam->GetType()->FullName, paramType->FullName));
	}

	//
	//	based on the parameter type, get a pointer to the obj and call GetParam
	//
	if (!pParam)
	{
		ret = static_cast<HR_DMUS>(base->GetParam(gtype, dwGroupBits, dwIndex, mtTime, &mtNext, NULL));
	}
	else if (paramType->IsValueType)
	{
		ValueParamsToPass pp(base, gtype, dwGroupBits, dwIndex, mtTime, &mtNext);
		ret = GetOrSetTypeValueParam(pp, pParam);
	}
	else
	{
		void* param = GetRefParamPtr(paramType, pParam);
		ret = static_cast<HR_DMUS>(base->GetParam(gtype, dwGroupBits, dwIndex, mtTime, &mtNext, param));
	}

	pmtNext = mtNext;
	return ret;
}

HR_DMUS IDirectMusicSegment::SetParam(Guid rguidType, UInt32 dwGroupBits, UInt32 dwIndex, Int32 mtTime, Object^ pParam)
{
	checkbase();
	HR_DMUS ret;
	GUID gtype = fromGuid(rguidType);

	//
	//	Get the required type, and check the parameters passed in
	//
	Type^ paramType = GetParamObjectRequired(rguidType);
	CheckParamType(paramType, pParam);

	//
	//	based on the parameter type, get a pointer to the obj and call GetParam
	//
	if (!pParam)
	{
		ret = static_cast<HR_DMUS>(base->SetParam(gtype, dwGroupBits, dwIndex, mtTime, NULL));
	}
	else if (paramType->IsValueType)
	{
		ValueParamsToPass pp(base, gtype, dwGroupBits, dwIndex, mtTime);
		ret = GetOrSetTypeValueParam(pp, pParam);
	}
	else
	{
		void* param = GetRefParamPtr(paramType, pParam);
		ret = static_cast<HR_DMUS>(base->SetParam(gtype, dwGroupBits, dwIndex, mtTime, param));
	}

	return ret;
}

HR_DMUS IDirectMusicSegment::GetGraph([Out] IDirectMusicGraph^% ppGraph)
{
	checkbase();
	ppGraph = nullptr;

	::IDirectMusicGraph* graph = NULL;
	HRESULT hr = base->GetGraph(&graph);
	if (SUCCEEDED(hr) && graph) { ppGraph = gcnew IDirectMusicGraph(graph, false); }	//GetGraph adds a reference already
	return static_cast<HR_DMUS>(hr);

}

HR_DMUS IDirectMusicSegment::SetGraph(IDirectMusicGraph^ ppGraph)
{
	checkbase();
	return static_cast<HR_DMUS>(base->SetGraph(ppGraph ? &ppGraph->baseref() : NULL));
}



//
//	Interfaces for GetObjectInPath
//
HR_DMUS IDirectSoundFXGargle::SetAllParameters(const DSFXGargle% pcDsFxGargle)
{
	checkbase();
	pin_ptr<const DSFXGargle> rev = &pcDsFxGargle;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXGargle*>(rev)));
}

HR_DMUS IDirectSoundFXGargle::GetAllParameters([Out] DSFXGargle% pDsFxGargle)
{
	checkbase();
	pin_ptr<DSFXGargle> rev = &pDsFxGargle;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXGargle*>(rev)));
}

HR_DMUS IDirectSoundFXChorus::SetAllParameters(const DSFXChorus% pcDsFxChorus)
{
	checkbase();
	pin_ptr<const DSFXChorus> rev = &pcDsFxChorus;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXChorus*>(rev)));
}

HR_DMUS IDirectSoundFXChorus::GetAllParameters([Out] DSFXChorus% pDsFxChorus)
{
	checkbase();
	pin_ptr<DSFXChorus> rev = &pDsFxChorus;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXChorus*>(rev)));
}

HR_DMUS IDirectSoundFXFlanger::SetAllParameters(const DSFXFlanger% pcDsFxFlanger)
{
	checkbase();
	pin_ptr<const DSFXFlanger> rev = &pcDsFxFlanger;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXFlanger*>(rev)));
}

HR_DMUS IDirectSoundFXFlanger::GetAllParameters([Out] DSFXFlanger% pDsFxFlanger)
{
	checkbase();
	pin_ptr<DSFXFlanger> rev = &pDsFxFlanger;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXFlanger*>(rev)));
}

HR_DMUS IDirectSoundFXEcho::SetAllParameters(const DSFXEcho% pcDsFxEcho)
{
	checkbase();
	pin_ptr<const DSFXEcho> rev = &pcDsFxEcho;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXEcho*>(rev)));
}

HR_DMUS IDirectSoundFXEcho::GetAllParameters([Out] DSFXEcho% pDsFxEcho)
{
	checkbase();
	pin_ptr<DSFXEcho> rev = &pDsFxEcho;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXEcho*>(rev)));
}

HR_DMUS IDirectSoundFXDistortion::SetAllParameters(const DSFXDistortion% pcDsFxDistortion)
{
	checkbase();
	pin_ptr<const DSFXDistortion> rev = &pcDsFxDistortion;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXDistortion*>(rev)));
}

HR_DMUS IDirectSoundFXDistortion::GetAllParameters([Out] DSFXDistortion% pDsFxDistortion)
{
	checkbase();
	pin_ptr<DSFXDistortion> rev = &pDsFxDistortion;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXDistortion*>(rev)));
}

HR_DMUS IDirectSoundFXCompressor::SetAllParameters(const DSFXCompressor% pcDsFxCompressor)
{
	checkbase();
	pin_ptr<const DSFXCompressor> rev = &pcDsFxCompressor;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXCompressor*>(rev)));
}

HR_DMUS IDirectSoundFXCompressor::GetAllParameters([Out] DSFXCompressor% pDsFxCompressor)
{
	checkbase();
	pin_ptr<DSFXCompressor> rev = &pDsFxCompressor;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXCompressor*>(rev)));
}

HR_DMUS IDirectSoundFXParamEq::SetAllParameters(const DSFXParamEq% pcDsFxParamEq)
{
	checkbase();
	pin_ptr<const DSFXParamEq> rev = &pcDsFxParamEq;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXParamEq*>(rev)));
}

HR_DMUS IDirectSoundFXParamEq::GetAllParameters([Out] DSFXParamEq% pDsFxParamEq)
{
	checkbase();
	pin_ptr<DSFXParamEq> rev = &pDsFxParamEq;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXParamEq*>(rev)));
}

HR_DMUS IDirectSoundFXWavesReverb::SetAllParameters(const DSFXWavesReverb% pcDsFxWavesReverb)
{
	checkbase();
	pin_ptr<const DSFXWavesReverb> rev = &pcDsFxWavesReverb;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXWavesReverb*>(rev)));
}

HR_DMUS IDirectSoundFXWavesReverb::GetAllParameters([Out] DSFXWavesReverb% pDsFxWavesReverb)
{
	checkbase();
	pin_ptr<DSFXWavesReverb> rev = &pDsFxWavesReverb;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXWavesReverb*>(rev)));
}

HR_DMUS IDirectSoundFXI3DL2Reverb::SetAllParameters(const DSFXI3DL2Reverb% pcDsFxI3DL2Reverb)
{
	checkbase();
	pin_ptr<const DSFXI3DL2Reverb> rev = &pcDsFxI3DL2Reverb;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<const ::DSFXI3DL2Reverb*>(rev)));
}

HR_DMUS IDirectSoundFXI3DL2Reverb::SetQuality(Int32 lQuality)
{
	checkbase();
	return static_cast<HR_DMUS>(base->SetQuality(lQuality));
}

HR_DMUS IDirectSoundFXI3DL2Reverb::GetAllParameters([Out] DSFXI3DL2Reverb% pDsFxI3DL2Reverb)
{
	checkbase();
	pin_ptr<DSFXI3DL2Reverb> rev = &pDsFxI3DL2Reverb;
	return static_cast<HR_DMUS>(base->SetAllParameters(reinterpret_cast<::DSFXI3DL2Reverb*>(rev)));
}

HR_DMUS IDirectSoundFXI3DL2Reverb::GetQuality(Int32% plQuality)
{
	checkbase();
	LONG qual;
	HR_DMUS ret = static_cast<HR_DMUS>(base->GetQuality(&qual));
	plQuality = qual;
	return ret;
}


}	//namespace DirectMidi
