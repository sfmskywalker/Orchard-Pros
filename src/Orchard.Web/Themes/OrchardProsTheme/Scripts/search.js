(function ($) {
    $(function() {
        var wrapper = $(".search-bar-wrapper");
        var form = wrapper.find("form");
        var searchButton = wrapper.find(".btn-search");
        var indexInput = wrapper.find("[name='index']");

        wrapper.on("click", "a[data-search-index]", function(e) {
            e.preventDefault();
            var button = $(this);
            var caption = button.data("caption");
            var searchIndex = button.data("search-index");

            indexInput.val(searchIndex);
            searchButton.text(caption);
        });
    });
})(jQuery);