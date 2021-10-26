using System;
using GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions;
using netDxf.Collections;
using netDxf.Tables;

namespace GEOCOM.GNSDatashop.Export.DXF.Factories
{
    public class DxfLayerFactory
    {
        /// <summary>
        /// Create a dxf layer object - without adding it to a parent (document, layers collection), enshuring name fulfills restrictions.
        /// </summary>
        /// <param name="name">layer name</param>
        /// <returns></returns>
        public static Layer CreateLayer(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name (layer name) null or empty");

            var result = new Layer(name.DxfCompatibleName(true));

            return result;
        }

        /// <summary>
        /// Create a layer and add it to the documents layers collection
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Layer CreateLayer(Layers owner, string name) 
        {
            if (null == owner) throw new ArgumentNullException("owner");

            var result = CreateLayer(name);

            owner.Add(result);

            return result;
        }

        /// <summary>
        /// Adds a sibling layer - logically a child as paent's name and name will be concatenated
        /// </summary>
        /// <param name="parent">logically a parent - implemented as a sibling</param>
        /// <param name="name">name of the child layer - will be prefixed by the parent's layer name</param>
        /// <returns></returns>
        public static Layer CreateLayer(Layer parent, string name)
        {
            if (null == parent) throw new ArgumentNullException("parent");

            return CreateLayer(parent.Owner, string.Format("{0} - {1}", parent.Name, name));
        }
    }
}
