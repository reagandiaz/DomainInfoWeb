using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DomainInfoService.Models
{
    public class ReportRequest
    {
        [Required]
        [DefaultValue(0)]
        public long Id { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Getpartial { get; set; }
    }
}
