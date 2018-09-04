<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ExportOptions
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.rbHtmlImage = New System.Windows.Forms.RadioButton
    Me.rbTIFF = New System.Windows.Forms.RadioButton
    Me.rbPNG = New System.Windows.Forms.RadioButton
    Me.rbJpeg = New System.Windows.Forms.RadioButton
    Me.rbText = New System.Windows.Forms.RadioButton
    Me.rbHtml = New System.Windows.Forms.RadioButton
    Me.rbPostscript = New System.Windows.Forms.RadioButton
    Me.GroupBox2 = New System.Windows.Forms.GroupBox
    Me.nuDown = New System.Windows.Forms.NumericUpDown
    Me.Label2 = New System.Windows.Forms.Label
    Me.nuStart = New System.Windows.Forms.NumericUpDown
    Me.Label1 = New System.Windows.Forms.Label
    Me.btOK = New System.Windows.Forms.Button
    Me.btCancel = New System.Windows.Forms.Button
    Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
    Me.GroupBox3 = New System.Windows.Forms.GroupBox
    Me.Label4 = New System.Windows.Forms.Label
    Me.Label3 = New System.Windows.Forms.Label
    Me.nuDPI = New System.Windows.Forms.NumericUpDown
    Me.GroupBox1.SuspendLayout()
    Me.GroupBox2.SuspendLayout()
    CType(Me.nuDown, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.nuStart, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.GroupBox3.SuspendLayout()
    CType(Me.nuDPI, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'GroupBox1
    '
    Me.GroupBox1.Controls.Add(Me.rbHtmlImage)
    Me.GroupBox1.Controls.Add(Me.rbTIFF)
    Me.GroupBox1.Controls.Add(Me.rbPNG)
    Me.GroupBox1.Controls.Add(Me.rbJpeg)
    Me.GroupBox1.Controls.Add(Me.rbText)
    Me.GroupBox1.Controls.Add(Me.rbHtml)
    Me.GroupBox1.Controls.Add(Me.rbPostscript)
    Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(260, 114)
    Me.GroupBox1.TabIndex = 0
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "File Format"
    '
    'rbHtmlImage
    '
    Me.rbHtmlImage.AutoSize = True
    Me.rbHtmlImage.Location = New System.Drawing.Point(6, 88)
    Me.rbHtmlImage.Name = "rbHtmlImage"
    Me.rbHtmlImage.Size = New System.Drawing.Size(128, 17)
    Me.rbHtmlImage.TabIndex = 6
    Me.rbHtmlImage.TabStop = True
    Me.rbHtmlImage.Tag = "HTML (*.html)|*.html"
    Me.rbHtmlImage.Text = "HTML (Image Viewer)"
    Me.rbHtmlImage.UseVisualStyleBackColor = True
    '
    'rbTIFF
    '
    Me.rbTIFF.AutoSize = True
    Me.rbTIFF.Location = New System.Drawing.Point(139, 42)
    Me.rbTIFF.Name = "rbTIFF"
    Me.rbTIFF.Size = New System.Drawing.Size(47, 17)
    Me.rbTIFF.TabIndex = 5
    Me.rbTIFF.TabStop = True
    Me.rbTIFF.Tag = "TIFF (*.tif)|*.tif"
    Me.rbTIFF.Text = "TIFF"
    Me.rbTIFF.UseVisualStyleBackColor = True
    '
    'rbPNG
    '
    Me.rbPNG.AutoSize = True
    Me.rbPNG.Location = New System.Drawing.Point(6, 65)
    Me.rbPNG.Name = "rbPNG"
    Me.rbPNG.Size = New System.Drawing.Size(48, 17)
    Me.rbPNG.TabIndex = 4
    Me.rbPNG.TabStop = True
    Me.rbPNG.Tag = "PNG (*.png)|*.png"
    Me.rbPNG.Text = "PNG"
    Me.rbPNG.UseVisualStyleBackColor = True
    '
    'rbJpeg
    '
    Me.rbJpeg.AutoSize = True
    Me.rbJpeg.Location = New System.Drawing.Point(139, 19)
    Me.rbJpeg.Name = "rbJpeg"
    Me.rbJpeg.Size = New System.Drawing.Size(55, 17)
    Me.rbJpeg.TabIndex = 3
    Me.rbJpeg.TabStop = True
    Me.rbJpeg.Tag = "JPEG (*.jpg)|*.jpg"
    Me.rbJpeg.Text = "JPEG "
    Me.rbJpeg.UseVisualStyleBackColor = True
    '
    'rbText
    '
    Me.rbText.AutoSize = True
    Me.rbText.Location = New System.Drawing.Point(6, 42)
    Me.rbText.Name = "rbText"
    Me.rbText.Size = New System.Drawing.Size(72, 17)
    Me.rbText.TabIndex = 2
    Me.rbText.TabStop = True
    Me.rbText.Tag = "Plain Text (*.txt)|*.txt"
    Me.rbText.Text = "Plain Text"
    Me.rbText.UseVisualStyleBackColor = True
    '
    'rbHtml
    '
    Me.rbHtml.AutoSize = True
    Me.rbHtml.Location = New System.Drawing.Point(139, 65)
    Me.rbHtml.Name = "rbHtml"
    Me.rbHtml.Size = New System.Drawing.Size(115, 17)
    Me.rbHtml.TabIndex = 1
    Me.rbHtml.TabStop = True
    Me.rbHtml.Tag = "HTML (*.html)|*.html"
    Me.rbHtml.Text = "HTML (Web Page)"
    Me.rbHtml.UseVisualStyleBackColor = True
    '
    'rbPostscript
    '
    Me.rbPostscript.AutoSize = True
    Me.rbPostscript.Location = New System.Drawing.Point(6, 19)
    Me.rbPostscript.Name = "rbPostscript"
    Me.rbPostscript.Size = New System.Drawing.Size(71, 17)
    Me.rbPostscript.TabIndex = 0
    Me.rbPostscript.TabStop = True
    Me.rbPostscript.Tag = "PostScript (*.ps)|*.ps"
    Me.rbPostscript.Text = "Postscript"
    Me.rbPostscript.UseVisualStyleBackColor = True
    '
    'GroupBox2
    '
    Me.GroupBox2.Controls.Add(Me.nuDown)
    Me.GroupBox2.Controls.Add(Me.Label2)
    Me.GroupBox2.Controls.Add(Me.nuStart)
    Me.GroupBox2.Controls.Add(Me.Label1)
    Me.GroupBox2.Location = New System.Drawing.Point(12, 189)
    Me.GroupBox2.Name = "GroupBox2"
    Me.GroupBox2.Size = New System.Drawing.Size(260, 57)
    Me.GroupBox2.TabIndex = 1
    Me.GroupBox2.TabStop = False
    Me.GroupBox2.Text = "Page Range"
    '
    'nuDown
    '
    Me.nuDown.Location = New System.Drawing.Point(168, 24)
    Me.nuDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
    Me.nuDown.Name = "nuDown"
    Me.nuDown.Size = New System.Drawing.Size(48, 20)
    Me.nuDown.TabIndex = 3
    Me.nuDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
    '
    'Label2
    '
    Me.Label2.AutoSize = True
    Me.Label2.Location = New System.Drawing.Point(120, 26)
    Me.Label2.Name = "Label2"
    Me.Label2.Size = New System.Drawing.Size(43, 13)
    Me.Label2.TabIndex = 2
    Me.Label2.Text = "to page"
    '
    'nuStart
    '
    Me.nuStart.Location = New System.Drawing.Point(65, 24)
    Me.nuStart.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
    Me.nuStart.Name = "nuStart"
    Me.nuStart.Size = New System.Drawing.Size(48, 20)
    Me.nuStart.TabIndex = 1
    Me.nuStart.Value = New Decimal(New Integer() {1, 0, 0, 0})
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(6, 26)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(57, 13)
    Me.Label1.TabIndex = 0
    Me.Label1.Text = "From page"
    '
    'btOK
    '
    Me.btOK.Location = New System.Drawing.Point(197, 253)
    Me.btOK.Name = "btOK"
    Me.btOK.Size = New System.Drawing.Size(75, 23)
    Me.btOK.TabIndex = 2
    Me.btOK.Text = "OK"
    Me.btOK.UseVisualStyleBackColor = True
    '
    'btCancel
    '
    Me.btCancel.Location = New System.Drawing.Point(116, 253)
    Me.btCancel.Name = "btCancel"
    Me.btCancel.Size = New System.Drawing.Size(75, 23)
    Me.btCancel.TabIndex = 3
    Me.btCancel.Text = "Cancel"
    Me.btCancel.UseVisualStyleBackColor = True
    '
    'GroupBox3
    '
    Me.GroupBox3.Controls.Add(Me.Label4)
    Me.GroupBox3.Controls.Add(Me.Label3)
    Me.GroupBox3.Controls.Add(Me.nuDPI)
    Me.GroupBox3.Location = New System.Drawing.Point(12, 132)
    Me.GroupBox3.Name = "GroupBox3"
    Me.GroupBox3.Size = New System.Drawing.Size(260, 53)
    Me.GroupBox3.TabIndex = 4
    Me.GroupBox3.TabStop = False
    Me.GroupBox3.Text = "Image Options"
    '
    'Label4
    '
    Me.Label4.AutoSize = True
    Me.Label4.Location = New System.Drawing.Point(129, 24)
    Me.Label4.Name = "Label4"
    Me.Label4.Size = New System.Drawing.Size(25, 13)
    Me.Label4.TabIndex = 2
    Me.Label4.Text = "DPI"
    '
    'Label3
    '
    Me.Label3.AutoSize = True
    Me.Label3.Location = New System.Drawing.Point(6, 24)
    Me.Label3.Name = "Label3"
    Me.Label3.Size = New System.Drawing.Size(60, 13)
    Me.Label3.TabIndex = 1
    Me.Label3.Text = "Resolution:"
    '
    'nuDPI
    '
    Me.nuDPI.Location = New System.Drawing.Point(66, 22)
    Me.nuDPI.Maximum = New Decimal(New Integer() {1200, 0, 0, 0})
    Me.nuDPI.Minimum = New Decimal(New Integer() {72, 0, 0, 0})
    Me.nuDPI.Name = "nuDPI"
    Me.nuDPI.Size = New System.Drawing.Size(58, 20)
    Me.nuDPI.TabIndex = 0
    Me.nuDPI.Value = New Decimal(New Integer() {72, 0, 0, 0})
    '
    'ExportOptions
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(284, 282)
    Me.Controls.Add(Me.GroupBox3)
    Me.Controls.Add(Me.btCancel)
    Me.Controls.Add(Me.btOK)
    Me.Controls.Add(Me.GroupBox2)
    Me.Controls.Add(Me.GroupBox1)
    Me.Name = "ExportOptions"
    Me.Text = "Export PDF Options"
    Me.GroupBox1.ResumeLayout(False)
    Me.GroupBox1.PerformLayout()
    Me.GroupBox2.ResumeLayout(False)
    Me.GroupBox2.PerformLayout()
    CType(Me.nuDown, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.nuStart, System.ComponentModel.ISupportInitialize).EndInit()
    Me.GroupBox3.ResumeLayout(False)
    Me.GroupBox3.PerformLayout()
    CType(Me.nuDPI, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents rbJpeg As System.Windows.Forms.RadioButton
  Friend WithEvents rbText As System.Windows.Forms.RadioButton
  Friend WithEvents rbHtml As System.Windows.Forms.RadioButton
  Friend WithEvents rbPostscript As System.Windows.Forms.RadioButton
  Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
  Friend WithEvents nuDown As System.Windows.Forms.NumericUpDown
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents nuStart As System.Windows.Forms.NumericUpDown
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents btOK As System.Windows.Forms.Button
  Friend WithEvents btCancel As System.Windows.Forms.Button
  Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
  Friend WithEvents rbTIFF As System.Windows.Forms.RadioButton
  Friend WithEvents rbPNG As System.Windows.Forms.RadioButton
  Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
  Friend WithEvents Label3 As System.Windows.Forms.Label
  Friend WithEvents nuDPI As System.Windows.Forms.NumericUpDown
  Friend WithEvents Label4 As System.Windows.Forms.Label
  Friend WithEvents rbHtmlImage As System.Windows.Forms.RadioButton
End Class
