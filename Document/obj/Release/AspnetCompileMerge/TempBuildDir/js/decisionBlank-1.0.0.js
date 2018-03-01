(function (window, undefined) {

    var document = window.document,
        navigator = window.navigator,
        location = window.location,
        appSettings = window.appSettings;
    var decisionBlank = function (options) {
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

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            this.fields.ID = docData.ID;
            this.fields.DocumentID = docData.DocumentID;
            this.fields.IsInput = docData.IsInput;


            this.fields.Document = {};
            this.fields.Document.ID = docData.Document.ID;
            this.fields.Document.Files = docData.Document.Files;
            this.fields.Document.ExternalSource = docData.Document.ExternalSource;
            this.fields.Document.CodeID = docData.Document.CodeID;
            this.fields.DocType = docData.DocTypeID;

            this.fields.Document.Number = $('<input type="text" style="width: 200px;">').val(docData.Document.Number);
            table.append(row('№ рішення:', this.fields.Document.Number));

            this.fields.Document.CreationDate = $('<input type="text" style="width: 200px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.Document.CreationDate) {
                var creationDate = new Date(+docData.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                this.fields.Document.CreationDate.datepicker("setDate", creationDate);
            }
            table.append(row('Дата ухвалення:', this.fields.Document.CreationDate));


            this.fields.WorkerID = docData.WorkerID;
            this.fields.HeadID = docData.HeadID;


            this.fields.IsControlled = docData.IsControlled;

            this.fields.IsSpeciallyControlled = docData.IsSpeciallyControlled;
            this.fields.IsIncreasedControlled = docData.IsIncreasedControlled;

            this.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 54 : 95))
                .attr('rows', (isMiniMode ? '3' : '5'))
                .val(docData.Content);
            table.append(row('Назва рішення:', this.fields.Content));
            
            this.fields.PublicContent = docData.PublicContent;
            this.fields.Changes = docData.Changes;

            this.fields.QuestionTypeID = docData.QuestionTypeID;


            this.fields.IsPublic = docData.IsPublic;
            this.fields.NumberCopies = docData.NumberCopies;
            
            this.fields.Document.Notes = $('<textarea style="width:98%; font-size:15px;" cols="81"></textarea>')
                .height((isMiniMode ? 38 : 54))
                .attr('rows', (isMiniMode ? '2' : '3'))
                .val(docData.Document.Notes);
            table.append(row('Примітки:', this.fields.Document.Notes));

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
                            success: function () {
                                return true;
                            },
                            error: function () {
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
            this.buttons.buttonCreate = $('<input type="button" value="Додати">').button().click(function () {
                var docObj = currentModel();

                docObj.ID = self.fields.ID;
                docObj.DocumentID = self.fields.DocumentID;
                docObj.IsInput = self.fields.IsInput;
                docObj.Document = {};
                docObj.Document.ID = self.fields.Document.ID;
                docObj.Document.DepartmentID = self.departmentID;
                docObj.Document.Files = self.fields.Document.Files;

                docObj.Document.ExternalSource = {};

                docObj.HeadName = '';
                docObj.HeadID = self.fields.HeadID;

                docObj.WorkerName = '';
                docObj.WorkerID = self.fields.WorkerID;

                docObj.DocTypeName = '';
                docObj.DocTypeID = self.fields.DocType;

                docObj.Changes = self.fields.Changes;

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

                docObj.Document.CodeName = '';
                docObj.Document.CodeID = self.fields.Document.CodeID;

                docObj.IsControlled = self.fields.IsControlled;
                docObj.IsSpeciallyControlled = self.fields.IsSpeciallyControlled;
                docObj.IsIncreasedControlled = self.fields.IsIncreasedControlled;

                docObj.IsPublic = self.fields.IsPublic;
                docObj.NumberCopies = self.fields.NumberCopies;

                docObj.Content = self.fields.Content.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();
                docObj.PublicContent = self.fields.PublicContent;

                docObj.QuestionTypeName = '';
                docObj.QuestionTypeID = self.fields.QuestionTypeID;

                docObj.Document.DocStatusID = self.fields.Document.DocStatusID;

                docObj.Document.TemplateId = 5;

                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=5&type=' + type;

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
                    error: function (xhr) {
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
            var docObj = currentModel();

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
            if (this.form)
                this.form.remove();
            if (this.dialog)
                this.dialog.remove();
        };
        
        function currentModel () {
            return {
                ID: 0,
                IsInput: false,
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
                IsPublic: false,
                NumberCopies: 0,
                HeadID: 0,
                WorkerID: 0,
                TemplateId: 5
            };
        }
    };

    window.DecisionBlank = decisionBlank;

})(window);
