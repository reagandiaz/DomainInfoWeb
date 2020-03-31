using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DomainInfoService.Models
{
    public class CreateRequest
    {
        [Required]
        [DefaultValue("yahoo.com")]
        public string ip { get; set; }

        [Required]
        [DefaultValue(new string[] { "GeoIP", "Ping", "ReverseDNS" })]
        public string[] tasks { get; set; }

    }
}
