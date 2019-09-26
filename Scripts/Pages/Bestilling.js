function HentStasjoner(stasjonA, stasjonB) {
    $.ajax({
        url: '/home/reiserute',
        type: 'GET',
        data: { "stasjA": stasjonA, "stasjB": stasjonB },
        dataType: 'json',
        success: function (stasjon) {
            VisDropDown(stasjon);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}


function VisDropDown(stasjon) {
    var utStreng = "";
    $("#MuligeReiser").append("<tr>")
    for (var i in stasjon) {
        //Legger kun til stasjon i reiseTil listen dersom denne ikke er valg i reisefra
        utStreng += "<td>" + stasjon[i].stasjons_navn+ "</td>";
        //Sjekker om det finnes elementer i listen fra før
        $("#MuligeReiser").append(utStreng);
    }
    $("#MuligeReiser").append("</tr>")
}
