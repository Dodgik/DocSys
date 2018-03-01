(function (window, $, undefined) {

    var appSettings = window.appSettings;
    var draftDocumentBlank = function (options) {
        var self = this,
            thisDoc = undefined;

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
            
            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            this.fields.ID = docData.ID;
            this.fields.DocumentID = docData.DocumentID;

            this.fields.Document = {};
            this.fields.Document.ID = docData.Document.ID;
            this.fields.Document.Files = docData.Document.Files;
            
            this.fields.Document.Destination = {};

            function getCodeName(codeId) {
                return (codeId === 8 ? 'Вихідний' : (codeId === 12 ? 'Внутрішній' : ''));
            }

            var codeId = docData.Document.CodeID || 8,
                codeName = (getCodeName(codeId) || docData.Document.CodeName);
            this.fields.Document.CodeName = $('<input type="text" valueid="' + codeId + '" style="width: 300px;">').val(codeName)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: [{ label: 'Вихідний', value: 'Вихідний', id: 8 }, { label: 'Внутрішній', value: 'Внутрішній', id: 12 }],
                    select: function(event, ui) {
                        $(this).attr('valueid', ui.item.id);
                        thisDoc.Document.CodeID = ui.item.id;
                        initForm({ Document: { CodeID: ui.item.id }, IsInput: docData.IsInput });
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Шифр:', this.fields.Document.CodeName.add(documentUI.createButtonForAutocomplete(this.fields.Document.CodeName))));

            this.fields.DocType = $('<input type="text" valueid="0" style="width: 300px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=doctype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term + '&code=' + self.fields.Document.CodeName.attr('valueid'),
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.DocTypeID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Найменування документа:', this.fields.DocType.add(documentUI.createButtonForAutocomplete(this.fields.DocType))));


            this.fields.Document.Destination.Organization = $('<input type="text" style="width: 300px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.Document.Destination.OrganizationID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Організація:', this.fields.Document.Destination.Organization
                    .add(documentUI.createButtonForAutocomplete(this.fields.Document.Destination.Organization))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
					.button({ icons: { primary: 'ui-icon-closethick' }, text: false })
					.click(function () {
					    self.fields.Document.Destination.Organization.attr('valueid', 0);
					    thisDoc.Document.Destination.OrganizationID = 0;
					    self.fields.Document.Destination.Organization.val('');
					}))));
            
            this.fields.Document.Destination.Department = $('<input type="text" style="width: 300px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.Document.Destination.DepartmentID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Підрозділ:', this.fields.Document.Destination.Department
                    .add(documentUI.createButtonForAutocomplete(this.fields.Document.Destination.Department))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
					.button({ icons: { primary: 'ui-icon-closethick' }, text: false })
					.click(function () {
					    self.fields.Document.Destination.Department.attr('valueid', 0);
					    self.fields.Document.Destination.Department.val('');
					    thisDoc.Document.Destination.DepartmentID = 0;
					}))));

            this.fields.Document.Destination.Worker = $('<input type="text" style="width: 300px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + userdata.departmentId + '&term=' + request.term,
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.Document.Destination.WorkerID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Працівник:', this.fields.Document.Destination.Worker
                    .add(documentUI.createButtonForAutocomplete(this.fields.Document.Destination.Worker))
                    .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
					.button({ icons: { primary: 'ui-icon-closethick' }, text: false })
					.click(function () {
					    self.fields.Document.Destination.Worker.attr('valueid', 0);
					    self.fields.Document.Destination.Worker.val('');
					    thisDoc.Document.Destination.WorkerID = 0;
					}))));
            
            var workerFullName = '',
                workerId = 0;
            if (docData.Worker) {
                workerId = docData.WorkerID;
                workerFullName = documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName);
            } else {
                workerId = userdata.worker.ID;
                workerFullName = documentUI.formatFullName(userdata.worker.LastName, userdata.worker.FirstName, userdata.worker.MiddleName);
            }
            
            this.fields.Worker = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(workerFullName)
                .attr('valueid', workerId)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + userdata.departmentId + '&term=' + request.term,
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.WorkerID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Підготовив:', this.fields.Worker.add(documentUI.createButtonForAutocomplete(this.fields.Worker))));

            this.fields.Head = $('<input type="text" valueid="0" style="width: 300px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            //url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + userdata.departmentId + '&term=' + request.term,
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&term=' + request.term,
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.HeadID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('За підписом:', this.fields.Head.add(documentUI.createButtonForAutocomplete(this.fields.Head))));

            this.fields.IsPublic = $('<input type="checkbox">');
            table.append(row('Публікувати:', this.fields.IsPublic));
            
            this.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height(114).attr('rows', '6');
            table.append(row('Зміст документа:', this.fields.Content));

            this.buttons.buttonCopyText = $('<button style="width: 540px; height:26px;">Копіювати</button>')
            .button({ icons: { primary: 'ui-icon-circle-arrow-s', secondary: 'ui-icon-circle-arrow-s' } }).click(function () {
                self.fields.PublicContent.val(self.fields.Content.val());
                return false;
            });
            table.append(row('', this.buttons.buttonCopyText));

            this.fields.PublicContent = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height(76).attr('rows', '4');
            table.append(row('Публічний зміст документа:', this.fields.PublicContent));

            this.fields.QuestionType = $('<input type="text" style="width: 300px;">')
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=questiontype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: 'GET',
                            dataType: 'json',
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
                        thisDoc.HeadID = ui.item.id;
                    }
                }).addClass('ui-widget ui-widget-content ui-corner-left');
            table.append(row('Категорія питання:', this.fields.QuestionType.add(documentUI.createButtonForAutocomplete(this.fields.QuestionType))));


            this.fields.Changes = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height(38)
                .attr('rows', '2')
                .val(docData.Changes);
            table.append(row('Внесені зміни:', this.fields.Changes));

            this.fields.Document.Notes = $('<textarea style="width:98%; font-size:15px;" cols="81"></textarea>')
                .height(38)
                .attr('rows', '2');
            table.append(row('Особливі відмітки:', this.fields.Document.Notes));

            this.fields.Document.DocStatusID = docData.Document.DocStatusID;

            this.fields.uploadButton = $('<div></div>');
            table.append(row('Прикріплені файли:', this.fields.uploadButton));
            recreateFileUploader();
            
            function recreateFileUploader() {
                var uploadButton = $('<div></div>');
                self.fields.uploadButton.replaceWith(uploadButton);
                self.fields.uploadButton = uploadButton;
                
                var uploadedList = [];
                for (var i in self.fields.Document.Files)
                    uploadedList.push({ id: self.fields.Document.Files[i].FileID, name: self.fields.Document.Files[i].FileName });
                var uploader = new qq.FileUploader({
                    element: self.fields.uploadButton[0],
                    action: appSettings.rootUrl + 'Uploader.ashx?documentid=' + thisDoc.DocumentID,
                    fileUrl: appSettings.rootUrl + 'File.ashx?id=',
                    openUrl: appSettings.rootUrl + 'ImageViewer.aspx?documentID=' + thisDoc.DocumentID + '&?id=',
                    debug: true,
                    uploadedList: uploadedList,
                    onComplete: function (id, fileName, response) {
                        self.fields.Document.Files.push({ DocumentID: thisDoc.DocumentID, FileID: response.fileID, FileName: fileName });
                        thisDoc.Document.DocStateID = -1;
                        sendDocData();
                    },
                    onRemove: function (id, file) {
                        if (confirm('Ви дійсно бажаєте видалити файл ' + file.name + ' ?')) {
                            for (var j in self.fields.Document.Files)
                                if (self.fields.Document.Files[j].FileID == file.id)
                                    self.fields.Document.Files.splice(j, 1);

                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=del&dep=' + documentUI.departmentID + '&fileID=' + file.id + '&documentid=' + self.fields.DocumentID,
                                type: 'GET',
                                dataType: 'json',
                                success: function () {
                                    return true;
                                },
                                error: function () {
                                    return true;
                                }
                            });
                            return true;
                        } else {
                            return false;
                        }
                    }
                });
            }

            //Buttons
            this.buttons.buttonSend = $('<input type="button" value="Надіслати">').button().click(function () {
                thisDoc = getDocData();
                if (thisDoc.Document.Destination.OrganizationName || thisDoc.Document.Destination.DepartmentID > 0 || thisDoc.Document.Destination.WorkerID > 0) {
                    thisDoc.Document.DocStateID = 0;
                    sendDocData({ close: true });
                } else {
                    alert('Не вказаний адресат');
                }
            });
            this.buttons.buttonSave = $('<input type="button" value="Зберегти">').button().click(function () {
                thisDoc.Document.DocStateID = -1;
                sendDocData({ close: true });
            });
            this.buttons.buttonDelete = $('<input type="button" value="Видалити">').button().click(function () {
                if (thisDoc.ID > 0) {
                    self.showDeleteForm(thisDoc.ID, function(msg) {
                        self.dialog.dialog('close');
                    });
                } else {
                    self.dialog.dialog('close');
                }
            });
            this.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                self.dialog.dialog('close');
            });


            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>')
                .append(this.buttons.buttonSend.add(this.buttons.buttonSave).add(this.buttons.buttonDelete).add(this.buttons.buttonCancel))));
            this.form = $('<div title="Створення документу" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            $('body').append(this.form);


            this.dialog = $(this.form).dialog({
                autoOpen: false,
                draggable: true,
                modal: true,
                position: ['top'],
                resizable: true,
                width: 860,
                close: function () {
                    self.dispose();
                },
                open: function () {
                    $('.ui-widget-overlay').css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
            setFields(docData);
            initForm(docData);

            function initForm(formData) {
                if (formData.Document.CodeID === 12) {
                    self.fields.Document.Destination.Organization.parents('tr').hide();
                    self.fields.Document.Destination.Department.parents('tr').show();
                    self.fields.Document.Destination.Worker.parents('tr').show();
                } else {
                    self.fields.Document.Destination.Organization.parents('tr').show();
                    self.fields.Document.Destination.Department.parents('tr').hide();
                    self.fields.Document.Destination.Worker.parents('tr').hide();
                }
                if (formData.Document.Destination && formData.Document.Destination.OrganizationID > 0) {
                    self.fields.Document.Destination.Organization.parents('tr').show();
                }
            }

            function getDocData() {
                var docObj = thisDoc;

                docObj.Document.Files = self.fields.Document.Files;

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

                docObj.Changes = self.fields.Changes.val().replace(/(\r\n|\n|\r)/gm, ' ').replace(/\s+/g, ' ').trim();
                docObj.Document.Notes = self.fields.Document.Notes.val().replace(/(\r\n|\n|\r)/gm, ' ').replace(/\s+/g, ' ').trim();

                docObj.Document.CodeName = self.fields.Document.CodeName.val();
                if (docObj.Document.CodeName)
                    docObj.Document.CodeID = parseFloat(self.fields.Document.CodeName.attr('valueid'));
                else
                    docObj.Document.CodeID = 0;

                docObj.IsPublic = self.fields.IsPublic.is(':checked');

                docObj.Content = self.fields.Content.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, ' \n').trim();
                docObj.PublicContent = self.fields.PublicContent.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, ' \n').trim();

                docObj.QuestionTypeName = self.fields.QuestionType.val();
                if (docObj.QuestionTypeName)
                    docObj.QuestionTypeID = parseFloat(self.fields.QuestionType.attr('valueid'));
                else
                    docObj.QuestionTypeID = 0;

                return docObj;
            }

            function setDocData(data) {
                thisDoc = data;
            }

            function setFields(data) {
                self.fields.Document.Files = data.Document.Files;

                self.fields.Document.Destination.Organization
                    .val(data.Document.Destination.OrganizationID > 0 ? data.Document.Destination.OrganizationName : '')
                    .attr('valueid', data.Document.Destination.OrganizationID);
                self.fields.Document.Destination.Department
                    .val(data.Document.Destination.DepartmentID > 0 ? data.Document.Destination.DepartmentName : '')
                    .attr('valueid', data.Document.Destination.DepartmentID);

                var worker = data.Document.Destination.Worker;
                var desWorkerName = (data.Document.Destination && data.Document.Destination.WorkerID > 0) ?
                                documentUI.formatFullName(worker.LastName, worker.FirstName, worker.MiddleName) : '';
                self.fields.Document.Destination.Worker.val(desWorkerName).attr('valueid', data.Document.Destination.WorkerID);

                var headName = (data.HeadID > 0) ? documentUI.formatFullName(data.Head.LastName, data.Head.FirstName, data.Head.MiddleName) : '';
                self.fields.Head.val(headName).attr('valueid', data.HeadID);

                var workerName = (data.WorkerID > 0) ? documentUI.formatFullName(data.Worker.LastName, data.Worker.FirstName, data.Worker.MiddleName) : '';
                self.fields.Worker.val(workerName).attr('valueid', data.WorkerID);

                self.fields.DocType.val(data.DocTypeID > 0 ? data.DocType.Name : '').attr('valueid', data.DocTypeID);
                
                self.fields.Changes.val(data.Changes);
                self.fields.Document.Notes.val(data.Document.Notes);

                self.fields.Document.CodeName.val(getCodeName(data.Document.CodeID) || data.Document.CodeName).attr('valueid', data.Document.CodeID);
                if (data.Document.ParentDocumentID > 0) {
                    self.fields.Document.CodeName.parents('tr').hide();
                }

                self.fields.IsPublic.prop('checked', data.IsPublic);

                self.fields.Content.val(data.Content);
                self.fields.PublicContent.val(data.PublicContent);

                self.fields.QuestionType.val(data.QuestionTypeID > 0 ? data.QuestionType.Name : '').attr('valueid', data.QuestionTypeID);

                self.fields.Document.Files = data.Document.Files;
                recreateFileUploader();
            }

            function sendDocData(p) {
                var docObj = getDocData();
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=draft&departmentID=' + userdata.departmentId;

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
        },

        this.showInsertForm = function (data) {
            thisDoc = getEmptyDocumentObject();

            if (data) {
                thisDoc = $.extend(true, thisDoc, data);
            }
            thisDoc.Document.DocStatusID = -1;

            this.createForm(thisDoc, 'ins');
            this.dialog.dialog('open');
        },

        this.showUpdateForm = function (documentId) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getblank&jdata=' + documentId;

            $.ajax({
                type: 'GET',
                cache: false,
                url: urlRequest,
                dataType: 'json',
                success: function (data) {
                    thisDoc = data;
                    self.createForm(thisDoc, 'upd');
                    self.dialog.dialog('open');
                }
            });
        },
        this.showViewForm = function (documentId) {
            self.showUpdateForm(documentId);
        },
        
        this.showDeleteForm = function (documentId, success) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=del&jdata=' + documentId;

            var deleteDlg = $('<div title="Видалити документ?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде дійсно видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
                    .dialog({
                        autoOpen: true,
                        modal: true,
                        position: ['center'],
                        resizable: false,
                        buttons: {
                            'Видалити': function () {
                                $.ajax({
                                    type: 'GET',
                                    cache: false,
                                    url: urlRequest,
                                    dataType: 'json',
                                    success: function () {
                                        if (success instanceof Function)
                                            success();
                                        deleteDlg.dialog('close');
                                    }
                                });
                            },
                            'Відмінити': function () {
                                $(this).dialog('close');
                            }
                        },
                        close: function () {
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
        
        
        function getEmptyDocumentObject () {
            var docObj = models.DocTemplate();
            docObj.TemplateId = 3;
            docObj.Document = models.Document();
            docObj.Document.DepartmentID = self.departmentID;
            docObj.Document.Destination = models.Destination();
            docObj.Document.Source = models.Source();
            docObj.Document.CodeID = 8;
            docObj.Document.DocStatusID = -1;

            return docObj;
        }
    };

    window.DraftDocumentBlank = draftDocumentBlank;

})(window, jQuery);