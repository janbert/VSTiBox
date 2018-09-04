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
Purpose: Definition of base functions, classes, and defines for wrapper classes
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
#include "directmidi/CDirectMidi.h"
#include <vcclr.h>
#include <string>

using namespace System;
using namespace directmidi;
using namespace System::Runtime::InteropServices;
using namespace System::Windows::Forms;	//to get hwnd from controls
using namespace System::Collections::Generic;
using namespace System::Threading;	//to get hevent from WaitHandles

namespace DirectMidi {

//
//	Wrapper classes
//

//wrap a C++ class - no constructor or destructor is used
template<typename T>
public ref class WrapperBaseNoDestructor
{
protected:
	//
	//	base type and validity checker
	//
	typedef T TBASE;
	TBASE* base;
	void checkbase() { if (!base) { throw gcnew ArgumentNullException("Already disposed of this object"); } }

internal:
	//
	//	allow treating this class as the base type
	//
	TBASE& baseref() { checkbase(); return *base; }
	operator TBASE&() { return baseref(); }
};

//wrap a C++ class - class's destructor is used
template<typename T>
public ref class WrapperBaseNoConstructor : public WrapperBaseNoDestructor<T>, public IDisposable
{
public:
	~WrapperBaseNoConstructor() { this->!WrapperBaseNoConstructor(); }
	!WrapperBaseNoConstructor() { if (base) { delete base; base = NULL; } }
};

//wrap a C++ class - class's default constructor, and class's destructor, are used
template<typename T>
public ref class WrapperBase : public WrapperBaseNoConstructor<T>
{
public:
	WrapperBase() { base = new T; }
};

//wrap a COM object - uses AddRef (optionally) and Release
template<typename T>
public ref class ComWrapperBase : public WrapperBaseNoDestructor<T>, public IDisposable
{
public:
	ComWrapperBase() { base = NULL; }
	~ComWrapperBase() { this->!ComWrapperBase(); }
	!ComWrapperBase() { if (base) { base->Release(); base = NULL; } }

internal:
	ComWrapperBase(TBASE* ref, bool addRef) { if (!ref) { base = NULL; } else { base = ref; if (addRef) { base->AddRef(); } } }
};

//base for a C++ implementation for an interface; calls into a .NET implementation
template<typename T_IFACE, typename T_CLASS>
class InterfaceImplWrapper : public T_IFACE
{
public:
	typedef T_IFACE TIFACE;
	typedef T_CLASS TCLASS;

protected:
	gcroot<TCLASS^> m_iface;

public:
	InterfaceImplWrapper(TCLASS^ iface) { m_iface = iface; }
};

//base for a C++ implementation of a COM interface; calls into a .NET implementation
template<typename T_IFACE, typename T_CLASS>
class ComInterfaceImplWrapper : public InterfaceImplWrapper<T_IFACE, T_CLASS>
{
private:
	IID m_iid;
	LONG m_cRef;

public:
	typedef InterfaceImplWrapper<T_IFACE, T_CLASS> TBASE;
	typedef typename TBASE::TIFACE TIFACE;
	typedef typename TBASE::TCLASS TCLASS;

public:
	virtual ~ComInterfaceImplWrapper() {};
	ComInterfaceImplWrapper(const IID& iid, TCLASS^ iface) : InterfaceImplWrapper(iface), m_iid(iid)
	{
		m_cRef = 1;
	}

	STDMETHOD_(ULONG,AddRef)()
	{
	    return InterlockedIncrement(&m_cRef);
	}

	STDMETHOD_(ULONG,Release)()
	{
	    if (!InterlockedDecrement(&m_cRef))
	    {
			delete this;
			return 0;
		}

	    return m_cRef;
	}

	STDMETHOD(QueryInterface)(const IID &iid, void **ppv)
	{
		if (iid == IID_IUnknown || iid == m_iid)
		{
	        *ppv = static_cast<TIFACE*>(this);
	    } 
		else
		{
	        *ppv = NULL;
			return E_NOINTERFACE;
		}
		static_cast<IUnknown*>(this)->AddRef();
		return S_OK;
	}

};



//
//	Wrappers for types inherited from one of the WrapperBase's
//
#define INHERITED_BASE_TYPE(child, parent, cpp)				\
	private:												\
		typedef parent PARENT;								\
	protected:												\
		typedef cpp TBASE;									\
		TBASE* base;										\
	internal:												\
		operator TBASE&() { checkbase(); return *base; }	\
		TBASE& baseref() { checkbase(); return *base; }

#define INHERITED_BASE_DESTRUCTOR(child)					\
	public:													\
		~child() { this->!child(); }						\
		!child() { if (base) { delete base; PARENT::base = NULL; base = NULL; } }

#define INHERITED_BASE_COPYCONSTRUCTOR(child)				\
	internal:												\
		child(const TBASE& val) { PARENT::base = base = new TBASE(val); }

#define INHERITED_BASE_REINTERPRET_COPYCONSTRUCTOR(child)	\
	internal:												\
		child(const TBASE& val) { PARENT::base = reinterpret_cast<PARENT::TBASE*>(base = new TBASE(val)); }

#define INHERITED_BASE_REINTERPRET_PTRCONSTRUCTOR(child)	\
	internal:												\
		child(TBASE* val) { PARENT::base = reinterpret_cast<PARENT::TBASE*>(base = val); }

#define INHERITED_BASE_NULLCONSTRUCTOR(child)				\
	public:													\
		child() { PARENT::base = NULL; base = NULL; }


#define INHERITED_BASE_NOCONSTRUCTOR(child, parent, cpp)	\
		INHERITED_BASE_TYPE(child, parent, cpp);			\
		INHERITED_BASE_DESTRUCTOR(child);


//
//	Convert to/from .NET types
//
typedef std::basic_string<wchar_t> widestring;

inline String^ toString(const TCHAR* str)
{
	return gcnew String(str);
}
inline void fromString(TCHAR* to, size_t maxlen, String^ from)
{
	if ((size_t)from->Length >= maxlen) { throw gcnew ArgumentOutOfRangeException("String does not fit in buffer"); }
	pin_ptr<const wchar_t> wch = PtrToStringChars(from);
	while (*wch) { *(to++) = *(wch++); }
	*to = '\0';
}
inline widestring fromString(String^ from)
{
	pin_ptr<const wchar_t> wch = PtrToStringChars(from);
	return widestring(wch);
}	


inline System::Guid toGuid(const GUID& guid)
{
	return System::Guid(guid.Data1, guid.Data2, guid.Data3, 
						guid.Data4[ 0 ], guid.Data4[ 1 ], 
						guid.Data4[ 2 ], guid.Data4[ 3 ], 
						guid.Data4[ 4 ], guid.Data4[ 5 ], 
						guid.Data4[ 6 ], guid.Data4[ 7 ]);
}
inline GUID fromGuid(System::Guid& guid)
{
	array<Byte>^ guidData = guid.ToByteArray();
	pin_ptr<Byte> data = &(guidData[ 0 ]);
	return *(GUID*)data;
}

//
//	Property defines
//
#define countof(arr) (sizeof(arr) / sizeof(arr[0]))

#define WRAPPER_PROPERTY(type, name)	\
	property type name					\
	{									\
		type get()						\
		{								\
			checkbase();				\
			return base->name;			\
		}								\
		void set(type val)				\
		{								\
			checkbase();				\
			base->name = val;			\
		}								\
	}

#define WRAPPER_PROPERTY_DIFFCAST(net, cpp, name)	\
	property net name								\
	{												\
		net get()									\
		{											\
			checkbase();							\
			return net(base->name);					\
		}											\
		void set(net val)							\
		{											\
			checkbase();							\
			base->name = (cpp)val;					\
		}											\
	}
#define WRAPPER_PROPERTY_CAST(type, name)	WRAPPER_PROPERTY_DIFFCAST(type, ::type, name)

#define WRAPPER_PROPERTY_COM_READONLY(type, name)			\
	property type^ name										\
	{														\
		type^ get()											\
		{													\
			checkbase();									\
			return gcnew type(base->name, true);			\
		}													\
	}

#define WRAPPER_PROPERTY_COM_NOADDREF(type, name)			\
	property type^ name										\
	{														\
		type^ get()											\
		{													\
			checkbase();									\
			return gcnew type(base->name, true);			\
		}													\
		void set(type^ val)									\
		{													\
			checkbase();									\
			base->name = (val ? &val->baseref() : NULL);	\
		}													\
	}

#define WRAPPER_PROPERTY_STRING_READONLY(name)		\
	property System::String^ name					\
	{												\
		System::String^ get()						\
		{											\
			checkbase();							\
			return toString(base->name);			\
		}											\
	}

#define WRAPPER_PROPERTY_STRING(name)							\
	property System::String^ name								\
	{															\
		System::String^ get()									\
		{														\
			checkbase();										\
			return toString(base->name);						\
		}														\
		void set(System::String^ val)							\
		{														\
			checkbase();										\
			fromString(base->name, sizeof(base->name), val);	\
		}														\
	}

#define WRAPPER_PROPERTY_GUID(name)		\
	property System::Guid name			\
	{									\
		System::Guid get()				\
		{								\
			checkbase();				\
			return toGuid(base->name);	\
		}								\
		void set(System::Guid val)		\
		{								\
			checkbase();				\
			base->name = fromGuid(val);	\
		}								\
	}

#define WRAPPER_CONST(type, ns, name)	\
	static const type name = ns::name

//
//	Struct wrapper
//
#define WRAPPER_STRUCT(net, cpp)	\
	[StructLayout(LayoutKind::Explicit, Size=sizeof(cpp))]	\
	public value struct net \
	{	\
		typedef cpp TBASE;	\

#define WRAPPER_STRUCT_MEMBER(type, mem)	\
	[FieldOffset(offsetof(TBASE, mem))] \
	type mem

#define WRAPPER_STRUCT_END	\
	}


}	//namespace DirectMidi
