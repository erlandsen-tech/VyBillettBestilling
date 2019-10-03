//Avreise må sendes i millisekunder siden 010170 for å unngå konverteringsfeil
function LeggIHandleKurv(StartId, StoppId, voksen, barn, student, honnor, avreise) {
    dialog();
    $.ajax({
        url: '/handlekurv/leggoppi',
        type: 'POST',
        data: {
            "StoppId": StoppId, "StartId": StartId, "Voksen": voksen,
            "Barn": barn, "Student": student, "Honnor": honnor,
            "avreise": avreise
        },
        traditional: true
    });
}

function HentStasjonsNavn(id, i) {
    $.ajax({
        url: '/home/HentStasjonsnavnMedId',
        type: 'GET',
        data: { "id": id },
        datatype: 'string',
        success: function (navn) {
            return navn;
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

/**
function VisDropDown(stasjon) {
    var utStreng = "";
    $("#MuligeReiser").append("<tr>")
    for (var i in stasjon) {
        //Legger kun til stasjon i reiseTil listen dersom denne ikke er valg i reisefra
        utStreng += "<td>" + stasjon[i].stasjons_navn + "</td>";
        //Sjekker om det finnes elementer i listen fra før
        $("#MuligeReiser").append(utStreng);
    }
    $("#MuligeReiser").append("</tr>")
}
**/
function oppdaterantall() {
    getAntallenheterIHandlekurv();
}
function dialog() {
    $("#dialog-confirm").dialog({
        autoopen: false,
        resizable: false,
        height: "auto",
        width: 400,
        modal: true,
        buttons: {
            "Gå til handlekurv": function () {
                $(this).dialog("close");
                window.location.href = '/Handlekurv/Handlekurv'
            },
            "Velg flere": function () {
                oppdaterantall();
                window.location.reload();
                $(this).dialog("close");

            },
            "Ny reise": function () {
                oppdaterantall();
                $(this).dialog("close");
                window.location.href = '/Home'
            }
        }
    });
};
