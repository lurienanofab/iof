(function ($) {
    var DisplayOption = {
        "Detail": 0,
        "Summary": 1
    };

    $.fn.iofsearch = function (options) {
        return this.each(function () {
            var $this = $(this);

            var opts = $.extend({}, { "action": "", "statusIdList": "", "displayOption": DisplayOption.Detail, "clientId": 0 }, options, $this.data());

            var summary = opts.displayOption === DisplayOption.Summary;
            var detail = !summary;

            var getStartDate = function () {
                var value = $(".start-date-text", $this).val();
                if (value) {
                    var d = new Date(value);
                    var m = moment([d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate()]);
                    if (m.isValid()) {
                        return m.format("YYYY-MM-DD");
                    }
                }

                return null;
            };

            var getEndDate = function () {
                var value = $(".end-date-text", $this).val();

                if (value) {
                    var d = new Date(value);
                    var m = moment([d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate()]);
                    if (m.isValid()) {
                        m = m.clone().add(1, 'days'); // always add 1 day to the end date
                        return m.format("YYYY-MM-DD");
                    }
                }

                return null;
            };

            var getFields = function () {
                return {
                    "clientId": opts.clientId,
                    "statusIdList": opts.statusIdList,
                    "startDate": getStartDate(),
                    "endDate": getEndDate(),
                    "vendorId": parseInt($(".vendor-name-list", $this).val()) || 0,
                    "vendorName": $(".vendor-name-text", $this).val(),
                    "keywords": $(".keywords", $this).val(),
                    "partNumber": $(".part-num", $this).val(),
                    "poid": parseInt($(".poid", $this).val()) || 0,
                    "shortCode": $(".shortcode", $this).val(),
                    "otherClientId": parseInt($(".other-client-id", $this).val()) || 0,
                    "includeSelf": $(".include-self", $this).prop("checked"),
                    "displayOption": opts.displayOption
                };
            };

            var stripAlpha = function (input) {
                if (input === null) return 0;
                var numbers = '0123456789';
                var result = '';
                for (x = 0; x < input.length; x++) {
                    if (numbers.indexOf(input[x]) !== -1)
                        result += input[x];
                }
                return parseInt(result);
            };

            var getCategory = function (row, type, set, meta) {
                return "[" + row.CategoryNumber + "] " + row.CategoryName; 
            };

            var getCreatedDate = function (row, type, set, meta) {
                return moment(row.CreatedDate).format("MM/DD/YYYY");
            };

            var getTotalPrice = function (row, type, set, meta) {
                return numeral(row.TotalPrice).format("$0,0.00");
            };

            var getPOID = function (row, type, set, meta) {
                var url = opts.action.replace(/{poid}/g, row.POID);
                return '<a href="' + url + '">' + row.POID + '</a>';
            };

            var getData = function (data, callback, settings) {
                $.extend(data, getFields());

                $.ajax({
                    "url": "ajax/datatables.ashx",
                    "type": "POST",
                    "contentType": "application/json",
                    "data": JSON.stringify(data)
                }).done(function (d) {
                    callback(d);
                });
            };

            var table = $(".datatable", $this).DataTable({
                "pagingType": "full_numbers",
                "pageLength": 10,
                "stateSave": true,
                "processing": true,
                "autoWidth": false,
                "order": [[7, "desc"]],
                "serverSide": false,
                "ajax": getData,
                "columns": [
                    { "name": "POID", "data": getPOID },
                    { "name": "DisplayName", "data": "DisplayName", "visible": summary },
                    { "name": "ApproverDisplayName", "data": "ApproverDisplayName", "visible": summary },
                    { "name": "PartNum", "data": "PartNum", "visible": detail },
                    { "name": "Description", "data": "Description", "visible": detail },
                    { "name": "Category", "data": getCategory, "visible": detail },
                    { "name": "VendorName", "data": "VendorName" },
                    { "name": "CreatedDate", "data": getCreatedDate, "className": "text-center" },
                    { "name": "ShortCode", "data": "ShortCode", "className": "text-center" },
                    { "name": "TotalPrice", "data": getTotalPrice, "className": "text-right" },
                    { "name": "StatusName", "data": "StatusName" }
                ],
                "language": {
                    "search": "Search:",
                    "processing": "Loading...",
                    "emptyTable": "No IOFs were found."
                },
                "initComplete": function (settings, json) {
                    var wrapper = this.closest(".dataTables_wrapper");
                    $(".dataTables_paginate", wrapper).css({
                        "-khtml-user-select": "none",
                        "-o-user-select": "none"
                    });
                }
            });

            $this.on("click", ".vendor-toggle", function (e) {
                e.preventDefault();

                if ($(this).data("vendorSearchType") === "text") {
                    $(this).data("vendorSearchType", "list").html("list");
                    // show the text input
                    $(".vendor-list-container", $this).hide();
                    $(".vendor-text-container", $this).show();
                    $(".vendor-name-text", $this).focus();
                } else {
                    $(this).data("vendorSearchType", "text").html("text");
                    // show the select input
                    $(".vendor-text-container", $this).hide();
                    $(".vendor-list-container", $this).show();
                    $(".vendor-name-text", $this).val("");
                }
            }).on("click", ".search-button", function (e) {
                table.ajax.reload();
            }).on("click", ".clear-button", function (e) {
                $(".vendor-name-text", $this).val("");
                $(".vendor-name-list", $this).find("option[value='-1']").prop("selected", true);
                $(".keywords", $this).val("");
                $(".part-num", $this).val("");
                $(".poid", $this).val("");
                $(".shortcode", $this).val("");
                $(".other-client-id", $this).find("option[value='" + opts.clientId + "']").prop("selected", true);
                table.ajax.reload();
            }).on("keypress", function (e) {
                if (e.which === 13) {
                    e.preventDefault();
                    table.ajax.reload();
                }
            });
        });
    };
})(jQuery);