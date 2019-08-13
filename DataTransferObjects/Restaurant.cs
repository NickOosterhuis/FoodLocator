using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Places.Search.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class Restaurant
    {
        public string Name { get; set; }
        public OpeningHours OpeningHours { get; set; }
        public double Rating { get; set; }
        public string Adress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PlaceId { get; set; }
        public string PhotoReference { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public bool OpenNow { get; set; }
        public IEnumerable<Period> OpeningPeriods { get; set; }
    }
}
