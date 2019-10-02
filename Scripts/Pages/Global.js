function getAntallenheterIHandlekurv() {
    $.ajax({
        url: '/Handlekurv/EnheterIKurv',
        type: 'GET',
        datatype: 'string',
        success: function (enheter) {
            document.getElementById("handleikon").innerHTML = enheter;
            document.getElementById("antallHeader").innerHTML = " " + enheter;
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}
