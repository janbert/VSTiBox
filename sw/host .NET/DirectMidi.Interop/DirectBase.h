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
Module : DirectBase.h
Purpose: Definition of wrappers around definitions in CDirectBase.h
Created: JH / 07-01-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

#pragma once
#include "DirectMusicDefs.h"

namespace DirectMidi {

[Flags]
public enum class SET : Byte
{
	NOEFFECT = SET_NOEFFECT,
	REVERB = SET_REVERB,
	CHORUS = SET_CHORUS,
	DELAY = SET_DELAY,
};

public enum class MidiMessage : Byte
{
	NOTE_OFF = directmidi::NOTE_OFF,
	NOTE_ON = directmidi::NOTE_ON,
	PATCH_CHANGE = directmidi::PATCH_CHANGE,
	POLY_PRESSURE = directmidi::POLY_PRESSURE,
	CONTROL_CHANGE = directmidi::CONTROL_CHANGE,
	CHANNEL_PREASURE = directmidi::CHANNEL_PREASURE,
	PITCH_BEND = directmidi::PITCH_BEND,
	START_SYS_EX = directmidi::START_SYS_EX,
	END_SYS_EX = directmidi::END_SYS_EX,
	SONG_POINTER = directmidi::SONG_POINTER,
	SONG_SELECT = directmidi::SONG_SELECT,
	TUNE_REQUEST = directmidi::TUNE_REQUEST,
	TIMING_CLOCK = directmidi::TIMING_CLOCK,
	RESET = directmidi::RESET,
	ACTIVE_SENSING = directmidi::ACTIVE_SENSING,
	START = directmidi::START,
	STOP = directmidi::STOP,
};

public enum class DM_LOCATION : Byte
{
	DM_LOAD_FROM_FILE = directmidi::DM_LOAD_FROM_FILE,
	DM_USE_MEMORY     = directmidi::DM_USE_MEMORY,
};

public ref struct Util abstract sealed
{
	static Int32 TimeCents(Double nTime) { return ::TimeCents(nTime); }
	static Int32 PitchCents(Double nFrequency) { return ::PitchCents(nFrequency); }
	static Int32 GainCents(Double nVoltage, Double nRefVoltage) { return ::GainCents(nVoltage, nRefVoltage); }
	static Int32 PercentUnits(Double nPercent) { return ::PercentUnits(nPercent); }
};


// Infoport structure
public ref class INFOPORT : WrapperBase<directmidi::INFOPORT>
{
public:
	WRAPPER_PROPERTY_STRING(szPortDescription);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_PC, DWORD, dwFlags);
	WRAPPER_PROPERTY(UInt32, dwClass);
	WRAPPER_PROPERTY(UInt32, dwType);
	WRAPPER_PROPERTY(UInt32, dwMemorySize);
	WRAPPER_PROPERTY(UInt32, dwMaxAudioChannels);
	WRAPPER_PROPERTY(UInt32, dwMaxVoices);
	WRAPPER_PROPERTY(UInt32, dwMaxChannelGroups);
	WRAPPER_PROPERTY(UInt32, dwSampleRate);
	WRAPPER_PROPERTY(UInt32, dwEffectFlags);
	WRAPPER_PROPERTY(UInt32, dwfShare);
	WRAPPER_PROPERTY(UInt32, dwFeatures);
	WRAPPER_PROPERTY_GUID(guidSynthGUID);
};

// Instrument Info structure
public ref class INSTRUMENTINFO : WrapperBase<directmidi::INSTRUMENTINFO>
{
public:
	WRAPPER_PROPERTY_STRING(szInstName);
	WRAPPER_PROPERTY(UInt32, dwPatchInCollection);
};

// Clock info structure
public ref class CLOCKINFO : WrapperBase<directmidi::CLOCKINFO>
{
public:
	WRAPPER_PROPERTY_CAST(DMUS_CLOCKTYPE, ctType);
	WRAPPER_PROPERTY_GUID(guidClock);
	WRAPPER_PROPERTY_STRING(szClockDescription);
	WRAPPER_PROPERTY(UInt32, dwFlags);
};

// Synthesizer stats
WRAPPER_STRUCT(SYNTHSTATS, directmidi::SYNTHSTATS)
	WRAPPER_STRUCT_MEMBER(UInt32, dwSize);
	WRAPPER_STRUCT_MEMBER(DMUS_SYNTHSTATS, dwValidStats);
	WRAPPER_STRUCT_MEMBER(UInt32, dwVoices);
	WRAPPER_STRUCT_MEMBER(UInt32, dwTotalCPU);
	WRAPPER_STRUCT_MEMBER(UInt32, dwCPUPerVoice);
	WRAPPER_STRUCT_MEMBER(UInt32, dwLostNotes);
	WRAPPER_STRUCT_MEMBER(UInt32, dwFreeMemory);
	WRAPPER_STRUCT_MEMBER(Int32, lPeakVolume);
	WRAPPER_STRUCT_MEMBER(UInt32, dwSynthMemUse);
WRAPPER_STRUCT_END;

// Region structure
WRAPPER_STRUCT(REGION, directmidi::REGION)
	WRAPPER_STRUCT_MEMBER(RGNRANGE, RangeKey);
	WRAPPER_STRUCT_MEMBER(RGNRANGE, RangeVelocity);
WRAPPER_STRUCT_END;

// Articulation parameters structure
WRAPPER_STRUCT(ARTICPARAMS, directmidi::ARTICPARAMS)
	WRAPPER_STRUCT_MEMBER(DMUS_LFOPARAMS, LFO);
	WRAPPER_STRUCT_MEMBER(DMUS_VEGPARAMS, VolEG);
	WRAPPER_STRUCT_MEMBER(DMUS_PEGPARAMS, PitchEG);
	WRAPPER_STRUCT_MEMBER(DMUS_MSCPARAMS, Misc);
WRAPPER_STRUCT_END;


//
//	CDMusicException definition
//
public ref class CDMusicException : System::Exception
{
	String^ errorToStringRet;
	String^ getErrorDescriptionRet;

public:
	Int32 m_hrCode;
	String^ m_strMethod;
	Int32 m_nLine;

	String^ ErrorToString() { return errorToStringRet; }
	String^ GetErrorDescription() { return getErrorDescriptionRet; }

	static void ASSERT_HR(HR_DMUS hr) 
	{ 
		if (FAILED(hr))
		{ 
			directmidi::CDMusicException ex(_T("CDMusicException::Assert"), (Int32)hr, __LINE__);
			throw gcnew CDMusicException(ex);
		}
	}

private:
	static String^ SafeErrorToString(directmidi::CDMusicException& ex)
	{
		try { return toString(ex.ErrorToString()); }
		catch(...) { return nullptr; }
	}
	static String^ SafeGetErrorDescription(directmidi::CDMusicException& ex)
	{
		try { return toString(ex.GetErrorDescription()); }
		catch(...) { return nullptr; }
	}

internal:
	CDMusicException(directmidi::CDMusicException& ex)
		: Exception(String::Format("Error string: {0}\nError description: {1}", SafeErrorToString(ex), SafeGetErrorDescription(ex)))
	{
		m_hrCode = ex.m_hrCode;
		m_strMethod = toString(ex.m_strMethod);
		m_nLine = ex.m_nLine;
		errorToStringRet = SafeErrorToString(ex);
		getErrorDescriptionRet = SafeGetErrorDescription(ex);
	}
};

#define DMIDI_STATIC_START	\
	try {

#define DMIDI_STATIC_END								\
	} catch(directmidi::CDMusicException& ex) {			\
		throw gcnew DirectMidi::CDMusicException(ex);	\
	}

#define DMIDI_METHOD_START	\
	checkbase();	\
	DMIDI_STATIC_START

#define DMIDI_METHOD_END	DMIDI_STATIC_END

template<typename T>
public ref class DMidiWrapperBase : public WrapperBaseNoConstructor<T>
{
public:
	DMidiWrapperBase()
	{
		DMIDI_STATIC_START;
		base = new T;
		DMIDI_STATIC_END;
	}
};

//
//	When inheriting from a base type defined with WrapperBaseNoDestructor
//	(to keep the object hierarchy correct, and also allow baseref() to be used correctly on the parent class),
//	we have to keep a pointer to the base class AND a pointer to the inheriting class,
//	and these must be kept in sync at all times
//
#define DMIDI_INHERITED_BASE_CONSTRUCTOR(child)		\
	public:											\
		child()										\
		{											\
			DMIDI_STATIC_START;						\
			PARENT::base = base = new TBASE;		\
			DMIDI_STATIC_END;						\
		}

#define DMIDI_INHERITED_BASE(child, parent, cpp)				\
		INHERITED_BASE_NOCONSTRUCTOR(child, parent, cpp);		\
		DMIDI_INHERITED_BASE_CONSTRUCTOR(child);



}	//namespace DirectMidi
