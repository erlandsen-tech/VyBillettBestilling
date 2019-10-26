using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VyBillettBestilling.ViewModels
{
    public class PrisOgBillett
    {
        public Pris Pris { get; set; }
        public List<Passasjer> Passasjerer { get; set; }
    }
}