(function (window, undefined) {

    var appSettings = window.appSettings;
    var adminDocumentBlank = function (options) {
        var self = this,
            thisDoc = undefined,
            testTime = new Date(1, 0, 1, 0, 0, 0),
            dicts = window.appData.dictionaries;

        this.success = null;
        this.document = {};
        this.departmentID = 0;
        this.templateID = 0;

        if (options) {
            if (options.success)
                this.success = options.success;
            if (options.departmentID)
                this.departmentID = options.departmentID;
            if (options.templateID)
                this.templateID = options.templateID;
        }


        this.blank = null;
        this.form = null;
        this.dialog = null;
        this.fields = {};
        this.buttons = {};
        this.actionPanel = null;

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

        this.createForm = function(docData, type) {
            this.dispose();

            var isMiniMode = ($(window).height() < 750);

            var row = function(name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title"></td>').append(name)).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            if (docData.Document.DepartmentID === userdata.departmentId || docData.Document.DepartmentID === 0) {
                createEditForm();
            }
            else {
                createReadForm();
            }

            function createEditForm() {

                self.fields.ID = docData.ID;
                self.fields.DocumentID = docData.DocumentID;

                self.fields.IsInput = $('<select><option value="1">Вхідний</option><option value="0">Вихідний</option></select>').change(function() {
                    thisDoc.IsInput = ($(this).val() == '1');
                    initForm(thisDoc);
                });
                table.append(row('', self.fields.IsInput));

                self.fields.Document = {};
                self.fields.Document.ID = docData.Document.ID;
                self.fields.Document.Files = docData.Document.Files;

                self.fields.Document.CodeName = $('<input type="text" valueid="0" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=documentcode&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                                type: "GET",
                                dataType: "json",
                                success: function(data) {
                                    response($.map(data, function(item) {
                                        return {
                                            id: parseInt(item[0]),
                                            label: item[0] + '. ' + item[1],
                                            value: item[0] + '. ' + item[1],
                                            option: this
                                        };
                                    }));
                                }
                            });
                        },
                        select: function(event, ui) {
                            $(this).attr('valueid', ui.item.id);
                            thisDoc.Document.CodeID = ui.item.id;
                            thisDoc.DocTypeID = 0;
                            self.fields.DocType.val('').attr('valueid', thisDoc.DocTypeID);
                            initForm(thisDoc);
                            if (type == 'ins' && docData.Document.Number == '') {
                                $.ajax({
                                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=3&type=getnextnumber&dep=' + documentUI.departmentID + '&code=' + ui.item.id,
                                    type: "GET",
                                    dataType: "json",
                                    success: function(data) {
                                        if (data) {
                                            if (thisDoc.IsInput) {
                                                self.fields.Document.Destination.Number.val(data);
                                            }
                                            else {
                                                self.fields.Document.Source.Number.val(data);
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }).addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('Шифр:', self.fields.Document.CodeName.add(documentUI.createButtonForAutocomplete(self.fields.Document.CodeName))));

                self.fields.DocType = $('<input type="text" valueid="0" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=doctype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term + '&code=' + self.fields.Document.CodeName.attr('valueid'),
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
                            thisDoc.DocTypeID = ui.item.id;
                            initForm(thisDoc);
                        }
                    }).addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('Найменування документа:', self.fields.DocType.add(documentUI.createButtonForAutocomplete(self.fields.DocType))));

                self.fields.Document.Source = {};
                self.fields.Document.CorrespondentTitle = $('<span>Кореспондент:</span>');
                table.append(row(self.fields.Document.CorrespondentTitle, '&nbsp;'));

                self.fields.Document.Source.Organization = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function (request, response) {
                            var organizationDepartmentId = -1;
                            if (localStorage['filter_organizations_by_department'] == 'true') {
                                organizationDepartmentId = userdata.departmentId;
                            }
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&orgtype=2&type=search&dep=' + documentUI.departmentID + '&term=' + request.term + '&orgdep=' + organizationDepartmentId,
                                type: 'GET',
                                dataType: 'json',
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
                            thisDoc.Document.Source.OrganizationID = ui.item.id;
                        }
                    }).addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Організація:', self.fields.Document.Source.Organization
                    .add(documentUI.createButtonForAutocomplete(self.fields.Document.Source.Organization))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.fields.Document.Source.Organization.attr('valueid', 0);
                            thisDoc.Document.Source.OrganizationID = 0;
                            self.fields.Document.Source.Organization.val('');
                        }))));

                self.fields.Document.Source.Department = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                                type: 'GET',
                                dataType: 'json',
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
                            thisDoc.Document.Source.DepartmentID = ui.item.id;
                        }
                    }).addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Підрозділ:', self.fields.Document.Source.Department
                    .add(documentUI.createButtonForAutocomplete(self.fields.Document.Source.Department))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.fields.Document.Source.Department.attr('valueid', 0);
                            self.fields.Document.Source.Department.val('');
                            thisDoc.Document.Source.DepartmentID = 0;
                        }))));

                self.fields.Document.Source.Worker = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + userdata.departmentId + '&term=' + request.term,
                                type: 'GET',
                                dataType: 'json',
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
                            thisDoc.Document.Source.WorkerID = ui.item.id;
                        }
                    }).addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Працівник:', self.fields.Document.Source.Worker
                    .add(documentUI.createButtonForAutocomplete(self.fields.Document.Source.Worker))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.fields.Document.Source.Worker.attr('valueid', 0);
                            self.fields.Document.Source.Worker.val('');
                            thisDoc.Document.Source.WorkerID = 0;
                        }))));

                self.fields.Document.Source.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Source.Number);
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Номер:', self.fields.Document.Source.Number));

                self.fields.Document.Source.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Дата створення:', self.fields.Document.Source.CreationDate));


                self.fields.Document.Destination = {};
                self.fields.Document.DestinationTitle = $('<span>Реєстрація:</span>');
                table.append(row(self.fields.Document.DestinationTitle, '&nbsp;'));

                self.fields.Document.Destination.Organization = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function (request, response) {
                            var organizationDepartmentId = -1;
                            if (localStorage['filter_organizations_by_department'] == 'true') {
                                organizationDepartmentId = userdata.departmentId;
                            }
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&orgtype=2&type=search&dep=' + documentUI.departmentID + '&term=' + request.term + '&orgdep=' + organizationDepartmentId,
                                type: 'GET',
                                dataType: 'json',
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
                            thisDoc.Document.Destination.OrganizationID = ui.item.id;
                        }
                    }).addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Організація:', self.fields.Document.Destination.Organization
                    .add(documentUI.createButtonForAutocomplete(self.fields.Document.Destination.Organization))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.fields.Document.Destination.Organization.attr('valueid', 0);
                            thisDoc.Document.Destination.OrganizationID = 0;
                            self.fields.Document.Destination.Organization.val('');
                        }))));

                self.fields.Document.Destination.Department = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                                type: 'GET',
                                dataType: 'json',
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
                            thisDoc.Document.Destination.DepartmentID = ui.item.id;
                        }
                    }).addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Підрозділ:', self.fields.Document.Destination.Department
                    .add(documentUI.createButtonForAutocomplete(self.fields.Document.Destination.Department))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.fields.Document.Destination.Department.attr('valueid', 0);
                            self.fields.Document.Destination.Department.val('');
                            thisDoc.Document.Destination.DepartmentID = 0;
                        }))));

                self.fields.Document.Destination.Worker = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + userdata.departmentId + '&term=' + request.term,
                                type: 'GET',
                                dataType: 'json',
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
                            thisDoc.Document.Destination.WorkerID = ui.item.id;
                        }
                    }).addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Працівник:', self.fields.Document.Destination.Worker
                    .add(documentUI.createButtonForAutocomplete(self.fields.Document.Destination.Worker))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function() {
                            self.fields.Document.Destination.Worker.attr('valueid', 0);
                            self.fields.Document.Destination.Worker.val('');
                            thisDoc.Document.Destination.WorkerID = 0;
                        }))));

                self.fields.Document.Destination.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Destination.Number);
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Вхідний номер:', self.fields.Document.Destination.Number));

                self.fields.Document.Destination.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Дата отримання:', self.fields.Document.Destination.CreationDate));


                self.fields.Worker = $('<input type="text" valueid="0" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                                type: "GET",
                                dataType: "json",
                                success: function(data) {
                                    response($.map(data, function(item) {
                                        return {
                                            id: parseInt(item[0]),
                                            label: documentUI.formatFullName(item[1], item[2], item[3]),
                                            value: documentUI.formatFullName(item[1], item[2], item[3]),
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
                table.append(row('Підготовив:', self.fields.Worker.add(documentUI.createButtonForAutocomplete(self.fields.Worker))));

                self.fields.Head = $('<input type="text" valueid="0" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                                type: "GET",
                                dataType: "json",
                                success: function(data) {
                                    response($.map(data, function(item) {
                                        return {
                                            id: parseInt(item[0]),
                                            label: documentUI.formatFullName(item[1], item[2], item[3]),
                                            value: documentUI.formatFullName(item[1], item[2], item[3]),
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
                table.append(row('За підписом:', self.fields.Head.add(documentUI.createButtonForAutocomplete(self.fields.Head))));

                self.fields.IsIncreasedControlled = $('<input type="checkbox">');
                table.append(row('Підвищений контроль:', self.fields.IsIncreasedControlled));

                self.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                    .height((isMiniMode ? 76 : 96))
                    .attr('rows', (isMiniMode ? '4' : '5'));
                table.append(row('Зміст документа:', self.fields.Content));

                self.buttons.buttonCopyText = $('<button style="width: 540px; height:26px;">Копіювати</button>')
                    .button({ icons: { primary: "ui-icon-circle-arrow-s", secondary: "ui-icon-circle-arrow-s" } }).click(function(e) {
                        self.fields.PublicContent.val(self.fields.Content.val());
                        return false;
                    });
                table.append(row('', self.buttons.buttonCopyText));

                self.fields.PublicContent = $('<textarea style="width:98%;" cols="81"></textarea>')
                    .height((isMiniMode ? 54 : 76))
                    .attr('rows', (isMiniMode ? '3' : '4'));
                table.append(row('Публічний зміст документа:', self.fields.PublicContent));

                var depLabels = getLabels();
                if (depLabels.length || thisDoc.Document.Labels.length) {
                    var triggerLabels = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                    var labelsWrap = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute; z-index: 1;" class="branches ui-state-default"></div>');
                    var labelsTable = $('<table></table>');
                    depLabels.forEach(function (depLabel) {
                        labelsTable.append('<tr><td><label><input type="checkbox" value="' + depLabel.id + '" ' + ($.inArray(depLabel.id, thisDoc.Document.Labels) > -1 ? 'checked="checked"' : '') + '/>' + depLabel.name + '</label></td></tr>');
                    });
                    thisDoc.Document.Labels.forEach(function (labelId) {
                        var label = getLabel(labelId);
                        if (label.departmentId !== self.departmentID) {
                            labelsTable.append('<tr><td><label><input type="checkbox" value="' + label.id + '" checked="checked" disabled="disabled"/>' + label.name + '</label></td></tr>');
                        }
                    });
                    labelsWrap.append(labelsTable);
                    var labelsContainer = $('<div></div>')
                        .append($('<div></div>').append(triggerLabels)
                            .click(function () {
                                if (labelsWrap.is(':visible')) {
                                    triggerLabels.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all");
                                    triggerLabels.find('span').removeClass("ui-icon-circle-arrow-s").addClass("ui-icon-circle-arrow-e");
                                    triggerLabels.find('a').text('Розгорнути');
                                    labelsWrap.hide();
                                }
                                else {
                                    triggerLabels.removeClass("ui-state-default ui-corner-all").addClass("ui-state-active ui-corner-top");
                                    triggerLabels.find('span').removeClass("ui-icon-circle-arrow-e").addClass("ui-icon-circle-arrow-s");
                                    triggerLabels.find('a').text('Згорнути');
                                    labelsWrap.show();
                                }
                            }))
                        .append(labelsWrap);
                    self.fields.Labels = labelsWrap;
                    table.append(row('Вибіркові мітки:', labelsContainer));
                }

                self.fields.QuestionType = $('<input type="text" style="width: 340px;">')
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function(request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=questiontype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
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
                table.append(row('Категорія питання:', self.fields.QuestionType.add(documentUI.createButtonForAutocomplete(self.fields.QuestionType))));

                self.fields.IsPublic = $('<input type="checkbox">');
                table.append(row('Публікувати:', self.fields.IsPublic));

                self.fields.NumberCopies = $('<input type="text">');
                table.append(row('Кількість копій:', self.fields.NumberCopies));


                self.fields.Changes = $('<textarea style="width:98%;" cols="81"></textarea>')
                    .height((isMiniMode ? 19 : 38))
                    .attr('rows', (isMiniMode ? '1' : '2'));
                table.append(row('Внесені зміни:', self.fields.Changes));

                self.fields.Document.Notes = $('<textarea style="width:98%; font-size:15px;" cols="81"></textarea>')
                    .height((isMiniMode ? 19 : 38))
                    .attr('rows', (isMiniMode ? '1' : '2'));
                table.append(row('Особливі відмітки:', self.fields.Document.Notes));


                self.fields.uploadButton = $('<div></div>');
                table.append(row('Прикріплені файли:', self.fields.uploadButton));
                recreateFileUploader();

                //Buttons
                self.buttons.buttonCreate = $('<input type="button" value="Додати">').button().click(function(e) {
                    sendDocData({ close: true });
                    return false;
                });

                if (type == 'ins')
                    self.buttons.buttonCreate.val('Додати');
                else if (type == 'upd')
                    self.buttons.buttonCreate.val('Модифікувати');

                self.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function(e) {
                    self.dialog.dialog("close");
                });


                self.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
                self.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(self.buttons.buttonCreate.add(self.buttons.buttonCancel))));
                self.commentsWrapper = $('<div style="" class="form-font-big comments-wrapper"></div>');
                var collappsePanel = $('<td class="collapsePanelContainer ui-state-default ui-corner-all" align="center" valign="top" style="width: 220px; display: none;"></td>').append(self.commentsWrapper);
                var collapseBtn = $('<td class="collapseHeaderLine ui-accordion-header ui-helper-reset ui-state-active ui-corner-top" valign="middle" style="width: 16px; cursor:pointer;"><span class="ui-icon ui-icon-circle-arrow-w" style="float:left;"></span></td>')
                    .click(function() {
                        if (collappsePanel.is(':visible')) {
                            collappsePanel.hide();
                        } else {
                            collappsePanel.show();
                        }
                    });
                self.blankWrapper = $('<table style="width:100%;"></table>').append($('<tr></tr>')
                    .append($('<td align="center"></td>').append(self.blank).append(self.actionPanel))
                    .append(collappsePanel)
                    .append(collapseBtn)
                );

                self.comments = window.formParts.commentsBlock({
                    appendTo: self.commentsWrapper,
                    DocumentID: docData.DocumentID
                });
                self.form = $('<div title="Створення документу" style="display:none;"></div>').append(self.blankWrapper);
                $('body').append(self.form);

            }

            function createReadForm() {

                self.fields.ID = docData.ID;
                self.fields.DocumentID = docData.DocumentID;

                self.fields.IsInput = $('<select><option value="1">Вхідний</option><option value="0">Вихідний</option></select>').attr('disabled', 'disabled');
                table.append(row('', self.fields.IsInput));

                self.fields.Document = {};
                self.fields.Document.ID = docData.Document.ID;
                self.fields.Document.Files = docData.Document.Files;

                self.fields.Document.CodeName = $('<input type="text" valueid="0" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('Шифр:', self.fields.Document.CodeName));

                self.fields.DocType = $('<input type="text" valueid="0" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('Найменування документа:', self.fields.DocType));

                self.fields.Document.Source = {};
                self.fields.Document.CorrespondentTitle = $('<span>Кореспондент:</span>');
                table.append(row(self.fields.Document.CorrespondentTitle, '&nbsp;'));

                self.fields.Document.Source.Organization = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Організація:', self.fields.Document.Source.Organization));

                self.fields.Document.Source.Department = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Підрозділ:', self.fields.Document.Source.Department));

                self.fields.Document.Source.Worker = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Працівник:', self.fields.Document.Source.Worker));

                self.fields.Document.Source.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Source.Number).attr('disabled', 'disabled');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Номер в адресанта:', self.fields.Document.Source.Number));

                self.fields.Document.Source.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Дата створення:', self.fields.Document.Source.CreationDate));


                self.fields.Document.Destination = {};
                self.fields.Document.DestinationTitle = $('<span>Реєстрація:</span>');
                table.append(row(self.fields.Document.DestinationTitle, '&nbsp;'));

                self.fields.Document.Destination.Organization = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Організація:', self.fields.Document.Destination.Organization));

                self.fields.Document.Destination.Department = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Підрозділ:', self.fields.Document.Destination.Department));

                self.fields.Document.Destination.Worker = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass('ui-widget ui-widget-content ui-corner-left');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Працівник:', self.fields.Document.Destination.Worker));

                self.fields.Document.Destination.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Destination.Number).attr('disabled', 'disabled');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Номер в адресата:', self.fields.Document.Destination.Number));

                self.fields.Document.Destination.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Дата отримання:', self.fields.Document.Destination.CreationDate));


                self.fields.Worker = $('<input type="text" valueid="0" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('Підготовив:', self.fields.Worker));

                self.fields.Head = $('<input type="text" valueid="0" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('За підписом:', self.fields.Head));

                self.fields.IsIncreasedControlled = $('<input type="checkbox">').attr('disabled', 'disabled');
                table.append(row('Підвищений контроль:', self.fields.IsIncreasedControlled));

                self.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>').attr('disabled', 'disabled')
                    .height((isMiniMode ? 76 : 96))
                    .attr('rows', (isMiniMode ? '4' : '5'));
                table.append(row('Зміст документа:', self.fields.Content));

                self.fields.PublicContent = $('<textarea style="width:98%;" cols="81"></textarea>').attr('disabled', 'disabled')
                    .height((isMiniMode ? 54 : 76))
                    .attr('rows', (isMiniMode ? '3' : '4'));
                table.append(row('Публічний зміст документа:', self.fields.PublicContent));

                var depLabels = getLabels();
                if (depLabels.length || thisDoc.Document.Labels.length) {
                    var triggerLabels = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                    var labelsWrap = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute; z-index: 1;" class="branches ui-state-default"></div>');
                    var labelsTable = $('<table></table>');
                    depLabels.forEach(function (depLabel) {
                        labelsTable.append('<tr><td><label><input type="checkbox" value="' + depLabel.id + '" ' + ($.inArray(depLabel.id, thisDoc.Document.Labels) > -1 ? 'checked="checked"' : '') + '/>' + depLabel.name + '</label></td></tr>');
                    });
                    thisDoc.Document.Labels.forEach(function (labelId) {
                        var label = getLabel(labelId);
                        if (label.departmentId !== self.departmentID) {
                            labelsTable.append('<tr><td><label><input type="checkbox" value="' + label.id + '" checked="checked" disabled="disabled"/>' + label.name + '</label></td></tr>');
                        }
                    });
                    labelsWrap.append(labelsTable);
                    var labelsContainer = $('<div></div>')
                        .append($('<div></div>').append(triggerLabels)
                            .click(function() {
                                if (labelsWrap.is(':visible')) {
                                    triggerLabels.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all");
                                    triggerLabels.find('span').removeClass("ui-icon-circle-arrow-s").addClass("ui-icon-circle-arrow-e");
                                    triggerLabels.find('a').text('Розгорнути');
                                    labelsWrap.hide();
                                }
                                else {
                                    triggerLabels.removeClass("ui-state-default ui-corner-all").addClass("ui-state-active ui-corner-top");
                                    triggerLabels.find('span').removeClass("ui-icon-circle-arrow-e").addClass("ui-icon-circle-arrow-s");
                                    triggerLabels.find('a').text('Згорнути');
                                    labelsWrap.show();
                                }
                            }))
                        .append(labelsWrap);
                    
                    labelsWrap.append($('<input type="button" value="Зберегти" style="float: right;">').button().click(function (e) {
                            var that = $(this);
                            that.css('background-image', '').css('background-color', '');
                            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=addlbl' + '&dep=' + userdata.departmentId + '&documentId=' + docData.DocumentID;
                            var checkedLabels = [];
                            if (self.fields.Labels) {
                                self.fields.Labels.find('input:checked:enabled').each(function () {
                                    var lbl = getLabel(parseInt($(this).val()));
                                    checkedLabels.push({
                                        ID: lbl.id,
                                        Name: lbl.name,
                                        DepartmentID: lbl.departmentId,
                                        WorkerID: 0
                                    });
                                });
                            }

                            $.ajax({
                                url: urlRequest,
                                type: "POST",
                                cache: false,
                                data: { 'jdata': JSON.stringify(checkedLabels) },
                                dataType: "json",
                                success: function (msg) {
                                    that.css('background-image', 'none').css('background-color', 'lightgreen');
                                },
                                error: function (xhr, status, error) {
                                    alert(xhr.responseText);
                                }
                            });
                        }));
                    self.fields.Labels = labelsWrap;
                    table.append(row('Вибіркові мітки:', labelsContainer));
                }

                self.fields.QuestionType = $('<input type="text" style="width: 340px;">').attr('disabled', 'disabled')
                    .addClass("ui-widget ui-widget-content ui-corner-left");
                table.append(row('Категорія питання:', self.fields.QuestionType));


                self.fields.IsPublic = $('<input type="checkbox">').attr('disabled', 'disabled');
                table.append(row('Публікувати:', self.fields.IsPublic));

                self.fields.NumberCopies = $('<input type="text">').attr('disabled', 'disabled');
                table.append(row('Кількість копій:', self.fields.NumberCopies));


                self.fields.Changes = $('<textarea style="width:98%;" cols="81"></textarea>').attr('disabled', 'disabled')
                    .height((isMiniMode ? 19 : 38))
                    .attr('rows', (isMiniMode ? '1' : '2'));
                table.append(row('Внесені зміни:', self.fields.Changes));

                self.fields.Document.Notes = $('<textarea style="width:98%; font-size:15px;" cols="81"></textarea>').attr('disabled', 'disabled')
                    .height((isMiniMode ? 19 : 38))
                    .attr('rows', (isMiniMode ? '1' : '2'));
                table.append(row('Особливі відмітки:', self.fields.Document.Notes));


                self.fields.uploadButton = $('<div></div>');
                table.append(row('Прикріплені файли:', self.fields.uploadButton));
                recreateFileUploader();


                var cards = docData.ControlCards,
                    parentControlCardId = 0,
                    lastChildrenControlCardNumber = 0,
                    headId = userdata.worker.ID,
                    hasBaseCard = false,
                    accessCard = null;

                cards.forEach(function(rCard) {
                    //if (cards.length > 0) {
                    var/*rCard = cards[0],*/
                        head = rCard.Head,
                        worker = rCard.Worker,
                        fixedWorker = rCard.FixedWorker,
                        isBaseCard = false,
                        foreignCard = (userdata.departmentId !== rCard.ExecutiveDepartmentID);

                    if (!hasBaseCard && !foreignCard) {
                        hasBaseCard = true;
                        isBaseCard = true;
                        accessCard = rCard;
                    }

                    parentControlCardId = rCard.ID;
                    headId = rCard.HeadID;
                    head = (head && rCard.HeadID > 0) ? documentUI.formatFullName(head.LastName, head.FirstName, head.MiddleName) : '';
                    worker = (worker && rCard.WorkerID > 0) ? documentUI.formatFullName(worker.LastName, worker.FirstName, worker.MiddleName) : '';
                    fixedWorker = (fixedWorker && rCard.FixedWorkerID > 0) ? documentUI.formatFullName(fixedWorker.LastName, fixedWorker.FirstName, fixedWorker.MiddleName) : '';

                    //table.append(row('&nbsp;'));
                    //table.append(row(rCard.CardNumber + '. Виконавець:', worker + '&nbsp;&nbsp;&nbsp;&nbsp; Закріплена за: ' + fixedWorker));
                    //table.append(row('&nbsp; Резолюція:', rCard.Resolution));


                    //var cardStatus = rCard.CardStatus ? rCard.CardStatus.Name : '';

                    //var endDate = '';
                    //if (rCard.EndDate) {
                    //    endDate = new Date(+rCard.EndDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    //    if (endDate > testTime) {
                    //        endDate = endDate;
                    //    }
                    //    else {
                    //        endDate = '';
                    //    }
                    //}
                    //var cardRow = $('<div> Стан: <span class="card-status-' + rCard.CardStatusID + '">' + cardStatus + '</span> </div>')
                    //    .prepend($('<input class="card-enddate' + (rCard.CardStatusID != 1 ? ' card-close' : '') + '" type="text" disabled="disabled">').datepicker({ changeMonth: true, changeYear: true }).datepicker("setDate", endDate))
                    //    .prepend(' Термін: ');
                    var cardRow = window.formParts.cardBlock({ card: rCard, cssClass: (foreignCard ? 'bg-gray' : ''), resolutionVisibility: !foreignCard, documentBlank: self });
                    table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>').append(cardRow)));
                    //table.append(row('&nbsp; ' + head, cardRow));

                    //table.append(row('&nbsp;'));

                    //if (isBaseCard) {
                    //    self.fields.InnerNumber = $('<input type="text" style="width: 200px;">').val(rCard.InnerNumber)
                    //        .add($('<input type="button" value="Зберегти">').button().click(function(e) {
                    //            var that = $(this);
                    //            that.css('background-image', '').css('background-color', '');
                    //            rCard.InnerNumber = self.fields.InnerNumber.val().trim();

                    //            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                    //            /*
                    //            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=innernumber' +
                    //                '&dep=' + userdata.departmentId + '&dep=' + userdata.departmentId +'&cardid=' + rCard.ID;
                    //            */
                    //            $.ajax({
                    //                url: urlRequest,
                    //                type: "POST",
                    //                cache: false,
                    //                data: { 'jdata': JSON.stringify(rCard) },
                    //                /*data: { 'jdata': rCard.InnerNumber },*/
                    //                dataType: "json",
                    //                success: function(msg) {
                    //                    that.css('background-image', 'none').css('background-color', 'lightgreen');
                    //                    if (self.success instanceof Function) {
                    //                        self.success(msg);
                    //                    }
                    //                },
                    //                error: function(xhr, status, error) {
                    //                    alert(xhr.responseText);
                    //                }
                    //            });
                    //        }));
                    //    table.append(row('Реєстраційний номер:', self.fields.InnerNumber));
                    //}

                    //self.buttons.buttonResponse = $('<input type="button" value="Відповісти">').button().click(function(e) {
                    //    //rCard.ControlResponseDate = self.fields.ControlResponseDate.val();
                    //    rCard.InnerNumber = self.fields.InnerNumber.val().trim();
                    //    rCard.ControlResponse = self.fields.ControlResponse.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                    //    var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                    //    $.ajax({
                    //        url: urlRequest,
                    //        type: "POST",
                    //        cache: false,
                    //        data: { 'jdata': JSON.stringify(rCard) },
                    //        dataType: "json",
                    //        success: function(msg) {
                    //            self.dialog.dialog("close");
                    //            /*if (ops.onCancel instanceof Function) {
                    //                ops.onCancel();
                    //            }*/
                    //        },
                    //        error: function(xhr, status, error) {
                    //            alert(xhr.responseText);
                    //        }
                    //    });
                    //});
                    //self.fields.ControlResponse = $('<textarea style="height:57px;width:98%;font-size:15px;" cols="81" rows="3" ' + (foreignCard ? 'disabled="disabled"' : '') + '></textarea>')
                    //    .val(rCard.ControlResponse);
                    //table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    //    .append('Контрольна відповідь:').append(self.fields.ControlResponse.add(self.buttons.buttonResponse))));


                    var ccc = rCard.ChildrenControlCards;
                    if (ccc.length > 0) {
                        table.append(row('&nbsp;&nbsp;&nbsp;&nbsp; Виконавці:'));
                        lastChildrenControlCardNumber = ccc[ccc.length - 1].CardNumber;
                        for (var v = 0; v < ccc.length; v++) {
                            var c = ccc[v];
                            var cWorker = c.WorkerID > 0 ? documentUI.formatFullName(c.Worker.LastName, c.Worker.FirstName, c.Worker.MiddleName) : '';
                            createWorkerRow({
                                ID: c.ID,
                                CardNumber: c.CardNumber,
                                ControlResponse: c.ControlResponse,
                                ControlResponseDate: c.ControlResponseDate,
                                worker: cWorker
                            });
                        }
                    }
                });

                if (accessCard) {
                    self.fields.InnerNumber = $('<input type="text" style="width: 200px;">').val(accessCard.InnerNumber)
                        .add($('<input type="button" value="Зберегти">').button().click(function (e) {
                            var that = $(this);
                            that.css('background-image', '').css('background-color', '');
                            accessCard.InnerNumber = self.fields.InnerNumber.val().trim();
                            /*
                            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                            */
                            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=innernumber' +
                                '&dep=' + userdata.departmentId + '&cardid=' + accessCard.ID;
                            
                            $.ajax({
                                url: urlRequest,
                                type: "POST",
                                cache: false,
                                /*data: { 'jdata': JSON.stringify(accessCard) },*/
                                data: { 'jdata': accessCard.InnerNumber },
                                dataType: "json",
                                success: function (msg) {
                                    that.css('background-image', 'none').css('background-color', 'lightgreen');
                                    if (self.success instanceof Function) {
                                        self.success(msg);
                                    }
                                },
                                error: function (xhr, status, error) {
                                    alert(xhr.responseText);
                                }
                            });
                        }));
                    table.append(row('Реєстраційний номер:', self.fields.InnerNumber));

                    self.buttons.buttonResponse = $('<input type="button" value="Відповісти">').button().click(function (e) {
                        //rCard.ControlResponseDate = self.fields.ControlResponseDate.val();
                        accessCard.InnerNumber = self.fields.InnerNumber.val().trim();
                        accessCard.ControlResponse = self.fields.ControlResponse.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                        var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                        $.ajax({
                            url: urlRequest,
                            type: "POST",
                            cache: false,
                            data: { 'jdata': JSON.stringify(accessCard) },
                            dataType: "json",
                            success: function (msg) {
                                self.dialog.dialog("close");
                                /*if (ops.onCancel instanceof Function) {
                                    ops.onCancel();
                                }*/
                            },
                            error: function (xhr, status, error) {
                                alert(xhr.responseText);
                            }
                        });
                    });
                    var foreignAccessCard = (userdata.departmentId !== accessCard.ExecutiveDepartmentID);
                    self.fields.ControlResponse = $('<textarea style="height:57px;width:98%;font-size:15px;" cols="81" rows="3" ' + (foreignAccessCard ? 'disabled="disabled"' : '') + '></textarea>')
                        .val(accessCard.ControlResponse);
                    table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                        .append('Контрольна відповідь:').append(self.fields.ControlResponse.add(self.buttons.buttonResponse))));

                }

                function createWorkerRow(p) {
                    var resp = $('<span></span>');
                    var rDate = new Date(+p.ControlResponseDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    if (p.ControlResponse || rDate > testTime) {
                        resp.append('Відповідь ');
                        if (rDate > testTime) {
                            var controlResponseDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                            controlResponseDate.datepicker("setDate", rDate);
                            resp.append(controlResponseDate);
                        }
                        if (p.ControlResponse) {
                            resp.append('<br>').append($('<span></span>').text(p.ControlResponse));
                        }
                    }
                    /*
                    $('<button type="button" title="Видалити" style="margin-left: 20px;"></button>').attr('tabIndex', -1).attr('controlCardId', p.ID)
                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                        .click(function () {
                            $.ajax({
                                type: "GET",
                                cache: false,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=del&jdata=' + $(this).attr('controlCardId') + '&dep=' + userdata.departmentId,
                                dataType: "json",
                                success: function (data) {
                                    resp.parents('tr').remove();
                                }
                            });
                        }).appendTo(resp);
                    */
                    table.append(row('&nbsp;&nbsp;&nbsp;&nbsp; ' + p.CardNumber + '. ' + p.worker, resp));
                }

                //Buttons
                self.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function(e) {
                    self.dialog.dialog("close");
                });

                self.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
                self.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(self.buttons.buttonCancel)));
                self.commentsWrapper = $('<div style="" class="form-font-big comments-wrapper"></div>');
                var collappsePanel = $('<td class="collapsePanelContainer ui-state-default ui-corner-all" align="center" valign="top" style="width: 220px; display: none;"></td>').append(self.commentsWrapper);
                var collapseBtn = $('<td class="collapseHeaderLine ui-accordion-header ui-helper-reset ui-state-active ui-corner-top" valign="middle" style="width: 16px; cursor:pointer;"><span class="ui-icon ui-icon-circle-arrow-w" style="float:left;"></span></td>')
                    .click(function () {
                        if (collappsePanel.is(':visible')) {
                            collappsePanel.hide();
                        } else {
                            collappsePanel.show();
                        }
                    });
                self.blankWrapper = $('<table style="width:100%;"></table>').append($('<tr></tr>')
                    .append($('<td align="center"></td>').append(self.blank).append(self.actionPanel))
                    .append(collappsePanel)
                    .append(collapseBtn)
                );
                self.comments = window.formParts.commentsBlock({
                    appendTo: self.commentsWrapper,
                    DocumentID: docData.DocumentID,
                    ControlCardID: accessCard ? accessCard.ID : 0
                });
                self.form = $('<div title="Документ" style="display:none;"></div>').append(self.blankWrapper);
                $('body').append(self.form);
            }

            self.dialog = $(self.form).dialog({
                autoOpen: false,
                draggable: true,
                modal: true,
                position: ["top"],
                resizable: true,
                width: 960,
                close: function(event, ui) {
                    self.dispose();
                },
                open: function(event, ui) {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
            setFields(docData);
            initForm(docData);


            function recreateFileUploader() {
                var uploadButton = $('<div></div>');
                self.fields.uploadButton.replaceWith(uploadButton);
                self.fields.uploadButton = uploadButton;

                var uploadedList = [];
                for (var i in self.fields.Document.Files) {
                    var f = self.fields.Document.Files[i];
                    uploadedList.push({
                        id: f.FileID,
                        name: f.FileName,
                        removable: f.DepartmentID === userdata.departmentId
                    });
                }
                var uploader = new qq.FileUploader({
                    element: self.fields.uploadButton[0],
                    action: appSettings.rootUrl + 'Uploader.ashx?documentid=' + thisDoc.DocumentID,
                    fileUrl: appSettings.rootUrl + 'File.ashx?id=',
                    openUrl: appSettings.rootUrl + 'ImageViewer.aspx?documentID=' + thisDoc.DocumentID + '&?id=',
                    debug: true,
                    uploadedList: uploadedList,
                    onComplete: function(id, fileName, response) {
                        self.fields.Document.Files.push({ DocumentID: thisDoc.DocumentID, FileID: response.fileID, FileName: fileName });
                    },
                    onRemove: function(id, file) {
                        if (confirm('Ви дійсно бажаєте видалити файл ' + file.name + ' ?')) {

                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=del&dep=' + documentUI.departmentID + '&fileID=' + file.id + '&documentid=' + self.fields.DocumentID,
                                type: 'GET',
                                dataType: 'json',
                                success: function() {
                                    for (var j in self.fields.Document.Files)
                                        if (self.fields.Document.Files[j].FileID == file.id)
                                            self.fields.Document.Files.splice(j, 1);
                                },
                                error: function(responce) {
                                    window.alert(responce.responseText);
                                }
                            });
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                });
            }

            function initForm(formData) {
                self.fields.Head.parents('tr').hide();
                self.fields.Worker.parents('tr').hide();

                self.fields.Document.Source.Organization.parents('tr').hide();
                self.fields.Document.Source.Department.parents('tr').hide();
                self.fields.Document.Source.Worker.parents('tr').hide();
                self.fields.Document.Destination.Organization.parents('tr').hide();
                self.fields.Document.Destination.Department.parents('tr').hide();
                self.fields.Document.Destination.Worker.parents('tr').hide();

                if (formData.IsInput) {
                    self.fields.Document.CorrespondentTitle.text('Кореспондент:');
                    self.fields.Document.DestinationTitle.text('Реєстрація:');
                    if (self.fields.Document.Source.Department.attr('valueid') === userdata.departmentId.toString()) {
                        self.fields.Document.Source.Department.val('').attr('valueid', 0);
                    }
                    if (!self.fields.Document.Destination.Department.val()) {
                        self.fields.Document.Destination.Department.val(userdata.departmentName).attr('valueid', userdata.departmentId);
                    }

                    self.fields.Document.Destination.Number.parents('tr').show();
                    self.fields.Document.Destination.CreationDate.parents('tr').show();
                }
                else {
                    self.fields.Document.CorrespondentTitle.text('Реєстрація:');
                    self.fields.Document.DestinationTitle.text('Кореспондент:');
                    self.fields.Head.parents('tr').show();
                    self.fields.Worker.parents('tr').show();

                    if (!self.fields.Document.Source.Department.val()) {
                        self.fields.Document.Source.Department.val(userdata.departmentName).attr('valueid', userdata.departmentId);
                    }
                    if (self.fields.Document.Destination.Department.attr('valueid') === userdata.departmentId.toString()) {
                        self.fields.Document.Destination.Department.val('').attr('valueid', 0);
                    }

                    self.fields.Document.Destination.Number.parents('tr').hide();
                    self.fields.Document.Destination.CreationDate.parents('tr').hide();
                }
                /*
                if (formData.Document.CodeID === 12) {
                    if (formData.IsInput) {
                        self.fields.Document.Source.Department.parents('tr').show();
                    } else {
                        self.fields.Document.Destination.Department.parents('tr').show();
                    }
                    
                    if (formData.DocTypeID === 6 || formData.DocTypeID === 16) {
                        if (formData.IsInput) {
                            self.fields.Document.Source.Worker.parents('tr').show();
                        } else {
                            self.fields.Document.Destination.Worker.parents('tr').show();
                        }
                    }
                } else {
                */
                if (formData.IsInput) {
                    self.fields.Document.Source.Organization.parents('tr').show();
                }
                else {
                    self.fields.Document.Destination.Organization.parents('tr').show();

                }
                //}
            }

            function getDocData() {
                var docObj = thisDoc;

                var cDateTime = new Date();
                var cTime = '';
                cTime = cTime + (cDateTime.getHours() < 10 ? '0' : '') + cDateTime.getHours();
                cTime = cTime + ':' + (cDateTime.getMinutes() < 10 ? '0' : '') + cDateTime.getMinutes();
                cTime = cTime + ':' + (cDateTime.getSeconds() < 10 ? '0' : '') + cDateTime.getSeconds();

                var cDate = '';
                cDate = cDate + (cDateTime.getDay() < 10 ? '0' : '') + cDateTime.getDay();
                cDate = cDate + '.' + ((cDateTime.getMonth() + 1) < 10 ? '0' : '') + (cDateTime.getMonth() + 1);
                cDate = cDate + '.' + cDateTime.getFullYear();


                docObj.DocTypeName = self.fields.DocType.val();
                if (docObj.DocTypeName)
                    docObj.DocTypeID = parseFloat(self.fields.DocType.attr('valueid'));
                else
                    docObj.DocTypeID = 0;

                docObj.Document.CodeName = self.fields.Document.CodeName.val();
                if (docObj.Document.CodeName)
                    docObj.Document.CodeID = parseFloat(self.fields.Document.CodeName.attr('valueid'));
                else
                    docObj.Document.CodeID = 0;

                docObj.IsInput = self.fields.IsInput.val() == '1';
                docObj.Document.Files = self.fields.Document.Files;

                docObj.Document.Source.OrganizationName = self.fields.Document.Source.Organization.val();
                if (docObj.Document.Source.OrganizationName)
                    docObj.Document.Source.OrganizationID = parseFloat(self.fields.Document.Source.Organization.attr('valueid'));
                else
                    docObj.Document.Source.OrganizationID = 0;

                docObj.Document.Source.DepartmentName = self.fields.Document.Source.Department.val();
                if (docObj.Document.Source.DepartmentName)
                    docObj.Document.Source.DepartmentID = parseFloat(self.fields.Document.Source.Department.attr('valueid'));
                else
                    docObj.Document.Source.DepartmentID = 0;

                docObj.Document.Source.WorkerName = self.fields.Document.Source.Worker.val();
                if (docObj.Document.Source.WorkerName)
                    docObj.Document.Source.WorkerID = parseFloat(self.fields.Document.Source.Worker.attr('valueid'));
                else
                    docObj.Document.Source.WorkerID = 0;
                docData.Document.Source.Number = self.fields.Document.Source.Number.val();

                var sDate = self.fields.Document.Source.CreationDate.val() || cDate;
                if (docObj.IsInput) {
                    /*
                    if (docObj.Document.CodeID === 12) {
                        if (!docData.Document.Source.CreationDate) {
                            docObj.Document.Source.CreationDate = sDate + ' ' + cTime;
                        }
                    } else {
                    */
                    docObj.Document.Source.CreationDate = sDate + ' ' + cTime;
                    //}
                }
                else {
                    docObj.Document.Source.CreationDate = sDate + ' ' + cTime;
                }


                docObj.Document.Destination.OrganizationName = self.fields.Document.Destination.Organization.val();
                if (docObj.Document.Destination.OrganizationName)
                    docObj.Document.Destination.OrganizationID = parseFloat(self.fields.Document.Destination.Organization.attr('valueid'));
                else
                    docObj.Document.Destination.OrganizationID = 0;

                docObj.Document.Destination.DepartmentName = self.fields.Document.Destination.Department.val();
                if (docObj.Document.Destination.DepartmentName)
                    docObj.Document.Destination.DepartmentID = parseFloat(self.fields.Document.Destination.Department.attr('valueid'));
                else
                    docObj.Document.Destination.DepartmentID = 0;

                docObj.Document.Destination.WorkerName = self.fields.Document.Destination.Worker.val();
                if (docObj.Document.Destination.WorkerName)
                    docObj.Document.Destination.WorkerID = parseFloat(self.fields.Document.Destination.Worker.attr('valueid'));
                else
                    docObj.Document.Destination.WorkerID = 0;
                docData.Document.Destination.Number = self.fields.Document.Destination.Number.val();

                var desDate = self.fields.Document.Destination.CreationDate.val() || cDate;
                if (docObj.IsInput) {
                    docObj.Document.Destination.CreationDate = desDate + ' ' + cTime;
                }
                else {
                    /*
                    if (docObj.Document.CodeID === 12) {
                        if (!docData.Document.Destination.CreationDate) {
                            docObj.Document.Destination.CreationDate = desDate + ' ' + cTime;
                        }
                    }
                    */
                }

                if (docObj.IsInput) {
                    docData.Document.Number = docData.Document.Destination.Number;
                    docData.Document.CreationDate = docData.Document.Destination.CreationDate;
                }
                else {
                    docData.Document.Number = docData.Document.Source.Number;
                    docData.Document.CreationDate = docData.Document.Source.CreationDate;
                }

                docObj.HeadName = self.fields.Head.val();
                if (docObj.HeadName)
                    docObj.HeadID = parseFloat(self.fields.Head.attr('valueid'));
                else
                    docObj.HeadID = 0;

                docObj.WorkerName = self.fields.Worker.val();
                if (docObj.WorkerName)
                    docObj.WorkerID = parseFloat(self.fields.Worker.attr('valueid'));
                else
                    docObj.WorkerID = 0;

                docObj.Changes = self.fields.Changes.val().replace(/(\r\n|\n|\r)/gm, ' ').replace(/\s+/g, ' ').trim();
                docObj.Document.Notes = self.fields.Document.Notes.val().replace(/(\r\n|\n|\r)/gm, ' ').replace(/\s+/g, ' ').trim();

                docObj.IsPublic = self.fields.IsPublic.is(':checked');
                docObj.NumberCopies = $.isNumeric(self.fields.NumberCopies.val()) ? parseFloat(self.fields.NumberCopies.val()) : 0;

                docObj.Content = self.fields.Content.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, ' \n').trim();
                docObj.PublicContent = self.fields.PublicContent.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, ' \n').trim();

                var checkedLabels = [];
                if (self.fields.Labels) {
                    self.fields.Labels.find('input:checked:enabled').each(function () {
                        checkedLabels.push($(this).val());
                    });
                }
                docObj.Document.Labels = checkedLabels;
                
                docObj.QuestionTypeName = self.fields.QuestionType.val();
                if (docObj.QuestionTypeName)
                    docObj.QuestionTypeID = parseFloat(self.fields.QuestionType.attr('valueid'));
                else
                    docObj.QuestionTypeID = 0;

                docObj.IsIncreasedControlled = self.fields.IsIncreasedControlled.is(':checked');

                return docObj;
            }

            function setDocData(data) {
                thisDoc = data;
            }

            function setFields(data) {
                self.fields.Document.Files = data.Document.Files;

                self.fields.IsInput.val(data.IsInput ? "1" : "0");

                self.fields.Document.CodeName
                    .val(data.Document.CodeName ? data.Document.CodeID + '. ' + data.Document.CodeName : "")
                    .attr('valueid', data.Document.CodeID);

                self.fields.DocType.val(data.DocTypeID > 0 ? data.DocType.Name : '').attr('valueid', data.DocTypeID);

                self.fields.Document.Source.Organization
                    .val(data.Document.Source.OrganizationID > 0 ? data.Document.Source.OrganizationName : '')
                    .attr('valueid', data.Document.Source.OrganizationID);
                self.fields.Document.Source.Department
                    .val(data.Document.Source.DepartmentID > 0 ? data.Document.Source.DepartmentName : '')
                    .attr('valueid', data.Document.Source.DepartmentID);

                var sWorker = data.Document.Source.Worker;
                var sWorkerName = (data.Document.Source && data.Document.Source.WorkerID > 0) ?
                    documentUI.formatFullName(sWorker.LastName, sWorker.FirstName, sWorker.MiddleName) : '';
                self.fields.Document.Source.Worker.val(sWorkerName).attr('valueid', data.Document.Source.WorkerID);

                self.fields.Document.Source.Number.val(data.Document.Source.Number);
                if (data.Document.Source.CreationDate) {
                    var sCreationDate = new Date(+data.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    self.fields.Document.Source.CreationDate.datepicker("setDate", sCreationDate);
                }

                self.fields.Document.Destination.Organization
                    .val(data.Document.Destination.OrganizationID > 0 ? data.Document.Destination.OrganizationName : '')
                    .attr('valueid', data.Document.Destination.OrganizationID);
                self.fields.Document.Destination.Department
                    .val(data.Document.Destination.DepartmentID > 0 ? data.Document.Destination.DepartmentName : '')
                    .attr('valueid', data.Document.Destination.DepartmentID);

                var dWorker = data.Document.Destination.Worker;
                var dWorkerName = (data.Document.Destination && data.Document.Destination.WorkerID > 0) ?
                    documentUI.formatFullName(dWorker.LastName, dWorker.FirstName, dWorker.MiddleName) : '';
                self.fields.Document.Destination.Worker.val(dWorkerName).attr('valueid', data.Document.Destination.WorkerID);

                self.fields.Document.Destination.Number.val(data.Document.Destination.Number);
                if (data.Document.Destination.CreationDate) {
                    var dCreationDate = new Date(+data.Document.Destination.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    var cTime = (new Date(1980, 1, 1)).getTime();
                    if (cTime < dCreationDate.getTime()) {
                        self.fields.Document.Destination.CreationDate.datepicker("setDate", dCreationDate);
                    }
                }

                var workerName = (data.WorkerID > 0) ? documentUI.formatFullName(data.Worker.LastName, data.Worker.FirstName, data.Worker.MiddleName) : '';
                self.fields.Worker.val(workerName).attr('valueid', data.WorkerID);

                var headName = (data.HeadID > 0) ? documentUI.formatFullName(data.Head.LastName, data.Head.FirstName, data.Head.MiddleName) : '';
                self.fields.Head.val(headName).attr('valueid', data.HeadID);


                self.fields.Changes.val(data.Changes);
                self.fields.Document.Notes.val(data.Document.Notes);

                self.fields.IsIncreasedControlled.attr('checked', data.IsIncreasedControlled);

                self.fields.IsPublic.prop('checked', data.IsPublic);

                self.fields.Content.val(data.Content);
                self.fields.PublicContent.val(data.PublicContent);

                var depLabels = getLabels();
                if (self.fields.Labels && (depLabels.length || thisDoc.Document.Labels.length)) {
                    self.fields.Labels.find('table').remove();
                    var labelsTable = $('<table></table>');
                    depLabels.forEach(function (depLabel) {
                        labelsTable.append('<tr><td><label><input type="checkbox" value="' + depLabel.id + '" ' + ($.inArray(depLabel.id, thisDoc.Document.Labels) > -1 ? 'checked="checked"' : '') + '/>' + depLabel.name + '</label></td></tr>');
                    });
                    thisDoc.Document.Labels.forEach(function (labelId) {
                        var label = getLabel(labelId);
                        if (label.departmentId !== self.departmentID) {
                            labelsTable.append('<tr><td><label><input type="checkbox" value="' + label.id + '" checked="checked" disabled="disabled"/>' + label.name + '</label></td></tr>');
                        }
                    });
                    self.fields.Labels.prepend(labelsTable);
                }

                self.fields.QuestionType.val(data.QuestionTypeID > 0 ? data.QuestionType.Name : '').attr('valueid', data.QuestionTypeID);

                self.fields.Document.Files = data.Document.Files;
                recreateFileUploader();

                self.fields.NumberCopies.val(data.NumberCopies);
            }

            function sendDocData(p) {
                var docObj = getDocData();
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=3&type=' + type + '&departmentID=' + userdata.departmentId;

                $.ajax({
                    url: urlRequest,
                    type: 'POST',
                    cache: false,
                    data: { 'jdata': JSON.stringify(docObj) },
                    dataType: 'json',
                    success: function(msg) {
                        if (msg.Data) {
                            setDocData(msg.Data);
                            if (!p || !p.close) {
                                setFields(msg.Data);
                            }
                        }
                        if (self.success instanceof Function) {
                            self.success(msg);
                        }
                        if (p && p.close) {
                            self.dialog.dialog('close');
                        }
                    },
                    error: function(xhr) {
                        alert(xhr.responseText);
                    }
                });
                return false;
            }
        };

        this.showInsertForm = function(data) {
            thisDoc = getEmptyDocumentObject();

            if (data) {
                thisDoc = $.extend(true, thisDoc, data);
            }


            this.createForm(thisDoc, 'ins');
            this.dialog.dialog("open");
        };

        this.showUpdateForm = function(documentID) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getadminblank&jdata=' + documentID;

            $.ajax({
                type: "GET",
                cache: false,
                url: urlRequest,
                dataType: "json",
                success: function(data) {
                    thisDoc = data;
                    self.createForm(data, 'upd');
                    self.dialog.dialog("open");
                }
            });
        };

        this.showViewForm = function(documentID) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getadminblank&jdata=' + documentID;

            $.ajax({
                type: "GET",
                cache: false,
                url: urlRequest,
                dataType: "json",
                success: function(data) {
                    thisDoc = data;
                    self.createForm(data, 'upd');
                    self.dialog.dialog("open");
                }
            });
        };

        this.showDeleteForm = function(documentID, success) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=del&jdata=' + documentID;

            var deleteDlg = $('<div title="Видалити документ?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде дійсно видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
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
                                success: function(data) {
                                    if (success instanceof Function)
                                        success();
                                    deleteDlg.dialog("close");
                                }
                            });
                        },
                        "Відмінити": function() {
                            $(this).dialog("close");
                        }
                    },
                    close: function(event, ui) {
                        if (deleteDlg)
                            deleteDlg.remove();
                    }
                });
        };

        this.dispose = function () {
            if (this.form)
                this.form.remove();
            if (this.dialog)
                this.dialog.remove();
        };


        function getEmptyDocumentObject() {
            var docObj = models.DocTemplate();
            docObj.TemplateId = 3;
            docObj.IsPublic = true;
            docObj.Document = models.Document();
            docObj.Document.DepartmentID = self.departmentID;
            docObj.Document.Destination = models.Destination();
            docObj.Document.Source = models.Source();

            return docObj;
        }
    };

    function formatDateTime(date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes;
        return date.getFullYear() + '.' + (date.getMonth() + 1) + '.'  + date.getDate() + '  ' + strTime;
    }

    window.AdminDocumentBlank = adminDocumentBlank;

})(window);