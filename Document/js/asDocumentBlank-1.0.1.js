(function (window, undefined) {

    var document = window.document,
        appSettings = window.appSettings;
    var asDocumentBlank = function (options) {
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
            var testTime = new Date(1, 0, 1, 0, 0, 0);
            var isMiniMode = ($(window).height() < 750);

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            this.fields.ID = docData.ID;
            this.fields.DocumentID = docData.DocumentID;

            this.fields.Document = {};
            this.fields.Document.ID = docData.Document.ID;
            this.fields.Document.Files = docData.Document.Files;

            this.fields.Document.CodeName = $('<input type="text" valueid="0" style="width: 300px;">').val(docData.Document.CodeName ? docData.Document.Code + '. ' + docData.Document.CodeName : "")
                .attr('valueid', docData.Document.CodeID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=documentcode&type=searchcode&dep=0&t=' + self.templateID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[2] + '. ' + item[1],
                                        value: item[2] + '. ' + item[1],
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
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=getnextnumber&dep=' + documentUI.departmentID + '&code=' + ui.item.id,
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

            this.fields.Document.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Number);
            table.append(row('Номер:', this.fields.Document.Number));

            this.fields.Document.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.Document.CreationDate) {
                var creationDate = new Date(+docData.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (creationDate > testTime)
                    this.fields.Document.CreationDate.datepicker("setDate", creationDate);
            }
            table.append(row('Дата реєстрації:', this.fields.Document.CreationDate));
            
            this.fields.ExecutiveDepartment = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.ExecutiveDepartment ? docData.ExecutiveDepartment.Name : "")
                .attr('valueid', docData.ExecutiveDepartmentID)
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
            table.append(row('Виконавче управління:', this.fields.ExecutiveDepartment.add(documentUI.createButtonForAutocomplete(this.fields.ExecutiveDepartment))));

            this.fields.SubjectRequest = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 36 : 54))
                .attr('rows', (isMiniMode ? '2' : '3'))
                .val(docData.SubjectRequest);
            table.append(row("Назва та адреса суб'єкта звернення:", this.fields.SubjectRequest));

            this.fields.ServiceName = $('<input type="text" style="width: 670px;">').val(docData.ServiceName);
            table.append(row('Назва а.п.:', this.fields.ServiceName));

            this.fields.ObjectForService = $('<input type="text" style="width: 670px;">').val(docData.ObjectForService);
            table.append(row("Об'єкт а.п.:", this.fields.ObjectForService));

            this.fields.DateReceivedToDepartment = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.DateReceivedToDepartment) {
                var dateReceivedToDepartment = new Date(+docData.DateReceivedToDepartment.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (dateReceivedToDepartment > testTime)
                    this.fields.DateReceivedToDepartment.datepicker("setDate", dateReceivedToDepartment);
            }
            table.append(row('Дата прийняття на розгляд:', this.fields.DateReceivedToDepartment));
            
            this.fields.ReceivedWorker = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.ReceivedWorker ? documentUI.formatFullName(docData.ReceivedWorker.LastName, docData.ReceivedWorker.FirstName, docData.ReceivedWorker.MiddleName) : "")
                .attr('valueid', docData.ReceivedWorkerID)
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
            table.append(row('Прийняв на розгляд:', this.fields.ReceivedWorker.add(documentUI.createButtonForAutocomplete(this.fields.ReceivedWorker))));

            this.fields.DateReturnFromDepartment = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.DateReturnFromDepartment) {
                var dateReturnFromDepartment = new Date(+docData.DateReturnFromDepartment.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (dateReturnFromDepartment > testTime)
                    this.fields.DateReturnFromDepartment.datepicker("setDate", dateReturnFromDepartment);
            }
            table.append(row('Дата передачі рузультату:', this.fields.DateReturnFromDepartment));

            this.fields.ReturnWorker = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.ReturnWorker ? documentUI.formatFullName(docData.ReturnWorker.LastName, docData.ReturnWorker.FirstName, docData.ReturnWorker.MiddleName) : "")
                .attr('valueid', docData.ReturnWorkerID)
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
            table.append(row('Передав результат:', this.fields.ReturnWorker.add(documentUI.createButtonForAutocomplete(this.fields.ReturnWorker))));


            this.fields.ServiceResult = $('<input type="text" style="width: 670px;">').val(docData.ServiceResult);
            table.append(row("Результат а.п.:", this.fields.ServiceResult));


            this.fields.DateResponseToClient = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.DateResponseToClient) {
                var dateResponseToClient = new Date(+docData.DateResponseToClient.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (dateResponseToClient > testTime)
                    this.fields.DateResponseToClient.datepicker("setDate", dateResponseToClient);
            }
            table.append(row('Дата отримання а.п.:', this.fields.DateResponseToClient));
            
            this.fields.ResponseClientInfo = $('<input type="text" style="width: 670px;">').val(docData.ResponseClientInfo);
            table.append(row("Отримання а.п.:", this.fields.ResponseClientInfo));
            
            this.fields.IsControlled = docData.IsControlled;

            this.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 36 : 54))
                .attr('rows', (isMiniMode ? '2' : '3'))
                .val(docData.Content);
            table.append(row('Короткий зміст документа:', this.fields.Content));

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

            //Buttons
            this.buttons.buttonCreate = $('<input type="button" value="Додати">').button().click(function(e) {
                var docObj = {};

                docObj.ID = self.fields.ID;
                docObj.DocumentID = self.fields.DocumentID;

                docObj.Document = {};
                docObj.Document.ID = self.fields.Document.ID;
                docObj.Document.DepartmentID = self.departmentID;
                docObj.Document.Files = self.fields.Document.Files;

                var departmentName = self.fields.ExecutiveDepartment.val();
                if (departmentName)
                    docObj.ExecutiveDepartmentID = parseFloat(self.fields.ExecutiveDepartment.attr('valueid'));
                else
                    docObj.ExecutiveDepartmentID = 0;

                docObj.SubjectRequest = self.fields.SubjectRequest.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();
                docObj.ServiceName = self.fields.ServiceName.val().trim();
                docObj.ObjectForService = self.fields.ObjectForService.val().trim();
                
                docObj.DateReceivedToDepartment = self.fields.DateReceivedToDepartment.val().trim();
                docObj.DateReturnFromDepartment = self.fields.DateReturnFromDepartment.val().trim();

                docObj.ReturnWorkerName = self.fields.ReceivedWorker.val();
                if (docObj.ReturnWorkerName) {
                    docObj.ReceivedWorkerID = parseFloat(self.fields.ReceivedWorker.attr('valueid'));
                } else {
                    docObj.ReceivedWorkerID = 0;
                }
                
                docObj.ReturnWorkerName = self.fields.ReturnWorker.val();
                if (docObj.ReturnWorkerName)
                    docObj.ReturnWorkerID = parseFloat(self.fields.ReturnWorker.attr('valueid'));
                else
                    docObj.ReturnWorkerID = 0;

                docObj.ServiceResult = self.fields.ServiceResult.val().trim();
                docObj.DateResponseToClient = self.fields.DateResponseToClient.val().trim();
                docObj.ResponseClientInfo = self.fields.ResponseClientInfo.val().trim();

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
                if (docObj.Document.CodeName) {
                    docObj.Document.CodeID = parseFloat(self.fields.Document.CodeName.attr('valueid'));
                } else {
                    docObj.Document.CodeID = 0;
                }
                docObj.IsControlled = self.fields.IsControlled;

                docObj.Content = self.fields.Content.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();
                
                docObj.Document.DocStatusID = self.fields.Document.DocStatusID;
                docObj.Document.TemplateId = 4;

                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=' + type;

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
                    }
                });
                return false;
            });

            if (type == 'ins')
                this.buttons.buttonCreate.val('Додати');
            else if (type == 'upd')
                this.buttons.buttonCreate.val('Модифікувати');

            this.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                self.dialog.dialog("close");
            });


            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreate.add(this.buttons.buttonCancel))));
            this.form = $('<div title="Створення документу" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            $('body').append(this.form);
            
            this.dialog = $(this.form).dialog({
                autoOpen: false,
                draggable: true,
                modal: true,
                position: ["top"],
                resizable: true,
                width: 860,
                close: function () {
                    self.dispose();
                },
                open: function () {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },

        this.showInsertForm = function () {
            var docObj = {
                ID: 0,
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
                SubjectRequest: '',
                ServiceName: '',
                ObjectForService: '',
                ExecutiveDepartmentID: 0,
                DateReceivedToDepartment: '',
                ReceivedWorkerID: 0,
                DateReturnFromDepartment: '',
                ReturnWorkerID: 0,
                ServiceResult: '',
                DateResponseToClient: '',
                ResponseClientInfo: '',
                IsControlled: false,
                TemplateId: 4
            };

            this.createForm(docObj, 'ins');
            this.dialog.dialog("open");
        },

        this.showUpdateForm = function (documentId) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getblank&jdata=' + documentId;

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

        this.showViewForm = function (documentId) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getblank&jdata=' + documentId;

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

        this.showDeleteForm = function (documentId, success) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + self.templateID + '&type=del&jdata=' + documentId;

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
                                    if (success instanceof Function) {
                                        success();
                                    }
                                    deleteDlg.dialog("close");
                                }
                            });
                        },
                        "Відмінити": function () {
                            $(this).dialog("close");
                        }
                    },
                    close: function () {
                        if (deleteDlg) {
                            deleteDlg.remove();
                        }
                    }
                });
        },

        this.dispose = function () {
            if (this.form) {
                this.form.remove();
            }
            if (this.dialog) {
                this.dialog.remove();
            }
        };
    };

    window.AsDocumentBlank = asDocumentBlank;
})(window);