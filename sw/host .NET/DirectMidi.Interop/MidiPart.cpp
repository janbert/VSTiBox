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
Module : MidiPart.cpp
Purpose: Code implementation of wrapper around classes defined in CMidiPart.h
Created: JH / 07-01-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

#include "MidiPart.h"
#include "AudioPart.h"

namespace DirectMidi {

//
//	CInstrument wrapper
//
Int32 CInstrument::SetPatch(UInt32 dwdstPatchMidi)
{
	DMIDI_METHOD_START;
	return base->SetPatch(dwdstPatchMidi);
	DMIDI_METHOD_END;
}

void CInstrument::SetNoteRange(UInt32 dwLowNote, UInt32 dwHighNote)
{
	DMIDI_METHOD_START;
	return base->SetNoteRange(dwLowNote, dwHighNote);
	DMIDI_METHOD_END;
}

void CInstrument::GetNoteRange(UInt32% pdwLowNote, UInt32% pdwHighNote)
{
	DMIDI_METHOD_START;
	DWORD lo, hi;
	base->GetNoteRange(&lo, &hi);
	pdwLowNote = lo;
	pdwHighNote = hi;
	DMIDI_METHOD_END;
}

//
//	CCollection wrapper
//
Int32 CCollection::EnumInstrument(UInt32 dwIndex, [Out] INSTRUMENTINFO^% InstInfo)
{
	DMIDI_METHOD_START;
	InstInfo = gcnew INSTRUMENTINFO;
	return base->EnumInstrument(dwIndex, &InstInfo->baseref());
	DMIDI_METHOD_END;
}

Int32 CCollection::GetInstrument([Out] CInstrument^% Instrument, INSTRUMENTINFO^ InstInfo)
{
	DMIDI_METHOD_START;
	Instrument = gcnew CInstrument;
	return base->GetInstrument(Instrument->baseref(), &InstInfo->baseref());
	DMIDI_METHOD_END;
}

Int32 CCollection::GetInstrument([Out] CInstrument^% Instrument, Int32 nIndex)
{
	DMIDI_METHOD_START;
	Instrument = gcnew CInstrument;
	return base->GetInstrument(Instrument->baseref(), nIndex);
	DMIDI_METHOD_END;
}

//
//	CDLSLoader wrapper
//
Int32 CDLSLoader::Initialize()
{ 
	DMIDI_METHOD_START;
	return base->Initialize();
	DMIDI_METHOD_END;
}

Int32 CDLSLoader::LoadDLS(String^ lpFileName, [Out] CCollection^% Collection)
{
	DMIDI_METHOD_START;
	Collection = gcnew CCollection;

	//NULL is acceptable as the first argument
	LPTSTR strUse = NULL;
	widestring str;
	if (lpFileName)
	{
		str = fromString(lpFileName);
		strUse = const_cast<LPTSTR>(str.c_str());
	}
	return base->LoadDLS(strUse, Collection);
	DMIDI_METHOD_END;
}

Int32 CDLSLoader::LoadDLSFromResource(String^ strResource, String^ strResourceType, [Out] CCollection^% Collection)
{ 
	DMIDI_METHOD_START;
	Collection = gcnew CCollection;
	return base->LoadDLSFromResource(const_cast<LPTSTR>(fromString(strResource).c_str()), const_cast<LPTSTR>(fromString(strResourceType).c_str()), Collection);
	DMIDI_METHOD_END;
}

Int32 CDLSLoader::LoadWaveFile(String^ lpstrFileName, [Out] CSampleInstrument^% SampleInstrument, DM_LOCATION bReadAlways)
{
	DMIDI_STATIC_START;
	SampleInstrument = gcnew CSampleInstrument;
	return TBASE::LoadWaveFile(const_cast<LPTSTR>(fromString(lpstrFileName).c_str()), SampleInstrument, static_cast<BOOL>(bReadAlways));
	DMIDI_STATIC_END;
}

Int32 CDLSLoader::SetSearchDirectory(String^ pszPath, Boolean fClear)
{
	DMIDI_METHOD_START;
	return base->SetSearchDirectory(const_cast<LPTSTR>(fromString(pszPath).c_str()), fClear);
	DMIDI_METHOD_END;
}

Int32 CDLSLoader::UnloadCollection(CCollection^ Collection)
{
	DMIDI_METHOD_START;
	return base->UnloadCollection(Collection);
	DMIDI_METHOD_END;
}

Int32 CDLSLoader::LoadSegment(String^ lpstrFileName, CSegment^% Segment, Boolean bIsMIDIFile)
{ 
	DMIDI_METHOD_START;
	Segment = gcnew CSegment;
	return base->LoadSegment(const_cast<LPTSTR>(fromString(lpstrFileName).c_str()), Segment, bIsMIDIFile);
	DMIDI_METHOD_END;
}

Int32 CDLSLoader::LoadSegmentFromResource(String^ strResource, String^ strResourceType, CSegment^% Segment, Boolean bIsMidiFile)
{
	DMIDI_METHOD_START;
	Segment = gcnew CSegment;
	return base->LoadSegmentFromResource(const_cast<TCHAR*>(fromString(strResource).c_str()), const_cast<TCHAR*>(fromString(strResourceType).c_str()), Segment->baseref(), bIsMidiFile); 
	DMIDI_METHOD_END;
}

//
//	CReceiver interface wrapper
//
void CReceiverWrapper::RecvMidiMsg(REFERENCE_TIME rt, DWORD dwChannel, DWORD dwBytesRead, BYTE *lpBuffer)
{
	array<Byte>^ msg = gcnew array<Byte>(dwBytesRead);
	if (dwBytesRead > 0)
	{
		pin_ptr<Byte> msgPtr = &msg[0];
		memcpy(msgPtr, lpBuffer, dwBytesRead);
	}
	m_iface->RecvMidiMsg(rt, dwChannel, dwBytesRead, msg);
}

void CReceiverWrapper::RecvMidiMsg(REFERENCE_TIME rt, DWORD dwChannel, DWORD dwMsg)
{
	m_iface->RecvMidiMsg(rt, dwChannel, dwMsg);
}

//
//	CMidiPort
//
template<typename TMidiPort>
Int32 CMidiPortBase<TMidiPort>::Initialize(CDirectMusic^ DMusic)
{
	DMIDI_METHOD_START;
	return base->Initialize(DMusic->baseref());
	DMIDI_METHOD_END;
}

template<typename TMidiPort>
UInt32 CMidiPortBase<TMidiPort>::GetNumPorts()
{
	DMIDI_METHOD_START;
	return base->GetNumPorts();
	DMIDI_METHOD_END;
}

template<typename TMidiPort>
Int32 CMidiPortBase<TMidiPort>::GetPortInfo(UInt32 dwNumPort, INFOPORT^% lpInfoPort)
{
	DMIDI_METHOD_START;
	lpInfoPort = gcnew INFOPORT();
	return base->GetPortInfo(dwNumPort, &lpInfoPort->baseref());
	DMIDI_METHOD_END;
}

template<typename TMidiPort>
Int32 CMidiPortBase<TMidiPort>::GetActivatedPortInfo(INFOPORT^% lpInfoPort)
{
	DMIDI_METHOD_START;
	lpInfoPort = gcnew INFOPORT();
	return base->GetActivatedPortInfo(&lpInfoPort->baseref());
	DMIDI_METHOD_END;
}

template<typename TMidiPort>
Int32 CMidiPortBase<TMidiPort>::KsProperty(Guid Set, UInt32 Id, KSPROPERTY_TYPE Flags, array<Byte>^ pvPropertyData, UInt32 ulDataLength, UInt32% pulBytesReturned)
{
	DMIDI_METHOD_START;
	DWORD lenRet;
	Int32 ret;

	//TODO: change the signature of this function; based on 'Set' (GUID_DMUS_PROP) and Flags (KSPROPERTY_TYPE_BASICSUPPORT implies DWORD-sized)
	GUID set = DirectMidi::fromGuid(Set);
	if (pvPropertyData)
	{
		pin_ptr<Byte> prop = &pvPropertyData[0];
		ret = base->KsProperty(set, Id, static_cast<ULONG>(Flags), prop, ulDataLength, &lenRet);
	}
	else
	{
		ret = base->KsProperty(set, Id, static_cast<ULONG>(Flags), NULL, ulDataLength, &lenRet);
	}

	pulBytesReturned = lenRet;
	return ret;
	DMIDI_METHOD_END;
}

template<typename TMidiPort>
Int32 CMidiPortBase<TMidiPort>::SetBuffer(UInt32 dwBufferSize)
{
	DMIDI_METHOD_START;
	return base->SetBuffer(dwBufferSize);
	DMIDI_METHOD_END;
}

template<typename TMidiPort>
Int32 CMidiPortBase<TMidiPort>::RemoveBuffer()
{
	DMIDI_METHOD_START;
	return base->RemoveBuffer();
	DMIDI_METHOD_END;
}


//
//	CInputPort
//
Int32 CInputPort::ActivatePort(INFOPORT^ InfoPort, UInt32 dwSysExSize)
{
	DMIDI_METHOD_START;
	return base->ActivatePort(&InfoPort->baseref(), dwSysExSize);
	DMIDI_METHOD_END;
}

Int32 CInputPort::ActivatePortFromInterface(IDirectMusicPort^ pPort, UInt32 dwSysExSize)
{
	DMIDI_METHOD_START;
	return base->ActivatePortFromInterface(&pPort->baseref(), dwSysExSize);
	DMIDI_METHOD_END;
}

Int32 CInputPort::ActivateNotification()
{
	DMIDI_METHOD_START;
	return base->ActivateNotification();
	DMIDI_METHOD_END;
}

Boolean CInputPort::SetThreadPriority(Int32 nPriority)
{
	DMIDI_METHOD_START;
	return (FALSE != base->SetThreadPriority(nPriority));
	DMIDI_METHOD_END;
}

Int32 CInputPort::SetReceiver(CReceiver^ Receiver)
{
	DMIDI_METHOD_START;
	return base->SetReceiver(Receiver->baseref());
	DMIDI_METHOD_END;
}

Int32 CInputPort::ReleasePort()
{
	DMIDI_METHOD_START;
	return base->ReleasePort();
	DMIDI_METHOD_END;
}

Int32 CInputPort::SetThru(UInt32 dwSourceChannel, UInt32 dwDestinationChannelGroup, UInt32 dwDestinationChannel, COutputPort^ dstMidiPort)
{
	DMIDI_METHOD_START;
	return base->SetThru(dwSourceChannel, dwDestinationChannelGroup, dwDestinationChannel, dstMidiPort);
	DMIDI_METHOD_END;
}

Int32 CInputPort::BreakThru(UInt32 dwSourceChannel, UInt32 dwDestinationChannelGroup, UInt32 dwDestinationChannel)
{
	DMIDI_METHOD_START;
	return base->BreakThru(dwSourceChannel, dwDestinationChannelGroup, dwDestinationChannel);
	DMIDI_METHOD_END;
}

Int32 CInputPort::TerminateNotification()
{
	DMIDI_METHOD_START;
	return base->TerminateNotification();
	DMIDI_METHOD_END;
}

UInt32 CInputPort::WaitForEvent(void* lpv)
{
	DMIDI_STATIC_START;
	return TBASE::WaitForEvent(lpv);
	DMIDI_STATIC_END;
}

void CInputPort::DecodeMidiMsg(UInt32 dwMsg, MidiMessage% Status, Byte% DataByte1, Byte% DataByte2)
{
	DMIDI_STATIC_START;
	BYTE st, d1, d2;
	TBASE::DecodeMidiMsg(dwMsg, &st, &d1, &d2);
	Status = static_cast<MidiMessage>(st);
	DataByte1 = d1;
	DataByte2 = d2;
	DMIDI_STATIC_END;
}

void CInputPort::DecodeMidiMsg(UInt32 Msg, MidiMessage% Command, Byte% Channel, Byte% DataByte1, Byte% DataByte2)
{
	DMIDI_STATIC_START;
	BYTE cmd, ch, b1, b2;
	TBASE::DecodeMidiMsg(Msg, &cmd, &ch, &b1, &b2);
	Command = static_cast<MidiMessage>(cmd);
	Channel = ch;
	DataByte1 = b1;
	DataByte2 = b2;
	DMIDI_STATIC_END;
}

//
//	COutputPort
//
Int32 COutputPort::ActivatePort(INFOPORT^ InfoPort, UInt32 dwSysExSize)
{
	DMIDI_METHOD_START;
	return base->ActivatePort(&InfoPort->baseref(), dwSysExSize);
	DMIDI_METHOD_END;
}

Int32 COutputPort::ActivatePortFromInterface(IDirectMusicPort^ pPort, UInt32 dwSysExSize)
{
	DMIDI_METHOD_START;
	return base->ActivatePortFromInterface(&pPort->baseref(), dwSysExSize);
	DMIDI_METHOD_END;
}

void COutputPort::SetPortParams(UInt32 dwVoices, UInt32 dwAudioChannels, UInt32 dwChannelGroups, SET dwEffectFlags, UInt32 dwSampleRate)
{
	DMIDI_METHOD_START;
	base->SetPortParams(dwVoices, dwAudioChannels, dwChannelGroups, static_cast<DWORD>(dwEffectFlags), dwSampleRate);
	DMIDI_METHOD_END;
}

Int32 COutputPort::ReleasePort()
{
	DMIDI_METHOD_START;
	return base->ReleasePort();
	DMIDI_METHOD_END;
}

Int32 COutputPort::AllocateMemory(CSampleInstrument^ SampleInstrument)
{
	DMIDI_METHOD_START;
	return base->AllocateMemory(SampleInstrument);
	DMIDI_METHOD_END;
}

Int32 COutputPort::DownloadInstrument(CInstrument^ Instrument)
{
	DMIDI_METHOD_START;
	return base->DownloadInstrument(Instrument);
	DMIDI_METHOD_END;
}

Int32 COutputPort::DownloadInstrument(CSampleInstrument^ SampleInstrument)
{
	DMIDI_METHOD_START;
	return base->DownloadInstrument(SampleInstrument);
	DMIDI_METHOD_END;
}

Int32 COutputPort::UnloadInstrument(CInstrument^ Instrument)
{
	DMIDI_METHOD_START;
	return base->UnloadInstrument(Instrument);
	DMIDI_METHOD_END;
}

Int32 COutputPort::UnloadInstrument(CSampleInstrument^ SampleInstrument)
{
	DMIDI_METHOD_START;
	return base->UnloadInstrument(SampleInstrument);
	DMIDI_METHOD_END;
}

Int32 COutputPort::DeallocateMemory(CSampleInstrument^ SampleInstrument)
{
	DMIDI_METHOD_START;
	return base->DeallocateMemory(SampleInstrument);
	DMIDI_METHOD_END;
}

Int32 COutputPort::CompactMemory()
{
	DMIDI_METHOD_START;
	return base->CompactMemory();
	DMIDI_METHOD_END;
}

Int32 COutputPort::GetChannelPriority(UInt32 dwChannelGroup, UInt32 dwChannel, UInt32% pdwPriority)
{
	DMIDI_METHOD_START;
	DWORD pri;
	Int32 ret = base->GetChannelPriority(dwChannelGroup, dwChannel, &pri);
	pdwPriority = pri;
	return ret;
	DMIDI_METHOD_END;
}

Int32 COutputPort::SetChannelPriority(UInt32 dwChannelGroup, UInt32 dwChannel, UInt32 dwPriority)
{
	DMIDI_METHOD_START;
	return base->SetChannelPriority(dwChannelGroup, dwChannel, dwPriority);
	DMIDI_METHOD_END;
}

Int32 COutputPort::GetSynthStats(SYNTHSTATS% SynthStats)
{
	DMIDI_METHOD_START;
	pin_ptr<SYNTHSTATS> stats = &SynthStats;
	return base->GetSynthStats(reinterpret_cast<directmidi::SYNTHSTATS*>(stats));
	DMIDI_METHOD_END;
}

Int32 COutputPort::GetFormat(WAVEFORMATEX% pWaveFormatEx, UInt32% pdwWaveFormatExSize, UInt32% pdwBufferSize)
{
	DMIDI_METHOD_START;
	DWORD wavsz, bufsz;
	pin_ptr<WAVEFORMATEX> wfex = &pWaveFormatEx;
	Int32 ret = base->GetFormat(reinterpret_cast<::WAVEFORMATEX*>(wfex), &wavsz, &bufsz);
	pdwWaveFormatExSize = wavsz;
	pdwBufferSize = bufsz;
	return ret;
	DMIDI_METHOD_END;
}

Int32 COutputPort::GetNumChannelGroups(UInt32% pdwChannelGroups)
{
	DMIDI_METHOD_START;
	DWORD grp;
	Int32 ret = base->GetNumChannelGroups(&grp);
	pdwChannelGroups = grp;
	return ret;
	DMIDI_METHOD_END;
}

Int32 COutputPort::SetNumChannelGroups(UInt32 dwChannelGroups)
{
	DMIDI_METHOD_START;
	return base->SetNumChannelGroups(dwChannelGroups);
	DMIDI_METHOD_END;
}

Int32 COutputPort::SetEffect(Boolean bEffect)
{
	DMIDI_METHOD_START;
	return base->SetEffect(bEffect);
	DMIDI_METHOD_END;
}

Int32 COutputPort::SendMidiMsg(UInt32 dwMsg, UInt32 dwChannelGroup)
{
	DMIDI_METHOD_START;
	return base->SendMidiMsg(dwMsg, dwChannelGroup);
	DMIDI_METHOD_END;
}

Int32 COutputPort::SendMidiMsg(array<Byte>^ lpMsg, UInt32 dwLength, UInt32 dwChannelGroup)
{
	DMIDI_METHOD_START;
	pin_ptr<Byte> pinMsg = &lpMsg[0];
	return base->SendMidiMsg(pinMsg, dwLength, dwChannelGroup);
	DMIDI_METHOD_END;
}

IReferenceClock^ COutputPort::GetReferenceClock()
{
	DMIDI_METHOD_START;
	return gcnew IReferenceClock(base->GetReferenceClock(), true);
	DMIDI_METHOD_END;
}

UInt32 COutputPort::EncodeMidiMsg(MidiMessage Status, Byte DataByte1, Byte DataByte2)
{
	DMIDI_STATIC_START;
	return TBASE::EncodeMidiMsg(static_cast<BYTE>(Status), DataByte1, DataByte2);
	DMIDI_STATIC_END;
}

UInt32 COutputPort::EncodeMidiMsg(MidiMessage Command, Byte Channel, Byte DataByte1, Byte DataByte2)
{
	DMIDI_STATIC_START;
	return TBASE::EncodeMidiMsg(static_cast<BYTE>(Command), Channel, DataByte1, DataByte2);
	DMIDI_STATIC_END;
}


//
//	CDirectMusic
//
Int32 CDirectMusic::Initialize(IntPtr hWnd, /* IDirectSound8* */ Object^ pDirectSound)
{
	DMIDI_METHOD_START;
	if (pDirectSound)
	{
		throw gcnew Exception("ERROR: IDirectSound is not wrapped; can't pass non-null here");
	}
	return base->Initialize(static_cast<HWND>(hWnd.ToPointer()), NULL);
	DMIDI_METHOD_END;
}

//
//	CSampleInstrument
//
void CSampleInstrument::SetPatch(UInt32 dwPatch)
{
	DMIDI_METHOD_START;
	base->SetPatch(dwPatch);
	DMIDI_METHOD_END;
}

void CSampleInstrument::SetWaveForm(array<Byte>^ pRawData, WAVEFORMATEX% pwfex, UInt32 dwSize)
{
	DMIDI_METHOD_START;
	pin_ptr<Byte> pinData = &pRawData[0];
	pin_ptr<WAVEFORMATEX> wfex = &pwfex;
	base->SetWaveForm(pinData, reinterpret_cast<::WAVEFORMATEX*>(wfex), dwSize);
	DMIDI_METHOD_END;
}

void CSampleInstrument::GetWaveForm(array<Byte>^% pRawData, WAVEFORMATEX% pwfex, UInt32% dwSize)
{
	DMIDI_METHOD_START;
	BYTE* data;
	DWORD sz;
	pin_ptr<WAVEFORMATEX> wfex = &pwfex;
	base->GetWaveForm(&data, reinterpret_cast<::WAVEFORMATEX*>(wfex), &sz);

	dwSize = sz;
	pRawData = gcnew array<Byte>(dwSize);
	if (dwSize > 0)
	{
		pin_ptr<Byte> pinData = &pRawData[0];
		memcpy(pinData, data, dwSize);
	}
	DMIDI_METHOD_END;
}

void CSampleInstrument::SetLoop(Boolean bLoop)
{
	DMIDI_METHOD_START;
	base->SetLoop(bLoop);
	DMIDI_METHOD_END;
}

void CSampleInstrument::SetWaveParams(Int32 lAttenuation, Int16 sFineTune, UInt16 usUnityNote, F_WSMP fulOptions)
{
	DMIDI_METHOD_START;
	base->SetWaveParams(lAttenuation, sFineTune, usUnityNote, static_cast<DWORD>(fulOptions));
	DMIDI_METHOD_END;
}

void CSampleInstrument::GetWaveParams(Int32% plAttenuation, Int16% psFineTune, UInt16% pusUnityNote, F_WSMP% pfulOptions)
{
	DMIDI_METHOD_START;
	LONG att;
	SHORT fine;
	USHORT unity;
	ULONG ops;
	base->GetWaveParams(&att, &fine, &unity, &ops);
	plAttenuation = att;
	psFineTune = fine;
	pusUnityNote = unity;
	pfulOptions = static_cast<F_WSMP>(ops);
	DMIDI_METHOD_END;
}

UInt32 CSampleInstrument::GetWaveFormSize()
{
	DMIDI_METHOD_START;
	return base->GetWaveFormSize();
	DMIDI_METHOD_END;
}

void CSampleInstrument::SetRegion(REGION% pRegion)
{
	DMIDI_METHOD_START;
	pin_ptr<REGION> rgn = &pRegion;
	base->SetRegion(reinterpret_cast<::REGION*>(rgn));
	DMIDI_METHOD_END;
}

void CSampleInstrument::SetArticulationParams(ARTICPARAMS% pArticParams)
{
	DMIDI_METHOD_START;
	pin_ptr<ARTICPARAMS> ap = &pArticParams;
	base->SetArticulationParams(reinterpret_cast<::ARTICPARAMS*>(ap));
	DMIDI_METHOD_END;
}

void CSampleInstrument::GetRegion(REGION% pRegion)
{
	DMIDI_METHOD_START;
	pin_ptr<REGION> rgn = &pRegion;
	base->GetRegion(reinterpret_cast<::REGION*>(rgn));
	DMIDI_METHOD_END;
}

void CSampleInstrument::GetArticulationParams(ARTICPARAMS% pArticParams)
{
	DMIDI_METHOD_START;
	pin_ptr<ARTICPARAMS> ap = &pArticParams;
	base->GetArticulationParams(reinterpret_cast<::ARTICPARAMS*>(ap));
	DMIDI_METHOD_END;
}

Int32 CSampleInstrument::ReleaseSample()
{
	DMIDI_METHOD_START;
	return base->ReleaseSample();
	DMIDI_METHOD_END;
}

//
//	CMasterClock
//
Int32 CMasterClock::Initialize(CDirectMusic^ DMusic)
{
	DMIDI_METHOD_START;
	return base->Initialize(DMusic->baseref());
	DMIDI_METHOD_END;
}

Int32 CMasterClock::ReleaseMasterClock()
{
	DMIDI_METHOD_START;
	return base->ReleaseMasterClock();
	DMIDI_METHOD_END;
}

UInt32 CMasterClock::GetNumClocks()
{
	DMIDI_METHOD_START;
	return base->GetNumClocks();
	DMIDI_METHOD_END;
}

Int32 CMasterClock::GetClockInfo(UInt32 dwNumClock, CLOCKINFO^ ClockInfo)
{
	DMIDI_METHOD_START;
	return base->GetClockInfo(dwNumClock, &ClockInfo->baseref());
	DMIDI_METHOD_END;
}

Int32 CMasterClock::ActivateMasterClock(CLOCKINFO^ ClockInfo)
{
	DMIDI_METHOD_START;
	return base->ActivateMasterClock(&ClockInfo->baseref());
	DMIDI_METHOD_END;
}

IReferenceClock^ CMasterClock::GetReferenceClock()
{
	DMIDI_METHOD_START;
	return gcnew IReferenceClock(base->GetReferenceClock(), true);
	DMIDI_METHOD_END;
}


}	//namespace directMidi
