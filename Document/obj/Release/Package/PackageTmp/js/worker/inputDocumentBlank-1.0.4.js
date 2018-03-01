(function (window) {

    var appSettings = window.appSettings;
    var inputDocumentBlank = function (options) {
        var self = this,
            testTime = new Date(1, 0, 1, 0, 0, 0);

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

        this.createForm = function (docData) {
            this.dispose();

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            //table.append(row('', 'Вхідний'));
            table.append(row('Шифр:', docData.Document.CodeName ? docData.Document.CodeID + '. ' + docData.Document.CodeName : ''));
            /*
            table.append(row('Найменування документа:', docData.DocType ? docData.DocType.Name : ''));
            table.append(row('Категорія питання:', (docData.QuestionType && docData.QuestionTypeID > 0) ? docData.QuestionType.Name : ''));
            table.append(row('&nbsp;'));
            */

            table.append(row('Адресант:'));
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Організація:', docData.Document.Source.OrganizationID > 0 ? docData.Document.Source.OrganizationName : ''));
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Підрозділ:', docData.Document.Source.DepartmentID > 0 ? docData.Document.Source.DepartmentName : ''));

            var sWorker = docData.Document.Source.Worker;
            var sWorkerName = docData.Document.Source.WorkerID > 0 ? documentUI.formatFullName(sWorker.LastName, sWorker.FirstName, sWorker.MiddleName) : '';
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Працівник:', sWorkerName));

            var ecDateStr = '';
            if (docData.Document.Source.CreationDate) {
                var ecDate = new Date(+docData.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, '$1'));
                ecDateStr = (ecDate.getDate() <= 9 ? '0' : '') + ecDate.getDate() + '.' + (ecDate.getMonth() < 9 ? 0 : '') + (ecDate.getMonth() + 1) + '.' + ecDate.getFullYear();
            }
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Дата складання:', ecDateStr));
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Номер документа:', docData.Document.Source.Number));
            table.append(row('&nbsp;'));


            
            var cDateStr = '';
            if (docData.Document.Destination.CreationDate) {
                var cDate = new Date(+docData.Document.Destination.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, '$1'));
                cDateStr = (cDate.getDate() <= 9 ? '0' : '') + cDate.getDate() + '.' + (cDate.getMonth() < 9 ? 0 : '') + (cDate.getMonth() + 1) + '.' + cDate.getFullYear();
            }
            table.append(row('Дата надходження:', cDateStr));
            table.append(row('Вхідний номер:', docData.Document.Destination.Number));
            table.append(row('&nbsp;'));


            table.append(row('Зміст документа:', docData.Content));
            
            table.append(row('Підготовив:', (docData.Worker && docData.WorkerID > 0) ? documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName) : ''));
            table.append(row('За підписом:', (docData.Head && docData.HeadID > 0) ? documentUI.formatFullName(docData.Head.LastName, docData.Head.FirstName, docData.Head.MiddleName) : ''));
            


            self.uploadButton = $('<div></div>');
            table.append(row('Прикріплені файли:', self.uploadButton));
            recreateFileUploader();
            /*
            var files = docData.Document.Files;
            var uploadButton = $('<div></div>');
            table.append(row('Прикріплені файли:', uploadButton));
            var uploadList = $('<ul class="qq-upload-list"></ul>').appendTo(uploadButton);
            for (var i in files) {
                var fileUrl = appSettings.rootUrl + 'File.ashx?id=' + files[i].FileID,
                openUrl = appSettings.rootUrl + 'ImageViewer.aspx?documentID=' + docData.DocumentID + '&?id=' + files[i].FileID;
                uploadList.append('<li><a target="_blank" href="' + fileUrl + '">' + files[i].FileName + '</a> <a target="_blank" href="' + openUrl + '">Перегляд</a></li>');
            }
            */

            table.append(row('&nbsp;'));
            table.append(row('Виконавці:'));
            docData.ControlCards.forEach(function (cc) {
                var foreignCard = (userdata.departmentId !== cc.ExecutiveDepartmentID),
                    ccRow = window.formParts.cardBlock({ card: cc, cssClass: (foreignCard ? 'bg-gray' : ''), resolutionVisibility: false, documentBlank: self });
                table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>').append(ccRow)));
            });
            table.append(row('&nbsp;'));

            var cards = docData.ControlCards,
                parentControlCardId = 0,
                lastChildrenControlCardNumber = 0,
                headId = userdata.worker.ID,
                accessCard = null;
            if (cards.length > 0) {
                var rCard = cards[0];
                cards.forEach(function(cc) {
                    if (cc.WorkerID === userdata.worker.ID) {
                        rCard = cc;
                    }
                });
                accessCard = rCard;
                parentControlCardId = rCard.ID;
                //headId = rCard.HeadID;
                var head = rCard.Head;
                table.append(row('Накладаючий резолюцію:', (head && rCard.HeadID > 0) ? documentUI.formatFullName(head.LastName, head.FirstName, head.MiddleName) : ''));
                table.append(row('Резолюція:', rCard.Resolution));
                
                table.append(row('&nbsp;'));

                this.buttons.buttonResponse = $('<input type="button" value="Відповісти">').button().click(function (e) {

                    //rCard.ControlResponseDate = self.fields.ControlResponseDate.val();
                    rCard.ControlResponse = self.fields.ControlResponse.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, ' \n').trim();

                    var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                    $.ajax({
                        url: urlRequest,
                        type: 'POST',
                        cache: false,
                        data: { 'jdata': JSON.stringify(rCard) },
                        dataType: 'json',
                        success: function (msg) {
                            self.dialog.dialog('close');
                        },
                        error: function (xhr, status, error) {
                            alert(xhr.responseText);
                        }
                    });
                });
                this.fields.ControlResponse = $('<textarea style="height:76px;width:98%;font-size:15px;" cols="81" rows="4"></textarea>').val(rCard.ControlResponse);
                table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                        .append('Контрольна відповідь:').append(this.fields.ControlResponse.add(this.buttons.buttonResponse))));


                table.append(row('Виконавці:'));
                var ccc = rCard.ChildrenControlCards;
                if (ccc.length > 0) {
                    lastChildrenControlCardNumber = ccc[ccc.length - 1].CardNumber;
                    for (var v = 0; v < ccc.length; v++) {
                        var c = ccc[v];
                        var worker = documentUI.formatFullName(c.Worker.LastName, c.Worker.FirstName, c.Worker.MiddleName);
                        createWorkerRow({
                            ID: c.ID,
                            CardNumber: c.CardNumber,
                            ControlResponse: c.ControlResponse,
                            ControlResponseDate: c.ControlResponseDate,
                            worker: worker
                        });
                    }
                }
            }

            function createWorkerRow(p) {
                var resp = $('<span></span>');
                var rDate = new Date(+p.ControlResponseDate.replace(/\/Date\((-?\d+)\)\//gi, '$1'));
                if (p.ControlResponse || rDate > testTime) {
                    resp.append('Відповідь ');
                    if (rDate > testTime) {
                        var controlResponseDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                        controlResponseDate.datepicker('setDate', rDate);
                        resp.append(controlResponseDate);
                    }
                    if (p.ControlResponse) {
                        resp.append('<br>').append($('<span></span>').text(p.ControlResponse));
                    }
                }
                $('<button type="button" title="Видалити" style="margin-left: 20px;"></button>').attr('tabIndex', -1).attr('controlCardId', p.ID)
                    .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                    .click(function () {
                        $.ajax({
                            type: 'GET',
                            cache: false,
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=del&jdata=' + $(this).attr('controlCardId') + '&dep=' + userdata.departmentId,
                            dataType: 'json',
                            success: function (data) {
                                resp.parents('tr').remove();
                            }
                        });
                    }).appendTo(resp);

                table.append(row(p.CardNumber + '. ' + p.worker, resp));
            }
            

            function recreateFileUploader() {
                var uploadButton = $('<div></div>');
                self.uploadButton.replaceWith(uploadButton);
                self.uploadButton = uploadButton;

                var uploadedList = [];
                for (var i in docData.Document.Files) {
                    var f = docData.Document.Files[i];
                    uploadedList.push({
                        id: f.FileID,
                        name: f.FileName,
                        removable: f.DepartmentID === userdata.departmentId
                    });
                }
                var uploader = new qq.FileUploader({
                    element: self.uploadButton[0],
                    action: appSettings.rootUrl + 'Uploader.ashx?documentid=' + docData.DocumentID,
                    fileUrl: appSettings.rootUrl + 'File.ashx?id=',
                    openUrl: appSettings.rootUrl + 'ImageViewer.aspx?documentID=' + docData.DocumentID + '&?id=',
                    debug: true,
                    uploadedList: uploadedList,
                    onComplete: function (id, fileName, response) {
                        docData.Document.Files.push({ DocumentID: docData.DocumentID, FileID: response.fileID, FileName: fileName });
                    },
                    onRemove: function (id, file) {
                        if (confirm('Ви дійсно бажаєте видалити файл ' + file.name + ' ?')) {

                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=del&dep=' + documentUI.departmentID + '&fileID=' + file.id + '&documentid=' + docData.DocumentID,
                                type: 'GET',
                                dataType: 'json',
                                success: function () {
                                    for (var j in docData.Document.Files)
                                        if (docData.Document.Files[j].FileID == file.id)
                                            docData.Document.Files.splice(j, 1);
                                },
                                error: function (responce) {
                                    window.alert(responce.responseText);
                                }
                            });
                            return true;
                        } else {
                            return false;
                        }
                    }
                });
            }

            this.buttons.buttonCreateCards = $('<input type="button" value="Додати виконавців">').button().click(function (e) {
                createCards();
            });
            /*
            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreateCards)));
            this.form = $('<div title="Документ" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            $('body').append(this.form);
            */
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

            function createCards() {
                var cardTable = $('<table></table>');
                var cardBlank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(cardTable);


                var defaultStartDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
                var defaultEndDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 2);
                
                var txtStartDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true }).datepicker('setDate', defaultStartDate);
                var txtEndDate = $('<input type="text">').datepicker({ changeMonth: true, changeYear: true }).datepicker('setDate', defaultEndDate);
                cardTable.append(row('Термін виконання з', txtStartDate.add($('<span> по: </span>').add(txtEndDate))));


                var txtResolution = $('<textarea style="height:95px;width:98%;font-size:15px;" cols="81" rows="5"></textarea>');
                var resolutions = $('<select style="width:98%;font-size:13px !important;"><option></option></select>').on('change', function () {
                    if ($(this).val()) {
                        txtResolution.val($(this).val());
                    }
                });
                window.appData.resolutions.forEach(function (r) {
                    resolutions.append('<option value="' + r + '">' + r + '</option>');
                });
                cardTable.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                        .append('Текст резолюції:').append(resolutions).append(txtResolution)));
                
                addWorkerField();

                function addWorkerField() {

                    var txtWorker = $('<input class="worker-todo" type="text" valueid="0" style="width: 300px;">')
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

                                addWorkerField();
                            }
                        }).addClass('ui-widget ui-widget-content ui-corner-left');

                    cardTable.append(row('Виконавець:', txtWorker.add(documentUI.createButtonForAutocomplete(txtWorker))
                        .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                            .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                            .click(function() {
                                txtWorker.attr('valueid', 0).val('');
                                txtWorker.parents('tr').remove();
                            })
                        )));
                }

                var btnAddCards = $('<input type="button" value="Додати">').button().click(function (e) {
                    var cardNumber = lastChildrenControlCardNumber + 1,
                        workers = $('input.worker-todo[valueid!=0]'),
                        workerIndex = 0;
                    if (workers.length > 0) {
                        addCard();
                    }
                    function addCard() {
                        var workerId = parseFloat($(workers[workerIndex]).attr('valueid')),
                            workerName = $(workers[workerIndex]).val();
                        if (workerId > 0) {
                            var ccObj = models.ControlCard();
                            ccObj.DocumentID = docData.DocumentID;
                            ccObj.HeadID = headId;
                            ccObj.WorkerID = ccObj.FixedWorkerID = workerId;
                            ccObj.DepartmentID = userdata.departmentId;
                            ccObj.ExecutiveDepartmentID = userdata.departmentId;
                            ccObj.Resolution = txtResolution.val().replace('{виконавець}', workerName).replace(/(\s*\r\n|\s*\n|\s*\r)/gm, ' \n').trim();

                            ccObj.StartDate = txtStartDate.val();
                            ccObj.EndDate = txtEndDate.val();
                            ccObj.ParentControlCardID = parentControlCardId;
                            ccObj.CardNumber = cardNumber;
                            cardNumber++;

                            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=ins' + '&dep=' + userdata.departmentId;
                            $.ajax({
                                url: urlRequest,
                                type: 'POST',
                                cache: false,
                                data: { 'jdata': JSON.stringify(ccObj) },
                                dataType: 'json',
                                success: function (msg) {
                                    workerIndex++;
                                    lastChildrenControlCardNumber = msg.Data.CardNumber;
                                    var delCard = $('<button type="button" title="Видалити" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                                        .click(function () {
                                            $.ajax({
                                                type: 'GET',
                                                cache: false,
                                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=del&jdata=' + msg.Data.ID + '&dep=' + userdata.departmentId,
                                                dataType: 'json',
                                                success: function (data) {
                                                    delCard.parents('tr').remove();
                                                }
                                            });
                                        });

                                    table.append(row(ccObj.CardNumber + '. ' + workerName, delCard));
                                    
                                    if (workers.length > workerIndex) {
                                        addCard();
                                    } else {
                                        $(cardForm).dialog('close');
                                    }
                                },
                                error: function (xhr, status, error) {
                                    alert(xhr.responseText);
                                }
                            });
                        }
                    }
                });
                var btnCancel = $('<input type="button" value="Відмінити">').button().click(function (e) {
                    $(cardForm).dialog('close');
                });

                var cardActions = $('<table style="width:100%;"></table>')
                .append($('<tr></tr>').append($('<td align="center"></td>').append(btnAddCards).append(btnCancel)));
                var cardForm = $('<div title="Виконавці" style="display:none;"></div>').append(cardBlank).append(cardActions);
                $('body').append(cardForm);

                $(cardForm).dialog({
                    autoOpen: true,
                    draggable: true,
                    modal: true,
                    position: ['top'],
                    resizable: true,
                    width: 660,
                    close: function () {
                        cardForm.remove();
                    },
                    open: function () {
                        $('.ui-widget-overlay').css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                    }
                });
            }
        },

        this.showViewForm = function (documentId) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getblank&jdata=' + documentId;

            $.ajax({
                type: 'GET',
                cache: false,
                url: urlRequest,
                dataType: 'json',
                success: function (data) {
                    self.createForm(data, 'upd');
                    self.dialog.dialog('open');
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

    window.InputDocumentBlank = inputDocumentBlank;

})(window);