using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.DBStore.DbAccess;
using GEOCOM.GNSD.PlotExtension.PlotExtension;
using GEOCOM.GNSDatashop.Model;
using Path = System.IO.Path;

namespace GEOCOM.GNSD.DatashopWorkflow.Admin
{
    using GEOCOM.GNSD.PlotExtension.Layout;

    public class PlotTemplateUpdater
    {
        // Logging
        private IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Searches all mxt and mxd files in the plot templates directory and creates a entry in a cache table named GNSD_PLOTDEFINTION to
        /// get quick access to the map extent for the web application
        /// Missing templates are removed in db.
        /// </summary>
        internal void UpdatePlotTemplatesInDb(string plotTemplatesDirectory, int mediumCode)
        {
            List<PlotdefinitionKey> existing = new List<PlotdefinitionKey>();

            // Hibernate object
            PlotDefinitionStore plotDefinitionStore = new PlotDefinitionStore();

            // iterate all mxt/mxd files in template directory
            DirectoryInfo templateDir = new DirectoryInfo(plotTemplatesDirectory);
            foreach (FileInfo file in Utils.Utils.GetFilesByExtensions(templateDir, "*.mxt", "*.mxd"))
            {
                string templatename = Path.GetFileNameWithoutExtension(file.Name);
                PlotdefinitionKey key = new PlotdefinitionKey(mediumCode, templatename);

                // check if an entry in db exists
                Plotdefinition definition = plotDefinitionStore.GetPlotdefinition(key);

                // create a new entry if the definition was not found
                if (definition == null)
                {
                    definition = new Plotdefinition();
                    definition.Roles = "BUSINESS,TEMP,ADMIN";
                    definition.PlotdefinitionKey = key;
                    definition.Description = string.Format("{0} ({1})", templatename, Path.GetFileName(file.Name));
                    plotDefinitionStore.Add(definition);
                    _log.DebugFormat("New plottemplate {0} added to database", templatename);
                }

                // keep track on existing definitions to delete all other entries afterward
                existing.Add(key);

                // default;
                double heightInCm = 20;
                double widthInCm = 20;

                try
                {
                    using (GNSPlotExtension plotExtension = new GNSPlotExtension())
                    {
                        IEnvelope env = PlotTemplateHelper.GetMapExtentInPageCoords(templatename);
                        {
                            widthInCm = env.Width;
                            heightInCm = env.Height;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("PlotTemplateUpdater: Cannot access plotextension", ex);
                }

                // save new extent on db
                definition.PlotHeightCm = heightInCm;
                definition.PlotWidthCm = widthInCm;
                plotDefinitionStore.Update(definition);
                _log.DebugFormat("Plottemplate {1:s} (medium {0:d} updated: Width = {3:f} and height = {2:f})", mediumCode, templatename, heightInCm, widthInCm);
            }

            //// remove all missing templates in db to avoid garbage 
            //// TODO Access code for all media or null
            foreach (Plotdefinition definition in plotDefinitionStore.LoadForMedium(0))
            {
                if (existing.IndexOf(definition.PlotdefinitionKey) < 0)
                {
                    plotDefinitionStore.Delete(definition);
                }
            }

            plotDefinitionStore.LoadForMedium(0);
        }
    }
}
