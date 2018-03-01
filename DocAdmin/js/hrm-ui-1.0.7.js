(function (window, $, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var postUi = function (options) {
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

        this.init = function () {
            this.jqGridID = (new Date()).getTime();
            this.pagerID = 'pager' + this.jqGridID;
            
            this.pager = $('<div id="' + this.pagerID + '"></div>').appendTo(this.appendTo);
            this.jqGrid = $('<table id="' + this.jqGridID + '"></table>').appendTo(this.appendTo).jqGrid({
                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=post&type=get&dep=' + this.departmentID,
                datatype: 'json',
                mtype: 'POST',
                height: '450',
                colNames: ['№', 'Посада', 'Підрозділ', 'isVacant', 'postTypeID', 'Вакантна', 'Тип'],
                colModel: [
                    { name: 'id', index: 'PostID', width: 20, sortable: false, hidden: true },
                    { name: 'name', index: 'Name', width: 420, sortable: false, hidden: false },
                    { name: 'departmentID', index: 'DepartmentID', width: 90, sortable: false, hidden: true },
                    { name: 'isVacant', index: 'IsVacant', width: 80, sortable: false, hidden: true },
                    { name: 'postTypeID', index: 'PostTypeID', width: 180, sortable: false, hidden: true },
                    { name: 'isVacantName', index: 'IsVacant', width: 80, sortable: false, hidden: false, formatter: function (cellValue, cOptions, rowObject) {
                            return (rowObject[3].toLowerCase() == "true" ? 'Так' : 'Ні');
                        }
                    },
                    { name: 'postTypeName', index: 'PostTypeID', width: 180, sortable: false, hidden: false, formatter: function (cellValue, cOptions, rowObject) {
                            if (rowObject[4] == '0')
                                return 'Не визначено';
                            else if (rowObject[4] == '1')
                                return 'Керівник';
                            else if (rowObject[4] == '2')
                                return 'Замісник керівника';
                            return rowObject[4];
                        }
                    }
                ],
                rownumbers: true,
                rowNum: 200,
                rowList: [50, 100, 200, 500, 1000, 2000],
                viewrecords: true,
                pager: this.pagerID,
                scroll: false,
                scrollrows: true,
                sortname: 'name',
                sortorder: "desc",

                //toolbar: [true, "top"],
                toppager: true,

                beforeRequest: function () {
                    var postData = $('#' + self.jqGridID).getGridParam('postData');

                    var filters = (postData && postData.filters) ? JSON.parse(postData.filters) : { groupOp: "AND", rules: [] };

                    for (var r in filters.rules) {
                        if (filters.rules[r].field === 'IsVacant') {
                            if (filters.rules[r].data.toLowerCase() === 'так')
                                filters.rules[r].data = 'true';
                            else if (filters.rules[r].data.toLowerCase() === 'ні')
                                filters.rules[r].data = 'false';
                        }
                        else if (filters.rules[r].field === 'PostTypeID') {
                            if (filters.rules[r].data.toLowerCase() === 'не визначено')
                                filters.rules[r].data = '0';
                            else if (filters.rules[r].data.toLowerCase() === 'керівник')
                                filters.rules[r].data = '1';
                            else if (filters.rules[r].data.toLowerCase() === 'замісник керівника')
                                filters.rules[r].data = '2';
                        }
                    }

                    $(self.jqGridID).jqGrid('setGridParam', { postData: { filters: JSON.stringify(filters) }, search: true });
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
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' });

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
                postData = {},
                title = null;
            
            if(o) {
                if(o.data)
                    postData = o.data;
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
            

            var name = $('<input type="text" style="width: 400px;">').val(postData.name ? postData.name : '');
            table.append(row('Посада:', name));
            
            var department = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(postData.departmentName ? postData.departmentName : '')
                .attr('valueid', postData.departmentID);
            var departmentBtn = '';
            var deps = window.navigationSettings.menuList;
            if (deps.length > 1) {
                department.autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function(request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=search&dep=' + self.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function(data) {
                                response($.map(data, function(item) {
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
                    select: function(event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                departmentBtn = documentUI.createButtonForAutocomplete(department);
                
                if (!postData.departmentName) {
                    for (var d in deps)
                        if (deps[d].id === parseFloat(postData.departmentID))
                            department.attr('valueid', deps[d].id).val(deps[d].name);
                }
            }
            else {
                department.attr('disabled', 'disabled');
                if (!postData.departmentName) {
                    department.attr('valueid', deps[0].id).val(deps[0].name);
                }
            }
            table.append(row('Підрозділ:', department.add(departmentBtn)));
            
            var isVacant = $('<input type="checkbox">').attr('checked', postData.isVacant ? postData.isVacant : false);
            table.append(row('Вакантна:', isVacant));
            
            var postType = $('<select></select>')
                .append('<option value="0">Не визначено</option>')
                .append('<option value="1">Керівник</option>')
                .append('<option value="2">Замісник керівника</option>')
                .val(postData.postTypeID ? postData.postTypeID : '');
            table.append(row('Тип:', postType));

            var buttonCreate = $('<input type="button" value="Зберегти">').button()
            .click(function () {
                var sendData = {
                    id: o.data.id,
                    name: name.val(),
                    departmentID: department.attr('valueid'),
                    isVacant: isVacant.is(':checked'),
                    postTypeID: postType.val()
                };
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx' + "?obj=" + 'post' +
                                "&type=" + o.type + '&dep=' + documentUI.departmentID;

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
            this.createFieldForm({ data: { id: 0, isVacant: true }, type: 'ins', title: 'Створення посади' });
        },
        this.showUpdateForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);

                this.createFieldForm({
                    data: {
                        id: rowObj.id,
                        name: rowObj.name,
                        departmentID: rowObj.departmentID,
                        departmentName: rowObj.departmentName,
                        isVacant: (rowObj.isVacant.toLowerCase() == "true"),
                        postTypeID: rowObj.postTypeID
                    },
                    type: 'upd',
                    title: 'Редагування посади'
                });
            }
            else
                alert("Будь ласка виберіть запис!");
        },
        this.showDelForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=post&type=del&pid=' + rowObj.id + '&dep=' + documentUI.departmentID;

                var deleteDlg = $('<div title="Видалити?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
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
                                            deleteDlg.dialog("close");
                                            self.jqGrid.trigger("reloadGrid");
                                        }
                                    });
                                },
                                "Відмінити": function () {
                                    $(this).dialog("close");
                                }
                            },
                            open: function (event, ui) {
                                //if ($.ui.dialog.overlay.instances.length > 0)
                                //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                            },
                            close: function (event, ui) {
                                if (deleteDlg)
                                    deleteDlg.remove();
                            }
                        });
            }
            else
                alert("Будь ласка виберіть запис!");
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

    window.PostUi = postUi;
})(window, jQuery);


(function (window, $, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var workerUi = function (options) {
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

        this.init = function () {
            this.jqGridID = (new Date()).getTime();
            this.pagerID = 'pager' + this.jqGridID;

            this.pager = $('<div id="' + this.pagerID + '"></div>').appendTo(this.appendTo);
            this.jqGrid = $('<table id="' + this.jqGridID + '"></table>').appendTo(this.appendTo).jqGrid({
                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=getlist&dep=' + this.departmentID,
                datatype: 'json',
                mtype: 'POST',
                height: '520',
                colNames: ['ID', 'Прізвище', "Ім'я", 'По батькові', 'Номер посади', 'Посада', 'Номер підрозділу', 'Підрозділ', 'Статус'],
                colModel: [
                    { name: 'id', index: 'id', width: 20, sortable: false, hidden: true },
                    { name: 'lastName', width: 160, sortable: false, hidden: false },
	                { name: 'firstName', width: 90, sortable: false, hidden: false },
	                { name: 'middleName', width: 130, sortable: false, hidden: false },
	                { name: 'postID', width: 50, sortable: false, hidden: true },
	                { name: 'postName', width: 300, sortable: false, hidden: false },
	                { name: 'departmentID', width: 50, sortable: false, hidden: true },
	                { name: 'departmentName', width: 270, sortable: false, hidden: true },
	                {
	                    name: 'isDismissed', width: 90, sortable: false, hidden: false,
	                    formatter: function (cellvalue, cOptions, rowObject) { return cellvalue.toLowerCase() == 'true' ? 'Звільнений' : ''; },
	                    unformat: function (cellvalue, cOptions, rowObject) { return cellvalue.toLowerCase() == 'звільнений'; }
	                }
                ],
                rownumbers: true,
                rowNum: 700,
                rowList: [200, 300, 500, 700, 1000],
                viewrecords: true,
                pager: this.pagerID,
                scroll: false,
                scrollrows: true,
                sortname: 'id',
                sortorder: "desc",

                //toolbar: [true, "top"],
                toppager: true,

                beforeRequest: function () {

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
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' });

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
                workerData = {},
                title = null;

            if (o) {
                if (o.data)
                    workerData = o.data;
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


            var lastName = $('<input type="text" style="width: 240px;">').val(workerData.lastName ? workerData.lastName : '');
            table.append(row('Прізвище:', lastName));
            var firstName = $('<input type="text" style="width: 240px;">').val(workerData.firstName ? workerData.firstName : '');
            table.append(row("Ім'я:", firstName));
            var middleName = $('<input type="text" style="width: 240px;">').val(workerData.middleName ? workerData.middleName : '');
            table.append(row('По батькові:', middleName));

            var department = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(workerData.departmentName ? workerData.departmentName : '')
                .attr('valueid', workerData.departmentID);
            var departmentBtn = '';
            var deps = window.navigationSettings.menuList;
            if (deps.length > 1) {
                department.autocomplete({
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
                departmentBtn = documentUI.createButtonForAutocomplete(department);

                if (!workerData.departmentName) {
                    for (var d in deps)
                        if (deps[d].id === parseFloat(workerData.departmentID))
                            department.attr('valueid', deps[d].id).val(deps[d].name);
                }
            }
            else {
                department.attr('disabled', 'disabled');
                if (!workerData.departmentName) {
                    department.attr('valueid', deps[0].id).val(deps[0].name);
                }
            }
            table.append(row('Підрозділ:', department.add(departmentBtn)));
            

            var post = $('<input type="text" style="width: 240px;">').val(workerData ? workerData.postName : '').attr('valueid', workerData ? workerData.postID : 0)
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function (request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=post&type=get&dep=' + department.attr('valueid') + '&term=' + request.term,
                                type: "GET",
                                dataType: "json",
                                success: function (data) {
                                    if (data)
                                        response($.map(data.rows, function(item) {
                                            return {
                                                id: parseInt(item.cell[0]),
                                                label: item.cell[1],
                                                value: item.cell[1],
                                                option: this
                                            };
                                        }));
                                    else
                                        response();
                                }
                            });
                        },
                        select: function (event, ui) {
                            $(this).attr('valueid', ui.item.id);
                        }
                    }).addClass("ui-widget ui-widget-content ui-corner-left");
            table.append(row('Посада:', post.add(documentUI.createButtonForAutocomplete(post))));


            var isDismissed = $('<input type="checkbox">').attr('checked', workerData.isDismissed ? workerData.isDismissed : false);
            table.append(row('Звільнений:', isDismissed));

            var buttonCreate = $('<input type="button" value="Зберегти">').button()
            .click(function () {
                var sendData = {
                    id: workerData.id || 0,
                    firstName: firstName.val(),
                    middleName: middleName.val(),
                    lastName: lastName.val(),
                    postID: parseFloat(post.attr('valueid')) || 0,
                    isDismissed: isDismissed.is(':checked')
                };
                
                var valRes = self.validate(sendData);
                if (valRes.isValid) {
                    var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx' + "?obj=" + 'worker' +
                        "&type=" + o.type + '&dep=' + documentUI.departmentID;

                    $.ajax({
                        url: urlRequest,
                        type: "POST",
                        cache: false,
                        data: { 'data': JSON.stringify(sendData) },
                        dataType: "json",
                        success: function() {
                            self.jqGrid.trigger("reloadGrid");
                            form.dialog("close");
                        },
                        error: function(xhr, status, error) {
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
            this.createFieldForm({ data: { id: 0 }, type: 'ins', title: 'Внесення данних працівника' });
        },
        this.showUpdateForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);

                this.createFieldForm({
                    data: {
                        id: rowObj.id,
                        firstName: rowObj.firstName,
                        middleName: rowObj.middleName,
                        lastName: rowObj.lastName,
                        departmentID: rowObj.departmentID,
                        departmentName: rowObj.departmentName,
                        postID: rowObj.postID,
                        postName: rowObj.postName,
                        isDismissed: rowObj.isDismissed
                    },
                    type: 'upd',
                    title: 'Редагування данних працівника'
                });
            }
            else
                alert("Будь ласка виберіть запис!");
        },
        this.showDelForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=del&wid=' + rowObj.id + '&dep=' + documentUI.departmentID;

                var deleteDlg = $('<div title="Видалити?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
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
                                        deleteDlg.dialog("close");
                                        self.jqGrid.trigger("reloadGrid");
                                    }
                                });
                            },
                            "Відмінити": function () {
                                $(this).dialog("close");
                            }
                        },
                        open: function (event, ui) {
                            //if ($.ui.dialog.overlay.instances.length > 0)
                            //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                        },
                        close: function (event, ui) {
                            if (deleteDlg)
                                deleteDlg.remove();
                        }
                    });
            }
            else
                alert("Будь ласка виберіть запис!");
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

        this.validate = function(worker) {
            var res = { isValid: false, message: null };
            
            if (worker == null)
                res.message = "Об'єкт данних є нульовий!";
            else if (worker == undefined)
                res.message = "Об'єкт данних є невизначеним!";
            else if (!worker.lastName)
                res.message = "Не коректне прізвище!";
            else if (!worker.firstName)
                res.message = "Не коректне ім'я!";
            else if (!worker.middleName)
                res.message = "Не коректне батькове ім'я!";
            else if (!$.isNumeric(worker.postID) || worker.postID <= 0)
                res.message = "Посада є не доступна!";
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

    window.WorkerUi = workerUi;
})(window, jQuery);

(function (window, $, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var departmentUi = function (options) {
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

        this.init = function () {
            this.jqGridID = (new Date()).getTime();
            this.pagerID = 'pager' + this.jqGridID;

            this.pager = $('<div id="' + this.pagerID + '"></div>').appendTo(this.appendTo);
            this.jqGrid = $('<table id="' + this.jqGridID + '"></table>').appendTo(this.appendTo).jqGrid({
                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=page&dep=' + this.departmentID,
                datatype: 'json',
                mtype: 'POST',
                height: '450',
                colNames: ['№', 'Відомство', 'ParrentDepartmentID'],
                colModel: [
                    { name: 'id', index: 'DepartmentID', width: 20, sortable: false, hidden: true },
                    { name: 'name', index: 'Name', width: 640, sortable: true, hidden: false },
                    { name: 'ParrentDepartmentID', index: 'ParrentDepartmentID', width: 90, sortable: false, hidden: true }
                ],
                rownumbers: true,
                rowNum: 200,
                rowList: [50, 100, 200, 500],
                viewrecords: true,
                pager: this.pagerID,
                scroll: false,
                scrollrows: true,
                sortname: 'name',
                sortorder: "asc",

                //toolbar: [true, "top"],
                toppager: true,

                beforeRequest: function () {
                },
                gridComplete: function () {
                },

                loadComplete: function (data) {
                    if (self.lastSelectedRowID) {
                        self.jqGrid.setSelection(self.lastSelectedRowID, true);
                    }
                },
                loadError: function (xhr, status, error) {
                    if (xhr.status == 403)
                        alert('Доступ заборонений');
                },
                onSelectRow: function (rowid, status) {
                    if (rowid) {
                        self.lastSelectedRowID = rowid;
                    }
                },

                ondblClickRow: function (rowid, iRow, iCol, e) {
                    //self.showUpdateForm();
                },

                multiselect: false
            })
            .filterToolbar({ searchOnEnter: true, stringResult: true, defaultSearch: "cn", groupOp: "AND" })
            .bindKeys({
                onEnter: function (rowid) {
                    //self.showUpdateForm();
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
            /*
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
            */
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
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' });

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
                postData = {},
                title = null;

            if (o) {
                if (o.data)
                    postData = o.data;
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


            var name = $('<input type="text" style="width: 400px;">').val(postData.name ? postData.name : '');
            table.append(row('Відомство:', name));

            var department = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(postData.departmentName ? postData.departmentName : '')
                .attr('valueid', postData.departmentID);
            var departmentBtn = '';
            var deps = window.navigationSettings.menuList;
            if (deps.length > 1) {
                department.autocomplete({
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
                departmentBtn = documentUI.createButtonForAutocomplete(department);

                if (!postData.departmentName) {
                    for (var d in deps)
                        if (deps[d].id === parseFloat(postData.departmentID))
                            department.attr('valueid', deps[d].id).val(deps[d].name);
                }
            }
            else {
                department.attr('disabled', 'disabled');
                if (!postData.departmentName) {
                    department.attr('valueid', deps[0].id).val(deps[0].name);
                }
            }
            table.append(row('Входить до:', department.add(departmentBtn)));

            var buttonCreate = $('<input type="button" value="Зберегти">').button()
            .click(function () {
                var sendData = name.val();
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=' + o.type + '&dep=' + documentUI.departmentID;

                $.ajax({
                    url: urlRequest,
                    type: "POST",
                    cache: false,
                    data: { 'data': sendData },
                    dataType: "json",
                    success: function (msg) {
                        self.lastSelectedRowID = parseFloat(msg.Data);
                        self.jqGrid.trigger("reloadGrid");
                        form.dialog("close");
                    },
                    error: function (xhr, status, error) {
                        alert(xhr.responseText);
                    }
                });
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
            this.createFieldForm({ data: { id: 0 }, type: 'ins', title: 'Створення відмства' });
        },
        this.showUpdateForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);

                this.createFieldForm({
                    data: {
                        id: rowObj.id,
                        name: rowObj.name,
                        departmentID: rowObj.departmentID,
                        departmentName: rowObj.departmentName,
                        isVacant: (rowObj.isVacant.toLowerCase() == "true"),
                        postTypeID: rowObj.postTypeID
                    },
                    type: 'upd',
                    title: 'Редагування відмства'
                });
            }
            else
                alert("Будь ласка виберіть запис!");
        },
        this.showDelForm = function () {
            var rowId = self.jqGrid.getGridParam('selrow');
            if (rowId) {
                var rowObj = self.jqGrid.getRowData(rowId);
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=del&pid=' + rowObj.id + '&dep=' + documentUI.departmentID;

                var deleteDlg = $('<div title="Видалити?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
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
                                            deleteDlg.dialog("close");
                                            self.jqGrid.trigger("reloadGrid");
                                        }
                                    });
                                },
                                "Відмінити": function () {
                                    $(this).dialog("close");
                                }
                            },
                            open: function (event, ui) {
                                //if ($.ui.dialog.overlay.instances.length > 0)
                                //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                            },
                            close: function (event, ui) {
                                if (deleteDlg)
                                    deleteDlg.remove();
                            }
                        });
            }
            else
                alert("Будь ласка виберіть запис!");
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
                //self.showDelForm();
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

    window.DepartmentUi = departmentUi;
})(window, jQuery);