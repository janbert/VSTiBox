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
Module : WrapperBase.h
Purpose: Declaration of classes
Created: JH / 07-19-2007
History: JH / 07-19-2007 
Date format: Day-month-year

	Update: 07/19/2007

	1. Fixed some method signature bugs

	2. Added support for getting/setting output port parameters with KsProperty
		
	3. Added support for both retrieving and implementing custom DirectMidi tool graphs on segments

	4. "Fixed" CSegment class in C++ code to return a reference to the IDirectMusicSegment even if it's not playing
	
*/

namespace DirectMidi {

//
//	Declarations for DirectMusicDefs.h
//
ref class IDirectSoundFXWavesReverb;
ref class IDirectSoundFXGargle;
ref class IDirectSoundFXChorus;
ref class IDirectSoundFXFlanger;
ref class IDirectSoundFXEcho;
ref class IDirectSoundFXDistortion;
ref class IDirectSoundFXCompressor;
ref class IDirectSoundFXParamEq;
ref class IDirectSoundFXI3DL2Reverb;
ref class IReferenceClock;
ref class IDirectMusicSegment;
ref class IDirectMusicPort;
ref class IDirectMusicPerformance;

//
//	Declarations for DirectMusicTools.h
//
ref class IDirectMusicTool;
ref class IDirectMusicGraph;

ref class CDirectMusicTool;

ref class DMUS_PMSG;

//
//	Declarations for MidiPart.h
//
ref class CInstrument;
ref class CCollection;
ref class CDLSLoader;
interface class CMidiPort;
ref class CReceiver;
ref class CInputPort;
ref class COutputPort;
ref class CDirectMusic;
ref class CSampleInstrument;
ref class CMasterClock;

//
//	Declarations for AudioPart.h
//
ref class ISegment;
ref class CSegment;
ref class CPerformance;
ref class CAPathPerformance;
ref class CPortPerformance;
ref class IAudioPath;
ref class CAudioPath;
ref class C3DSegment;
ref class C3DListener;
ref class C3DBuffer;

}	//namespace DirectMidi
