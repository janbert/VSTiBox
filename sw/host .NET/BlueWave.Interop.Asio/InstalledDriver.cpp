//
// BlueWave.Interop.Asio by Rob Philpott. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk.  This file and the code contained within is freeware and may be
// distributed and edited without restriction. You may be bound by licencing restrictions
// imposed by Steinberg - check with them prior to distributing anything.
// 

#include "AsioRedirect.h"
#include "InstalledDriver.h"

// we need this for registry access
using namespace Microsoft::Win32;

// and we need this for typed lists
using namespace System::Collections::Generic;

namespace BlueWave
{
	namespace Interop
	{
		namespace Asio
		{
			InstalledDriver::InstalledDriver(String^ name, String^ clsId)
			{
				// remember the name and CLSID
				_name = name;
				_clsId = clsId;
			}

			array<InstalledDriver^>^ InstalledDriver::GetInstalledDriversFromRegistry()
			{
				// our settings are in the local machine
				RegistryKey^ localMachine = Registry::LocalMachine;

				// in the software/asio folder
				RegistryKey^ asioRoot = localMachine->OpenSubKey("SOFTWARE\\ASIO");

				// now read all the names of subkeys below that
				array<String^>^ subkeyNames = asioRoot->GetSubKeyNames();

				// create a generic list of installed drivers
				List<InstalledDriver^> list = gcnew List<InstalledDriver^>();

				// iterate through and get the stuff we need
				for (int index = 0; index < subkeyNames->Length; index++)
				{
					// get the registry key detailing the driver
					RegistryKey^ driverKey = asioRoot->OpenSubKey(subkeyNames[index]);

					// and extract what we need
					String^ name = static_cast<String^>(driverKey->GetValue("Description"));
					String^ clsid = static_cast<String^>(driverKey->GetValue("CLSID"));

					// ST: If the description value is not present, use the subkeyname
					if (!name)
						name = subkeyNames[index];

					// and close again
					driverKey->Close();

					// add to our list
					list.Add(gcnew InstalledDriver(name, clsid));
				}

				// and return as an array
				return list.ToArray();
			}

			String^ InstalledDriver::ClsId::get()
			{
				return _clsId;
			}

			String^ InstalledDriver::Name::get()
			{
				return _name;
			}

			String^ InstalledDriver::ToString()
			{
				return _name;
			}
		}
	}
}