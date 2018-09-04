//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

#include "stdio.h"
#include "BufferSize.h"
#include "Channel.h"
#include "InstalledDriver.h"
#include "AsioDriver.h"
#include "Channel.h"
#include <vcclr.h>

using namespace System::Diagnostics;

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
			array<InstalledDriver^>^ AsioDriver::InstalledDrivers::get()
			{
				// if we don't know what drivers are installed, ask the InstalledDriver class
				if (!_installedDrivers) _installedDrivers = InstalledDriver::GetInstalledDriversFromRegistry();

				// and return
				return _installedDrivers;
			}

			// static forward to instance method
			void BufferSwitch(long bufferIndex, ASIOBool directProcess)
			{
				AsioDriver::Instance->OnBufferSwitch(bufferIndex, directProcess);
			}

			// static forward to instance method
			long AsioMessage(long a, long b , void* c , double* d)
			{
				return AsioDriver::Instance->OnAsioMessage(a, b, c, d);
			}

			// static forward to instance method
			ASIOTime* BufferSwitchTimeInfo(ASIOTime* params, long doubleBufferIndex, ASIOBool directProcess)
			{
				return AsioDriver::Instance->OnBufferSwitchTimeInfo(params, doubleBufferIndex, directProcess);
			}

			// static forward to instance method
			void SampleRateDidChange(ASIOSampleRate rate)
			{
				AsioDriver::Instance->OnSampleRateDidChange(rate);
			}

			// selects a driver into our singleton instance
			AsioDriver^ AsioDriver::SelectDriver(InstalledDriver^ installedDriver)
			{
				// create a new instance of the driver (this will become the singleton)
				_instance = gcnew AsioDriver();

				// a default maximum of 32 input and output channels unless otherwise told
				_instance->InternalSelectDriver(installedDriver);

				// and return the instance
				return _instance;
			}

			void AsioDriver::InternalSelectDriver(InstalledDriver^ installedDriver)
			{
#if ENABLETRACE
				TextWriterTraceListener^ myWriter = gcnew TextWriterTraceListener( "Trace.log" );
				Trace::Listeners->Add( myWriter );
#endif

				// initialize COM lib
				CoInitialize(0);
				
				long inputs, outputs;

				// class and interface id for Asio driver
				CLSID m_clsid;

				// convert from managed string to unmanaged chaos string
				pin_ptr<const wchar_t> clsid = PtrToStringChars(installedDriver->ClsId);

				// convert string from registry to CLSID
				CLSIDFromString((LPOLESTR)clsid, &m_clsid);

				// and actually create the object and return its interface (clsid used twice)
				LPVOID pAsio = NULL;
				HRESULT rc = CoCreateInstance(m_clsid, NULL, CLSCTX_INPROC_SERVER, m_clsid, &pAsio);

				// cast the result back to our ASIO interface
				_pDriver = (IAsio*) pAsio;

				// and we're ready to use it
				if(_pDriver->init(0) == ASIOTrue)
				{
					// get the number of inputs and outputs supported by the driver
					_pDriver->getChannels(&inputs, &outputs);

					// and remember these (with a host specified ceiling)
					_nInputs = inputs;
					_nOutputs = outputs;

					// create the ASIO callback struct
					_pCallbacks = new ASIOCallbacks();

					// and convert our delegates to unmanaged typedefs
					_pCallbacks->bufferSwitch = BufferSwitch; 
					_pCallbacks->asioMessage = AsioMessage;
					_pCallbacks->bufferSwitchTimeInfo = BufferSwitchTimeInfo;
					_pCallbacks->sampleRateDidChange = SampleRateDidChange;
				}
				else
				{
					char errorMessage[1000];
					_pDriver->getErrorMessage(errorMessage);			
					throw gcnew ApplicationException(gcnew String(errorMessage));
					//throw gcnew ApplicationException("Driver did not initialise properly");
				}
			}

			// return the singleton instance
			AsioDriver^ AsioDriver::Instance::get()
			{
				return _instance;
			}

			int AsioDriver::Version::get()
			{
				// make sure a driver has been engaged

				CheckInitialised();

				// refer to driver
				return _pDriver->getDriverVersion();
			}

			String^ AsioDriver::DriverName::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				// refer to driver
				char driverName[1000];
				_pDriver->getDriverName(driverName);
				return gcnew String(driverName);
			}

			String^ AsioDriver::ErrorMessage::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				// refer to driver
				char errorMessage[1000];
				_pDriver->getErrorMessage(errorMessage);
				return gcnew String(errorMessage);
			}

			int AsioDriver::NumberInputChannels::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				// we got this earlier on
				return _nInputs;
			}

			int AsioDriver::NumberOutputChannels::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				// we got this earlier on
				return _nOutputs;
			}

			BufferSize^ AsioDriver::BufferSizex::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				return gcnew BufferSize(_pDriver);
			}

			double AsioDriver::SampleRate::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				// refer to driver
				double rate;
				_pDriver->getSampleRate(&rate);
				return rate;
			}

			void AsioDriver::SetSampleRate(double rate)
			{
				// make sure a driver has been engaged
				CheckInitialised();

				_pDriver->setSampleRate(rate);
			}

			void AsioDriver::CreateBuffers(int bufferSize)
			{
				// we need the total number of channels here
				int totalChannels = _nInputs + _nOutputs;

				// create our input and output channel arrays
				_inputChannels = gcnew array<Channel^>(_nInputs);
				_outputChannels = gcnew array<Channel^>(_nOutputs);

				// each channel needs a buffer info
				ASIOBufferInfo* pBufferInfos = new ASIOBufferInfo[totalChannels];

				// now create each input channel and set up its buffer
				for (int index = 0; index < _nInputs; index++)
				{
					pBufferInfos[index].isInput = 1;
					pBufferInfos[index].channelNum = index;
					pBufferInfos[index].buffers[0] = 0;
					pBufferInfos[index].buffers[1] = 0;
				}

				// and do the same for output channels
				for (int index = 0; index < _nOutputs; index++)
				{
					pBufferInfos[index + _nInputs].isInput = 0;
					pBufferInfos[index + _nInputs].channelNum = index;
					pBufferInfos[index + _nInputs].buffers[0] = 0;
					pBufferInfos[index + _nInputs].buffers[1] = 0;
				}

				// JBK: added checks
				if (bufferSize > BufferSizex->m_nMaxSize)
				{
					// use the drivers maximum buffer size
					bufferSize = BufferSizex->m_nMaxSize;
				}
				else if(bufferSize < BufferSizex->m_nMinSize)
				{
					// use the drivers minimum buffer size
					bufferSize = BufferSizex->m_nMinSize;
				}

				//if (useMaxBufferSize)
				//{
				//	// use the drivers maximum buffer size
				//	bufferSize = BufferSizex->m_nMaxSize;
				//}
				//else
				//{
				//	// use the drivers preferred buffer size
				//	bufferSize = BufferSizex->m_nPreferredSize;
				//}

				// get the driver to create its buffers
				_pDriver->createBuffers(pBufferInfos, totalChannels, bufferSize, _pCallbacks);

				// now go and create the managed channel objects to manipulate these buffers
				for (int index = 0; index < _nInputs; index++)
				{
					_inputChannels[index] = gcnew Channel(_pDriver, true, index,
						pBufferInfos[index].buffers[0],
						pBufferInfos[index].buffers[1],
						bufferSize);
				}

				for (int index = 0; index < _nOutputs; index++)
				{
					_outputChannels[index] = gcnew Channel(_pDriver, false, index,
						pBufferInfos[index + _nInputs].buffers[0],
						pBufferInfos[index + _nInputs].buffers[1],
						bufferSize);
				}
				_outputReadySupport = _pDriver->outputReady();
#if ENABLETRACE
				Trace::WriteLine(String::Format("outputReady(): {0}", _outputReadySupport));
#endif
			}

			void AsioDriver::DisposeBuffers()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				_pDriver->disposeBuffers();
			}

			array<Channel^>^ AsioDriver::InputChannels::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				return _inputChannels;
			}

			array<Channel^>^ AsioDriver::OutputChannels::get()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				return _outputChannels;
			}

			void AsioDriver::Start()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				_pDriver->start();
			}

			void AsioDriver::Stop()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				_pDriver->stop();
			}

			void AsioDriver::Release()
			{
				// only if a driver has been engaged
				if (_pDriver != NULL)
				{
					// release COM object
					_pDriver->Release();
					_pDriver = NULL;
				}
				
				// release COM lib
				CoUninitialize();

#if ENABLETRACE
				// flush all trace info to disk
				Trace::Flush();
#endif
			}

			void AsioDriver::ShowControlPanel()
			{
				// make sure a driver has been engaged
				CheckInitialised();

				_pDriver->controlPanel();
			}

			void AsioDriver::OnBufferSwitch(long doubleBufferIndex, ASIOBool directProcess)
			{
#if ENABLETRACE
				Trace::WriteLine(String::Format("OnBufferSwitch({0}, {1})", doubleBufferIndex, directProcess));
#endif
				// a buffer switch is occuring, first off, 
				// tell all channels what buffer needs to be read/written
				for (int index = 0; index < _nInputs; index++)
				{
					_inputChannels[index]->SetDoubleBufferIndex(doubleBufferIndex);
				}
				for (int index = 0; index < _nOutputs; index++)
				{
					_outputChannels[index]->SetDoubleBufferIndex(doubleBufferIndex);
				}

				// next we raise an event so that the caller can do their buffer manipulation
				if (_bufferUpdateEvent != nullptr)
				{
					_bufferUpdateEvent(this, gcnew EventArgs());
				}

				// notify driver we're done, see ASIO SDK for further explanation
				if (_outputReadySupport == ASE_OK)
					_pDriver->outputReady();
			}

			ASIOTime* AsioDriver::OnBufferSwitchTimeInfo(ASIOTime* params, long doubleBufferIndex, ASIOBool directProcess)
			{
				// no implementation
				return nullptr;
			}

			void AsioDriver::OnSampleRateDidChange(ASIOSampleRate rate)
			{
				// no implementation
			}

			long AsioDriver::OnAsioMessage(long selector, long value, void* message, double* opt)
			{
#if ENABLETRACE
				Trace::WriteLine(String::Format("OnAsioMessage({0})", selector));
#endif

				switch (selector)
				{
				case kAsioSelectorSupported:
					switch (value)
					{
					case kAsioEngineVersion:
						return 1;
					case kAsioResetRequest:
						return 0;
					case kAsioBufferSizeChange:
						return 0;
					case kAsioResyncRequest:
						return 0;
					case kAsioLatenciesChanged:
						return 0;
					case kAsioSupportsTimeInfo:
						return 1;
					case kAsioSupportsTimeCode:
						return 1;
					}
				case kAsioEngineVersion:
					return 2;
				case kAsioResetRequest:
					return 1;
				case kAsioBufferSizeChange:
					return 0;
				case kAsioResyncRequest:
					return 0;
				case kAsioLatenciesChanged:
					return 0;
				case kAsioSupportsTimeInfo:
					return 0;
				case kAsioSupportsTimeCode:
					return 0;
				}
				return 0;
			}

			void AsioDriver::CheckInitialised()
			{
				if (_pDriver == NULL)
				{
					throw gcnew ApplicationException("Select driver first");
				}
			}
		}
	}
}