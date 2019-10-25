function informasjon() {
    $("#dialog-informasjon").removeClass('hidden');
    $("#dialog-informasjon").dialog({
        autoopen: false,
        resizable: false,
        height: "auto",
        width: 400,
        modal: true,
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        }
    });
}
function LagHovedstrekning() {
    var hovstr_navn = $('#hovstr_navn').val();
    var hovstr_kortnavn = $('#hovstr_kortnavn').val();
    var nettid = $('#nettid').val();
    var stasjonsliste = $('#multiselect').val();
    LagHovedstrekningMedInput(hovstr_navn, hovstr_kortnavn, nettid, stasjonsliste);
}

function LagHovedstrekningMedInput(hovstr_navn, hovstr_kortnavn, nettid, stasjonsliste) {
    $.ajax({
        url: '/Manage/StrekningCreate',
        type: 'POST',
        data: {
            "hovstr_navn": hovstr_navn, "hovstr_kortnavn": hovstr_kortnavn,
            "nettid": nettid, "stasjonsliste": stasjonsliste
        },
        traditional: true,
        success: function() {
            window.location.href = '/Manage/StrekningsListe'
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}
