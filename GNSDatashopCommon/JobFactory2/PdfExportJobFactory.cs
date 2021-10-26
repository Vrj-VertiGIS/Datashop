using System;
using System.Collections.Generic;
using System.Linq;
using GEOCOM.GNSD.Common.Model;

namespace GEOCOM.GNSD.Common.JobFactory
{
    public class PdfExportJobFactory
    {
        public LayoutExportModel CreateJob(IEnumerable<MapExtent> mapExtents)
        {
            if (mapExtents == null)
            {
                throw new Exception("No map extends defined!");    
            }

            var model = new LayoutExportModel();
            model.Perimeters = new ExportPerimeter[mapExtents.Count()];
            for (int i = 0; i < model.Perimeters.Length; i++)
            {
                var perimeter = new ExportPerimeter { MapExtent = mapExtents.ElementAt(i) };
                model.Perimeters[i] = perimeter;
            }

            return model;
        }
    }
}