/* teVirtualMIDI C# interface - v1.3.0.43
 *
 * Copyright 2009-2019, Tobias Erichsen
 * All rights reserved, unauthorized usage & distribution is prohibited.
 *
 * For technical or commercial requests contact: info <at> tobias-erichsen <dot> de
 *
 * teVirtualMIDI.sys is a kernel-mode device-driver which can be used to dynamically create & destroy
 * midiports on Windows (XP to Windows 10, 32bit & 64bit).  The "back-end" of teVirtualMIDI can be used
 * to create & destroy such ports and receive and transmit data from/to those created ports.
 *
 * File: TeVirtualMIDI.cs
 *
 * This file implements the C#-class-wrapper for the teVirtualMIDI-driver.
 * This class encapsualtes the native C-type interface which is integrated
 * in the teVirtualMIDI32.dll and the teVirtualMIDI64.dll.
 */

using System;
using System.Runtime.InteropServices;


namespace TobiasErichsen.teVirtualMIDI
{

    [Serializable()]
    public class TeVirtualMIDIException : System.Exception
    {

        /* defines of specific WIN32-error-codes that the native teVirtualMIDI-driver
		 * is using to communicate specific problems to the application */
        private const int ERROR_PATH_NOT_FOUND = 3;
        private const int ERROR_INVALID_HANDLE = 6;
        private const int ERROR_TOO_MANY_CMDS = 56;
        private const int ERROR_TOO_MANY_SESS = 69;
        private const int ERROR_INVALID_NAME = 123;
        private const int ERROR_MOD_NOT_FOUND = 126;
        private const int ERROR_BAD_ARGUMENTS = 160;
        private const int ERROR_ALREADY_EXISTS = 183;
        private const int ERROR_OLD_WIN_VERSION = 1150;
        private const int ERROR_REVISION_MISMATCH = 1306;
        private const int ERROR_ALIAS_EXISTS = 1379;

        public TeVirtualMIDIException() : base()
        {
        }

        public TeVirtualMIDIException(string message) : base(message)
        {
        }

        public TeVirtualMIDIException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected TeVirtualMIDIException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
        }

        public int reasonCode
        {

            get
            {

                return this.fReasonCode;
            }

            set
            {

                this.fReasonCode = value;

            }

        }

        private int fReasonCode;

        private static string reasonCodeToString(int reasonCode)
        {

            switch (reasonCode)
            {

                case ERROR_OLD_WIN_VERSION:
                    return "Your Windows-version is too old for dynamic MIDI-port creation.";

                case ERROR_INVALID_NAME:
                    return "You need to specify at least 1 character as MIDI-portname!";

                case ERROR_ALREADY_EXISTS:
                    return "The name for the MIDI-port you specified is already in use!";

                case ERROR_ALIAS_EXISTS:
                    return "The name for the MIDI-port you specified is already in use!";

                case ERROR_PATH_NOT_FOUND:
                    return "Possibly the teVirtualMIDI-driver has not been installed!";

                case ERROR_MOD_NOT_FOUND:
                    return "The teVirtualMIDIxx.dll could not be loaded!";

                case ERROR_REVISION_MISMATCH:
                    return "The teVirtualMIDIxx.dll and teVirtualMIDI.sys driver differ in version!";

                case ERROR_TOO_MANY_SESS:
                    return "Maximum number of ports reached";

                case ERROR_INVALID_HANDLE:
                    return "Port not enabled";

                case ERROR_TOO_MANY_CMDS:
                    return "MIDI-command too large";

                case ERROR_BAD_ARGUMENTS:
                    return "Invalid flags specified";

                default:
                    return "Unspecified virtualMIDI-error: " + reasonCode;
            }
        }


        public static void ThrowExceptionForReasonCode(int reasonCode)
        {

            TeVirtualMIDIException exception = new TeVirtualMIDIException(reasonCodeToString(reasonCode));

            exception.reasonCode = reasonCode;

            throw exception;

        }

    }






    public class TeVirtualMIDI
    {

        /* default size of sysex-buffer */
        private const UInt32 TE_VM_DEFAULT_SYSEX_SIZE = 65535;

        /* constant for loading of teVirtualMIDI-interface-DLL, either 32 or 64 bit */
        private const string DllName = "teVirtualMIDI.dll";
        /* private const string DllName = "teVirtualMIDI32.dll"; */
        /* private const string DllName = "teVirtualMIDI64.dll"; */

        /* TE_VM_LOGGING_MISC - log internal stuff (port enable, disable...) */
        public const UInt32 TE_VM_LOGGING_MISC = 1;
        /* TE_VM_LOGGING_RX - log data received from the driver */
        public const UInt32 TE_VM_LOGGING_RX = 2;
        /* TE_VM_LOGGING_TX - log data sent to the driver */
        public const UInt32 TE_VM_LOGGING_TX = 4;

        /* TE_VM_FLAGS_PARSE_RX - parse incoming data into single, valid MIDI-commands */
        public const UInt32 TE_VM_FLAGS_PARSE_RX = 1;
        /* TE_VM_FLAGS_PARSE_TX - parse outgoing data into single, valid MIDI-commands */
        public const UInt32 TE_VM_FLAGS_PARSE_TX = 2;
        /* TE_VM_FLAGS_INSTANTIATE_RX_ONLY - Only the "midi-out" part of the port is created */
        public const UInt32 TE_VM_FLAGS_INSTANTIATE_RX_ONLY = 4;
        /* TE_VM_FLAGS_INSTANTIATE_TX_ONLY - Only the "midi-in" part of the port is created */
        public const UInt32 TE_VM_FLAGS_INSTANTIATE_TX_ONLY = 8;
        /* TE_VM_FLAGS_INSTANTIATE_BOTH - a bidirectional port is created */
        public const UInt32 TE_VM_FLAGS_INSTANTIATE_BOTH = 12;


        /* static initializer to retrieve version-info from DLL... */
        static TeVirtualMIDI()
        {

            fVersionString = Marshal.PtrToStringAuto(virtualMIDIGetVersion(ref fVersionMajor, ref fVersionMinor, ref fVersionRelease, ref fVersionBuild));
            fDriverVersionString = Marshal.PtrToStringAuto(virtualMIDIGetDriverVersion(ref fDriverVersionMajor, ref fDriverVersionMinor, ref fDriverVersionRelease, ref fDriverVersionBuild));

        }


        public TeVirtualMIDI(string portName, UInt32 maxSysexLength = TE_VM_DEFAULT_SYSEX_SIZE, UInt32 flags = TE_VM_FLAGS_PARSE_RX)
        {

            fInstance = virtualMIDICreatePortEx2(portName, IntPtr.Zero, IntPtr.Zero, maxSysexLength, flags);

            if (fInstance == IntPtr.Zero)
            {

                int lastError = Marshal.GetLastWin32Error();

                TeVirtualMIDIException.ThrowExceptionForReasonCode(lastError);

            }

            fReadBuffer = new byte[maxSysexLength];
            fReadProcessIds = new UInt64[17];
            fMaxSysexLength = maxSysexLength;

        }

        public TeVirtualMIDI(string portName, UInt32 maxSysexLength, UInt32 flags, ref Guid manufacturer, ref Guid product)
        {

            fInstance = virtualMIDICreatePortEx3(portName, IntPtr.Zero, IntPtr.Zero, maxSysexLength, flags, ref manufacturer, ref product);

            if (fInstance == IntPtr.Zero)
            {

                int lastError = Marshal.GetLastWin32Error();

                TeVirtualMIDIException.ThrowExceptionForReasonCode(lastError);

            }

            fReadBuffer = new byte[maxSysexLength];
            fReadProcessIds = new UInt64[17];
            fMaxSysexLength = maxSysexLength;

        }


        ~TeVirtualMIDI()
        {

            if (fInstance != IntPtr.Zero)
            {

                virtualMIDIClosePort(fInstance);

                fInstance = IntPtr.Zero;

            }
        }


        public static int versionMajor
        {

            get
            {

                return fVersionMajor;

            }

        }


        public static int versionMinor
        {

            get
            {

                return fVersionMinor;

            }

        }


        public static int versionRelease
        {

            get
            {

                return fVersionRelease;

            }

        }


        public static int versionBuild
        {

            get
            {

                return fVersionBuild;

            }

        }


        public static String versionString
        {

            get
            {

                return fVersionString;

            }

        }

        public static int driverVersionMajor
        {

            get
            {

                return fDriverVersionMajor;

            }

        }


        public static int driverVersionMinor
        {

            get
            {

                return fDriverVersionMinor;

            }

        }


        public static int driverVersionRelease
        {
            get
            {
                return fDriverVersionRelease;
            }
        }


        public static int driverVersionBuild
        {
            get
            {
                return fDriverVersionBuild;
            }
        }


        public static String driverVersionString
        {
            get
            {
                return fDriverVersionString;
            }
        }


        public static UInt32 logging(UInt32 loggingMask)
        {
            return virtualMIDILogging(loggingMask);
        }


        public void shutdown()
        {
            if (!virtualMIDIShutdown(fInstance))
            {
                int lastError = Marshal.GetLastWin32Error();
                TeVirtualMIDIException.ThrowExceptionForReasonCode(lastError);
            }

        }

        public void sendCommand(byte[] command)
        {
            if ((command == null) || (command.Length == 0))
            {
                return;
            }

            if (!virtualMIDISendData(fInstance, command, (UInt32)command.Length))
            {
                int lastError = Marshal.GetLastWin32Error();
                TeVirtualMIDIException.ThrowExceptionForReasonCode(lastError);
            }
        }

        public byte[] GetCommand()
        {
            UInt32 length = fMaxSysexLength;

            if (!virtualMIDIGetData(fInstance, fReadBuffer, ref length))
            {

                int lastError = Marshal.GetLastWin32Error();
                TeVirtualMIDIException.ThrowExceptionForReasonCode(lastError);

            }

            byte[] outBytes = new byte[length];
            Array.Copy(fReadBuffer, outBytes, length);
            return outBytes;
        }

        public UInt64[] getProcessIds()
        {
            UInt32 length = 17 * sizeof(ulong);
            UInt32 count;

            if (!virtualMIDIGetProcesses(fInstance, fReadProcessIds, ref length))
            {

                int lastError = Marshal.GetLastWin32Error();
                TeVirtualMIDIException.ThrowExceptionForReasonCode(lastError);

            }

            count = length / sizeof(ulong);
            UInt64[] outIds = new UInt64[count];
            Array.Copy(fReadProcessIds, outIds, count);
            return outIds;
        }


        private byte[] fReadBuffer;
        private IntPtr fInstance;
        private UInt32 fMaxSysexLength;
        private UInt64[] fReadProcessIds;
        private static ushort fVersionMajor;
        private static ushort fVersionMinor;
        private static ushort fVersionRelease;
        private static ushort fVersionBuild;
        private static String fVersionString;

        private static ushort fDriverVersionMajor;
        private static ushort fDriverVersionMinor;
        private static ushort fDriverVersionRelease;
        private static ushort fDriverVersionBuild;
        private static String fDriverVersionString;


        [DllImport(DllName, EntryPoint = "virtualMIDICreatePortEx3", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr virtualMIDICreatePortEx3(string portName, IntPtr callback, IntPtr dwCallbackInstance, UInt32 maxSysexLength, UInt32 flags, ref Guid manufacturer, ref Guid product);

        [DllImport(DllName, EntryPoint = "virtualMIDICreatePortEx2", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr virtualMIDICreatePortEx2(string portName, IntPtr callback, IntPtr dwCallbackInstance, UInt32 maxSysexLength, UInt32 flags);

        [DllImport(DllName, EntryPoint = "virtualMIDIClosePort", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void virtualMIDIClosePort(IntPtr instance);

        [DllImport(DllName, EntryPoint = "virtualMIDIShutdown", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern Boolean virtualMIDIShutdown(IntPtr instance);

        [DllImport(DllName, EntryPoint = "virtualMIDISendData", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern Boolean virtualMIDISendData(IntPtr midiPort, byte[] midiDataBytes, UInt32 length);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetData", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern Boolean virtualMIDIGetData(IntPtr midiPort, [Out] byte[] midiDataBytes, ref UInt32 length);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetProcesses", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern Boolean virtualMIDIGetProcesses(IntPtr midiPort, [Out] UInt64[] processIds, ref UInt32 length);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetVersion", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr virtualMIDIGetVersion(ref ushort major, ref ushort minor, ref ushort release, ref ushort build);

        [DllImport(DllName, EntryPoint = "virtualMIDIGetDriverVersion", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr virtualMIDIGetDriverVersion(ref ushort major, ref ushort minor, ref ushort release, ref ushort build);

        [DllImport(DllName, EntryPoint = "virtualMIDILogging", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern UInt32 virtualMIDILogging(UInt32 loggingMask);
    }
}
