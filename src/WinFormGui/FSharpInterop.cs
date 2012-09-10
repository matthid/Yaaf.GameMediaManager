// ----------------------------------------------------------------------------
// This file (FSharpInterop.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System.Diagnostics;

    public static class TracerExtensions
    {
        public static void Log(
            this Logging.LoggingInterfaces.ITracer tracer, TraceEventType type, string format, params object[] data)
        {
            Logging.CSharpInteropLog(tracer, type, string.Format(format, data));
        }

        public static void LogError(this Logging.LoggingInterfaces.ITracer tracer, string format, params object[] data)
        {
            tracer.Log(TraceEventType.Error, format, data);
        }

        public static void LogWarning(
            this Logging.LoggingInterfaces.ITracer tracer, string format, params object[] data)
        {
            tracer.Log(TraceEventType.Warning, format, data);
        }

        public static void LogInformation(
            this Logging.LoggingInterfaces.ITracer tracer, string format, params object[] data)
        {
            tracer.Log(TraceEventType.Information, format, data);
        }

        public static void LogCritical(
            this Logging.LoggingInterfaces.ITracer tracer, string format, params object[] data)
        {
            tracer.Log(TraceEventType.Critical, format, data);
        }

        public static void LogVerbose(
            this Logging.LoggingInterfaces.ITracer tracer, string format, params object[] data)
        {
            tracer.Log(TraceEventType.Verbose, format, data);
        }
    }
}