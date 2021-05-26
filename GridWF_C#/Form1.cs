#region Copyright Syncfusion Inc. 2001 - 2005
//
//  Copyright Syncfusion Inc. 2001 - 2005. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.ComponentModel;

namespace GridPopulationSample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private Syncfusion.Windows.Forms.Grid.GridControl gridControl1;

		private DataTable dt;
		private int numArrayRows;
		private int numArrayCols;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.SetUpData();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gridControl1 = new Syncfusion.Windows.Forms.Grid.GridControl();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// gridControl1
			// 
			this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridControl1.DefaultColWidth = 75;
			this.gridControl1.DefaultGridBorderStyle = Syncfusion.Windows.Forms.Grid.GridBorderStyle.Solid;
			this.gridControl1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.gridControl1.Location = new System.Drawing.Point(8, 8);
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.gridControl1.RowCount = 1000;
			this.gridControl1.RowHeightEntries.AddRange(new Syncfusion.Windows.Forms.Grid.GridRowHeight[] {
																											  new Syncfusion.Windows.Forms.Grid.GridRowHeight(0, 24)});
			this.gridControl1.Size = new System.Drawing.Size(808, 240);
			this.gridControl1.SmartSizeBox = false;
			this.gridControl1.TabIndex = 0;
			this.gridControl1.ThemesEnabled = true;
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.button1.Location = new System.Drawing.Point(328, 264);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(128, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Wire/Unwire FilterBar";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(824, 294);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.gridControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main() 
		{
			Application.Run(new Form1());
		}


		private void SetUpData()
		{
			this.Cursor = Cursors.WaitCursor;
			this.numArrayCols = 10;
			this.numArrayRows = 10;

			this.dt = new DataTable();
			for(int i =0;i<this.numArrayCols;i++)
			{
				DataColumn col = new DataColumn(string.Format("Column {0}",Convert.ToChar(65+i)));
				dt.Columns.Add(col);
			}

			Random r = new Random();
			for (int i = 0; i < this.numArrayRows; ++i)
			{
				DataRow row= dt.NewRow();
				for(int j = 0; j < this.numArrayCols; ++j)
				{
					int ch = r.Next(87);
					ch = ch>65?ch:65;
					row[j] = (Convert.ToChar(ch));
				}
				this.dt.Rows.Add(row);
			}
			this.Cursor = Cursors.Arrow;
		}


		
		GridControlFilterBar filterBar;
		
		private void Form1_Load(object sender, System.EventArgs e)
		{
			filterBar = new GridControlFilterBar();
			filterBar.WireGrid(this.gridControl1,this.dt);

			this.gridControl1.ResetVolatileData();

			this.gridControl1.QueryCellInfo += new GridQueryCellInfoEventHandler(QueryCellInfoHandler);
			this.gridControl1.QueryColCount += new GridRowColCountEventHandler(GridQueryColCount);
			this.gridControl1.QueryRowCount += new GridRowColCountEventHandler(GridQueryRowCount);
			this.gridControl1.SaveCellInfo += new GridSaveCellInfoEventHandler(SaveCellInfoHandler);
						
			for(int i = 1;i<=10;i++)
				this.gridControl1[0,i].Text = string.Format("Column {0}",Convert.ToChar(64+i));
			
			this.gridControl1.Refresh();
		}


		#region Virtual Mode

		private void QueryCellInfoHandler(object sender, GridQueryCellInfoEventArgs e)
		{
			int filterCheck = 0;
			if(filterBar.IsWired)
				filterCheck = 1;
			if(e.ColIndex > 0 && e.RowIndex > 0 && e.RowIndex != filterCheck)
			{
				e.Style.CellValue = this.dt.DefaultView[e.RowIndex-(1+filterCheck)][e.ColIndex - 1];
				e.Handled = true;
			}	
			else
				if(e.ColIndex == 0 && e.RowIndex>0)
			{
				e.Style.Text = " ";
			}
		}
		private void SaveCellInfoHandler(object sender, GridSaveCellInfoEventArgs e)
		{
			if(e.ColIndex > 0 && e.RowIndex > 1)
			{
				this.dt.DefaultView[e.RowIndex-2][e.ColIndex - 1] = e.Style.CellValue;
				e.Handled = true;
			}
		}
		private void GridQueryColCount(object sender, GridRowColCountEventArgs e)
		{
			e.Count = this.numArrayCols;
			e.Handled = true;
		}

		private void GridQueryRowCount(object sender, GridRowColCountEventArgs e)
		{
			e.Count = filterBar.RowCount;
			e.Handled = true;
		}
	
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			if(this.filterBar.IsWired)
				this.filterBar.UnwireGrid();
			else
				this.filterBar.WireGrid(this.gridControl1,this.dt);
			
		}
	
	}

}
