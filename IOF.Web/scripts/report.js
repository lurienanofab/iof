(function ($) {
    var moneyFormat = "$0,0.00";
    var dateFormat = "MM/DD/YYYY h:mm A";

    var render = {
        "date": function (data, type, row) {
            if (type === "display" || type === "filter") {
                if (data)
                    return moment(data).format(dateFormat);
                else
                    return "never";
            }

            return data;
        },
        "money": function (data, type, row) {
            if (type === "display" || type === "filter") {
                if (data)
                    return numeral(data).format(moneyFormat);
                else
                    return "";
            }

            return data;
        }
    };

    var reports = {
        "store-manager-report": {
            "stateSave": false,
            "autoWidth": false,
            "order": [[0, "desc"]],
            "columns": [
                { "data": "LastOrdered", "render": render.date, "width": "140px" },
                { "data": "Unit" },
                { "data": "UnitPrice", "render": render.money, "className": "text-right" },
                { "data": "StorePackagePrice", "render": render.money, "className": "text-right" },
                { "data": "StorePackageQuantity", "className": "text-right" },
                { "data": "StoreUnitPrice", "render": render.money, "className": "text-right" },
                { "data": "LastPurchased", "render": render.date, "width": "140px", "className": "text-right" },
                { "data": "ItemID", "visible": false },
                { "data": "StoreItemID", "visible": false },
                { "data": "VendorName", "visible": false },
                { "data": "Description", "visible": false },
                { "data": "StoreDescription", "visible": false }
            ],
            "createdRow": function (row, data, dataIndex) {
                var r = this.api().row(row);

                var getItemName = function (id, desc) {
                    if (id)
                        return ('[' + id + '] ' + desc).trim();
                    else
                        return '';
                };

                var html =
                    '<div class="row">'
                    + '<div class="col-xs-2 text-right"><strong>IOF Vendor</strong></div>'
                    + '<div class="col-xs-10">' + data.VendorName + '</div>'
                    + '</div>'
                    + '<div class="row">'
                    + '<div class="col-xs-2 text-right"><strong>IOF Item</strong></div>'
                    + '<div class="col-xs-10">' + getItemName(data.ItemID, data.Description) + '</div>'
                    + '</div>'
                    + '<div class="row">'
                    + '<div class="col-xs-2 text-right"><strong>Store Item</strong></div>'
                    + '<div class="col-xs-10">' + getItemName(data.StoreItemID, data.StoreDescription) + '</div>'
                    + '</div>';

                r.child(html).show();
            },
            "rowCallback": function (row, data, index) {
                var isEven = $(row).hasClass("even");
                var r = this.api().row(row);
                r.child().removeClass("child-even").removeClass("child-odd");
                r.child().addClass(isEven ? "child-even" : "child-odd");
            }
        }
    };

    $.fn.report = function (options) {
        return this.each(function () {
            var $this = $(this);

            var opt = $.extend({}, { "reportType": "item-report" }, options, $this.data());

            var itemSearch = $(".item-search", $this);

            /**
             * Get the currently selected item id.
             * @returns {number} - An item id.
             */
            var selectedItemId = function () {
                var hid = $(".selected-item-id", $this);
                var val = hid.val();
                var result = parseInt(val);
                return result;
            };

            if (itemSearch.length > 0) {
                itemSearch.addClass("loading");
                $(".datatable", itemSearch).DataTable({
                    "autoWidth": false,
                    "stateSave": true,
                    "initComplete": function (settings, json) {
                        itemSearch.removeClass("loading");
                    },
                    "createdRow": function (row, data, dataIndex) {
                        var itemId = parseInt(data[0]);
                        if (itemId === selectedItemId()) {
                            $(row).addClass("selected");
                        }
                    },
                    "language": {
                        "emptyTable": "No items were found for the selected client."
                    }
                });
            }

            if (reports[opt.reportType]) {
                var table = $(".datatable", $this).DataTable($.extend(true, {}, reports[opt.reportType], {
                    "ajax": "ajax.ashx?type=" + opt.reportType,
                    "initComplete": function (settings, json) {
                        $(".loader", $this).hide();
                        $(".table-container", $this).removeClass("loading");
                    }
                }));

                if (opt.reportType === "store-manager-report") {
                    $.fn.dataTable.ext.search.push(function (settings, searchData, index, rowData, counter) {
                        var result = true;

                        if ($(".price-filter", $this).prop("checked")) {
                            if (searchData[3].length === 0)
                                result = false; //there is no store price
                            else {
                                var iofPrice = numeral(searchData[2]).value();
                                var storePrice = numeral(searchData[3]).value();

                                result = iofPrice > storePrice;
                            }
                        }

                        if ($(".purchased-filter", $this).prop("checked")) {
                            result = result && searchData[6] !== "never";
                        }

                        return result;
                    });

                    $this.on("change", ".price-filter, .purchased-filter", function (e) {
                        table.draw();
                    });
                }

                
            }

            $this.data("api", {
                "selectedItemId": selectedItemId
            });
        });
    };
}(jQuery));