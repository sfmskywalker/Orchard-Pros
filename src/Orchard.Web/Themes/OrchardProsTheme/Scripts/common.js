(function($) {
    $(function() {
        $("a[data-submit-form]").on("click", function (e) {
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

        $("[data-controller]").each(function() {
            var controllee = $(this);
            var controller = $("#" + controllee.data("controller"));
            var groupName = controller.attr("name");
            var radios = $("input[name=\"" + groupName + "\"]:radio");

            radios.on("change", function () {
                if (controller.is(":checked")) {
                    controllee.removeClass("hidden").addClass("show");
                } else {
                    controllee.removeClass("show").addClass("hidden");
                }
            });
        });
    });
})(jQuery);