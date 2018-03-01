(function (window, $, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var statements = function (selector, options) {
        var self = this;

        this.appendTo = "body",
        this.departmentID = 0,
        this.templateID = 0,
        this.isReception = false,
        this.jqGridID = '',
        this.lastSelectedRowID = 0,
        this.searchPanel = {},
        this.searchPanelContainer = '',
        this.detailsCells = [],
        this.rowsToColor = [],
        this.defaultStartDate = new Date(new Date().getFullYear() - 2, new Date().getMonth()),
        this.defaultEndDate = new Date(new Date().getFullYear(), new Date().getMonth() + 1);

        if (options) {
            if (options.appendTo)
                this.appendTo = options.appendTo;
            if (options.departmentID)
                this.departmentID = options.departmentID;
            if (options.templateID)
                this.templateID = options.templateID;
            if (options.isReception)
                this.isReception = options.isReception;
        }

        var findInDict = function (opts) {
            var res = {},
                d = appData.dictionaries[opts.dictName] || [];

            for (var r in d) {
                if (d[r].id === parseFloat(opts.id)) {
                    res = d[r];
                    break;
                }
            }

            return res;
        };

        this.init = function () {

            var createDetailCell = function (cell) {
                self.detailsCells.push(cell);
                return cell;
            };

            $("#" + selector).remove();

            this.searchPanel.searchInput = $('<input type="text" style="width: 275px;">').keypress(function (e) {
                if (e.which == 13)
                    $('#dg' + selector)[0].triggerToolbar();
            });
            this.searchPanel.searchButton = $('<input type="button" value="Пошук" style="margin-left: 10px;">').button().click(function () {
                $('#dg' + selector)[0].triggerToolbar();
            });
            this.searchPanel.textFromPeriod = $('<span> За період з:</span>');

            this.searchPanel.dateInputFromPeriod = $('<input type="text" style="width: 80px;">').datepicker({ changeMonth: true, changeYear: true })
                .datepicker("setDate", self.defaultStartDate)
                .keypress(function (e) {
                    if (e.which == 13)
                        $('#dg' + selector)[0].triggerToolbar();
                });
            this.searchPanel.textToPeriod = $('<span> по: </span>');

            this.searchPanel.dateInputToPeriod = $('<input type="text" style="width: 80px;">').datepicker({ changeMonth: true, changeYear: true })
                .datepicker("setDate", self.defaultEndDate)
                .keypress(function (e) {
                    if (e.which == 13)
                        $('#dg' + selector)[0].triggerToolbar();
                });
            this.searchPanel.clearButton = $('<input type="button" value="Очистити" style="margin-left: 25px;">').button().click(function () {
                self.searchPanel.searchInput.val('');
                self.searchPanel.dateInputFromPeriod.datepicker("setDate", self.defaultStartDate);
                self.searchPanel.dateInputToPeriod.datepicker("setDate", self.defaultEndDate);
                $('#dg' + selector)[0].clearToolbar();
            });

            this.searchPanelContainer = this.searchPanel.searchInput
                .add(this.searchPanel.textFromPeriod)
                .add(this.searchPanel.dateInputFromPeriod)
                .add(this.searchPanel.textToPeriod)
                .add(this.searchPanel.dateInputToPeriod)
                .add(this.searchPanel.searchButton)
                .add(this.searchPanel.clearButton);

            $(this.appendTo).append($('<table></table>').attr('id', selector)
                .append($('<tr></tr>').append($('<td colspan=3 valign="top"></td>').append(this.searchPanelContainer)))
                .append($("<tr></tr>").append($('<td valign="top"></td>')
                    .append('<div id="pager' + selector + '"></div>')
                    .append('<table id="dg' + selector + '"><tr><td></td></tr></table>')
            
                    .append($('<input type="button" value="Додати">').button().click(function () {
                        self.showInsertForm();
                    }))
                    .append($('<input type="button" value="Модифікувати">').button().click(function () {
                        self.showUpdateForm();
                    }))
                    .append($('<input type="button" value="Видалити">').button().click(function () {
                        self.showDelForm();
                    }))
                )
                .append($('<td id="collapsePanel' + selector + '" valign="top" style="overflow-y: scroll; overflow-x: hidden;" class="ui-state-default ui-corner-all"></td>')
                .append('<div id="loadDetails' + selector + '" class="loading-icon" style="display: none; position: relative; left: 150px; top: 270px; z-index: 1"></div>')
                .append($("<table></table>").attr('id', "tblOverview" + selector)
                    .append($('<tr><td colspan="2" align="center">ДЕТАЛІ</td></tr>'))
                    .append($('<tr><td style="background-color: #F0F0F0">Номер:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Дата:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Прізвище:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Адреса:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Вид:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Тип:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Надійшов:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Ознака:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Суб\'єкт:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Зміст:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Термін:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Статус:</td></tr>').append(createDetailCell($('<td></td>'))))
                    .append($('<tr><td style="background-color: #F0F0F0">Резолюція:</td></tr>').append(createDetailCell($('<td></td>'))))
                    ))
                .append($('<td id="collapseHeader' + selector + '" valign="middle" style="width: 16px; cursor:pointer;" class="ui-accordion-header ui-helper-reset ui-state-active ui-corner-top"></td>')
                    .append('<span class="ui-icon ui-icon-circle-arrow-w" style="float:left;"></span>'))
                )
            );



            this.jqGridID = 'dg' + selector;

            $('#dg' + selector).jqGrid({
                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getlist&isReception=' + self.isReception + '&departmentID=' + this.departmentID,
                datatype: 'json',
                mtype: 'POST',
                height: '400',
                colNames: ['№', 'documentID', 'Дата', 'Прізвище', 'fullName', 'Адреса', 'Керівник', 'Номер', 'Номер в орг.',
                    'Зміст', 'DeliveryTypeID', 'inputDocTypeID', 'inputMethodID', 'inputSignID', 'inputSubjectTypeID'],
                colModel: [
                    { name: 'id', index: 'DocStatementID', width: 50, sortable: true, hidden: true },
                    { name: 'documentID', width: 20, sortable: false, hidden: true },
                    { name: 'creationDate', index: 'CreationDate', width: 90, sortable: true, searchoptions: { dataInit: function (el) { $(el).datepicker({ changeMonth: true, changeYear: true }); } } },
                    { name: 'lastName', index: 'CitizenLastName', width: 140, sortable: true },
                    { name: 'fullName', width: 40, sortable: false, hidden: true },
                    { name: 'address', index: 'CityObjectName', width: 240, sortable: false },
                    { name: 'head', index: 'HeadLastName', width: 110, sortable: false, hidden: !self.isReception },
                    { name: 'number', index: 'Number', width: 85, sortable: true },
                    { name: 'externalNumber', index: 'ExternalNumber', width: 100, sortable: false, hidden: self.isReception },
                    { name: 'content', width: 80, sortable: false, hidden: true },
                    { name: 'deliveryTypeID', width: 40, sortable: false, hidden: true },
                    { name: 'inputDocTypeID', width: 40, sortable: false, hidden: true },
                    { name: 'inputMethodID', width: 40, sortable: false, hidden: true },
                    { name: 'inputSignID', width: 40, sortable: false, hidden: true },
                    { name: 'inputSubjectTypeID', width: 40, sortable: false, hidden: true }
                ],
                rownumbers: true,
                rowNum: 50,
                rowList: [50, 100, 200, 500, 1000, 2000],
                viewrecords: true,
                pager: '#pager' + selector,
                scroll: false,
                scrollrows: true,
                sortname: 'creationDate',
                sortorder: "desc",

                //toolbar: [true, "top"],
                toppager: true,

                beforeRequest: function () {
                    var sText = self.searchPanel.searchInput.val(),
                    creationDateStart = self.searchPanel.dateInputFromPeriod.val(),
                    creationDateEnd = self.searchPanel.dateInputToPeriod.val(),
                    postData = $('#dg' + selector).getGridParam('postData');

                    var filters = (postData && postData.filters) ? JSON.parse(postData.filters) : { groupOp: "AND", rules: [] };

                    if (sText)
                        filters.rules.push({ "field": "Content", "op": "cn", "data": sText });
                    if (creationDateStart)
                        filters.rules.push({ "field": "CreationDateStart", "op": "cn", "data": creationDateStart });
                    if (creationDateEnd)
                        filters.rules.push({ "field": "CreationDateEnd", "op": "cn", "data": creationDateEnd });

                    $('#dg' + selector).jqGrid('setGridParam', { postData: { filters: JSON.stringify(filters) }, search: true });

                    self.rowsToColor = [];
                },
                gridComplete: function () {
                    for (var i = 0; i < self.rowsToColor.length; i++) {
                        //var status = $("#" + self.rowsToColor[i]).find("td").eq(7).html();
                        //if (status == "Complete") {
                        $("#" + self.rowsToColor[i]).addClass('ui-state-highlight-2').removeClass('ui-widget-content');
                        // }
                    }
                },

                loadComplete: function () {
                    if (self.lastSelectedRowID) {
                        $('#dg' + selector).setSelection(self.lastSelectedRowID, true);
                    }
                },
                loadError: function (xhr) {
                    if (xhr.status == 403)
                        window.alert('Доступ заборонений');
                },
                onSelectRow: function (rowid) {
                    if (rowid) {
                        self.lastSelectedRowID = rowid;
                        var rowObj = $('#dg' + selector).jqGrid('getRowData', rowid);

                        self.detailsCells[0].text(rowObj.number);
                        self.detailsCells[1].text(rowObj.creationDate);
                        self.detailsCells[2].text(rowObj.fullName);
                        self.detailsCells[3].text(rowObj.address);
                        self.detailsCells[4].text(findInDict({ dictName: 'inputDocTypes', id: rowObj.inputDocTypeID }).name || '');
                        self.detailsCells[5].text(findInDict({ dictName: 'inputMethods', id: rowObj.inputMethodID }).name || '');
                        self.detailsCells[6].text(findInDict({ dictName: 'deliveryTypes', id: rowObj.deliveryTypeID }).name || '');
                        self.detailsCells[7].text(findInDict({ dictName: 'inputSigns', id: rowObj.inputSignID }).name || '');
                        self.detailsCells[8].text(findInDict({ dictName: 'inputSubjectTypes', id: rowObj.inputSubjectTypeID }).name || '');
                        self.detailsCells[9].text(rowObj.content);

                        $('#loadDetails' + selector).show();
                        var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=getlast&jdata=' + rowObj.documentID + '&dep=' + userdata.departmentId;
                        $.ajax({
                            type: "GET",
                            cache: false,
                            url: urlRequest,
                            dataType: "json",
                            success: function (data) {
                                $('#loadDetails' + selector).hide();
                                if (data) {
                                    var endDate = new Date(+data.EndDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                                    self.detailsCells[10].text($.datepicker.formatDate('yy-mm-dd', endDate));

                                    self.detailsCells[11].text(findInDict({ dictName: 'cardStatuses', id: data.CardStatusID }).name || '');
                                    self.detailsCells[12].text(data.Resolution);
                                }
                                else {
                                    self.detailsCells[10].text('');
                                    self.detailsCells[11].text('');
                                    self.detailsCells[12].text('');
                                }
                            }
                        });
                    }
                },

                ondblClickRow: function () {
                    self.showViewForm();
                },

                multiselect: false,
                subGrid: true,
                subGridOptions: {
                    "plusicon": "ui-icon-triangle-1-e",
                    "minusicon": "ui-icon-triangle-1-s",
                    "openicon": "ui-icon-arrowreturn-1-e",
                    "reloadOnExpand": false,
                    "selectOnExpand": true
                },

                subGridRowExpanded: function (subGridId, rowId) {
                    var subgridTableId = subGridId + "_t",
                        pagerId = "p_" + subgridTableId;

                    $("#" + subGridId).html("<table id='" + subgridTableId + "' class='scroll'></table><div id='" + pagerId + "' class='scroll'></div>");

                    var rowObj = $('#dg' + selector).jqGrid('getRowData', rowId);

                    $("#" + subgridTableId).jqGrid({
                        url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=getlist&id=' + rowObj.documentID + '&dep=' + userdata.departmentId,
                        datatype: "json",
                        mtype: 'POST',
                        height: '100%',
                        colNames: ['id', 'documentID', '№', 'Початок', 'Виконати до', 'Стан картки', 'Виконавець', 'Контрольна відповідь', 'Резолюція', 'Бланк'],
                        colModel: [
                            { name: "id", index: "id", width: 80, sortable: false, hidden: true },
                            { name: "documentID", width: 130, sortable: false, hidden: true },
                            { name: "cardNumber", width: 20, sortable: false },
                            { name: "startDate", width: 90, sortable: false },
                            { name: "endDate", width: 95, sortable: false },
                            { name: "cardStatusName", width: 200, sortable: false },
                            { name: "workerID", width: 130, sortable: false },
                            { name: "ControlResponse", width: 130, sortable: false, hidden: true },
                            { name: "Resolution", width: 130, sortable: false, hidden: true },
                            { name: 'get', width: 85, formatter: 'showlink', formatoptions: { baseLinkUrl: appSettings.rootUrl + 'Handlers/DataPoint.ashx', addParam: '&obj=card&type=rep&docStatementID=' + rowObj.id + '&dep=' + userdata.departmentId, idName: 'jdata' } }
                        ],
                        rowNum: 1000,
                        rowList: [],
                        pgbuttons: false,
                        viewrecords: true,
                        pgtext: null,
                        pager: "#" + pagerId,

                        loadError: function (xhr) {
                            if (xhr.status == 403) {
                                window.alert('Доступ заборонений');
                            }
                        },

                        ondblClickRow: function (rowid, iRow, iCol, e) {
                            e.stopPropagation();
                            var subRowObj = $("#" + subgridTableId).jqGrid('getRowData', rowid);

                            var ccBlank = new ControlCardBlankA({ departmentID: self.departmentID });
                            ccBlank.showViewForm({ controlCardID: subRowObj.id }, function () {
                                $("#" + subgridTableId).trigger("reloadGrid");
                            });
                        }
                    })
                    .navGrid("#" + pagerId, { add: false, edit: false, view: false, del: false, search: false, refresh: false })
                    .navButtonAdd("#" + pagerId, {
                        caption: "",
                        title: "Додати новий запис",
                        buttonicon: "ui-icon-plus",
                        onClickButton: function () {
                            var ccBlank = new ControlCardBlankA({ departmentID: self.departmentID, documentID: rowObj.documentID });

                            var cardNumber = 1;
                            var cardNumbers = $("#" + subgridTableId).jqGrid('getCol', 'cardNumber', false);
                            if (cardNumbers && cardNumbers.length > 0)
                                cardNumber = cardNumber + Math.max.apply(Math, cardNumbers);

                            ccBlank.showInsertForm({ cardNumber: cardNumber, documentID: rowObj.documentID }, function () {
                                $("#" + subgridTableId).trigger("reloadGrid");
                            });
                        },
                        position: "last"
                    })
                    .navButtonAdd("#" + pagerId, {
                        caption: "",
                        title: "Редагувати вибраний запис",
                        buttonicon: "ui-icon-pencil",
                        onClickButton: function () {
                            var subRowId = $("#" + subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $("#" + subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankA({ departmentID: self.departmentID });
                                ccBlank.showUpdateForm({ controlCardID: subRowObj.id }, function () {
                                    $("#" + subgridTableId).trigger("reloadGrid");
                                });
                            }
                            else
                                window.alert("Будь ласка виберіть картку!");
                        },
                        position: "last"
                    })
                    .navButtonAdd("#" + pagerId, {
                        caption: "",
                        title: "Переглянути обраний запис",
                        buttonicon: "ui-icon-document",
                        onClickButton: function () {
                            var subRowId = $("#" + subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $("#" + subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankA({ departmentID: self.departmentID });
                                ccBlank.showViewForm({ controlCardID: subRowObj.id }, function () {
                                    $("#" + subgridTableId).trigger("reloadGrid");
                                });
                            }
                            else
                                window.alert("Будь ласка виберіть картку!");
                        },
                        position: "last"
                    })
                    .navButtonAdd("#" + pagerId, {
                        caption: "",
                        title: "Продовжити обраний запис",
                        buttonicon: "ui-icon-link",
                        onClickButton: function () {
                            var subRowId = $("#" + subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $("#" + subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankA({ departmentID: self.departmentID });
                                ccBlank.showContinueForm({ controlCardID: subRowObj.id }, function () {
                                    $("#" + subgridTableId).trigger("reloadGrid");
                                });
                            }
                            else
                                window.alert("Будь ласка виберіть картку!");
                        },
                        position: "last"
                    })
                    .navButtonAdd("#" + pagerId, {
                        caption: "",
                        title: "Видалити вибраний запис",
                        buttonicon: "ui-icon-trash",
                        onClickButton: function () {
                            var subRowId = $("#" + subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $("#" + subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankA({ departmentID: self.departmentID });
                                ccBlank.showDeleteForm({ controlCardID: subRowObj.id }, function () {
                                    $("#" + subgridTableId).trigger("reloadGrid");
                                });
                            }
                            else
                                window.alert("Будь ласка виберіть картку!");
                        },
                        position: "last"
                    })
                    /*.navButtonAdd("#" + pagerID, {
                    caption: "",
                    title: "Зняти",
                    buttonicon: "ui-icon-cancel",
                    onClickButton: function () {

                    return false;
                    },
                    position: "last"
                    })*/;
                }
            })
            .filterToolbar({ searchOnEnter: true, stringResult: true, defaultSearch: "cn", groupOp: "AND" })
            .bindKeys({
                onEnter: function () {
                    self.showViewForm();
                },
                scrollingRows: true
            })
            .navGrid('#pager' + selector, { add: false, edit: false, view: false, del: false, search: false, refresh: false, cloneToTop: true })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Додати новий запис",
                buttonicon: "ui-icon-plus",
                onClickButton: function () {
                    self.showInsertForm();
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Змінити вибраний запис",
                buttonicon: "ui-icon-pencil",
                onClickButton: function () {
                    self.showUpdateForm();
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Переглянути обраний запис",
                buttonicon: "ui-icon-document",
                onClickButton: function () {
                    self.showViewForm();
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Видалити вибраний запис",
                buttonicon: "ui-icon-trash",
                onClickButton: function () {
                    self.showDelForm();
                },
                position: "last"
            })
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Показати панель пошуку",
                buttonicon: "ui-icon-search",
                onClickButton: function () {
                    $('#dg' + selector)[0].toggleToolbar();
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Поновити",
                buttonicon: "ui-icon-refresh",
                onClickButton: function () {
                    $('#dg' + selector)[0].triggerToolbar();
                },
                position: "last"
            })
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' });

            //$('#dg' + selector)[0].toggleToolbar();
            $('#dg' + selector).gridResize({
                minWidth: 640, minHeight: 550,
                stop: function () {

                }
            });

            $('#collapseHeader' + selector).click(function (event) {
                event.preventDefault();
                if ($('#collapsePanel' + selector).is(':visible')) {
                    $('#collapseHeader' + selector).removeClass("ui-state-active ui-corner-top")
                        .addClass("ui-state-default ui-corner-all")
                        .children(".ui-icon").removeClass("ui-icon-circle-arrow-w").addClass("ui-icon-circle-arrow-e");
                    $('#collapsePanel' + selector).hide();
                } else {
                    $('#collapseHeader' + selector).removeClass("ui-state-default ui-corner-all")
                        .addClass("ui-state-active ui-corner-top")
                        .children(".ui-icon").removeClass("ui-icon-circle-arrow-e").addClass("ui-icon-circle-arrow-w");
                    $('#collapsePanel' + selector).show();
                }
                return false;
            });

            $(document).bind('keyup', self.keyCodeParser);
        };


        this.showInsertForm = function () {
            var stbBlock = $('<div title="Створення документу" style="display:none;"></div>');

            new StatementBlankB({
                departmentID: self.departmentID,
                dictionaries: appData ? (appData.dictionaries || {}) : {},
                docData: { IsReception: self.isReception },
                templateID: self.templateID,
                appendTo: stbBlock,
                fields: { Head: { hidden: !self.isReception }, ExternalSource: { hidden: self.isReception } },
                onSave: function (data) {
                    //var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=1&type=' + type;
                    var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=docstatement&type=ins';

                    $.ajax({
                        url: urlRequest,
                        type: "POST",
                        cache: false,
                        data: { 'jdata': JSON.stringify(data) },
                        dataType: "json",
                        success: function (msg) {
                            if (msg) {
                                self.lastSelectedRowID = parseFloat(msg.Data);
                            }
                            $('#' + self.jqGridID).trigger("reloadGrid");
                            stBlankBDlg.dialog("close");
                        },
                        error: function (xhr) {
                            window.alert(xhr.responseText);
                        }
                    });

                },
                onCancel: function () {
                    stBlankBDlg.dialog("close");
                }
            });

            var stBlankBDlg = stbBlock.dialog({
                autoOpen: true,
                draggable: true,
                modal: true,
                position: ["top"],
                resizable: true,
                width: 740,
                close: function () {
                    $(this).dialog("destroy");
                    $(this).remove();
                },
                open: function () {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },
        this.showUpdateForm = function () {
            var rowId = $('#' + self.jqGridID).jqGrid('getGridParam', 'selrow');
            if (rowId) {
                var rowObj = $('#' + self.jqGridID).jqGrid('getRowData', rowId);

                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=docstatement&type=get&jdata=' + rowObj.id;

                $.ajax({
                    type: "GET",
                    cache: false,
                    url: urlRequest,
                    dataType: "json",
                    success: function (data) {
                        var stbBlock = $('<div title="Редагування документу" style="display:none;"></div>');

                        new StatementBlankB({
                            departmentID: self.departmentID,
                            dictionaries: appData ? (appData.dictionaries || {}) : {},
                            docData: data,
                            templateID: self.templateID,
                            appendTo: stbBlock,
                            fields: { Head: { hidden: !self.isReception }, ExternalSource: { hidden: self.isReception } },

                            onSave: function (doc) {
                                //var urlRequest2 = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=1&type=' + type;
                                var urlRequest2 = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=docstatement&type=upd';

                                $.ajax({
                                    url: urlRequest2,
                                    type: "POST",
                                    cache: false,
                                    data: { 'jdata': JSON.stringify(doc) },
                                    dataType: "json",
                                    success: function (msg) {
                                        if (msg) {
                                            self.lastSelectedRowID = parseFloat(msg.Data);
                                        }
                                        $('#' + self.jqGridID).trigger("reloadGrid");
                                        stBlankBDlg.dialog("close");
                                    },
                                    error: function (xhr) {
                                        window.alert(xhr.responseText);
                                    }
                                });

                            },
                            onCancel: function () {
                                stBlankBDlg.dialog("close");
                            }
                        });

                        var stBlankBDlg = stbBlock.dialog({
                            autoOpen: true,
                            draggable: true,
                            modal: true,
                            position: ["top"],
                            resizable: true,
                            width: 740,
                            close: function () {
                                $(this).dialog("destroy");
                                $(this).remove();
                            },
                            open: function () {
                                $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                            }
                        });

                    }
                });

            }
            else {
                window.alert("Будь ласка виберіть запис!");
            }
        },
        this.showViewForm = function () {
            this.showUpdateForm();
        },
        this.showDelForm = function () {
            var rowId = $('#' + self.jqGridID).jqGrid('getGridParam', 'selrow');
            if (rowId) {
                var rowObj = $('#' + self.jqGridID).jqGrid('getRowData', rowId);
                //var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=del&jdata=' + documentID;
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=docstatement&type=del&jdata=' + rowObj.id;

                var deleteDlg = $('<div title="Видалити документ?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде дійсно видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
                        .dialog({
                            autoOpen: true,
                            modal: true,
                            position: ["center"],
                            resizable: false,
                            buttons: {
                                "Видалити": function () {
                                    $.ajax({
                                        type: "GET",
                                        cache: false,
                                        url: urlRequest,
                                        dataType: "json",
                                        success: function () {
                                            $('#' + self.jqGridID).trigger("reloadGrid");
                                            deleteDlg.dialog("close");
                                        }
                                    });
                                },
                                "Відмінити": function () {
                                    $(this).dialog("close");
                                }
                            },
                            close: function () {
                                deleteDlg.remove();
                            }
                        });
            }
            else {
                window.alert("Будь ласка виберіть запис!");
            }
        },

        this.showSearchPanel = function () {
            $('#dg' + selector)[0].toggleToolbar();
        },

        this.updateGrid = function () {
            $('#dg' + selector)[0].triggerToolbar();
        },

        this.clearFilter = function () {
            self.searchPanel.searchInput.val('');
            self.searchPanel.dateInputFromPeriod.datepicker("setDate", self.defaultStartDate);
            self.searchPanel.dateInputToPeriod.datepicker("setDate", self.defaultEndDate);
            $('#dg' + selector)[0].clearToolbar();
        },

        this.keyCodeParser = function (e) {
            var keys = {
                backspace: 8,
                tab: 9,
                enter: 13,
                shift: 16,
                ctrl: 17,
                alt: 18,
                pause: 19,
                capslock: 20,
                escape: 27,
                pageup: 33,
                pagedown: 34,
                end: 35,
                home: 36,
                leftarrow: 37,
                uparrow: 38,
                rightarrow: 39,
                downarrow: 40,
                insert: 45,
                del: 46,
                0: 48,
                1: 49,
                2: 50,
                3: 51,
                4: 52,
                5: 53,
                6: 54,
                7: 55,
                8: 56,
                9: 57,
                a: 65,
                b: 66,
                c: 67,
                d: 68,
                e: 69,
                f: 70,
                g: 71,
                h: 72,
                i: 73,
                j: 74,
                k: 75,
                l: 76,
                m: 77,
                n: 78,
                o: 79,
                p: 80,
                q: 81,
                r: 82,
                s: 83,
                t: 84,
                u: 85,
                v: 86,
                w: 87,
                x: 88,
                y: 89,
                z: 90
            };

            if (e.keyCode == keys.insert && (e.altKey)) {
                self.showInsertForm();
            }
            else if (e.keyCode == keys.del && (e.altKey)) {
                self.showDelForm();
            }
            else if (e.keyCode == keys.a && (e.altKey)) {
                self.updateGrid();
            }
            else if (e.keyCode == keys.s && (e.altKey)) {
                self.showSearchPanel();
            }
            else if (e.keyCode == keys.z && (e.altKey)) {
                self.clearFilter();
            }
        },

        this.dispose = function () {
            $(document).unbind('keyup', self.keyCodeParser);
            //this.form.remove();
        };

        this.init();
    };

    window.Statements = statements;
})(window, jQuery);