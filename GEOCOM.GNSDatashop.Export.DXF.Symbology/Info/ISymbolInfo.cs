namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Info
{
    /// <summary>
    /// Indicator interface to qualify class as SymbolInfo 
    /// </summary>
    public interface ISymbolInfo
    {
        bool IsVisible { get; }

        byte Opacity { get; }
    }
}
