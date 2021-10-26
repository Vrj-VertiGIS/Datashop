using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    /// <summary>
    /// Keep info about an erroneous layer.
    /// Do not store the layer or the exception heere as this might
    /// prevent the system from unloading this data structures (partly)
    /// in case of lack of memory (outofmemoryexception).
    /// </summary>
    public class ErroneousLayerInfo : LayerInfo
    {
        public enum ExceptionTypeEnum { outOfMemory, com, any, none };
        public ExceptionTypeEnum ExceptionType { get; private set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; private set; }
        public uint HResult { get; private set; }

        private StoLanguage _lng = null;

        public ErroneousLayerInfo(ILayer layer, Exception ex)
            : base(layer)
        {
            ExceptionType = TypeFromException(ex);
            ExceptionMessage = (ExceptionType != ExceptionTypeEnum.none) ? ex.Message : string.Empty;
            HResult = (uint)((ExceptionType == ExceptionTypeEnum.com) ? ((COMException)ex).HResult : 0);

            IsSupported = true;
            Message = string.Empty;
        }

        public ErroneousLayerInfo(IEnumerable<IElement> drawing, Exception ex)
            : base(drawing)
        {
            ExceptionType = TypeFromException(ex);
            ExceptionMessage = (ExceptionType != ExceptionTypeEnum.none) ? ex.Message : string.Empty;
            HResult = (uint)((ExceptionType == ExceptionTypeEnum.com) ? ((COMException)ex).HResult : 0);

            IsSupported = true;
            Message = string.Empty;
        }

        public ErroneousLayerInfo(ILayer layer)
            : this(layer, null)
        {
        }

        public ErroneousLayerInfo(IEnumerable<IElement> drawing)
            : this(drawing, null)
        {
        }

        public override string ToString()
            => ToString(false);

        public string ToString(bool extended, StoLanguage lng = null)
        {
            _lng = lng ?? new StoLanguage() { AppName = Product.TechnicalAppname };
            return (extended)
                ? $"{LayerName} - {ErrorInfo()}"
                : LayerName;
        }

        private string ErrorInfo()
            => (ExceptionTypeEnum.none == ExceptionType)
                ? !IsSupported
                    ? UnsupportedLayerInfo
                    : !Valid 
                        ? _lng.LoadStr(10915, "Invalid - i.e. not in database, no database connection")
                        : !string.IsNullOrEmpty(Message)
                            ? Message
                            : _lng.LoadStr(10916, "No error info given")
                : (ExceptionTypeEnum.outOfMemory == ExceptionType)
                    ? _lng.LoadStr(10907, "Layer contains too many features. Please export using a selection layer. The output file will not contain any data of this layer")
                    : (ExceptionTypeEnum.com == ExceptionType)
                        ? COMExceptionInfo
                        : (!string.IsNullOrEmpty(Message))
                            ? string.Format(Message, ExceptionMessage)
                            : ExceptionMessage;

        private string UnsupportedLayerInfo
        {
            get
            {
                switch (LayerType)
                {
                    case LayerTypeEnum.annotation: return StrNotSupported(10926, "annotation layers");
                    case LayerTypeEnum.dimension: return StrNotSupported(10925, "dimensioning layers");
                    case LayerTypeEnum.raster: return StrNotSupported(40927, "raster layer");
                    case LayerTypeEnum.representation: return StrNotSupported(10924, "representations");
                    case LayerTypeEnum.feature: return UnsupportedFeatureLayerInfo;
                    case LayerTypeEnum.any: return StrNotSupported(40928, "layer");
                    default: return string.Empty;
                }
            }
        }

        private string UnsupportedFeatureLayerInfo
            => StrNotSupported($"{_lng.LoadStr(10930, "feature layer")} ({_lng.LoadStr(10932, "geometry type")}:{GeometryType})");

        private string StrNotSupported(int id, string defText)
            => StrNotSupported(_lng.LoadStr(id, defText));
        private string StrNotSupported(string text)
            => _lng.FmtLoadStr(10922, "{0} - not supported. Layer will be ignored", text);

        private string COMExceptionInfo
        {
            get
            {
                switch (HResult)
                {
                    case 0x80040207:
                        {
                            return (!string.IsNullOrEmpty(LayerDefinitionExpression))
                                ? $"{_lng.LoadStr(10940, "Invalid definition query")}: {LayerDefinitionExpression}"
                                : $"{_lng.LoadStr(10940, "Invalid definition query")}.";
                        }
                    case 0x80040653:
                        {
                            return (!string.IsNullOrEmpty(LayerDefinitionExpression))
                                ? _lng.FmtLoadStr(10942, "Invalid definition query - field {0} not found on layer", LayerDefinitionExpression)
                                : _lng.FmtLoadStr(10942, "Invalid definition query - field {0} not found on layer", string.Empty);
                        }
                    default:
                        return $"{_lng.FmtLoadStr(10944, "COM-exception")}: {HResult} - {ExceptionMessage}";
                }
            }
        }

        private ExceptionTypeEnum TypeFromException(Exception ex)
            => (null == ex) ? ExceptionTypeEnum.none
                : ex is OutOfMemoryException ? ExceptionTypeEnum.outOfMemory
                : ex is COMException ? ExceptionTypeEnum.com
                : ExceptionTypeEnum.any;
    }
}
