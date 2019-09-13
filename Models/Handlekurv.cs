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
        public string HandlekurvId { get; set; }
        public int BillettId { get; set; }
        public int Antall { get; set; }
        public System.DateTime LagetDato { get; set; }
        public virtual Billett Billett { get; set; }
    }
}