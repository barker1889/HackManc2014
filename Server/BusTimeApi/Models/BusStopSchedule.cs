namespace Server.BusTimeApi.Models
{
    public class BusStopSchedule
    {
        public string atcocode { get; set; }
        public string smscode { get; set; }
        public string request_time { get; set; }
        public Departures departures { get; set; }
    }
}