(function (window, $, undefined) {

    var appSettings = window.appSettings;
    var workerStatementBlank = function (options) {
        var self = this,
            thisDoc = undefined,
            testTime = new Date(1, 0, 1, 0, 0, 0);

        var o = {
            dictionaries: {}
        },
        autocompleteButton = function (elem) {
            return $('<button type="button" class="ui-button-call-list">&nbsp;</button>').attr("tabIndex", -1)
                    .button({ icons: { primary: 'ui-icon-triangle-1-s' }, text: false })
                    .removeClass("ui-corner-all")
                    .addClass("ui-corner-right ui-button-icon")
                    .click(function () {
                        // close if already visible
                        if ($(elem).autocomplete("widget").is(":visible")) {
                            $(elem).autocomplete("close");
                            return;
                        }
                        // work around a bug (likely same cause as #5265)
                        $(this).blur();
                        // pass empty string as value to search for, displaying all results
                        $(elem).autocomplete("search", "");
                        $(elem).focus();
                    });
        },
        formatFullName = function (sName, fName, tName) {
            var fullName = sName || "";

            if (fName) {
                fullName = fullName + " " + fName.substr(0, 1) + ".";
                if (tName)
                    fullName = fullName + " " + tName.substr(0, 1) + ".";
            }

            return fullName;
        };

        this.document = {};
        this.departmentID = 0;
        this.templateID = 0;

        $.extend(o, options);

        if (options) {
            if (options.departmentID)
                this.departmentID = options.departmentID;
            if (options.templateID)
                this.templateID = options.templateID;
        }


        this.blank = null,
        this.fields = {},
        this.buttons = {},
        this.actionPanel = null,

        this.createForm = function (ops) {
            this.dispose();
            var docData = ops.docData;

            thisDoc = getEmptyDocumentObject();

            if (docData) {
                thisDoc = $.extend(true, thisDoc, docData);
            }

            var appendTo = ops.appendTo || 'body',
                fields = ops.fields || {},
                dicts = o.dictionaries;
            var isMiniMode = ($(window).height() < 750);

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title"></td>').append(name)).append($('<td></td>').append(obj));
            },
            comboBox = function (prms) {
                var cb = $('<select><option value="0"></option></select>').attr('disabled', 'disabled');
                if (prms.items) {
                    var items = prms.items;
                    for (var it in items)
                        cb.append('<option value="' + items[it].id + '" ' + (items[it].id == prms.selected ? 'selected="selected"' : '') + '>' + items[it].name + '</option>');
                }
                return cb;
            };

            var table = $('<table style="border: 1px dashed #C0C0C0; font-weight: bold;"></table>');
            var tableN1 = $('<table></table>');
            //var tableN2 = $('<table style="border-left-style: dashed; border-left-width: 1px; border-left-color: #C0C0C0;"></table>');

            this.fields.IsReception = thisDoc.IsReception;
            this.fields.ID = thisDoc.ID;
            this.fields.DocumentID = thisDoc.DocumentID;

            this.fields.Document = {};
            this.fields.Document.ID = thisDoc.Document.ID;
            this.fields.Document.CodeID = thisDoc.Document.CodeID;
            this.fields.Document.DocStatusID = thisDoc.Document.DocStatusID;
            this.fields.Document.Files = thisDoc.Document.Files;
            this.fields.Document.Notes = thisDoc.Document.Notes;
            this.fields.Document.DepartmentID = thisDoc.Document.DepartmentID;

            this.fields.Document.Number = $('<input type="text" style="width: 138px;">').val(thisDoc.Document.Number).attr('disabled', 'disabled');
            tableN1.append(row('Вхідний номер:', this.fields.Document.Number));

            this.fields.Document.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
            if (thisDoc.Document.CreationDate) {
                var creationDate = new Date(+thisDoc.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                this.fields.Document.CreationDate.datepicker("setDate", creationDate);
            }
            tableN1.append(row('Дата надходження:', this.fields.Document.CreationDate));

            this.fields.Head = $('<input type="text" valueid="0" style="width: 138px;">')
                .val(thisDoc.Head ? formatFullName(thisDoc.Head.LastName, thisDoc.Head.FirstName, thisDoc.Head.MiddleName) : "").attr('disabled', 'disabled');
            var headRow = row('Керівник прийому:', this.fields.Head);
            tableN1.append(headRow);
            if (fields.Head != undefined && fields.Head.hidden)
                headRow.hide();

            var triggerBc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
            var branches = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute;" class="ui-state-default"></div>');
            if (dicts.branchTypes) {
                var btTable = $('<table></table>');
                var bts = dicts.branchTypes;
                for (var bt in bts) {
                    if ($.inArray(bts[bt].id, thisDoc.Branches) > -1) {
                        btTable.append('<tr><td><label><input type="checkbox" value="' + bts[bt].id + '" checked="checked" disabled="disabled"/>' + bts[bt].name + '</label></td></tr>');
                    }
                }
                branches.append(btTable);
            }
            var branchesContainer = $('<div></div>')
                .append($('<div></div>').append(triggerBc)
                .click(function () {
                    if (branches.is(':visible')) {
                        triggerBc.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all");
                        triggerBc.find('span').removeClass("ui-icon-circle-arrow-s").addClass("ui-icon-circle-arrow-e");
                        triggerBc.find('a').text('Розгорнути');
                        branches.hide();
                    }
                    else {
                        triggerBc.removeClass("ui-state-default ui-corner-all").addClass("ui-state-active ui-corner-top");
                        triggerBc.find('span').removeClass("ui-icon-circle-arrow-e").addClass("ui-icon-circle-arrow-s");
                        triggerBc.find('a').text('Згорнути');
                        branches.show();
                    }
                }))
                .append(branches);
            this.fields.Branches = branches;
            tableN1.append(row('Галузі заяви:', branchesContainer));

            /*
            this.fields.InputDocType = comboBox({ items: dicts.inputDocTypes, selected: thisDoc.InputDocTypeID });
            tableN2.append(row('Вид звернення:', this.fields.InputDocType));

            this.fields.InputMethod = comboBox({ items: dicts.inputMethods, selected: thisDoc.InputMethodID });
            tableN2.append(row('Тип звернення:', this.fields.InputMethod));

            this.fields.InputSubjectType = comboBox({ items: dicts.inputSubjectTypes, selected: thisDoc.InputSubjectTypeID });
            tableN2.append(row("Суб'єкт:", this.fields.InputSubjectType));

            this.fields.InputSign = comboBox({ items: dicts.inputSigns, selected: thisDoc.InputSignID });
            tableN2.append(row('Ознака надходження:', this.fields.InputSign));

            this.fields.DeliveryType = comboBox({ items: dicts.deliveryTypes, selected: thisDoc.DeliveryTypeID });
            tableN2.append(row('Тип надходження:', this.fields.DeliveryType));
            */
            table.append($('<tr></tr>').append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN1)).append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>')/*.append(tableN2)*/));


            this.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>').attr('disabled', 'disabled')
                .height((isMiniMode ? 38 : 38))
                .attr('rows', (isMiniMode ? '2' : '2'))
                .val(thisDoc.Content);
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append('Короткий зміст документа:').append(this.fields.Content)));

            /*--------------------------------------------------------*/
            if (thisDoc.Document.Source && thisDoc.Document.Source.OrganizationID > 0) {
                var tableN3 = $('<table></table>');

                this.fields.Document.Source = {};
                this.fields.Document.Source.ID = thisDoc.Document.Source ? thisDoc.Document.Source.ID : 0;
                this.fields.Document.Source.Organization = $('<input type="text" style="width: 300px;">').val(thisDoc.Document.Source ? thisDoc.Document.Source.OrganizationName : '').attr('disabled', 'disabled');

                tableN3.append(row('Організація:', this.fields.Document.Source.Organization));

                this.fields.Document.Source.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                if (thisDoc.Document.Source && thisDoc.Document.Source.CreationDate) {
                    var externalCreationDate = new Date(+thisDoc.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    if (externalCreationDate > testTime) {
                        this.fields.Document.Source.CreationDate.datepicker("setDate", externalCreationDate);
                    }
                }
                tableN3.append(row('Дата складання документа:', this.fields.Document.Source.CreationDate));

                this.fields.Document.Source.Number = $('<input type="text" style="width: 138px;">').val(thisDoc.Document.Source ? thisDoc.Document.Source.Number : '').attr('disabled', 'disabled');
                tableN3.append(row('Номер документа в організації:', this.fields.Document.Source.Number));

                this.fields.IsNeedAnswer = $('<input type="checkbox">').attr('checked', thisDoc.IsNeedAnswer).attr('disabled', 'disabled');
                tableN3.append(row('Потреба у відповіді:', this.fields.IsNeedAnswer));

                if (fields.Source != undefined && fields.Source.hidden)
                    tableN3.hide();

                table.append($('<tr></tr>').append($('<td colspan="2" style="background-color: #EFEFEF; border-bottom: 1px dashed #C0C0C0; border-top: 1px dashed #C0C0C0;"></td>').append(tableN3)));
            }

            /*--------------------------------------------------------*/
            this.fields.CitizenID = thisDoc.CitizenID;
            this.fields.Citizen = {};
            this.fields.Citizen.ID = thisDoc.Document.Source.Citizen.ID;
            var tableN4 = $('<table></table>');
            var tableN4Row = $('<tr></tr>').appendTo(tableN4);

            tableN4Row.append('<td style="width: 94px;">Прізвище:</td>');
            this.fields.Citizen.LastName = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.LastName : '').attr('disabled', 'disabled');
            tableN4Row.append($('<td></td>').append(this.fields.Citizen.LastName));

            tableN4Row.append('<td style="padding-left: 10px;">Ім\'я:</td>');
            this.fields.Citizen.FirstName = $('<input type="text" style="width: 100px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.FirstName : '').attr('disabled', 'disabled');
            tableN4Row.append($('<td></td>').append(this.fields.Citizen.FirstName));

            tableN4Row.append('<td style="padding-left: 10px;">По батькові:</td>');
            this.fields.Citizen.MiddleName = $('<input type="text" style="width: 100px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.MiddleName : '').attr('disabled', 'disabled');
            tableN4Row.append($('<td></td>').append(this.fields.Citizen.MiddleName));

            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN4)));
            /*--------------------------------------------------------*/

            var tableN5 = $('<table></table>');
            var tableN5Row = $('<tr></tr>').appendTo(tableN5);

            tableN5Row.append('<td style="width: 94px;">Адреса:</td>');
            this.fields.Citizen.Address = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Address : '').attr('disabled', 'disabled');
            tableN5Row.append($('<td></td>').append(this.fields.Citizen.Address));

            tableN5Row.append('<td style="padding-left: 10px;">Телефон:</td>');
            this.fields.Citizen.PhoneNumber = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.PhoneNumber : '').attr('disabled', 'disabled');
            tableN5Row.append($('<td></td>').append(this.fields.Citizen.PhoneNumber));

            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN5)));
            /*--------------------------------------------------------*/

            var tableN6 = $('<table></table>');
            var tableN6Row = $('<tr></tr>').appendTo(tableN6);

            this.fields.Citizen.CityObjectTypeShortName = $('<span style="padding-left: 67px;">вул.</span>').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.CityObjectTypeShortName : '').attr('disabled', 'disabled');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.CityObjectTypeShortName));
            this.fields.Citizen.CityObject = $('<input type="text" style="width: 204px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.CityObjectName : '').attr('disabled', 'disabled');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.CityObject));

            tableN6Row.append('<td style="padding-left: 10px;">буд.</td>');
            this.fields.Citizen.HouseNumber = $('<input type="text" style="width: 30px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.HouseNumber : '').attr('disabled', 'disabled');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.HouseNumber));

            tableN6Row.append('<td style="padding-left: 10px;">корп.</td>');
            this.fields.Citizen.Corps = $('<input type="text" style="width: 30px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Corps : '').attr('disabled', 'disabled');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.Corps));

            tableN6Row.append('<td style="padding-left: 10px;">кв.</td>');
            this.fields.Citizen.ApartmentNumber = $('<input type="text" style="width: 60px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.ApartmentNumber : '').attr('disabled', 'disabled');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.ApartmentNumber));

            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN6)));
            /*--------------------------------------------------------*/

            if (thisDoc.Document.Source.Citizen && thisDoc.Document.Source.Citizen.Work) {
                this.fields.Citizen.Work = $('<input type="text" style="width: 468px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Work : '').attr('disabled', 'disabled');
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Місце роботи:</td></tr>').append($('<td></td>').append(this.fields.Citizen.Work))))));
            }
            /*
            this.fields.Citizen.Sex = $('<select><option value="0">Не визанчено</option><option value="1">Чоловіча</option><option value="2">Жіноча</option></select>').attr('disabled', 'disabled')
                .val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Sex : 0);
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                .append($('<tr><td style="width: 94px;">Стать:</td></tr>').append($('<td></td>').append(this.fields.Citizen.Sex))))));
            */
            /*--------------------------------------------------------*/

            this.fields.Citizen.SocialStatus = comboBox({ items: dicts.socialStatuses, selected: thisDoc.Document.Source.Citizen.SocialStatusID });
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                .append($('<tr><td style="width: 94px;">Соц. статус:</td></tr>').append($('<td></td>').append(this.fields.Citizen.SocialStatus))))));


            var triggerSoc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
            var socialCategories = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px;" class="ui-state-default"></div>');
            if (dicts.socialCategories) {
                var scTable = $('<table></table>');
                var scs = dicts.socialCategories;
                for (var cs in scs) {
                    if ($.inArray(scs[cs].id, thisDoc.Document.Source.Citizen.SocialCategories) > -1) {
                        scTable.append('<tr><td><label><input type="checkbox" value="' + scs[cs].id + '" checked="checked" disabled="disabled"/>' + scs[cs].name + '</label></td></tr>');
                    }
                }
                socialCategories.append(scTable);
            }
            var socialCategoriesContainer = $('<div></div>')
                .append($('<div></div>').append(triggerSoc)
                .click(function () {
                    if (socialCategories.is(':visible')) {
                        triggerSoc.removeClass("ui-state-active ui-corner-top").addClass("ui-state-default ui-corner-all")
                        .children(".ui-icon").removeClass("ui-icon-circle-arrow-s").addClass("ui-icon-circle-arrow-e");
                        triggerSoc.find('a').text('Розгорнути');
                        socialCategories.hide();
                    }
                    else {
                        triggerSoc.removeClass("ui-state-default ui-corner-all").addClass("ui-state-active ui-corner-top")
                        .children(".ui-icon").removeClass("ui-icon-circle-arrow-e").addClass("ui-icon-circle-arrow-s");
                        triggerSoc.find('a').text('Згорнути');
                        socialCategories.show();
                    }
                }))
                .append(socialCategories);
            this.fields.Citizen.SocialCategories = socialCategories;
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                .append($('<tr><td style="width: 94px;">Соц. категорії:</td></tr>').append($('<td></td>').append(socialCategoriesContainer))))));



            self.fields.uploadButton = $('<div></div>');
            table.append(row('Прикріплені файли:', self.fields.uploadButton));
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

            function recreateFileUploader() {
                var uploadButton = $('<div></div>');
                self.fields.uploadButton.replaceWith(uploadButton);
                self.fields.uploadButton = uploadButton;

                var uploadedList = [];
                for (var i in thisDoc.Document.Files) {
                    var f = thisDoc.Document.Files[i];
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
                    onComplete: function (id, fileName, response) {
                        thisDoc.Document.Files.push({ DocumentID: thisDoc.DocumentID, FileID: response.fileID, FileName: fileName });
                    },
                    onRemove: function (id, file) {
                        if (confirm('Ви дійсно бажаєте видалити файл ' + file.name + ' ?')) {

                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=del&dep=' + documentUI.departmentID + '&fileID=' + file.id + '&documentid=' + thisDoc.DocumentID,
                                type: 'GET',
                                dataType: 'json',
                                success: function () {
                                    for (var j in thisDoc.Document.Files)
                                        if (thisDoc.Document.Files[j].FileID == file.id)
                                            thisDoc.Document.Files.splice(j, 1);
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

            table.append(row('&nbsp;'));
            docData.ControlCards.forEach(function (cc) {
                var foreignCard = (userdata.departmentId !== cc.ExecutiveDepartmentID),
                    ccRow = window.formParts.cardBlock({ card: cc, cssClass: (foreignCard ? 'bg-gray' : ''), resolutionVisibility: false, documentBlank: self });
                table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>').append(ccRow)));
            });
            table.append(row('&nbsp;'));

            var cards = docData.ControlCards,
                parentControlCardId = 0,
                lastChildrenControlCardNumber = 0,
                headId = userdata.worker.ID;
            if (cards.length > 0) {
                var rCard = cards[0];
                cards.forEach(function (cc) {
                    if (cc.WorkerID === userdata.worker.ID) {
                        rCard = cc;
                    }
                });
                parentControlCardId = rCard.ID;
                //headId = rCard.HeadID;
                var head = rCard.Head;
                table.append(row('Накладаючий резолюцію:', (head && rCard.HeadID > 0) ? documentUI.formatFullName(head.LastName, head.FirstName, head.MiddleName) : ''));
                table.append(row('Резолюція:', rCard.Resolution));

                table.append(row('&nbsp;'));

                this.buttons.buttonResponse = $('<input type="button" value="Відповісти">').button().click(function (e) {

                    //rCard.ControlResponseDate = self.fields.ControlResponseDate.val();
                    rCard.ControlResponse = self.fields.ControlResponse.val().replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                    var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                    $.ajax({
                        url: urlRequest,
                        type: "POST",
                        cache: false,
                        data: { 'jdata': JSON.stringify(rCard) },
                        dataType: "json",
                        success: function (msg) {
                            //self.dialog.dialog("close");
                            if (ops.onCancel instanceof Function) {
                                ops.onCancel();
                            }
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
                        var worker = c.WorkerID > 0 ? documentUI.formatFullName(c.Worker.LastName, c.Worker.FirstName, c.Worker.MiddleName) : '';
                        createWorkerRow({
                            ID: c.ID,
                            CardNumber: c.CardNumber,
                            ControlResponse: c.ControlResponse,
                            ControlResponseDate: c.ControlResponseDate,
                            worker: worker
                        });
                    }
                }

                cards.forEach(function (cc) {
                    if (cc.WorkerID !== userdata.worker.ID && cc.DepartmentID === userdata.departmentId) {
                        var worker = cc.WorkerID > 0 ? documentUI.formatFullName(cc.Worker.LastName, cc.Worker.FirstName, cc.Worker.MiddleName) : '';
                        createWorkerRow({
                            ID: cc.ID,
                            CardNumber: cc.CardNumber,
                            ControlResponse: cc.ControlResponse,
                            ControlResponseDate: cc.ControlResponseDate,
                            worker: worker
                        });
                    }
                });
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

                table.append(row(p.CardNumber + '. ' + p.worker, resp));
            }

            this.buttons.buttonCreateCards = $('<input type="button" value="Додати виконавців">').button().click(function (e) {
                createCards();
            });


            //Buttons
            /*
            this.buttons.buttonCreate = $('<input type="button" value="Зберегти">').button().click(function () {
                sendDocData({ close: true });
                return false;
            });

            this.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                if (ops.onCancel instanceof Function) {
                    ops.onCancel();
                }
                return false;
            });
            */

            /*
            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>')
                .append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreateCards)));
            this.form = $('<div title="Документ" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            $('body').append(this.form);
            */


            this.blank = $('<div class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>')
                .append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreateCards)));

            //this.form = $('<div title="Документ" style="display:none;"></div>').append(this.blank).append(this.actionPanel);
            //$('body').append(this.form);
            this.blank.append(this.actionPanel);

            this.blank.appendTo(appendTo);


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

                                addWorkerField();
                            }
                        }).addClass("ui-widget ui-widget-content ui-corner-left");

                    cardTable.append(row('Виконавець:', txtWorker.add(documentUI.createButtonForAutocomplete(txtWorker))
                        .add($('<button type="button" title="Очистити поле" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                            .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                            .click(function () {
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
                            ccObj.Resolution = txtResolution.val().replace('{виконавець}', workerName).replace(/(\s*\r\n|\s*\n|\s*\r)/gm, " \n").trim();

                            ccObj.StartDate = txtStartDate.val();
                            ccObj.EndDate = txtEndDate.val();
                            //ccObj.ParentControlCardID = parentControlCardId;
                            ccObj.CardNumber = cardNumber;
                            cardNumber++;

                            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=ins' + '&dep=' + userdata.departmentId;
                            $.ajax({
                                url: urlRequest,
                                type: "POST",
                                cache: false,
                                data: { 'jdata': JSON.stringify(ccObj) },
                                dataType: "json",
                                success: function (msg) {
                                    workerIndex++;
                                    lastChildrenControlCardNumber = msg.Data.CardNumber;
                                    var delCard = $('<button type="button" title="Видалити" style="margin-left: 20px;"></button>').attr('tabIndex', -1)
                                        .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                                        .click(function () {
                                            $.ajax({
                                                type: "GET",
                                                cache: false,
                                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=del&jdata=' + msg.Data.ID + '&dep=' + userdata.departmentId,
                                                dataType: "json",
                                                success: function (data) {
                                                    delCard.parents('tr').remove();
                                                }
                                            });
                                        });

                                    table.append(row(ccObj.CardNumber + '. ' + workerName, delCard));

                                    if (workers.length > workerIndex) {
                                        addCard();
                                    } else {
                                        $(cardForm).dialog("close");
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
                    $(cardForm).dialog("close");
                });

                var cardActions = $('<table style="width:100%;"></table>')
                .append($('<tr></tr>').append($('<td align="center"></td>').append(btnAddCards).append(btnCancel)));
                var cardForm = $('<div title="Виконавці" style="display:none;"></div>').append(cardBlank).append(cardActions);
                $('body').append(cardForm);

                $(cardForm).dialog({
                    autoOpen: true,
                    draggable: true,
                    modal: true,
                    position: ["top"],
                    resizable: true,
                    width: 660,
                    close: function () {
                        cardForm.remove();
                    },
                    open: function () {
                        $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                    }
                });
            }

            setFields(docData);
            initForm(docData);

            function initForm(formData) {

            }

            function getDocData() {
                var docObj = thisDoc;

                docObj.IsReception = self.fields.IsReception;
                docObj.ID = self.fields.ID;
                docObj.DocumentID = self.fields.DocumentID;
                docObj.Document.ID = self.fields.Document.ID;
                docObj.Document.DepartmentID = self.departmentID;
                docObj.Document.DocStatusID = self.fields.Document.DocStatusID;
                docObj.Document.CodeID = self.fields.Document.CodeID;

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

                docObj.Document.Files = self.fields.Document.Files;
                docObj.Document.Notes = self.fields.Document.Notes;
                docObj.Document.Number = self.fields.Document.Number.val();

                docObj.Document.Source.ID = self.fields.Document.Source.ID;

                docObj.Document.Source.OrganizationName = self.fields.Document.Source.Organization.val();
                if (docObj.Document.Source.OrganizationName)
                    docObj.Document.Source.OrganizationID = parseFloat(self.fields.Document.Source.Organization.attr('valueid'));
                else
                    docObj.Document.Source.OrganizationID = 0;

                docObj.Document.Source.CreationDate = self.fields.Document.Source.CreationDate.val();
                docObj.Document.Source.Number = self.fields.Document.Source.Number.val();
                docObj.Document.Source.CitizenID = self.fields.CitizenID;

                docObj.Document.Destination.CreationDate = docObj.Document.CreationDate;
                docObj.Document.Destination.DepartmentID = userdata.departmentId;

                var checkedBranches = [];
                self.fields.Branches.find('input:checked').each(function () { checkedBranches.push($(this).val()); });
                docObj.Branches = checkedBranches;

                docObj.Content = self.fields.Content.val().replace(/(\r\n|\n|\r)/gm, " ").replace(/\s+/g, " ").trim();

                docObj.HeadName = self.fields.Head.val();
                if (docObj.HeadName)
                    docObj.HeadID = parseFloat(self.fields.Head.attr('valueid'));
                else
                    docObj.HeadID = 0;


                docObj.DeliveryTypeID = self.fields.DeliveryType.val();
                docObj.InputDocTypeID = self.fields.InputDocType.val();
                docObj.InputMethodID = self.fields.InputMethod.val();
                docObj.InputSignID = self.fields.InputSign.val();
                docObj.InputSubjectTypeID = self.fields.InputSubjectType.val();
                docObj.IsNeedAnswer = self.fields.IsNeedAnswer.is(':checked');

                docObj.CitizenID = self.fields.CitizenID;
                docObj.Citizen = {};
                docObj.Citizen.ID = self.fields.Citizen.ID;
                docObj.Citizen.Address = self.fields.Citizen.Address.val();
                docObj.Citizen.PhoneNumber = self.fields.Citizen.PhoneNumber.val();
                docObj.Citizen.ApartmentNumber = self.fields.Citizen.ApartmentNumber.val();
                docObj.Citizen.CityObjectID = self.fields.Citizen.CityObject.attr('valueid');
                docObj.Citizen.Corps = self.fields.Citizen.Corps.val();
                docObj.Citizen.HouseNumber = self.fields.Citizen.HouseNumber.val();
                docObj.Citizen.LastName = self.fields.Citizen.LastName.val();
                docObj.Citizen.FirstName = self.fields.Citizen.FirstName.val();
                docObj.Citizen.MiddleName = self.fields.Citizen.MiddleName.val();
                docObj.Citizen.Sex = self.fields.Citizen.Sex.val();
                docObj.Citizen.SocialStatusID = self.fields.Citizen.SocialStatus.val();
                docObj.Citizen.Work = self.fields.Citizen.Work.val();


                docObj.Document.TemplateId = self.fields.IsReception ? 2 : 1;

                var checkedSocialCategories = [];
                self.fields.Citizen.SocialCategories.find('input:checked').each(function () { checkedSocialCategories.push($(this).val()); });
                docObj.Citizen.SocialCategories = checkedSocialCategories;

                return docObj;
            }

            function setDocData(data) {
                thisDoc = data;
            }

            function setFields(data) {

            }

            function sendDocData(p) {
                var docObj = getDocData();
                if (ops.onSave instanceof Function) {
                    ops.onSave(docObj);
                }

                return false;
            }

            return this.blank;
        },

        this.dispose = function () {
            if (this.blank) {
                this.blank.remove();
            }
        };


        function getEmptyDocumentObject() {
            var docObj = models.DocStatement();
            docObj.TemplateId = 3;
            docObj.Document = models.Document();
            docObj.Document.DepartmentID = self.departmentID;
            docObj.Document.Destination = models.Destination();
            docObj.Document.Source = models.Source();
            docObj.Document.Source.Citizen = models.Citizen();

            return docObj;
        }

        this.createForm(options);
    };

    window.WorkerStatementBlank = workerStatementBlank;

})(window, jQuery);
