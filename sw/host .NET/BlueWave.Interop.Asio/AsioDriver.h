//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

#pragma once
#pragma managed
using namespace System;

#include "AsioRedirect.h"

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
				// represents an ASIO driver (also some static for all drivers)
				public ref class AsioDriver
				{
				internal:

					// we'll maintain a list of drivers
					static array<InstalledDriver^>^ _installedDrivers;

					// but you can only have one active at once
					static AsioDriver^ _instance;

					// our elusive COM interface
					IAsio* _pDriver;

					// a struct which specifies callback addresses
					ASIOCallbacks* _pCallbacks;

					// the number of input channels supported by the driver, and our max
					int _nInputs;

					// and the number of output channels
					int _nOutputs;

					// is it usefull to call outputReady each time we have updated the outputbuffers 
					ASIOError _outputReadySupport;

					// the static callback methods - we'll forward these to instance members
					static void OnSampleRateDidChange(ASIOSampleRate rate);
					static long OnAsioMessage(long selector, long value, void* message, double* opt);

					// select a driver once an instance of this class has been created
					void InternalSelectDriver(InstalledDriver^ installedDriver);

					// our instance based handlers
					void OnBufferSwitch(long doubleBufferIndex, ASIOBool directProcess);
					ASIOTime* OnBufferSwitchTimeInfo(ASIOTime* params, long doubleBufferIndex, ASIOBool directProcess);

					// safety function to make sure all is well before attempting any operations
					void CheckInitialised();

					// the input channels
					array<Channel^>^ _inputChannels;

					// the output channels
					array<Channel^>^ _outputChannels;

					// C++ likes 'non-trivial' events
					EventHandler^ _bufferUpdateEvent;

					// return the instance of currently selected driver
					static property AsioDriver^ Instance { AsioDriver^ get(); }

				public:

					// returns the installed drivers
					static property array<InstalledDriver^>^ InstalledDrivers	{ array<InstalledDriver^>^ get(); }

					// select and initialise driver
					static AsioDriver^ SelectDriver(InstalledDriver^ installedDriver);

					// basic information properties
					property int					Version					{ int get(); }
					property String^				DriverName				{ String^ get(); }
					property String^				ErrorMessage			{ String^ get(); }
					property int					NumberInputChannels		{ int get(); }
					property int					NumberOutputChannels	{ int get(); }
					property BufferSize^			BufferSizex				{ BufferSize^ get(); }
					property double					SampleRate				{ double get();}
					property array<Channel^>^		InputChannels			{ array<Channel^>^ get(); }
					property array<Channel^>^		OutputChannels			{ array<Channel^>^ get(); }

					// basic methods
					void Start();
					void Stop();
					void ShowControlPanel();
					void CreateBuffers(int bufferSize);
					void DisposeBuffers();
					void Release();
					void SetSampleRate(double rate);

					// and the buffer update event - bit strange the way this works in c++
					event EventHandler^ BufferUpdate
					{
						void add(EventHandler^ e) { _bufferUpdateEvent += e; }
						void remove(EventHandler^ e) { _bufferUpdateEvent -= e; }
					}
				};
			}
		}
	}