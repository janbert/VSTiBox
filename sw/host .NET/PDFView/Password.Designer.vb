<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
<Global.System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726")> _
Partial Class Password
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
    Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents UsernameLabel As System.Windows.Forms.Label
  Friend WithEvents UsernameTextBox As System.Windows.Forms.TextBox
  Friend WithEvents OK As System.Windows.Forms.Button
    Friend WithEvents Cancel As System.Windows.Forms.Button

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Password))
    Me.LogoPictureBox = New System.Windows.Forms.PictureBox
    Me.UsernameLabel = New System.Windows.Forms.Label
    Me.UsernameTextBox = New System.Windows.Forms.TextBox
    Me.OK = New System.Windows.Forms.Button
    Me.Cancel = New System.Windows.Forms.Button
    Me.OwnerTextBox = New System.Windows.Forms.TextBox
    Me.Label1 = New System.Windows.Forms.Label
    CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'LogoPictureBox
    '
    Me.LogoPictureBox.Image = CType(resources.GetObject("LogoPictureBox.Image"), System.Drawing.Image)
    Me.LogoPictureBox.Location = New System.Drawing.Point(0, 3)
    Me.LogoPictureBox.Name = "LogoPictureBox"
    Me.LogoPictureBox.Size = New System.Drawing.Size(165, 144)
    Me.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
    Me.LogoPictureBox.TabIndex = 0
    Me.LogoPictureBox.TabStop = False
    '
    'UsernameLabel
    '
    Me.UsernameLabel.Location = New System.Drawing.Point(171, 9)
    Me.UsernameLabel.Name = "UsernameLabel"
    Me.UsernameLabel.Size = New System.Drawing.Size(220, 23)
    Me.UsernameLabel.TabIndex = 0
    Me.UsernameLabel.Text = "User Password"
    Me.UsernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'UsernameTextBox
    '
    Me.UsernameTextBox.Location = New System.Drawing.Point(171, 35)
    Me.UsernameTextBox.Name = "UsernameTextBox"
    Me.UsernameTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
    Me.UsernameTextBox.Size = New System.Drawing.Size(220, 20)
    Me.UsernameTextBox.TabIndex = 1
    '
    'OK
    '
    Me.OK.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.OK.Location = New System.Drawing.Point(297, 113)
    Me.OK.Name = "OK"
    Me.OK.Size = New System.Drawing.Size(94, 23)
    Me.OK.TabIndex = 4
    Me.OK.Text = "&OK"
    '
    'Cancel
    '
    Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.Cancel.Location = New System.Drawing.Point(197, 113)
    Me.Cancel.Name = "Cancel"
    Me.Cancel.Size = New System.Drawing.Size(94, 23)
    Me.Cancel.TabIndex = 5
    Me.Cancel.Text = "&Cancel"
    '
    'OwnerTextBox
    '
    Me.OwnerTextBox.Location = New System.Drawing.Point(171, 84)
    Me.OwnerTextBox.Name = "OwnerTextBox"
    Me.OwnerTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
    Me.OwnerTextBox.Size = New System.Drawing.Size(220, 20)
    Me.OwnerTextBox.TabIndex = 7
    '
    'Label1
    '
    Me.Label1.Location = New System.Drawing.Point(171, 58)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(220, 23)
    Me.Label1.TabIndex = 6
    Me.Label1.Text = "Owner Password"
    Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'Password
    '
    Me.AcceptButton = Me.OK
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.CancelButton = Me.Cancel
    Me.ClientSize = New System.Drawing.Size(401, 143)
    Me.Controls.Add(Me.OwnerTextBox)
    Me.Controls.Add(Me.Label1)
    Me.Controls.Add(Me.Cancel)
    Me.Controls.Add(Me.OK)
    Me.Controls.Add(Me.UsernameTextBox)
    Me.Controls.Add(Me.UsernameLabel)
    Me.Controls.Add(Me.LogoPictureBox)
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "Password"
    Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "Please Enter the Password"
    CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents OwnerTextBox As System.Windows.Forms.TextBox
  Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
