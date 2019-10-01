using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    //trenger en rutetabell for togene
    //Denne benyttes til bestilling
    public class Rute
    {
        [Key]
        public int Id { get; set; }
        public int Start_id { get; set; }
        public int Stopp_id { get; set; }
        public List<Stasjon> sti { get; set; }
        public DateTime DateTime{ get; set; }
    }
}