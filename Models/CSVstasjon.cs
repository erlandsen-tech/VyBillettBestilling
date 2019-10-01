using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;

namespace VyBillettBestilling.Models
{
    [DelimitedRecord(";")]
    public class CSVstasjon
    {
        public int id;
        public string name;
        public string slug;
        public string uic;
        public string uic8_sncf;
        public double latitude;
        public double longitude;
        public string parent_station_id;
        public string country;
        public string time_zone;
        public string is_city;
        public string is_main_station;
        public string is_airport;
        public string is_suggestable;
        public string country_hint;
        public string main_station_hint;
        public string sncf_id;
        public string sncf_tvs_id;
        public string sncf_is_enabled;
        public string idtgv_id;
        public string idtgv_is_enabled;
        public string db_id;
        public string db_is_enabled;
        public string busbud_id;
        public string busbud_is_enabled;
        public string distribusion_id;
        public string distribusion_is_enabled;
        public string flixbus_id;
        public string flixbus_is_enabled;
        public string cff_id;
        public string cff_is_enabled;
        public string leoexpress_id;
        public string leoexpress_is_enabled;
        public string obb_id;
        public string obb_is_enabled;
        public string ouigo_id;
        public string ouigo_is_enabled;
        public string trenitalia_id;
        public string trenitalia_is_enabled;
        public string trenitalia_rtvt_id;
        public string ntv_rtiv_id;
        public string ntv_id;
        public string ntv_is_enabled;
        public string hkx_id;
        public string hkx_is_enabled;
        public string renfe_id;
        public string renfe_is_enabled;
        public string atoc_id;
        public string atoc_is_enabled;
        public string benerail_id;
        public string benerail_is_enabled;
        public string westbahn_id;
        public string westbahn_is_enabled;
        public string sncf_self_service_machine;
        public string same_as;
        public string info_de;
        public string info_en;
        public string info_es;
        public string info_fr;
        public string info_it;
        public string info_nb;
        public string info_nl;
        public string info_cs;
        public string info_da;
        public string info_hu;
        public string info_ja;
        public string info_ko;
        public string info_pl;
        public string info_pt;
        public string info_ru;
        public string info_sv;
        public string info_tr;
        public string info_zh;
        public string normalised_code;
        public static List<CSVstasjon> convertEngine()
        {
            var engine = new FileHelperEngine<CSVstasjon>();
            // To Read Use:
            var result = engine.ReadFile("C:/Users/johni/Source/Repos/varleg/VyBillettBestilling/SeedData/NorskeStasjoner.csv").ToList();
            return result;
        }
    }
}