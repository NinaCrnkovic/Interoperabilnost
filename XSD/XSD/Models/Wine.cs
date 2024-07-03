namespace XSD.Models
{
    using System.Diagnostics.Metrics;
    using System.Xml.Serialization;

    public class Wine
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [XmlElement("Description")]
        public string? Description { get; set; }

        [XmlElement("Category")]
        public string? Category { get; set; }

        [XmlElement("Type")]
        public string? Type { get; set; }

        [XmlElement("Country")]
        public string? Country { get; set; }

        [XmlElement("Vintage")]
        public int Vintage { get; set; }

        [XmlElement("Winery")]
        public string? Winery { get; set; }



        public override string ToString()
        {
            return $"Wine{{Id={Id}, Name={Name}, Description={Description}, Category={Category}, Type={Type}, Country={Country}, Vintage={Vintage}, Winery={Winery}}}";
        }
    }
}
