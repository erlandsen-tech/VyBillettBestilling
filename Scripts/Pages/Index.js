
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

})
