(function (window, undefined) {

    var document = window.document,
        navigator = window.navigator,
        location = window.location,
        appSettings = window.appSettings;
    var documentBlank = function (options) {
        var self = this;

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


        this.blank = null,
        this.form = null,
        this.dialog = null,
        this.fields = {},
        this.buttons = {},
        this.actionPanel = null,

        this.createForm = function (docData, type) {
            this.dispose();

            var isMiniMode = ($(window).height() < 750);

            var configWindow = function (isInput) {
                if (isInput) {
                    $(self.fields.Document.ExternalSource.Organization.parents('tr.blank-row')).find('td.blank-row-title').text('Звідки надійшов документ:');
                    $(self.fields.Document.Number.parents('tr.blank-row')).find('td.blank-row-title').text('Вхідний номер:');
                    $(self.fields.Document.CreationDate.parents('tr.blank-row')).find('td.blank-row-title').text('Дата надходження документа:');
                    self.fields.Document.ExternalSource.CreationDate.parents('tr').show();
                    self.fields.Document.ExternalSource.Number.parents('tr').show();
                    self.fields.Head.parents('tr').hide();
                    self.fields.Worker.parents('tr').hide();
                }
                else {
                    $(self.fields.Document.ExternalSource.Organization.parents('tr.blank-row')).find('td.blank-row-title').text('Організація:');
                    $(self.fields.Document.Number.parents('tr.blank-row')).find('td.blank-row-title').text('Номер:');
                    $(self.fields.Document.CreationDate.parents('tr.blank-row')).find('td.blank-row-title').text('Дата створення документа:');
                    self.fields.Document.ExternalSource.CreationDate.parents('tr').hide();
                    self.fields.Document.ExternalSource.Number.parents('tr').hide();
                    self.fields.Head.parents('tr').show();
                    self.fields.Worker.parents('tr').show();
                }
            };
            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            this.fields.ID = docData.ID;
            this.fields.DocumentID = docData.DocumentID;

            this.fields.IsInput = $('<select><option value="1">Вхідний</option><option value="0">Вихідний</option></select>').change(function () {
                configWindow($(this).val() == '1');
            });
            table.append(row('', this.fields.IsInput));
            this.fields.IsInput.val(docData.IsInput ? "1" : "0");


            this.fields.Document = {};
            this.fields.Document.ID = docData.Document.ID;
            this.fields.Document.Files = docData.Document.Files;
            this.fields.Document.ExternalSource = {};
            this.fields.Document.ExternalSource.ID = $('<input type="hidden" value="0">').val(docData.Document.ExternalSource ? docData.Document.ExternalSource.ID : 0);
            this.fields.Document.ExternalSource.Organization = $('<input type="text" style="width: 300px;">').val(docData.Document.ExternalSource ? docData.Document.ExternalSource.OrganizationName : '')
                .attr('valueid', docData.Document.ExternalSource ? docData.Document.ExternalSource.OrganizationID : 0)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
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

            table.append(row('Звідки надійшов документ:', this.fields.Document.ExternalSource.ID
                    .add(this.fields.Document.ExternalSource.Organization)
                    .add(documentUI.createButtonForAutocomplete(this.fields.Document.ExternalSource.Organization))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr("tabIndex", -1)
					.button({ icons: { primary: 'ui-icon-closethick' }, text: false })
					.click(function () {
					    self.fields.Document.ExternalSource.Organization.attr('valueid', 0);
					    self.fields.Document.ExternalSource.Organization.val('');
					}))));

            this.fields.Document.CodeName = $('<input type="text" valueid="0" style="width: 300px;">').val(docData.Document.CodeName ? docData.Document.CodeID + '. ' + docData.Document.CodeName : "")
                .attr('valueid', docData.Document.CodeID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=documentcode&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
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
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                        if (type == 'ins' && docData.Document.Number == '') {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=3&type=getnextnumber&dep=' + documentUI.departmentID + '&code=' + ui.item.id,
                                type: "GET",
                                dataType: "json",
                                success: function (data) {
                                    if (data)
                                        self.fields.Document.Number.val(data);
                                }
                            });
                        }
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
            table.append(row('Шифр:', this.fields.Document.CodeName.add(documentUI.createButtonForAutocomplete(this.fields.Document.CodeName))));


            this.fields.DocType = $('<input type="text" valueid="0" style="width: 300px;">').val(docData.DocType ? docData.DocType.Name : "").attr('valueid', docData.DocTypeID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=doctype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term + '&code=' + self.fields.Document.CodeName.attr('valueid'),
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
            table.append(row('Найменування документа:', this.fields.DocType.add(documentUI.createButtonForAutocomplete(this.fields.DocType))));

            this.fields.Document.ExternalSource.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.Document.ExternalSource && docData.Document.ExternalSource.CreationDate) {
                var externalCreationDate = new Date(+docData.Document.ExternalSource.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                this.fields.Document.ExternalSource.CreationDate.datepicker("setDate", externalCreationDate);
            }
            table.append(row('Дата складання документа:', this.fields.Document.ExternalSource.CreationDate));

            this.fields.Document.ExternalSource.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.ExternalSource ? docData.Document.ExternalSource.Number : '');
            table.append(row('Номер документа в організації:', this.fields.Document.ExternalSource.Number));


            this.fields.Document.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Number);
            table.append(row('Вхідний номер:', this.fields.Document.Number));

            this.fields.Document.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.Document.CreationDate) {
                var creationDate = new Date(+docData.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                this.fields.Document.CreationDate.datepicker("setDate", creationDate);
            }
            table.append(row('Дата надходження документа:', this.fields.Document.CreationDate));


            this.fields.Worker = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.Worker ? documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName) : "")
                .attr('valueid', docData.WorkerID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
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
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
            table.append(row('Підготовив:', this.fields.Worker.add(documentUI.createButtonForAutocomplete(this.fields.Worker))));

            this.fields.Head = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.Head ? documentUI.formatFullName(docData.Head.LastName, docData.Head.FirstName, docData.Head.MiddleName) : "")
                .attr('valueid', docData.HeadID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
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
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
            table.append(row('За підписом:', this.fields.Head.add(documentUI.createButtonForAutocomplete(this.fields.Head))));


            this.fields.IsControlled = docData.IsControlled;
            
            this.fields.IsSpeciallyControlled = docData.IsSpeciallyControlled;

            this.fields.IsIncreasedControlled = $('<input type="checkbox">').attr('checked', docData.IsIncreasedControlled);
            table.append(row('Підвищений контроль:', this.fields.IsIncreasedControlled));

            this.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 54 : 76))
                .attr('rows', (isMiniMode ? '3' : '4'))
                .val(docData.Content);
            table.append(row('Короткий зміст документа:', this.fields.Content));

            this.buttons.buttonCopyText = $('<button style="width: 540px; height:26px;">Копіювати</button>')
            .button({ icons: { primary: "ui-icon-circle-arrow-s", secondary: "ui-icon-circle-arrow-s"} }).click(function (e) {
                self.fields.PublicContent.val(self.fields.Content.val());
                return false;
            });
            table.append(row('', this.buttons.buttonCopyText));

            this.fields.PublicContent = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 54 : 76))
                .attr('rows', (isMiniMode ? '3' : '4'))
                .val(docData.PublicContent);
            table.append(row('Публічний зміст документа:', this.fields.PublicContent));

            this.fields.QuestionType = $('<input type="text" style="width: 300px;">').val(docData.QuestionType ? docData.QuestionType.Name : "").attr('valueid', docData.QuestionTypeID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=questiontype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
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
            table.append(row('Категорія питання:', this.fields.QuestionType.add(documentUI.createButtonForAutocomplete(this.fields.QuestionType))));


            this.fields.IsPublic = $('<input type="checkbox">').attr('checked', docData.IsPublic);
            table.append(row('Публікувати:', this.fields.IsPublic));

            this.fields.NumberCopies = $('<input type="text">').val(docData.NumberCopies);
            table.append(row('Кількість копій:', this.fields.NumberCopies));


            this.fields.Changes = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 16 : 38))
                .attr('rows', (isMiniMode ? '1' : '2'))
                .val(docData.Changes);
            table.append(row('Внесені зміни:', this.fields.Changes));

            this.fields.Document.Notes = $('<textarea style="width:98%; font-size:15px;" cols="81"></textarea>')
                .height((isMiniMode ? 16 : 38))
                .attr('rows', (isMiniMode ? '1' : '2'))
                .val(docData.Document.Notes);
            table.append(row('Особливі відмітки:', this.fields.Document.Notes));

            this.fields.Document.DocStatusID = docData.Document.DocStatusID;

            var uploadButton = $('<div></div>');
            table.append(row('Прикріплені файли:', uploadButton));
            var uploadedList = [];
            for (var i in self.fields.Document.Files)
                uploadedList.push({ id: self.fields.Document.Files[i].FileID, name: self.fields.Document.Files[i].FileName });
            var uploader = new qq.FileUploader({
                element: uploadButton[0],
                action: appSettings.rootUrl + 'Uploader.ashx?documentid=' + self.fields.DocumentID,
                fileUrl: appSettings.rootUrl + 'File.ashx?id=',
                openUrl: appSettings.rootUrl + 'ImageViewer.aspx?documentID=' + self.fields.DocumentID + '&?id=',
                debug: true,
                uploadedList: uploadedList,
                onComplete: function (id, fileName, response) {
                    self.fields.Document.Files.push({ DocumentID: docData.DocumentID, FileID: response.fileID, FileName: fileName });
                },
                onRemove: function (id, file) {
                    if (confirm("Ви дійсно бажаєте видалити файл " + file.name + " ?")) {
                        for (var j in self.fields.Document.Files)
                            if (self.fields.Document.Files[j].FileID == file.id)
                                self.fields.Document.Files.splice(j, 1);

                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=del&dep=' + documentUI.departmentID + '&fileID=' + file.id + '&documentid=' + self.fields.DocumentID,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                return true;
                            },
                            error: function (data) {
                                return true;
                            }
                        });
                        return true;
                    }
                    else
                        return false;
                }
            });

            //var rs = new RemoteScanner(uploadButton);


            //Buttons
            this.buttons.buttonCreate = $('<input type="button" value="Додати">').button().click(function (e) {
                var docObj = {};

                docObj.ID = self.fields.ID;
                docObj.DocumentID = self.fields.DocumentID;
                docObj.IsInput = self.fields.IsInput.val() == '1';
                docObj.Document = {};
                docObj.Document.ID = self.fields.Document.ID;
                docObj.Document.DepartmentID = self.departmentID;
                docObj.Document.Files = self.fields.Document.Files;

                docObj.Document.ExternalSource = {};
                docObj.Document.ExternalSource.ID = parseFloat(self.fields.Document.ExternalSource.ID.val());

                docObj.Document.ExternalSource.OrganizationName = self.fields.Document.ExternalSource.Organization.val();
                if (docObj.Document.ExternalSource.OrganizationName)
                    docObj.Document.ExternalSource.OrganizationID = parseFloat(self.fields.Document.ExternalSource.Organization.attr('valueid'));
                else
                    docObj.Document.ExternalSource.OrganizationID = 0;

                //docObj.Document.ExternalSource.CreationDate = $.datepicker.formatDate('yy-mm-dd', $.datepicker.parseDate('dd.mm.yy', self.fields.Document.ExternalSource.CreationDate.val()));
                docObj.Document.ExternalSource.CreationDate = self.fields.Document.ExternalSource.CreationDate.val();
                docObj.Document.ExternalSource.Number = self.fields.Document.ExternalSource.Number.val();


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

                docObj.DocTypeName = self.fields.DocType.val();
                if (docObj.DocTypeName)
                    docObj.DocTypeID = parseFloat(self.fields.DocType.attr('valueid'));
                else
                    docObj.DocTypeID = 0;

                docObj.Changes = self.fields.Changes.val().replace(/(\r\n|\n|\r)/gm, " ").replace(/\s+/g, " ").trim();

                docObj.Document.Notes = self.fields.Document.Notes.val().replace(/(\r\n|\n|\r)/gm, " ").replace(/\s+/g, " ").trim();
                docObj.Document.Number = self.fields.Document.Number.val();

                var cDateTime = new Date();
                var cTime = '';
                cTime = cTime + (cDateTime.getHours() < 10 ? '0' : '') + cDateTime.getHours();
                cTime = cTime + ':' + (cDateTime.getMinutes() < 10 ? '0' : '') + cDateTime.getMinutes();
                cTime = cTime + ':' + (cDateTime.getSeconds() < 10 ? '0' : '') + cDateTime.getSeconds();
                
                var cDate = '';
                cDate = cDate + (cDateTime.getDay() < 10 ? '0' : '') + cDateTime.getDay();
                cDate = cDate + '.' + ((cDateTime.getMonth() + 1) < 10 ? '0' : '') + (cDateTime.getMonth() + 1);
                cDate = cDate + '.' + cDateTime.getFullYear();

                var crDate = self.fields.Document.CreationDate.val() || cDate;
                docObj.Document.CreationDate = crDate + ' ' + cTime;

                docObj.Document.CodeName = self.fields.Document.CodeName.val();
                if (docObj.Document.CodeName)
                    docObj.Document.CodeID = parseFloat(self.fields.Document.CodeName.attr('valueid'));
                else
                    docObj.Document.CodeID = 0;

                docObj.IsControlled = self.fields.IsControlled;
                docObj.IsSpeciallyControlled = self.fields.IsSpeciallyControlled;
                docObj.IsIncreasedControlled = self.fields.IsIncreasedControlled.is(':checked');

                docObj.IsPublic = self.fields.IsPublic.is(':checked');
                docObj.NumberCopies = $.isNumeric(self.fields.NumberCopies.val()) ? parseFloat(self.fields.NumberCopies.val()) : 0;

                docObj.Content = self.fields.Content.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();
                docObj.PublicContent = self.fields.PublicContent.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                docObj.QuestionTypeName = self.fields.QuestionType.val();
                if (docObj.QuestionTypeName)
                    docObj.QuestionTypeID = parseFloat(self.fields.QuestionType.attr('valueid'));
                else
                    docObj.QuestionTypeID = 0;

                docObj.Document.DocStatusID = self.fields.Document.DocStatusID;

                docObj.Document.TemplateId = 3;

                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=3&type=' + type;

                $.ajax({
                    url: urlRequest,
                    type: "POST",
                    cache: false,
                    data: { 'jdata': JSON.stringify(docObj) },
                    dataType: "json",
                    success: function (msg) {
                        if (self.success instanceof Function)
                            self.success(msg);
                        self.dialog.dialog("close");
                    },
                    error: function (xhr, status, error) {
                        alert(xhr.responseText);
                        //var err = eval("(" + xhr.responseText + ")");
                        //alert('Error: ' + err.Message);
                    }
                });
                return false;
            });

            if (type == 'ins')
                this.buttons.buttonCreate.val('Додати');
            else if (type == 'upd')
                this.buttons.buttonCreate.val('Модифікувати');

            this.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function (e) {
                self.dialog.dialog("close");
            });


            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreate.add(this.buttons.buttonCancel))));
            this.form = $('<div title="Створення документу" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            $('body').append(this.form);

            configWindow(docData.IsInput);

            this.dialog = $(this.form).dialog({
                autoOpen: false,
                draggable: true,
                modal: true,
                position: ["top"],
                resizable: true,
                width: 860,
                close: function (event, ui) {
                    self.dispose();
                },
                open: function (event, ui) {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },

        this.showInsertForm = function () {
            var docObj = {
                ID: 0,
                IsInput: true,
                Document: {
                    ID: 0,
                    DepartmentID: self.departmentID,
                    DocStatusID: 0,
                    Number: '',
                    CreationDate: '',
                    ExternalSource: {
                        ID: 0,
                        OrganizationID: 0,
                        OrganizationName: '',
                        CreationDate: '',
                        Number: ''
                    },
                    CodeID: 0,
                    CodeName: '',
                    Files: [],
                    Notes: ''
                },
                Content: '',
                PublicContent: '',
                Changes: '',
                DocTypeID: 0,
                QuestionTypeID: 0,
                IsControlled: false,
                IsSpeciallyControlled: false,
                IsIncreasedControlled: false,
                IsPublic: true,
                NumberCopies: 0,
                HeadID: 0,
                WorkerID: 0,
                TemplateId: 3
            };

            this.createForm(docObj, 'ins');
            this.dialog.dialog("open");
        },

        this.showUpdateForm = function (documentID) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getblank&jdata=' + documentID;

            $.ajax({
                type: "GET",
                cache: false,
                url: urlRequest,
                dataType: "json",
                success: function (data) {
                    self.createForm(data, 'upd');
                    self.dialog.dialog("open");
                }
            });
        },

        this.showViewForm = function (documentID) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getblank&jdata=' + documentID;

            $.ajax({
                type: "GET",
                cache: false,
                url: urlRequest,
                dataType: "json",
                success: function (data) {
                    self.createForm(data, 'upd');
                    self.dialog.dialog("open");
                }
            });
        },

        this.showDeleteForm = function (documentID, success) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=del&jdata=' + documentID;

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
                                    success: function (data) {
                                        if (success instanceof Function)
                                            success();
                                        deleteDlg.dialog("close");
                                    }
                                });
                            },
                            "Відмінити": function () {
                                $(this).dialog("close");
                            }
                        },
                        close: function (event, ui) {
                            if (deleteDlg)
                                deleteDlg.remove();
                        }
                    });
        },

        this.dispose = function () {
            if (this.form)
                this.form.remove();
            if (this.dialog)
                this.dialog.remove();
        };
    };

    window.documentBlank = documentBlank;

})(window);