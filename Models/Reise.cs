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
        public int StartId { get; set; }
        public int StoppId { get; set; }
        public string ReiseFra { get; set; }
        public string ReiseTil { get; set; }
        public DateTime StartTid { get; set; }
        public DateTime StoppTid { get; set; }
        public List<Stasjon> ReiseRute { get; set; }
    }
}