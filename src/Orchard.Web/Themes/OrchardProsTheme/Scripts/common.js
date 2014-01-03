(function($) {
    $(function() {
        $("a[data-submit-form").on("click", function (e) {
            $(this).parents("form:first").submit();
            e.preventDefault();
        });
    });
})(jQuery);