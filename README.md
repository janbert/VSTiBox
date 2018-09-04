# VSTiBox
VSTiBox is a dedicated VST host synth for live performance. It is a Windows based machine that contains all components to run VST software synths that are controlled by an external midi keyboard. Key components are:
- Micro-ATX with Intel i5 running Windows 7
- Custom wooden/acrylic lasercut case
- Custon VST host software written in C#
- Custom audio mixer with multiple I/O and headphone out
- Custom linear audio PSU 230Vac in, +/-10Vdc out
- Custom interface boards with 8x 24PPR encoders and 8x RGB silicon pad buttons 

See my project page at hackaday for more info: https://hackaday.io/project/160858-vstibox

# Build instructions
For all hardware v1.0 is added including the GERBER files that I used for ordering the pcb's. Both the mixer and the control interface contain bugs and need some rewiring. I have corrected the schematics in v1.1, but did not take the time to finish the pcb's. I do recommend finalizing the v1.1 boards if you want to build your own, instead of re-ordering my v1.0 versions. 

# Libraries
This project uses a number of libraries. 
- BlueWave.Interop.Asio	https://www.codeproject.com/Articles/24536/Low-Latency-Audio-using-ASIO-Drivers-in-NET?msg=4526889
- VST.NET               https://github.com/obiwanjacobi/vst.net
- midi-dot-net	        https://code.google.com/archive/p/midi-dot-net/
- NAudio	              https://github.com/naudio/NAudio
- OpenHardwareMonitor	  https://openhardwaremonitor.org
- PDFViewer	            https://www.codeproject.com/Articles/37458/PDF-Viewer-Control-Without-Acrobat-Reader-Installe

# License
This project is distributed under the the GNU general public license v3.


