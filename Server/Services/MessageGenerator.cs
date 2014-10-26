using System;
using System.Collections.Generic;
using System.Text;
using Server.BusTimeApi.Models;

namespace Server.Services
{
    public class MessageGenerator
    {
        public string GenerateNextDeparturesMessage(string busStopName, IEnumerable<All> nextDepartures)
        {
            var messageStart = string.Format("The next three buses to depart from {0} are, ", busStopName);

            var busTimes = new StringBuilder();

            foreach (var departure in nextDepartures)
            {
                var departureTime = DateTime.Parse(departure.aimed_departure_time);

                var friendlyDepartureTime = departureTime.ToString("HH mm");

                busTimes.AppendFormat("the {0} at {1}, ", departure.line, friendlyDepartureTime);
            }

            return messageStart + busTimes + ". Double tap to repeat.";
        }
    }
}
