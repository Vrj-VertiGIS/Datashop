using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class ILogExtensions
    {
        private delegate void Logger(object msg);
        private delegate void ExceptionLogger(object msg, Exception ex);

        private static IEnumerable<string> Lines(string msg)
            => msg.Split(new char[] { '\n' }, StringSplitOptions.None);

        private static void PutLog(Logger logger, IEnumerable<string> lines)
        {
            foreach (var line in lines)
                logger?.Invoke(line);
        }

        private static void PutLog(ExceptionLogger logger, Exception ex, IEnumerable<string> lines)
        {
            var firstLine = lines?.Take(1).FirstOrDefault();
            if (!string.IsNullOrEmpty(firstLine))
            {
                logger?.Invoke(firstLine, ex);
                foreach (var line in lines.Skip(1))
                    logger?.Invoke(line, null);
            }
        }

        private static void PutLog(Logger logger, object msgObject)
        {
            if (msgObject is string msg)
                PutLog(logger, Lines(msg));
            else if (msgObject is IEnumerable<string>)
                PutLog(logger, msgObject as IEnumerable<string>);
            else
                logger?.Invoke(msgObject);
        }

        private static void PutLog(ExceptionLogger logger, Exception ex, object msgObject)
        {
            if (msgObject is string msg)
                PutLog(logger, ex, Lines(msg));
            else if (msgObject is IEnumerable<string>)
                PutLog(logger, ex, msgObject as IEnumerable<string>);
            else
                logger?.Invoke(msgObject, ex);
        }

        public static void InfoNL(this ILog logger, object msg)
            => PutLog(logger.Info, msg);
        public static void ErrorNL(this ILog logger, object msg)
            => PutLog(logger.Error, msg);
        public static void FatalNL(this ILog logger, object msg)
            => PutLog(logger.Fatal, msg);
        public static void WarnNL(this ILog logger, object msg)
            => PutLog(logger.Warn, msg);
        public static void DebugNL(this ILog logger, object msg)
            => PutLog(logger.Debug, msg);

        public static void InfoNL(this ILog logger, object msg, Exception ex)
            => PutLog(logger.Info, ex, msg);
        public static void ErrorNL(this ILog logger, object msg, Exception ex)
            => PutLog(logger.Error, ex, msg);
        public static void FatalNL(this ILog logger, object msg, Exception ex)
            => PutLog(logger.Fatal, ex, msg);
        public static void WarnNL(this ILog logger, object msg, Exception ex)
            => PutLog(logger.Warn, ex, msg);
        public static void DebugNL(this ILog logger, object msg, Exception ex)
            => PutLog(logger.Debug, ex, msg);
    }
}
