namespace GEOCOM.GNSDatashop.Export.DXF.Interface
{
    public interface IDxfWriterOptions
    {
        /// <summary>
        /// Use the arcmap selection. Intersect selected features with other selection criteria
        /// (i.e. selection layer)
        /// </summary>
        bool UseSelectionSet { get; set; }

        /// <summary>
        /// Reference scale of the map - the scale wich defines the size of the
        /// symbols (i.e. a 12 dot character will be printet 12 dots hight
        /// when the map is printet at that scale. Used to translate symbol
        /// sizes from dots (font size, line size,...) to dxf units (meters).
        /// </summary>
        double ReferenceScale { get; set; }

        /// <summary>
        /// The current scale of the map
        /// </summary>
        double CurrentScale { get; set; }

        /// <summary>
        /// Write the binary variant 
        /// </summary>
        bool BinaryDXF { get; set; }
    }
}
