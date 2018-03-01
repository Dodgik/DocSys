function Organizations(options) {
    this.lastSelectedRowID = 0;
    this.o = {};
    this.wrapper = null;

    $.extend(this.o, options);

    this.init();
}

Organizations.prototype.init = function () {
    var that = this;
    this.jqGridID = (new Date()).getTime();

    this.wrapper = $('<div>');
    $(this.o.element).replaceWith(this.wrapper);
    
    this.buttonPanel = $('<div>').appendTo(this.wrapper);
    this.jqGrid = $('<table id="' + this.jqGridID + '"></table>').appendTo(this.wrapper);

    this.jqGrid.jqGrid({
        url: that.o.serviceUrl,
        postData: { type: 'page' },
        datatype: 'json',
        mtype: 'POST',
        height: '500',
        colNames: ['№', 'Організація', 'Тип'],
        colModel: [
            { name: 'id', index: 'PostID', width: 24, sortable: false, hidden: true },
            { name: 'name', index: 'Name', width: 500, sortable: true, hidden: false },
            { name: 'organizationTypeID', index: 'OrganizationTypeID', width: 180, sortable: false, hidden: true }
        ],
        multiselect: false,
        rownumbers: true,
        rowNum: 50,
        rowList: [50, 100, 200, 500, 1000, 2000],
        viewrecords: true,
        scroll: true,
        scrollrows: true,
        sortname: 'name',
        sortorder: "asc",

        beforeRequest: function () {
            var postData = that.jqGrid.getGridParam('postData');
            var filters = (postData && postData.filters) ? JSON.parse(postData.filters) : { groupOp: "AND", rules: [] };

            if (that.o.organizationTypeID) {
                filters.rules.push({ "field": "OrganizationTypeID", "op": "cn", "data": that.o.organizationTypeID });
            }
            that.jqGrid.jqGrid('setGridParam', { postData: { filters: JSON.stringify(filters) }, search: true });
        },
        loadComplete: function() {
            if (that.lastSelectedRowID) {
                that.jqGrid.setSelection(that.lastSelectedRowID, true);
            }
        },
        loadError: function(xhr) {
            if (xhr.status === "403") {
                window.alert('Доступ заборонений');
            }
        },
        onSelectRow: function(rowid) {
            if (rowid) {
                that.lastSelectedRowID = rowid;
            }
        },
        ondblClickRow: function(rowid) {
            if (that.o.onSelect instanceof Function) {
                var rowObj = that.jqGrid.jqGrid('getRowData', rowid);
                that.o.onSelect(rowObj);
            }
        }
    })
    .filterToolbar({ searchOnEnter: true, stringResult: true, defaultSearch: "cn", groupOp: "AND" });
    
    this.jqGrid.gridResize({ minWidth: 524, minHeight: 500, stop: function () { } });

    this.buttonPanel.append($('<input type="button" value="Застосувати">').button().click(function () {
        if (that.o.onSelect instanceof Function) {
            var rowId = that.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = that.jqGrid.jqGrid('getRowData', rowId);
                that.o.onSelect(rowObj);
            } else {
                window.alert("Будь ласка виберіть запис!");
            }
        }
    }));
    this.buttonPanel.append($('<input type="button" value="Створити">').button().click(function () {
        that.showOrgDialog(function (orgId) {
            that.lastSelectedRowID = parseFloat(orgId);
            that.jqGrid.trigger("reloadGrid");
        });

    }));
    this.buttonPanel.append($('<input type="button" value="Закрити">').button().click(function () {
        if (that.o.onClose instanceof Function) {
            that.o.onClose();
        }
    }));
};

Organizations.prototype.createOrganization = function (orgName, callback) {
    var urlRequest = this.o.serviceUrl + '&type=ins';
    var sendObj = { ID: 0, Name: orgName, OrganizationTypeID: this.o.organizationTypeID };

    $.ajax({
        url: urlRequest,
        type: "POST",
        cache: false,
        data: { 'data': JSON.stringify(sendObj) },
        dataType: "json",
        success: function(msg) {
            if (callback instanceof Function) {
                callback(msg.Message);
            }
        },
        error: function (xhr) {
            window.alert(xhr.responseText);
        }
    });
};
Organizations.prototype.showOrgDialog = function (callback) {
    var that = this,
        inputTitle = "Назва організації:";
    var orgDlg = $('<div title="Добавлення нової організації"></div>').appendTo(this.wrapper)
        .append(inputTitle).append('<br>');
    var orgInput = $('<input>').appendTo(orgDlg)
        .attr({ 'type': 'text' }).css({ 'width': '450px' });
    
    orgDlg.dialog({
        autoOpen: true,
        draggable: true,
        modal: true,
        position: ["center", "center"],
        resizable: true,
        minWidth: 470,
        buttons: [{
            text: "Додати",
            title: "Додати нову організацію",
            click: function () {
                var orgName = orgInput.val();
                if (orgName) {
                    that.createOrganization(orgName, function (orgId) {
                        orgDlg.dialog('close');
                        if (callback instanceof Function) {
                            callback(orgId);
                        }
                    });
                }
                else {
                    window.alert('Введіть назву організації!');
                }
            }
        },
        {
            text: "Закрити",
            title: "Закрити вікно",
            click: function () {
                orgDlg.dialog('close');
            }
        }],
        close: function () {
            orgDlg.dialog("destroy");
            orgDlg.remove();
        }
    });
};

Organizations.prototype.destroy = function () {
    if (this.wrapper !== null) {
        $(this.wrapper).remove();
    }
};