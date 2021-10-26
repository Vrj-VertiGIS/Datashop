using System;
using System.Xml.Serialization;

namespace GNSDatashopAdmin.Controls
{
    using System.Diagnostics.CodeAnalysis;

    public enum DsDynamicReportContainerType
    {
        Inline,

        Tabbed
    }

    [Serializable]
    public class DsDynamicReportConfig
    {
        /// <summary>
        /// In a report, the panels are either tabbed or inline
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        [XmlAttribute]
        public DsDynamicReportContainerType ContainerType = DsDynamicReportContainerType.Inline;

        /// <summary>
        /// The CSS class for the overall container when inline layout.
        /// Default value is DSDRContainerInline
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        [XmlAttribute]
        public string ContainerCssClass = "DSDRContainerInline";

        /// <summary>
        /// The CSS class for for the overall container when tabbed layout
        /// Default value is DSDRTabs
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "Reviewed. Suppression is OK here.")]
        [XmlAttribute]
        public string HeaderCssClass = "DSDRTabs";

        /// <summary>
        /// The collection of panels (should not be empty...)
        /// </summary>
        [XmlArray(IsNullable = false)]
        [XmlArrayItem("Panel")]
        public DsDynamicPanelConfig[] Panels { get; set; }
    }

    public class DsDynamicPanelConfig
    {
        /// <summary>
        /// An optional ID that makes the panel visible outside the report (using the FindControlRecursive report method
        /// </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>
        /// The panel title
        /// </summary>
        [XmlAttribute]
        public string HeadCaption { get; set; }

        /// <summary>
        /// The CSS class for the title when the panel is not collapsable
        /// Default value is DSDRInlineHead or DSDRTab
        /// </summary>
        [XmlAttribute]
        public string HeadCssClass { get; set; }

        /// <summary>
        /// The CSS class for the title when the panel is collapsed
        /// Default value is DSDRInlineCollapsed or DSDRTabUnselected
        /// </summary>
        [XmlAttribute]
        public string HeadCollapsedCssClass { get; set; }

        /// <summary>
        /// The CSS class for the title when the panel is expanded
        /// Default value is DSDRInlineExpanded or DSDRTabSelected
        /// </summary>
        [XmlAttribute]
        public string HeadExpandedCssClass { get; set; }

        /// <summary>
        /// This is not used anymore since we use anchors instead of divs
        /// Default value is DSDRInlineHover or DSDRTabHover
        /// </summary>
        [XmlAttribute]
        public string HeadHoverCssClass { get; set; }

        /// <summary>
        /// The CSS class for the panel's body
        /// Default value is DSDRInlinePanel or DSDRTabbedPanel
        /// </summary>
        [XmlAttribute]
        public string BodyCssClass { get; set; }

        /// <summary>
        /// Panel's body is collapsable (irrelevant when tabbed panels)
        /// </summary>
        [XmlAttribute]
        public bool BodyIsCollapsable { get; set; }

        /// <summary>
        ///  The panel is collapsed at load time (irrelevant when tabbed panels)
        /// </summary>
        [XmlAttribute]
        public bool BodyIsCollapsed { get; set; }

        /// <summary>
        /// Optional script that will be carried out when the panel is expanded
        /// </summary>
        [XmlAttribute]
        public string OnClientExpand { get; set; }

        /// <summary>
        /// The panel consists of zero, one or more tables
        /// </summary>
        [XmlArray]
        [XmlArrayItem("Table")]
        public DsDynamicTableConfig[] Tables { get; set; }
    }

    /// <summary>
    /// The TableLAyout controls how the variable captions are positionned relatively to the variables
    ///  - vertical : the caption is on top of the variable
    ///  - horizontal : the caption is on the left of the variable
    /// </summary>
    public enum DsDynamicTableLayout
    {
        Vertical,
        Horizontal
    }

    public class DsDynamicTableConfig
    {
        /// <summary>
        /// An optional ID that makes the panel visible outside the report (using the FindControlRecursive report method
        /// </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>
        /// An optional title for the table.
        /// </summary>
        [XmlAttribute]
        public string FrameCaption { get; set; }

        /// <summary>
        /// The CSS class of the div encapsuiating the table (allows margins, padding, framing, etc)
        /// Default value is DSDRTableFrame
        /// </summary>
        [XmlAttribute]
        public string FrameCssClass { get; set; }

        /// <summary>
        /// The table layout can be either Vertical or Horizontal
        /// </summary>
        [XmlAttribute]
        public DsDynamicTableLayout Layout { get; set; }

        /// <summary>
        /// The CSS class of the table
        /// Default value is DSDRPanelTable
        /// </summary>
        [XmlAttribute]
        public string CssClass { get; set; }

        /// <summary>
        /// A table consists of rows (may also be empty if you just want to get a frame and a title and fill it with your own stuff)
        /// </summary>
        [XmlArray]
        [XmlArrayItem("Row")]
        public DsDynamicTableRowConfig[] Rows { get; set; }

        public void CheckDefault(string defaultCssClass)
        {
            if (string.IsNullOrEmpty(CssClass))
            {
                CssClass = defaultCssClass;
            }
        }
    }

    public class DsDynamicTableRowConfig
    {
        /// <summary>
        /// The Css class for the first row when vertical layout
        /// Default value is DSDRPanelTableRow
        /// </summary>
        [XmlAttribute]
        public string CssClassRow1 { get; set; }

        /// <summary>
        /// The Css class for the second row when vertical layout
        /// Default value is DSDRPanelTableRow
        /// </summary>
        [XmlAttribute]
        public string CssClassRow2 { get; set; }

        /// <summary>
        /// A row consists of cells
        /// </summary>
        [XmlArray]
        [XmlArrayItem("Cell")]
        public DsDynamicTableCellConfig[] Cells { get; set; }

        public void CheckDefault(string defaultCssClass)
        {
            if (string.IsNullOrEmpty(CssClassRow1))
            {
                CssClassRow1 = defaultCssClass;
            }
            if (string.IsNullOrEmpty(CssClassRow2))
            {
                CssClassRow2 = defaultCssClass;
            }
        }
    }

    public class DsDynamicTableCellConfig
    {
        /// <summary>
        /// An optional ID that makes the panel visible outside the report (using the FindControlRecursive report method
        /// </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>
        /// The title for the variable
        /// </summary>
        [XmlAttribute]
        public string Caption { get; set; }

        /// <summary>
        /// The CSS class for the title
        /// Default value is DSDRPanelTableCellCaption
        /// </summary>
        [XmlAttribute]
        public string CaptionCssClass { get; set; }

        /// <summary>
        /// The id of the variable. The list of the available ID's is provided by the container's designer
        /// If the variableId is not found, it is written in the variable cell.
        /// </summary>
        [XmlAttribute]
        public string VariableId { get; set; }

        /// <summary>
        /// You can optionally format the variable yourself (this needs to be changed)
        /// </summary>
        [XmlAttribute]
        public string VariableFormatString { get; set; }

        /// <summary>
        /// The CSS class for the variable
        /// Default value is DSDRPanelTableCellVariable
        /// </summary>
        [XmlAttribute]
        public string VariableCssClass { get; set; }

        /// <summary>
        /// The amount of cells the variable will span (depending on the table layout)
        /// </summary>
        [XmlAttribute]
        public int Colspan { get; set; }

        public void CheckDefault(string defaultCaptionCssClass, string defaultVariableCssClass)
        {
            if (string.IsNullOrEmpty(CaptionCssClass))
            {
                CaptionCssClass = defaultCaptionCssClass;
            }
            if (string.IsNullOrEmpty(VariableFormatString))
            {
                VariableFormatString = "{0}";
            }
            if (string.IsNullOrEmpty(VariableCssClass))
            {
                VariableCssClass = defaultVariableCssClass;
            }
        }
    }
}