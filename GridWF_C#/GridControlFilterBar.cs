using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using Syncfusion.Windows.Forms.Grid;

namespace GridPopulationSample
{
	/// <summary>
	/// Summary description for GridControlFilterBar.
	/// </summary>
	public class GridControlFilterBar
	{
		private GridControl _grid;
		private DataView filterView;
		private DataView originalView;
		public int RowCount;
		public int FilterRowIndex = 1;

		public void WireGrid(GridControl grid, DataTable dt)
		{
			this._grid = grid;
			this.originalView = dt.DefaultView;
			this.filterView = new DataView(dt);
			RowCount = dt.Rows.Count + 1;
			_grid.Model.Rows.FrozenCount += 1;
			if(this._grid != null)
			{
				this._grid.CurrentCellAcceptedChanges += new CancelEventHandler(_grid_CurrentCellAcceptedChanges);
				this._grid.CurrentCellCloseDropDown += new Syncfusion.Windows.Forms.PopupClosedEventHandler(_grid_CurrentCellCloseDropDown);
				this._grid.QueryCellInfo += new GridQueryCellInfoEventHandler(_grid_QueryCellInfo);
				_grid.Refresh();
			}
		}

		bool inUnwire = false;
		public void UnwireGrid()
		{
			this._grid.CurrentCellAcceptedChanges -= new CancelEventHandler(_grid_CurrentCellAcceptedChanges);
			this._grid.CurrentCellCloseDropDown -= new Syncfusion.Windows.Forms.PopupClosedEventHandler(_grid_CurrentCellCloseDropDown);
			this._grid.QueryCellInfo -= new GridQueryCellInfoEventHandler(_grid_QueryCellInfo);
			originalView.RowFilter = "";
			this.RowCount = originalView.Table.Rows.Count;
			_grid.Model.Rows.FrozenCount -= 1;
			inUnwire = true;
			for(int i = 1;i<_grid.ColCount;i++)
				_grid[FilterRowIndex,i].Text ="";
			_grid.Refresh();
			this._grid = null;
			inUnwire = false;
		}
		public bool IsWired
		{
			get{return (this._grid!= null)&&!inUnwire;}
		}
		protected virtual DataTable CreateUniqueEntries(string colName)
		{
			DataRow row1;
			DataTable table1 = new DataTable(colName);
			table1.Columns.Add(new DataColumn(colName));
			row1 = table1.NewRow();
			row1[0] = "[None]";
			table1.Rows.Add(row1);
			string text1 = "";
			ArrayList tempArray = new ArrayList();
			filterView.Sort = colName +" ASC";
			for (int num1 = 0; num1 < filterView.Count; num1++)
			{
				text1 = filterView[num1].Row[colName].ToString();
				if(tempArray.Count==0 || !tempArray.Contains(text1))
				{
					row1 = table1.NewRow();
					row1[0] = text1;
					tempArray.Add(text1);
					table1.Rows.Add(row1);
				}
			}
			return table1;
		}

		ArrayList filters = new ArrayList();
		struct filter
		{
			public string colname,filterString;
			public filter(string colname, string filterString)
			{
				this.colname = colname;
				this.filterString = filterString;
			}
		}

		public void SetFilters()
		{
			string FilterString = "";
			foreach(filter fil in filters)
			{
				if(filters.IndexOf(fil)>0)
					FilterString += " AND ";

				FilterString += "["+fil.colname+"] = "+fil.filterString;
			}
			originalView.RowFilter = FilterString;
			RowCount = originalView.Count+1;
			_grid.Refresh();
		}

		#region Event Handlers
		private void _grid_CurrentCellAcceptedChanges(object sender, CancelEventArgs e)
		{
			GridCurrentCell cc = this._grid.CurrentCell;
			if(cc.ColIndex>0 && cc.RowIndex ==1)
			{
				foreach(filter fil in filters)
				{
					if(fil.colname == originalView.Table.Columns[cc.ColIndex - 1].ColumnName)
					{
						filters.Remove(fil);
						break;
					}
				}
				if(cc.Renderer.StyleInfo.Text != "[None]")
					filters.Add(new filter(originalView.Table.Columns[cc.ColIndex - 1].ColumnName,"'" + cc.Renderer.StyleInfo.Text + "'"));
				SetFilters();
			}

		}

		private void _grid_CurrentCellCloseDropDown(object sender, Syncfusion.Windows.Forms.PopupClosedEventArgs e)
		{
			GridCurrentCell cc = this._grid.CurrentCell;
			if(cc.ColIndex>0 && cc.RowIndex ==1)
				cc.ConfirmChanges();

		}


		private void _grid_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
		{
			if(e.ColIndex>0 && e.RowIndex == FilterRowIndex)
			{
				e.Style.CellType = GridCellTypeName.ComboBox;
				e.Style.ExclusiveChoiceList = true;
				e.Style.DataSource = CreateUniqueEntries(originalView.Table.Columns[e.ColIndex - 1].ColumnName);
				e.Style.ValueMember = originalView.Table.Columns[e.ColIndex - 1].ColumnName;
			}

		}
		#endregion
	}

}
