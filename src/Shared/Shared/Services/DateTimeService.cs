using Application.Interfaces;

namespace Shared.Services
{
    internal class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
