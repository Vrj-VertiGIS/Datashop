using log4net;
using System;

namespace GEOCOM.GNSDatashop.Export.DXF.Common
{
    public static class Logger
    {
        public static readonly ILog _log = LogManager.GetLogger("DxfWriter");

        public static bool IsDebugEnabled => _log.IsDebugEnabled;

        public static bool IsInfoEnabled => _log.IsInfoEnabled;

        public static bool IsWarnEnabled => _log.IsWarnEnabled;

        public static bool IsErrorEnabled => _log.IsErrorEnabled;

        public static bool IsFatalEnabled => _log.IsFatalEnabled;


        public static void Debug(object message)
            => _log.Debug(message);

        public static void Debug(object message, Exception exception)
            => _log.Debug(message, exception);

        public static void DebugFormat(string format, params object[] args)
            => _log.DebugFormat(format, args);

        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
            => _log.DebugFormat(provider, format, args);

        public static void Error(object message)
            => _log.Error(message);

        public static void Error(object message, Exception exception)
            => _log.Error(message, exception);

        public static void ErrorFormat(string format, params object[] args)
            => _log.ErrorFormat(format, args);

        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
            => _log.ErrorFormat(provider, format, args);

        public static void Fatal(object message)
            => _log.Fatal(message);

        public static void Fatal(object message, Exception exception)
            => _log.Fatal(message, exception);

        public static void FatalFormat(string format, params object[] args)
            => _log.FatalFormat(format, args);

        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
            => _log.FatalFormat(provider, format, args);

        public static void Info(object message)
            => _log.Info(message);

        public static void Info(object message, Exception exception)
            => _log.Info(message, exception);

        public static void InfoFormat(string format, params object[] args)
            => _log.InfoFormat(format, args);

        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
            => _log.InfoFormat(provider, format, args);

        public static void Warn(object message)
            => _log.Warn(message);

        public static void Warn(object message, Exception exception)
            => _log.Warn(message, exception);

        public static void WarnFormat(string format, params object[] args)
            => _log.WarnFormat(format, args);

        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
            => _log.WarnFormat(provider, format, args);
    }
}
