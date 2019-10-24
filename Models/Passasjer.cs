using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Passasjer
    {
        public int ptypId { get; set; }
        public double rabatt { get; set; }
        public string typenavn { get; set; }
        public double ovreAlder { get; set; }
        public double nedreAlder { get; set; }
    }
}