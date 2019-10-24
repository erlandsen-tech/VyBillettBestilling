using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Math;
using static VyBillettBestilling.Models.VyDbContext;

namespace VyBillettBestilling.Models
{
    public class Utilities { }

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

    //public class AvstandComparer : Comparer<DbHovedstrekning>
    //{
    //    private readonly double sinbr, cosbr, sinlen, coslen;
    //    private DbStasjon fellesStasjon;
    //    public AvstandComparer(double bredde, double lengde, DbStasjon fellesStasjon)
    //    {
    //        sinbr = Sin(bredde); cosbr = Cos(bredde);
    //        sinlen = Sin(lengde); coslen = Cos(lengde);
    //        this.fellesStasjon = fellesStasjon;
    //    }

    //    public override int Compare(DbHovedstrekning x, DbHovedstrekning y)
    //    {
    //        DbStasjon xRettSt, yRettSt;
    //        if ((xRettSt = x.Stasjoner.First()) == fellesStasjon)
    //            xRettSt = x.Stasjoner.Last();
    //        else if (x.Stasjoner.Last() != fellesStasjon)
    //            throw new InvalidOperationException("AvstandComparer er uriktig initialisert. Er fellesStasjon satt riktig?");
    //        if ((yRettSt = y.Stasjoner.First()) == fellesStasjon)
    //            yRettSt = y.Stasjoner.Last();
    //        else if (y.Stasjoner.Last() != fellesStasjon)
    //            throw new InvalidOperationException("AvstandComparer er uriktig initialisert. Er fellesStasjon satt riktig?");
    //        double a = (Sin(xRettSt.Breddegrad) * sinbr) + (Cos(xRettSt.Breddegrad) * cosbr * ((Sin(xRettSt.Lengdegrad) * sinlen) + (Cos(xRettSt.Lengdegrad) * coslen)));
    //        double b = (Sin(yRettSt.Breddegrad) * sinbr) + (Cos(yRettSt.Breddegrad) * cosbr * ((Sin(yRettSt.Lengdegrad) * sinlen) + (Cos(yRettSt.Lengdegrad) * coslen)));
    //        return (a > b) ? -1 : (a < b) ? 1 : 0; // Storre a tilsvarer lavere x, siden acos (som er synkende) egentlig
    //                                               // skulle vaert utfort for vinkelberegning. Men det trengs ikke her.
    //    }

    //    public Comparer<DbHovedstrekning> oppdatertComparer(DbStasjon fellesStasjon)
    //    {
    //        this.fellesStasjon = fellesStasjon;
    //        return this;
    //    }

    //    public DbHovedstrekning[] SorterEtterEndedistanse(IEnumerable<DbHovedstrekning> strekninger)
    //    {
    //        DbHovedstrekning[] strekk = strekninger.ToArray();
    //        if (strekk.Length > 1)
    //        {
    //            DbStasjon a = strekk[0].Stasjoner.First();
    //            DbStasjon b = strekk[0].Stasjoner.Last();
    //            IEnumerable<DbStasjon> tmp;
    //            int i;
    //            for (i = 1; i < strekk.Length; ++i)
    //            {
    //                tmp = strekk[i].Stasjoner;
    //                if (tmp.First() != a && tmp.Last() != a)
    //                    a = null;
    //                if (tmp.First() != b && tmp.Last() != b)
    //                    b = null;
    //            }
    //            if (a == null && b == null)
    //                throw new ArgumentException("DbHovedstrekning-ene har ikke en felles endestasjon"); // Kan ikke sortere disse

    //            if (a == null || b == null) // Ellers har alle de samme (parvise) endepunktene, sa de trenger ikke sortering
    //            {
    //                if (b != null)
    //                    a = b; // setter a til a vaere den ene fellesstasjonen hvis den ikke er det fra for. b kan gjenbrukes fra her
    //                double[] nokler = new double[strekk.Length];
    //                for (i = 0; i < strekk.Length; ++i)
    //                {
    //                    tmp = strekk[i].Stasjoner;
    //                    if ((b = tmp.First()) == a)
    //                        b = tmp.Last(); // Setter - pa nokkelverdiene her, sa sorteringen blir riktig (stigende) med det samme.
    //                    nokler[i] = -(Sin(b.Breddegrad) * sinbr) - (Cos(b.Breddegrad) * cosbr * ((Sin(b.Lengdegrad) * sinlen) + (Cos(b.Lengdegrad) * coslen)));
    //                }
    //                Array.Sort(nokler, strekk);
    //            }
    //        }
    //        return strekk;
    //    }
    //}

}