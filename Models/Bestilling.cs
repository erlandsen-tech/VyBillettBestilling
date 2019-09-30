using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Bestilling
    {
        public string ReiseFra { get; set; }
        public string ReiseTil { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
        public int AntallStudent { get; set; }
        public DateTime DateTime { get; set; }
    }
}