using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Math;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class Utilities
    {
    }

    public static class Klodeavstander
    {
        public static double indreDistanseJorda(double breddeA, double lengdeA, double breddeB, double lengdeB)
        {
            // Beregner avstanden mellom to punkter gjennom jorda, i kilometer. Ikke veldig praktisk her, men tar den med for a vise beregningen
            // https://en.wikipedia.org/wiki/World_Geodetic_System#WGS84
            // https://en.wikipedia.org/wiki/Geographic_coordinate_system
            // Avstand mellom to punkter i rommet:  sqrt( (Za-Zb)^2 + (Ya-Yb)^2 + (Xa-Xb)^2 )
            // ..pa en kule med bredde(B)- og lengde(L)grader: 
            // radien * sqrt( (sin(Bi)-sin(Bj))^2 + (sin(Li)*cos(Bi) - sin(Lj)*cos(Bj))^2 + (cos(Li)*cos(Bi) - cos(Lj)*cos(Bj))^2 )
            // ..forkortet: (Radien (6371) er jordas gjennomsnittsverdi. Siden jorda ikke er helt rund blir det sma avvik)
            double d = 2 * (1 - (Sin(breddeA) * Sin(breddeB)) - (Cos(breddeA) * Cos(breddeB) * ((Sin(lengdeA) * Sin(lengdeB)) + (Cos(lengdeA) * Cos(lengdeB)))));
            return (d <= 0) ? 0 : 6371 * Sqrt(d); // Sikring mot avrundingsfeil som gir ugyldig verdi til Sqrt
        }
        public static double overflateDistanseJorda(double breddeA, double lengdeA, double breddeB, double lengdeB)
        {
            // Beregner avstanden mellom to punkter pa jordoverflaten, i kilometer.
            // Bruker cosinussetningen (forenklet her, siden a og b er like) for a finne vinkelen (fra jordsenter) mellom punktene.
            // vinkel = acos(1 - c^2/2) // [c er indreDistanse].  Vinkelen kan omregnes til kilometer pa jordoverflaten,
            // sma avvik siden jorda ikke er helt rund. En grad tilsvarer 111.195 km ved gjennomsnittsverdi (6371 * 2 * pi / 360)
            double d = (Sin(breddeA) * Sin(breddeB)) + (Cos(breddeA) * Cos(breddeB) * ((Sin(lengdeA) * Sin(lengdeB)) + (Cos(lengdeA) * Cos(lengdeB))));
            return 111.195 * ((d >= 1) ? 0 : (d <= -1) ? 180 : Acos(d)); // Sikring mot avrundingsfeil som gir ugyldige verdier til Acos
        }
    }
}