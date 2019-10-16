$(function () {

    $('#Fodselsdato').datepicker(
        {
            maxDate: 0,
            locale: 'no',
            changeYear: true,
        });

    $(".trigger").click(function () { $("#Fodselsdato").datepicker("show"); });

});
