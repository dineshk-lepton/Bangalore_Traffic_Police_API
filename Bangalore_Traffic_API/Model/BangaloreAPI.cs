using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangalore_Traffic_API.Model
{
    public class BangaloreAPI
    {        
            public int id { get; set; }
            public string last_modified_by { get; set; }

            public List<EventMapImages> event_map_images { get; set; }
          
            public string event_type { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string? endlatitude { get; set; }
            public string? endlongitude { get; set; }
            //public string location { get; set; }
        public string address { get; set; }
        public string event_cause { get; set; }
            public bool requires_road_closure { get; set; }
            public string start_datetime { get; set; }
            public string end_datetime { get; set; }
            public string status { get; set; }
            public string authenticated { get; set; }
            public string modified_datetime { get; set; }
            public string? map_file { get; set; }
            public string? direction { get; set; }
            public string description { get; set; }

    }

    
    public class EventMapImages
        {
            [JsonProperty("id")]
            public int image_id { get; set; }
            public string image { get; set; }

            [JsonProperty("event")]           
            public string? imgevent { get; set; }

            public int api_data_id { get; set; }
    }
}
