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
Module : AudioPart.cpp
Purpose: Code implementation of wrapper around classes defined in CAudioPart.h
Created: JH / 07-01-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

#include "AudioPart.h"
#include "MidiPart.h"

namespace DirectMidi {

//
//	CPerformance
//
Int32 CPerformance::Stop(ISegment^ Segment, Int64 i64StopTime, UInt32 dwFlags)
{
	DMIDI_METHOD_START;
	return base->Stop(Segment->baseref(), i64StopTime, dwFlags);
	DMIDI_METHOD_END;
}

Int32 CPerformance::StopAll()
{
	DMIDI_METHOD_START;
	return base->StopAll();
	DMIDI_METHOD_END;
}

Int32 CPerformance::SetMasterVolume(Int32 nVolume)
{
	DMIDI_METHOD_START;
	return base->SetMasterVolume(nVolume);
	DMIDI_METHOD_END;
}

Int32 CPerformance::SetMasterTempo(Single fTempo)
{
	DMIDI_METHOD_START;
	return base->SetMasterTempo(fTempo);
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetMasterVolume(Int32% nVolume)
{
	DMIDI_METHOD_START;
	long v;
	Int32 ret = base->GetMasterVolume(&v);
	nVolume = v;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetMasterTempo(Single% nTempo)
{
	DMIDI_METHOD_START;
	float t;
	Int32 ret = base->GetMasterTempo(&t);
	nTempo = t;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::AdjustTime(Int64 rtAmount)
{
	DMIDI_METHOD_START;
	return base->AdjustTime(rtAmount);
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetLatencyTime(Int64% prtTime)
{
	DMIDI_METHOD_START;
	REFERENCE_TIME t;
	Int32 ret = base->GetLatencyTime(&t);
	prtTime = t;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::IsPlaying(ISegment^ Segment)
{
	DMIDI_METHOD_START;
	return base->IsPlaying(Segment->baseref());
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetQueueTime(Int64% prtTime)
{
	DMIDI_METHOD_START;
	REFERENCE_TIME t;
	Int32 ret = base->GetQueueTime(&t);
	prtTime = t;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetResolvedTime(Int64 rtTime, Int64% prtResolved, UInt32 dwTimeResolveFlags)
{
	DMIDI_METHOD_START;
	REFERENCE_TIME r;
	Int32 ret = base->GetResolvedTime(rtTime, &r, dwTimeResolveFlags);
	prtResolved = r;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetTime(Int64% prtNow, Int32% pmtNow)
{
	DMIDI_METHOD_START;
	REFERENCE_TIME r;
	MUSIC_TIME m;
	Int32 ret = base->GetTime(&r, &m);
	prtNow = r;
	pmtNow = m;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::MusicToReferenceTime(Int32 mtTime, Int64% prtTime)
{
	DMIDI_METHOD_START;
	REFERENCE_TIME r;
	Int32 ret = base->MusicToReferenceTime(mtTime, &r);
	prtTime = r;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::ReferenceToMusicTime(Int64 rtTime, Int32% pmtTime)
{
	DMIDI_METHOD_START;
	MUSIC_TIME m;
	Int32 ret = base->ReferenceToMusicTime(rtTime, &m);
	pmtTime = m;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetPrepareTime(UInt32% pdwMilliSeconds)
{
	DMIDI_METHOD_START;
	DWORD ms;
	Int32 ret = base->GetPrepareTime(&ms);
	pdwMilliSeconds = ms;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::RhythmToTime(UInt16 wMeasure, Byte bBeat, Byte bGrid, Int16 nOffset, DMUS_TIMESIGNATURE% pTimeSig, Int32% pmtTime)
{
	DMIDI_METHOD_START;
	MUSIC_TIME time;
	pin_ptr<DMUS_TIMESIGNATURE> tsig = &pTimeSig;
	Int32 ret = base->RhythmToTime(wMeasure, bBeat, bGrid, nOffset, reinterpret_cast<::DMUS_TIMESIGNATURE*>(tsig), &time);
	pmtTime = time;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::SetPrepareTime(UInt32 dwMilliSeconds)
{
	DMIDI_METHOD_START;
	return base->SetPrepareTime(dwMilliSeconds);
	DMIDI_METHOD_END;
}

Int32 CPerformance::TimeToRhythm(Int32 mtTime, DMUS_TIMESIGNATURE% pTimeSig, UInt16% pwMeasure, Byte% pbBeat, Byte% pbGrid, Int16% pnOffset)
{
	DMIDI_METHOD_START;
	BYTE beat, grid;
	WORD measure;
	SHORT offset;
	pin_ptr<DMUS_TIMESIGNATURE> tsig = &pTimeSig;
	Int32 ret = base->TimeToRhythm(mtTime, reinterpret_cast<::DMUS_TIMESIGNATURE*>(tsig), &measure, &beat, &grid, &offset);
	pwMeasure = measure;
	pbBeat = beat;
	pbGrid = grid;
	pnOffset = offset;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::Invalidate(Int32 mtTime, UInt32 dwFlags)
{
	DMIDI_METHOD_START;
	return base->Invalidate(mtTime, dwFlags);
	DMIDI_METHOD_END;
}

Int32 CPerformance::GetBumperLength(UInt32% pdwMilliSeconds)
{
	DMIDI_METHOD_START;
	DWORD ms;
	Int32 ret = base->GetBumperLength(&ms);
	pdwMilliSeconds = ms;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::SetBumperLength(UInt32 dwMilliSeconds)
{
	DMIDI_METHOD_START;
	return base->SetBumperLength(dwMilliSeconds);
	DMIDI_METHOD_END;
}

Int32 CPerformance::PChannelInfo(UInt32 dwPChannel, IDirectMusicPort^% ppPort, UInt32% pdwGroup, UInt32% pdwMChannel)
{
	DMIDI_METHOD_START;
	::IDirectMusicPort* port;
	DWORD grp, chan;
	Int32 ret = base->PChannelInfo(dwPChannel, &port, &grp, &chan);	//must call release on return
	pdwGroup = grp;
	pdwMChannel = chan;
	ppPort = gcnew IDirectMusicPort(port, false);
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::PChannelInfo(UInt32 dwPChannel, IDirectMusicPort^% ppPort)
{
	DMIDI_METHOD_START;
	::IDirectMusicPort* port;
	Int32 ret = base->PChannelInfo(dwPChannel, &port, NULL, NULL);	//must call release on return
	ppPort = gcnew IDirectMusicPort(port, false);
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::PChannelInfo(UInt32 dwPChannel, UInt32% pdwGroup, UInt32% pdwMChannel)
{
	DMIDI_METHOD_START;
	DWORD grp, chan;
	Int32 ret = base->PChannelInfo(dwPChannel, NULL, &grp, &chan);
	pdwGroup = grp;
	pdwMChannel = chan;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::DownloadInstrument(CInstrument^ Instrument, UInt32 dwPChannel, UInt32% pdwGroup, UInt32% pdwMChannel)
{
	DMIDI_METHOD_START;
	DWORD grp, mch;
	Int32 ret = base->DownloadInstrument(Instrument->baseref(), dwPChannel, &grp, &mch);
	pdwGroup = grp;
	pdwMChannel = mch;
	return ret;
	DMIDI_METHOD_END;
}

Int32 CPerformance::UnloadInstrument(CInstrument^ Instrument)
{
	DMIDI_METHOD_START;
	return base->UnloadInstrument(Instrument->baseref());
	DMIDI_METHOD_END;
}

//
//	CAPathPerformance wrapper
//
Int32 CAPathPerformance::Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, IntPtr hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount, DMUS_AUDIOF dwFlags, DMUS_AUDIOPARAMS^ pAudioParams)
{
	DMIDI_METHOD_START;
	if (ppDirectSound)
	{
		throw gcnew ArgumentException("ERROR: IDirectSound is not wrapped; can't pass non-null here");
	}
	return base->Initialize(DMusic->baseref(), NULL, static_cast<HWND>(hWnd.ToPointer()), static_cast<DWORD>(dwDefaultPathType), dwPChannelCount, static_cast<DWORD>(dwFlags), pAudioParams ? &pAudioParams->baseref() : NULL);
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::CreateAudioPath(CAudioPath^% Path, DMUS_APATH dwType, UInt32 dwPChannelCount, Boolean bActivate)
{
	DMIDI_METHOD_START;
	Path = gcnew CAudioPath;
	return base->CreateAudioPath(Path->baseref(), static_cast<DWORD>(dwType), dwPChannelCount, bActivate);
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::PlaySegment(ISegment^ Segment, IAudioPath^ Path, DMUS_SEGF dwFlags, Int64 i64StartTime)
{
	DMIDI_METHOD_START;
	return base->PlaySegment(Segment->baseref(), Path ? &Path->baseref() : NULL, static_cast<DWORD>(dwFlags), i64StartTime);
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::PlaySegment(C3DSegment^ Segment, DMUS_SEGF dwFlags, Int64 i64StartTime)
{
	DMIDI_METHOD_START;
	return base->PlaySegment(Segment, static_cast<DWORD>(dwFlags), i64StartTime);
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::GetDefaultAudioPath(CAudioPath^% Path)
{
	DMIDI_METHOD_START;
	Path = gcnew CAudioPath;
	return base->GetDefaultAudioPath(Path);
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::SetDefaultAudioPath(CAudioPath^ Path)
{
	DMIDI_METHOD_START;
	return base->SetDefaultAudioPath(Path);
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::RemoveDefaultAudioPath()
{
	DMIDI_METHOD_START;
	return base->RemoveDefaultAudioPath();
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::ReleasePerformance()
{
	DMIDI_METHOD_START;
	return base->ReleasePerformance();
	DMIDI_METHOD_END;
}

Int32 CAPathPerformance::SendMidiMsg(MidiMessage bStatus, Byte bByte1, Byte bByte2, UInt32 dwPChannel, IAudioPath^ Path)
{
	DMIDI_METHOD_START;
	return base->SendMidiMsg(static_cast<BYTE>(bStatus), bByte1, bByte2, dwPChannel, Path->baseref());
	DMIDI_METHOD_END;
}

//
//	CPortPerformance wrapper
//
Int32 CPortPerformance::Initialize(CDirectMusic^ DMusic, /* IDirectSound * */Object^ pDirectSound, IntPtr hWnd)
{
	DMIDI_METHOD_START;
	if (pDirectSound)
	{
		throw gcnew ArgumentException("ERROR: IDirectSound is not wrapped; can't pass non-null here");
	}
	return base->Initialize(DMusic->baseref(), NULL, static_cast<HWND>(hWnd.ToPointer()));
	DMIDI_METHOD_END;
}

Int32 CPortPerformance::AddPort(COutputPort^ OutPort, UInt32 dwBlockNum, UInt32 dwGroup)
{
	DMIDI_METHOD_START;
	//NOTE: use winspool.h's define for AddPort, as this is what directmidi accidentally uses
	return base->winspool_AddPort(OutPort->baseref(), dwBlockNum, dwGroup);
	DMIDI_METHOD_END;
}

Int32 CPortPerformance::RemovePort()
{
	DMIDI_METHOD_START;
	return base->RemovePort();
	DMIDI_METHOD_END;
}

Int32 CPortPerformance::PlaySegment(ISegment^ Segment, DMUS_SEGF dwFlags, Int64 i64StartTime)
{
	DMIDI_METHOD_START;
	return base->PlaySegment(Segment->baseref(), static_cast<DWORD>(dwFlags), i64StartTime);
	DMIDI_METHOD_END;
}

Int32 CPortPerformance::ReleasePerformance()
{
	DMIDI_METHOD_START;
	return base->ReleasePerformance();
	DMIDI_METHOD_END;
}

Int32 CPortPerformance::AssignPChannel(COutputPort^ OutPort, UInt32 dwPChannel, UInt32 dwGroup, UInt32 dwMChannel)
{
	DMIDI_METHOD_START;
	return base->AssignPChannel(OutPort->baseref(), dwPChannel, dwGroup, dwMChannel);
	DMIDI_METHOD_END;
}

Int32 CPortPerformance::SendMidiMsg(MidiMessage bStatus, Byte bByte1, Byte bByte2, UInt32 dwPChannel)
{
	DMIDI_METHOD_START;
	return base->SendMidiMsg(static_cast<BYTE>(bStatus), bByte1, bByte2, dwPChannel);
	DMIDI_METHOD_END;
}

//
//	CAudioPath wrapper
//
Int32 IAudioPath::Activate(Boolean bActivate)
{
	DMIDI_METHOD_START;
	return base->Activate(bActivate);
	DMIDI_METHOD_END;
}

Int32 IAudioPath::SetVolume(Int32 lVolume, UInt32 dwDuration)
{
	DMIDI_METHOD_START;
	return base->SetVolume(lVolume, dwDuration);
	DMIDI_METHOD_END;
}

Int32 IAudioPath::Get3DBuffer(C3DBuffer^% _3DBuffer)
{
	DMIDI_METHOD_START;
	_3DBuffer = gcnew C3DBuffer;
	return base->Get3DBuffer(_3DBuffer->baseref());
	DMIDI_METHOD_END;
}

Int32 IAudioPath::Get3DListener(C3DListener^% _3DListener)
{
	DMIDI_METHOD_START;
	_3DListener = gcnew C3DListener;
	return base->Get3DListener(_3DListener->baseref());
	DMIDI_METHOD_END;
}

Int32 IAudioPath::ReleaseAudioPath()
{
	DMIDI_METHOD_START;
	return base->ReleaseAudioPath();
	DMIDI_METHOD_END;
}

static GUID GetObjectInPathIID(Type^ t)
{
	if (t->Equals(IDirectSoundFXGargle::typeid))		{ return IID_IDirectSoundFXGargle; }
	if (t->Equals(IDirectSoundFXChorus::typeid))		{ return IID_IDirectSoundFXChorus; }
	if (t->Equals(IDirectSoundFXFlanger::typeid))		{ return IID_IDirectSoundFXFlanger; }
	if (t->Equals(IDirectSoundFXEcho::typeid))			{ return IID_IDirectSoundFXEcho; }
	if (t->Equals(IDirectSoundFXDistortion::typeid))	{ return IID_IDirectSoundFXDistortion; }
	if (t->Equals(IDirectSoundFXCompressor::typeid))	{ return IID_IDirectSoundFXCompressor; }
	if (t->Equals(IDirectSoundFXParamEq::typeid))		{ return IID_IDirectSoundFXParamEq; }
	if (t->Equals(IDirectSoundFXWavesReverb::typeid))	{ return IID_IDirectSoundFXWavesReverb; }
	if (t->Equals(IDirectSoundFXI3DL2Reverb::typeid))	{ return IID_IDirectSoundFXI3DL2Reverb; }
	throw gcnew ArgumentException(String::Format("Type {0} can't be retrieved by GetObjectInPath", t->ToString()));
}

static Object^ GetObjectInPathInterface(Type^ t, void* iface)
{
	if (t->Equals(IDirectSoundFXGargle::typeid))		{ return gcnew IDirectSoundFXGargle((::IDirectSoundFXGargle*)iface, false); }
	if (t->Equals(IDirectSoundFXChorus::typeid))		{ return gcnew IDirectSoundFXChorus((::IDirectSoundFXChorus*)iface, false); }
	if (t->Equals(IDirectSoundFXFlanger::typeid))		{ return gcnew IDirectSoundFXFlanger((::IDirectSoundFXFlanger*)iface, false); }
	if (t->Equals(IDirectSoundFXEcho::typeid))			{ return gcnew IDirectSoundFXEcho((::IDirectSoundFXEcho*)iface, false); }
	if (t->Equals(IDirectSoundFXDistortion::typeid))	{ return gcnew IDirectSoundFXDistortion((::IDirectSoundFXDistortion*)iface, false); }
	if (t->Equals(IDirectSoundFXCompressor::typeid))	{ return gcnew IDirectSoundFXCompressor((::IDirectSoundFXCompressor*)iface, false); }
	if (t->Equals(IDirectSoundFXParamEq::typeid))		{ return gcnew IDirectSoundFXParamEq((::IDirectSoundFXParamEq*)iface, false); }
	if (t->Equals(IDirectSoundFXWavesReverb::typeid))	{ return gcnew IDirectSoundFXWavesReverb((::IDirectSoundFXWavesReverb*)iface, false); }
	if (t->Equals(IDirectSoundFXI3DL2Reverb::typeid))	{ return gcnew IDirectSoundFXI3DL2Reverb((::IDirectSoundFXI3DL2Reverb*)iface, false); }
	throw gcnew ArgumentException(String::Format("Type {0} can't be retrieved by GetObjectInPath", t->ToString()));
}

Int32 IAudioPath::GetObjectInPath(UInt32 dwPChannel, DMUS_PATH dwStage, UInt32 dwBuffer, Guid guidObject, UInt32 dwIndex, Type^ iidInterface, Object^% ppObject)
{
	DMIDI_METHOD_START;
	void* iface;
	Int32 ret = base->GetObjectInPath(dwPChannel, static_cast<DWORD>(dwStage), dwBuffer, fromGuid(guidObject), dwIndex, GetObjectInPathIID(iidInterface), &iface);
	ppObject = GetObjectInPathInterface(iidInterface, iface);
	return ret;
	DMIDI_METHOD_END;
}


//
//	C3DBuffer
//
Int32 C3DBuffer::GetMode(UInt32% pdwMode)
{
	DMIDI_METHOD_START;
	DWORD mode;
	Int32 ret = base->GetMode(&mode);
	pdwMode = mode;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetMode(UInt32 dwMode, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetMode(dwMode, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetAllBufferParameters(DS3DBUFFER% pDs3dBuffer)
{
	DMIDI_METHOD_START;
	pin_ptr<DS3DBUFFER> buf = &pDs3dBuffer;
	return base->GetAllBufferParameters(reinterpret_cast<::DS3DBUFFER*>(buf));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetAllBufferParameters(const DS3DBUFFER% pcDs3dBuffer, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	pin_ptr<const DS3DBUFFER> buf = &pcDs3dBuffer;
	return base->SetAllBufferParameters(reinterpret_cast<const ::DS3DBUFFER*>(buf), static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetMaxDistance(Single% pflMaxDistance)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetMaxDistance(&f);
	pflMaxDistance = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetMinDistance(Single% pflMinDistance)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetMinDistance(&f);
	pflMinDistance = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetMaxDistance(Single flMaxDistance, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetMaxDistance(flMaxDistance, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetMinDistance(Single flMinDistance, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetMinDistance(flMinDistance, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetBufferPosition(D3DVECTOR% pvPosition)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> pos = &pvPosition;
	return base->GetBufferPosition(reinterpret_cast<::D3DVECTOR*>(pos));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetBufferPosition(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetBufferPosition(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetConeAngles(UInt32% pdwInsideConeAngle, UInt32% pdwOutsideConeAngle)
{
	DMIDI_METHOD_START;
	DWORD in, out;
	Int32 ret = base->GetConeAngles(&in, &out);
	pdwInsideConeAngle = in;
	pdwOutsideConeAngle = out;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetConeOrientation(D3DVECTOR% pvOrientation)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> ori = &pvOrientation;
	return base->GetConeOrientation(reinterpret_cast<::D3DVECTOR*>(ori));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetConeOutsideVolume(Int32% plConeOutsideVolume)
{
	DMIDI_METHOD_START;
	LONG vol;
	Int32 ret = base->GetConeOutsideVolume(&vol);
	plConeOutsideVolume = vol;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetConeAngles(UInt32 dwInsideConeAngle, UInt32 dwOutsideConeAngle, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetConeAngles(dwInsideConeAngle, dwOutsideConeAngle, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetConeOrientation(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetConeOrientation(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetConeOutsideVolume(Int32 lConeOutsideVolume, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetConeOutsideVolume(lConeOutsideVolume, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::GetVelocity(D3DVECTOR% pvVelocity)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> vel = &pvVelocity;
	return base->GetVelocity(reinterpret_cast<::D3DVECTOR*>(vel));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::SetVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetVelocity(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DBuffer::ReleaseBuffer()
{
	DMIDI_METHOD_START;
	return base->ReleaseBuffer();
	DMIDI_METHOD_END;
}

//
//	C3DListener wrapper
//
Int32 C3DListener::CommitDeferredSettings()
{
	DMIDI_METHOD_START;
	return base->CommitDeferredSettings();
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetAllListenerParameters(DS3DLISTENER% pListener)
{
	DMIDI_METHOD_START;
	pin_ptr<DS3DLISTENER> lis = &pListener;
	return base->GetAllListenerParameters(reinterpret_cast<::DS3DLISTENER*>(lis));
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetAllListenerParameters(const DS3DLISTENER% pcListener, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	pin_ptr<const DS3DLISTENER> lis = &pcListener;
	return base->SetAllListenerParameters(reinterpret_cast<const ::DS3DLISTENER*>(lis), static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetDistanceFactor(Single% pflDistanceFactor)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetDistanceFactor(&f);
	pflDistanceFactor = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetDopplerFactor(Single% pflDopplerFactor)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetDopplerFactor(&f);
	pflDopplerFactor = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetRolloffFactor(Single% pflRolloffFactor)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetRolloffFactor(&f);
	pflRolloffFactor = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetDistanceFactor(Single flDistanceFactor, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetDistanceFactor(flDistanceFactor, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetDopplerFactor(Single flDopplerFactor, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetDistanceFactor(flDopplerFactor, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetRolloffFactor(Single flRolloffFactor, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetDistanceFactor(flRolloffFactor, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetOrientation(D3DVECTOR% pvOrientFront, D3DVECTOR% pvOrientTop)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> front = &pvOrientFront;
	pin_ptr<D3DVECTOR> top = &pvOrientTop;
	return base->GetOrientation(reinterpret_cast<::D3DVECTOR*>(front), reinterpret_cast<::D3DVECTOR*>(top));
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetListenerPosition(D3DVECTOR% pvPosition)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> pos = &pvPosition;
	return base->GetListenerPosition(reinterpret_cast<::D3DVECTOR*>(pos));
	DMIDI_METHOD_END;
}

Int32 C3DListener::GetVelocity(D3DVECTOR% pvVelocity)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> vel = &pvVelocity;
	return base->GetVelocity(reinterpret_cast<::D3DVECTOR*>(vel));
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetOrientation(Single xFront, Single yFront, Single zFront, Single xTop, Single yTop, Single zTop, UInt32 dwApply)
{
	DMIDI_METHOD_START;
	return base->SetOrientation(xFront, yFront, zFront, xTop, yTop, zTop, dwApply);
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetListenerPosition(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetListenerPosition(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DListener::SetVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetVelocity(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DListener::ReleaseListener()
{
	DMIDI_METHOD_START;
	return base->ReleaseListener();
	DMIDI_METHOD_END;
}

//
//	CSegment wrapper
//
Int32 ISegment::Download(CPerformance^ Performance)
{
	DMIDI_METHOD_START;
	return base->Download(Performance->baseref());
	DMIDI_METHOD_END;
}

Int32 ISegment::Unload(CPerformance^ Performance)
{
	DMIDI_METHOD_START;
	return base->Unload(Performance->baseref());
	DMIDI_METHOD_END;
}

Int32 ISegment::UnloadAllPerformances()
{
	DMIDI_METHOD_START;
	return base->UnloadAllPerformances();
	DMIDI_METHOD_END;
}

Int32 ISegment::ConnectToDLS(CCollection^ Collection)
{
	DMIDI_METHOD_START;
	return base->ConnectToDLS(Collection->baseref());
	DMIDI_METHOD_END;
}

Int32 ISegment::GetRepeats(UInt32% pdwRepeats)
{
	DMIDI_METHOD_START;
	DWORD r;
	Int32 ret = base->GetRepeats(&r);
	pdwRepeats = r;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetSegment(IDirectMusicSegment^% ppSegment)
{
	DMIDI_METHOD_START;
	::IDirectMusicSegment* iface;
	Int32 ret = base->GetSegment(&iface);	//must release iface
	ppSegment = gcnew IDirectMusicSegment(iface, false);
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetSeek(Int32% pmtSeek)
{
	DMIDI_METHOD_START;
	MUSIC_TIME t;
	Int32 ret = base->GetSeek(&t);
	pmtSeek = t;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetStartPoint(Int32% pmtStart)
{
	DMIDI_METHOD_START;
	MUSIC_TIME t;
	Int32 ret = base->GetStartPoint(&t);
	pmtStart = t;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetStartTime(Int32% pmtStart)
{
	DMIDI_METHOD_START;
	MUSIC_TIME t;
	Int32 ret = base->GetStartTime(&t);
	pmtStart = t;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetDefaultResolution(UInt32% pdwResolution)
{
	DMIDI_METHOD_START;
	DWORD r;
	Int32 ret = base->GetDefaultResolution(&r);
	pdwResolution = r;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetLength(Int32% pmtLength)
{
	DMIDI_METHOD_START;
	MUSIC_TIME len;
	Int32 ret = base->GetLength(&len);
	pmtLength = len;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::GetLoopPoints(Int32% pmtStart, Int32% pmtEnd)
{
	DMIDI_METHOD_START;
	MUSIC_TIME st, end;
	Int32 ret = base->GetLoopPoints(&st, &end);
	pmtStart = st;
	pmtEnd = end;
	return ret;
	DMIDI_METHOD_END;
}

Int32 ISegment::SetDefaultResolution(UInt32 dwResolution)
{
	DMIDI_METHOD_START;
	return base->SetDefaultResolution(dwResolution);
	DMIDI_METHOD_END;
}

Int32 ISegment::SetLength(Int32 mtLength)
{
	DMIDI_METHOD_START;
	return base->SetLength(mtLength);
	DMIDI_METHOD_END;
}

Int32 ISegment::SetLoopPoints(Int32 mtStart, Int32 mtEnd)
{
	DMIDI_METHOD_START;
	return base->SetLoopPoints(mtStart, mtEnd);
	DMIDI_METHOD_END;
}

Int32 ISegment::SetRepeats(UInt32 dwRepeats)
{
	DMIDI_METHOD_START;
	return base->SetRepeats(dwRepeats);
	DMIDI_METHOD_END;
}

Int32 ISegment::SetStartPoint(Int32 mtStart)
{
	DMIDI_METHOD_START;
	return base->SetStartPoint(mtStart);
	DMIDI_METHOD_END;
}

Int32 ISegment::SetPChannelsUsed(UInt32 dwNumPChannels, array<UInt32>^ paPChannels)
{
	DMIDI_METHOD_START;
	pin_ptr<UInt32> ch = &paPChannels[0];
	return base->SetPChannelsUsed(dwNumPChannels, reinterpret_cast<DWORD*>(ch));
	DMIDI_METHOD_END;
}

Int32 ISegment::ReleaseSegment()
{
	DMIDI_METHOD_START;
	return base->ReleaseSegment();
	DMIDI_METHOD_END;
}

CSegment^ CSegment::CloneFrom(const ISegment^ Segment)
{
	DMIDI_METHOD_START;
	base->operator=(const_cast<ISegment^>(Segment)->baseref());
	return this;
	DMIDI_METHOD_END;
}

//
//	C3DSegment (inherited from C3DBuffer)
//
Int32 C3DSegment::GetMode(UInt32% pdwMode)
{
	DMIDI_METHOD_START;
	DWORD mode;
	Int32 ret = base->GetMode(&mode);
	pdwMode = mode;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetMode(UInt32 dwMode, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetMode(dwMode, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetAllBufferParameters(DS3DBUFFER% pDs3dBuffer)
{
	DMIDI_METHOD_START;
	pin_ptr<DS3DBUFFER> buf = &pDs3dBuffer;
	return base->GetAllBufferParameters(reinterpret_cast<::DS3DBUFFER*>(buf));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetAllBufferParameters(const DS3DBUFFER% pcDs3dBuffer, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	pin_ptr<const DS3DBUFFER> buf = &pcDs3dBuffer;
	return base->SetAllBufferParameters(reinterpret_cast<const ::DS3DBUFFER*>(buf), static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetMaxDistance(Single% pflMaxDistance)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetMaxDistance(&f);
	pflMaxDistance = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetMinDistance(Single% pflMinDistance)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetMinDistance(&f);
	pflMinDistance = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetMaxDistance(Single flMaxDistance, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetMaxDistance(flMaxDistance, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetMinDistance(Single flMinDistance, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetMinDistance(flMinDistance, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetBufferPosition(D3DVECTOR% pvPosition)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> pos = &pvPosition;
	return base->GetBufferPosition(reinterpret_cast<::D3DVECTOR*>(pos));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetBufferPosition(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetBufferPosition(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetConeAngles(UInt32% pdwInsideConeAngle, UInt32% pdwOutsideConeAngle)
{
	DMIDI_METHOD_START;
	DWORD in, out;
	Int32 ret = base->GetConeAngles(&in, &out);
	pdwInsideConeAngle = in;
	pdwOutsideConeAngle = out;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetConeOrientation(D3DVECTOR% pvOrientation)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> ori = &pvOrientation;
	return base->GetConeOrientation(reinterpret_cast<::D3DVECTOR*>(ori));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetConeOutsideVolume(Int32% plConeOutsideVolume)
{
	DMIDI_METHOD_START;
	LONG vol;
	Int32 ret = base->GetConeOutsideVolume(&vol);
	plConeOutsideVolume = vol;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetConeAngles(UInt32 dwInsideConeAngle, UInt32 dwOutsideConeAngle, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetConeAngles(dwInsideConeAngle, dwOutsideConeAngle, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetConeOrientation(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetConeOrientation(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetConeOutsideVolume(Int32 lConeOutsideVolume, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetConeOutsideVolume(lConeOutsideVolume, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetBufferVelocity(D3DVECTOR% pvVelocity)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> vel = &pvVelocity;
	return base->directmidi::C3DBuffer::GetVelocity(reinterpret_cast<::D3DVECTOR*>(vel));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetBufferVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->directmidi::C3DBuffer::SetVelocity(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::ReleaseBuffer()
{
	DMIDI_METHOD_START;
	return base->ReleaseBuffer();
	DMIDI_METHOD_END;
}

//
//	C3DSegment (inherited from C3DListener)
//
Int32 C3DSegment::CommitDeferredSettings()
{
	DMIDI_METHOD_START;
	return base->CommitDeferredSettings();
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetAllListenerParameters(DS3DLISTENER% pListener)
{
	DMIDI_METHOD_START;
	pin_ptr<DS3DLISTENER> lis = &pListener;
	return base->GetAllListenerParameters(reinterpret_cast<::DS3DLISTENER*>(lis));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetAllListenerParameters(const DS3DLISTENER% pcListener, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	pin_ptr<const DS3DLISTENER> lis = &pcListener;
	return base->SetAllListenerParameters(reinterpret_cast<const ::DS3DLISTENER*>(lis), static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetDistanceFactor(Single% pflDistanceFactor)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetDistanceFactor(&f);
	pflDistanceFactor = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetDopplerFactor(Single% pflDopplerFactor)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetDopplerFactor(&f);
	pflDopplerFactor = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetRolloffFactor(Single% pflRolloffFactor)
{
	DMIDI_METHOD_START;
	D3DVALUE f;
	Int32 ret = base->GetRolloffFactor(&f);
	pflRolloffFactor = f;
	return ret;
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetDistanceFactor(Single flDistanceFactor, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetDistanceFactor(flDistanceFactor, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetDopplerFactor(Single flDopplerFactor, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetDistanceFactor(flDopplerFactor, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetRolloffFactor(Single flRolloffFactor, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetDistanceFactor(flRolloffFactor, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetOrientation(D3DVECTOR% pvOrientFront, D3DVECTOR% pvOrientTop)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> front = &pvOrientFront;
	pin_ptr<D3DVECTOR> top = &pvOrientTop;
	return base->GetOrientation(reinterpret_cast<::D3DVECTOR*>(front), reinterpret_cast<::D3DVECTOR*>(top));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetListenerPosition(D3DVECTOR% pvPosition)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> pos = &pvPosition;
	return base->GetListenerPosition(reinterpret_cast<::D3DVECTOR*>(pos));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetListenerVelocity(D3DVECTOR% pvVelocity)
{
	DMIDI_METHOD_START;
	pin_ptr<D3DVECTOR> vel = &pvVelocity;
	return base->directmidi::C3DListener::GetVelocity(reinterpret_cast<::D3DVECTOR*>(vel));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetOrientation(Single xFront, Single yFront, Single zFront, Single xTop, Single yTop, Single zTop, UInt32 dwApply)
{
	DMIDI_METHOD_START;
	return base->SetOrientation(xFront, yFront, zFront, xTop, yTop, zTop, dwApply);
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetListenerPosition(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->SetListenerPosition(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetListenerVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply)
{
	DMIDI_METHOD_START;
	return base->directmidi::C3DListener::SetVelocity(x, y, z, static_cast<DWORD>(dwApply));
	DMIDI_METHOD_END;
}

Int32 C3DSegment::ReleaseListener()
{
	DMIDI_METHOD_START;
	return base->ReleaseListener();
	DMIDI_METHOD_END;
}


//
//	C3DSegment
//
Int32 C3DSegment::Initialize(CAPathPerformance^ Performance, DMUS_APATH dwType, UInt32 dwPChannelCount)
{
	DMIDI_METHOD_START;
	return base->Initialize(Performance->baseref(), static_cast<DWORD>(dwType), dwPChannelCount);
	DMIDI_METHOD_END;
}

Int32 C3DSegment::Initialize(CAudioPath^ Path)
{
	DMIDI_METHOD_START;
	return base->Initialize(Path->baseref());
	DMIDI_METHOD_END;
}

Int32 C3DSegment::ReleaseSegment()
{
	DMIDI_METHOD_START;
	return base->ReleaseSegment();
	DMIDI_METHOD_END;
}

Int32 C3DSegment::SetVolume(Int32 lVolume, UInt32 dwDuration)
{
	DMIDI_METHOD_START;
	return base->SetVolume(lVolume, dwDuration);
	DMIDI_METHOD_END;
}

Int32 C3DSegment::GetObjectInPath(UInt32 dwPChannel, DMUS_PATH dwStage, UInt32 dwBuffer, Guid guidObject, UInt32 dwIndex, Type^ iidInterface, Object^% ppObject)
{
	DMIDI_METHOD_START;
	void* iface;
	Int32 ret = base->GetObjectInPath(dwPChannel, static_cast<DWORD>(dwStage), dwBuffer, fromGuid(guidObject), dwIndex, GetObjectInPathIID(iidInterface), &iface);
	ppObject = GetObjectInPathInterface(iidInterface, iface);
	return ret;
	DMIDI_METHOD_END;
}

C3DSegment^ C3DSegment::CloneFrom(const C3DSegment^ Segment)
{
	DMIDI_METHOD_START;
	base->operator=(const_cast<C3DSegment^>(Segment)->baseref());
	return this;
	DMIDI_METHOD_END;
}

C3DSegment^ C3DSegment::CloneFrom(const ISegment^ Segment)
{
	DMIDI_METHOD_START;
	base->operator=(const_cast<ISegment^>(Segment)->baseref());
	return this;
	DMIDI_METHOD_END;
}

IAudioPath^ C3DSegment::GetAudioPath()
{
	DMIDI_METHOD_START;
	return gcnew CAudioPathPtr(&base->GetAudioPath());
	DMIDI_METHOD_END;
}


}	//namespace directMidi
