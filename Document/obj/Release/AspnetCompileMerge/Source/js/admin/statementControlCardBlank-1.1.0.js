﻿(function (window, $, undefined) {

    var appSettings = window.appSettings;
    var statementControlCardBlank = function (options) {
        options = options || {};
        var self = this,
            thisCc = undefined;

        this.departmentID = 0,
        this.templateID = 0;

        if (options.departmentID) {
            this.departmentID = options.departmentID;
        }
        if (options.templateID) {
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

            this.fields.Head = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.Head ? documentUI.formatFullName(docData.Head.LastName, docData.Head.FirstName, docData.Head.MiddleName) : '')
                .attr('valueid', docData.HeadID)
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
            table.append(row('Накладаючий резолюцію:', this.fields.Head.add(documentUI.createButtonForAutocomplete(this.fields.Head))));

            if (options.groupCards) {
                this.fields.cardsGroup = [];
                this.fields.cardsGroupTable = $('<table style="border: 1px dashed #C0C0C0;"></table>');
                table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2"></td>').append(this.fields.cardsGroupTable)));
                this.fields.buttonAddCardToGroup = $('<input type="button" value="Додати виконавця" style="height: 24px; width: 200px;">').button().click(function (e) {
                    addCardToGroup();
                });
                table.append(row('', this.fields.buttonAddCardToGroup));
                if (docData.Group && docData.Group.length) {
                    docData.Group.forEach(function (g) {
                        addCardToGroup(g);
                    });
                } else {
                    addCardToGroup();
                }
            } else {
                /*
                if (docData.ID) {

                    this.fields.ExecutiveDepartment = $('<input type="text" valueid="0" style="width: 300px;">')
                        .attr('disabled', 'disabled')
                        .val(docData.ExecutiveDepartment ? docData.ExecutiveDepartment.Name : '')
                        .attr('valueid', docData.ExecutiveDepartmentID);
                    table.append(row('Виконавче управління:', this.fields.ExecutiveDepartment));

                    this.fields.Worker = $('<input type="text" valueid="0" style="width: 300px;">')
                        .attr('disabled', 'disabled')
                        .val(docData.Worker ? documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName) : '')
                        .attr('valueid', docData.WorkerID);
                    table.append(row('Виконавець:', this.fields.Worker));
                } else {
                */
                    this.fields.ExecutiveDepartment = $('<input type="text" valueid="0" style="width: 300px;">')
                        .val(docData.ExecutiveDepartment ? docData.ExecutiveDepartment.Name : '')
                        .attr('valueid', docData.ExecutiveDepartmentID)
                        .autocomplete({
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
                    table.append(row('Виконавче управління:', this.fields.ExecutiveDepartment.add(documentUI.createButtonForAutocomplete(this.fields.ExecutiveDepartment))));

                    this.fields.Worker = $('<input type="text" valueid="0" style="width: 300px;">')
                        .val(docData.Worker ? documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName) : '')
                        .attr('valueid', docData.WorkerID)
                        .autocomplete({
                            delay: 0,
                            minLength: 0,
                            source: function(request, response) {
                                $.ajax({
                                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + self.departmentID + '&term=' + request.term,
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
                    table.append(row('Виконавець:', this.fields.Worker.add(documentUI.createButtonForAutocomplete(this.fields.Worker))));
                //}
            }

            function removeCardFromoGroup(index) {
                self.fields.cardsGroup.splice(index, 1);
                self.fields.cardsGroup.forEach(function (r, i) {
                    r.number = i + 1;
                    $(r.rows[0]).find('.number-in-group').text(r.number);
                });
            }

            function addCardToGroup(g) {
                var cg = {
                    ExecutiveDepartment: null,
                    Worker: null,
                    rows: [],
                    number: 1
                },
                    executiveDepartmentName = '',
                    executiveDepartmentId = 0,
                    workerName = '',
                    workerId = 0;
                if (g) {
                    cg.card = g;
                    cg.number = g.CardNumber;
                    executiveDepartmentName = g.ExecutiveDepartment ? g.ExecutiveDepartment.Name : '';
                    executiveDepartmentId = g.ExecutiveDepartmentID;
                    workerName = g.Worker ? documentUI.formatFullName(g.Worker.LastName, g.Worker.FirstName, g.Worker.MiddleName) : '';
                    workerId = g.WorkerID;
                } else {
                    if (self.fields.cardsGroup.length) {
                        cg.number = self.fields.cardsGroup[self.fields.cardsGroup.length - 1].number + 1;
                    } else {
                        cg.number = 1;
                    }
                }

                var executiveDepartmentRow = '';
                if (cg.card && cg.card.ID) {
                    executiveDepartmentRow = cg.ExecutiveDepartment = $('<input type="text" valueid="0" style="width: 280px;">')
                        .attr('disabled', 'disabled')
                        .val(executiveDepartmentName)
                        .attr('valueid', executiveDepartmentId);
                } else {
                    executiveDepartmentRow = cg.ExecutiveDepartment = $('<input type="text" valueid="0" style="width: 280px;">')
                        .val(executiveDepartmentName)
                        .attr('valueid', executiveDepartmentId)
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
                    executiveDepartmentRow = cg.ExecutiveDepartment.add(documentUI.createButtonForAutocomplete(cg.ExecutiveDepartment))
                        .add($('<button type="button" title="Очистити поле" style="margin-left: 14px;"></button>').attr("tabIndex", -1)
                            .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                            .click(function() {
                                cg.ExecutiveDepartment.attr('valueid', 0);
                                cg.ExecutiveDepartment.val('');
                            }));
                }

                var depRow = $('<tr class="blank-row"></tr>')
                    .append($('<td class="number-in-group">' + cg.number + '</td>'))
                    .append($('<td class="blank-row-title">' + 'Виконавче управління:' + '</td>'))
                    .append($('<td></td>').append(executiveDepartmentRow))
                    .append($('<td rowspan="2"></td>').append(
                        $('<button type="button" title="Видалити поля" style="height: 46px; margin-left: 12px;"></button>').attr("tabIndex", -1)
                                .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                                .click(function () {
                                    if (cg.card && cg.card.InnerNumber) {
                                        alert('Зняти виконавця неможливо, так як він зареєстрував у себе цей документ');
                                    } else {
                                        cg.rows.forEach(function(r) {
                                            $(r).remove();
                                        });
                                        removeCardFromoGroup(cg.number - 1);
                                    }
                                })
                        )
                    );
                cg.rows.push(depRow);
                self.fields.cardsGroupTable.append(depRow);

                var workerRowContent = '';
                if (cg.card && cg.card.ID) {
                    workerRowContent = cg.Worker = $('<input type="text" valueid="0" style="width: 280px;">')
                        .attr('disabled', 'disabled')
                        .val(workerName)
                        .attr('valueid', workerId);
                } else {
                    workerRowContent = cg.Worker = $('<input type="text" valueid="0" style="width: 280px;">')
                        .val(workerName)
                        .attr('valueid', workerId)
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
                    workerRowContent = cg.Worker.add(documentUI.createButtonForAutocomplete(cg.Worker))
                        .add($('<button type="button" title="Очистити поле" style="margin-left: 16px;"></button>').attr("tabIndex", -1)
                            .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                            .click(function() {
                                cg.Worker.attr('valueid', 0);
                                cg.Worker.val('');
                            }));
                }

                var workerRow = $('<tr class="blank-row"></tr>')
                    .append('<td></td>')
                    .append($('<td class="blank-row-title">' + 'Виконавець:' + '</td>'))
                    .append($('<td></td>').append(workerRowContent));
                cg.rows.push(workerRow);
                self.fields.cardsGroupTable.append(workerRow);

                self.fields.cardsGroup.push(cg);

                return cg;
            }

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



            this.fields.Resolution = $('<textarea style="height:95px;width:98%;font-size:15px;" cols="81" rows="5"></textarea>').val(docData.Resolution);
            var resolutions = $('<select style="width:98%;font-size:13px !important;"><option></option></select>').on('change', function () {
                if ($(this).val()) {
                    var workerName = '';
                    if (self.fields.cardsGroup && self.fields.cardsGroup.length) {
                        workerName = self.fields.cardsGroup[0].Worker.val();
                    } else if (self.fields.Worker) {
                        workerName = self.fields.Worker.val();
                    }
                    self.fields.Resolution.val($(this).val().replace('{виконавець}', workerName));
                }
            });
            window.appData.resolutions.forEach(function (r) {
                resolutions.append('<option value="' + r + '">' + r + '</option>');
            });
            table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('Текст резолюції:').append(resolutions).append(this.fields.Resolution)));


            this.fields.IsSpeciallyControlled = $('<input type="checkbox">').attr('checked', docData.IsSpeciallyControlled);
            table.append(row('Особливий контроль:', this.fields.IsSpeciallyControlled));

            this.fields.FixedWorker = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.FixedWorker ? documentUI.formatFullName(docData.FixedWorker.LastName, docData.FixedWorker.FirstName, docData.FixedWorker.MiddleName) : '')
                .attr('valueid', docData.FixedWorkerID)
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
            table.append(row('За ким закріплена картка:', this.fields.FixedWorker.add(documentUI.createButtonForAutocomplete(this.fields.FixedWorker))));

            this.fields.ControlResponseDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.ControlResponseDate) {
                var controlResponseDate = new Date(+docData.ControlResponseDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (controlResponseDate > testTime)
                    this.fields.ControlResponseDate.datepicker("setDate", controlResponseDate);
            }
            table.append(row('Дата контрольної відповіді:', this.fields.ControlResponseDate));

            this.fields.ControlResponse = $('<textarea style="height:76px;width:98%;font-size:15px;" cols="81" rows="4"></textarea>').val(docData.ControlResponse);
            table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('Контрольна відповідь:').append(this.fields.ControlResponse)));

            this.fields.HeadResponse = $('<input type="text" valueid="0" style="width: 300px;">')
                .val(docData.HeadResponse ? documentUI.formatFullName(docData.HeadResponse.LastName, docData.HeadResponse.FirstName, docData.HeadResponse.MiddleName) : '')
                .attr('valueid', docData.HeadResponseID)
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
            table.append(row('Підписав відповідь:', this.fields.HeadResponse.add(documentUI.createButtonForAutocomplete(this.fields.HeadResponse))));
/*
            this.fields.IsDecisionOfSession = $('<input type="checkbox">').attr('checked', docData.IsDecisionOfSession);
            this.fields.IsDecisionOfExecutiveCommittee = $('<input type="checkbox">').attr('checked', docData.IsDecisionOfExecutiveCommittee);
            this.fields.IsOrderOfHeader = $('<input type="checkbox">').attr('checked', docData.IsOrderOfHeader);
            this.fields.IsActionWorkPlan = $('<input type="checkbox">').attr('checked', docData.IsActionWorkPlan);
            this.fields.IsSendResponse = $('<input type="checkbox">').attr('checked', docData.IsSendResponse);
            this.fields.IsSendInterimResponse = $('<input type="checkbox">').attr('checked', docData.IsSendInterimResponse);
            this.fields.IsLeftToWorker = $('<input type="checkbox">').attr('checked', docData.IsLeftToWorker);
            this.fields.IsAcquaintedWorker = $('<input type="checkbox">').attr('checked', docData.IsAcquaintedWorker);
            table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('<table style="width:100%;"></table>')
                        .append($('<tr></tr>').append('<td>Рішення сесії</td>').append($('<td></td>').append(this.fields.IsDecisionOfSession))
                            .append('<td>Направлена відповідь (інформація)</td>').append($('<td></td>').append(this.fields.IsSendResponse)))
                        .append($('<tr></tr>').append('<td>Рішення виконкому</td>').append($('<td></td>').append(this.fields.IsDecisionOfExecutiveCommittee))
                            .append('<td>Направлена проміжна відповідь</td>').append($('<td></td>').append(this.fields.IsSendInterimResponse)))
                        .append($('<tr></tr>').append('<td>Розпорядження голови</td>').append($('<td></td>').append(this.fields.IsOrderOfHeader))
                            .append('<td>Залишено для роботи у виконавця</td>').append($('<td></td>').append(this.fields.IsLeftToWorker)))
                        .append($('<tr></tr>').append('<td>Заходи, робочий план</td>').append($('<td></td>').append(this.fields.IsActionWorkPlan))
                            .append('<td>Виконавець ознайомлений</td>').append($('<td></td>').append(this.fields.IsAcquaintedWorker)))
                    ));
*/
            /*table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                    .append('<table style="width:100%;"></table>')
                        .append($('<tr></tr>').append('<td>Рішення сесії</td>').append($('<td></td>').append(this.fields.IsDecisionOfSession))
                            .append('<td>Направлена відповідь (інформація)</td>').append($('<td></td>').append(this.fields.IsDecisionOfExecutiveCommittee)))
                        .append($('<tr></tr>').append('<td>Рішення виконкому</td>').append($('<td></td>').append(this.fields.IsOrderOfHeader))
                            .append('<td>Направлена проміжна відповідь</td>').append($('<td></td>').append(this.fields.IsActionWorkPlan)))
                        .append($('<tr></tr>').append('<td>Розпорядження голови</td>').append($('<td></td>').append(this.fields.IsSendResponse))
                            .append('<td>Залишено для роботи у виконавця</td>').append($('<td></td>').append(this.fields.IsSendInterimResponse)))
                        .append($('<tr></tr>').append('<td>Заходи, робочий план</td>').append($('<td></td>').append(this.fields.IsLeftToWorker))
                            .append('<td>Виконавець ознайомлений</td>').append($('<td></td>').append(this.fields.IsAcquaintedWorker)))
                    ));*/

            this.fields.DocStatus = $('<input type="text" style="width: 300px;">').val(docData.DocStatus ? docData.DocStatus.Name : '')
                .attr('valueid', docData.DocStatusID)
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=docstatus&type=search&dep=' + self.departmentID + '&term=' + request.term + '&templateId=' + self.templateID,
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
            table.append(row('Результат розгляду:', this.fields.DocStatus.add(documentUI.createButtonForAutocomplete(this.fields.DocStatus))));

            //this.fields.DocStatus = 0;

            this.fields.CardStatus = $('<input type="text" style="width: 300px;">').val(docData.CardStatus ? docData.CardStatus.Name : '')
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
            this.fields.InnerNumber = docData.InnerNumber;

            //Buttons
            this.buttons.buttonCreate = $('<input type="button" value="Додати">').button().click(function (e) {
                var ccObj = thisCc;

                ccObj.ID = self.fields.ID;
                ccObj.DocumentID = self.fields.DocumentID;

                var cardNumber = parseInt(self.fields.CardNumber.text());
                if (!isNaN(cardNumber)) {
                    ccObj.CardNumber = cardNumber;
                }

                var headName = self.fields.Head.val();
                if (headName)
                    ccObj.HeadID = parseFloat(self.fields.Head.attr('valueid'));
                else
                    ccObj.HeadID = 0;

                ccObj.StartDate = self.fields.StartDate.val();
                ccObj.EndDate = self.fields.EndDate.val();

                ccObj.Notes = '';
                ccObj.Resolution = self.fields.Resolution.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                ccObj.IsSpeciallyControlled = self.fields.IsSpeciallyControlled.is(':checked');

                var fixedWorkerName = self.fields.FixedWorker.val();
                if (fixedWorkerName)
                    ccObj.FixedWorkerID = parseFloat(self.fields.FixedWorker.attr('valueid'));
                else
                    ccObj.FixedWorkerID = 0;
                ccObj.ControlResponseDate = self.fields.ControlResponseDate.val();
                ccObj.ControlResponse = self.fields.ControlResponse.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                var headResponseName = self.fields.HeadResponse.val();
                if (headResponseName)
                    ccObj.HeadResponseID = parseFloat(self.fields.HeadResponse.attr('valueid'));
                else
                    ccObj.HeadResponseID = 0;

/*
                ccObj.IsDecisionOfSession = self.fields.IsDecisionOfSession.is(':checked');
                ccObj.IsDecisionOfExecutiveCommittee = self.fields.IsDecisionOfExecutiveCommittee.is(':checked');
                ccObj.IsOrderOfHeader = self.fields.IsOrderOfHeader.is(':checked');
                ccObj.IsActionWorkPlan = self.fields.IsActionWorkPlan.is(':checked');
                ccObj.IsSendResponse = self.fields.IsSendResponse.is(':checked');
                ccObj.IsSendInterimResponse = self.fields.IsSendInterimResponse.is(':checked');
                ccObj.IsLeftToWorker = self.fields.IsLeftToWorker.is(':checked');
                ccObj.IsAcquaintedWorker = self.fields.IsAcquaintedWorker.is(':checked');
*/
                ccObj.IsContinuation = self.fields.IsContinuation;
                ccObj.InnerNumber = self.fields.InnerNumber;

                var docStatusName = self.fields.DocStatus.val();
                if (docStatusName)
                    ccObj.DocStatusID = parseFloat(self.fields.DocStatus.attr('valueid'));
                else
                    ccObj.DocStatusID = 0;
                
                var cardStatusName = self.fields.CardStatus.val();
                if (cardStatusName)
                    ccObj.CardStatusID = parseFloat(self.fields.CardStatus.attr('valueid'));
                else
                    ccObj.CardStatusID = 0;
                delete ccObj.CardStatus;
                delete ccObj.DocStatus;
                delete ccObj.ExecutiveDepartment;
                delete ccObj.Head;
                delete ccObj.Worker;
                delete ccObj.HeadResponse;
                delete ccObj.FixedWorker;
                delete ccObj.ChildrenControlCards;


                if (options.groupCards) {
                    var group = [];
                    self.fields.cardsGroup.forEach(function (r, i) {
                        var cg = {
                            ExecutiveDepartmentID: 0,
                            WorkerID: 0,
                            CardNumber: r.number,
                            InnerNumber: ''
                        };
                        if (r.card) {
                            cg.ID = r.card.ID;
                            if (r.card.InnerNumber) {
                                cg.InnerNumber = r.card.InnerNumber;
                            }
                            if (r.card.ControlResponse) {
                                cg.ControlResponse = r.card.ControlResponse;
                            }
                        }
                        if (cg.CardNumber === 1) {
                            ccObj.CardNumber = cg.CardNumber;
                            ccObj.WorkerID = cg.WorkerID;
                            ccObj.ExecutiveDepartmentID = cg.ExecutiveDepartmentID;
                        }

                        if (r.Worker.val()) {
                            cg.WorkerID = parseFloat(r.Worker.attr('valueid'));
                        }

                        if (r.ExecutiveDepartment.val()) {
                            cg.ExecutiveDepartmentID = parseFloat(r.ExecutiveDepartment.attr('valueid'));
                        }
                        var tmpCcObj = $.extend({}, ccObj, { ID:0, Group: null, CardStatus: null });
                        cg = $.extend({}, tmpCcObj, cg);

                        var existCard = null;
                        ccObj.Group && ccObj.Group.forEach(function (g) {
                            if (cg.ID === g.ID) {
                                existCard = g;
                            }
                        });
                        
                        if (existCard) {
                            delete cg.ID;
                            cg = $.extend(existCard, cg);
                        } else {
                            cg.ID = 0;
                        }

                        delete cg.Group;
                        delete cg.CardStatus;
                        delete cg.DocStatus;
                        delete cg.ChildrenControlCards;
                        group.push(cg);
                    });
                    ccObj.Group = group;
                } else {
                    var workerName = self.fields.Worker.val();
                    if (workerName) {
                        ccObj.WorkerID = parseFloat(self.fields.Worker.attr('valueid'));
                    } else {
                        ccObj.WorkerID = 0;
                    }

                    var departmentName = self.fields.ExecutiveDepartment.val();
                    if (departmentName) {
                        ccObj.ExecutiveDepartmentID = parseFloat(self.fields.ExecutiveDepartment.attr('valueid'));
                    } else {
                        ccObj.ExecutiveDepartmentID = 0;
                    }
                }

                var obj = options.groupCards ? 'cardg' : 'card';
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=' + obj + '&type=' + type + '&dep=' + userdata.departmentId;

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
                close: function (event, ui) {
                    self.dispose();
                },
                open: function (event, ui) {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },

        this.showInsertForm = function (params, onsuccess) {
            thisCc = getEmptyDocumentObject();
            thisCc.DepartmentID = userdata.departmentId;

            if (params && params.cardNumber)
                thisCc.CardNumber = params.cardNumber;
            if (params && params.documentID)
                thisCc.DocumentID = params.documentID;
            if (params && params.ParentControlCardID)
                thisCc.ParentControlCardID = params.ParentControlCardID;

            this.createForm(thisCc, 'ins', onsuccess);
            this.dialog.dialog("open");
        },

        this.showUpdateForm = function (params, onsuccess) {
            if (params) {
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=get&jdata=' + params.controlCardID + '&dep=' + userdata.departmentId,
                    dataType: "json",
                    success: function (data) {
                        thisCc = data;
                        self.createForm(data, 'upd', onsuccess);
                        self.dialog.dialog("open");
                    }
                });
            }
        },

        this.showViewForm = function (params, onsuccess) {
            if (params) {
                var obj = 'cardb';
                if (options.groupCards) {
                    obj = 'cardg';
                }
                var url = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=' + obj + '&type=get&jdata=' + params.controlCardID + '&dep=' + userdata.departmentId;
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: url,
                    dataType: "json",
                    success: function (data) {
                        thisCc = data;
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
                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cardb&type=get&jdata=' + params.controlCardID + '&dep=' + userdata.departmentId,
                    dataType: "json",
                    success: function (data) {
                        data.CardStatusID = 2;
                        data.CardStatus.ID = 2;
                        data.CardStatus.Name = 'ВІДПРАВЛЕНО НА ДООПРАЦЮВАННЯ';
                        thisCc = data;
                        self.createForm(data, 'upd', function () {
                            var newData = getEmptyDocumentObject();
                            newData.DocumentID = data.DocumentID;
                            newData.CardNumber = data.CardNumber;
                            newData.ExecutiveDepartment = data.ExecutiveDepartment;
                            newData.ExecutiveDepartmentID = data.ExecutiveDepartmentID;
                            newData.DepartmentID = userdata.departmentId;
                            newData.HeadID = data.HeadID;
                            newData.Head = data.Head;
                            newData.WorkerID = data.WorkerID;
                            newData.Worker = data.Worker;
                            newData.IsContinuation = true;

                            thisCc = newData;
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
                                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=del&jdata=' + params.controlCardID + '&dep=' + userdata.departmentId,
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

        function getEmptyDocumentObject() {
            var ccObj = {
                ID: 0,
                DocumentID: 0,

                CardNumber: 1,
                HeadID: 0,
                WorkerID: 0,
                DepartmentID: userdata.departmentId,
                ExecutiveDepartmentID: 0,

                StartDate: '',
                EndDate: '',
                Resolution: '',
                Notes: '',
                IsSpeciallyControlled: false,
                FixedWorkerID: 0,

                ControlResponseDate: '',
                ControlResponse: '',
                HeadResponseID: 0,

                DocStatusID: 0,
                CardStatusID: 1,
                CardStatus: { ID: 1, Name: 'СТОЇТЬ НА КОНТРОЛІ' },

                IsDecisionOfSession: false,
                IsDecisionOfExecutiveCommittee: false,
                IsOrderOfHeader: false,
                IsActionWorkPlan: false,
                IsSendResponse: false,
                IsSendInterimResponse: false,
                IsLeftToWorker: false,
                IsAcquaintedWorker: false,

                IsContinuation: false,
                ParentControlCardID: 0,
                InnerNumber: ''
            };

            return ccObj;
        }
    };

    window.StatementControlCardBlank = statementControlCardBlank;

})(window, jQuery);