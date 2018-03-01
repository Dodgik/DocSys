(function (window, $, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var organizationsUi = function (options) {
        var self = this;

        this.appendTo = "body",
        this.departmentID = 0,
        this.templateID = 0,
        this.jqGridID = '',
        this.jqGrid = null,
        this.pagerID = '',
        this.pager = null,
        this.lastSelectedRowID = 0;

        if (options) {
            if (options.appendTo)
                this.appendTo = options.appendTo;
            if (options.departmentID)
                this.departmentID = options.departmentID;
            if (options.templateID)
                this.templateID = options.templateID;
        }

        function getFilterParams() {
            var postData = self.jqGrid.getGridParam('postData'),
                departmentId = self.orgDepartment.attr('valueid');

            var filters = (postData && postData.filters) ? JSON.parse(postData.filters) : { groupOp: 'AND', rules: [] };

            if (departmentId) {
                filters.rules.push({ 'field': 'DepartmentID', 'op': 'cn', 'data': departmentId });
            }
            if (options.organizationTypeID) {
                filters.rules.push({ "field": "OrganizationTypeID", "op": "cn", "data": options.organizationTypeID });
            }
            
            return filters;
        }
        
        this.init = function () {
            this.jqGridID = (new Date()).getTime();
            this.pagerID = 'pager' + this.jqGridID;
            

            this.searchButton = $('<input type="button" value="Пошук" style="margin-left: 10px;">').button().click(function () {
                self.jqGrid[0].triggerToolbar();
            });
            this.hiddenSearchPanel = $('<div></div>');
            this.filterRow1 = $('<div class="filter-row"></div>').appendTo(this.hiddenSearchPanel);
            this.searchPanel = $('<div class="org-accordion"><h3></h3></div>').append(this.hiddenSearchPanel);
            this.orgDepartment = $('<input name="organization" type="text" valueid="" style="width: 300px;" value="">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=search&dep=' + self.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
            this.filterOptions3 = $('<div class="filter-options">Відомство криейтор організації: </div>').appendTo(this.filterRow1)
                .append(this.orgDepartment)
                .append(documentUI.createButtonForAutocomplete(this.orgDepartment))
                .append($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.orgDepartment.attr('valueid', '');
                            self.orgDepartment.val('');
                        }))
                .append(this.searchButton);

            this.pager = $('<div id="' + this.pagerID + '"></div>').appendTo(this.appendTo);
            this.jqGrid = $('<table id="' + this.jqGridID + '"></table>').appendTo(this.appendTo);

            $(this.appendTo).append($('<table></table>')
                .append($('<tr></tr>').append($('<td colspan=3 valign="top"></td>').append(this.searchPanel)))
                .append($('<tr></tr>').append($('<td valign="top"></td>').append(this.pager).append(this.jqGrid))));

            this.jqGrid.jqGrid({
                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=org&type=page&dep=' + this.departmentID,
                datatype: 'json',
                mtype: 'POST',
                height: '600',
                colNames: ['№', 'Організація', 'Тип'],
                colModel: [
                    { name: 'id', index: 'PostID', width: 50, sortable: false, hidden: false },
                    { name: 'name', index: 'Name', width: 800, sortable: true, hidden: false },
                    { name: 'organizationTypeID', index: 'OrganizationTypeID', width: 180, sortable: false, hidden: true }
                ],
                rownumbers: true,
                rowNum: 700,
                rowList: [200, 300, 500, 700, 1000],
                viewrecords: true,
                pager: this.pagerID,
                scroll: false,
                scrollrows: true,
                sortname: 'name',
                sortorder: "asc",

                //toolbar: [true, "top"],
                toppager: true,

                beforeRequest: function () {
                    var filters = getFilterParams();
                    self.jqGrid.jqGrid('setGridParam', { postData: { filters: JSON.stringify(filters) }, search: true });
                },
                gridComplete: function () {
                },

                loadComplete: function (data) {
                    if (self.lastSelectedRowID)
                        self.jqGrid.setSelection(self.lastSelectedRowID, true);
                },
                loadError: function (xhr, status, error) {
                    if (xhr.status == 403)
                        alert('Доступ заборонений');
                },
                onSelectRow: function (rowid, status) {
                    if (rowid)
                        self.lastSelectedRowID = rowid;
                },

                ondblClickRow: function (rowid, iRow, iCol, e) {
                    self.showUpdateForm();
                },

                multiselect: false
            })
            .filterToolbar({ searchOnEnter: true, stringResult: true, defaultSearch: "cn", groupOp: "AND" })
            .bindKeys({
                onEnter: function (rowid) {
                    self.showUpdateForm();
                },
                scrollingRows: true
            })
            .navGrid(this.pagerID, { add: false, edit: false, view: false, del: false, search: false, refresh: false, cloneToTop: true })
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
                    self.jqGrid[0].toggleToolbar();
                    return false;
                },
                position: "last"
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Поновити",
                buttonicon: "ui-icon-refresh",
                onClickButton: function () {
                    self.jqGrid[0].triggerToolbar();
                    return false;
                },
                position: "last"
            })
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: "",
                title: "Замінити використання",
                buttonicon: "ui-icon-link",
                onClickButton: function () {
                    self.showRepForm();
                    return false;
                },
                position: "last"
            });

            //self.jqGrid[0].toggleToolbar();
            self.jqGrid.gridResize({
                minWidth: 640, minHeight: 550,
                stop: function (grid, ev, ui) {

                }
            });

            $(document).bind('keyup', self.keyCodeParser);
        };


        this.createFieldForm = function (o) {
            var form,
                orgData = { ID: 0, OrganizationTypeID: 2 },
                title = null;

            if (o) {
                if (o.data)
                    orgData = o.data;
                if (o.title)
                    title = o.title;
            }

            var row = function (fieldName, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + fieldName + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            form = $('<div style="display: none;"></div>').appendTo(this.appendTo);
            if (title)
                form.attr('title', title);
            form.append(table);


            var name = $('<input type="text" style="width: 240px;">').val(orgData.Name ? orgData.Name : '');
            table.append(row('Назва:', name));

            var buttonCreate = $('<input type="button" value="Зберегти">').button()
            .click(function () {
                var sendData = {
                    ID: orgData.id || 0,
                    OrganizationTypeID: 2,
                    Name: name.val()
                };

                var valRes = self.validate(sendData);
                if (valRes.isValid) {
                    var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=org&type=' + o.type + '&dep=' + documentUI.departmentID;

                    $.ajax({
                        url: urlRequest,
                        type: "POST",
                        cache: false,
                        data: { 'data': JSON.stringify(sendData) },
                        dataType: "json",
                        success: function () {
                            self.jqGrid.trigger("reloadGrid");
                            form.dialog("close");
                        },
                        error: function (xhr, status, error) {
                            alert(xhr.responseText);
                        }
                    });
                }
                else
                    window.alert(valRes.message);
            });

            var buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                form.dialog("close");
            });

            table.append($('<tr></tr>').append($('<td colspan="2" align="center"></dt>').append(buttonCreate.add(buttonCancel))));


            form = form.dialog({
                autoOpen: true,
                draggable: true,
                modal: true,
                position: ["top"],
                resizable: true,
                width: 540,
                open: function (event, ui) {
                    //if ($.ui.dialog.overlay.instances.length > 0)
                    //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                },
                close: function (event, ui) {
                    $(form).remove();
                }
            });

            return form;
        },

        this.showInsertForm = function () {
            this.createFieldForm({ data: { id: 0, OrganizationTypeID: 2 }, type: 'ins', title: 'Внесення Організації' });
        },
        this.showUpdateForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);

                this.createFieldForm({
                    data: {
                        id: rowObj.id,
                        Name: rowObj.name,
                        OrganizationTypeID: rowObj.organizationTypeID
                    },
                    type: 'upd',
                    title: 'Редагування Організації'
                });
            }
            else
                alert("Будь ласка виберіть запис!");
        },
        this.showDelForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=org&type=del&data=' + rowObj.id + '&dep=' + documentUI.departmentID;

                var deleteDlg = $('<div title="Видалити?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
                    .dialog({
                        autoOpen: true,
                        modal: true,
                        position: ["center"],
                        resizable: false,
                        buttons: {
                            "Видалити": function() {
                                $.ajax({
                                    type: "GET",
                                    cache: false,
                                    url: urlRequest,
                                    dataType: "json",
                                    success: function(result) {
                                        if (result.Success) {
                                            deleteDlg.dialog("close");
                                            self.jqGrid.trigger("reloadGrid");
                                        }
                                        else {
                                            deleteDlg.dialog("close");
                                            alert("Не може бути видалена, так як на даний момент використовуэться.");
                                        }
                                    }
                                });
                            },
                            "Відмінити": function() {
                                $(this).dialog("close");
                            }
                        },
                        open: function(event, ui) {
                            //if ($.ui.dialog.overlay.instances.length > 0)
                            //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                        },
                        close: function(event, ui) {
                            if (deleteDlg)
                                deleteDlg.remove();
                        }
                    });
            } else {
                alert("Будь ласка виберіть запис!");
            }
        },
        this.showRepForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);

                var row = function (fieldName, obj) {
                    return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + fieldName + '</td>')).append($('<td></td>').append(obj));
                };
                var table = $('<table></table>');

                var form = $('<div style="display: none;"></div>').appendTo(this.appendTo);
                form.append(table);

                table.append(row('Замінити організацію:', rowObj.id + '. ' + rowObj.name));

                var name = $('<input type="text" style="width: 400px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&type=search&orgtype=2&dep=' + self.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseFloat(item[0]),
                                        label: item[0] + '. ' + item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('На:', name));

                var buttonCreate = $('<input type="button" value="Замінити">').button()
                .click(function () {
                    var toIdStr = name.attr('valueid'),
                        toId = toIdStr ? parseInt(toIdStr, 10) : 0;
                    if (toId) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=org&type=rep&dep=' + documentUI.departmentID,
                            type: "POST",
                            cache: false,
                            data: { 'data': rowObj.id, 'toId': name.attr('valueid') },
                            dataType: "json",
                            success: function() {
                                self.jqGrid.trigger("reloadGrid");
                                form.dialog("close");
                            },
                            error: function(xhr, status, error) {
                                alert(xhr.responseText);
                            }
                        });
                    } else {
                        alert('Виберіть  організацію  для заміни.');
                    }
                });

                var buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                    form.dialog("close");
                });

                table.append($('<tr></tr>').append($('<td colspan="2" align="center"></dt>').append(buttonCreate.add(buttonCancel))));


                form = form.dialog({
                    autoOpen: true,
                    draggable: true,
                    modal: true,
                    position: ["top"],
                    resizable: true,
                    width: 640,
                    open: function (event, ui) {
                        //if ($.ui.dialog.overlay.instances.length > 0)
                        //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                    },
                    close: function (event, ui) {
                        $(form).remove();
                    }
                });

            } else {
                alert("Будь ласка виберіть запис!");
            }
        },

        this.showSearchPanel = function () {
            self.jqGrid[0].toggleToolbar();
        },

        this.updateGrid = function () {
            self.jqGrid[0].triggerToolbar();
        },

        this.clearFilter = function () {
            self.jqGrid[0].clearToolbar();
        },

        this.validate = function (obj) {
            var res = { isValid: false, message: null };

            if (obj == null)
                res.message = "Об'єкт данних є нульовий!";
            else if (!obj.Name)
                res.message = "Не коректна назва!";
            else
                res = { isValid: true, message: "Об'єкт данних є коректним!" };

            return res;
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
        };

        this.init();
    };

    window.OrganizationsUi = organizationsUi;
})(window, jQuery);
