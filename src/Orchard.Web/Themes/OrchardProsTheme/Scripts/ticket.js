(function ($) {

    var smoothScroll = function(target) {
        var scrollToElement = $(target);

        var offset = scrollToElement.offset().top;
        $("html:not(:animated),body:not(:animated)").animate({ scrollTop: offset }, 500, function () {
            window.location.hash = target;
        });
    };

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

        $("body").on("click", "[data-reply-subject]", function (e) {
            e.preventDefault();

            var link = $(this);
            var replyId = link.data("reply-id");
            var subject = link.data("reply-subject");

            $("#post-reply input[name='Title']").val(subject);

            if (replyId) {
                $("#post-reply input[name='ContentItemId']").val(replyId);
            }

            smoothScroll(link.attr("href"));
        });
    });
})(jQuery);