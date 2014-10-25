using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Server.BusTimeApi.Models;

namespace Server.BusTimeApi
{
    public class BusScheduleWrapper
    {
        private readonly string _atcoCode;

        public BusScheduleWrapper(string atcoCode)
        {
            _atcoCode = atcoCode;
        }

        public BusStopSchedule GetBusTimes(DateTime requestTime)
        {
            var day = requestTime.ToString("yyyy-MM-dd");
            var time = requestTime.ToString("HH:mm");
            
            var apiUrl = string.Format("http://transportapi.com/v3/uk/bus/stop/{0}/{1}/{2}/timetable.json?group=no&api_key=14b9f6c556fb28ce24e28b4322d0191a&app_id=bb1956b4", _atcoCode, day, time);
            
            var request = WebRequest.Create(apiUrl);
            string text;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            var model = JsonConvert.DeserializeObject<BusStopSchedule>(text);

            return model;
        }
    }
}
