
$("#Delete").click(function () {
    var DeleteCheck = confirm('確定刪除嗎？');
    if (DeleteCheck) {
        let host = window.location.protocol + "//" + window.location.host + "/Home/DeleteAction";
        let Id = $("input[name='Id']").val();
        $.ajax({
            url: host,
            type: "post",
            async: false,
            data: {
                'Id': Id
            },
            beforeSend: function (xhr) {
                //for CSRF
                xhr.setRequestHeader("requestverificationtoken",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.item1) {
                    alert(result.item2);
                    window.location.href = "/";
                }
            },
            error: function () {
                console.log("Error.");
            }
        });

    }
});