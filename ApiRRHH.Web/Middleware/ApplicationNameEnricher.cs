using Serilog.Core;
using Serilog.Events;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ApiRRHH.Web.Middleware
{
    public class ApplicationNameEnricher : ILogEventEnricher
    {
        private LogEventProperty _cachedProperty;

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(GetProperty(propertyFactory));
        }

        private LogEventProperty GetProperty(ILogEventPropertyFactory propertyFactory)
        {
            // Don't care about thread-safety, in the worst case the field gets overwritten and one property will be GCed
            if (_cachedProperty == null)
                _cachedProperty = CreateProperty(propertyFactory);

            return _cachedProperty;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
        {
            var value = Assembly.GetEntryAssembly()?.GetName().Name;
            return propertyFactory.CreateProperty(SerilogPropertyNamesIdentifiers.ApplicationName, value);
        }
    }
}
