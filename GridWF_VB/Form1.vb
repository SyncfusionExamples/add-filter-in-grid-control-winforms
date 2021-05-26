Imports Microsoft.VisualBasic
#Region "Copyright Syncfusion Inc. 2001 - 2005"
'
'  Copyright Syncfusion Inc. 2001 - 2005. All rights reserved.
'
'  Use of this code is subject to the terms of our license.
'  A copy of the current license can be obtained at any time by e-mailing
'  licensing@syncfusion.com. Any infringement will be prosecuted under
'  applicable laws. 
'
#End Region

Imports System
Imports System.Drawing
Imports System.Collections
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports Syncfusion.Windows.Forms.Grid
Imports Syncfusion.ComponentModel

Namespace GridPopulationSample
	''' <summary>
	''' Summary description for Form1.
	''' </summary>
	Public Class Form1
		Inherits System.Windows.Forms.Form
		Private gridControl1 As Syncfusion.Windows.Forms.Grid.GridControl

		Private dt As DataTable
		Private numArrayRows As Integer
		Private numArrayCols As Integer
		Private WithEvents button1 As System.Windows.Forms.Button
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing

		Public Sub New()
			'
			' Required for Windows Form Designer support
			'
			InitializeComponent()

			Me.SetUpData()

		End Sub

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If components IsNot Nothing Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"
		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.gridControl1 = New Syncfusion.Windows.Forms.Grid.GridControl()
			Me.button1 = New System.Windows.Forms.Button()
			CType(Me.gridControl1, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			' 
			' gridControl1
			' 
			Me.gridControl1.Anchor = (CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
			Me.gridControl1.DefaultColWidth = 75
			Me.gridControl1.DefaultGridBorderStyle = Syncfusion.Windows.Forms.Grid.GridBorderStyle.Solid
			Me.gridControl1.ForeColor = System.Drawing.SystemColors.ControlText
			Me.gridControl1.Location = New System.Drawing.Point(8, 8)
			Me.gridControl1.Name = "gridControl1"
			Me.gridControl1.RightToLeft = System.Windows.Forms.RightToLeft.No
			Me.gridControl1.RowCount = 1000
			Me.gridControl1.RowHeightEntries.AddRange(New Syncfusion.Windows.Forms.Grid.GridRowHeight() { New Syncfusion.Windows.Forms.Grid.GridRowHeight(0, 24)})
			Me.gridControl1.Size = New System.Drawing.Size(808, 240)
			Me.gridControl1.SmartSizeBox = False
			Me.gridControl1.TabIndex = 0
			Me.gridControl1.ThemesEnabled = True
			' 
			' button1
			' 
			Me.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.button1.Location = New System.Drawing.Point(328, 264)
			Me.button1.Name = "button1"
			Me.button1.Size = New System.Drawing.Size(128, 23)
			Me.button1.TabIndex = 1
			Me.button1.Text = "Wire/Unwire FilterBar"
'			Me.button1.Click += New System.EventHandler(Me.button1_Click)
			' 
			' Form1
			' 
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(824, 294)
			Me.Controls.Add(Me.button1)
			Me.Controls.Add(Me.gridControl1)
			Me.Name = "Form1"
			Me.Text = "Form1"
'			Me.Load += New System.EventHandler(Me.Form1_Load)
			CType(Me.gridControl1, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub
		#End Region

		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		<STAThread> _
		Public Shared Sub Main()
			Application.Run(New Form1())
		End Sub


		Private Sub SetUpData()
			Me.Cursor = Cursors.WaitCursor
			Me.numArrayCols = 10
			Me.numArrayRows = 10

			Me.dt = New DataTable()
			For i As Integer = 0 To Me.numArrayCols - 1
				Dim col As New DataColumn(String.Format("Column {0}",Convert.ToChar(65+i)))
				dt.Columns.Add(col)
			Next i

			Dim r As New Random()
			For i As Integer = 0 To Me.numArrayRows - 1
				Dim row As DataRow= dt.NewRow()
				For j As Integer = 0 To Me.numArrayCols - 1
					Dim ch As Integer = r.Next(87)
					ch = If(ch>65, ch, 65)
					row(j) = (Convert.ToChar(ch))
				Next j
				Me.dt.Rows.Add(row)
			Next i
			Me.Cursor = Cursors.Arrow
		End Sub



		Private filterBar As GridControlFilterBar

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			filterBar = New GridControlFilterBar()
			filterBar.WireGrid(Me.gridControl1,Me.dt)

			Me.gridControl1.ResetVolatileData()

			AddHandler gridControl1.QueryCellInfo, AddressOf QueryCellInfoHandler
			AddHandler gridControl1.QueryColCount, AddressOf GridQueryColCount
			AddHandler gridControl1.QueryRowCount, AddressOf GridQueryRowCount
			AddHandler gridControl1.SaveCellInfo, AddressOf SaveCellInfoHandler

			For i As Integer = 1 To 10
				Me.gridControl1(0,i).Text = String.Format("Column {0}",Convert.ToChar(64+i))
			Next i

			Me.gridControl1.Refresh()
		End Sub


		#Region "Virtual Mode"

		Private Sub QueryCellInfoHandler(ByVal sender As Object, ByVal e As GridQueryCellInfoEventArgs)
			Dim filterCheck As Integer = 0
			If filterBar.IsWired Then
				filterCheck = 1
			End If
			If e.ColIndex > 0 AndAlso e.RowIndex > 0 AndAlso e.RowIndex <> filterCheck Then
				e.Style.CellValue = Me.dt.DefaultView(e.RowIndex-(1+filterCheck))(e.ColIndex - 1)
				e.Handled = True
			Else
				If e.ColIndex = 0 AndAlso e.RowIndex>0 Then
				e.Style.Text = " "
				End If
			End If
		End Sub
		Private Sub SaveCellInfoHandler(ByVal sender As Object, ByVal e As GridSaveCellInfoEventArgs)
			If e.ColIndex > 0 AndAlso e.RowIndex > 1 Then
				Me.dt.DefaultView(e.RowIndex-2)(e.ColIndex - 1) = e.Style.CellValue
				e.Handled = True
			End If
		End Sub
		Private Sub GridQueryColCount(ByVal sender As Object, ByVal e As GridRowColCountEventArgs)
			e.Count = Me.numArrayCols
			e.Handled = True
		End Sub

		Private Sub GridQueryRowCount(ByVal sender As Object, ByVal e As GridRowColCountEventArgs)
			e.Count = filterBar.RowCount
			e.Handled = True
		End Sub

		#End Region

		Private Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles button1.Click
			If Me.filterBar.IsWired Then
				Me.filterBar.UnwireGrid()
			Else
				Me.filterBar.WireGrid(Me.gridControl1,Me.dt)
			End If

		End Sub

	End Class

End Namespace
