using netDxf;
using netDxf.Blocks;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class BlockInfo 
    {
        private Block _block;
        private BoundingRectangle _extent;

        public BlockInfo(Block block, BoundingRectangle extent)
        {
            _block = block;
            _extent = extent;
        }

        public Block Block => _block;
        public BoundingRectangle Extent => _extent;
    }
}