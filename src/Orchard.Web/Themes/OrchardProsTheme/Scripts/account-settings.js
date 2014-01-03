(function($) {
    $(function() {
        $("#delete-avatar").on("click", function (e) {
            $("#" + $(this).data("state-field")).val("true");
            $(this).hide();
            $("#current-avatar").hide();
            $("#avatar-removed-hint").toggleClass("hidden show");
        });

        $("#delete-wallpaper").on("click", function (e) {
            $("#" + $(this).data("state-field")).val("true");
            $(this).hide();
            $("#current-wallpaper").hide();
            $("#wallpaper-removed-hint").toggleClass("hidden show");
        });
    });
})(jQuery);