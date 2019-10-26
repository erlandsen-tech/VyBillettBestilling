using System.Collections.Generic;

namespace VyBillettBestilling.ViewModels
{
    public class KonverterModel
    {
        public Stasjon stasjon(Model.Stasjon fra)
        {
            var til = new ViewModels.Stasjon();
            til.hovedstrekning_Ider = fra.hovedstrekning_Ider;
            til.id = fra.id;
            til.breddegrad = fra.breddegrad;
            til.lengdegrad = fra.lengdegrad;
            til.nett_id = fra.nett_id;
            til.stasjon_navn = fra.stasjon_navn;
            til.stasjon_sted = fra.stasjon_sted;
            return til;
        }
        public IEnumerable<Stasjon> stasjon(List<Model.Stasjon> fra)
        {
            var til = new List<Stasjon>();
            foreach(Model.Stasjon frastasj in fra)
            {
                til.Add(stasjon(frastasj));
            }
            return til;
        }
        public Hovedstrekning hovedstrekning(Model.Hovedstrekning fra)
        {
            var til = new Hovedstrekning();
            til.hovstr_kortnavn = fra.hovstr_kortnavn;
            til.hovstr_navn = fra.hovstr_navn;
            til.id = fra.id;
            til.nett_id = fra.nett_id;
            til.stasjon_Ider = fra.stasjon_Ider;
            return til;
        }
        public List<Hovedstrekning> hovedstrekning(List<Model.Hovedstrekning> fra)
        {
            var til = new List<Hovedstrekning>();
            foreach(Model.Hovedstrekning frastrk in fra)
            {
                til.Add(hovedstrekning(frastrk));
            }
            return til;
        }
        public Nett nett(Model.Nett fra)
        {
            var til = new Nett();
            til.hovedstrekning_Ider = fra.hovedstrekning_Ider;
            til.id = fra.id;
            til.nett_navn = fra.nett_navn;
            til.stasjon_Ider = fra.stasjon_Ider;
            return til;
        }
        public List<Nett> nett(List<Model.Nett> fra)
        {
            var til = new List<Nett>();
            foreach(Model.Nett fraNett in fra)
            {
                til.Add(nett(fraNett));
            }
            return til;
        }
        //public Handlekurv handlekurv(Model.Handlekurv fra)
        //{
        //    var til = new Handlekurv();
        //    til.Billetter = fra.Billetter;
        //    til.HandlekurvId = fra.HandlekurvId;
        //    til.SistOppdatert = fra.SistOppdatert;
        //    return til;
        //}
        public HovedstrekningCreateView hvstCreatevIew (Model.HovedstrekningCreateView fra)
        {
            var til = new HovedstrekningCreateView();
            til.hovstr_kortnavn = fra.hovstr_kortnavn;
            til.hovstr_navn = fra.hovstr_navn;
            til.nettid = fra.nettid;
            til.stasjonsliste = fra.stasjonsliste;
            return til;
        }
        public Passasjer passasjer (Model.Passasjer fra)
        {
            var til = new Passasjer();
            til.nedreAlder = fra.nedreAlder;
            til.ovreAlder = fra.ovreAlder;
            til.ptypId = fra.ptypId;
            til.rabatt = fra.rabatt;
            til.typenavn = fra.typenavn;
            return til;
        }
        public List<Passasjer> passasjer(List<Model.Passasjer> fra)
        {
            var til = new List<Passasjer>();
            foreach(Model.Passasjer fraPass in fra)
            {
                til.Add(passasjer(fraPass));
            }
            return til;
        }
        public Pris pris (Model.Pris fra)
        {
            var til = new Pris();
            til.Id = fra.Id;
            til.prisPrKm = fra.prisPrKm;
            return til;
        }
        public Rute rute (Model.Rute fra)
        {
            var til = new Rute();
            til.DateTime = fra.DateTime;
            til.Id = fra.Id;
            til.Start_id = fra.Start_id;
            til.Stopp_id = fra.Stopp_id;
            return til;
        }
        public RuteView ruteView (Model.RuteView fra)
        {
            var til = new RuteView();
            til.Id = fra.Id;
            til.StartId = fra.StartId;
            til.DateTime = fra.DateTime;
            til.StartNavn = fra.StartNavn;
            til.StoppId = fra.StoppId;
            til.StoppNavn = fra.StoppNavn;
            til.AntallBarn = fra.AntallBarn;
            til.AntallHonnor = fra.AntallHonnor;
            til.AntallStudent = fra.AntallStudent;
            til.AntallVoksne = fra.AntallVoksne;
            return til;
        }
        public List<RuteView> ruteView (List<Model.RuteView> fra)
        {
            var til = new List<RuteView>();
            foreach(Model.RuteView fraRvw in fra)
            {
                til.Add(ruteView(fraRvw));
            }
            return til;
        }
        public HovedstrekningCreateView hovedstrekningCreateView (Model.HovedstrekningCreateView fra)
        {
            var til = new HovedstrekningCreateView();
            til.hovstr_kortnavn = fra.hovstr_kortnavn;
            til.hovstr_navn = fra.hovstr_navn;
            til.nettid = fra.nettid;
            til.stasjonsliste = fra.stasjonsliste;
            return til;
        }
        public Model.Stasjon stasjon(Stasjon fra)
        {
            var til = new Model.Stasjon();
            til.hovedstrekning_Ider = fra.hovedstrekning_Ider;
            til.id = fra.id;
            til.breddegrad = fra.breddegrad;
            til.lengdegrad = fra.lengdegrad;
            til.nett_id = fra.nett_id;
            til.stasjon_navn = fra.stasjon_navn;
            til.stasjon_sted = fra.stasjon_sted;
            return til;
        }
        public IEnumerable<Model.Stasjon> stasjon(List<Stasjon> fra)
        {
            var til = new List<Model.Stasjon>();
            foreach(Stasjon frastasj in fra)
            {
                til.Add(stasjon(frastasj));
            }
            return til;
        }
        public Model.Hovedstrekning hovedstrekning(Hovedstrekning fra)
        {
            var til = new Model.Hovedstrekning
            {
                hovstr_kortnavn = fra.hovstr_kortnavn,
                hovstr_navn = fra.hovstr_navn,
                id = fra.id,
                nett_id = fra.nett_id,
                stasjon_Ider = fra.stasjon_Ider
            };
            return til;
        }
        public List<Model.Hovedstrekning> hovedstrekning(List<Hovedstrekning> fra)
        {
            var til = new List<Model.Hovedstrekning>();
            foreach(Hovedstrekning frastrk in fra)
            {
                til.Add(hovedstrekning(frastrk));
            }
            return til;
        }
        public Model.Nett nett(Nett fra)
        {
            var til = new Model.Nett();
            til.hovedstrekning_Ider = fra.hovedstrekning_Ider;
            til.id = fra.id;
            til.nett_navn = fra.nett_navn;
            til.stasjon_Ider = fra.stasjon_Ider;
            return til;
        }
        public List<Model.Nett> nett(List<Nett> fra)
        {
            var til = new List<Model.Nett>();
            foreach(Nett fraNett in fra)
            {
                til.Add(nett(fraNett));
            }
            return til;
        }

       // public Model.Handlekurv handlekurv(Handlekurv fra)
       // {
       //     var til = new Model.Handlekurv();
       //     til.Billetter = fra.Billetter;
       //     til.HandlekurvId = fra.HandlekurvId;
       //     til.SistOppdatert = fra.SistOppdatert;
       //     return til;
       // }
        public Model.HovedstrekningCreateView hvstCreatevIew (HovedstrekningCreateView fra)
        {
            var til = new Model.HovedstrekningCreateView();
            til.hovstr_kortnavn = fra.hovstr_kortnavn;
            til.hovstr_navn = fra.hovstr_navn;
            til.nettid = fra.nettid;
            til.stasjonsliste = fra.stasjonsliste;
            return til;
        }
        public Model.Passasjer passasjer (Passasjer fra)
        {
            var til = new Model.Passasjer();
            til.nedreAlder = fra.nedreAlder;
            til.ovreAlder = fra.ovreAlder;
            til.ptypId = fra.ptypId;
            til.rabatt = fra.rabatt;
            til.typenavn = fra.typenavn;
            return til;
        }
        public List<Model.Passasjer> passasjer(List<Passasjer> fra)
        {
            var til = new List<Model.Passasjer>();
            foreach(Passasjer fraPass in fra)
            {
                til.Add(passasjer(fraPass));
            }
            return til;
        }
        public Model.Pris pris (Pris fra)
        {
            var til = new Model.Pris();
            til.Id = fra.Id;
            til.prisPrKm = fra.prisPrKm;
            return til;
        }
        public Model.Rute rute (Rute fra)
        {
            var til = new Model.Rute();
            til.DateTime = fra.DateTime;
            til.Id = fra.Id;
            til.Start_id = fra.Start_id;
            til.Stopp_id = fra.Stopp_id;
            return til;
        }
        public Model.RuteView ruteView (RuteView fra)
        {
            var til = new Model.RuteView();
            til.Id = fra.Id;
            til.StartId = fra.StartId;
            til.StartNavn = fra.StartNavn;
            til.StoppId = fra.StoppId;
            til.StoppNavn = fra.StoppNavn;
            til.AntallBarn = fra.AntallBarn;
            til.AntallHonnor = fra.AntallHonnor;
            til.AntallStudent = fra.AntallStudent;
            til.AntallVoksne = fra.AntallVoksne;
            return til;
        }
        public List<Model.RuteView> ruteView (List<RuteView> fra)
        {
            var til = new List<Model.RuteView>();
            foreach(RuteView fraRvw in fra)
            {
                til.Add(ruteView(fraRvw));
            }
            return til;
        }

        public Model.HovedstrekningCreateView hovedstrekningCreateView (HovedstrekningCreateView fra)
        {
            var til = new Model.HovedstrekningCreateView();
            til.hovstr_kortnavn = fra.hovstr_kortnavn;
            til.hovstr_navn = fra.hovstr_navn;
            til.nettid = fra.nettid;
            til.stasjonsliste = fra.stasjonsliste;
            return til;
        }
    }
}