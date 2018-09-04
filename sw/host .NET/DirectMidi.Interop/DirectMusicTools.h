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
Module : DirectMusicDefs.h
Purpose: Definition of wrappers around tool graph items in the DirectX headers
Created: JH / 07-19-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

#pragma once
#include "DirectBase.h"
#include <dmplugin.h>

namespace DirectMidi {

//
//
//		Enumerations
//
//
public enum class DMUS_PMSGT : UInt32
{
	MIDI = DMUS_PMSGT_MIDI,
	NOTE = DMUS_PMSGT_NOTE,
  	SYSEX = DMUS_PMSGT_SYSEX,
	NOTIFICATION = DMUS_PMSGT_NOTIFICATION,
	TEMPO = DMUS_PMSGT_TEMPO,
	CURVE = DMUS_PMSGT_CURVE,
	TIMESIG = DMUS_PMSGT_TIMESIG,
	PATCH = DMUS_PMSGT_PATCH,
	TRANSPOSE = DMUS_PMSGT_TRANSPOSE,
	CHANNEL_PRIORITY = DMUS_PMSGT_CHANNEL_PRIORITY,
	STOP = DMUS_PMSGT_STOP,
	DIRTY = DMUS_PMSGT_DIRTY,
	WAVE = DMUS_PMSGT_WAVE,
	LYRIC = DMUS_PMSGT_LYRIC,
	SCRIPTLYRIC = DMUS_PMSGT_SCRIPTLYRIC,
	USER = DMUS_PMSGT_USER,
};

[Flags]
public enum class DMUS_NOTEF : Byte
{
	NOTEON = DMUS_NOTEF_NOTEON,
	NOINVALIDATE = DMUS_NOTEF_NOINVALIDATE,
	NOINVALIDATE_INSCALE = DMUS_NOTEF_NOINVALIDATE_INSCALE,
	NOINVALIDATE_INCHORD = DMUS_NOTEF_NOINVALIDATE_INCHORD, 
	REGENERATE = DMUS_NOTEF_REGENERATE,
};

[Flags]
public enum class DMUS_PLAYMODE : Byte
{
	//basic flags
	KEY_ROOT = DMUS_PLAYMODE_KEY_ROOT,
	CHORD_ROOT = DMUS_PLAYMODE_CHORD_ROOT,
	SCALE_INTERVALS = DMUS_PLAYMODE_SCALE_INTERVALS,
	CHORD_INTERVALS = DMUS_PLAYMODE_CHORD_INTERVALS,
	NONE = DMUS_PLAYMODE_NONE,

	//compound flags
	ALWAYSPLAY = DMUS_PLAYMODE_ALWAYSPLAY,
	FIXED = DMUS_PLAYMODE_FIXED,
	FIXEDTOCHORD = DMUS_PLAYMODE_FIXEDTOCHORD,
	FIXEDTOKEY = DMUS_PLAYMODE_FIXEDTOKEY,
	MELODIC = DMUS_PLAYMODE_MELODIC,
	NORMALCHORD = DMUS_PLAYMODE_NORMALCHORD,
	PEDALPOINT = DMUS_PLAYMODE_PEDALPOINT,
	PEDALPOINTALWAYS = DMUS_PLAYMODE_PEDALPOINTALWAYS,
	PEDALPOINTCHORD = DMUS_PLAYMODE_PEDALPOINTCHORD,
};

public enum class DMUS_CURVET : Byte
{
	CCCURVE = DMUS_CURVET_CCCURVE,
	MATCURVE = DMUS_CURVET_MATCURVE,
	PATCURVE = DMUS_CURVET_PATCURVE,
	PBCURVE = DMUS_CURVET_PBCURVE,
	RPNCURVE = DMUS_CURVET_RPNCURVE,
	NRPNCURVE = DMUS_CURVET_NRPNCURVE,
};

public enum class DMUS_CURVES : Byte
{
	EXP = DMUS_CURVES_EXP,
	INSTANT = DMUS_CURVES_INSTANT,
	LINEAR = DMUS_CURVES_LINEAR,
	LOG = DMUS_CURVES_LOG,
	SINE = DMUS_CURVES_SINE,
};

[Flags]
public enum class DMUS_CURVE : Byte
{
	RESET = DMUS_CURVE_RESET,
	START_FROM_CURRENT = DMUS_CURVE_START_FROM_CURRENT,
};

public ref struct GUID_NOTIFICATION abstract sealed
{
	static Guid CHORD = toGuid(GUID_NOTIFICATION_CHORD);
	static Guid COMMAND = toGuid(GUID_NOTIFICATION_COMMAND);
	static Guid MEASUREANDBEAT = toGuid(GUID_NOTIFICATION_MEASUREANDBEAT);
	static Guid PERFORMANCE = toGuid(GUID_NOTIFICATION_PERFORMANCE);
	static Guid RECOMPOSE = toGuid(GUID_NOTIFICATION_RECOMPOSE);
	static Guid SEGMENT = toGuid(GUID_NOTIFICATION_SEGMENT);
};

public enum class DMUS_NOTIFICATION : UInt32
{
	//for SEGMENT
	SEGABORT = DMUS_NOTIFICATION_SEGABORT,
	SEGALMOSTEND = DMUS_NOTIFICATION_SEGALMOSTEND,
	SEGEND = DMUS_NOTIFICATION_SEGEND,
	SEGLOOP = DMUS_NOTIFICATION_SEGLOOP,
	SEGSTART = DMUS_NOTIFICATION_SEGSTART,

	//for COMMAND
	GROOVE = DMUS_NOTIFICATION_GROOVE,
	EMBELLISHMENT = DMUS_NOTIFICATION_EMBELLISHMENT,

	//for PERFORMANCE
	MUSICALMOSTEND = DMUS_NOTIFICATION_MUSICALMOSTEND,
	MUSICSTARTED = DMUS_NOTIFICATION_MUSICSTARTED,
	MUSICSTOPPED = DMUS_NOTIFICATION_MUSICSTOPPED,

	//for MEASUREANDBEAT
	MEASUREBEAT = DMUS_NOTIFICATION_MEASUREBEAT,

	//for CHORD
	CHORD = DMUS_NOTIFICATION_CHORD,

	//for RECOMPOSE
	RECOMPOSE = DMUS_NOTIFICATION_RECOMPOSE,

};

[Flags]
public enum class DMUS_WAVEF : Byte
{
	IGNORELOOPS = DMUS_WAVEF_IGNORELOOPS,
	NOINVALIDATE = DMUS_WAVEF_NOINVALIDATE,
	OFF = DMUS_WAVEF_OFF,
	STREAMING = DMUS_WAVEF_STREAMING,
};


//
//
//		Tool graphs
//
//

public enum class DMUS_PMSGF_TOOL : UInt32
{
	IMMEDIATE = DMUS_PMSGF_TOOL_IMMEDIATE,
	QUEUE = DMUS_PMSGF_TOOL_QUEUE,
	ATTIME = DMUS_PMSGF_TOOL_ATTIME,
};

public ref class IDirectMusicGraph : public ComWrapperBase<::IDirectMusicGraph>
{
internal:
	IDirectMusicGraph(TBASE* ref, bool addRef) : ComWrapperBase(ref, addRef) {}

public:
	IDirectMusicGraph();

public:
	HR_DMUS GetTool(UInt32 dwIndex, [Out] IDirectMusicTool^% ppTool);
	HR_DMUS InsertTool(IDirectMusicTool^ pTool, array<UInt32>^ pdwChannels, UInt32 cPChannels, Int32 lIndex);
	HR_DMUS RemoveTool(IDirectMusicTool^ pTool);
	HR_DMUS StampPMsg(DMUS_PMSG^ pPMSG);
};

public ref class IDirectMusicTool : public ComWrapperBase<::IDirectMusicTool>
{
internal:
	IDirectMusicTool(TBASE* ref, bool addRef) : ComWrapperBase(ref, addRef) {}

public:
	HR_DMUS Flush(IDirectMusicPerformance^ pPerf, DMUS_PMSG^ pMSG, Int64 rtTime);
	HR_DMUS GetMediaTypeArraySize([Out] UInt32% pdwNumElements);
	HR_DMUS GetMediaTypes([Out] array<DMUS_PMSGT>^% psdwMediaTypes, UInt32 dwNumElements);
	HR_DMUS GetMsgDeliveryType(DMUS_PMSGF_TOOL% pdwDeliveryType);
	HR_DMUS Init(IDirectMusicGraph^ pGraph);
	HR_DMUS ProcessPMsg(IDirectMusicPerformance^ pPerf, DMUS_PMSG^ pMsg);
};

class CDirectMusicToolWrapper : public ComInterfaceImplWrapper<::IDirectMusicTool, CDirectMusicTool>
{
public:
	CDirectMusicToolWrapper(TCLASS^ iface) : ComInterfaceImplWrapper(IID_IDirectMusicTool, iface) {}

	STDMETHOD(Init)(::IDirectMusicGraph* pGraph);
    STDMETHOD(GetMsgDeliveryType)(DWORD* pdwDeliveryType);
    STDMETHOD(GetMediaTypeArraySize)(DWORD* pdwNumElements);
    STDMETHOD(GetMediaTypes)(DWORD** padwMediaTypes, DWORD dwNumElements);
	STDMETHOD(ProcessPMsg)(::IDirectMusicPerformance* pPerf, ::DMUS_PMSG* pPMSG);
	STDMETHOD(Flush)(::IDirectMusicPerformance* pPerf, ::DMUS_PMSG* pPMSG, REFERENCE_TIME rtTime);
};

public ref class CDirectMusicTool abstract
{
public:
	IDirectMusicTool^ QueryInterface();

	virtual HR_DMUS Init(IDirectMusicGraph^ pGraph) = 0;
	virtual HR_DMUS GetMsgDeliveryType([Out] DMUS_PMSGF_TOOL% pdwDeliveryType) = 0;
	virtual HR_DMUS GetMediaTypeArraySize([Out] UInt32% pdwNumElements) = 0;
	virtual HR_DMUS GetMediaTypes(array<DMUS_PMSGT>^ padwMediaTypes, UInt32 dwNumElements) = 0;
	virtual HR_DMUS ProcessPMsg(IDirectMusicPerformance^ pPerf, DMUS_PMSG^ pPMSG) = 0;
	virtual HR_DMUS Flush(IDirectMusicPerformance^ pPerf, DMUS_PMSG^ pPMSG, Int64 rtTime) = 0;
};



//
//
//		DirectMusic messages
//		when these are returned to the app, the data in the DMUS_PMSG is not duplicated;
//		therefore, these structures must not exist past the point the message is valid
//
//
public ref class DMUS_PMSG : public WrapperBaseNoDestructor<::DMUS_PMSG>
{
protected:
	DMUS_PMSG() { base = NULL; }

internal:
	static DMUS_PMSG^ FromPMSG(::DMUS_PMSG* msg);
	DMUS_PMSG(TBASE* val) { base = val; }

public:
    WRAPPER_PROPERTY(UInt32, dwSize);
    WRAPPER_PROPERTY(Int64, rtTime);
    WRAPPER_PROPERTY(Int32, mtTime);
    WRAPPER_PROPERTY(UInt32, dwFlags);
    WRAPPER_PROPERTY(UInt32, dwPChannel);
    WRAPPER_PROPERTY(UInt32, dwVirtualTrackID);
    WRAPPER_PROPERTY_COM_READONLY(IDirectMusicTool, pTool);
    WRAPPER_PROPERTY_COM_READONLY(IDirectMusicGraph, pGraph);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_PMSGT, DWORD, dwType);
	//NOTE: punkUser is not wrapped
	WRAPPER_PROPERTY(UInt32, dwVoiceID);
	WRAPPER_PROPERTY(UInt32, dwGroupID);
};

public ref class DMUS_CHANNEL_PRIORITY_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_CHANNEL_PRIORITY_PMSG, DMUS_PMSG, ::DMUS_CHANNEL_PRIORITY_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_CHANNEL_PRIORITY_PMSG);

public:
	WRAPPER_PROPERTY(UInt32, dwChannelPriority);
};

public ref class DMUS_CURVE_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_CURVE_PMSG, DMUS_PMSG, ::DMUS_CURVE_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_CURVE_PMSG);

public:
	WRAPPER_PROPERTY(Int32, mtDuration);
	WRAPPER_PROPERTY(Int32, mtOriginalStart);
	WRAPPER_PROPERTY(Int32, mtResetDuration);
	WRAPPER_PROPERTY(Int16, nStartValue);
	WRAPPER_PROPERTY(Int16, nEndValue);
	WRAPPER_PROPERTY(Int16, nResetValue);
	WRAPPER_PROPERTY(UInt16, wMeasure);
	WRAPPER_PROPERTY(Int16, nOffset);
	WRAPPER_PROPERTY(Byte, bBeat);
	WRAPPER_PROPERTY(Byte, bGrid);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_CURVET, BYTE, bType);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_CURVES, BYTE, bCurveShape);
	WRAPPER_PROPERTY(Byte, bCCData);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_CURVE, BYTE, bFlags);
	WRAPPER_PROPERTY(UInt16, wParamType);
	WRAPPER_PROPERTY(UInt16, wMergeIndex); 
};

public ref class DMUS_LYRIC_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_LYRIC_PMSG, DMUS_PMSG, ::DMUS_LYRIC_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_LYRIC_PMSG);

public:
	WRAPPER_PROPERTY_STRING_READONLY(wszString); 
};

public ref class DMUS_MIDI_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_MIDI_PMSG, DMUS_PMSG, ::DMUS_MIDI_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_MIDI_PMSG);

public:
	WRAPPER_PROPERTY(Byte, bStatus); 
	WRAPPER_PROPERTY(Byte, bByte1); 
	WRAPPER_PROPERTY(Byte, bByte2); 
	//NOTE: bPad is not wrapped; it exists only for padding
};

public ref class DMUS_NOTE_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_NOTE_PMSG, DMUS_PMSG, ::DMUS_NOTE_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_NOTE_PMSG);

public:
	WRAPPER_PROPERTY(Int32, mtDuration);
	WRAPPER_PROPERTY(UInt16, wMusicValue);
	WRAPPER_PROPERTY(UInt16, wMeasure);
	WRAPPER_PROPERTY(Int16, nOffset);
	WRAPPER_PROPERTY(Byte, bBeat);
	WRAPPER_PROPERTY(Byte, bGrid);
	WRAPPER_PROPERTY(Byte, bVelocity);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_NOTEF, BYTE, bFlags);
	WRAPPER_PROPERTY(Byte, bTimeRange);
	WRAPPER_PROPERTY(Byte, bDurRange);
	WRAPPER_PROPERTY(Byte, bVelRange);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_PLAYMODE, BYTE, bPlayModeFlags);
	WRAPPER_PROPERTY(Byte, bSubChordLevel);
	WRAPPER_PROPERTY(Byte, bMidiValue);
	WRAPPER_PROPERTY(SByte, cTranspose);
};

public ref class DMUS_NOTIFICATION_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_NOTIFICATION_PMSG, DMUS_PMSG, ::DMUS_NOTIFICATION_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_NOTIFICATION_PMSG);

public:
	WRAPPER_PROPERTY_GUID(guidNotificationType);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_NOTIFICATION, DWORD, dwNotificationOption);
	WRAPPER_PROPERTY(UInt32, dwField1);
	WRAPPER_PROPERTY(UInt32, dwField2);
};

public ref class DMUS_PATCH_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_PATCH_PMSG, DMUS_PMSG, ::DMUS_PATCH_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_PATCH_PMSG);

public:
	WRAPPER_PROPERTY(Byte, byInstrument);
	WRAPPER_PROPERTY(Byte, byMSB);
	WRAPPER_PROPERTY(Byte, byLSB);
	//NOTE: not including byPad; padding value is ignored
};

public ref class DMUS_SYSEX_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_SYSEX_PMSG, DMUS_PMSG, ::DMUS_SYSEX_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_SYSEX_PMSG);

public:
	WRAPPER_PROPERTY(UInt32, dwLen);
	property array<Byte>^ abData
	{
		array<Byte>^ get()
		{
			checkbase();
			array<Byte>^ ret = gcnew array<Byte>(base->dwLen);
			pin_ptr<Byte> data = &ret[0];
			memcpy(data, base->abData, base->dwLen);
			return ret;
		}
	};
};

public ref class DMUS_TEMPO_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_TEMPO_PMSG, DMUS_PMSG, ::DMUS_TEMPO_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_TEMPO_PMSG);

public:
	WRAPPER_PROPERTY(Double, dblTempo);
};

public ref class DMUS_TIMESIG_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_TIMESIG_PMSG, DMUS_PMSG, ::DMUS_TIMESIG_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_TIMESIG_PMSG);

public:
	WRAPPER_PROPERTY(Byte, bBeatsPerMeasure);
	WRAPPER_PROPERTY(Byte, bBeat);
	WRAPPER_PROPERTY(UInt16, wGridsPerBeat);
};

public ref class DMUS_TRANSPOSE_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_TRANSPOSE_PMSG, DMUS_PMSG, ::DMUS_TRANSPOSE_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_TRANSPOSE_PMSG);

public:
	WRAPPER_PROPERTY(Int16, nTranspose);
	WRAPPER_PROPERTY(UInt16, wMergeIndex);
};

public ref class DMUS_WAVE_PMSG : public DMUS_PMSG
{
	INHERITED_BASE_TYPE(DMUS_WAVE_PMSG, DMUS_PMSG, ::DMUS_WAVE_PMSG);
	INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(DMUS_WAVE_PMSG);

public:
	WRAPPER_PROPERTY(Int64, rtStartOffset);
	WRAPPER_PROPERTY(Int64, rtDuration);
	WRAPPER_PROPERTY(Int32, lOffset);
	WRAPPER_PROPERTY(Int32, lVolume);
	WRAPPER_PROPERTY(Int32, lPitch);
	WRAPPER_PROPERTY_DIFFCAST(DMUS_WAVEF, BYTE, bFlags);
};




}	//namespace DirectMidi
