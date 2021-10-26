namespace GEOCOM.GNSDatashop.Model.DatashopWorkflow
{
    public enum PlotJobSteps
    {
        PreProcess = 1,
        Process = 2,
        WaitForDocuments = 3,
        Package = 4,
        Send = 5,
        JobDone = 6,
        Archive = 7
    }
}