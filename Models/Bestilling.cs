using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.Models
{
    public class Bestilling
    {
        [Key]
        public int Id { get; set; }
        public List <Billett> Billetter { get; set; }
    }
}