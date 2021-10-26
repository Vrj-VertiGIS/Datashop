using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.GNSDatashop.Export.DXF.Common;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using GEOCOM.GNSDatashop.Export.DXF.Common.Interface;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.LayerWriter
{
    public abstract class DxfDataWriterBase
    {
        protected _IDxfWriterContext _context;

        protected int _entitiesWritten = 0;
        public int EntitiesWritten
            => _entitiesWritten;

        internal DxfDataWriterBase(_IDxfWriterContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Post-construction initialization. Will not have all required date prior to complete construction of descendants.
        /// Call this at the end of any - direct - descentant's constructor.
        /// </summary>
        protected virtual void Init()
        {
            IGeometryExtensions.Init(DotsToMeter);
        }

        internal virtual double DotsToMeterUnscaled => 25.4 / 72000.0 * _context.MapCurrentScale;

        internal virtual double DotsToMeterScaled => (Math.Abs(_context.MapReferenceScale) > 0)
                ? (25.4 / 72000.0) * _context.MapReferenceScale
                : DotsToMeterUnscaled;

        public IntPtr HDC => _context.HDC;                          // Handy shortcut to in-memory device context
        public Graphics Graphics => _context.Graphics;              // Handy shortcut to Graphics object drawing to in-memory dc
        public IDisplay RenderDisplay => _context.RenderDisplay;    // Handy shortcut to rendering arcmap display 

        /// <summary>
        /// The scaling factor in effect to convert dot units to meters 
        /// Use this property. Properties DotstOmegerScaled/DotstToMeterUnscaled are
        /// for class-internal use only and are used to evaluate the value made
        /// available below.
        /// </summary>
        public virtual double DotsToMeter => DotsToMeterScaled;

        /// <summary>
        /// Continue the processing
        /// </summary>
        internal bool Continue => _context.CancelTracker.Continue();

        /// <summary>
        /// Advance step progressor
        /// </summary>
        internal void Step()
        {
            _context.StepProgressor.StepItem();
        }

        /// <summary>
        /// Clip 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal IGeometry Clip(IGeometry geometry)
        {
            return (null != geometry) && (!geometry.IsEmpty)
                ? _context.RegionOfInterest.Clip(geometry)
                : null;
        }

        /// <summary>
        /// Write a dxf object - this may be a single object...
        /// </summary>
        /// <param name="dxfEntity"></param>
        internal void WriteEntity(EntityObject dxfEntity)
        {
            _context.DxfDocument.AddEntity(dxfEntity);

            _entitiesWritten++;
        }

        /// <summary>
        /// ...or a group of objects (i.e. a polygon consisting of rings)
        /// </summary>
        /// <param name="dxfEntity"></param>
        internal void WriteEntity(IEnumerable<EntityObject> dxfEntity)
        {
            var entities = dxfEntity.ToList();  // Avoid reparsing the enumerator

            _context.DxfDocument.AddEntity(entities);

            _entitiesWritten += entities.Count;
        }
    }
}
