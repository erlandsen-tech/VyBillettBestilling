using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.ViewModels
{
    //trenger en rutetabell for togene
    //Denne benyttes til bestilling
    // og visning av togtider
    public class RuteView
    {
        [Key]
        public int Id { get; set; }
        public string StartNavn { get; set; }
        public string StoppNavn { get; set; }
        public int StartId { get; set; }
        public int StoppId { get; set; }
        public DateTime DateTime{ get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
        public int AntallStudent { get; set; }
        public int AntallHonnor { get; set; }
    }
}