(function($) {
    $(function() {
        $("a[data-submit-form").on("click", function (e) {
            $(this).parents("form:first").submit();
            e.preventDefault();
        });

        $("a[data-post]").on("click", function (e) {
            e.preventDefault();

            var link = $(this);
            var prompt = link.data("prompt");

            if (prompt && prompt.length > 0) {
                if (!confirm(prompt)) {
                    return;
                }
            }

            var url = link.attr("href");
            var token = link.data("post");
            var form = $("<form method=\"post\" action=\"" + url + "\"><input type=\"hidden\" name=\"__RequestVerificationToken\" value=\"" + token + "\" /></form>");

            $("body").append(form);
            form.submit();
        });
    });
})(jQuery);