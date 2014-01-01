(function($) {
    $(function() {
        $("#replies").on("click", ".btn-solve-ticket", function (e) {
            var button = $(this);
            var replyId = button.data("reply");
            var isOwnReply = button.data("reply-is-self");
            var popup = $("#solve-ticket-popup");

            popup.find("[name='ReplyId']").val(replyId);

            if (isOwnReply) {
                popup.find("#own-reply").show();
                popup.find("#other-reply").hide();
            } else {
                popup.find("#own-reply").hide();
                popup.find("#other-reply").show();
            }
            popup.show();
        });

        $(".overlay .popup .close").on("click", function () {
            var overlay = $(this).parents(".overlay:first");
            overlay.hide();
        });
    });
})(jQuery);