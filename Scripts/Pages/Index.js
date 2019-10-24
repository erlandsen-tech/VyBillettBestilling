$(function () {
    $(".reiseTil").prop("disabled", true);
    $("#seleksjonsBoks").hide();
});

$("#ReiseFra").change(function () {
    $(".reiseTil").prop("disabled", false);
});

$(".reiseTil").change(function () {
    if ($(".reiseTil").val() == "") {
        $("#seleksjonsBoks").hide('slow¨');
    }
    else {
        $("#seleksjonsBoks").show('slow');
    }
})

$("#voksen").TouchSpin({
    min: 0,
    max: 100,
    step: 1
});

$("#barn").TouchSpin({
    min: 0,
    max: 100,
    step: 1
});

$("#student").TouchSpin({
    min: 0,
    max: 100,
    step: 1
});

$("#honnor").TouchSpin({
    min: 0,
    max: 100,
    step: 1
});
$(function () {

    $('#date').datetimepicker(
        {
            step: 60,
            minDate: 0,
            minTime: '00:00',
            locale: 'no',
        });

    $(".trigger").click(function () { $("#date").datetimepicker("show"); });

});

function VisDropDown(stasjon, nettId) {
    var utStreng = "";
    for (var i in stasjon) {
        //Legger kun til stasjon i reiseTil listen dersom denne ikke er valg i reisefra
        if (stasjon[i].id != $("#ReiseFra option:selected").val() &&
            nettId == stasjon[i].nett_id) {
            utStreng += "<option value='" + stasjon[i].id + "'>" + stasjon[i].stasjon_navn + "</option>";
        }
    }
    //Sjekker om det finnes elementer i listen fra før
    if ($("#ReiseFra option:selected").val() == "" && $("#ReiseFra option").length < 2) {
        $("#ReiseFra").append(utStreng);
    }
    else {
        $("#ReiseTil").append(utStreng);
    }
}
function skifte() {
    //Sørger for å tømme listen i ReiseTil ved skifte av frastasjon
    $("#ReiseTil").empty();
    VisReiseTil($("#ReiseFra option:selected").val());
}
function HentStasjoner(nettId) {
    $.ajax({
        url: '/home/stasjonsliste',
        type: 'GET',
        dataType: 'json',
        success: function (stasjon) {
            VisDropDown(stasjon, nettId);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}

function VisReiseTil(stasjon_id) {
    $.ajax({
        url: '/home/hentnettforstasjon',
        type: 'GET',
        dataType: 'json',
        data: {"id": stasjon_id},
        success: function (nettId) {
            HentStasjoner(nettId);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}
