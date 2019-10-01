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
function LeggIHandleKurv(Id, voksen, barn, student, honnor) {
    oppdaterantall(voksen, barn, student, honnor);
    $.ajax({
        url: '/handlekurv/leggoppi',
        type: 'POST',
        data: { "Id": Id, "Voksen": voksen, "Barn": barn, "Student": student, "Honnor": honnor },
        traditional: true
    });
}

async function HentStasjonsNavn(id, i) {
    $.ajax({
        url: '/home/HentStasjonsnavnMedId',
        type: 'GET',
        data: { "id": id },
        datatype: 'string',
        success: function (navn) {
            console.log(navn);
            return navn;
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
function oppdaterantall()
{
    document.getElementById("handleikon").innerHTML = 'New Phrase';
}
