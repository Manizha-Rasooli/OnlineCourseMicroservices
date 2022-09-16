using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models
{
    /// <summary>
    /// This class get Client datas from appsetting.json
    /// </summary>
    public class ClientSettings
    {
        public Client WebClient { get; set; }
        public Client WebClientForUser { get; set; }
    }

    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
