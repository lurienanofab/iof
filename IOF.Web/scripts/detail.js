(function ($) {
    $.fn.detail = function (options) {
        return this.each(function () {
            var $this = $(this);

            var opt = $.extend({}, { "ajaxUrl": "ajax/purchaser.ashx" }, options, $this.data());

            var getRowByIndex = function (index) {
                return $this.find(".po-items > tbody > tr").eq(index);
            };

            var getItem = function (row) {
                return {
                    partNum: row.find(".partnum").data("value"),
                    quantity: parseFloat(row.find(".quantity").data("value")),
                    unitPrice: parseFloat(row.find(".unit-price").data("value"))
                };
            };

            var updateInventory = function () {
                return !$(".update-po-only", $this).prop("checked");
            };

            var recalc = function () {
                var total = 0;
                $(".po-items > tbody > tr", $this).each(function () {
                    var row = $(this);
                    var extPrice = parseFloat(row.find(".ext-price").data("value"));
                    total += extPrice;
                });
                $(".po-items > tfoot > tr > .total-ext-price .value").html(numeral(total).format("$0,0.00"));
            };

            var editIndex = -1;

            var cancelEdit = function () {
                if (editIndex >= 0) {
                    var row = getRowByIndex(editIndex);
                    var item = row.data("item");

                    var field;

                    field = row.find(".partnum");
                    field.find("input[type='text']").val("").hide();
                    field.find(".value").html(item.partNum).show();
                    field.data("value", item.partNum);

                    field = row.find(".quantity");
                    field.find("input[type='text']").val("").hide();
                    field.find(".value").html(item.quantity + " " + field.data("unit")).show();
                    field.data("value", item.quantity);

                    field = row.find(".unit-price");
                    field.find("input[type='text']").val("").hide();
                    field.find(".value").html(numeral(item.unitPrice).format("$0,0.00")).show();
                    field.data("value", item.unitPrice);

                    row.find(".working").hide();
                    row.find(".item-update-cancel").hide();
                    row.find(".item-edit").show();

                    row.data("item", null);

                    editIndex = -1;
                }
            };

            var beginEdit = function (index) {
                cancelEdit();

                var row = getRowByIndex(index);

                var item = getItem(row);

                row.find(".partnum .value").hide();
                row.find(".partnum input[type='text']").val(item.partNum).show();

                row.find(".quantity .value").hide();
                row.find(".quantity input[type='text']").val(item.quantity).show();

                row.find(".unit-price .value").hide();
                row.find(".unit-price input[type='text']").val(item.unitPrice.toFixed(2)).show();

                row.find(".item-edit").hide();
                row.find(".item-update-cancel").show();

                row.data("item", item);

                editIndex = index;
            };

            var update = function (args) {
                var def = $.Deferred();

                var row = getRowByIndex(editIndex);

                var item = row.data("item");

                item.partNum = row.find(".partnum input[type='text']").val();
                item.quantity = row.find(".quantity input[type='text']").val();
                item.unitPrice = row.find(".unit-price input[type='text']").val();

                row.find(".item-edit").hide();
                row.find(".item-update-cancel").hide();
                row.find(".working").show();

                $.ajax({
                    "url": opt.ajaxUrl,
                    "method": "POST",
                    "data": $.extend(item, args, { "updateInventory": updateInventory() })
                }).done(function () {
                    var extPrice = item.quantity * item.unitPrice;
                    row.find(".ext-price").data("value", extPrice);
                    row.find(".ext-price .value").html(numeral(extPrice).format("$0,0.00"));
                    row.data("item", item);
                    cancelEdit();
                    recalc();
                    def.resolve();
                }).fail(def.reject);

                return def.promise();
            };

            $this.on("click", "[data-command='edit']", function (e) {
                e.preventDefault();
                var args = $(this).data();
                beginEdit(args.index);
            }).on("click", "[data-command='cancel']", function (e) {
                e.preventDefault();
                cancelEdit();
            }).on("click", "[data-command='update']", function (e) {
                e.preventDefault();
                var args = $(this).data();
                update(args);
            });
        });
    };
}(jQuery));