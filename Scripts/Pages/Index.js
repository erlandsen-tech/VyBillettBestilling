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
            step: 5,
            minDate: 0,
            minTime: '00:00',
            locale: 'no',
        });

    $(".trigger").click(function () { $("#date").datetimepicker("show"); });

});

function VisDropDown(stasjon) {
    var utStreng = "";
    for (var i in stasjon) {
        //Legger kun til stasjon i reiseTil listen dersom denne ikke er valg i reisefra
        if (stasjon[i].id != $("#ReiseFra option:selected").val()) {
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
    HentStasjoner();
}
function HentStasjoner() {
    $.ajax({
        url: '/home/stasjonsliste',
        type: 'GET',
        dataType: 'json',
        success: function (stasjon) {
            VisDropDown(stasjon);
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}
