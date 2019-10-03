using System;

namespace VyBillettBestilling.Models
{
    public class Billett
    {
        public int Id { get; set; }
        public Passasjer Passasjertype { get; set; }
        public int Antall { get; set; }
        public string StartStasjon { get; set; }
        public string StoppStasjon { get; set; }
        public double Rabattsats { get; set; }
        public double Pris { get; set; }
        public DateTime Avreise { get; set; }
        public DateTime Kjopsdato { get; set; }
    }
}