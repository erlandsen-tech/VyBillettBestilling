using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VyBillettBestilling.Models;

namespace VyBillettBestilling.Models
{
    public class DummyData
    {
        public DummyData()
        {

            VyDbContext.DbStasjon OsloS = new VyDbContext.DbStasjon();
            VyDbContext.DbStasjon KrSand = new VyDbContext.DbStasjon();
            VyDbContext.DbStasjon Stavanger = new VyDbContext.DbStasjon();
            VyDbContext.DbStasjon Bergen = new VyDbContext.DbStasjon();
            VyDbContext.DbDelstrekning OsloKrSand = new VyDbContext.DbDelstrekning();
            VyDbContext.DbDelstrekning KrSandStavanger = new VyDbContext.DbDelstrekning();
            VyDbContext.DbDelstrekning StavangerBergen = new VyDbContext.DbDelstrekning();

            OsloS.StasjNavn = "OsloS";
            OsloS.StasjonId = 0;
            KrSand.StasjNavn = "Kristiansand Stasjon";
            KrSand.StasjonId = 1;
            Stavanger.StasjNavn = "Stavanger";
            Stavanger.StasjonId = 2;
            Bergen.StasjNavn = "Bergen";
            Bergen.StasjonId = 3;
            OsloKrSand.DelstrId = 0;
            OsloKrSand.Start = OsloS;
            OsloKrSand.Stopp = KrSand;
            KrSandStavanger.DelstrId = 1;
            KrSandStavanger.Start = KrSand;
            KrSandStavanger.Stopp = Stavanger;
            StavangerBergen.DelstrId = 2;
            StavangerBergen.Start = Stavanger;
            StavangerBergen.Stopp = Bergen;
        }
    }
}