var documentUI = {
    departmentID: 0,
    addButtonToAutocomplete: function (elem) {
        $("<button type='button'>&nbsp;</button>").attr("tabIndex", -1)
                    .insertAfter($(elem))
					.button({
					    icons: {
					        primary: "ui-icon-triangle-1-s"
					    },
					    text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
					.click(function () {
					    // close if already visible
					    if ($(elem).autocomplete("widget").is(":visible")) {
					        $(elem).autocomplete("close");
					        return;
					    }

					    // work around a bug (likely same cause as #5265)
					    $(this).blur();

					    // pass empty string as value to search for, displaying all results
					    $(elem).autocomplete("search", "");
					    $(elem).focus();
					});
    },
    createButtonForAutocomplete: function (elem) {
        return $('<button type="button" class="ui-button-call-list">&nbsp;</button>').attr("tabIndex", -1)
					.button({ icons: { primary: 'ui-icon-triangle-1-s' }, text: false })
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
					.click(function () {
					    // close if already visible
					    if ($(elem).autocomplete("widget").is(":visible")) {
					        $(elem).autocomplete("close");
					        return;
					    }
					    // work around a bug (likely same cause as #5265)
					    $(this).blur();
					    // pass empty string as value to search for, displaying all results
					    $(elem).autocomplete("search", "");
					    $(elem).focus();
					});
    },
    formatFullName: function (sName, fName, tName) {
        var fullName = sName || "";

        if (fName) {
            fullName = fullName + " " + fName.substr(0, 1) + ".";
            if (tName)
                fullName = fullName + " " + tName.substr(0, 1) + ".";
        }

        return fullName;
    }
};
/*
$.widget("ui.multicheck", $.ui.autocomplete, {
    options: {
        selectedItems: []
    },

    _renderMenu: function (ul, items) {
        var self = this,
			doc = this.element[0].ownerDocument;

        self.menu.element.unbind('click')
        .click(function (event) {
            if (!$(event.target).closest(".ui-menu-item a").length) {
                return;
            }
            $(event.target).find("input").each(function () {
                var cbx = $(this);
                cbx.attr('checked', !cbx.is(':checked'));
            });
            self.menu.select(event);
        });

        self.menu.options.selected = function (event, ui) {

            var item = ui.item.data("item.autocomplete"),
            previous = self.previous;

            // only trigger when focus was lost (click on menu)
            if (self.element[0] !== doc.activeElement) {
                self.element.focus();
                self.previous = previous;
                // #6109 - IE triggers two focus events and the second
                // is asynchronous, so we need to reset the previous
                // term synchronously and asynchronously :-(
                setTimeout(function () {
                    self.previous = previous;
                    self.selectedItem = item;
                }, 1);
            }

            if (false !== self._trigger("select", event, { item: item })) {
                self.element.val(item.value);
            }
            // reset the term after the select event
            // this allows custom select handling to work properly
            self.term = self.element.val();

            self.selectedItem = item;
            if ($.inArray(item.id, self.options.selectedItems) > -1) {
                self.options.selectedItems = $.grep(self.options.selectedItems, function (val) { return val != item.id; });
            } else {
                self.options.selectedItems.push(item.id);
            }
        };

        $.each(items, function (index, item) {
            self._renderItem(ul, item);
        });
    },
    _renderItem: function (ul, item) {
        var self = this;
        if ($.inArray(parseInt(item.id), self.options.selectedItems) > -1) {
            return $("<li></li>")
			.data("item.autocomplete", item)
			.append($('<a><input type="checkbox" checked="true" />' + item.label + '</a>'))
			.appendTo(ul);
        } else {
            return $("<li></li>")
			.data("item.autocomplete", item)
			.append($('<a><input type="checkbox" />' + item.label + '</a>'))
			.appendTo(ul);
        }
    }
});
*/