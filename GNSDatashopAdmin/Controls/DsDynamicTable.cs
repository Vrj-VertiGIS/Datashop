using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml.Serialization;

namespace GNSDatashopAdmin.Controls
{

    public class DsDynamicTable : Panel 
    {
        public DsDynamicTable(DsDynamicTableConfig tableConfig, DsDynamicReportData data)
        {
            if (!string.IsNullOrEmpty(tableConfig.Id))
                this.ID = tableConfig.Id;
            Attributes.Add("class", (string.IsNullOrEmpty(tableConfig.FrameCssClass) ? "DSDRTableFrame" : tableConfig.FrameCssClass));
            if (!string.IsNullOrEmpty(tableConfig.FrameCaption))
            {
                var caption = new Label();
                caption.CssClass = "DSDRFrameCaption";
                caption.Text = tableConfig.FrameCaption;
                Controls.Add(caption);
            }

            if (tableConfig.Rows != null)
            {
                var table = new HtmlTable();
                this.Controls.Add(table);
                tableConfig.CheckDefault("DSDRPanelTable");
                table.Attributes.Add("class", tableConfig.CssClass);
                foreach (DsDynamicTableRowConfig rowConfig in tableConfig.Rows)
                {
                    rowConfig.CheckDefault("DSDRPanelTableRow");
                    var row1 = new HtmlTableRow();
                    row1.Attributes.Add("class", rowConfig.CssClassRow1);
                    table.Rows.Add(row1);
                    var row2 = row1;
                    if (tableConfig.Layout == DsDynamicTableLayout.Vertical)
                    {
                        row2 = new HtmlTableRow();
                        row2.Attributes.Add("class", rowConfig.CssClassRow2);
                        table.Rows.Add(row2);
                    }
                    if (rowConfig.Cells != null)
                    {
                        foreach (DsDynamicTableCellConfig cellConfig in rowConfig.Cells)
                        {
                            cellConfig.CheckDefault("DSDRPanelTableCellCaption", "DSDRPanelTableCellVariable");
                            var cellCaption = new HtmlTableCell();
                            cellCaption.Attributes.Add("class", cellConfig.CaptionCssClass);
                            cellCaption.InnerText = cellConfig.Caption;
                            row1.Cells.Add(cellCaption);
                            var cellVariable = new HtmlTableCell();
                            if (!string.IsNullOrEmpty(cellConfig.Id))
                                cellVariable.ID = cellConfig.Id;
                            cellVariable.Attributes.Add("class", cellConfig.VariableCssClass);
                            if (!string.IsNullOrEmpty(cellConfig.VariableId))
                            {
                                if (data.ContainsKey(cellConfig.VariableId))
                                {
                                    var variable = data[cellConfig.VariableId];
                                    cellVariable.Attributes.Add("wrap", "true");
                                    var celltext = string.Format(cellConfig.VariableFormatString, variable);
                                    cellVariable.InnerText = celltext.Replace(";", "; ");
                                }
                            }
                            cellVariable.InnerHtml += "&nbsp;"; // to preserve cell borders if any
                            row2.Cells.Add(cellVariable);
                            // colspan
                            if (cellConfig.Colspan > 1)
                            {
                                if (tableConfig.Layout == DsDynamicTableLayout.Vertical)
                                {
                                    var colspan = cellConfig.Colspan.ToString();
                                    cellCaption.Attributes.Add("colspan", colspan);
                                    cellVariable.Attributes.Add("colspan", colspan);
                                }
                                else
                                {
                                    var colspan = string.Format("{0}", cellConfig.Colspan*2 - 1);
                                    cellVariable.Attributes.Add("colspan", colspan);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
