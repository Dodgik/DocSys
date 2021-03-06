﻿(function (window, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var asDocuments = function (selector, options) {
        var self = this;

        this.appendTo = "body",
        this.departmentID = 0,
        this.templateID = 0,
        this.jqGridID = '',
        this.lastSelectedRowID = 0,
        this.searchPanel = {},
        this.searchPanelContainer = '',
        this.detailsCells = [],
        this.rowsToColor = [],
        this.rowsToColor2 = [],
        this.defaultStartDate = new Date(new Date().getFullYear() - 1, new Date().getMonth()),
        this.defaultEndDate = new Date(new Date().getFullYear(), new Date().getMonth() + 1);

        if (options) {
            if (options.appendTo)
                this.appendTo = options.appendTo;
            if (options.departmentID)
                this.departmentID = options.departmentID;
            if (options.templateID)
                this.templateID = options.templateID;
        }

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
            this.searchPanel.searchButton = $('<input type="button" value="Пошук" style="margin-left: 10px;">').button().click(function (e) {
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
            this.searchPanel.clearButton = $('<input type="button" value="Очистити" style="margin-left: 25px;">').button().click(function (e) {
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
                    .append('<table id="dg' + selector + '"><tr><td></td></tr></table>'))
                    .append($('<td id="collapsePanel' + selector + '" valign="top" style="overflow-y: scroll; overflow-x: hidden;" class="ui-state-default ui-corner-all"></td>')
                    .append('<div id="loadDetails' + selector + '" class="loading-icon" style="display: none; position: relative; left: 150px; top: 270px; z-index: 1"></div>')
                    .append($("<table></table>").attr('id', "tblOverview" + selector)
                        .append($('<tr><td colspan="2" align="center">ДЕТАЛІ</td></tr>'))
                        .append($('<tr><td style="background-color: #F0F0F0">Номер:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Дата:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Управління:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Зміст:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">На розгляд:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Дата рузультату:</td></tr>').append(createDetailCell($('<td></td>'))))

                        .append($('<tr><td style="background-color: #F0F0F0">Результат:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Дата отримання:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Отримав:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Відмітки:</td></tr>').append(createDetailCell($('<td></td>'))))
                            ))
                    .append($('<td id="collapseHeader' + selector + '" valign="middle" style="width: 16px; cursor:pointer;" class="ui-accordion-header ui-helper-reset ui-state-active ui-corner-top"></td>').append('<span class="ui-icon ui-icon-circle-arrow-w" style="float:left;"></span>'))
                )
            );



            this.jqGridID = 'dg' + selector;

            $('#dg' + selector).jqGrid({
                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getlist&isReception=true&departmentID=' + this.departmentID,
                datatype: 'json',
                mtype: 'POST',
                height: '550',
                colNames: ['№', 'documentID', 'Дата', '№', 'Шифр', "Суб'єкт звернення", 'Назва а.п.', "Об'єкт а.п.",
                    'Зміст', 'Відмітки', 'Контроль', '', '', '', '', '', '', '', '', ''],
                colModel: [
                    { name: 'id', index: 'DocAdminServiceID', width: 50, sortable: true, hidden: true },
                    { name: 'documentID', width: 20, sortable: false, hidden: true },
                    { name: 'creationDate', index: 'CreationDate', width: 100, sortable: true, searchoptions: { dataInit: function (el) { $(el).datepicker({ changeMonth: true, changeYear: true }); } } },
                    { name: 'number', index: 'Number', width: 60, sortable: true },
                    { name: 'documentCode', index: 'DocumentCode', width: 50, sortable: false },
                    { name: 'subjectRequest', index: 'SubjectRequest', width: 200, sortable: false },
                    { name: 'serviceName', index: 'ServiceName', width: 190, sortable: false },
                    { name: 'objectForService', index: 'ObjectForService', width: 190, sortable: false },
                    
                    { name: 'content', sortable: false, hidden: true },
                    { name: 'notes', sortable: false, hidden: true },
                    { name: 'isControlled', sortable: false, hidden: true },
                    { name: 'executiveDepartmentID', sortable: false, hidden: true },
                    { name: 'executiveDepartmentName', sortable: false, hidden: true },
                    { name: 'dateReceivedToDepartment', sortable: false, hidden: true },
                    { name: 'receivedWorkerID', sortable: false, hidden: true },
                    { name: 'dateReturnFromDepartment', sortable: false, hidden: true },
                    { name: 'returnWorkerID', sortable: false, hidden: true },
                    { name: 'serviceResult', sortable: false, hidden: true },
                    { name: 'dateResponseToClient', sortable: false, hidden: true },
                    { name: 'responseClientInfo', sortable: false, hidden: true }
                ],
                rownumbers: true,
                rowNum: 100,
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
                    self.rowsToColor2 = [];
                },
                gridComplete: function () {
                    for (var i = 0; i < self.rowsToColor.length; i++) {
                        //var status = $("#" + self.rowsToColor[i]).find("td").eq(7).html();
                        //if (status == "Complete") {
                        $("#" + self.rowsToColor[i]).addClass('ui-state-highlight-2').removeClass('ui-widget-content');
                        // }
                    }
                    for (var r = 0; r < self.rowsToColor2.length; r++) {
                        $("#" + self.rowsToColor2[r]).addClass('ui-state-highlight-red');
                    }
                },

                loadComplete: function (data) {
                    if (self.lastSelectedRowID)
                        $('#dg' + selector).setSelection(self.lastSelectedRowID, true);
                },
                loadError: function (xhr, status, error) {
                    if (xhr.status == 403)
                        alert('Доступ заборонений');
                },
                onSelectRow: function (rowid, status) {
                    if (rowid) {
                        self.lastSelectedRowID = rowid;
                        var rowObj = $('#dg' + selector).jqGrid('getRowData', rowid);

                        self.detailsCells[0].text(rowObj.number);
                        self.detailsCells[1].text(rowObj.creationDate);
                        self.detailsCells[2].text(rowObj.executiveDepartmentName);
                        self.detailsCells[3].text(rowObj.content);
                        self.detailsCells[4].text(rowObj.dateReceivedToDepartment);
                        self.detailsCells[5].text(rowObj.dateReturnFromDepartment);
                        self.detailsCells[6].text(rowObj.serviceResult);
                        self.detailsCells[7].text(rowObj.dateResponseToClient);
                        self.detailsCells[8].text(rowObj.responseClientInfo);
                        self.detailsCells[9].text(rowObj.notes);
                    }
                },

                ondblClickRow: function (rowid, iRow, iCol, e) {
                    self.showViewForm();
                },

                multiselect: false,
                subGrid: false
            })
            .filterToolbar({ searchOnEnter: true, stringResult: true, defaultSearch: "cn", groupOp: "AND" })
            .bindKeys({
                onEnter: function (rowid) {
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
                    return false;
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Змінити вибраний запис",
                buttonicon: "ui-icon-pencil",
                onClickButton: function () {
                    self.showUpdateForm();
                    return false;
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Переглянути обраний запис",
                buttonicon: "ui-icon-document",
                onClickButton: function () {
                    self.showViewForm();
                    return false;
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Видалити вибраний запис",
                buttonicon: "ui-icon-trash",
                onClickButton: function () {
                    self.showDelForm();
                    return false;
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
                    return false;
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Поновити",
                buttonicon: "ui-icon-refresh",
                onClickButton: function () {
                    $('#dg' + selector)[0].triggerToolbar();
                    return false;
                },
                position: "last"
            })
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' });

            //$('#dg' + selector)[0].toggleToolbar();
            $('#dg' + selector).gridResize({ minWidth: 640, minHeight: 550,
                stop: function (grid, ev, ui) {
                    
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
            var docBlank = new AsDocumentBlank({
                departmentID: self.departmentID,
                templateID: self.templateID,
                success: function (msg) {
                    if (msg)
                        self.lastSelectedRowID = parseFloat(msg);
                    $('#' + self.jqGridID).trigger("reloadGrid");
                }
            });
            docBlank.showInsertForm();
        },
        this.showUpdateForm = function () {
            var rowID = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowID) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowID);

                var docBlank = new AsDocumentBlank({
                    departmentID: self.departmentID,
                    templateID: self.templateID,
                    success: function (msg) {
                        $('#' + self.jqGridID).trigger("reloadGrid");
                    }
                });
                docBlank.showUpdateForm(rowObj.id);
            }
            else
                alert("Будь ласка виберіть запис!");
        },
        this.showViewForm = function () {
            var rowID = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowID) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowID);

                var docBlank = new AsDocumentBlank({
                    departmentID: self.departmentID,
                    templateID: self.templateID,
                    success: function (data) {
                        $('#' + self.jqGridID).trigger("reloadGrid");
                    }
                });
                docBlank.showViewForm(rowObj.id);
            }
            else
                alert("Будь ласка виберіть запис!");
        },
        this.showDelForm = function () {
            var rowID = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowID) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowID);
                var docBlank = new AsDocumentBlank({
                    departmentID: self.departmentID,
                    templateID: self.templateID,
                    success: function (data) {
                        $('#' + self.jqGridID).trigger("reloadGrid");
                    }
                });
                docBlank.showDeleteForm(rowObj.id, function (data) {
                    $('#' + self.jqGridID).trigger("reloadGrid");
                });
            }
            else
                alert("Будь ласка виберіть запис!");
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

    window.AsDocuments = asDocuments;
})(window);