using System.Collections.Generic;
using ESRI.ArcGIS.Display;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.MapSymbology.Alignment
{
    public class HVAlignments
    {
        private static readonly List<HVAlignment> _alignments = new List<HVAlignment>()
        {
            new HVAlignment("LT", TextAlignment.TopLeft, MTextAttachmentPoint.TopLeft, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVATop),
            new HVAlignment("LC", TextAlignment.TopLeft, MTextAttachmentPoint.TopLeft, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVATop),
            new HVAlignment("LH", TextAlignment.MiddleLeft, MTextAttachmentPoint.MiddleLeft, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVACenter),
            new HVAlignment("LL", TextAlignment.BaselineLeft, MTextAttachmentPoint.BottomLeft, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVABaseline),
            new HVAlignment("LB", TextAlignment.BottomLeft, MTextAttachmentPoint.BottomLeft, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVABottom),
            new HVAlignment("CT", TextAlignment.TopCenter, MTextAttachmentPoint.TopCenter, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVATop),
            new HVAlignment("CC", TextAlignment.TopCenter, MTextAttachmentPoint.TopCenter, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVATop),
            new HVAlignment("CH", TextAlignment.MiddleCenter, MTextAttachmentPoint.MiddleCenter, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVACenter),
            new HVAlignment("CL", TextAlignment.BaselineCenter, MTextAttachmentPoint.BottomCenter, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVABaseline),
            new HVAlignment("CB", TextAlignment.BottomCenter, MTextAttachmentPoint.BottomCenter, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVABottom),
            new HVAlignment("RT", TextAlignment.TopRight, MTextAttachmentPoint.TopRight, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVATop),
            new HVAlignment("RC", TextAlignment.TopRight, MTextAttachmentPoint.TopRight, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVATop),
            new HVAlignment("RH", TextAlignment.MiddleRight, MTextAttachmentPoint.MiddleRight, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVACenter),
            new HVAlignment("RL", TextAlignment.BaselineRight, MTextAttachmentPoint.BottomRight, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVABaseline),
            new HVAlignment("RB", TextAlignment.BottomRight, MTextAttachmentPoint.BottomRight, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVABottom)
        };

        private static readonly Dictionary<string, HVAlignment> _alignmentsByGEOCOM;

        private static readonly Dictionary<TextAlignment, HVAlignment> _alignmentsByDxf;

        private static readonly Dictionary<MTextAttachmentPoint, HVAlignment> _alignmentsByAttachementPoint;

        private static readonly Dictionary<Tuple<esriTextHorizontalAlignment, esriTextVerticalAlignment>, HVAlignment> _alignmentsByESRI;

        private static readonly HVAlignment _defaultAlignment;

        static HVAlignments()
        {
            var byGEOCOM = new Dictionary<string, HVAlignment>();
            var byDxf = new Dictionary<TextAlignment, HVAlignment>();
            var byDxfAttachementPoint = new Dictionary<MTextAttachmentPoint, HVAlignment>();
            var byESRI = new Dictionary<Tuple<esriTextHorizontalAlignment, esriTextVerticalAlignment>, HVAlignment>();

            foreach (var alignment in _alignments)
            {
                byGEOCOM.Add(alignment.GEONISAlignment, alignment);
                if (!byDxf.ContainsKey(alignment.DxfAlignment))           // There are duplicates heere - add the first of these only
                    byDxf.Add(alignment.DxfAlignment, alignment);
                var esriKey = new Tuple<esriTextHorizontalAlignment, esriTextVerticalAlignment>(alignment.HAlignment, alignment.VAlignment);
                if (!byESRI.ContainsKey(esriKey))                           // There are duplicates heere - add the first of these only
                    byESRI.Add(esriKey, alignment);
                if (!byDxfAttachementPoint.ContainsKey(alignment.DxfAttachementPoint)) // There are duplicates heere - add the first of these only
                    byDxfAttachementPoint.Add(alignment.DxfAttachementPoint, alignment);
            }

            _alignmentsByGEOCOM = byGEOCOM;
            _alignmentsByDxf = byDxf;
            _alignmentsByAttachementPoint = byDxfAttachementPoint;
            _alignmentsByESRI = byESRI;

            _defaultAlignment = byGEOCOM["LT"];
        }

        public static bool TryGetAlignment(string alByGEOCOM, out HVAlignment alignment)
        {
            var result =_alignmentsByGEOCOM.TryGetValue(alByGEOCOM.ToUpper(), out alignment);
            if (!result)
                alignment = _defaultAlignment;
            return result;
        }

        public static bool TryGetAlignment(TextAlignment alByDxf, out HVAlignment alignment)
        {
            var result = _alignmentsByDxf.TryGetValue(alByDxf, out alignment);
            if (!result)
                alignment = _defaultAlignment;
            return result;
        }

        public static bool TryGetAlignment(esriTextHorizontalAlignment hAlignment, esriTextVerticalAlignment vAlignment, out HVAlignment alignment)
        {
            var result = _alignmentsByESRI.TryGetValue(new Tuple<esriTextHorizontalAlignment, esriTextVerticalAlignment>(hAlignment, vAlignment), out alignment);
            if (!result)
                alignment = _defaultAlignment;
            return result;
        }

        private struct Tuple<H, V>
        {
            public H Horizontal { get; set; }
            public V Vertical { get; set; }

            public Tuple(H horiz, V vert)
            {
                Horizontal = horiz;
                Vertical = vert;
            }
        }
    }
}
