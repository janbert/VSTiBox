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
Module : AudioPart.h
Purpose: Definition of wrappers around classes defined in CAudioPart.h
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
//	CPerformance wrapper
//
public ref class CPerformance abstract : public DirectMidi::WrapperBaseNoDestructor<directmidi::CPerformance>
{
public:
	Int32 Stop(ISegment^ Segment, Int64 i64StopTime, UInt32 dwFlags);
	Int32 Stop(ISegment^ Segment, Int64 i64StopTime) { return Stop(Segment, i64StopTime, 0); }
	Int32 Stop(ISegment^ Segment) { return Stop(Segment, 0); }
	Int32 StopAll();
	Int32 SetMasterVolume(Int32 nVolume);
	Int32 SetMasterTempo(Single fTempo);
	Int32 GetMasterVolume([Out] Int32% nVolume);
	Int32 GetMasterTempo([Out] Single% nTempo);
	Int32 IsPlaying(ISegment^ Segment);
	Int32 AdjustTime(Int64 rtAmount);
	Int32 GetLatencyTime([Out] Int64% prtTime);
	Int32 GetPrepareTime([Out] UInt32% pdwMilliSeconds);
	Int32 GetQueueTime([Out] Int64% prtTime);
	Int32 GetResolvedTime(Int64 rtTime, [Out] Int64% prtResolved, UInt32 dwTimeResolveFlags);
	Int32 GetTime([Out] Int64% prtNow, [Out] Int32% pmtNow);
	Int32 MusicToReferenceTime(Int32 mtTime, [Out] Int64% prtTime);
	Int32 ReferenceToMusicTime(Int64 rtTime, [Out] Int32% pmtTime);
	Int32 RhythmToTime(UInt16 wMeasure, Byte bBeat, Byte bGrid, Int16 nOffset, [In] DMUS_TIMESIGNATURE% pTimeSig, [Out] Int32% pmtTime);
	Int32 SetPrepareTime(UInt32 dwMilliSeconds);
	Int32 TimeToRhythm(Int32 mtTime, [In] DMUS_TIMESIGNATURE% pTimeSig, [Out] UInt16% pwMeasure, [Out] Byte% pbBeat, [Out] Byte% pbGrid, [Out] Int16% pnOffset);
	Int32 Invalidate(Int32 mtTime, UInt32 dwFlags);
	Int32 GetBumperLength([Out] UInt32% pdwMilliSeconds);
	Int32 SetBumperLength(UInt32 dwMilliSeconds);
	Int32 PChannelInfo(UInt32 dwPChannel, [Out] IDirectMusicPort^% ppPort, [Out] UInt32% pdwGroup, [Out] UInt32% pdwMChannel);
	Int32 PChannelInfo(UInt32 dwPChannel, [Out] IDirectMusicPort^% ppPort);
	Int32 PChannelInfo(UInt32 dwPChannel, [Out] UInt32% pdwGroup, [Out] UInt32% pdwMChannel);
	Int32 DownloadInstrument(CInstrument^ Instrument, UInt32 dwPChannel, [Out] UInt32% pdwGroup, [Out] UInt32% pdwMChannel);
	Int32 UnloadInstrument(CInstrument^ Instrument);
};

//
//	CAPathPerformance wrapper
//
public ref class CAPathPerformance : public CPerformance, public IDisposable
{
	DMIDI_INHERITED_BASE(CAPathPerformance, CPerformance, directmidi::CAPathPerformance);

public:
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, IntPtr hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount, DMUS_AUDIOF dwFlags, DMUS_AUDIOPARAMS^ pAudioParams);
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, Control^ hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount, DMUS_AUDIOF dwFlags, DMUS_AUDIOPARAMS^ pAudioParams) { return Initialize(DMusic, ppDirectSound, hWnd ? hWnd->Handle : IntPtr::Zero, dwDefaultPathType, dwPChannelCount, dwFlags, pAudioParams); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, IntPtr hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount, DMUS_AUDIOF dwFlags) { return Initialize(DMusic, ppDirectSound, hWnd, dwDefaultPathType, dwPChannelCount, dwFlags, nullptr); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, Control^ hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount, DMUS_AUDIOF dwFlags) { return Initialize(DMusic, ppDirectSound, hWnd ? hWnd->Handle : IntPtr::Zero, dwDefaultPathType, dwPChannelCount, dwFlags); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, IntPtr hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount) { return Initialize(DMusic, ppDirectSound, hWnd, dwDefaultPathType, dwPChannelCount, static_cast<DMUS_AUDIOF>(0)); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, Control^ hWnd, DMUS_APATH dwDefaultPathType, UInt32 dwPChannelCount) { return Initialize(DMusic, ppDirectSound, hWnd ? hWnd->Handle : IntPtr::Zero, dwDefaultPathType, dwPChannelCount); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, IntPtr hWnd, DMUS_APATH dwDefaultPathType) { return Initialize(DMusic, ppDirectSound, hWnd, dwDefaultPathType, 64); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, Control^ hWnd, DMUS_APATH dwDefaultPathType) { return Initialize(DMusic, ppDirectSound, hWnd ? hWnd->Handle : IntPtr::Zero, dwDefaultPathType); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, IntPtr hWnd) { return Initialize(DMusic, ppDirectSound, hWnd, DMUS_APATH::SHARED_STEREOPLUSREVERB); }
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound ** */Object^ ppDirectSound, Control^ hWnd) { return Initialize(DMusic, ppDirectSound, hWnd ? hWnd->Handle : IntPtr::Zero); }
	Int32 CreateAudioPath([Out] CAudioPath^% Path, DMUS_APATH dwType, UInt32 dwPChannelCount, Boolean bActivate);
	Int32 PlaySegment(ISegment^ Segment, IAudioPath^ Path, DMUS_SEGF dwFlags, Int64 i64StartTime);
	Int32 PlaySegment(ISegment^ Segment, IAudioPath^ Path, DMUS_SEGF dwFlags) { return PlaySegment(Segment, Path, dwFlags, 0); }
	Int32 PlaySegment(ISegment^ Segment, IAudioPath^ Path) { return PlaySegment(Segment, Path, static_cast<DMUS_SEGF>(0)); }
	Int32 PlaySegment(C3DSegment^ Segment, DMUS_SEGF dwFlags, Int64 i64StartTime);
	Int32 PlaySegment(C3DSegment^ Segment, DMUS_SEGF dwFlags) { return PlaySegment(Segment, dwFlags, 0); }
	Int32 PlaySegment(C3DSegment^ Segment) { return PlaySegment(Segment, static_cast<DMUS_SEGF>(0)); }
	Int32 GetDefaultAudioPath([Out] CAudioPath^% Path);
	Int32 SetDefaultAudioPath(CAudioPath^ Path);
	Int32 RemoveDefaultAudioPath();
	Int32 ReleasePerformance();
	Int32 SendMidiMsg(MidiMessage bStatus, Byte bByte1, Byte bByte2, UInt32 dwPChannel, IAudioPath^ Path);
};

//
//	CPortPerformance wrapper
//

//NOTE: winspool.h has a #define for AddPort, which renames directmidi's AddPort function
#ifdef UNICODE
#define winspool_AddPort  AddPortW
#else
#define winspool_AddPort  AddPortA
#endif // !UNICODE
#undef AddPort

public ref class CPortPerformance : public CPerformance, public IDisposable
{
	DMIDI_INHERITED_BASE(CPortPerformance, CPerformance, directmidi::CPortPerformance);

public:
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound * */Object^ pDirectSound, IntPtr hWnd);
	Int32 Initialize(CDirectMusic^ DMusic, /* IDirectSound * */Object^ pDirectSound, Control^ hWnd) { return Initialize(DMusic, pDirectSound, hWnd ? hWnd->Handle : IntPtr::Zero); }
	Int32 AddPort(COutputPort^ OutPort, UInt32 dwBlockNum, UInt32 dwGroup);
	Int32 RemovePort();
	Int32 PlaySegment(ISegment^ Segment, DMUS_SEGF dwFlags, Int64 i64StartTime);
	Int32 PlaySegment(ISegment^ Segment, DMUS_SEGF dwFlags) { return PlaySegment(Segment, dwFlags, 0); }
	Int32 PlaySegment(ISegment^ Segment) { return PlaySegment(Segment, static_cast<DMUS_SEGF>(0)); }
	Int32 ReleasePerformance();
	Int32 AssignPChannel(COutputPort^ OutPort, UInt32 dwPChannel, UInt32 dwGroup, UInt32 dwMChannel);
	Int32 SendMidiMsg(MidiMessage bStatus, Byte bByte1, Byte bByte2, UInt32 dwPChannel);
};

//
//	CAudioPath wrapper
//
public ref class IAudioPath abstract : public WrapperBaseNoDestructor<directmidi::CAudioPath>
{
public:
	Int32 Activate(Boolean bActivate);
	Int32 SetVolume(Int32 lVolume, UInt32 dwDuration);
	Int32 Get3DBuffer([Out] C3DBuffer^% _3DBuffer);	
	Int32 Get3DListener([Out] C3DListener^% _3DListener);
	Int32 ReleaseAudioPath();
	Int32 GetObjectInPath(UInt32 dwPChannel, DMUS_PATH dwStage, UInt32 dwBuffer, Guid guidObject, UInt32 dwIndex, Type^ iidInterface, [Out] Object^% ppObject);
};

public ref class CAudioPath : public IAudioPath, public IDisposable
{
	DMIDI_INHERITED_BASE(CAudioPath, IAudioPath, directmidi::CAudioPath);
};

//
//	C3DBuffer wrapper
//
public ref class C3DBuffer : public DMidiWrapperBase<directmidi::C3DBuffer>
{
public:
	Int32 GetMode([Out] UInt32% pdwMode);
	Int32 SetMode(UInt32 dwMode, DS3D_APPLY dwApply);
	Int32 SetMode(UInt32 dwMode) { return SetMode(dwMode, DS3D_APPLY::IMMEDIATE); }
	Int32 GetAllBufferParameters([Out] DS3DBUFFER% pDs3dBuffer);
	Int32 SetAllBufferParameters([In] const DS3DBUFFER% pcDs3dBuffer, DS3D_APPLY dwApply);
	Int32 SetAllBufferParameters([In] const DS3DBUFFER% pcDs3dBuffer) { return SetAllBufferParameters(pcDs3dBuffer, DS3D_APPLY::IMMEDIATE); }
	Int32 GetMaxDistance([Out] Single% pflMaxDistance);
	Int32 GetMinDistance([Out] Single% pflMinDistance);
	Int32 SetMaxDistance(Single flMaxDistance, DS3D_APPLY dwApply);
	Int32 SetMaxDistance(Single flMaxDistance) { return SetMaxDistance(flMaxDistance, DS3D_APPLY::IMMEDIATE); }
	Int32 SetMinDistance(Single flMinDistance, DS3D_APPLY dwApply);
	Int32 SetMinDistance(Single flMinDistance) { return SetMinDistance(flMinDistance, DS3D_APPLY::IMMEDIATE); }
	Int32 GetBufferPosition([Out] D3DVECTOR% pvPosition);
	Int32 SetBufferPosition(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetBufferPosition(Single x, Single y, Single z) { return SetBufferPosition(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 GetConeAngles([Out] UInt32% pdwInsideConeAngle, [Out] UInt32% pdwOutsideConeAngle);
	Int32 GetConeOrientation([Out] D3DVECTOR% pvOrientation);
	Int32 GetConeOutsideVolume([Out] Int32% plConeOutsideVolume);
	Int32 SetConeAngles(UInt32 dwInsideConeAngle, UInt32 dwOutsideConeAngle, DS3D_APPLY dwApply);
	Int32 SetConeAngles(UInt32 dwInsideConeAngle, UInt32 dwOutsideConeAngle) { return SetConeAngles(dwInsideConeAngle, dwOutsideConeAngle, DS3D_APPLY::IMMEDIATE); }
	Int32 SetConeOrientation(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetConeOrientation(Single x, Single y, Single z) { return SetConeOrientation(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 SetConeOutsideVolume(Int32 lConeOutsideVolume, DS3D_APPLY dwApply);
	Int32 SetConeOutsideVolume(Int32 lConeOutsideVolume) { return SetConeOutsideVolume(lConeOutsideVolume, DS3D_APPLY::IMMEDIATE); }
	Int32 GetVelocity([Out] D3DVECTOR% pvVelocity);
	Int32 SetVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetVelocity(Single x, Single y, Single z) { return SetVelocity(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 ReleaseBuffer();
};

//
//	C3DListener wrapper
//
public ref class C3DListener : public DMidiWrapperBase<directmidi::C3DListener>
{
public:
	Int32 CommitDeferredSettings();
	Int32 GetAllListenerParameters([Out] DS3DLISTENER% pListener);
	Int32 SetAllListenerParameters([In] const DS3DLISTENER% pcListener, DS3D_APPLY dwApply);
	Int32 SetAllListenerParameters([In] const DS3DLISTENER% pcListener) { return SetAllListenerParameters(pcListener, DS3D_APPLY::IMMEDIATE); }
	Int32 GetDistanceFactor([Out] Single% pflDistanceFactor);
	Int32 GetDopplerFactor([Out] Single% pflDopplerFactor);
	Int32 GetRolloffFactor([Out] Single% pflRolloffFactor);
	Int32 SetDistanceFactor(Single flDistanceFactor, DS3D_APPLY dwApply);
	Int32 SetDistanceFactor(Single flDistanceFactor) { return SetDistanceFactor(flDistanceFactor, DS3D_APPLY::IMMEDIATE); }
	Int32 SetDopplerFactor(Single flDopplerFactor, DS3D_APPLY dwApply);
	Int32 SetDopplerFactor(Single flDopplerFactor) { return SetDopplerFactor(flDopplerFactor, DS3D_APPLY::IMMEDIATE); }
	Int32 SetRolloffFactor(Single flRolloffFactor, DS3D_APPLY dwApply);
	Int32 SetRolloffFactor(Single flRolloffFactor) { return SetRolloffFactor(flRolloffFactor, DS3D_APPLY::IMMEDIATE); }
	Int32 GetOrientation([Out] D3DVECTOR% pvOrientFront, [Out] D3DVECTOR% pvOrientTop);
	Int32 GetListenerPosition([Out] D3DVECTOR% pvPosition);
	Int32 GetVelocity([Out] D3DVECTOR% pvVelocity);
	Int32 SetOrientation(Single xFront, Single yFront, Single zFront, Single xTop, Single yTop, Single zTop, UInt32 dwApply);
	Int32 SetListenerPosition(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetListenerPosition(Single x, Single y, Single z) { return SetListenerPosition(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 SetVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetVelocity(Single x, Single y, Single z) { return SetVelocity(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 ReleaseListener();
};

//
//	CSegment wrapper
//
public ref class ISegment abstract : public DirectMidi::WrapperBaseNoDestructor<directmidi::CSegment>
{
public:
	Int32 Download(CPerformance^ Performance);
	Int32 Unload(CPerformance^ Performance);
	Int32 UnloadAllPerformances();
	Int32 ConnectToDLS(CCollection^ Collection);
	Int32 GetRepeats([Out] UInt32% pdwRepeats);
	Int32 GetSeek([Out] Int32% pmtSeek);
	Int32 GetSegment([Out] IDirectMusicSegment^% ppSegment);
	Int32 GetStartPoint([Out] Int32% pmtStart);
	Int32 GetStartTime([Out] Int32% pmtStart);
	Int32 GetDefaultResolution([Out] UInt32% pdwResolution);
	Int32 GetLength([Out] Int32% pmtLength);
	Int32 GetLoopPoints([Out] Int32% pmtStart, [Out] Int32% pmtEnd);
	Int32 SetDefaultResolution(UInt32 dwResolution);
	Int32 SetLength(Int32 mtLength);
	Int32 SetLoopPoints(Int32 mtStart, Int32 mtEnd);
	Int32 SetRepeats(UInt32 dwRepeats);
	Int32 SetStartPoint(Int32 mtStart);
	Int32 SetPChannelsUsed(UInt32 dwNumPChannels, array<UInt32>^ paPChannels);
	Int32 ReleaseSegment();
};

public ref class CSegment : public ISegment, public IDisposable
{
	DMIDI_INHERITED_BASE(CSegment, ISegment, directmidi::CSegment);

public:
	//equals operator
	CSegment^ CloneFrom(const ISegment^ Segment);
};

//
//	C3DSegment wrapper
//
//	Although in C++ this inherits from multiple classes,
//	.net doesn't allow multiple-base-class inheritance;
//	due to the difficulty of doing multiple-inheritance using interfaces
//	in this wrapper infrastructure, and due to the limited number
//	of methods which require references to the interfaces we chose
//	not to inherit from, this only inherits from CSegment.
//	
public ref class C3DSegment : public ISegment, public IDisposable
{
	DMIDI_INHERITED_BASE(C3DSegment, ISegment, directmidi::C3DSegment);

//
//	C3DBuffer members
//
public:
	Int32 GetMode([Out] UInt32% pdwMode);
	Int32 SetMode(UInt32 dwMode, DS3D_APPLY dwApply);
	Int32 SetMode(UInt32 dwMode) { return SetMode(dwMode, DS3D_APPLY::IMMEDIATE); }
	Int32 GetAllBufferParameters([Out] DS3DBUFFER% pDs3dBuffer);
	Int32 SetAllBufferParameters([In] const DS3DBUFFER% pcDs3dBuffer, DS3D_APPLY dwApply);
	Int32 SetAllBufferParameters([In] const DS3DBUFFER% pcDs3dBuffer) { return SetAllBufferParameters(pcDs3dBuffer, DS3D_APPLY::IMMEDIATE); }
	Int32 GetMaxDistance([Out] Single% pflMaxDistance);
	Int32 GetMinDistance([Out] Single% pflMinDistance);
	Int32 SetMaxDistance(Single flMaxDistance, DS3D_APPLY dwApply);
	Int32 SetMaxDistance(Single flMaxDistance) { return SetMaxDistance(flMaxDistance, DS3D_APPLY::IMMEDIATE); }
	Int32 SetMinDistance(Single flMinDistance, DS3D_APPLY dwApply);
	Int32 SetMinDistance(Single flMinDistance) { return SetMinDistance(flMinDistance, DS3D_APPLY::IMMEDIATE); }
	Int32 GetBufferPosition([Out] D3DVECTOR% pvPosition);
	Int32 SetBufferPosition(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetBufferPosition(Single x, Single y, Single z) { return SetBufferPosition(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 GetConeAngles([Out] UInt32% pdwInsideConeAngle, [Out] UInt32% pdwOutsideConeAngle);
	Int32 GetConeOrientation([Out] D3DVECTOR% pvOrientation);
	Int32 GetConeOutsideVolume([Out] Int32% plConeOutsideVolume);
	Int32 SetConeAngles(UInt32 dwInsideConeAngle, UInt32 dwOutsideConeAngle, DS3D_APPLY dwApply);
	Int32 SetConeAngles(UInt32 dwInsideConeAngle, UInt32 dwOutsideConeAngle) { return SetConeAngles(dwInsideConeAngle, dwOutsideConeAngle, DS3D_APPLY::IMMEDIATE); }
	Int32 SetConeOrientation(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetConeOrientation(Single x, Single y, Single z) { return SetConeOrientation(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 SetConeOutsideVolume(Int32 lConeOutsideVolume, DS3D_APPLY dwApply);
	Int32 SetConeOutsideVolume(Int32 lConeOutsideVolume) { return SetConeOutsideVolume(lConeOutsideVolume, DS3D_APPLY::IMMEDIATE); }
	Int32 GetBufferVelocity([Out] D3DVECTOR% pvVelocity);
	Int32 SetBufferVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetBufferVelocity(Single x, Single y, Single z) { return SetBufferVelocity(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 ReleaseBuffer();

//
//	C3DListener members
//
public:
	Int32 CommitDeferredSettings();
	Int32 GetAllListenerParameters([Out] DS3DLISTENER% pListener);
	Int32 SetAllListenerParameters([In] const DS3DLISTENER% pcListener, DS3D_APPLY dwApply);
	Int32 SetAllListenerParameters([In] const DS3DLISTENER% pcListener) { return SetAllListenerParameters(pcListener, DS3D_APPLY::IMMEDIATE); }
	Int32 GetDistanceFactor([Out] Single% pflDistanceFactor);
	Int32 GetDopplerFactor([Out] Single% pflDopplerFactor);
	Int32 GetRolloffFactor([Out] Single% pflRolloffFactor);
	Int32 SetDistanceFactor(Single flDistanceFactor, DS3D_APPLY dwApply);
	Int32 SetDistanceFactor(Single flDistanceFactor) { return SetDistanceFactor(flDistanceFactor, DS3D_APPLY::IMMEDIATE); }
	Int32 SetDopplerFactor(Single flDopplerFactor, DS3D_APPLY dwApply);
	Int32 SetDopplerFactor(Single flDopplerFactor) { return SetDopplerFactor(flDopplerFactor, DS3D_APPLY::IMMEDIATE); }
	Int32 SetRolloffFactor(Single flRolloffFactor, DS3D_APPLY dwApply);
	Int32 SetRolloffFactor(Single flRolloffFactor) { return SetRolloffFactor(flRolloffFactor, DS3D_APPLY::IMMEDIATE); }
	Int32 GetOrientation([Out] D3DVECTOR% pvOrientFront, [Out] D3DVECTOR% pvOrientTop);
	Int32 GetListenerPosition([Out] D3DVECTOR% pvPosition);
	Int32 GetListenerVelocity([Out] D3DVECTOR% pvVelocity);
	Int32 SetOrientation(Single xFront, Single yFront, Single zFront, Single xTop, Single yTop, Single zTop, UInt32 dwApply);
	Int32 SetListenerPosition(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetListenerPosition(Single x, Single y, Single z) { return SetListenerPosition(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 SetListenerVelocity(Single x, Single y, Single z, DS3D_APPLY dwApply);
	Int32 SetListenerVelocity(Single x, Single y, Single z) { return SetListenerVelocity(x, y, z, DS3D_APPLY::IMMEDIATE); }
	Int32 ReleaseListener();

//
//	C3DSegment members
//
public:
	Int32 Initialize(CAPathPerformance^ Performance, DMUS_APATH dwType, UInt32 dwPChannelCount);
	Int32 Initialize(CAPathPerformance^ Performance, DMUS_APATH dwType) { return Initialize(Performance, dwType, 64); }
	Int32 Initialize(CAPathPerformance^ Performance) { return Initialize(Performance, DMUS_APATH::DYNAMIC_3D); }
	Int32 Initialize(CAudioPath^ Path);
	Int32 ReleaseSegment();
	Int32 SetVolume(Int32 lVolume, UInt32 dwDuration);
	Int32 GetObjectInPath(UInt32 dwPChannel, DMUS_PATH dwStage, UInt32 dwBuffer, Guid guidObject, UInt32 dwIndex, Type^ iidInterface, [Out] Object^% ppObject);

	//NOTE: the return value should not be used
	//		past the lifespan of this object, 
	//		or past the next method call for this item
	IAudioPath^ GetAudioPath();

	//equals operators
	C3DSegment^ CloneFrom(const C3DSegment^ Segment);
	C3DSegment^ CloneFrom(const ISegment^ Segment);

private:
	//this class is used for returning from GetAudioPath
	ref class CAudioPathPtr : public IAudioPath, public IDisposable
	{
	public:
		CAudioPathPtr(directmidi::CAudioPath* ptr) { base = ptr; }
		~CAudioPathPtr() { this->!CAudioPathPtr(); }
		!CAudioPathPtr() { base = NULL; }
	};

};


}	//namespace directMidi
