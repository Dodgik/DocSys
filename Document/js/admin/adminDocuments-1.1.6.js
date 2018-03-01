(function (window, $, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var adminDocuments = function (selector, options) {
        options = options || {};
        var self = this,
            dicts = window.appData.dictionaries;

        this.appendTo = 'body',
        this.departmentID = 0,
        this.templateID = 0,
        this.jqGridID = '',
        this.lastSelectedRowID = 0,
        this.searchPanel = {},
        this.searchPanelContainer = '',
        this.detailsCells = [],
        this.rowsToColor = [],
        this.rowsToColor2 = [],
        this.rowsToColorRead = [],
        this.defaultStartDate = new Date(new Date().getFullYear(), new Date().getMonth() - 6),
        this.defaultEndDate = new Date(new Date().getFullYear(), new Date().getMonth() + 1);

        if (options.appendTo) {
            this.appendTo = options.appendTo;
        }
        if (options.departmentID) {
            this.departmentID = options.departmentID;
        }
        if (options.templateID) {
            this.templateID = options.templateID;
        }

        function isVisibleCol(name) {
            return localStorage['col_visible_' + name] === 'true';
        }

        function setVisibleCol(name, val) {
            localStorage['col_visible_' + name] = val;
        }

        function getVisibleCol(name) {
            return localStorage['col_visible_' + name];
        }

        function getLabel(id) {
            var l = null, i = 0, j = dicts.labels.length;
            for (; i < j; i++) {
                if (dicts.labels[i].id === id) {
                    l = dicts.labels[i];
                    break;
                }
            }
            return l;
        }

        function getLabels() {
            var l = [], i = 0, j = dicts.labels.length;
            for (; i < j; i++) {
                if (dicts.labels[i].departmentId === self.departmentID) {
                    l.push(dicts.labels[i]);
                }
            }
            return l;
        }

        function getFilterParams() {
            var sText = self.searchPanel.searchInput.val(),
                creationDateStart = self.searchPanel.dateInputFromPeriod.val(),
                creationDateEnd = self.searchPanel.dateInputToPeriod.val(),
                postData = $('#dg' + selector).getGridParam('postData'),
                endDateFrom = self.searchPanel.filterOptions.find('input[name=EndDateFrom]').val(),
                endDateTo = self.searchPanel.filterOptions.find('input[name=EndDateTo]').val(),
                innerEndDateFrom = self.searchPanel.filterOptions2.find('input[name=InnerEndDateFrom]').val(),
                innerEndDateTo = self.searchPanel.filterOptions2.find('input[name=InnerEndDateTo]').val(),
                isDepartmentOwner = $('input[name=IsDepartmentOwner]:checked').val(),
                lableId = $('select[name=LableID]').val(),
                docStatusId = self.searchPanel.filterOptions5.find('select[name=DocStatusID]').val();

            var filters = (postData && postData.filters) ? JSON.parse(postData.filters) : { groupOp: 'AND', rules: [] };

            if (sText) {
                filters.rules.push({ 'field': 'Content', 'op': 'cn', 'data': sText });
            }
            if (creationDateStart) {
                filters.rules.push({ 'field': 'CreationDateStart', 'op': 'cn', 'data': creationDateStart });
            }
            if (creationDateEnd) {
                filters.rules.push({ 'field': 'CreationDateEnd', 'op': 'cn', 'data': creationDateEnd });
            }
            if (self.searchPanel.filterOptions.find('input[name=controlled]').is(':checked')) {
                filters.rules.push({ 'field': 'Controlled', 'op': 'cn', 'data': true });
            }
            if (endDateFrom) {
                filters.rules.push({ 'field': 'EndDateFrom', 'op': 'cn', 'data': endDateFrom });
            }
            if (endDateTo) {
                filters.rules.push({ 'field': 'EndDateTo', 'op': 'cn', 'data': endDateTo });
            }
            if (self.searchPanel.filterOptions2.find('input[name=ControlledInner]').is(':checked')) {
                filters.rules.push({ 'field': 'ControlledInner', 'op': 'cn', 'data': true });
            }
            if (innerEndDateFrom) {
                filters.rules.push({ 'field': 'InnerEndDateFrom', 'op': 'cn', 'data': innerEndDateFrom });
            }
            if (innerEndDateTo) {
                filters.rules.push({ 'field': 'InnerEndDateTo', 'op': 'cn', 'data': innerEndDateTo });
            }
            if (isDepartmentOwner) {
                if (isDepartmentOwner.toLowerCase() === 'true') {
                    filters.rules.push({ 'field': 'IsDepartmentOwner', 'op': 'cn', 'data': true });
                } else {
                    filters.rules.push({ 'field': 'IsDepartmentOwner', 'op': 'cn', 'data': false });
                }
            }
            if (lableId) {
                filters.rules.push({ 'field': 'LableID', 'op': 'cn', 'data': lableId });
            }
            if (docStatusId) {
                filters.rules.push({ 'field': 'DocStatusID', 'op': 'cn', 'data': docStatusId });
            }

            return filters;
        }

        this.init = function () {

            var createDetailCell = function (cell) {
                self.detailsCells.push(cell);
                return cell;
            };

            $('#' + selector).remove();

            this.searchPanel.searchInput = $('<input type="text" style="width: 275px;">').keypress(function (e) {
                if (e.which == 13)
                    $('#dg' + selector)[0].triggerToolbar();
            });
            this.searchPanel.searchButton = $('<input type="button" value="Пошук" style="margin-left: 10px;">').button().click(function () {
                $('#dg' + selector)[0].triggerToolbar();
            });
            this.searchPanel.textFromPeriod = $('<span> За період з:</span>');

            this.searchPanel.dateInputFromPeriod = $('<input type="text" style="width: 80px;">').datepicker({ changeMonth: true, changeYear: true })
                .datepicker('setDate', self.defaultStartDate)
                .keypress(function (e) {
                    if (e.which == 13)
                        $('#dg' + selector)[0].triggerToolbar();
                });
            this.searchPanel.textToPeriod = $('<span> по: </span>');

            this.searchPanel.dateInputToPeriod = $('<input type="text" style="width: 80px;">').datepicker({ changeMonth: true, changeYear: true })
                .datepicker('setDate', self.defaultEndDate)
                .keypress(function (e) {
                    if (e.which == 13)
                        $('#dg' + selector)[0].triggerToolbar();
                });
            this.searchPanel.clearButton = $('<input type="button" value="Очистити" style="margin-left: 25px;">').button().click(function () {
                self.clearFilter();
            });

            this.searchPanel.hiddenSearchPanel = $('<div></div>');
            this.searchPanel.filterRow1 = $('<div class="filter-row"></div>').appendTo(this.searchPanel.hiddenSearchPanel);
            this.searchPanel.filterRow2 = $('<div class="filter-row"></div>').appendTo(this.searchPanel.hiddenSearchPanel);
            this.searchPanel.filterRow3 = $('<div class="filter-row"></div>').appendTo(this.searchPanel.hiddenSearchPanel);

            this.searchPanel.columnsOptions = $('<div class="columns-options">Колонки: </div>').appendTo(this.searchPanel.filterRow1);
            this.searchPanel.filterOptions = $('<div class="filter-options"></div>').appendTo(this.searchPanel.filterRow1);
            this.searchPanel.btnDownloadPage = $('<button class="btn fr download-page" title="Скачати сторінку"></button>').appendTo(this.searchPanel.filterRow1).click(function () {
                var filters = getFilterParams(),
                    postData = $('#dg' + selector).getGridParam('postData'),
                    i = 0, j = filters.rules.length,
                    vars = {},
                    getStr = '&page=' + postData.page + '&rows=' + postData.rows;
                for (; i < j; i++) {
                    var p = filters.rules[i];
                    vars[p.field] = p.data;
                }
                for (var v in vars) {
                    getStr = getStr + '&' + v + '=' + vars[v];
                }
                console.log(postData);
                console.log(filters);
                console.log(getStr);

                var reportUrl = 'Handlers/DataPoint.ashx?obj=adocs&type=getpagedoc' + getStr + '&dep=' + userdata.departmentId;
                window.location.href = window.appSettings.rootUrl + reportUrl;
                return false;
            });
            this.searchPanel.searchSubPanel = $('<div id="accordion"><h3></h3></div>').append(this.searchPanel.hiddenSearchPanel);


            this.searchPanel.filterOptions.append('<label><input name="controlled" type="checkbox">На контролі</label>');
            this.searchPanel.filterOptions.append(
                $('<label> | Дата контролю - з: </label>').append($('<input name="EndDateFrom" type="text" style="width: 80px;">')
                    .datepicker({ changeMonth: true, changeYear: true }))
                    .keypress(function (e) {
                        if (e.which == 13)
                            $('#dg' + selector)[0].triggerToolbar();
                    })
                    .append(' по: ')
                    .append($('<input name="EndDateTo" type="text" style="width: 80px;">')
                    .datepicker({ changeMonth: true, changeYear: true }))
                    .keypress(function (e) {
                        if (e.which == 13)
                            $('#dg' + selector)[0].triggerToolbar();
                    })
            );

            if (getVisibleCol('InnerNumber') == undefined) {
                setVisibleCol('InnerNumber', true);
            }

            var isInnerNumber = isVisibleCol('InnerNumber');

            this.searchPanel.columnsOptions
                .append($('<label>Внутрішній № </label>')
                    .prepend($('<input type="checkbox" ' + (isInnerNumber ? 'checked="checked" ' : '') + '>').change(function () {
                        if ($(this).is(':checked')) {
                            $('#dg' + selector).jqGrid('showCol', 'InnerNumber');
                            setVisibleCol('InnerNumber', true);
                        } else {
                            $('#dg' + selector).jqGrid('hideCol', 'InnerNumber');
                            setVisibleCol('InnerNumber', false);
                        }
                    }))
                );

            this.searchPanel.filterOptions2 = $('<div class="filter-options">Внутрішній контроль: </div>').appendTo(this.searchPanel.filterRow2);
            $('<label><input name="ControlledInner" type="checkbox">На контролі</label>').appendTo(this.searchPanel.filterOptions2);
            this.searchPanel.filterOptions2.append($('<label> | Дата контролю - з: </label>').append($('<input name="InnerEndDateFrom" type="text" style="width: 80px;">')
                .datepicker({ changeMonth: true, changeYear: true }))
                .keypress(function (e) {
                    if (e.which == 13)
                        $('#dg' + selector)[0].triggerToolbar();
                })
                .append(' по: ')
                .append($('<input name="InnerEndDateTo" type="text" style="width: 80px;">')
                .datepicker({ changeMonth: true, changeYear: true }))
                .keypress(function (e) {
                    if (e.which == 13)
                        $('#dg' + selector)[0].triggerToolbar();
                })
            );

            this.searchPanel.filterOptions3 = $('<div class="filter-options">Відомство реэстратор документу: </div>').appendTo(this.searchPanel.filterRow3);
            $('<label><input type="radio" name="IsDepartmentOwner" value="" checked="checked" class="default-value">Всі</label>').appendTo(this.searchPanel.filterOptions3);
            $('<label><input type="radio" name="IsDepartmentOwner" value="true">Це</label>').appendTo(this.searchPanel.filterOptions3);
            $('<label><input type="radio" name="IsDepartmentOwner" value="false">Інше</label>').appendTo(this.searchPanel.filterOptions3);

            
            var depLabels = getLabels();
            if (depLabels.length) {
                this.searchPanel.filterOptions4 = $('<div class="filter-options">&nbsp;&nbsp;&nbsp;&nbsp; | &nbsp;&nbsp;&nbsp;&nbsp; Вибіркова мітка: </div>').appendTo(this.searchPanel.filterRow3);
                $('<select name="LableID" class=""><option class="default-value" value=""></option>' +
                    (function() {
                        var str = '';
                        depLabels.forEach(function (depLabel) {
                            str += '<option value="' + depLabel.id + '">' + depLabel.name + '</option>';
                        });
                        return str;
                    })() +
                    '</select>').appendTo(this.searchPanel.filterOptions4);
            }
            
            this.searchPanel.filterOptions5 = $('<div class="filter-options">&nbsp;&nbsp;&nbsp;&nbsp; | &nbsp;&nbsp;&nbsp;&nbsp; Результат розгляду: </div>').appendTo(this.searchPanel.filterRow3);
            $('<select name="DocStatusID" class=""><option class="default-value" value=""></option>' +
                (function () {
                    var str = '';
                    dicts.docStatuses.forEach(function (docStatus) {
                        str += '<option value="' + docStatus.id + '">' + docStatus.name + '</option>';
                    });
                    return str;
                })() +
                '</select>').appendTo(this.searchPanel.filterOptions5);
            
            this.searchPanel.searchSubPanel.accordion({ collapsible: true, active: false, animate: 0, heightStyle: 'content' });

            this.searchPanelContainer = this.searchPanel.searchInput
                .add(this.searchPanel.textFromPeriod)
                .add(this.searchPanel.dateInputFromPeriod)
                .add(this.searchPanel.textToPeriod)
                .add(this.searchPanel.dateInputToPeriod)
                .add(this.searchPanel.searchButton)
                .add(this.searchPanel.clearButton)
                .add(this.searchPanel.searchSubPanel);

            $(this.appendTo).append($('<table></table>').attr('id', selector)
                    .append($('<tr></tr>').append($('<td colspan=3 valign="top"></td>').append(this.searchPanelContainer)))
                    .append($('<tr></tr>').append($('<td valign="top"></td>')
                    .append('<div id="pager' + selector + '"></div>')
                    .append('<table id="dg' + selector + '"><tr><td></td></tr></table>'))
                    .append($('<td id="collapsePanel' + selector + '" valign="top" style="overflow-y: scroll; overflow-x: hidden;" class="ui-state-default ui-corner-all"></td>')
                    .append('<div id="loadDetails' + selector + '" class="loading-icon" style="display: none; position: relative; left: 150px; top: 270px; z-index: 1"></div>')
                    .append($('<table></table>').attr('id', 'tblOverview' + selector)
                        .append($('<tr><td colspan="2" align="center">ДЕТАЛІ</td></tr>'))
                        .append($('<tr><td style="background-color: #F0F0F0"></td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Дата:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Дата:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Зміст:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Зміни:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Відмітки:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Контроль:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Тип:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Питання:</td></tr>').append(createDetailCell($('<td></td>'))))
                        .append($('<tr><td style="background-color: #F0F0F0">Статус:</td></tr>').append(createDetailCell($('<td></td>'))))
                            ))
                    .append($('<td id="collapseHeader' + selector + '" valign="middle" style="width: 16px; cursor:pointer;" class="ui-accordion-header ui-helper-reset ui-state-active ui-corner-top"></td>').append('<span class="ui-icon ui-icon-circle-arrow-w" style="float:left;"></span>'))
                )
            );



            this.jqGridID = 'dg' + selector;

            $('#' + this.jqGridID).jqGrid({
                url: options.url,
                datatype: 'json',
                mtype: 'POST',
                height: '550',
                colNames: ['№', 'documentID', 'Дата', '№', '№ в організації',
                    'Зміст', 'Зміни', 'Відмітки', 'Контроль', 'Особливий контроль',
                    'Підвищений контроль', '__', 'docTypeID', 'docTypeName', 'docStatusID',
                    'docStatusName', 'Шифр', 'questionTypeID', 'questionTypeName', 'Організація',
                    'DepartmentID', 'SourceDepartmentID', 'DestinationDepartmentID', 'Вн. №', 'ControlCardID'],
                colModel: [
                    { name: 'id', index: 'DocTemplateID', width: 50, sortable: true, hidden: true },
                    { name: 'documentID', width: 20, sortable: false, hidden: true },
                    { name: 'creationDate', index: 'CreationDate', width: 100, sortable: true, searchoptions: { dataInit: function (el) { $(el).datepicker({ changeMonth: true, changeYear: true }); } } },
                    { name: 'number', index: 'Number', width: 60, sortable: true },
                    { name: 'externalNumber', index: 'ExternalNumber', width: 190, sortable: false },
                    
                    { name: 'content', sortable: false, hidden: true },
                    { name: 'changes', sortable: false, hidden: true },
                    { name: 'notes', sortable: false, hidden: true },
                    { name: 'isControlled', sortable: false, hidden: true },
                    { name: 'isSpeciallyControlled', index: 'IsSpeciallyControlled', width: 80, sortable: false, hidden: true },
                    
                    { name: 'isIncreasedControlled', index: 'IsIncreasedControlled', width: 80, sortable: false, hidden: true },
                    { name: 'isInput', width: 16, index: 'IsInput', sortable: false, formatter: function (cellValue, cOptions, rowObject) {
                            if (cellValue.toLowerCase() === 'true') {
                                return '<span class="ui-icon ui-icon-arrowthick-1-s ui-icon-red">' + cellValue + '</span>';
                            } else {
                                return '<span class="ui-icon ui-icon-arrowthick-1-n ui-icon-green">' + cellValue + '</span>';
                            }
                        },
                        searchoptions: { dataInit: function(el) {
                            $(el).attr('type', 'hidden');
                            $(el).after('<span class="dn ui-icon ui-icon-arrowthick-1-s ui-icon-red" style="margin-top: 3px;">True</span>');
                            $(el).after('<span class="dn ui-icon ui-icon-arrowthick-1-n ui-icon-green" style="margin-top: 3px;">False</span>');
                            $(el).parent('div').addClass('cp filter-isInput').on('click', function () {
                                var red = $(this).find('.ui-icon-red'),
                                    green = $(this).find('.ui-icon-green'),
                                    input = $(this).find('input');
                                if (red.is(':visible')) {
                                    $(this).css('background-color', 'yellow');
                                    red.addClass('dn');
                                    green.removeClass('dn');
                                    input.val('false');
                                } else if (green.is(':visible')) {
                                    $(this).css('background-color', '');
                                    red.addClass('dn');
                                    green.addClass('dn');
                                    input.val('');
                                } else {
                                    $(this).css('background-color', 'yellow');
                                    red.removeClass('dn');
                                    green.addClass('dn');
                                    input.val('true');
                                }
                            });
                        } }
                    },
                    { name: 'docTypeID', width: 40, sortable: false, hidden: true },
                    { name: 'docTypeName', width: 40, sortable: false, hidden: true },
                    {
                        name: 'docStatusID', width: 40, sortable: false, hidden: true, formatter: function (cellValue, cOptions, rowObject) {
                            if (rowObject[8].toLowerCase() == 'true')
                                self.rowsToColor.push(cOptions.rowId);
                            if (rowObject[10].toLowerCase() == 'true')
                                self.rowsToColor2.push(cOptions.rowId);
                            if (rowObject[20].toLowerCase() !== userdata.departmentId.toString()) {
                                self.rowsToColorRead.push(cOptions.rowId);
                            }
                            return cellValue;
                        }
                    },
                    
                    { name: 'docStatusName', width: 40, sortable: false, hidden: true },
                    { name: 'documentCodeID', index: 'DocumentCodeID', width: 50, sortable: false },
                    { name: 'questionTypeID', width: 40, sortable: false, hidden: true },
                    { name: 'questionTypeName', width: 40, sortable: false, hidden: true },
                    { name: 'organizationName', index: 'OrganizationName', width: 360, sortable: false },
                    { name: 'DepartmentID', width: 0, sortable: false, hidden: true },
                    { name: 'SourceDepartmentID', width: 0, sortable: false, hidden: true },
                    { name: 'DestinationDepartmentID', width: 0, sortable: false, hidden: true },
                    { name: 'InnerNumber', index: 'InnerNumber', width: 90, sortable: false, hidden: !isInnerNumber },
                    { name: 'ControlCardID', width: 0, sortable: false, hidden: true }
                ],
                rownumbers: true,
                rowNum: 100,
                rowList: [50, 100, 200, 500, 1000, 2000],
                viewrecords: true,
                pager: '#pager' + selector,
                scroll: false,
                scrollrows: true,
                sortname: 'creationDate',
                sortorder: 'desc',

                //toolbar: [true, 'top'],
                toppager: true,
                
                beforeRequest: function () {
                    var filters = getFilterParams();

                    $('#dg' + selector).jqGrid('setGridParam', { postData: { filters: JSON.stringify(filters) }, search: true });

                    self.rowsToColor = [];
                    self.rowsToColor2 = [];
                    self.rowsToColorRead = [];
                },
                gridComplete: function () {
                    for (var i = 0; i < self.rowsToColor.length; i++) {
                        //var status = $('#' + self.rowsToColor[i]).find('td').eq(7).html();
                        //if (status == 'Complete') {
                        $('#' + self.rowsToColor[i]).addClass('ui-state-highlight-2').removeClass('ui-widget-content');
                        // }
                    }
                    for (var r = 0; r < self.rowsToColor2.length; r++) {
                        $('#' + self.rowsToColor2[r]).addClass('ui-state-highlight-red');
                    }
                    for (var r = 0; r < self.rowsToColorRead.length; r++) {
                        $('#' + self.rowsToColorRead[r]).addClass('ui-state-highlight-readonly');
                    }
                },

                loadComplete: function () {
                    if (self.lastSelectedRowID)
                        $('#dg' + selector).setSelection(self.lastSelectedRowID, true);
                },
                loadError: function (xhr) {
                    if (xhr.status == 403) {
                        alert('Доступ заборонений');
                    }
                },
                onSelectRow: function (rowid) {
                    if (rowid) {
                        self.lastSelectedRowID = rowid;
                        var rowObj = $('#dg' + selector).jqGrid('getRowData', rowid),
                            isInput = $(rowObj.isInput).text().toLowerCase();

                        self.detailsCells[0].text(isInput == 'true' ? 'Вхідний' : 'Вихідний');
                        self.detailsCells[1].text(rowObj.number);
                        self.detailsCells[2].text(rowObj.creationDate);
                        self.detailsCells[3].text(rowObj.content);
                        self.detailsCells[4].text(rowObj.changes);
                        self.detailsCells[5].text(rowObj.notes);
                        self.detailsCells[6].text(rowObj.isSpeciallyControlled.toLowerCase() == 'true' ? 'так' : 'ні');
                        self.detailsCells[7].text(rowObj.docTypeName);
                        self.detailsCells[8].text(rowObj.questionTypeName);
                        self.detailsCells[9].text(rowObj.docStatusName);
                    }
                },

                ondblClickRow: function (rowid, iRow, iCol, e) {
                    e.stopPropagation();
                    self.showViewForm();
                },

                multiselect: false,
                subGrid: true,
                subGridOptions: {
                    'plusicon': 'ui-icon-triangle-1-e',
                    'minusicon': 'ui-icon-triangle-1-s',
                    'openicon': 'ui-icon-arrowreturn-1-e',
                    'reloadOnExpand': false,
                    'selectOnExpand': true
                },

                subGridRowExpanded: function (subGridId, rowId) {
                    var subgridTableId = subGridId + '_t',
                        pagerId = 'p_' + subgridTableId;

                    var currentGridId = 'dg' + selector;

                    createSubGrid({ currentGridId: currentGridId, pagerId: pagerId, rowId: rowId, subgridTableId: subgridTableId, subGridId: subGridId });
                }
            })
            .filterToolbar({ searchOnEnter: true, stringResult: true, defaultSearch: 'cn', groupOp: 'AND' })
            .bindKeys({
                onEnter: function (rowid) {
                    self.showViewForm();
                },
                scrollingRows: true
            })
            .navGrid('#pager' + selector, { add: false, edit: false, view: false, del: false, search: false, refresh: false, cloneToTop: true })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: '',
                title: 'Додати новий запис',
                buttonicon: 'ui-icon-plus',
                onClickButton: function () {
                    self.showInsertForm();
                    return false;
                },
                position: 'last'
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: '',
                title: 'Змінити вибраний запис',
                buttonicon: 'ui-icon-pencil',
                onClickButton: function () {
                    self.showUpdateForm();
                    return false;
                },
                position: 'last'
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: '',
                title: 'Переглянути обраний запис',
                buttonicon: 'ui-icon-document',
                onClickButton: function () {
                    self.showViewForm();
                    return false;
                },
                position: 'last'
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: '',
                title: 'Видалити вибраний запис',
                buttonicon: 'ui-icon-trash',
                onClickButton: function () {
                    self.showDelForm();
                    return false;
                },
                position: 'last'
            })
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: '',
                title: 'Показати панель пошуку',
                buttonicon: 'ui-icon-search',
                onClickButton: function () {
                    $('#dg' + selector)[0].toggleToolbar();
                    return false;
                },
                position: 'last'
            })
            .navButtonAdd('#pg_' + this.jqGridID + '_toppager', {
                caption: '',
                title: 'Поновити',
                buttonicon: 'ui-icon-refresh',
                onClickButton: function () {
                    $('#dg' + selector)[0].triggerToolbar();
                    return false;
                },
                position: 'last'
            })
            .navSeparatorAdd('#pg_' + this.jqGridID + '_toppager', { sepclass: 'ui-separator', sepcontent: '' })
            .navButtonAdd('#pg_' + self.jqGridID + '_toppager', {
                caption: '',
                title: 'Відповідь',
                buttonicon: 'ui-icon-arrowreturnthick-1-w',
                onClickButton: function () {
                    self.showReplayForm();
                    return false;
                },
                position: 'last'
            });

            //console.log('g-width=' + $('#' + this.jqGridID).width());
            //$('#' + this.jqGridID).setGridWidth($('#' + this.jqGridID).width());

            var dialogWidth = $(this.appendTo).width();
            if (dialogWidth > 1100) {
                var gWidth = $('#' + this.jqGridID).width();
                $('#' + this.jqGridID).setGridWidth(gWidth + dialogWidth - 1100);
            }
            
            function createSubGrid(p) {
                var scope = this,
                    replayGridId = p.subgridTableId + 'replay',
                    replayPagerId = p.pagerId + 'replay';
                $('#' + p.subGridId).append('<div>Виконавці:</div>');
                var rowObj = $('#' + p.currentGridId).jqGrid('getRowData', p.rowId);

                var cardListUrl = '';
                if (rowObj.DepartmentID !== userdata.departmentId.toString() && rowObj.ControlCardID) {
                    //cardListUrl = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=getsubcards&id=' + rowObj.ControlCardID + '&dep=' + userdata.departmentId;
                    cardListUrl = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=getdeptop&id=' + rowObj.documentID + '&dep=' + userdata.departmentId;
                } else {
                    cardListUrl = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=getlist&id=' + rowObj.documentID + '&dep=' + userdata.departmentId;
                }
                //var cardListUrl = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=getlist&id=' + rowObj.documentID + '&dep=' + userdata.departmentId;

                createSubCardsGrid({
                    currentGridId: p.currentGridId,
                    pagerId: p.pagerId,
                    rowId: p.rowId,
                    subgridTableId: p.subgridTableId,
                    subGridId: p.subGridId,
                    url: cardListUrl,
                    ParentControlCardID: rowObj.ControlCardID
                });

                function createSubCardsGrid(s) {
                    $('#' + s.subGridId).append('<table id="' + s.subgridTableId + '" class="scroll"></table><div id="' + s.pagerId + '" class="scroll"></div>');

                    $('#' + s.subgridTableId).jqGrid({
                        url: s.url,
                        datatype: 'json',
                        mtype: 'POST',
                        height: '100%',
                        colNames: ['id', 'documentID', '№', 'Початок', 'Виконати до', 'Стан картки', 'Виконавець', 'Контрольна відповідь', 'Резолюція', 'Бланк', 'groupId', 'InnerNumber', 'Дії'],
                        colModel: [
                            { name: 'id', index: 'id', width: 80, sortable: false, hidden: true },
                            { name: 'documentID', width: 130, sortable: false, hidden: true },
                            { name: 'cardNumber', width: 20, sortable: false },
                            { name: 'startDate', width: 90, sortable: false },
                            { name: 'endDate', width: 100, sortable: false },
                            { name: 'cardStatusName', width: 300, sortable: false },
                            { name: 'workerName', width: 160, sortable: false },
                            { name: 'ControlResponse', width: 130, sortable: false, hidden: true },
                            { name: 'Resolution', width: 130, sortable: false, hidden: true },
                            { name: 'get', width: 85, formatter: 'showlink', hidden: true },
                            { name: 'groupId', width: 50, sortable: false, hidden: true },
                            { name: 'InnerNumber', width: 20, sortable: false, hidden: true },
                            { name: 'ActionCommentID', width: 80, sortable: false, formatter: function(cellValue, cOptions, rowObject) {
                                    var link = '';
                                    if (cellValue && cellValue !== '0') {
                                        link = '<span class="open-action-info" actioncommentid= "' + cellValue + '" documentid= "' + rowObject[1] + '">Виконано</span>';
                                    }
                                    return link;
                                }
                            }
                        ],
                        rowNum: 1000,
                        rowList: [],
                        pgbuttons: false,
                        viewrecords: true,
                        pgtext: null,
                        pager: '#' + s.pagerId,

                        loadError: function (xhr) {
                            if (xhr.status == 403)
                                alert('Доступ заборонений');
                        },

                        ondblClickRow: function (rowid, iRow, iCol, e) {
                            e.stopPropagation();
                            var subRowObj = $('#' + s.subgridTableId).jqGrid('getRowData', rowid),
                                groupId = subRowObj.groupId ? parseInt(subRowObj.groupId, 10) : 0;

                            //var ccBlank = new ControlCardBlankD({ departmentID: self.departmentID, templateID: self.templateID });
                            var ccBlank = new ControlCardGroupDocument({
                                departmentID: self.departmentID,
                                documentID: rowObj.documentID,
                                templateID: self.templateID,
                                groupCards: !!groupId
                            });
                            ccBlank.showViewForm({ controlCardID: subRowObj.id }, function () {
                                $('#' + s.subgridTableId).trigger('reloadGrid');
                            });
                        },

                        multiselect: false,
                        subGrid: true,
                        subGridOptions: {
                            'plusicon': 'ui-icon-triangle-1-e',
                            'minusicon': 'ui-icon-triangle-1-s',
                            'openicon': 'ui-icon-arrowreturn-1-e',
                            'reloadOnExpand': false,
                            'selectOnExpand': true,
                            expandOnLoad: false
                        },

                        subGridRowExpanded: function (subGridId, rowId) {
                            var subgridTableId = subGridId + '_ct',
                                pagerId = 'cp_' + subgridTableId;

                            var currentGridId = replayGridId;
                            
                            var subRowObj = $('#' + s.subgridTableId).jqGrid('getRowData', rowId);
                            
                            createSubCardsGrid({
                                currentGridId: currentGridId,
                                pagerId: pagerId,
                                rowId: rowId,
                                subgridTableId: subgridTableId,
                                subGridId: subGridId,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=getsubcards&id=' + subRowObj.id + '&dep=' + userdata.departmentId,
                                ParentControlCardID: subRowObj.id
                            });
                        }
                    })
                    .navGrid('#' + s.pagerId, { add: false, edit: false, view: false, del: false, search: false, refresh: false })
                    .navButtonAdd('#' + s.pagerId, {
                        caption: '',
                        title: 'Додати новий запис',
                        buttonicon: 'ui-icon-plus',
                        onClickButton: function () {
                            var ccBlank = new ControlCardBlankD({
                                departmentID: self.departmentID,
                                documentID: rowObj.documentID,
                                templateID: self.templateID
                            });

                            var cardNumber = 1;
                            var cardNumbers = $('#' + s.subgridTableId).jqGrid('getCol', 'cardNumber', false);
                            if (cardNumbers && cardNumbers.length > 0)
                                cardNumber = cardNumber + Math.max.apply(Math, cardNumbers);

                            ccBlank.showInsertForm({ cardNumber: cardNumber, documentID: rowObj.documentID, ParentControlCardID: s.ParentControlCardID }, function () {
                                $('#' + s.subgridTableId).trigger('reloadGrid');
                            });
                        },
                        position: 'last'
                    })
                    .navButtonAdd('#' + s.pagerId, {
                        caption: '',
                        title: 'Редагувати вибраний запис',
                        buttonicon: 'ui-icon-pencil',
                        onClickButton: function () {
                            var subRowId = $('#' + s.subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $('#' + s.subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankD({ departmentID: self.departmentID, templateID: self.templateID });
                                ccBlank.showUpdateForm({ controlCardID: subRowObj.id }, function () {
                                    $('#' + s.subgridTableId).trigger('reloadGrid');
                                });
                            } else {
                                alert('Будь ласка виберіть картку!');
                            }
                        },
                        position: 'last'
                    })
                    .navButtonAdd('#' + s.pagerId, {
                        caption: '',
                        title: 'Переглянути обраний запис',
                        buttonicon: 'ui-icon-document',
                        onClickButton: function () {
                            var subRowId = $('#' + s.subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $('#' + s.subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankD({ departmentID: self.departmentID, templateID: self.templateID });
                                ccBlank.showViewForm({ controlCardID: subRowObj.id }, function () {
                                    $('#' + s.subgridTableId).trigger('reloadGrid');
                                });
                            } else {
                                alert('Будь ласка виберіть картку!');
                            }
                        },
                        position: 'last'
                    })
                    .navButtonAdd('#' + s.pagerId, {
                        caption: '',
                        title: 'Продовжити обраний запис',
                        buttonicon: 'ui-icon-link',
                        onClickButton: function () {
                            var subRowId = $('#' + s.subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $('#' + s.subgridTableId).jqGrid('getRowData', subRowId);

                                var ccBlank = new ControlCardBlankD({
                                    departmentID: self.departmentID,
                                    documentID: rowObj.documentID,
                                    templateID: self.templateID
                                });
                                ccBlank.showContinueForm({ controlCardID: subRowObj.id }, function () {
                                    $('#' + s.subgridTableId).trigger('reloadGrid');
                                });
                            } else {
                                alert('Будь ласка виберіть картку!');
                            }
                        },
                        position: 'last'
                    })
                    .navButtonAdd('#' + s.pagerId, {
                        caption: '',
                        title: 'Видалити вибраний запис',
                        buttonicon: 'ui-icon-trash',
                        onClickButton: function () {
                            var subRowId = $('#' + s.subgridTableId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $('#' + s.subgridTableId).jqGrid('getRowData', subRowId);
                                if (subRowObj.InnerNumber) {
                                    alert('Зняти виконавця неможливо, так як він зареєстрував у себе цей документ');
                                } else {
                                    var ccBlank = new ControlCardBlankD({ departmentID: self.departmentID, templateID: self.templateID });
                                    ccBlank.showDeleteForm({ controlCardID: subRowObj.id }, function() {
                                        $('#' + s.subgridTableId).trigger('reloadGrid');
                                    });
                                }
                            } else {
                                alert('Будь ласка виберіть картку!');
                            }
                        },
                        position: 'last'
                    })
                    .navSeparatorAdd('#' + s.pagerId, { sepclass: 'ui-separator', sepcontent: '' })
                    .navSeparatorAdd('#' + s.pagerId, { sepclass: 'ui-separator', sepcontent: '' })
                    .navButtonAdd("#" + s.pagerId, {
                        caption: "",
                        title: "Додати новий запис",
                        buttonicon: "ui-icon-circle-plus",
                        onClickButton: function () {

                            var ccBlank = new ControlCardGroupDocument({
                                departmentID: self.departmentID,
                                documentID: rowObj.documentID,
                                templateID: self.templateID,
                                groupCards: true
                            });

                            var cardNumber = 1;
                            var cardNumbers = $('#' + s.subgridTableId).jqGrid('getCol', 'cardNumber', false);
                            if (cardNumbers && cardNumbers.length > 0)
                                cardNumber = cardNumber + Math.max.apply(Math, cardNumbers);

                            ccBlank.showInsertForm({ cardNumber: cardNumber, documentID: rowObj.documentID, ParentControlCardID: s.ParentControlCardID }, function () {
                                $('#' + s.subgridTableId).trigger('reloadGrid');
                            });
                        },
                        position: "last"
                    })
                    .navButtonAdd('#' + s.pagerId, {
                        caption: '',
                        title: 'Поновити',
                        buttonicon: 'ui-icon-refresh',
                        onClickButton: function () {
                            $('#' + s.subgridTableId).trigger('reloadGrid');
                            return false;
                        },
                        position: 'last'
                    });
                    
                    $('#' + s.subgridTableId).gridResize({ stop: function (grid, ev, ui) { } });
                    
                    //console.log('subg-width=' + $('#' + s.subGridId).width());
                    //$('#' + s.subgridTableId).setGridWidth($('#' + s.subGridId).width() - 40);
                }


                $('#' + p.subGridId).append('<div>Відповіді:</div>');
                $('#' + p.subGridId).append('<table id="' + replayGridId + '" class="scroll"></table><div id="' + replayPagerId + '" class="scroll"></div>');

                $('#' + replayGridId).jqGrid({
                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=adocs&type=getreplaypage&documentId=' + rowObj.documentID,
                    datatype: 'json',
                    mtype: 'POST',
                    height: '100%',
                    colNames: ['№', 'documentID', 'Дата', '№', '№ в організації', 'Зміст', 'Зміни', 'Відмітки', 'Контроль',
                        'Особливий контроль', 'Підвищений контроль', 'Вхідний', 'docTypeID', 'docTypeName', 'docStatusID', 'docStatusName', 'Шифр',
                        'questionTypeID', 'questionTypeName', 'organizationID', 'Організація'],
                    colModel: [
                        { name: 'id', index: 'DocStatementID', width: 50, sortable: true, hidden: true },
                        { name: 'documentID', width: 20, sortable: false, hidden: true },
                        {
                            name: 'creationDate',
                            index: 'CreationDate',
                            width: 100,
                            sortable: true,
                            searchoptions: {
                                dataInit: function (el) {
                                    $(el).datepicker({ changeMonth: true, changeYear: true });
                                }
                            }
                        },
                        { name: 'number', index: 'Number', width: 60, sortable: true },
                        { name: 'externalNumber', index: 'ExternalNumber', width: 190, sortable: false },
                        { name: 'content', sortable: false, hidden: true },
                        { name: 'changes', sortable: false, hidden: true },
                        { name: 'notes', sortable: false, hidden: true },
                        { name: 'isControlled', sortable: false, hidden: true },
                        { name: 'isSpeciallyControlled', index: 'IsSpeciallyControlled', width: 80, sortable: false, hidden: true },
                        { name: 'isIncreasedControlled', index: 'IsIncreasedControlled', width: 80, sortable: false, hidden: true },
                        { name: 'isInput', index: 'IsInput', sortable: false, hidden: true },
                        { name: 'docTypeID', width: 40, sortable: false, hidden: true },
                        { name: 'docTypeName', width: 40, sortable: false, hidden: true },
                        {
                            name: 'docStatusID',
                            width: 40,
                            sortable: false,
                            hidden: true,
                            formatter: function (cellValue, cOptions, rowObject) {
                                if (rowObject[8].toLowerCase() == 'true')
                                    scope.rowsToColor.push(cOptions.rowId);
                                if (rowObject[10].toLowerCase() == 'true')
                                    scope.rowsToColor2.push(cOptions.rowId);
                                /*
                                if (rowObject[20].toLowerCase() !== userdata.departmentId.toString()) {
                                    scope.rowsToColorRead.push(cOptions.rowId);
                                }
                                */
                                return cellValue;
                            }
                        },
                        { name: 'docStatusName', width: 40, sortable: false, hidden: true },
                        { name: 'documentCodeID', index: 'DocumentCodeID', width: 50, sortable: false },
                        { name: 'questionTypeID', width: 40, sortable: false, hidden: true },
                        { name: 'questionTypeName', width: 40, sortable: false, hidden: true },
                        { name: 'organizationID', width: 40, sortable: false, hidden: true },
                        { name: 'organizationName', index: 'OrganizationName', width: 360, sortable: false }
                    ],
                    rownumbers: true,
                    rowNum: 25,
                    rowList: [25, 50, 100, 200, 500, 1000],
                    viewrecords: true,
                    pager: '#' + replayPagerId,
                    scroll: false,
                    scrollrows: true,
                    sortname: 'creationDate',
                    sortorder: 'desc',

                    beforeRequest: function () {
                        scope.rowsToColor = [];
                        scope.rowsToColor2 = [];
                        scope.rowsToColorRead = [];
                    },
                    gridComplete: function () {
                        for (var i = 0; i < scope.rowsToColor.length; i++) {
                            $('#' + scope.rowsToColor[i]).addClass('ui-state-highlight-2').removeClass('ui-widget-content');
                        }
                        for (var r = 0; r < scope.rowsToColor2.length; r++) {
                            $('#' + scope.rowsToColor2[r]).addClass('ui-state-highlight-red');
                        }
                        /*
                        for (var r = 0; r < self.rowsToColorRead.length; r++) {
                            $('#' + self.rowsToColorRead[r]).addClass('ui-state-highlight-readonly');
                        }
                        */
                    },

                    loadComplete: function () {
                        if (scope.lastSelectedRowID)
                            $('#' + p.subGridId).setSelection(scope.lastSelectedRowID, true);
                    },
                    loadError: function (xhr) {
                        if (xhr.status == 403)
                            alert('Доступ заборонений');
                    },
                    ondblClickRow: function (rowid, iRow, iCol, e) {
                        e.stopPropagation();
                        var subRowObj = $('#' + replayGridId).jqGrid('getRowData', rowid);

                        var docBlank = new AdminDocumentBlank({
                            departmentID: self.departmentID,
                            templateID: self.templateID,
                            success: function () {
                                $('#' + replayGridId).trigger('reloadGrid');
                            }
                        });
                        docBlank.showViewForm(subRowObj.id);
                    },

                    multiselect: false,
                    subGrid: true,
                    subGridOptions: {
                        'plusicon': 'ui-icon-triangle-1-e',
                        'minusicon': 'ui-icon-triangle-1-s',
                        'openicon': 'ui-icon-arrowreturn-1-e',
                        'reloadOnExpand': false,
                        'selectOnExpand': true
                    },

                    subGridRowExpanded: function (subGridId, rowId) {
                        var subgridTableId = subGridId + '_t',
                            pagerId = 'p_' + subgridTableId;

                        var currentGridId = replayGridId;

                        createSubGrid({ currentGridId: currentGridId, pagerId: pagerId, rowId: rowId, subgridTableId: subgridTableId, subGridId: subGridId });
                    }
                })
                    .navGrid('#' + replayPagerId, { add: false, edit: false, view: false, del: false, search: false, refresh: false })
                    .navButtonAdd('#' + replayPagerId, {
                        caption: '',
                        title: 'Переглянути обраний запис',
                        buttonicon: 'ui-icon-document',
                        onClickButton: function () {
                            var subRowId = $('#' + replayGridId).jqGrid('getGridParam', 'selrow');
                            if (subRowId) {
                                var subRowObj = $('#' + replayGridId).jqGrid('getRowData', subRowId);

                                var docBlank = new AdminDocumentBlank({
                                    departmentID: self.departmentID,
                                    templateID: self.templateID,
                                    success: function () {
                                        $('#' + s.subgridTableId).trigger('reloadGrid');
                                    }
                                });
                                docBlank.showViewForm(subRowObj.id);
                            } else {
                                alert('Будь ласка виберіть запис!');
                            }
                        },
                        position: 'last'
                    });
                $('#' + replayGridId).gridResize({ stop: function (grid, ev, ui) { } });

            }


            $('#dg' + selector).gridResize({ minWidth: 640, minHeight: 550, stop: function (grid, ev, ui) { } });

            $('#collapseHeader' + selector).click(function (event) {
                event.preventDefault();
                if ($('#collapsePanel' + selector).is(':visible')) {
                    $('#collapseHeader' + selector).removeClass('ui-state-active ui-corner-top')
                        .addClass('ui-state-default ui-corner-all')
                        .children('.ui-icon').removeClass('ui-icon-circle-arrow-w').addClass('ui-icon-circle-arrow-e');
                    $('#collapsePanel' + selector).hide();
                } else {
                    $('#collapseHeader' + selector).removeClass('ui-state-default ui-corner-all')
                        .addClass('ui-state-active ui-corner-top')
                        .children('.ui-icon').removeClass('ui-icon-circle-arrow-e').addClass('ui-icon-circle-arrow-w');
                    $('#collapsePanel' + selector).show();
                }
                return false;
            });

            $(document).bind('keyup', self.keyCodeParser);
            $(document).on('click', '.open-action-info', function() {
                self.showComments($(this).attr('documentid'), $(this).attr('actioncommentid'));
            });
        };


        this.showInsertForm = function () {
            var docBlank = new AdminDocumentBlank({
                departmentID: self.departmentID,
                templateID: self.templateID,
                success: function (msg) {
                    if (msg)
                        self.lastSelectedRowID = parseFloat(msg);
                    $('#' + self.jqGridID).trigger('reloadGrid');
                }
            });
            docBlank.showInsertForm();
        },
        this.showUpdateForm = function () {
            var rowId = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowId) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowId);

                var docBlank = new AdminDocumentBlank({
                    departmentID: self.departmentID,
                    templateID: self.templateID,
                    success: function() {
                        $('#' + self.jqGridID).trigger('reloadGrid');
                    }
                });
                docBlank.showUpdateForm(rowObj.id);
            } else {
                alert('Будь ласка виберіть запис!');
            }
        },
        this.showViewForm = function () {
            var rowId = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowId) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowId);

                var docBlank = new AdminDocumentBlank({
                    departmentID: self.departmentID,
                    templateID: self.templateID,
                    success: function () {
                        $('#' + self.jqGridID).trigger('reloadGrid');
                    }
                });
                docBlank.showViewForm(rowObj.id);
            } else {
                alert('Будь ласка виберіть запис!');
            }
        },

        this.showReplayForm = function () {
            var rowId = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowId) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowId);

                var currentDocument = new AdminDocumentBlank({
                    departmentID: userdata.departmentId,
                    templateID: self.templateID,
                    success: function (msg) {
                        $('#' + self.jqGridID).trigger('reloadGrid');
                    }
                });

                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=getadminblank&jdata=' + rowObj.id;
                $.ajax({
                    type: 'GET',
                    cache: false,
                    url: urlRequest,
                    dataType: 'json',
                    success: function (data) {
                        var docPart = {
                            Document: {
                                ParentDocumentID: rowObj.documentID,
                                CodeID: data.Document.CodeID,
                                CodeName: data.Document.CodeName
                            },
                            IsInput: !data.IsInput,
                            Content: data.Content,
                            QuestionTypeID: data.QuestionTypeID,
                            QuestionType: data.QuestionType,
                            DocTypeID: data.DocTypeID,
                            DocType: data.DocType
                        };
                        if (data.IsInput) {
                            docPart.Document.Destination = data.Document.Source;
                            docPart.Document.Destination.Number = '';
                            docPart.Document.Destination.CreationDate = '';
                        } else {
                            docPart.Document.Source = data.Document.Destination;
                            docPart.Document.Source.Number = '';
                            docPart.Document.Source.CreationDate = '';
                        }
                        currentDocument.showInsertForm(docPart);
                    }
                });


            } else {
                alert('Будь ласка виберіть запис!');
            }
        },
        this.showDelForm = function () {
            var rowId = $('#' + self.jqGridID).getGridParam('selrow');
            if (rowId) {
                var rowObj = $('#' + self.jqGridID).getRowData(rowId);
                var docBlank = new AdminDocumentBlank({
                    departmentID: self.departmentID,
                    templateID: self.templateID,
                    success: function () {
                        $('#' + self.jqGridID).trigger('reloadGrid');
                    }
                });
                docBlank.showDeleteForm(rowObj.id, function () {
                    $('#' + self.jqGridID).trigger('reloadGrid');
                });
            } else {
                alert('Будь ласка виберіть запис!');
            }
        },

        this.showSearchPanel = function () {
            $('#dg' + selector)[0].toggleToolbar();
        },

        this.showComments = function (documentId, actionCommentId) {
            self.commentsDialog = $('<div documentId="' + documentId + '"></div>').appendTo('body');
            self.comments = window.formParts.commentsBlock({
                appendTo: self.commentsDialog,
                DocumentID: documentId,
                selectedCommentId: actionCommentId
            });
            self.commentsDialog = self.commentsDialog.dialog({
                autoOpen: true,
                draggable: true,
                modal: true,
                position: ['top'],
                resizable: true,
                width: 960,
                close: function (event, ui) {
                    self.commentsDialog.remove();
                },
                open: function (event, ui) {
                    $('.ui-widget-overlay').css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },

        this.updateGrid = function () {
            $('#dg' + selector)[0].triggerToolbar();
        },

        this.clearFilter = function () {
            self.searchPanel.searchInput.val('');
            self.searchPanel.dateInputFromPeriod.datepicker('setDate', self.defaultStartDate);
            self.searchPanel.dateInputToPeriod.datepicker('setDate', self.defaultEndDate);
            self.searchPanel.filterOptions.find('input[name=controlled]').prop("checked", false);
            self.searchPanel.filterOptions.find('input[name=EndDateFrom]').val('');
            self.searchPanel.filterOptions.find('input[name=EndDateTo]').val('');
            self.searchPanel.filterOptions2.find('input[name=ControlledInner]').prop("checked", false);
            self.searchPanel.filterOptions2.find('input[name=InnerEndDateFrom]').val('');
            self.searchPanel.filterOptions2.find('input[name=InnerEndDateTo]').val('');
            self.searchPanel.filterOptions3.find('input.default-value[name=IsDepartmentOwner]').prop('checked', true);
            if (self.searchPanel.filterOptions4) {
                self.searchPanel.filterOptions4.find('select[name=LableID]').val('');
            }
            self.searchPanel.filterOptions5.find('select[name=DocStatusID]').val('');
            
            $('.filter-isInput').css('background-color', '').find('input').val('');
            $('.filter-isInput').find('span').addClass('dn');
            
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

    window.AdminDocuments = adminDocuments;
})(window, jQuery);