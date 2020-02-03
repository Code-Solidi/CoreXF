using System;

using DateTimeService;

namespace ServiceExporter
{
    internal class DateTimeServiceImpl : IDateTimeService
    {
        public DateTime Get() => DateTime.UtcNow;
    }
}