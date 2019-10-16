function GetInfo() {
    $.ajax({
        url: '/Account/GetUserInfo',
        type: 'GET',
        dataType: 'json',
        success: function () {
            alert("fikk det til!");
        },
        error: function (x, y, z) {
            alert(x + '\n' + y + '\n' + z);
        }
    });
