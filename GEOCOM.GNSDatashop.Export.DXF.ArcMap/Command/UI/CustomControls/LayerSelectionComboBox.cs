using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using ESRI.ArcGIS.Geodatabase;

namespace GEOCOM.GNSDatashop.Export.DXF.ArcMap.Command.UI.CustomControls
{
    public partial class LayerSelectionComboBox : ComboBox
    {

        [DllImport("user32.dll")]
        private static extern long LockWindowUpdate(Int32 handle);

        private List<esriGeometryType> _supportedGeometryTypes = new List<esriGeometryType>();
        private List<esriFeatureType> _supportedFeatureTypes = new List<esriFeatureType>();
        private bool _noLayerEntry = true;

        public LayerSelectionComboBox()
        {
            InitializeComponent();
        }

        public LayerSelectionComboBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public bool NoLayerEntry { get => _noLayerEntry; set => _noLayerEntry = value; }

        public IEnumerable<ILayer> Layers
        {
            get
            {
                return (DataSource != null)
                    ? ((IEnumerable<LayerSelectionComboItem>) DataSource).Select(l => l.Layer)
                    : new List<ILayer>();
            }
        }

        public void Set_Layers(IEnumerable<ILayer> value)
        {
            LockWindowUpdate(Handle.ToInt32());

            var selectedLayerBefore = (null != DataSource) && (null != SelectedItem)
                ? SelectedItem as LayerSelectionComboItem
                : null;

            var values = value.Where(l => IsSupportedLayer(l)).OrderBy(l => l.Name).Select(l => new LayerSelectionComboItem(l));
            if (_noLayerEntry)
                values = new List<LayerSelectionComboItem>() { new LayerSelectionComboItem(null) }.Union(values);
            DataSource = values.ToArray();

            SetSelectedItem(selectedLayerBefore);

            LockWindowUpdate(0);
        }

        public IList<esriGeometryType> SupportedGeometryTypes
        {
            get => _supportedGeometryTypes;
        }

        public IList<esriFeatureType> SupportedFeatureTypes
            => _supportedFeatureTypes;

        /// <summary>
        /// Set the set of supported geometry types (layers implementing a given egometry type)
        /// Not a setter property due to type binding issues with ressource manager
        /// </summary>
        /// <param name="geometryTypes"></param>
        public void SetSupportedGeometryTypes(IEnumerable<esriGeometryType> geometryTypes)
        {
            _supportedGeometryTypes.Clear();
            _supportedGeometryTypes.AddRange(geometryTypes);
        }

        /// <summary>
        /// List of feature types to limit layers selection.
        /// </summary>
        /// <param name="featureTypes"></param>
        public void SetSupportedFeatureTypes(IEnumerable<esriFeatureType> featureTypes)
        {
            _supportedFeatureTypes.Clear();
            _supportedFeatureTypes.AddRange(featureTypes);
        }

        public ILayer SelectedLayer
        {
            get => (0 <= SelectedIndex)
            ? ((LayerSelectionComboItem) Items[SelectedIndex]).Layer
            : null;

            set => SetSelectedItem(new LayerSelectionComboItem(value));
        }

        public void ToRegistry(string key, string valueName)
        {
            var selected = (0 <= SelectedIndex)
                ? ((LayerSelectionComboItem) Items[SelectedIndex]).ToString()
                : string.Empty;
            Registry.SetValue(key, valueName, selected, RegistryValueKind.String);
        }

        public void FromRegistry(string key, string valueName)
        {
            var selected = (string) GetRegistryValue(key, valueName, string.Empty);
            SetSelectedItem(selected);
        }

        private void SetSelectedItem(LayerSelectionComboItem layerItem)
        {
            var selected = ItemsEnum.FirstOrDefault(l => l.Equals(layerItem));
            SetSelectedItemCore(selected);
        }

        private void SetSelectedItem(string layerName)
        {
            var selected = ItemsEnum.FirstOrDefault(l => l.Equals(layerName));
            SetSelectedItemCore(selected);
        }

        private void SetSelectedItemCore(LayerSelectionComboItem selected)
        {
            SelectedItem = (null != selected)
                ? selected
                : (0 < Items.Count)
                    ? Items[0]
                    : null;
        }

        private int ImageIndexFromGeometry(esriGeometryType geometryType)
        {
            switch (geometryType)
            {
                case esriGeometryType.esriGeometryPoint: return 0; 
                case esriGeometryType.esriGeometryPolyline: return 1;
                case esriGeometryType.esriGeometryPolygon: return 2;
                default: return 3;
            }
        }

        private bool IsSupportedLayer(ILayer layer)
            => ((layer is IFeatureLayer fl)
            && (null != fl.FeatureClass)    // Invalid feature layer - i.e. data source not connected/not available
            && ((0 >= _supportedFeatureTypes.Count) || (_supportedGeometryTypes.Any(t => t.Equals(fl.FeatureClass.ShapeType))))
            && ((0 >= _supportedFeatureTypes.Count) || (_supportedFeatureTypes.Any(f => f.Equals(fl.FeatureClass.FeatureType)))));

        private void LayerSelectionComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var drawItem = Items[e.Index] as LayerSelectionComboItem;

            if (null != drawItem)
            {
                var img = imglLayerTypes.Images[ImageIndexFromGeometry(drawItem.GeometryType)];

                var imgTop = e.Bounds.Top + (e.Bounds.Height - img.Height) / 2;
                var imgLeft = e.Bounds.Left + 2;
                e.Graphics.DrawImageUnscaled(img, imgLeft, imgTop);

                var imageXSize = new Size(img.Width + 4, 0);
                var textRect = new RectangleF(e.Bounds.Location + imageXSize, e.Bounds.Size - imageXSize);
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, SystemBrushes.ControlText, textRect);
            }
        }

        private IEnumerable<LayerSelectionComboItem> ItemsEnum
        {
            get
            {
                for (int i = 0; i < Items.Count; i++)
                    yield return Items[i] as LayerSelectionComboItem;
            }
        }

        private object GetRegistryValue(string keyName, string valueName, object defaultValue)
        {
            var value = Registry.GetValue(keyName, valueName, defaultValue);
            return (null != value)
                ? value
                : defaultValue;
        }

    }
}