(function($) {
    $(function() {
        $("[data-tags]").each(function() {
            var input = $(this);
            input.select2({
                multiple: true,
                minimumInputLength: 1,
                query: function (options) {
                    $.ajax({
                        url: input.data("tags") + "?term=" + options.term,
                        cache: false
                    }).then(function (tags) {
                        var results = $.map(tags, function(element) {
                            return {
                                id: element,
                                text: element
                            };
                        });

                        var data = {
                            more: false,
                            results: results
                        };
                        options.callback(data);
                    });
                    
                },
                createSearchChoice: function(term) {
                    return {
                        id: $.trim(term),
                        text: $.trim(term)
                    };
                },
                initSelection: function (element, callback) {
                    var data = [];
                    $(element.val().split(",")).each(function () {
                        data.push({ id: this, text: this });
                    });
                    callback(data);
                }
            });
        });
    });
})(jQuery);