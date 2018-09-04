Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO


Public Class PrinterUtil

  Private WithEvents mPrintDocument As PrintDocument
  Private mFileName As String
  Private mStartPage As Integer
  Private mEndPage As Integer
  Private mCurrentPage As Integer
  Private mPassword As String

  Public Shared Function ListAllInstalledPrinters() As List(Of String)
    ListAllInstalledPrinters = New List(Of String)
    For Each InstalledPrinter As String In _
    PrinterSettings.InstalledPrinters
      ListAllInstalledPrinters.Add(InstalledPrinter)
    Next InstalledPrinter
  End Function

  Public Shared Function CreateCustomPrintToFilePort() As String
    Dim TempString As String = Format(Now, "yyyyMMDDhhmmssfff")
    Dim p1 As PrinterInterOp.PORT_INFO_1
    p1.pPortName = System.IO.Path.GetTempPath & "\" & TempString & ".prn"
    CreateCustomPrintToFilePort = p1.pPortName
    PrinterInterOp.AddPortEx(Nothing, 1, p1, "Local Port")
  End Function

  Public Shared Function CreateNewTempPrinter(ByVal PrinterName As String, ByVal PortName As String, ByVal PrintDriverName As String) As Integer
    Dim PrintInterop As New PrinterInterOp
    CreateNewTempPrinter = PrintInterop.AddPrinter(PrinterName, PortName, PrintDriverName, "WinPrint")
  End Function

  Public Shared Sub SendFileToPrinter(ByVal PrinterName As String, ByVal FileName As String)
    Dim RP As New RawPrinterHelper
    RP.SendFileToPrinter(PrinterName, FileName)
  End Sub

  Public Shared Sub PrintImageToPrinter(ByRef myFileName As String, ByVal myPrinterSettings As PrinterSettings, Optional ByVal password As String = "")
    Dim PrintUtil As New PrinterUtil
    PrintUtil.mPrintDocument = New PrintDocument
    PrintUtil.mFileName = myFileName
    PrintUtil.mPrintDocument.PrinterSettings.PrinterName = myPrinterSettings.PrinterName
    Dim StandardPrint As New StandardPrintController
    PrintUtil.mPrintDocument.PrintController = StandardPrint
    PrintUtil.mPrintDocument.PrinterSettings = myPrinterSettings
    PrintUtil.mStartPage = myPrinterSettings.FromPage
    PrintUtil.mEndPage = myPrinterSettings.ToPage
    PrintUtil.mCurrentPage = PrintUtil.mStartPage
    PrintUtil.mPassword = password
    Cursor.Current = Cursors.WaitCursor
    PrintUtil.mPrintDocument.Print()
    PrintUtil = Nothing
    GC.Collect()
    Cursor.Current = Cursors.Default
  End Sub

  Public Shared Sub PrintPDFDirectlyToPrinter(ByVal FileName As String)
    Dim PD As New PrintDialog
    If PD.ShowDialog = DialogResult.OK Then
      Dim RP As New RawPrinterHelper
      RP.SendFileToPrinter(PD.PrinterSettings.PrinterName, FileName)
    End If
  End Sub

  Public Shared Sub PrintImagesToPrinter(ByVal FileName As String, Optional ByVal password As String = "")
    Dim PD As New PrintDialog
    Dim PageCount As Integer = PDFView.ImageUtil.GetImageFrameCount(FileName)
    PD.AllowPrintToFile = True
    PD.AllowSomePages = True
    PD.PrinterSettings.FromPage = 1
    PD.PrinterSettings.ToPage = PageCount
    PD.PrinterSettings.MaximumPage = PageCount
    PD.PrinterSettings.MinimumPage = 1
    If PD.ShowDialog = DialogResult.OK Then
      Dim BeginningPage As Integer = 1
      Dim EndingPage As Integer = PageCount
      If PD.PrinterSettings.PrintRange = PrintRange.SomePages Then
        BeginningPage = PD.PrinterSettings.FromPage
        EndingPage = PD.PrinterSettings.ToPage
      End If
      PrintImageToPrinter(FileName, PD.PrinterSettings, password)
    End If
  End Sub

  Private Sub PrintDocument1_PrintPage(ByVal sender As Object, _
     ByVal e As PrintPageEventArgs) Handles mPrintDocument.PrintPage

    Dim RenderDPI As Integer = 300 'Set to 600 if this resolution is too low (speed vs. time)
    Dim image As System.Drawing.Image = ImageUtil.GetImagePageFromFileForPrint(mFileName, mCurrentPage, RenderDPI, mPassword)

    'Comment out if you do not want Auto-Rotate
    If (image.Height > image.Width And e.Graphics.VisibleClipBounds.Width > e.Graphics.VisibleClipBounds.Height) _
    Or (image.Width > image.Height And e.Graphics.VisibleClipBounds.Height > e.Graphics.VisibleClipBounds.Width) Then
      image.RotateFlip(RotateFlipType.Rotate270FlipNone)
    End If

    Dim ScalePercentage As Single
    Dim XMaxPixels As Integer = (e.Graphics.VisibleClipBounds.Width / 100) * image.HorizontalResolution
    Dim YMaxPixels As Integer = (e.Graphics.VisibleClipBounds.Height / 100) * image.VerticalResolution
    Dim XFactor As Single = XMaxPixels / image.Width
    Dim YFactor As Single = YMaxPixels / image.Height
    Dim OptimalDPI As Integer

    If YFactor > XFactor Then
      ScalePercentage = XFactor
      OptimalDPI = RenderDPI * XFactor
    Else
      ScalePercentage = YFactor
      OptimalDPI = RenderDPI * YFactor
    End If

    If ScalePercentage < 0.75F Then 'Re-render the image to create a smaller print file and save printer processing time
      image = ImageUtil.GetImagePageFromFileForPrint(mFileName, mCurrentPage, OptimalDPI, mPassword)
      If (image.Height > image.Width And e.Graphics.VisibleClipBounds.Width > e.Graphics.VisibleClipBounds.Height) _
      Or (image.Width > image.Height And e.Graphics.VisibleClipBounds.Height > e.Graphics.VisibleClipBounds.Width) Then
        image.RotateFlip(RotateFlipType.Rotate270FlipNone)
      End If
    End If

    e.Graphics.ScaleTransform(ScalePercentage, ScalePercentage)
    e.Graphics.DrawImage(image, 0, 0)

    If mCurrentPage >= mEndPage Then
      e.HasMorePages = False
    Else
      e.HasMorePages = True
    End If

    image.Dispose()
    mCurrentPage += 1

  End Sub

End Class

Friend Class PrinterInterOp

  Public Structure PORT_INFO_1
    Dim pPortName As String
  End Structure

  Structure PrinterDefaults
    Public pDataType As String
    Public pDevMode As Int32
    Public permissions As Int32
  End Structure

  Public Function AddPrinter(ByVal psPrinterName As String, ByVal psPortName As String, _
                             ByVal psDriverName As String, _
                             ByVal psPrintProcessor As String) As IntPtr
    Dim pi2 As New PRINTER_INFO_2
    Dim iError As Integer
    With pi2
      .pPrinterName = psPrinterName
      .pPortName = psPortName
      .pDriverName = psDriverName
      .pPrintProcessor = psPrintProcessor
    End With
    Dim rtn As Int32
    rtn = AddPrinter("", 2, pi2)
    If rtn = 0 Then
      iError = Marshal.GetLastWin32Error()
      Select Case iError
        Case 1796
          MsgBox("The specified port is unknown")
          Exit Select
        Case 1797
          MsgBox("Printer driver is unknown or not loaded")
          Exit Select
        Case 1798
          MsgBox("The print processor is unknown")
          Exit Select
        Case 1801
          MsgBox("Printer name is invalid")
          Exit Select
        Case 1802
          MsgBox("Printer name already exists")
          Exit Select
        Case Else
          MsgBox("An error occured. Errorcode: " & iError)
          Exit Select
      End Select
    Else
      MsgBox("Printer was created successfully")
    End If
    ClosePrinter(rtn)
    Return rtn
  End Function

  Public Sub RemovePrinter(ByVal PrinterName As String)
    Dim hPrinter As IntPtr
    Dim PrinterDefs As New PrinterDefaults
    With PrinterDefs
      .pDevMode = 0&
      .permissions = PRINTER_ALL_ACCESS
    End With
    If OpenPrinter(PrinterName, hPrinter, PrinterDefs) <> 0 Then

      If hPrinter <> 0 Then
        If DeletePrinter(hPrinter) <> 0 Then
          MsgBox("Printer deleted")
        Else
          MsgBox("Error :" & Err.LastDllError)
        End If  'DeletePrinter
      End If  'hPrinter
      ClosePrinter(hPrinter)
    Else
      MsgBox("Error :" & Err.LastDllError)
    End If  'OpenPrinter

  End Sub

  Public Declare Function AddPortEx Lib "winspool.drv" Alias "AddPortExA" _
(ByVal pName As String, ByVal pLevel As Integer, _
ByRef lpBuffer As PORT_INFO_1, ByVal pMonitorName As String) As Integer

  Public Declare Function DeletePort Lib "winspool.drv" _
(ByVal pName As String, ByVal pLevel As Integer, _
 ByVal pMonitorName As String) As Integer

  Public Declare Auto Function OpenPrinter Lib "winspool.drv" _
                          (ByVal pPrinterName As String, _
                          ByRef phPrinter As IntPtr, _
                          ByVal pDefault As PrinterDefaults) As Int32

  Public Declare Auto Function ResetPrinter Lib "winspool.drv" _
                          (ByRef phPrinter As IntPtr, _
                          ByVal pDefault As PrinterDefaults) As Int32

  Public Declare Function DeletePrinter Lib "winspool.drv" _
                         (ByVal hPrinter As IntPtr) As Int32

  Public Declare Function ClosePrinter Lib "winspool.drv" _
                (ByVal hPrinter As IntPtr) As Int32

  Private Declare Function AddPrinter Lib "winspool.drv" _
     Alias "AddPrinterA" _
    (ByVal pServerName As String, _
     ByVal Level As Int32, _
  ByVal pPrinter As PRINTER_INFO_2) As Int32

  Public Declare Function GetPrinter Lib "winspool.drv" Alias "GetPrinterW" _
  (ByVal hPrinter As IntPtr, ByVal Level As Integer, ByVal pPrinter As IntPtr, _
   ByVal cbBuf As Integer, ByRef pcbNeeded As Integer) As Integer

  Public Const PRINTER_ACCESS_ADMINISTER As Int32 = &H4
  Public Const STANDARD_RIGHTS_REQUIRED As Int32 = &HF0000
  Public Const PRINTER_ACCESS_USE As Int32 = &H8
  Public Const PRINTER_ALL_ACCESS As Int32 = _
                    STANDARD_RIGHTS_REQUIRED Or _
                    PRINTER_ACCESS_USE Or _
                    PRINTER_ACCESS_ADMINISTER

  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto), System.Security.SuppressUnmanagedCodeSecurity()> _
 Friend Class PRINTER_INFO_2
    <MarshalAs(UnmanagedType.LPStr)> Public pServerName As String
    <MarshalAs(UnmanagedType.LPStr)> Public pPrinterName As String
    <MarshalAs(UnmanagedType.LPStr)> Public pShareName As String
    <MarshalAs(UnmanagedType.LPStr)> Public pPortName As String
    <MarshalAs(UnmanagedType.LPStr)> Public pDriverName As String
    <MarshalAs(UnmanagedType.LPStr)> Public pComment As String
    <MarshalAs(UnmanagedType.LPStr)> Public pLocation As String
    <MarshalAs(UnmanagedType.U4)> Public pDevMode As Int32
    <MarshalAs(UnmanagedType.LPStr)> Public pSepFile As String
    <MarshalAs(UnmanagedType.LPStr)> Public pPrintProcessor As String
    <MarshalAs(UnmanagedType.LPStr)> Public pDatatype As String
    <MarshalAs(UnmanagedType.LPStr)> Public pParameters As String
    <MarshalAs(UnmanagedType.U4)> Public pSecurityDescriptor As Int32
    Public Attributes As UInteger
    Public Priority As UInteger
    Public DefaultPriority As UInteger
    Public StartTime As UInteger
    Public UntilTime As UInteger
    Public Status As UInteger
    Public cJobs As UInteger
    Public AveragePPM As UInteger

    Public Sub New(ByVal hPrinter As IntPtr)
      Dim BytesWritten As Int32 = 0
      Dim ptBuf As IntPtr
      ptBuf = Marshal.AllocHGlobal(1)
      Dim PI As New PrinterInterOp
      If Not GetPrinter(hPrinter, 2, ptBuf, 1, BytesWritten) Then
        If BytesWritten > 0 Then
          '\\ Free the buffer allocated
          Marshal.FreeHGlobal(ptBuf)
          ptBuf = Marshal.AllocHGlobal(BytesWritten)
          If GetPrinter(hPrinter, 2, ptBuf, BytesWritten, BytesWritten) Then
            Marshal.PtrToStructure(ptBuf, Me)
            '\\ Fill any missing members
            If pServerName Is Nothing Then
              pServerName = ""
            End If
          Else
            Throw New Win32Exception()
          End If
          '\\ Free this buffer again
          Marshal.FreeHGlobal(ptBuf)
        Else
          Throw New Win32Exception()
        End If
      Else
        Throw New Win32Exception()
      End If
    End Sub

    Public Sub New()
    End Sub

  End Class
End Class

Friend Class RawPrinterHelper
  ' Structure and API declarions:
  <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
  Public Class DOCINFOA
    <MarshalAs(UnmanagedType.LPStr)> _
    Public pDocName As String
    <MarshalAs(UnmanagedType.LPStr)> _
    Public pOutputFile As String
    <MarshalAs(UnmanagedType.LPStr)> _
    Public pDataType As String
  End Class
  <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function OpenPrinter(<MarshalAs(UnmanagedType.LPStr)> ByVal szPrinter As String, ByRef hPrinter As IntPtr, ByVal pd As IntPtr) As Boolean
  End Function

  <DllImport("winspool.Drv", EntryPoint:="ClosePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function ClosePrinter(ByVal hPrinter As IntPtr) As Boolean
  End Function

  <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function StartDocPrinter(ByVal hPrinter As IntPtr, ByVal level As Int32, <[In](), MarshalAs(UnmanagedType.LPStruct)> ByVal di As DOCINFOA) As Boolean
  End Function

  <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function EndDocPrinter(ByVal hPrinter As IntPtr) As Boolean
  End Function

  <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function StartPagePrinter(ByVal hPrinter As IntPtr) As Boolean
  End Function

  <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function EndPagePrinter(ByVal hPrinter As IntPtr) As Boolean
  End Function

  <DllImport("winspool.Drv", EntryPoint:="WritePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
  Public Shared Function WritePrinter(ByVal hPrinter As IntPtr, ByVal pBytes As IntPtr, ByVal dwCount As Int32, ByRef dwWritten As Int32) As Boolean
  End Function

  ' SendBytesToPrinter()
  ' When the function is given a printer name and an unmanaged array
  ' of bytes, the function sends those bytes to the print queue.
  ' Returns true on success, false on failure.
  Public Function SendBytesToPrinter(ByVal szPrinterName As String, ByVal pBytes As IntPtr, ByVal dwCount As Int32) As Boolean
    Dim dwError As Int32 = 0, dwWritten As Int32 = 0
    Dim hPrinter As New IntPtr(0)
    Dim di As New DOCINFOA()
    Dim bSuccess As Boolean = False
    ' Assume failure unless you specifically succeed.
    di.pDocName = "My VB.NET RAW Document"
    di.pDataType = "RAW"

    ' Open the printer.
    If OpenPrinter(szPrinterName.Normalize(), hPrinter, IntPtr.Zero) Then
      ' Start a document.
      If StartDocPrinter(hPrinter, 1, di) Then
        ' Start a page.
        If StartPagePrinter(hPrinter) Then
          ' Write your bytes.
          bSuccess = WritePrinter(hPrinter, pBytes, dwCount, dwWritten)
          EndPagePrinter(hPrinter)
        End If
        EndDocPrinter(hPrinter)
      End If
      ClosePrinter(hPrinter)
    End If
    ' If you did not succeed, GetLastError may give more information
    ' about why not.
    If bSuccess = False Then
      dwError = Marshal.GetLastWin32Error()
    End If
    Return bSuccess
  End Function

  Public Function SendFileToPrinter(ByVal szPrinterName As String, ByVal szFileName As String) As Boolean
    ' Open the file.
    Dim fs As New FileStream(szFileName, FileMode.Open)
    ' Create a BinaryReader on the file.
    Dim br As New BinaryReader(fs)
    ' Dim an array of bytes big enough to hold the file's contents.
    Dim bytes As [Byte]() = New [Byte](fs.Length - 1) {}
    Dim bSuccess As Boolean = False
    ' Your unmanaged pointer.
    Dim pUnmanagedBytes As New IntPtr(0)
    Dim nLength As Integer

    nLength = Convert.ToInt32(fs.Length)
    ' Read the contents of the file into the array.
    bytes = br.ReadBytes(nLength)
    ' Allocate some unmanaged memory for those bytes.
    pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength)
    ' Copy the managed byte array into the unmanaged array.
    Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength)
    ' Send the unmanaged bytes to the printer.
    bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength)
    ' Free the unmanaged memory that you allocated earlier.
    Marshal.FreeCoTaskMem(pUnmanagedBytes)
    fs.Close()
    br.Close()
    Return bSuccess
  End Function
  Public Function SendStringToPrinter(ByVal szPrinterName As String, ByVal szString As String) As Boolean
    Dim pBytes As IntPtr
    Dim dwCount As Int32
    ' How many characters are in the string?
    dwCount = szString.Length
    ' Assume that the printer is expecting ANSI text, and then convert
    ' the string to ANSI text.
    pBytes = Marshal.StringToCoTaskMemAnsi(szString)
    ' Send the converted ANSI string to the printer.
    SendBytesToPrinter(szPrinterName, pBytes, dwCount)
    Marshal.FreeCoTaskMem(pBytes)
    Return True
  End Function
End Class


