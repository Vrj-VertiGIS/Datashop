using netDxf;
using System;
using System.Linq;
using netDxf.Entities;
using System.Collections.Generic;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology
{
    public class EntityObjectInfo
    {
        private List<EntityObject> _objects;        // Entity objects - possibly used to make up a BLOCK

        private BoundingRectangle _extent;          // Union of all EntityObject(s) (min/max) bounding boxes

        public EntityObjectInfo()
        {
            _objects = new List<EntityObject>();
            _extent = null;
        }

        public EntityObjectInfo(EntityObject obj, BoundingRectangle extent)
        {
            _objects = new List<EntityObject>(1) { obj };
            _extent = extent;
        }

        public EntityObjectInfo(IList<EntityObject> obj, BoundingRectangle extent)
        {
            _objects = obj as List<EntityObject>;
            _extent = extent;
        }

        public EntityObjectInfo(IEnumerable<EntityObject> obj, BoundingRectangle extent)
        {
            _objects = new List<EntityObject>();
            _objects.AddRange(obj);

            _extent = extent;
        }

        public static EntityObjectInfo operator +(EntityObjectInfo u, EntityObjectInfo v)
        {
            u._objects.AddRange(v._objects);

            if (null == u._extent)
                u._extent = v._extent;
            else
                u._extent = BoundingRectangle.Union(u._extent, v._extent);

            return u;
        }

        public IEnumerable<EntityObject> Objects => _objects;

        /// <summary>
        /// We explicitely expect only one object => throw an exception otherwise
        /// </summary>
        public EntityObject Object
        {
            get
            {
                if ((null != _objects) && (1 == _objects.Count))
                    return _objects[0];

                throw new System.IndexOutOfRangeException($"Expected 1, found {_objects.Count} entity objects");
            }
        }

        public BoundingRectangle Extent => (null != _extent)
            ? _extent
            : new BoundingRectangle(Vector2.Zero, Vector2.Zero);

#if DEBUG
        public IEnumerable<string> ToStrings()
        {
            yield return "Entity objects:";
            foreach (var eo in _objects)
                foreach (var str in eo.Dump())
                    yield return str;
        }

#endif
    }
}
