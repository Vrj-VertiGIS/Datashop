using netDxf.Header;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI
{
    public class CmbDxfVersionItem
    {
        private DxfVersion _item;
        public DxfVersion DxfVersion { get => _item; set => _item = value; }
        public CmbDxfVersionItem(DxfVersion dxfVersion)
        {
            _item = dxfVersion;
        }
        public override string ToString()
        {
            return DxfVersion.ToString();
        }

        public bool IsSupported => _item >= DxfVersion.AutoCad2000;

    }

}
