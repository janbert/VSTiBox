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
Module : MidiPart.h
Purpose: Definition of wrappers around classes defined in CMidiPart.h
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
#include "DirectBase.h"

namespace DirectMidi {

//
//	CInstrument wrapper
//
public ref class CInstrument : public DMidiWrapperBase<directmidi::CInstrument>
{
public:
	Int32 SetPatch(UInt32 dwdstPatchMidi);
	void SetNoteRange(UInt32 dwLowNote, UInt32 dwHighNote);
	void GetNoteRange([Out] UInt32% pdwLowNote, [Out] UInt32% pdwHighNote);

	WRAPPER_PROPERTY(UInt32, m_dwPatchInCollection);
	WRAPPER_PROPERTY(UInt32, m_dwPatchInMidi);
	WRAPPER_PROPERTY_STRING(m_strInstName);
};

//
//	CCollection wrapper
//
public ref class CCollection : public DMidiWrapperBase<directmidi::CCollection>
{
public:
	Int32 EnumInstrument(UInt32 dwIndex, [Out] INSTRUMENTINFO^% InstInfo);
	Int32 GetInstrument([Out] CInstrument^% Instrument, INSTRUMENTINFO^ InstInfo);
	Int32 GetInstrument([Out] CInstrument^% Instrument, Int32 nIndex);
};

//
//	CDLSLoader wrapper
//
public ref class CDLSLoader : public DMidiWrapperBase<directmidi::CDLSLoader>
{
public:
	Int32 Initialize();
	Int32 LoadDLS(String^ lpFileName, [Out] CCollection^% Collection);
	Int32 LoadDLSFromResource(String^ strResource, String^ strResourceType, [Out] CCollection^% Collection);
	static Int32 LoadWaveFile(String^ lpstrFileName, [Out] CSampleInstrument^% SampleInstrument, DM_LOCATION bReadAlways);
	Int32 SetSearchDirectory(String^ pszPath, Boolean fClear);
	Int32 UnloadCollection(CCollection^ Collection);
	Int32 LoadSegment(String^ lpstrFileName, [Out] CSegment^% Segment, Boolean bIsMIDIFile);
	Int32 LoadSegment(String^ lpstrFileName, [Out] CSegment^% Segment) { return LoadSegment(lpstrFileName, Segment, false); }
	Int32 LoadSegmentFromResource(String^ strResource, String^ strResourceType, [Out] CSegment^% Segment, Boolean bIsMidiFile);
	Int32 LoadSegmentFromResource(String^ strResource, String^ strResourceType, [Out] CSegment^% Segment) { return LoadSegmentFromResource(strResource, strResourceType, Segment, false); }
};

//
//	CReceiver wrapper
//
class CReceiverWrapper : public InterfaceImplWrapper<directmidi::CReceiver, DirectMidi::CReceiver>
{
public:
	CReceiverWrapper(TCLASS^ iface) : InterfaceImplWrapper(iface) {}
	virtual void RecvMidiMsg(REFERENCE_TIME rt, DWORD dwChannel, DWORD dwBytesRead, BYTE *lpBuffer);
	virtual void RecvMidiMsg(REFERENCE_TIME rt, DWORD dwChannel, DWORD dwMsg);
};

public ref class CReceiver abstract : public WrapperBaseNoConstructor<CReceiverWrapper>
{
public:
	CReceiver() { base = new CReceiverWrapper(this); }
	virtual void RecvMidiMsg(Int64 rt, UInt32 dwChannel, UInt32 dwBytesRead, array<Byte>^ lpBuffer) = 0;
	virtual void RecvMidiMsg(Int64 rt, UInt32 dwChannel, UInt32 dwMsg) = 0;
};

//
//	CMidiPort wrapper
//
public interface class CMidiPort
{
public:
	virtual Int32 Initialize(CDirectMusic^ DMusic) = 0;
	virtual UInt32 GetNumPorts() = 0;
	virtual Int32 GetPortInfo(UInt32 dwNumPort, [Out] INFOPORT^% lpInfoPort) = 0;
	virtual Int32 GetActivatedPortInfo([Out] INFOPORT^% lpInfoPort) = 0;
	virtual Int32 KsProperty(Guid Set, UInt32 Id, KSPROPERTY_TYPE Flags, array<Byte>^ pvPropertyData, UInt32 ulDataLength, [Out] UInt32% pulBytesReturned);
	virtual Int32 SetBuffer(UInt32 dwBufferSize) = 0;
	virtual Int32 RemoveBuffer() = 0;
};

template<typename TMidiPort>
public ref class CMidiPortBase : public DMidiWrapperBase<TMidiPort>, public CMidiPort
{
public:
	virtual Int32 Initialize(CDirectMusic^ DMusic);
	virtual UInt32 GetNumPorts();
	virtual Int32 GetPortInfo(UInt32 dwNumPort, [Out] INFOPORT^% lpInfoPort);
	virtual Int32 GetActivatedPortInfo([Out] INFOPORT^% lpInfoPort);
	virtual Int32 KsProperty(Guid Set, UInt32 Id, KSPROPERTY_TYPE Flags, array<Byte>^ pvPropertyData, UInt32 ulDataLength, [Out] UInt32% pulBytesReturned);
	virtual Int32 SetBuffer(UInt32 dwBufferSize);
	virtual Int32 RemoveBuffer();
};

//
//	CInputPort wrapper
//
public ref class CInputPort : public CMidiPortBase<directmidi::CInputPort>, public CMidiPort
{
public:
	Int32 ActivatePort(INFOPORT^ InfoPort, UInt32 dwSysExSize);
	Int32 ActivatePort(INFOPORT^ InfoPort) { return ActivatePort(InfoPort, 32); }
	Int32 ActivatePortFromInterface(IDirectMusicPort^ pPort, UInt32 dwSysExSize);
	Int32 ActivatePortFromInterface(IDirectMusicPort^ pPort) { return ActivatePortFromInterface(pPort, 32); }
	Int32 ActivateNotification();
	Boolean SetThreadPriority(Int32 nPriority);
	Int32 SetReceiver(CReceiver^ Receiver);
	Int32 ReleasePort();
	Int32 SetThru(UInt32 dwSourceChannel, UInt32 dwDestinationChannelGroup, UInt32 dwDestinationChannel, COutputPort^ dstMidiPort);
	Int32 BreakThru(UInt32 dwSourceChannel, UInt32 dwDestinationChannelGroup, UInt32 dwDestinationChannel);
	Int32 TerminateNotification();
	static UInt32 WaitForEvent(void* lpv);
	static void DecodeMidiMsg(UInt32 dwMsg, [Out] MidiMessage% Status, [Out] Byte% DataByte1, [Out] Byte% DataByte2);
	static void DecodeMidiMsg(UInt32 Msg, [Out] MidiMessage% Command, [Out] Byte% Channel, [Out] Byte% DataByte1, [Out] Byte% DataByte2);
};

//
//	COutputPort wrapper
//
public ref class COutputPort : public CMidiPortBase<directmidi::COutputPort>, public CMidiPort
{
public:
	Int32 ActivatePort(INFOPORT^ InfoPort, UInt32 dwSysExSize);
	Int32 ActivatePort(INFOPORT^ InfoPort) { return ActivatePort(InfoPort, 32); }
	Int32 ActivatePortFromInterface(IDirectMusicPort^ pPort, UInt32 dwSysExSize);
	Int32 ActivatePortFromInterface(IDirectMusicPort^ pPort) { return ActivatePortFromInterface(pPort, 32); }
	void SetPortParams(UInt32 dwVoices, UInt32 dwAudioChannels, UInt32 dwChannelGroups, SET dwEffectFlags, UInt32 dwSampleRate);
	Int32 ReleasePort();
	Int32 AllocateMemory(CSampleInstrument^ SampleInstrument);
	Int32 DownloadInstrument(CInstrument^ Instrument);
	Int32 DownloadInstrument(CSampleInstrument^ SampleInstrument);
	Int32 UnloadInstrument(CInstrument^ Instrument);
	Int32 UnloadInstrument(CSampleInstrument^ SampleInstrument);
	Int32 DeallocateMemory(CSampleInstrument^ SampleInstrument);
	Int32 CompactMemory();
	Int32 GetChannelPriority(UInt32 dwChannelGroup, UInt32 dwChannel, [Out] UInt32% pdwPriority);
	Int32 SetChannelPriority(UInt32 dwChannelGroup, UInt32 dwChannel, UInt32 dwPriority);
	Int32 GetSynthStats([Out] SYNTHSTATS% SynthStats);
	Int32 GetFormat([Out] WAVEFORMATEX% pWaveFormatEx, [Out] UInt32% pdwWaveFormatExSize, [Out] UInt32% pdwBufferSize);
	Int32 GetNumChannelGroups([Out] UInt32% pdwChannelGroups);
	Int32 SetNumChannelGroups(UInt32 dwChannelGroups);
	Int32 SetEffect(Boolean bEffect);
	Int32 SendMidiMsg(UInt32 dwMsg, UInt32 dwChannelGroup);
	Int32 SendMidiMsg(array<Byte>^ lpMsg, UInt32 dwLength, UInt32 dwChannelGroup);
	IReferenceClock^ GetReferenceClock();
	static UInt32 EncodeMidiMsg(MidiMessage Status, Byte DataByte1, Byte DataByte2);
	static UInt32 EncodeMidiMsg(MidiMessage Command, Byte Channel, Byte DataByte1, Byte DataByte2);
};

//
//	CDirectMusic wrapper
//
public ref class CDirectMusic : public DMidiWrapperBase<directmidi::CDirectMusic>
{
public:
	Int32 Initialize(IntPtr hWnd, /* IDirectSound8* */ Object^ pDirectSound);
	Int32 Initialize(Control^ hWnd, /* IDirectSound8* */ Object^ pDirectSound) { return Initialize(hWnd ? hWnd->Handle : IntPtr::Zero, pDirectSound); }
	Int32 Initialize(IntPtr hWnd) { return Initialize(hWnd, nullptr); }
	Int32 Initialize(Control^ hWnd) { return Initialize(hWnd ? hWnd->Handle : IntPtr::Zero); }
	Int32 Initialize() { return Initialize(IntPtr::Zero); }
};

//
//	CSampleInstrument wrapper
//
public ref class CSampleInstrument : public DMidiWrapperBase<directmidi::CSampleInstrument>
{
public:
	void SetPatch(UInt32 dwPatch);
	void SetWaveForm(array<Byte>^ pRawData, [In] WAVEFORMATEX% pwfex, UInt32 dwSize);
	void GetWaveForm([Out] array<Byte>^% pRawData, [Out] WAVEFORMATEX% pwfex, [Out] UInt32% dwSize);
	void SetLoop(Boolean bLoop);
	void SetWaveParams(Int32 lAttenuation, Int16 sFineTune, UInt16 usUnityNote, F_WSMP fulOptions);
	void GetWaveParams([Out] Int32% plAttenuation, [Out] Int16% psFineTune, [Out] UInt16% pusUnityNote, [Out] F_WSMP% pfulOptions);
	UInt32 GetWaveFormSize();
	void SetRegion([In] REGION% pRegion);
	void SetArticulationParams([In] ARTICPARAMS% pArticParams);
	void GetRegion([Out] REGION% pRegion);
	void GetArticulationParams([Out] ARTICPARAMS% pArticParams);
	Int32 ReleaseSample();
};

//
//	CMasterClock wrapper
//
public ref class CMasterClock : public DMidiWrapperBase<directmidi::CMasterClock>
{
public:
	Int32 Initialize(CDirectMusic^ DMusic);
	Int32 ReleaseMasterClock();
	UInt32 GetNumClocks();
	Int32 GetClockInfo(UInt32 dwNumClock, CLOCKINFO^ ClockInfo);
	Int32 ActivateMasterClock(CLOCKINFO^ ClockInfo);
	IReferenceClock^ GetReferenceClock();
};



}	//namespace directMidi
