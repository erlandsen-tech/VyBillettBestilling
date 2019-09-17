using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Reise
    {
        [Key]
        public int Id { get; set; }
        public string ReiseFra { get; set; }
        public string ReiseTil { get; set; }
    }
}