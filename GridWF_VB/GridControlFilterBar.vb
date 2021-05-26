Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Data
Imports Syncfusion.Windows.Forms.Grid

Namespace GridPopulationSample
	''' <summary>
	''' Summary description for GridControlFilterBar.
	''' </summary>
	Public Class GridControlFilterBar
		Private _grid As GridControl
		Private filterView As DataView
		Private originalView As DataView
		Public RowCount As Integer
		Public FilterRowIndex As Integer = 1

		Public Sub WireGrid(ByVal grid As GridControl, ByVal dt As DataTable)
			Me._grid = grid
			Me.originalView = dt.DefaultView
			Me.filterView = New DataView(dt)
			RowCount = dt.Rows.Count + 1
			_grid.Model.Rows.FrozenCount += 1
			If Me._grid IsNot Nothing Then
				AddHandler _grid.CurrentCellAcceptedChanges, AddressOf _grid_CurrentCellAcceptedChanges
				AddHandler _grid.CurrentCellCloseDropDown, AddressOf _grid_CurrentCellCloseDropDown
				AddHandler _grid.QueryCellInfo, AddressOf _grid_QueryCellInfo
				_grid.Refresh()
			End If
		End Sub

		Private inUnwire As Boolean = False
		Public Sub UnwireGrid()
			RemoveHandler _grid.CurrentCellAcceptedChanges, AddressOf _grid_CurrentCellAcceptedChanges
			RemoveHandler _grid.CurrentCellCloseDropDown, AddressOf _grid_CurrentCellCloseDropDown
			RemoveHandler _grid.QueryCellInfo, AddressOf _grid_QueryCellInfo
			originalView.RowFilter = ""
			Me.RowCount = originalView.Table.Rows.Count
			_grid.Model.Rows.FrozenCount -= 1
			inUnwire = True
			For i As Integer = 1 To _grid.ColCount - 1
				_grid(FilterRowIndex,i).Text =""
			Next i
			_grid.Refresh()
			Me._grid = Nothing
			inUnwire = False
		End Sub
		Public ReadOnly Property IsWired() As Boolean
			Get
				Return (Me._grid IsNot Nothing) AndAlso Not inUnwire
			End Get
		End Property
		Protected Overridable Function CreateUniqueEntries(ByVal colName As String) As DataTable
			Dim row1 As DataRow
			Dim table1 As New DataTable(colName)
			table1.Columns.Add(New DataColumn(colName))
			row1 = table1.NewRow()
			row1(0) = "[None]"
			table1.Rows.Add(row1)
			Dim text1 As String = ""
			Dim tempArray As New ArrayList()
			filterView.Sort = colName &" ASC"
			For num1 As Integer = 0 To filterView.Count - 1
				text1 = filterView(num1).Row(colName).ToString()
				If tempArray.Count=0 OrElse (Not tempArray.Contains(text1)) Then
					row1 = table1.NewRow()
					row1(0) = text1
					tempArray.Add(text1)
					table1.Rows.Add(row1)
				End If
			Next num1
			Return table1
		End Function

		Private filters As New ArrayList()
		Private Structure filter
			Public colname, filterString As String
			Public Sub New(ByVal colname As String, ByVal filterString As String)
				Me.colname = colname
				Me.filterString = filterString
			End Sub
		End Structure

		Public Sub SetFilters()
			Dim FilterString As String = ""
			For Each fil As filter In filters
				If filters.IndexOf(fil)>0 Then
					FilterString &= " AND "
				End If

				FilterString &= "[" & fil.colname &"] = " & fil.filterString
			Next fil
			originalView.RowFilter = FilterString
			RowCount = originalView.Count+1
			_grid.Refresh()
		End Sub

		#Region "Event Handlers"
		Private Sub _grid_CurrentCellAcceptedChanges(ByVal sender As Object, ByVal e As CancelEventArgs)
			Dim cc As GridCurrentCell = Me._grid.CurrentCell
			If cc.ColIndex>0 AndAlso cc.RowIndex =1 Then
				For Each fil As filter In filters
					If fil.colname = originalView.Table.Columns(cc.ColIndex - 1).ColumnName Then
						filters.Remove(fil)
						Exit For
					End If
				Next fil
				If cc.Renderer.StyleInfo.Text <> "[None]" Then
					filters.Add(New filter(originalView.Table.Columns(cc.ColIndex - 1).ColumnName,"'" & cc.Renderer.StyleInfo.Text & "'"))
				End If
				SetFilters()
			End If

		End Sub

		Private Sub _grid_CurrentCellCloseDropDown(ByVal sender As Object, ByVal e As Syncfusion.Windows.Forms.PopupClosedEventArgs)
			Dim cc As GridCurrentCell = Me._grid.CurrentCell
			If cc.ColIndex>0 AndAlso cc.RowIndex =1 Then
				cc.ConfirmChanges()
			End If

		End Sub


		Private Sub _grid_QueryCellInfo(ByVal sender As Object, ByVal e As GridQueryCellInfoEventArgs)
			If e.ColIndex>0 AndAlso e.RowIndex = FilterRowIndex Then
				e.Style.CellType = GridCellTypeName.ComboBox
				e.Style.ExclusiveChoiceList = True
				e.Style.DataSource = CreateUniqueEntries(originalView.Table.Columns(e.ColIndex - 1).ColumnName)
				e.Style.ValueMember = originalView.Table.Columns(e.ColIndex - 1).ColumnName
			End If

		End Sub
		#End Region
	End Class

End Namespace
