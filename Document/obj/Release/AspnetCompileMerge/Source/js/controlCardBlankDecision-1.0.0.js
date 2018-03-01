(function (window, undefined) {

    var appSettings = window.appSettings;
    var controlCardBlankDecision = function (options) {
        var self = this;

        this.departmentID = 0,
        this.templateID = 0;

        if (options) {
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

        this.createForm = function (docData, type, onsuccess) {
            this.dispose();

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');
            var testTime = new Date(1, 0, 1, 0, 0, 0);

            this.fields.ID = docData.ID;
            this.fields.DocumentID = docData.DocumentID;


            this.fields.CardNumber = $('<span></span>').text(docData.CardNumber);
            table.append(row('Номер картки:', this.fields.CardNumber));

            this.fields.Worker = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.Worker ? documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName) : "")
                .attr('valueid', docData.WorkerID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + self.departmentID + '&term=' + request.term,
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
            table.append(row('Виконавець:', this.fields.Worker.add(documentUI.createButtonForAutocomplete(this.fields.Worker))));

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


            this.fields.StartDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.StartDate) {
                var startDate = new Date(+docData.StartDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (startDate > testTime)
                    this.fields.StartDate.datepicker("setDate", startDate);
            }
            table.append(row('Дата початку контролю:', this.fields.StartDate));

            this.fields.EndDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.EndDate) {
                var endDate = new Date(+docData.EndDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (endDate > testTime)
                    this.fields.EndDate.datepicker("setDate", endDate);
            }
            table.append(row('До якої дати виконати:', this.fields.EndDate));
            

            this.fields.Resolution = $('<textarea style="height:56px;width:98%;font-size:15px;" cols="81" rows="3"></textarea>').val(docData.Resolution);
            table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('Знято рішенням:').append(this.fields.Resolution)));
            
            this.fields.IsSpeciallyControlled = docData.IsSpeciallyControlled;
            
            this.fields.ControlResponseDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.ControlResponseDate) {
                var controlResponseDate = new Date(+docData.ControlResponseDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (controlResponseDate > testTime)
                    this.fields.ControlResponseDate.datepicker("setDate", controlResponseDate);
            }
            table.append(row('Дата зняття:', this.fields.ControlResponseDate));

            this.fields.ControlResponse = $('<textarea style="height:36px;width:98%;font-size:15px;" cols="81" rows="2"></textarea>').val(docData.ControlResponse);
            table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('Лист вхідний:').append(this.fields.ControlResponse)));

            this.fields.Notes = $('<textarea style="height:36px;width:98%;font-size:15px;" cols="81" rows="2"></textarea>').val(docData.Notes);
            table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('Лист вихідний:').append(this.fields.Notes)));

            this.fields.IsDecisionOfSession = docData.IsDecisionOfSession;
            this.fields.IsDecisionOfExecutiveCommittee = docData.IsDecisionOfExecutiveCommittee;
            this.fields.IsOrderOfHeader = docData.IsOrderOfHeader;
            this.fields.IsActionWorkPlan = docData.IsActionWorkPlan;
            this.fields.IsSendResponse = docData.IsSendResponse;
            this.fields.IsSendInterimResponse = docData.IsSendInterimResponse;
            this.fields.IsLeftToWorker = docData.IsLeftToWorker;
            this.fields.IsAcquaintedWorker = docData.IsAcquaintedWorker;
            
            this.fields.DocStatus = 0;

            this.fields.CardStatus = $('<input type="text" style="width: 300px;">').val(docData.CardStatus ? docData.CardStatus.Name : "")
                .attr('valueid', docData.CardStatusID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardstatus&type=search&dep=' + self.departmentID + '&term=' + request.term,
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
            table.append(row('Стан картки:', this.fields.CardStatus.add(documentUI.createButtonForAutocomplete(this.fields.CardStatus))));

            this.fields.IsContinuation = docData.IsContinuation;

            //Buttons
            this.buttons.buttonCreate = $('<input type="button" value="Додати">').button().click(function () {
                var ccObj = {};

                ccObj.ID = self.fields.ID;
                ccObj.DocumentID = self.fields.DocumentID;

                var cardNumber = parseInt(self.fields.CardNumber.text());
                if (!isNaN(cardNumber)) {
                    ccObj.CardNumber = cardNumber;
                }

                var workerName = self.fields.Worker.val();
                if (workerName)
                    ccObj.WorkerID = parseFloat(self.fields.Worker.attr('valueid'));
                else
                    ccObj.WorkerID = 0;
                ccObj.HeadID = ccObj.HeadID;
                ccObj.FixedWorkerID = ccObj.WorkerID;
                ccObj.HeadResponseID = 0;

                var departmentName = self.fields.ExecutiveDepartment.val();
                if (departmentName)
                    ccObj.ExecutiveDepartmentID = parseFloat(self.fields.ExecutiveDepartment.attr('valueid'));
                else
                    ccObj.ExecutiveDepartmentID = 0;

                ccObj.StartDate = self.fields.StartDate.val();
                ccObj.EndDate = self.fields.EndDate.val();

                ccObj.Notes = self.fields.Notes.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();
                ccObj.Resolution = self.fields.Resolution.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                ccObj.IsSpeciallyControlled = self.fields.IsSpeciallyControlled;

                ccObj.ControlResponseDate = self.fields.ControlResponseDate.val();
                ccObj.ControlResponse = self.fields.ControlResponse.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();
                
                ccObj.IsDecisionOfSession = self.fields.IsDecisionOfSession;
                ccObj.IsDecisionOfExecutiveCommittee = self.fields.IsDecisionOfExecutiveCommittee;
                ccObj.IsOrderOfHeader = self.fields.IsOrderOfHeader;
                ccObj.IsActionWorkPlan = self.fields.IsActionWorkPlan;
                ccObj.IsSendResponse = self.fields.IsSendResponse;
                ccObj.IsSendInterimResponse = self.fields.IsSendInterimResponse;
                ccObj.IsLeftToWorker = self.fields.IsLeftToWorker;
                ccObj.IsAcquaintedWorker = self.fields.IsAcquaintedWorker;

                ccObj.IsContinuation = self.fields.IsContinuation;

                ccObj.DocStatusID = 0;

                var cardStatusName = self.fields.CardStatus.val();
                if (cardStatusName)
                    ccObj.CardStatusID = parseFloat(self.fields.CardStatus.attr('valueid'));
                else
                    ccObj.CardStatusID = 0;


                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=' + type;

                $.ajax({
                    url: urlRequest,
                    type: "POST",
                    cache: false,
                    data: { 'jdata': JSON.stringify(ccObj) },
                    dataType: "json",
                    success: function (msg) {
                        self.dialog.dialog("close");
                        if (onsuccess instanceof Function)
                            onsuccess(msg);
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


            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold" class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreate.add(this.buttons.buttonCancel))));
            this.form = $('<div title="Створення картки" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            $('body').append(this.form);

            this.dialog = $(this.form).dialog({
                autoOpen: false,
                draggable: false,
                modal: true,
                position: ["center", "top"],
                resizable: false,
                width: 640,
                close: function () {
                    self.dispose();
                },
                open: function () {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },

        this.showInsertForm = function (params, onsuccess) {
            var ccObj = {
                ID: 0,
                DocumentID: 0,

                CardNumber: 1,
                HeadID: 0,
                WorkerID: 0,
                ExecutiveDepartmentID: 0,

                StartDate: '',
                EndDate: '',
                Resolution: '',
                Notes: '',
                IsSpeciallyControlled: true,
                FixedWorkerID: 0,

                ControlResponseDate: '',
                ControlResponse: '',
                HeadResponseID: 0,

                DocStatusID: 0,
                CardStatusID: 1,
                CardStatus: { ID: 1, Name: 'СТОЇТЬ НА КОНТРОЛІ' },

                IsDecisionOfSession: true,
                IsDecisionOfExecutiveCommittee: false,
                IsOrderOfHeader: false,
                IsActionWorkPlan: false,
                IsSendResponse: false,
                IsSendInterimResponse: false,
                IsLeftToWorker: false,
                IsAcquaintedWorker: false,
                IsContinuation: false
            };

            if (params && params.cardNumber)
                ccObj.CardNumber = params.cardNumber;
            if (params && params.documentID)
                ccObj.DocumentID = params.documentID;

            this.createForm(ccObj, 'ins', onsuccess);
            this.dialog.dialog("open");
        },

        this.showUpdateForm = function (params, onsuccess) {
            if (params) {
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=get&jdata=' + params.controlCardID,
                    dataType: "json",
                    success: function (data) {
                        self.createForm(data, 'upd', onsuccess);
                        self.dialog.dialog("open");
                    }
                });
            }
        },

        this.showViewForm = function (params, onsuccess) {
            if (params) {
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=get&jdata=' + params.controlCardID,
                    dataType: "json",
                    success: function (data) {
                        self.createForm(data, 'upd', onsuccess);
                        self.dialog.dialog("open");
                    }
                });
            }
        },

        this.showContinueForm = function (params, onsuccess) {
            if (params) {
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=get&jdata=' + params.controlCardID,
                    dataType: "json",
                    success: function (data) {
                        data.CardStatusID = 2;
                        data.CardStatus.ID = 2;
                        data.CardStatus.Name = 'ВІДПРАВЛЕНО НА ДООПРАЦЮВАННЯ';
                        self.createForm(data, 'upd', function () {
                            var newData = {
                                ID: 0,
                                DocumentID: data.DocumentID,
                                CardNumber: data.CardNumber,
                                CardStatusID: 1,
                                CardStatus: { ID: 1, Name: 'СТОЇТЬ НА КОНТРОЛІ' },
                                ExecutiveDepartment: data.ExecutiveDepartment,
                                ExecutiveDepartmentID: data.ExecutiveDepartmentID,
                                HeadID: data.HeadID,
                                Head: data.Head,
                                WorkerID: data.WorkerID,
                                Worker: data.Worker,
                                IsSpeciallyControlled: true,
                                IsContinuation: true
                            };
                            self.createForm(newData, 'ins', onsuccess);
                            self.dialog.dialog("open");
                        });
                        self.dialog.dialog("open");
                    }
                });
            }
        },

        this.showDeleteForm = function (params, success) {
            if (params) {
                var deleteDlg = $('<div title="Видалити картку?" style="display:none;"><p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>Цей об"єкт буде дійсно видалений і не підлягатиме відновленню. Ви дійсно бажаєте цього?</p></div>')
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
                                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=del&jdata=' + params.controlCardID,
                                    dataType: "json",
                                    success: function () {
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
                            //deleteDlg.dialog("destroy");
                        }
                    });
            }
        },

        this.dispose = function () {
            if (this.form)
                this.form.remove();
            if (this.dialog)
                this.dialog.remove();
        };
    };

    window.CardBlankDecision = controlCardBlankDecision;

})(window);
