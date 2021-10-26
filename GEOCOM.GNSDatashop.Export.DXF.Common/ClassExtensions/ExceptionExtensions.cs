using System;
using System.Runtime.InteropServices;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ExceptionExtensions
    {
        private static string ExceptionInfoCore(this Exception ex)
            => (ex is COMException cex)
                ? $"{cex.GetType()} {cex.ErrorCode:X8}:{cex.Message} @ {cex.Source}"
                : $"{ex.GetType()} {ex.Message} @ {ex.Source}";

        private static string ExceptionInfoCore(this Exception ex, int depth)
            => (0 < depth)
                ? $"[{ex.ExceptionInfoCore()}]"
                : ex.ExceptionInfoCore();

        private static string ExceptionInfoCore(this Exception ex, int depth, bool newLines)
            => (0 < depth)
                ? newLines
                    ? $"\n{Tabs(depth)}{ex.ExceptionInfoCore(depth)}"
                    : $"{Tabs(depth)}{ex.ExceptionInfoCore(depth)}"
                : ex.ExceptionInfoCore(depth);

        private static string Tabs(int depth)
            => new string('\t', depth);

        private static string ExceptionInfo(this Exception ex, bool newLines, int depth = 0)
            => (null == ex)
                ? string.Empty
                : (2 < depth)
                    ? (newLines) ? $"\n{Tabs(depth)}..." : $"{Tabs(depth)}..."
                    : $"{ex.ExceptionInfoCore(depth, newLines)}{ex.InnerException.ExceptionInfo(newLines, depth + 1)}";

        public static string ToString(this Exception ex, bool newLines)
            => ExceptionInfo(ex, newLines, 0);
    }
}
