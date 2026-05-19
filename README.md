# How to add filter in WinForms GridControl?

This sample illustrates how to add filter in WinForms GridControl.

There is no built-in support for having a filter bar in a [WinForms GridControl](https://www.syncfusion.com/winforms-ui-controls/grid-control), because the GridFilterBar uses DataView’s RowFilter property for filtering.

If you are using the GridControl in virtual mode with a data table then the filter bar function can be implemented. In the given sample, the GridControlFilterBar is created to enable filter bar in grid control.

### Creating the GridControlFilterBar:

``` csharp
public class GridControlFilterBar
{
    private GridControl _grid;
    private DataView filterView;
    private DataView originalView;
    public int RowCount;
    public int FilterRowIndex = 1;
   
    //Wire the Grid with the filterbar
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
 
    //Unwire the Grid from the filter 
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
 
    //Return the Grid is wired or not.
    public bool IsWired
    {
        get{return (this._grid!= null)&&!inUnwire;}
    }
 
    //Data table for creating a unique entries 
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
 
    //Filter collection
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
 
    //Setting the filter condition 
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
}
```

### Event Handlers for the GridControlFilterBar:

``` csharp
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
```

### Wiring the Grid with the GridControlFilterBar:

``` csharp
GridControlFilterBar filterBar;
private void Form1_Load(object sender, System.EventArgs e)
{
    //Creating the object for GridControlFilterBar 
    filterBar = new GridControlFilterBar();
    filterBar.WireGrid(this.gridControl1,this.dt);
 
    //Hook the event to wire/unwire the grid from filter
    this.btnWire.Click += btnWire_Click;   
}
void btnWire_Click(object sender, EventArgs e)
{
    //Wire/unwire the Grid from the filter bar
    if (this.filterBar.IsWired)
        this.filterBar.UnwireGrid();
    else
        this.filterBar.WireGrid(this.gridControl1, this.dt);
}
```