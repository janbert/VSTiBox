Imports System.Drawing.Imaging
Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Public Class ImageUtil

  Public Shared Function MakeGrayscale(ByVal original As System.Drawing.Bitmap) As System.Drawing.Bitmap
    'create a blank bitmap the same size as original
    Dim newBitmap As New System.Drawing.Bitmap(original.Width, original.Height)

    'get a graphics object from the new image
    Dim g As Graphics = Graphics.FromImage(newBitmap)

    'create the grayscale ColorMatrix
    Dim colorMatrix As New ColorMatrix(New Single()() {New Single() {0.3, 0.3, 0.3, 0, 0}, New Single() {0.59, 0.59, 0.59, 0, 0}, New Single() {0.11, 0.11, 0.11, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})

    'create some image attributes
    Dim attributes As New ImageAttributes()

    'set the color matrix attribute
    attributes.SetColorMatrix(colorMatrix)

    'draw the original image on the new image
    'using the grayscale color matrix
    g.DrawImage(original, New Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, _
     GraphicsUnit.Pixel, attributes)

    'dispose the Graphics object
    g.Dispose()

    Return newBitmap
  End Function

  Public Shared Function BitmapTo1Bpp(ByVal img As System.Drawing.Bitmap) As System.Drawing.Bitmap

    If img.PixelFormat <> PixelFormat.Format32bppPArgb Then
      Dim temp As New System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb)
      Dim g As Graphics = Graphics.FromImage(temp)
      g.DrawImage(img, New Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel)
      img.Dispose()
      g.Dispose()
      img = temp
    End If

    Dim imageTemp As System.Drawing.Image
    imageTemp = img

    'lock the bits of the original bitmap
    Dim bmdo As BitmapData = img.LockBits(New Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat)

    'and the new 1bpp bitmap
    Dim bm As New System.Drawing.Bitmap(imageTemp.Width, imageTemp.Height, PixelFormat.Format1bppIndexed)
    Dim bmdn As BitmapData = bm.LockBits(New Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed)

    'for diagnostics
    Dim dt As DateTime = DateTime.Now

    'scan through the pixels Y by X
    Dim y As Integer
    For y = 0 To img.Height - 1
      Dim x As Integer
      For x = 0 To img.Width - 1
        'generate the address of the colour pixel
        Dim index As Integer = y * bmdo.Stride + x * 4
        'check its brightness
        If Color.FromArgb(Marshal.ReadByte(bmdo.Scan0, index + 2), Marshal.ReadByte(bmdo.Scan0, index + 1), Marshal.ReadByte(bmdo.Scan0, index)).GetBrightness() > 0.5F Then
          Dim imgUtil As New ImageUtil
          imgUtil.SetIndexedPixel(x, y, bmdn, True) 'set it if its bright.
        End If
      Next x
    Next y
    'tidy up
    bm.UnlockBits(bmdn)
    img.UnlockBits(bmdo)
    imageTemp = Nothing
    'display the 1bpp image.
    Return bm
  End Function

  Public Shared Sub ScaleImage(ByRef img As Drawing.Image, ByVal scale_factor As Single)
    Dim bm_source As New Drawing.Bitmap(img)
    Dim bm_dest As New Drawing.Bitmap( _
        CInt(bm_source.Width * scale_factor), _
        CInt(bm_source.Height * scale_factor))
    Dim gr_dest As Graphics = Graphics.FromImage(bm_dest)
    gr_dest.DrawImage(bm_source, 0, 0, _
        bm_dest.Width + 1, _
        bm_dest.Height + 1)
    bm_source = Nothing
    gr_dest.Dispose()
    img = bm_dest
  End Sub

  Public Shared Sub ScaleImageToPicBox(ByRef oPict As PictureBox, ByRef img As Drawing.Image)
    Dim xPercent As Single = oPict.Width / img.Width
    Dim yPercent As Single = oPict.Height / img.Height
    Dim scalePercent As Single = 0
    If xPercent > yPercent Then
      scalePercent = yPercent
    Else
      scalePercent = xPercent
    End If
    ScaleImage(img, scalePercent)
  End Sub

  Public Shared Sub RotateImageClockwise(ByRef pPicBox As PictureBox)
    pPicBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone)
    FlipDimensions(pPicBox)
    RecalcPageLocation(pPicBox)
    pPicBox.Refresh()
  End Sub

  Public Shared Sub RotateImageCounterclockwise(ByRef pPicBox As PictureBox)
    pPicBox.Image.RotateFlip(RotateFlipType.Rotate270FlipNone)
    FlipDimensions(pPicBox)
    RecalcPageLocation(pPicBox)
    pPicBox.Refresh()
  End Sub

  Public Shared Sub FlipDimensions(ByRef pPicbox As PictureBox)
    Dim height As Integer = pPicbox.Height
    Dim width As Integer = pPicbox.Width
    pPicbox.Height = width
    pPicbox.Width = height
  End Sub

  Public Shared Sub ApplyRotation(ByRef img As Drawing.Image, ByVal numberOfRotations As Integer)
    If numberOfRotations < 0 Then
      For i As Integer = 1 To Math.Abs(numberOfRotations)
        img.RotateFlip(RotateFlipType.Rotate270FlipNone)
      Next
    End If

    If numberOfRotations > 0 Then
      For i As Integer = 1 To numberOfRotations
        img.RotateFlip(RotateFlipType.Rotate90FlipNone)
      Next
    End If

  End Sub

  Public Shared Sub PictureBoxZoomActual(ByRef pPicBox As PictureBox)
    If Not Nothing Is pPicBox And Not Nothing Is pPicBox.Image Then
      pPicBox.Width = pPicBox.Image.Width
      pPicBox.Height = pPicBox.Image.Height
    End If
  End Sub

  Public Shared Sub PictureBoxZoomPageWidth(ByRef pPicBox As PictureBox)
    If Not Nothing Is pPicBox And Not Nothing Is pPicBox.Parent Then
      pPicBox.Width = pPicBox.Parent.ClientSize.Width - 18
      Dim ScaleAmount As Double = (pPicBox.Width / pPicBox.Image.Width)
      pPicBox.Height = CInt(pPicBox.Image.Height * ScaleAmount)
    End If
  End Sub

  Public Shared Sub PictureBoxZoomFit(ByRef pPicBox As PictureBox)
    If Not Nothing Is pPicBox And Not Nothing Is pPicBox.Parent Then
      PictureBoxCenter(pPicBox, (pPicBox.Parent.ClientSize.Width - 7), (pPicBox.Parent.ClientSize.Height - 7))
      pPicBox.Location = New Point(0, 0)
    End If
  End Sub

  Public Shared Sub PictureBoxZoomIn(ByRef pPicBox As PictureBox)
    If Not Nothing Is pPicBox And Not Nothing Is pPicBox.Parent Then
      PictureBoxCenter(pPicBox, (pPicBox.Width * 1.25), (pPicBox.Height * 1.25))
    End If
  End Sub

  Public Shared Sub PictureBoxZoomOut(ByRef pPicBox As PictureBox)
    If Not Nothing Is pPicBox Then
      PictureBoxCenter(pPicBox, (pPicBox.Width / 1.25), (pPicBox.Height / 1.25))
    End If
  End Sub

  Public Shared Sub PictureBoxCenter(ByRef oPictureBox As PictureBox, ByVal psWidth As Integer, ByVal psHeight As Integer)
    oPictureBox.Width = psWidth
    oPictureBox.Height = psHeight
    RecalcPageLocation(oPictureBox)
  End Sub

  Public Shared Sub RecalcPageLocation(ByRef oPictureBox As PictureBox)

    Dim PageSize As New Size(oPictureBox.Size)
    Dim ClientBounds As New Size(oPictureBox.Parent.ClientSize)
    Dim PageLocation As New Point(oPictureBox.Location)
    Dim HorizontalScrollPosition As Integer = CType(oPictureBox.Parent, Panel).HorizontalScroll.Value
    Dim VerticalScrollPosition As Integer = CType(oPictureBox.Parent, Panel).VerticalScroll.Value
    Dim LeftMargin As Integer = oPictureBox.Parent.Margin.Left
    Dim TopMargin As Integer = oPictureBox.Parent.Margin.Top

    If PageSize.Width < ClientBounds.Width AndAlso PageSize.Height > ClientBounds.Height Then
      'Center vertically
      PageLocation = New Point((ClientBounds.Width - PageSize.Width) / 2 + LeftMargin, TopMargin - VerticalScrollPosition)
    ElseIf PageSize.Width > ClientBounds.Width AndAlso PageSize.Height < ClientBounds.Height Then
      'Center horizontally
      PageLocation = New Point(LeftMargin - HorizontalScrollPosition, (ClientBounds.Height - PageSize.Height) / 2 + TopMargin)
    ElseIf PageSize.Width < ClientBounds.Width AndAlso PageSize.Height < ClientBounds.Height Then
      'center both
      PageLocation = New Point((ClientBounds.Width - PageSize.Width) / 2 + LeftMargin, (ClientBounds.Height - PageSize.Height) / 2 + TopMargin)
    Else
      PageLocation = New Point(LeftMargin - HorizontalScrollPosition, TopMargin - VerticalScrollPosition)
    End If
    oPictureBox.Location = PageLocation
    oPictureBox.Bounds = New Rectangle(PageLocation, PageSize)

  End Sub

  Public Shared Sub PictureBoxZoomFitMany(ByRef pPicBox As PictureBox)
    If Not Nothing Is pPicBox.Parent Then
      Dim Height As Integer = pPicBox.Parent.ClientSize.Height - 14
      Dim Width As Integer = pPicBox.Parent.ClientSize.Width - 14
      Dim Location As New Point(0, 0)
      For Each itemControl As Control In pPicBox.Parent.Controls
        If TypeOf itemControl Is PictureBox Then
          itemControl.Height = Height
          itemControl.Width = Width
          itemControl.Location = Location
        End If
      Next
    End If
  End Sub

  Public Shared Function GenerateThumbnail(ByVal original As System.Drawing.Image, ByVal percentage As Integer) As System.Drawing.Image
    If percentage < 1 Then
      Throw New Exception("Thumbnail size must be aat least 1% of the original size")
    End If
    Dim tn As New System.Drawing.Bitmap(CInt(original.Width * 0.01F * percentage), CInt(original.Height * 0.01F * percentage))
    Dim g As Graphics = Graphics.FromImage(tn)
    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
    g.DrawImage(original, New Rectangle(0, 0, tn.Width, tn.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel)
    g.Dispose()
    Return CType(tn, System.Drawing.Image)
  End Function

  Public Shared Function SaveImageToTiff(ByVal img As System.Drawing.Image) As String
    Dim sTemp As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\DDI_"
    Dim sTStamp As String = Format(Now, "yyyyMMddhhmmssfff")
    Dim FileName As String = sTemp & sTStamp & ".tif"
    img.Save(FileName, ImageFormat.Tiff)
    Return FileName
  End Function

  Public Shared Function GetFrameFromTiff(ByVal Filename As String, ByVal FrameNumber As Integer) As System.Drawing.Image
    Dim fs As FileStream = File.Open(Filename, FileMode.Open, FileAccess.Read)
    Dim bm As System.Drawing.Bitmap = CType(System.Drawing.Bitmap.FromStream(fs), System.Drawing.Bitmap)
    bm.SelectActiveFrame(FrameDimension.Page, FrameNumber)
    Dim temp As New System.Drawing.Bitmap(bm.Width, bm.Height)
    Dim g As Graphics = Graphics.FromImage(temp)
    g.InterpolationMode = InterpolationMode.NearestNeighbor
    g.DrawImage(bm, 0, 0, bm.Width, bm.Height)
    g.Dispose()
    GetFrameFromTiff = temp
    fs.Close()
  End Function


  Public Shared Function GetImagePageFromFileForPrint(ByVal sFileName As String, ByVal iPageNumber As Integer, Optional ByVal DPI As Integer = 300, Optional ByVal password As String = "") As System.Drawing.Image
    GetImagePageFromFileForPrint = Nothing
    If ImageUtil.IsPDF(sFileName) Then 'get image of page from file for printing
      GetImagePageFromFileForPrint = ConvertPDF.PDFConvert.GetPageFromPDF(sFileName, iPageNumber, DPI, password, True)
    ElseIf ImageUtil.IsTiff(sFileName) Then
      GetImagePageFromFileForPrint = ImageUtil.GetFrameFromTiff(sFileName, iPageNumber - 1)
    End If
  End Function

  Public Shared Function GetImageFrameCount(ByVal sFileName As String, Optional ByVal userPassword As String = "") As Integer
    If ImageUtil.IsPDF(sFileName) Then
      GetImageFrameCount = iTextSharpUtil.GetPDFPageCount(sFileName, userPassword)
    ElseIf ImageUtil.IsTiff(sFileName) Then
      GetImageFrameCount = GetTiffFrameCount(sFileName)
    End If
  End Function

  Public Shared Function GetTiffFrameCount(ByVal FileName As String) As Integer
    Dim bm As New System.Drawing.Bitmap(FileName)
    GetTiffFrameCount = bm.GetFrameCount(FrameDimension.Page)
    bm.Dispose()
  End Function

  Public Shared Sub DeleteFile(ByVal filename As String)
    Try
      System.IO.File.Delete(filename)
    Catch ex As Exception
    End Try
  End Sub

  Public Shared Function IsTiff(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.tiff*$", RegexOptions.IgnoreCase)
  End Function

  Public Shared Function IsPDF(ByVal filename As String) As Boolean
    If Nothing Is filename Then Return False
    Return Regex.IsMatch(filename, "\.pdf$", RegexOptions.IgnoreCase)
  End Function

  Public Shared Function CropBitmap(ByRef bmp As System.Drawing.Bitmap, ByVal cropX As Integer, ByVal cropY As Integer, ByVal cropWidth As Integer, ByVal cropHeight As Integer) As System.Drawing.Bitmap
    Dim rect As New Rectangle(cropX, cropY, cropWidth, cropHeight)
    Dim cropped As System.Drawing.Bitmap = bmp.Clone(rect, bmp.PixelFormat)
    Return cropped
  End Function

  Public Shared Sub CompressTiff(ByVal Filename As String)
    Dim myBitmap As System.Drawing.Bitmap
    Dim myImageCodecInfo As ImageCodecInfo
    Dim myEncoder As Encoder
    Dim myEncoderParameter1 As EncoderParameter
    Dim myEncoderParameter2 As EncoderParameter
    Dim myEncoderParameters As EncoderParameters

    ' Create a Bitmap object based on a BMP file.
    myBitmap = New System.Drawing.Bitmap(Filename)

    ' Get an ImageCodecInfo object that represents the TIFF codec.
    myImageCodecInfo = GetEncoderInfo("image/tiff")

    ' Create an Encoder object based on the GUID
    ' for the Compression parameter category.
    myEncoder = Encoder.Compression

    ' Create an EncoderParameters object.
    ' An EncoderParameters object has an array of EncoderParameter
    ' objects. In this case, there is only one
    ' EncoderParameter object in the array.
    myEncoderParameters = New EncoderParameters(2)

    ' Save the bitmap as a TIFF file with LZW compression.
    myEncoderParameter1 = New EncoderParameter(myEncoder, Fix(EncoderValue.CompressionLZW))
    myEncoderParameter2 = New EncoderParameter(Encoder.SaveFlag, CLng(EncoderValue.MultiFrame))

    myEncoderParameters.Param(0) = myEncoderParameter1
    myEncoderParameters.Param(1) = myEncoderParameter2
    Dim outputfilename As String = Filename.Replace(".tif", Now.Ticks & ".tif")
    myBitmap.Save(outputfilename, myImageCodecInfo, myEncoderParameters)
    Dim FrameCount As Integer = myBitmap.GetFrameCount(FrameDimension.Page)
    If FrameCount > 1 Then
      For iFrame As Integer = 1 To FrameCount - 1
        myBitmap.SelectActiveFrame(FrameDimension.Page, iFrame)
        myEncoderParameters.Param(0) = New EncoderParameter(Encoder.SaveFlag, CLng(EncoderValue.FrameDimensionPage))
        myBitmap.SaveAdd(myBitmap, myEncoderParameters)
      Next
    End If
    myEncoderParameters.Param(0) = New EncoderParameter(Encoder.SaveFlag, CLng(EncoderValue.Flush))
    myBitmap.SaveAdd(myEncoderParameters)
    myBitmap.Dispose()
    While File.Exists(Filename)
      Try
        File.Delete(Filename)
      Catch
      End Try
    End While
    File.Move(outputfilename, Filename)
  End Sub

  'Needed subroutine for 1bit conversion
  Protected Sub SetIndexedPixel(ByVal x As Integer, ByVal y As Integer, ByVal bmd As BitmapData, ByVal pixel As Boolean)
    Dim index As Integer = y * bmd.Stride + (x >> 3)
    Dim p As Byte = Marshal.ReadByte(bmd.Scan0, index)
    Dim mask As Byte = &H80 >> (x And &H7)
    If pixel Then
      p = p Or mask
    Else
      p = p And CByte(mask ^ &HFF)
    End If
    Marshal.WriteByte(bmd.Scan0, index, p)
  End Sub

  Private Shared Function GetEncoderInfo(ByVal mimeType As String) As ImageCodecInfo
    Dim j As Integer
    Dim encoders() As ImageCodecInfo
    encoders = ImageCodecInfo.GetImageEncoders()

    j = 0
    While j < encoders.Length
      If encoders(j).MimeType = mimeType Then
        Return encoders(j)
      End If
      j += 1
    End While
    Return Nothing

  End Function

End Class
