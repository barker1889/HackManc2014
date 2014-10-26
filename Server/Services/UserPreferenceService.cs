using System.Collections.Generic;
using System.Linq;

namespace Server.Services
{
    public class UserPreferenceService
    {
        private readonly List<BusRoutePreference> _fakeUserPreferences = new List<BusRoutePreference>();

        public UserPreferenceService()
        {
            _fakeUserPreferences = new List<BusRoutePreference>
                {
                    new BusRoutePreference
                        {
                            userTagId = "110057f2cd",
                            Routes = new List<BusRoute>
                                {
                                    new BusRoute
                                        {
                                            RouteNumber = "2",
                                            Direction = "Shudehill Interchange (Shudehill)"
                                        },
                                    new BusRoute
                                        {
                                            RouteNumber = "3",
                                            Direction = "Spinningfields, Quay Street/Deansgate"
                                        }
                                }
                        }
                };
        }

        public List<BusRoute> GetBusRoutesPreferencesForTag(string tagId)
        {
            var routePreferencesForUser = _fakeUserPreferences.SingleOrDefault(userPref => userPref.userTagId == tagId);
            return routePreferencesForUser != null
                       ? routePreferencesForUser.Routes
                       : new List<BusRoute>();
        }
    }

    public class BusRoutePreference
    {
        public string userTagId { get; set; }
        public List<BusRoute> Routes { get; set; }
    }

    public class BusRoute
    {
        public string RouteNumber { get; set; }
        public string Direction { get; set; }
    }
}
