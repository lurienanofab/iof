(function ($) {
    function DateRange(range) {
        var sd = null;
        var ed = null;

        switch (range) {
            case "yesterday":
                sd = moment().startOf("day").subtract(1, "days");
                ed = moment().startOf("day").subtract(1, "days");
                break;
            case "this-week":
                sd = moment().startOf("week");
                ed = sd.clone().add(1, "weeks").subtract(1, "days");
                break;
            case "last-week":
                sd = moment().startOf("week").subtract(1, "weeks");
                ed = sd.clone().add(1, "weeks").subtract(1, "days");
                break;
            case "this-month":
                sd = moment().startOf("month");
                ed = sd.clone().add(1, "months").subtract(1, "days");
                break;
            case "last-month":
                sd = moment().startOf("month").subtract(1, "months");
                ed = sd.clone().add(1, "months").subtract(1, "days");
                break;
            case "this-year":
                sd = moment().startOf("year");
                ed = sd.clone().add(1, "years").subtract(1, "days");
                break;
            case "last-year":
                sd = moment().startOf("year").subtract(1, "years");
                ed = sd.clone().add(1, "years").subtract(1, "days");
                break;
            default: //today
                range = "today";
                sd = moment().startOf("day");
                ed = moment().startOf("day");
                break;
        }

        this.range = range;
        this.start = sd;
        this.end = ed;

        var self = this;

        this.toString = function () {
            return self.range + ": " + self.start.format("MM/DD/YYYY") + " to " + self.end.format("MM/DD/YYYY");
        };
    }

    $.fn.purchsearch = function (options) {
        return this.each(function () {
            var $this = $(this);

            var opt = $.extend({}, { default: 0 }, options, $this.data());

            $(".start-date", $this).attr("placeholder", "MM/DD/YYYY");
            $(".end-date", $this).attr("placeholder", "MM/DD/YYYY");

            var getDateRange = function (range) {
                return new DateRange(range);
            };

            var setDates = function (range) {
                if (!range) {
                    $(".start-date", $this).val("").focus();
                    $(".end-date", $this).val("");
                } else {
                    var dateRange = getDateRange(range);
                    $(".start-date", $this).val(dateRange.start.format("MM/DD/YYYY"));
                    $(".end-date", $this).val(dateRange.end.format("MM/DD/YYYY"));
                }
            };

            $this.on("change", ".date-preset-select", function (e) {
                setDates($(this).val());
            }).on("click", ".clear-date", function (e) {
                e.preventDefault();
                $(".date-preset-select option:eq(0)", $this).prop("selected", true);
                setDates($(".date-preset-select", $this).val());
            });

            $this.data("functions", { "getDateRange": getDateRange });
        });
    };
}(jQuery));