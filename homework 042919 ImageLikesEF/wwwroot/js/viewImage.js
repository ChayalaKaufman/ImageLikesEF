$(() => {

    setInterval(() => {
        const id = $("#like").data('id')
        $.get("/home/getLikes", { id: id }, function (likes) {
            $("#viewLikes").text(likes);
        })
    }, 1000)

    $("#like").on('click', function () {
        const id = $("#like").data('id');
        $.post('/home/like', { id: id }, function () {
            $("#like").prop('disabled', true);
            //$("#heart").Attr('glyphicon glyphicon-heart'); changing to full heart
        })
    })

})