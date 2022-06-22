

$("#Back").click(function () {
    var backCheck = confirm('確定返回嗎？');
    if (backCheck) {
        window.location.href = "/"
    } 
});
