using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Handlekurv
    {
        [Key]
        public int HandlekurvId { get; set; }
        public List<Billett> Billetter { get; set; }
        public System.DateTime SistOppdatert { get; set; }
    }
}