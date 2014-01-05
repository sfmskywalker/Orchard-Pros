(function ($) {

    var toggle = function(jQuery, show) {
        jQuery.toggleClass("invisible", !show);
    };

    $(function() {
        $("body").on("click", ".voting .rate a", function (e) {
            e.preventDefault();

            var link = $(this);
            var url = link.attr("href");
            var wrapper = link.closest("ul");

            $.ajax({
                url: url,
                type: "POST",
                data: { __RequestVerificationToken: $("[data-antiforgery-token]").data("antiforgery-token") }
            }).done(function(data) {
                var votes = wrapper.find(".votes");
                var positive = wrapper.find(".rate.positive");
                var negative = wrapper.find(".rate.negative");
                var votesClass = data.Points == 0 ? "neutral" : data.Points < 0 ? "negative" : "positive";
                var votesSign = data.Points == 0 ? "" : data.Points < 0 ? "-" : "+";

                wrapper.find(".number").text(votesSign + data.Points);
                votes.attr("class", "votes " + votesClass);
                toggle(positive, data.Caps.VoteUp);
                toggle(negative, data.Caps.VoteDown);
            });
        });
    });
})(jQuery);