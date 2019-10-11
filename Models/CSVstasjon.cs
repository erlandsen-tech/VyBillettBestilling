using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;

namespace VyBillettBestilling.Models
{
    //Hjelpemodell for import av stasjonsliste fra CSV (semikolondelt) ved bygging av database
    [IgnoreFirst(1)]
    [DelimitedRecord(";")]
    public class CSVstasjon
    {
        public string srsName;
        public int srsDimension;
        public string ns1id2;
        public string ns2gyldigFra;
        public string ns2gyldigTil;
        public string ns2lokalId;
        public string ns2navnerom;
        public string ns2versjonId; // spesifisert som string, selv om den ser ut som int
        public string ns1id3;
        public string nettnavn;
        public string ns1pos;
        public double breddegrad;
        public double lengdegrad;
        public string ns2banekortnavn;
        public string ns2banenavn;
        public string ns2baneformål;
        public string ns2banestatus;
        public double ns2sporkilometer;
        public string ns2stasjonstype;
        public string ns2stasjonsnavn;

        public static List<CSVstasjon> convertEngine()
        {
            var engine = new FileHelperEngine<CSVstasjon>();
            // To Read Use:
            var result = engine.ReadFile("C:/Users/ivar/Source/Repos/varleg/VyBillettBestilling/SeedData/NorskeStasjoner.csv").ToList();
            return result;
        }
    }
}