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
        public int ReiseFra { get; set; }
        public int ReiseTil { get; set; }
    }
}