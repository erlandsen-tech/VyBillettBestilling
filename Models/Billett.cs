using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Billett
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Antall { get; set; }
        public string ReiseFra { get; set; }
        public string ReiseTil { get; set; }
    }
}