
$(document).ready(function () {
    $('.reiseFra').select2({
        placeholder: "Reise Fra",
        allowClear: true
    });
    $('.reiseTil').select2({
        placeholder: "Reise Til",
        allowClear: true

    });
    $(".reiseTil").prop("disabled", true);
    $("#seleksjonsBoks").hide();
});

$(".reiseFra").change(function () {
    if ($(".reiseFra").val() == "") {

        $(".reiseTil").prop("disabled", true);
    }
    else {
        $(".reiseTil").prop("disabled", false);
    }
});

$(".reiseTil").change(function () {
    if ($(".reiseTil").val() == "") {
        $("#seleksjonsBoks").hide( 'slow¨' );
    }
    else {
        $("#seleksjonsBoks").show( 'slow' );
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
$('#date').datetimepicker(
    {
        step: 5,
        minDate: 0,
        minTime: '00:00',
        locale: 'no'
    });

