using System.Collections.Generic;
using System.Linq;
using Server.BusTimeApi.Models;

namespace Server.Services
{
    public class BusTimeProcessor
    {
        private readonly BusStopSchedule _schedule;

        public BusTimeProcessor(BusStopSchedule schedule)
        {
            _schedule = schedule;
        }

        public IEnumerable<All> GetNextThreeBuses()
        {
            return _schedule.departures.all.OrderBy(departure => departure.aimed_departure_time).Take(3);
        }
    }
}
