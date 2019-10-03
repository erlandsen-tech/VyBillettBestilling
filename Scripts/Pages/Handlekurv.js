function betal() {
    alert("Ikke implementert da dette er en demoside")
}
function slett(Id) {
    $.ajax({
        url: '/Handlekurv/Slett',
        type: 'DELETE',
        datatype: 'number',
        data: { 'Id': Id },
        success: function () {
            window.location.reload()
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
}