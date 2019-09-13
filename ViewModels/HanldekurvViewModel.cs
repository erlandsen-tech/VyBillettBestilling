using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.ViewModels
{
    public class HanldekurvViewModel
    {

        public List<Handlekurv> HandlekurvInnhold { get; set; }
        public decimal TotalSum { get; set; }
    }
}