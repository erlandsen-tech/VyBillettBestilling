using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Math;

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
            // ..forkortet: (Radien er jordas middelverdi. Siden jorda ikke er helt rund blir det sma avvik)
            return 6371 * Sqrt(2 * (1 - (Sin(breddeA)*Sin(breddeB)) - (Cos(breddeA)*Cos(breddeB) * ((Sin(lengdeA)*Sin(lengdeB)) + (Cos(lengdeA)*Cos(lengdeB)))) ));
        }
        public static double overflateDistanseJorda(double breddeA, double lengdeA, double breddeB, double lengdeB)
        {
            // Beregner avstanden mellom to punkter pa jordoverflaten, i kilometer.
            // Bruker cosinussetningen (forenklet her, siden a og b er like) for a finne vinkelen (fra jordsenter) mellom punktene.
            // vinkel = acos(1 - c^2/2) // [c er indreDistanse].  Vinkelen kan omregnes til kilometer pa jordoverflaten, sma avvik siden jorda ikke er helt rund
            return 111.195 * Acos( (Sin(breddeA)*Sin(breddeB)) + (Cos(breddeA)*Cos(breddeB) * ((Sin(lengdeA)*Sin(lengdeB)) + (Cos(lengdeA)*Cos(lengdeB)))) );
        }
    }
}