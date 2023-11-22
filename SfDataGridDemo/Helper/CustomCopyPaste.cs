using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using Syncfusion.UI.Xaml.Grids;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataGridDemo
{    
    public class CustomCopyPaste : DataGridClipboardController
    {
        private SfDataGrid dataGrid;
        public CustomCopyPaste(SfDataGrid sfgrid)
            : base(sfgrid)
        {
            dataGrid = sfgrid;
        }

        protected override void PasteToRow(object clipboardcontent, object selectedRecords)
        {
            if (dataGrid.SelectionUnit == GridSelectionUnit.Row)
                base.PasteToRow(clipboardcontent, selectedRecords);
            else
            {
                //Get the copied value.
                clipboardcontent = Regex.Split(clipboardcontent.ToString(), @"\t");
                var copyValue = (string[])clipboardcontent;

                int cellcount = copyValue.Count();
                var selectionContoller = this.dataGrid.SelectionController as GridCellSelectionController;
                var lastselectedindex = selectionContoller.CurrentCellManager.CurrentRowColumnIndex.ColumnIndex;
                //Get the PressedRowColumnIndex value using reflection.
                var Propertyinfo = (this.dataGrid.SelectionController as GridCellSelectionController).GetType().GetProperty("PressedRowColumnIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var pressedrowcolumnindex = Propertyinfo.GetValue(this.dataGrid.SelectionController);
                var pressedindex = ((RowColumnIndex)(pressedrowcolumnindex)).ColumnIndex;
                var pastecolumnindex = pressedindex < lastselectedindex ? pressedindex : lastselectedindex;

                int columnindex = 0;
                var columnstartindex = this.dataGrid.ResolveToGridVisibleColumnIndex(pastecolumnindex);
                for (int i = columnstartindex; i < cellcount + columnstartindex; i++)
                {
                    if (dataGrid.PasteOption.HasFlag(GridPasteOptions.IncludeHiddenColumn))
                    {
                        if (dataGrid.Columns.Count <= i)
                            break;
                        PasteToCell(selectedRecords, dataGrid.Columns[i], copyValue[columnindex]);
                        columnindex++;
                    }
                    else
                    {
                        if (dataGrid.Columns.Count <= i)
                            break;
                        //Paste the copied value here include empty string value.
                        if (!dataGrid.Columns[i].IsHidden)
                        {
                            PasteToCell(selectedRecords, dataGrid.Columns[i], copyValue[columnindex]);
                            columnindex++;
                        }
                        else
                            cellcount++;
                    }
                }
            }
        }
    }
}