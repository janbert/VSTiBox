//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

#pragma once
#pragma managed
using namespace System;

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
			// represents an installed ASIO driver
			public ref class InstalledDriver
			{
			private:

				// the name of the driver
				String^ _name;

				// its COM CLSID
				String^ _clsId;

			internal:

				// internal construction only
				InstalledDriver(String^ name, String^ clsId);

				// this will read all drivers and the CLSIDs from the registry
				static array<InstalledDriver^>^ GetInstalledDriversFromRegistry();

				// this returns a string representation of the CLSID
				property String^ ClsId { String^ get(); };

			public:

				// both these just return name
				virtual String^ ToString() override;
				property String^ Name { String^ get(); };
			};
		}
	}
}