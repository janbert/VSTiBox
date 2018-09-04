Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Collections
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms

Namespace ConvertPDF
  ''' <summary>
  ''' Create by : TaGoH
  ''' URL of the last version: http://www.codeproject.com/KB/cs/GhostScriptUseWithCSharp.aspx
  ''' Description:
  ''' Class to convert a pdf to an image using GhostScript DLL
  ''' A big Credit for this code go to:Rangel Avulso
  ''' I mainly create a better interface and refactor it to made it ready to use!
  ''' </summary>
  Public Class PDFConvert
#Region "Static"
    ''' <summary>Use to check for default transformation</summary>
    Private Shared useSimpleAnsiConversion As Boolean = True
    ''' <summary>Thanks to 	tchu_2000 to remind that u should never hardcode strings! :)</summary>
    Private Const GS_OutputFileFormat As String = "-sOutputFile={0}"
    Private Const GS_DeviceFormat As String = "-sDEVICE={0}"
    Private Const GS_FirstParameter As String = "pdf2img"
    Private Const GS_ResolutionXFormat As String = "-r{0}"
    Private Const GS_ResolutionXYFormat As String = "-r{0}x{1}"
    Private Const GS_GraphicsAlphaBits As String = "-dGraphicsAlphaBits={0}"
    Private Const GS_TextAlphaBits As String = "-dTextAlphaBits={0}"
    Private Const GS_FirstPageFormat As String = "-dFirstPage={0}"
    Private Const GS_LastPageFormat As String = "-dLastPage={0}"
    Private Const GS_FitPage As String = "-dPDFFitPage"
    Private Const GS_PageSizeFormat As String = "-g{0}x{1}"
    Private Const GS_DefaultPaperSize As String = "-sPAPERSIZE={0}"
    Private Const GS_JpegQualityFormat As String = "-dJPEGQ={0}"
    Private Const GS_RenderingThreads As String = "-dNumRenderingThreads={0}"
    Private Const GS_Fixed1stParameter As String = "-dNOPAUSE"
    Private Const GS_Fixed2ndParameter As String = "-dBATCH"
    Private Const GS_Fixed3rdParameter As String = "-dSAFER"
    Private Const GS_FixedMedia As String = "-dFIXEDMEDIA"
    Private Const GS_QuiteOperation As String = "-q"
    Private Const GS_StandardOutputDevice As String = "-"
    Private Const GS_MultiplePageCharacter As String = "%"
    Private Const GS_MaxBitmap As String = "-dMaxBitmap={0}"
    Private Const GS_BufferSpace As String = "-dBufferSpace={0}"
    Private Const GS_Password As String = "-sPDFPassword={0}"
#End Region
#Region "Windows Import"
    ''' <summary>Needed to copy memory from one location to another, used to fill the struct</summary>
    ''' <param name="Destination"></param>
    ''' <param name="Source"></param>
    ''' <param name="Length"></param>
    <DllImport("kernel32.dll", EntryPoint:="RtlMoveMemory")> _
    Private Shared Sub CopyMemory(ByVal Destination As IntPtr, ByVal Source As IntPtr, ByVal Length As UInteger)
    End Sub
#End Region
#Region "GhostScript Import"

    ''' <summary>Create a new instance of Ghostscript. This instance is passed to most other gsapi functions. The caller_handle will be provided to callback functions.
    '''  At this stage, Ghostscript supports only one instance. </summary>
    ''' <param name="pinstance"></param>
    ''' <param name="caller_handle"></param>
    ''' <returns></returns>
    <DllImport("gsdll32.dll", EntryPoint:="gsapi_new_instance")> _
    Private Shared Function gsapi_new_instance(ByRef pinstance As IntPtr, ByVal caller_handle As IntPtr) As Integer
    End Function

    ''' <summary>This is the important function that will perform the conversion</summary>
    ''' <param name="instance"></param>
    ''' <param name="argc"></param>
    ''' <param name="argv"></param>
    ''' <returns></returns>
    <DllImport("gsdll32.dll", EntryPoint:="gsapi_init_with_args")> _
    Private Shared Function gsapi_init_with_args(ByVal instance As IntPtr, ByVal argc As Integer, ByVal argv As IntPtr) As Integer
    End Function
    ''' <summary>
    ''' Exit the interpreter. This must be called on shutdown if gsapi_init_with_args() has been called, and just before gsapi_delete_instance(). 
    ''' </summary>
    ''' <param name="instance"></param>
    ''' <returns></returns>
    <DllImport("gsdll32.dll", EntryPoint:="gsapi_exit")> _
    Private Shared Function gsapi_exit(ByVal instance As IntPtr) As Integer
    End Function

    ''' <summary>
    ''' Destroy an instance of Ghostscript. Before you call this, Ghostscript must have finished. If Ghostscript has been initialised, you must call gsapi_exit before gsapi_delete_instance. 
    ''' </summary>
    ''' <param name="instance"></param>
    <DllImport("gsdll32.dll", EntryPoint:="gsapi_delete_instance")> _
    Private Shared Sub gsapi_delete_instance(ByVal instance As IntPtr)
    End Sub
    ''' <summary>Get info about the version of Ghostscript i'm using</summary>
    ''' <param name="pGSRevisionInfo"></param>
    ''' <param name="intLen"></param>
    ''' <returns></returns>
    <DllImport("gsdll32.dll", EntryPoint:="gsapi_revision")> _
    Private Shared Function gsapi_revision(ByRef pGSRevisionInfo As GS_Revision, ByVal intLen As Integer) As Integer
    End Function
    ''' <summary>Use a different I/O</summary>
    ''' <param name="lngGSInstance"></param>
    ''' <param name="gsdll_stdin">Function that menage the Standard INPUT</param>
    ''' <param name="gsdll_stdout">Function that menage the Standard OUTPUT</param>
    ''' <param name="gsdll_stderr">Function that menage the Standard ERROR output</param>
    ''' <returns></returns>
    <DllImport("gsdll32.dll", EntryPoint:="gsapi_set_stdio")> _
    Private Shared Function gsapi_set_stdio(ByVal lngGSInstance As IntPtr, ByVal gsdll_stdin As StdioCallBack, ByVal gsdll_stdout As StdioCallBack, ByVal gsdll_stderr As StdioCallBack) As Integer
    End Function

#End Region
#Region "Const"
    Const e_Quit As Integer = -101
    Const e_NeedInput As Integer = -106
    Const BW_TIFF_G4 As String = "tiffg4"
    Const BW_TIFF_LZW As String = "tifflzw"
    Const GRAY_TIFF_NC As String = "tiffgray"
    Const GRAY_PNG = "pnggray"
    Const GRAY_JPG = "jpeggray"
    Const COLOR_TIFF_RGB As String = "tiff24nc"
    Const COLOR_TIFF_CMYK As String = "tiff32nc"
    Const COLOR_TIFF_CMYK_SEP As String = "tiffsep"
    Const COLOR_PNG_RGB As String = "png16m"
    Const COLOR_JPEG = "jpeg"
    Const PRINT_DPI As Integer = 300
    Const VIEW_DPI As Integer = 200
#End Region
#Region "Variables"
    Private _sDeviceFormat As String
    Private _sParametersUsed As String

    Private _iWidth As Integer
    Private _iHeight As Integer
    Private _iResolutionX As Integer
    Private _iResolutionY As Integer
    Private _iJPEGQuality As Integer
    Private _iMaxBitmap As Integer = 0
    Private _iMaxBuffer As Integer = 0
    Private _sUserPassword As String
    ''' <summary>The first page to convert in image</summary>
    Private _iFirstPageToConvert As Integer = -1
    ''' <summary>The last page to conver in an image</summary>
    Private _iLastPageToConvert As Integer = -1
    ''' <summary>This parameter is used to control subsample antialiasing of graphics</summary>
    Private _iGraphicsAlphaBit As Integer = -1
    ''' <summary>This parameter is used to control subsample antialiasing of text</summary>
    Private _iTextAlphaBit As Integer = -1
    ''' <summary>In how many thread i should perform the conversion</summary>
    ''' <remarks>This is a Major innovation since 8.63 NEVER use it with previous version!</remarks>
    Private _iRenderingThreads As Integer = -1

    ''' <summary>In how many thread i should perform the conversion</summary>
    ''' <remarks>This is a Major innovation since 8.63 NEVER use it with previous version!</remarks>
    ''' <value>Set it to 0 made the program set it to Environment.ProcessorCount HT machine could want to perform a check for this..</value>
    Public Property RenderingThreads() As Integer
      Get
        Return _iRenderingThreads
      End Get
      Set(ByVal value As Integer)
        If value = 0 Then
          _iRenderingThreads = Environment.ProcessorCount
        Else
          _iRenderingThreads = value
        End If
      End Set
    End Property
    Private _bFitPage As Boolean
    Private _bThrowOnlyException As Boolean = False
    Private _bRedirectIO As Boolean = False
    Private _bForcePageSize As Boolean = False
    ''' <summary>The pagesize of the output</summary>
    Private _sDefaultPageSize As String
    Private _objHandle As IntPtr
    ''' <summary>If true i will try to output everypage to a different file!</summary>
    Private _didOutputToMultipleFile As Boolean = False

    Private myProcess As System.Diagnostics.Process
    Public output As StringBuilder
    'public string output;
    'private List<byte> outputBytes;
    'public string error;
#End Region
#Region "Proprieties"
    ''' <summary>
    ''' What format to use to convert
    ''' is suggested to use png256 instead of jpeg for document!
    ''' they are smaller and better suited!
    ''' </summary>
    Public Property OutputFormat() As String
      Get
        Return _sDeviceFormat
      End Get
      Set(ByVal value As String)
        _sDeviceFormat = value
      End Set
    End Property

    ''' <summary>The pagesize of the output</summary>
    ''' <remarks>Without this parameter the output should be letter, complain to USA for this :) if the document specify a different size it will take precedece over this!</remarks>
    Public Property DefaultPageSize() As String
      Get
        Return _sDefaultPageSize
      End Get
      Set(ByVal value As String)
        _sDefaultPageSize = value
      End Set
    End Property

    ''' <summary>If set to true and page default page size will force the rendering in that output format</summary>
    Public Property ForcePageSize() As Boolean
      Get
        Return _bForcePageSize
      End Get
      Set(ByVal value As Boolean)
        _bForcePageSize = value
      End Set
    End Property

    Public Property ParametersUsed() As String
      Get
        Return _sParametersUsed
      End Get
      Set(ByVal value As String)
        _sParametersUsed = value
      End Set
    End Property

    Public Property Width() As Integer
      Get
        Return _iWidth
      End Get
      Set(ByVal value As Integer)
        _iWidth = value
      End Set
    End Property

    Public Property Height() As Integer
      Get
        Return _iHeight
      End Get
      Set(ByVal value As Integer)
        _iHeight = value
      End Set
    End Property

    Public Property MaxBitmap() As Integer
      Get
        Return _iMaxBitmap
      End Get
      Set(ByVal value As Integer)
        _iMaxBitmap = value
      End Set
    End Property

    Public Property MaxBuffer() As Integer
      Get
        Return _iMaxBuffer
      End Get
      Set(ByVal value As Integer)
        _iMaxBuffer = value
      End Set
    End Property

    Public Property UserPassword() As String
      Get
        Return _sUserPassword
      End Get
      Set(ByVal value As String)
        _sUserPassword = value
      End Set
    End Property

    Public Property ResolutionX() As Integer
      Get
        Return _iResolutionX
      End Get
      Set(ByVal value As Integer)
        _iResolutionX = value
      End Set
    End Property

    Public Property ResolutionY() As Integer
      Get
        Return _iResolutionY
      End Get
      Set(ByVal value As Integer)
        _iResolutionY = value
      End Set
    End Property

    ''' <summary>This parameter is used to control subsample antialiasing of graphics</summary>
    ''' <value>Value MUST BE below or equal 0 if not set, or 1,2,or 4 NO OTHER VALUES!</value>
    Public Property GraphicsAlphaBit() As Integer
      Get
        Return _iGraphicsAlphaBit
      End Get
      Set(ByVal value As Integer)
        If (value > 4) Or (value = 3) Then
          Throw New ArgumentOutOfRangeException("The Graphics Alpha Bit must have a value between 1 2 and 4, or <= 0 if not set")
        End If
        _iGraphicsAlphaBit = value
      End Set
    End Property
    ''' <summary>This parameter is used to control subsample antialiasing of text</summary>
    ''' <value>Value MUST BE below or equal 0 if not set, or 1,2,or 4 NO OTHER VALUES!</value>
    Public Property TextAlphaBit() As Integer
      Get
        Return _iTextAlphaBit
      End Get
      Set(ByVal value As Integer)
        If (value > 4) Or (value = 3) Then
          Throw New ArgumentOutOfRangeException("The Text ALpha Bit must have a value between 1 2 and 4, or <= 0 if not set")
        End If
        _iTextAlphaBit = value
      End Set
    End Property

    Public Property FitPage() As [Boolean]
      Get
        Return _bFitPage
      End Get
      Set(ByVal value As [Boolean])
        _bFitPage = value
      End Set
    End Property
    ''' <summary>Quality of compression of JPG</summary>
    Public Property JPEGQuality() As Integer
      Get
        Return _iJPEGQuality
      End Get
      Set(ByVal value As Integer)
        _iJPEGQuality = value
      End Set
    End Property
    ''' <summary>The first page to convert in image</summary>
    Public Property FirstPageToConvert() As Integer
      Get
        Return _iFirstPageToConvert
      End Get
      Set(ByVal value As Integer)
        _iFirstPageToConvert = value
      End Set
    End Property
    ''' <summary>The last page to conver in an image</summary>
    Public Property LastPageToConvert() As Integer
      Get
        Return _iLastPageToConvert
      End Get
      Set(ByVal value As Integer)
        _iLastPageToConvert = value
      End Set
    End Property
    ''' <summary>Set to True if u want the program to never display Messagebox
    ''' but otherwise throw exception</summary>
    Public Property ThrowOnlyException() As [Boolean]
      Get
        Return _bThrowOnlyException
      End Get
      Set(ByVal value As [Boolean])
        _bThrowOnlyException = value
      End Set
    End Property
    ''' <summary>If i should redirect the Output of Ghostscript library somewhere</summary>
    Public Property RedirectIO() As Boolean
      Get
        Return _bRedirectIO
      End Get
      Set(ByVal value As Boolean)
        _bRedirectIO = value
      End Set
    End Property
    ''' <summary>If true i will try to output everypage to a different file!</summary>
    Public Property OutputToMultipleFile() As Boolean
      Get
        Return _didOutputToMultipleFile
      End Get
      Set(ByVal value As Boolean)
        _didOutputToMultipleFile = value
      End Set
    End Property
#End Region
#Region "Init"
    Public Sub New(ByVal objHandle As IntPtr)
      _objHandle = objHandle
    End Sub

    Public Sub New()
      _objHandle = IntPtr.Zero
    End Sub
#End Region

#Region "Convert"
    ''' <summary>Convert a single file!</summary>
    ''' <param name="inputFile">The file PDf to convert</param>
    ''' <param name="outputFile">The image file that will be created</param>
    ''' <remarks>You must pass all the parameter for the conversion
    ''' as Proprieties of this class</remarks>
    ''' <returns>True if the conversion succed!</returns>
    Public Function Convert(ByVal inputFile As String, ByVal outputFile As String) As Boolean
      Return Convert(inputFile, outputFile, _bThrowOnlyException, Nothing)
    End Function

    ''' <summary>Convert a single file!</summary>
    ''' <param name="inputFile">The file PDf to convert</param>
    ''' <param name="outputFile">The image file that will be created</param>
    ''' <param name="parameters">You must pass all the parameter for the conversion here</param>
    ''' <remarks>Thanks to 	tchu_2000 for the help!</remarks>
    ''' <returns>True if the conversion succed!</returns>
    Public Function Convert(ByVal inputFile As String, ByVal outputFile As String, ByVal parameters As String) As Boolean
      Return Convert(inputFile, outputFile, _bThrowOnlyException, parameters)
    End Function

    ''' <summary>Convert a single file!</summary>
    ''' <param name="inputFile">The file PDf to convert</param>
    ''' <param name="outputFile">The image file that will be created</param>
    ''' <param name="throwException">if the function should throw an exception
    ''' or display a message box</param>
    ''' <remarks>You must pass all the parameter for the conversion
    ''' as Proprieties of this class</remarks>
    ''' <returns>True if the conversion succed!</returns>
    Private Function Convert(ByVal inputFile As String, ByVal outputFile As String, ByVal throwException As Boolean, ByVal options As String) As Boolean
      '#Region "Check Input"
      'Avoid to work when the file doesn't exist
      If String.IsNullOrEmpty(inputFile) Then
        If throwException Then
          Throw New ArgumentNullException("inputFile")
        Else
          System.Windows.Forms.MessageBox.Show("The inputfile is missing")
          Return False
        End If
      End If
      If Not System.IO.File.Exists(inputFile) Then
        If throwException Then
          Throw New ArgumentException(String.Format("The file :'{0}' doesn't exist", inputFile), "inputFile")
        Else
          System.Windows.Forms.MessageBox.Show(String.Format("The file :'{0}' doesn't exist", inputFile))
          Return False
        End If
      End If
      If String.IsNullOrEmpty(_sDeviceFormat) Then
        If throwException Then
          Throw New ArgumentNullException("Device")
        Else
          System.Windows.Forms.MessageBox.Show("You didn't provide a device for the conversion")
          Return False
        End If
      End If
      'be sure that if i specify multiple page outpage i added the % to the filename!
      '#End Region
      '#Region "Variables"
      Dim intReturn As Integer, intCounter As Integer, intElementCount As Integer
      'The pointer to the current istance of the dll
      Dim intGSInstanceHandle As IntPtr
      Dim aAnsiArgs As Object()
      Dim aPtrArgs As IntPtr()
      Dim aGCHandle As GCHandle()
      Dim callerHandle As IntPtr, intptrArgs As IntPtr
      Dim gchandleArgs As GCHandle
      '#End Region
      'Generate the list of the parameters i need to pass to the dll
      Dim sArgs As String() = GetGeneratedArgs(inputFile, outputFile, options)
      '#Region "Convert Unicode strings to null terminated ANSI byte arrays"
      ' Convert the Unicode strings to null terminated ANSI byte arrays
      ' then get pointers to the byte arrays.
      intElementCount = sArgs.Length
      aAnsiArgs = New Object(intElementCount - 1) {}
      aPtrArgs = New IntPtr(intElementCount - 1) {}
      aGCHandle = New GCHandle(intElementCount - 1) {}
      'Convert the parameters
      For intCounter = 0 To intElementCount - 1
        aAnsiArgs(intCounter) = StringToAnsiZ(sArgs(intCounter))
        aGCHandle(intCounter) = GCHandle.Alloc(aAnsiArgs(intCounter), GCHandleType.Pinned)
        aPtrArgs(intCounter) = aGCHandle(intCounter).AddrOfPinnedObject()
      Next
      gchandleArgs = GCHandle.Alloc(aPtrArgs, GCHandleType.Pinned)
      intptrArgs = gchandleArgs.AddrOfPinnedObject()
      '#End Region
      '#Region "Create a new istance of the library!"
      intReturn = -1
      Try
        intReturn = gsapi_new_instance(intGSInstanceHandle, _objHandle)
        'Be sure that we create an istance!
        If intReturn < 0 Then
          ClearParameters(aGCHandle, gchandleArgs)
          If throwException Then
            Throw New ApplicationException("I can't create a new istance of Ghostscript please verify no other istance are running!")
          Else
            System.Windows.Forms.MessageBox.Show("I can't create a new istance of Ghostscript please verify no other istance are running!")
            Return False
          End If
        End If
      Catch ex As DllNotFoundException
        'in this case the dll we r using isn't the dll we expect
        ClearParameters(aGCHandle, gchandleArgs)
        If throwException Then
          Throw New ApplicationException("The gsdll32.dll wasn't found in default dlls search path" & "or is not in correct version (doesn't expose the required methods). Please download " & "the version 8.64 from the original website")
        Else
          'Barbara post write much better then me, thanks her for the nice words :P
          System.Windows.Forms.MessageBox.Show("The gsdll32.dll wasn't found in default dlls search path" & "or is not in correct version (doesn't expose the required methods). Please download " & "the version 8.64 from the original website")
          Return False
        End If
      End Try
      callerHandle = IntPtr.Zero
      'remove unwanter handler
      '#End Region
      '#Region "Capture the I/O"
      If (_bRedirectIO) Then
        Dim stdinCallback As StdioCallBack
        stdinCallback = AddressOf gsdll_stdin
        Dim stdoutCallback As StdioCallBack
        stdoutCallback = AddressOf gsdll_stdout
        Dim stderrCallback As StdioCallBack
        stderrCallback = AddressOf gsdll_stderr
        intReturn = gsapi_set_stdio(intGSInstanceHandle, stdinCallback, stdoutCallback, stderrCallback)
        If output Is Nothing Then
          output = New StringBuilder()
        Else
          output.Remove(0, output.Length)
        End If
        myProcess = System.Diagnostics.Process.GetCurrentProcess()
        AddHandler myProcess.OutputDataReceived, AddressOf SaveOutputToImage
      End If
      '#End Region
      intReturn = -1
      'if nothing change it there is an error!
      'Ok now is time to call the interesting method
      Try
        intReturn = gsapi_init_with_args(intGSInstanceHandle, intElementCount, intptrArgs)
      Catch ex As Exception
        If throwException Then
          Throw New ApplicationException(ex.Message, ex)
        Else
          System.Windows.Forms.MessageBox.Show(ex.Message)
        End If
      Finally
        'No matter what happen i MUST close the istance!
        'free all the memory
        ClearParameters(aGCHandle, gchandleArgs)
        gsapi_exit(intGSInstanceHandle)
        'Close the istance
        gsapi_delete_instance(intGSInstanceHandle)
        'delete it
        'In case i was looking for output now stop
        If myProcess IsNot Nothing Then
          RemoveHandler myProcess.OutputDataReceived, AddressOf SaveOutputToImage
        End If
      End Try
      'Conversion was successfull if return code was 0 or e_Quit
      'e_Quit = -101
      Return (intReturn = 0) Or (intReturn = e_Quit)
    End Function

    ''' <summary>Remove the memory allocated</summary>
    ''' <param name="aGCHandle"></param>
    ''' <param name="gchandleArgs"></param>
    Private Sub ClearParameters(ByRef aGCHandle As GCHandle(), ByRef gchandleArgs As GCHandle)
      For intCounter As Integer = 0 To aGCHandle.Length - 1
        aGCHandle(intCounter).Free()
      Next
      gchandleArgs.Free()
    End Sub
#Region "Test (code not used)"
    Private Sub SaveOutputToImage(ByVal sender As Object, ByVal e As System.Diagnostics.DataReceivedEventArgs)
      output.Append(e.Data)
    End Sub

    Public Function Convert(ByVal inputFile As String) As System.Drawing.Image
      _bRedirectIO = True
      If Convert(inputFile, "%stdout", _bThrowOnlyException) Then
        If (output IsNot Nothing) AndAlso (output.Length > 0) Then
          'StringReader sr = new StringReader(output.ToString());
          'MemoryStream ms = new MemoryStream(UTF8Encoding.Default.GetBytes(output.ToString()));
          Dim returnImage As System.Drawing.Image = TryCast(System.Drawing.Image.FromStream(myProcess.StandardOutput.BaseStream).Clone(), System.Drawing.Image)
          'ms.Close();
          Return returnImage
        End If
      End If
      Return Nothing
    End Function
#End Region
#End Region

#Region "Accessory Functions"
    ''' <summary>This function create the list of parameters to pass to the dll with parameters given directly from the program</summary>
    ''' <param name="inputFile"></param>
    ''' <param name="outputFile"></param>
    ''' <param name="otherParameters">The other parameters i could be interested</param>
    ''' <remarks>Be very Cautious using this! code provided and modified from tchu_2000</remarks>
    ''' <returns></returns>
    Private Function GetGeneratedArgs(ByVal inputFile As String, ByVal outputFile As String, ByVal otherParameters As String) As String()
      If Not String.IsNullOrEmpty(otherParameters) Then
        Return GetGeneratedArgs(inputFile, outputFile, otherParameters.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries))
      Else
        Return GetGeneratedArgs(inputFile, outputFile, DirectCast(Nothing, String()))
      End If
    End Function

    ''' <summary>This function create the list of parameters to pass to the dll</summary>
    ''' <param name="inputFile">the file to convert</param>
    ''' <param name="outputFile">where to write the image</param>
    ''' <returns>the list of the arguments</returns>
    Private Function GetGeneratedArgs(ByVal inputFile As String, ByVal outputFile As String, ByVal presetParameters As String()) As String()
      Dim args As String()
      Dim lstExtraArgs As New ArrayList()
      'ok if i haven't been passed a list of parameters create my own
      If (presetParameters Is Nothing) OrElse (presetParameters.Length = 0) Then
        'Ok now check argument per argument and compile them
        'If i want a jpeg i can set also quality
        If _sDeviceFormat = "jpeg" AndAlso _iJPEGQuality > 0 AndAlso _iJPEGQuality < 101 Then
          lstExtraArgs.Add(String.Format(GS_JpegQualityFormat, _iJPEGQuality))
        End If
        'if i provide size it will override the paper size
        If _iWidth > 0 AndAlso _iHeight > 0 Then
          lstExtraArgs.Add(String.Format(GS_PageSizeFormat, _iWidth, _iHeight))
        Else
          'otherwise if aviable use the papersize
          If Not String.IsNullOrEmpty(_sDefaultPageSize) Then
            lstExtraArgs.Add(String.Format(GS_DefaultPaperSize, _sDefaultPageSize))
            'It have no meaning to set it if the default page is not set!
            If _bForcePageSize Then
              lstExtraArgs.Add(GS_FixedMedia)
            End If
          End If
        End If

        'not set antialiasing settings
        If _iGraphicsAlphaBit > 0 Then
          lstExtraArgs.Add(String.Format(GS_GraphicsAlphaBits, _iGraphicsAlphaBit))
        End If
        If _iTextAlphaBit > 0 Then
          lstExtraArgs.Add(String.Format(GS_TextAlphaBits, _iTextAlphaBit))
        End If
        'Should i try to fit?
        If _bFitPage Then
          lstExtraArgs.Add(GS_FitPage)
        End If
        'Should I try to speed up rendering?
        If _iMaxBitmap > 0 Then
          lstExtraArgs.Add([String].Format(GS_MaxBitmap, _iMaxBitmap))
        End If
        If _iMaxBuffer > 0 Then
          lstExtraArgs.Add([String].Format(GS_BufferSpace, _iMaxBuffer))
        End If
        'Password
        If _sUserPassword <> "" Then
          lstExtraArgs.Add([String].Format(GS_Password, _sUserPassword))
        End If
        'Do i have a forced resolution?
        If _iResolutionX > 0 Then
          If _iResolutionY > 0 Then
            lstExtraArgs.Add([String].Format(GS_ResolutionXYFormat, _iResolutionX, _iResolutionY))
          Else
            lstExtraArgs.Add([String].Format(GS_ResolutionXFormat, _iResolutionX))
          End If
        End If
        If _iFirstPageToConvert > 0 Then
          lstExtraArgs.Add([String].Format(GS_FirstPageFormat, _iFirstPageToConvert))
        End If
        If _iLastPageToConvert > 0 Then
          If (_iFirstPageToConvert > 0) AndAlso (_iFirstPageToConvert > _iLastPageToConvert) Then
            Throw New ArgumentOutOfRangeException(String.Format("The 1st page to convert ({0}) can't be after then the last one ({1})", _iFirstPageToConvert, _iLastPageToConvert))
          End If
          lstExtraArgs.Add([String].Format(GS_LastPageFormat, _iLastPageToConvert))
        End If
        'Set in how many threads i want to do the work
        If _iRenderingThreads > 0 Then
          lstExtraArgs.Add([String].Format(GS_RenderingThreads, _iRenderingThreads))
        End If

        'If i want to redirect write it to the standard output!
        If _bRedirectIO Then
          'In this case you must also use the -q switch to prevent Ghostscript
          'from writing messages to standard output which become
          'mixed with the intended output stream. 
          outputFile = GS_StandardOutputDevice
          lstExtraArgs.Add(GS_QuiteOperation)
        End If
        Dim iFixedCount As Integer = 7
        'This are the mandatory options
        Dim iExtraArgsCount As Integer = lstExtraArgs.Count
        args = New String(iFixedCount + (lstExtraArgs.Count - 1)) {}
        args(1) = GS_Fixed1stParameter
        '"-dNOPAUSE";//I don't want interruptions
        args(2) = GS_Fixed2ndParameter
        '"-dBATCH";//stop after
        args(3) = GS_Fixed3rdParameter
        '"-dSAFER";
        args(4) = String.Format(GS_DeviceFormat, _sDeviceFormat)
        'what kind of export format i should provide
        'For a complete list watch here:
        'http://pages.cs.wisc.edu/~ghost/doc/cvs/Devices.htm
        'Fill the remaining parameters
        For i As Integer = 0 To iExtraArgsCount - 1
          args(5 + i) = DirectCast(lstExtraArgs(i), String)
        Next
      Else
        '3 arguments MUST be added 0 (meaningless) and at the end the output and the inputfile
        args = New String(presetParameters.Length + 2) {}
        'now use the parameters i receive (thanks CrucialBT to point this out!)
        For i As Integer = 1 To presetParameters.Length - 1
          args(i) = presetParameters(i - 1)
        Next
      End If
      args(0) = GS_FirstParameter
      'this parameter have little real use
      'Now check if i want to update to 1 file per page i have to be sure do add % to the output filename
      If (_didOutputToMultipleFile) AndAlso (Not outputFile.Contains(GS_MultiplePageCharacter)) Then
        ' Thanks to Spillie to show me the error!
        Dim lastDotIndex As Integer = outputFile.LastIndexOf("."c)
        If lastDotIndex > 0 Then
          outputFile = outputFile.Insert(lastDotIndex, "%d")
        End If
      End If
      'Ok now save them to be shown 4 debug use
      _sParametersUsed = String.Empty
      'Copy all the args except the 1st that is useless and the last 2
      For i As Integer = 1 To args.Length - 3
        _sParametersUsed += " " & args(i)
      Next
      'Fill outputfile and inputfile as last 2 arguments!
      args(args.Length - 2) = String.Format(GS_OutputFileFormat, outputFile)
      args(args.Length - 1) = String.Format("{0}", inputFile)

      _sParametersUsed += (" " & String.Format(GS_OutputFileFormat, String.Format("""{0}""", outputFile)) & " ") + String.Format("""{0}""", inputFile)
      Return args
    End Function

    ''' <summary>
    ''' Convert a Unicode string to a null terminated Ansi string for Ghostscript.
    ''' The result is stored in a byte array
    ''' </summary>
    ''' <param name="str">The parameter i want to convert</param>
    ''' <returns>the byte array that contain the string</returns>
    Private Shared Function StringToAnsiZ(ByVal str As String) As Byte()
      ' Later you will need to convert
      ' this byte array to a pointer with
      ' GCHandle.Alloc(XXXX, GCHandleType.Pinned)
      ' and GSHandle.AddrOfPinnedObject()
      'int intElementCount,intCounter;

      'This with Encoding.Default should work also with Chineese Japaneese
      'Thanks to tchu_2000 I18N related patch
      If str Is Nothing Then
        str = [String].Empty
      End If
      Return Encoding.[Default].GetBytes(str)
    End Function

    ''' <summary>Convert a Pointer to a string to a real string</summary>
    ''' <param name="strz">the pointer to the string in memory</param>
    ''' <returns>The string</returns>
    Public Shared Function AnsiZtoString(ByVal strz As IntPtr) As String
      If strz <> IntPtr.Zero Then
        Return Marshal.PtrToStringAnsi(strz)
      Else
        Return String.Empty
      End If
    End Function
#End Region
#Region "Menage Standard Input & Standard Output"
    Public Function gsdll_stdin(ByVal intGSInstanceHandle As IntPtr, ByVal strz As IntPtr, ByVal intBytes As Integer) As Integer
      ' This is dumb code that reads one byte at a time
      ' Ghostscript doesn't mind this, it is just very slow
      'If intBytes = 0 Then
      '    Return 0
      'Else
      '    Dim ich As Integer = Console.Read()
      '    If ich = -1 Then
      '        Return 0
      '    Else
      '        ' EOF
      '        Dim bch As Byte = CByte(ich)
      '        Dim gcByte As GCHandle = GCHandle.Alloc(bch, GCHandleType.Pinned)
      '        Dim ptrByte As IntPtr = gcByte.AddrOfPinnedObject()
      '        CopyMemory(strz, ptrByte, 1)
      '        ptrByte = IntPtr.Zero
      '        gcByte.Free()
      '        Return 1
      '    End If
      'End If
    End Function

    Public Function gsdll_stdout(ByVal intGSInstanceHandle As IntPtr, ByVal strz As IntPtr, ByVal intBytes As Integer) As Integer
      If intBytes > 0 Then
        'Console.Write(Marshal.PtrToStringAnsi(strz))
      End If
      Return 0
    End Function

    Public Function gsdll_stderr(ByVal intGSInstanceHandle As IntPtr, ByVal strz As IntPtr, ByVal intBytes As Integer) As Integer
      'return gsdll_stdout(intGSInstanceHandle, strz, intBytes);
      'Console.Write(Marshal.PtrToStringAnsi(strz))
      Return intBytes
    End Function
#End Region
#Region "Menage Revision"
    Public Function GetRevision() As GhostScriptRevision
      ' Check revision number of Ghostscript
      Dim intReturn As Integer
      Dim udtGSRevInfo As New GS_Revision()
      Dim output As GhostScriptRevision
      Dim gcRevision As GCHandle
      gcRevision = GCHandle.Alloc(udtGSRevInfo, GCHandleType.Pinned)
      intReturn = gsapi_revision(udtGSRevInfo, 16)
      output.intRevision = udtGSRevInfo.intRevision
      output.intRevisionDate = udtGSRevInfo.intRevisionDate
      output.ProductInformation = AnsiZtoString(udtGSRevInfo.strProduct)
      output.CopyrightInformations = AnsiZtoString(udtGSRevInfo.strCopyright)
      gcRevision.Free()
      Return output
    End Function
#End Region

#Region "Simple Conversion Functions"

    Public Shared Function ConvertPdfToGraphic(ByVal inputFileName As String, _
                                               ByVal outputFileName As String, _
                                               ByVal fileFormat As String, _
                                               ByVal DPI As Integer, _
                                               Optional ByVal startPageNumber As Integer = 0, _
                                               Optional ByVal endPageNumber As Integer = 0, _
                                               Optional ByVal ToPrinter As Boolean = False, _
                                               Optional ByVal Password As String = "" _
                                               ) As String
      ConvertPdfToGraphic = ""
      Dim converter As New ConvertPDF.PDFConvert
      Dim Converted As Boolean = False
      converter.RenderingThreads = Environment.ProcessorCount
      If fileFormat.Contains("tif") Then
        converter.OutputToMultipleFile = False
      Else
        converter.OutputToMultipleFile = True
      End If
      If startPageNumber = 0 Then
        converter.FirstPageToConvert = -1
        converter.LastPageToConvert = -1
      Else
        converter.FirstPageToConvert = startPageNumber
        converter.LastPageToConvert = endPageNumber
      End If
      converter.FitPage = False
      converter.JPEGQuality = 70
      If ToPrinter = True Then 'Turn off anti-aliasing
        converter.TextAlphaBit = -1
        converter.GraphicsAlphaBit = -1
      Else 'Turn on anti-aliasing
        converter.TextAlphaBit = 4
        converter.GraphicsAlphaBit = 4
      End If
      converter.ResolutionX = DPI
      converter.ResolutionY = DPI
      converter.OutputFormat = fileFormat
      converter.UserPassword = Password
      Dim input As System.IO.FileInfo = New FileInfo(inputFileName)
      'If the output file exists already, be sure to add a random name at the end until it is unique
      While File.Exists(outputFileName)
        Dim suffix As String = System.Text.RegularExpressions.Regex.Replace(outputFileName, "^.+\.(.+$)", "$1")
        outputFileName = outputFileName.Replace("." & suffix, "(" & Now.Ticks & ")." & suffix)
      End While
      Converted = converter.Convert(input.FullName, outputFileName)
      If Converted Then
        ConvertPdfToGraphic = outputFileName
      Else
        ConvertPdfToGraphic = ""
      End If
    End Function

    Public Shared Function GetPageFromPDF(ByVal filename As String, ByVal PageNumber As Integer, Optional ByVal DPI As Integer = VIEW_DPI, Optional ByVal Password As String = "", Optional ByVal forPrinting As Boolean = False) As System.Drawing.Image
      Dim converter As New ConvertPDF.PDFConvert
      Dim Converted As Boolean = False
      converter.RenderingThreads = Environment.ProcessorCount
      converter.OutputToMultipleFile = False
      converter.MaxBitmap = 100000000 '100 MB
      converter.MaxBuffer = 200000000 '200 MB
      If PageNumber > 0 Then
        converter.FirstPageToConvert = PageNumber
        converter.LastPageToConvert = PageNumber
      Else
        GetPageFromPDF = Nothing
        Exit Function
      End If
      converter.FitPage = False
      converter.JPEGQuality = 70
      converter.UserPassword = Password
      If DPI <> VIEW_DPI Then 'Custom resolution
        converter.ResolutionX = DPI
        converter.ResolutionY = DPI
      Else ' Default resolution
        converter.ResolutionX = VIEW_DPI
        converter.ResolutionY = VIEW_DPI
      End If
      If forPrinting Then 'Turn off anti-aliasing (crisp edges)
        converter.TextAlphaBit = -1
        converter.GraphicsAlphaBit = -1
      Else 'Turn on anti-aliasing (smooth edges)
        converter.TextAlphaBit = 4
        converter.GraphicsAlphaBit = 4
      End If
      converter.OutputFormat = COLOR_PNG_RGB
      Dim output As String = System.IO.Path.GetTempPath & Now.Ticks & ".png"
      Converted = converter.Convert(filename, output)
      If Converted Then
        GetPageFromPDF = New Bitmap(output)
        ImageUtil.DeleteFile(output)
      Else
        GetPageFromPDF = Nothing
      End If
    End Function

#End Region
  End Class

  ''' <summary>Delegate used by Ghostscript to perform I/O operations</summary>
  ''' <param name="handle"></param>
  ''' <param name="strptr"></param>
  ''' <param name="count"></param>
  ''' <returns></returns>
  Public Delegate Function StdioCallBack(ByVal handle As IntPtr, ByVal strptr As IntPtr, ByVal count As Integer) As Integer
  ''' <summary>This struct is filled with the information of the version of this ghostscript</summary>
  ''' <remarks>Have the layout defined cuz i will fill it with a kernel copy memory</remarks>
  <StructLayout(LayoutKind.Sequential)> _
  Structure GS_Revision
    Public strProduct As IntPtr
    Public strCopyright As IntPtr
    Public intRevision As Integer
    Public intRevisionDate As Integer
  End Structure

  Public Structure GhostScriptRevision
    Public ProductInformation As String
    Public CopyrightInformations As String
    Public intRevision As Integer
    Public intRevisionDate As Integer
  End Structure

End Namespace
