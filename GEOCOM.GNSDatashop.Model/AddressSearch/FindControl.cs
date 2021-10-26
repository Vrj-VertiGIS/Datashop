namespace GEOCOM.GNSDatashop.Model.AddressSearch
{
    public class FindControl
    {
        public SearchControl control;

        public SearchControlDef controlDef;

        public string UsesKey;

        public FindControl(SearchControl ctrl, SearchControlDef ctrldef)
        {
            controlDef = ctrldef;
            control = ctrl;
        }
    }
}