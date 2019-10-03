using System;

namespace VyBillettBestilling.Models
{
    public class Bestilling
    {
        public int ReiseFra { get; set; }
        public int ReiseTil { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
        public int AntallStudent { get; set; }
        public int AntallHonnor { get; set; }
        public DateTime StartTid { get; set; }
    }
}