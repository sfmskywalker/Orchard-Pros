(function($) {
    $(function() {
        $("#replies").on("click", ".btn-solve-ticket", function (e) {
            var button = $(this);
            var replyId = button.data("reply");
            var popup = $("#solve-ticket-popup");

            popup.find("[name='ReplyId']").val(replyId);
            popup.show();
        });

        $(".overlay .popup .close").on("click", function () {
            var overlay = $(this).parents(".overlay:first");
            overlay.hide();
        });
    });
})(jQuery);