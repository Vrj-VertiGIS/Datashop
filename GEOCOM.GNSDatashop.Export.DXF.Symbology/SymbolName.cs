namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public static class SymbolName
    {
        private static uint _current = 0;

        public static void Reset()
        {
            _current = 0;
        }
        public static string Create()
        {
            return (++_current).ToString();
        }
    }
}
