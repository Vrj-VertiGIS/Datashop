using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using GEOCOM.Common;
using GEOCOM.Common.Logging;
using GEOCOM.GNSD.PlotExtension.Config;
using GEOCOM.GNSD.PlotExtension.Utils;

namespace GEOCOM.GNSD.PlotExtension.Layout
{
    using System.Collections.Generic;

    public class PageLayoutManager
    {
        private static PageLayoutManager _pageLayout;
        private IPageLayout _currentPageLayout;
        private String _currentPlotTemplateFullName;

        private OriginalTextEnumerator _originalTextEnumerator;

        // log4net
        private static IMsg _log = new Msg(MethodBase.GetCurrentMethod().DeclaringType);

        ~PageLayoutManager()
        {
            try
            {
                // disconnect two map documents to prevent application error in event viewer.
                ReplaceMapForMapSurrounds(_currentPageLayout, null);
            }
            catch (Exception e)
            {
                try
                {
                    _log?.Warn(e.Message, e);
                }
                catch (Exception)
                {
                   // in case the log throws an exception
                }
            }
         
        }

        public static PageLayoutManager Instance
        {
            get
            {
                if (_pageLayout == null)
                {
                    _pageLayout = new PageLayoutManager();
                }
                return _pageLayout;
            }
        }

        private void InitializeOriginalTextValues()
        {
            _originalTextEnumerator = new OriginalTextEnumerator();
            if (CurrentPageLayout != null)
            {
                // iterates all elements (not recursiv)
                IGraphicsContainer gc = CurrentPageLayout as IGraphicsContainer;
                gc.Reset();
                IElement element = null;
                while ((element = gc.Next()) != null)
                {
                    // replaces text
                    // groupes are processed recursiv
                    CacheOriginalText(element);
                }
            }            
        }

        public IPageLayout CurrentPageLayout
        {
            get
            {
                return _currentPageLayout;
            }
        }

        public void SetPlotTemplateName(String plotTemplateName)
        {
            String newPlotTemplateFullName = PlotTemplateHelper.GetTemplateFullName(plotTemplateName);
            if (_currentPlotTemplateFullName == null ||
                !_currentPlotTemplateFullName.Equals(newPlotTemplateFullName))
            {
                _currentPlotTemplateFullName = PlotTemplateHelper.GetTemplateFullName(plotTemplateName);
                ReplacePlotTemplateMapWithServerMap();

                InitializeOriginalTextValues();
            }
        }

        /// <summary>
        /// Iterates all text elements on layout and replaces variables with their value
        /// </summary>
        /// <param name="variables">A dictionary of variables</param>
        public void ReplaceTextVars(Dictionary<string, string> variables)
        {
            if (CurrentPageLayout != null)
            {
                _originalTextEnumerator.Reset();

                // iterates all elements (not recursiv)
                IGraphicsContainer gc = CurrentPageLayout as IGraphicsContainer;
                gc.Reset();
                IElement element = null;
                while ((element = gc.Next()) != null)
                {
                    // replaces text
                    // groupes are processed recursiv
                    ReplaceTextVarsForElement(element, variables);
                }
            }
        }

        private void ReplacePlotTemplateMapWithServerMap()
        {
            // get server map
            IMapDocument serverMapDocument = new MapDocumentClass();
            IMapDocument plotTemplateDocument = new MapDocumentClass();

            try
            {
                Assert.True(File.Exists(PlotExtensionConfig.Instance.MxdPath.Path), String.Format("Mxd file ('{0}') configured in {1} doesn't exist!", PlotExtensionConfig.Instance.MxdPath.Path, PlotExtensionConfig.Instance.ConfigFilePath));

                serverMapDocument.Open(PlotExtensionConfig.Instance.MxdPath.Path, string.Empty);  // TODO Maybe later a password may be configured
                // TODO At the moment always the first map is fetched. Maybe this should be passed by XML in the job description later on
                IMap serverMap = serverMapDocument.get_Map(0);
                plotTemplateDocument.Open(_currentPlotTemplateFullName, string.Empty);

                // Replace the map with servermap
                // Keep a reference to execute the job
                _currentPageLayout = plotTemplateDocument.PageLayout;

                // TODO: What happens if we have more than one map not tested approach: mapDocument.SetActiveView(...);
                IMapFrame mapFrame = LayerHelper.GetFirstMapFrameFound(_currentPageLayout);

                // Keep SpatialRef
                ISpatialReference spatialReference = serverMap.SpatialReference;

                // replaces the map with the servermap
                // this will create a new empty PageLayout so we have..
                plotTemplateDocument.ReplaceContents(serverMap as IMxdContents);
                mapFrame.Map = serverMap;
                ReplaceMapForMapSurrounds(_currentPageLayout, mapFrame);

                // .. to restore the layout again
                plotTemplateDocument.ReplaceContents(_currentPageLayout as IMxdContents);

                // reset new spatialreferece -> recalc units
                mapFrame.Map.SpatialReference = spatialReference;
            }
            catch (Exception ex)
            {
                _log.Error(String.Format("PlotExtension: Replacing template failed with error: {0:s}", ex.Message), ex);
            }
            finally
            {
                serverMapDocument.Close();
                plotTemplateDocument.Close();
            }
        }

        /// <summary>
        /// TODO add support for multiple maps (i.e. overview)
        /// </summary>
        /// <param name="plotTemplatePageLayout">The *.mxt/*.mxt page layout</param>
        /// <param name="mapFrame">The map frame</param>
        private void ReplaceMapForMapSurrounds(IPageLayout plotTemplatePageLayout, IMapFrame mapFrame)
        {
            IGraphicsContainer gc = plotTemplatePageLayout as IGraphicsContainer;
            gc.Reset();
            IElement element = null;

            while ((element = gc.Next()) != null)
            {
                if (element is IMapSurroundFrame)
                {
                    SetMapSurroundOnElement(element, mapFrame);
                }
                else if (element is IGroupElement)
                {
                    IGroupElement group = element as IGroupElement;
                    IElement groupItem;
                    IEnumElement elements = group.Elements;
                    while ((groupItem = elements.Next()) != null)
                    {
                        if (groupItem is IMapSurroundFrame)
                        {
                            SetMapSurroundOnElement(groupItem, mapFrame);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the mapSurround on the element to the mapFrame.Map.
        /// </summary>
        /// <param name="element">The element in the graphic container</param>
        /// <param name="mapFrame">The map frame</param>
        private void SetMapSurroundOnElement(IElement element, IMapFrame mapFrame)
        {
            IMapSurround mapSurround = (element as IMapSurroundFrame).MapSurround;
            if (mapSurround != null)
            {
                mapSurround.Map = mapFrame?.Map;
            }
            else
            {
                _log.Warn("Empty mapsurround found in template");
            }
        }

        /// <summary>
        /// Iterates all text elements in a grouped element list and replaces variables with their value
        /// </summary>
        /// <param name="rootElement">group or simple text element</param>
        /// <param name="variables">dictionary of variables</param>
        private void ReplaceTextVarsForElement(IElement rootElement, Dictionary<string, string> variables)
        {
            if (rootElement != null)
            {
                if (rootElement is ITextElement)
                {
                    ITextElement textEl = rootElement as ITextElement;
                    _originalTextEnumerator.MoveNext();
                    textEl.Text = ReplaceVars(_originalTextEnumerator.Current, variables);
                }
                else if (rootElement is IGroupElement)
                {
                    IGroupElement group = rootElement as IGroupElement;

                    IElement groupItem;
                    IEnumElement elements = group.Elements;
                    while ((groupItem = elements.Next()) != null)
                    {
                        // Replace recursiv in group
                        ReplaceTextVarsForElement(groupItem, variables);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates all text elements in a grouped element list and caches the
        /// original texts to cache.
        /// </summary>
        /// <param name="rootElement">group or simple text element</param>
        private void CacheOriginalText(IElement rootElement)
        {
            if (rootElement != null)
            {
                if (rootElement is ITextElement)
                {
                    ITextElement textEl = rootElement as ITextElement;
                    _originalTextEnumerator.AddText(textEl.Text);
                }
                else if (rootElement is IGroupElement)
                {
                    IGroupElement group = rootElement as IGroupElement;

                    IElement groupItem;
                    IEnumElement elements = group.Elements;
                    while ((groupItem = elements.Next()) != null)
                    {
                        // Replace recursiv in group
                        CacheOriginalText(groupItem);
                    }
                }
            }
        }

        private string ReplaceVars(string p, Dictionary<string, string> variables)
        {
            foreach (KeyValuePair<string, string> var in variables)
            {
                string varName = "%(" + var.Key + ")";
                if (p.Contains(varName)) p = p.Replace(varName, var.Value);
            }
            return p;
        }
    }
}
