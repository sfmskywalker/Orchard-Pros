(function ($) {
    $(function () {
        $(".mdd_editor").each(function () {
            var textArea = $(this);
            textArea.MarkdownDeep({
                help_location: textArea.data("help-location"),
                disableTabHandling: true,
                preview: textArea.data("preview")
            });
        });
    });
})(jQuery);