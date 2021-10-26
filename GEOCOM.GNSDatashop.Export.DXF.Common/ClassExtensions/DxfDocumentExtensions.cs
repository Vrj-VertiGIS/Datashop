using System.Linq;
using System.Collections.Generic;
using netDxf;
using netDxf.Entities;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class DxfDocumentExtensions
    {
        public static bool IsEmpty(this DxfDocument document)
        {
            var isEmpty = (0 >= document.Points.Count()) 
                && (0 >= document.LwPolylines.Count()) && (0 >= document.Polylines.Count())
                && (0 >= document.Hatches.Count()) && (0 >= document.Texts.Count())
                && (0 >= document.MTexts.Count()) && (0 >= document.Inserts.Count())
                && (0 >= document.Lines.Count()) && (0 >= document.Circles.Count())
                && (0 >= document.MLines.Count());

            return isEmpty;
        }

        /// <summary>
        /// Return all entities of a layer in an enumerator
        /// Since this may be called in an out-of-memory situation, we may not
        /// use linq expressions forming intermediary lists (i.e. Union);
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IEnumerable<EntityObject> Entities(this DxfDocument d, System.Func<EntityObject, bool> pred)
        {
            for (var e = d.Points.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Inserts.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.LwPolylines.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Polylines.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Hatches.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Texts.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.MTexts.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Inserts.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Lines.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.Circles.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
            for (var e = d.MLines.GetEnumerator(); e.MoveNext();) if (pred(e.Current)) yield return e.Current;
        }

        public static int EntityCount(this DxfDocument document, System.Func<EntityObject, bool> pred)
            => document.Entities(pred).Count();

    }
}
