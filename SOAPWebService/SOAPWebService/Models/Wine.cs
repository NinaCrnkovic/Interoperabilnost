using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOAPWebService.Models
{
    public class Wine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public int Vintage { get; set; }
        public string Winery { get; set; }
    }
}

