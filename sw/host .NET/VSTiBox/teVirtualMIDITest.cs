/* teVirtualMIDI C# interface
 *
 * Copyright 2009-2019, Tobias Erichsen
 * All rights reserved, unauthorized usage & distribution is prohibited.
 *
 *
 * File: TeVirtualMIDITest.cs
 *
 * This file contains a sample using the TeVirtualMIDI-class, which
 * implements a simple loopback-MIDI-port.
 */

using System;
using System.Threading;


namespace TobiasErichsen.teVirtualMIDI.test {

	class Program {

		public static TeVirtualMIDI port;

		public static void WorkThreadFunction() {

			byte[] command;
            
			try {
				while ( true ) {

					command = port.getCommand();

					port.sendCommand( command );

					Console.WriteLine( "command: " + byteArrayToString( command ) );

				}
			} catch ( Exception ex ) {

				Console.WriteLine( "thread aborting: " + ex.Message );

			}

		}

		public static string byteArrayToString( byte[] ba ) {

			string hex = BitConverter.ToString( ba );

			return hex.Replace( "-" , ":" );

		}

		static void Main( string[] args ) {

			Console.WriteLine( "teVirtualMIDI C# loopback sample" );
			Console.WriteLine( "using dll-version:    " + TeVirtualMIDI.versionString );
			Console.WriteLine( "using driver-version: " + TeVirtualMIDI.driverVersionString );

			TeVirtualMIDI.logging( TeVirtualMIDI.TE_VM_LOGGING_MISC | TeVirtualMIDI.TE_VM_LOGGING_RX | TeVirtualMIDI.TE_VM_LOGGING_TX );
			
			Guid manufacturer = new Guid( "aa4e075f-3504-4aab-9b06-9a4104a91cf0" );
			Guid product = new Guid( "bb4e075f-3504-4aab-9b06-9a4104a91cf0" );

			port = new TeVirtualMIDI( "C# loopback", 65535, TeVirtualMIDI.TE_VM_FLAGS_PARSE_RX, ref manufacturer, ref product );

//			alernatively: simple instantiation without any "special" stuff:
//			port = new TeVirtualMIDI( "C# loopback" );

			Thread thread = new Thread( new ThreadStart( WorkThreadFunction ) );
			thread.Start();

			Console.WriteLine( "Virtual port created - press enter to close port again" );
			Console.ReadKey();
			port.shutdown();
			Console.WriteLine( "Virtual port closed - press enter to terminate application" );
			Console.ReadKey();

		}

	}

}
