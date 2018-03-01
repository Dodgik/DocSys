(function (window, $, undefined) {

    var appSettings = window.appSettings;
    var statementBlankB = function (options) {
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
            if (options.success)
                this.success = options.success;
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
                dicts = o.dictionaries,
                isReadOnly = false;
            var isMiniMode = ($(window).height() < 750);
            
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

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title"></td>').append(name)).append($('<td></td>').append(obj));
            },
            comboBox = function (prms) {
                var cb = $('<select><option value="0"></option></select>');
                if (prms.items) {
                    var items = prms.items;
                    for (var it in items)
                        cb.append('<option value="' + items[it].id + '" ' + (items[it].id == prms.selected ? 'selected="selected"' : '') + '>' + items[it].name + '</option>');
                }
                return cb;
            };
            
            var table = $('<table style="border: 1px dashed #C0C0C0; font-weight: bold;"></table>');

            if (thisDoc.Document.DepartmentID === userdata.departmentId || thisDoc.Document.DepartmentID === 0) {
                createEditForm();
            } else {
                isReadOnly = true;
                createReadForm();
            }

            function createEditForm() {

                var tableN1 = $('<table></table>');
                var tableN2 = $('<table style="border-left-style: dashed; border-left-width: 1px; border-left-color: #C0C0C0;"></table>');

                self.fields.IsReception = thisDoc.IsReception;
                self.fields.ID = thisDoc.ID;
                self.fields.DocumentID = thisDoc.DocumentID;

                self.fields.Document = {};
                self.fields.Document.ID = thisDoc.Document.ID;
                self.fields.Document.CodeID = thisDoc.Document.CodeID;
                self.fields.Document.DocStatusID = thisDoc.Document.DocStatusID;
                self.fields.Document.Files = thisDoc.Document.Files;
                self.fields.Document.Notes = thisDoc.Document.Notes;
                self.fields.Document.DepartmentID = thisDoc.Document.DepartmentID;

                self.fields.Document.Number = $('<input type="text" style="width: 138px;">').val(thisDoc.Document.Number);
                tableN1.append(row('Вхідний номер:', self.fields.Document.Number));

                self.fields.Document.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true });
                if (thisDoc.Document.CreationDate) {
                    var creationDate = new Date(+thisDoc.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    self.fields.Document.CreationDate.datepicker("setDate", creationDate);
                }
                tableN1.append(row('Дата надходження:', self.fields.Document.CreationDate));

                self.fields.Head = $('<input type="text" valueid="0" style="width: 138px;">')
                    .val(thisDoc.Head ? formatFullName(thisDoc.Head.LastName, thisDoc.Head.FirstName, thisDoc.Head.MiddleName) : "")
                    .attr('valueid', thisDoc.HeadID)
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
                                            id: parseFloat(item[0]),
                                            label: formatFullName(item[1], item[2], item[3]),
                                            value: formatFullName(item[1], item[2], item[3]),
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
                var headRow = row('Керівник прийому:', self.fields.Head);
                tableN1.append(headRow);
                if (fields.Head != undefined && fields.Head.hidden)
                    headRow.hide();

                var triggerBc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                var branches = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute;" class="branches ui-state-default"></div>');
                if (dicts.branchTypes) {
                    var btTable = $('<table></table>');
                    var bts = dicts.branchTypes;
                    for (var bt in bts) {
                        btTable.append('<tr><td><label><input type="checkbox" value="' + bts[bt].id + '" ' + ($.inArray(bts[bt].id, thisDoc.Branches) > -1 ? 'checked="checked"' : '') + '/>' + bts[bt].name + '</label></td></tr>');
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
                self.fields.Branches = branches;
                tableN1.append(row('Галузі заяви:', branchesContainer));


                
                var depLabels = getLabels();
                if (depLabels.length || thisDoc.Document.Labels.length) {
                    var triggerLabels = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                    var labelsWrap = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute;" class="branches ui-state-default"></div>');
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
                    tableN1.append(row('Вибіркові мітки:', labelsContainer));
                }


                self.fields.InputDocType = comboBox({ items: dicts.inputDocTypes, selected: thisDoc.InputDocTypeID });
                tableN2.append(row('Вид звернення:', self.fields.InputDocType));

                self.fields.InputMethod = comboBox({ items: dicts.inputMethods, selected: thisDoc.InputMethodID });
                tableN2.append(row('Тип звернення:', self.fields.InputMethod));

                self.fields.InputSubjectType = comboBox({ items: dicts.inputSubjectTypes, selected: thisDoc.InputSubjectTypeID });
                tableN2.append(row("Суб'єкт:", self.fields.InputSubjectType));

                self.fields.InputSign = comboBox({ items: dicts.inputSigns, selected: thisDoc.InputSignID });
                tableN2.append(row('Ознака надходження:', self.fields.InputSign));

                self.fields.DeliveryType = comboBox({ items: dicts.deliveryTypes, selected: thisDoc.DeliveryTypeID });
                tableN2.append(row('Тип надходження:', self.fields.DeliveryType));

                table.append($('<tr></tr>').append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN1)).append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN2)));


                self.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                    .height((isMiniMode ? 38 : 38))
                    .attr('rows', (isMiniMode ? '2' : '2'))
                    .val(thisDoc.Content);
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append('Короткий зміст документа:').append(self.fields.Content)));

                /*--------------------------------------------------------*/
                var tableN3 = $('<table></table>');

                self.fields.Document.Source = {};
                self.fields.Document.Source.ID = thisDoc.Document.Source ? thisDoc.Document.Source.ID : 0;
                self.fields.Document.Source.Organization = $('<input type="text" style="width: 300px;">').val(thisDoc.Document.Source ? thisDoc.Document.Source.OrganizationName : '')
                    .attr('valueid', thisDoc.Document.Source ? thisDoc.Document.Source.OrganizationID : 0)
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function (request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&type=search&orgtype=1&dep=' + self.departmentID + '&term=' + request.term,
                                type: "GET",
                                dataType: "json",
                                success: function (data) {
                                    response($.map(data, function (item) {
                                        return {
                                            id: parseFloat(item[0]),
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

                tableN3.append(row('Організація:', self.fields.Document.Source.Organization
                        .add(autocompleteButton(self.fields.Document.Source.Organization))
                        .add($('<button type="button" title="Список" style="margin-left: 4px;"></button>').attr("tabIndex", -1)
                            .button({ icons: { primary: 'ui-icon-bookmark' }, text: false })
                            .click(function () {
                                var orgsDlg = $('<div>').appendTo(appendTo).attr({
                                    'title': 'Управління організаціями'
                                });
                                var orgsEl = $('<div>').appendTo(orgsDlg);

                                orgsDlg.dialog({
                                    autoOpen: true,
                                    draggable: true,
                                    modal: true,
                                    position: ["center", "center"],
                                    resizable: true,
                                    minWidth: 560,
                                    minHeight: 650,
                                    close: function () {
                                        orgsDlg.dialog("destroy");
                                        orgsDlg.remove();
                                    },
                                    open: function () {

                                    }
                                });

                                var orgs = new Organizations({
                                    serviceUrl: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=org',
                                    element: orgsEl,
                                    organizationTypeID: 1,
                                    onSelect: function (sRow) {
                                        self.fields.Document.Source.Organization.attr('valueid', sRow.id).val(sRow.name);
                                        orgsDlg.dialog("close");
                                    },
                                    onClose: function () {
                                        orgsDlg.dialog("close");
                                    }
                                });

                            }))
                        .add($('<button type="button" title="Очистити поле" style="margin-left: 16px;"></button>').attr("tabIndex", -1)
                            .button({ icons: { primary: 'ui-icon-closethick' }, text: false })
                            .click(function () {
                                self.fields.Document.Source.Organization.attr('valueid', 0);
                                self.fields.Document.Source.Organization.val('');
                            }))
                ));

                self.fields.Document.Source.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true });
                if (thisDoc.Document.Source && thisDoc.Document.Source.CreationDate) {
                    var externalCreationDate = new Date(+thisDoc.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    if (externalCreationDate > testTime) {
                        self.fields.Document.Source.CreationDate.datepicker("setDate", externalCreationDate);
                    }
                }
                tableN3.append(row('Дата складання документа:', self.fields.Document.Source.CreationDate));

                self.fields.Document.Source.Number = $('<input type="text" style="width: 138px;">').val(thisDoc.Document.Source ? thisDoc.Document.Source.Number : '');
                tableN3.append(row('Номер документа в організації:', self.fields.Document.Source.Number));

                self.fields.IsNeedAnswer = $('<input type="checkbox">').attr('checked', thisDoc.IsNeedAnswer);
                tableN3.append(row('Потреба у відповіді:', self.fields.IsNeedAnswer));

                if (fields.Source != undefined && fields.Source.hidden)
                    tableN3.hide();

                table.append($('<tr></tr>').append($('<td colspan="2" style="background-color: #EFEFEF; border-bottom: 1px dashed #C0C0C0; border-top: 1px dashed #C0C0C0;"></td>').append(tableN3)));


                /*--------------------------------------------------------*/
                self.fields.CitizenID = thisDoc.CitizenID;
                self.fields.Citizen = {};
                self.fields.Citizen.ID = thisDoc.Document.Source.Citizen.ID;
                var tableN4 = $('<table></table>');
                var tableN4Row = $('<tr></tr>').appendTo(tableN4);

                tableN4Row.append('<td style="width: 94px;">Прізвище:</td>');
                self.fields.Citizen.LastName = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.LastName : '');
                tableN4Row.append($('<td></td>').append(self.fields.Citizen.LastName));

                tableN4Row.append('<td style="padding-left: 10px;">Ім\'я:</td>');
                self.fields.Citizen.FirstName = $('<input type="text" style="width: 100px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.FirstName : '');
                tableN4Row.append($('<td></td>').append(self.fields.Citizen.FirstName));

                tableN4Row.append('<td style="padding-left: 10px;">По батькові:</td>');
                self.fields.Citizen.MiddleName = $('<input type="text" style="width: 100px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.MiddleName : '');
                tableN4Row.append($('<td></td>').append(self.fields.Citizen.MiddleName));

                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN4)));
                /*--------------------------------------------------------*/

                var tableN5 = $('<table></table>');
                var tableN5Row = $('<tr></tr>').appendTo(tableN5);

                tableN5Row.append('<td style="width: 94px;">Адреса:</td>');
                self.fields.Citizen.Address = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Address : '');
                tableN5Row.append($('<td></td>').append(self.fields.Citizen.Address));

                tableN5Row.append('<td style="padding-left: 10px;">Телефон:</td>');
                self.fields.Citizen.PhoneNumber = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.PhoneNumber : '');
                tableN5Row.append($('<td></td>').append(self.fields.Citizen.PhoneNumber));

                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN5)));
                /*--------------------------------------------------------*/

                var tableN6 = $('<table></table>');
                var tableN6Row = $('<tr></tr>').appendTo(tableN6);

                self.fields.Citizen.CityObjectTypeShortName = $('<span style="padding-left: 67px;">вул.</span>').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.CityObjectTypeShortName : '');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.CityObjectTypeShortName));
                self.fields.Citizen.CityObject = $('<input type="text" style="width: 204px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.CityObjectName : '')
                    .attr('valueid', thisDoc.Document.Source.Citizen.CityObjectID)
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: function (request, response) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=cityobject&type=search&dep=' + self.departmentID + '&term=' + request.term,
                                type: "GET",
                                dataType: "json",
                                success: function (data) {
                                    response($.map(data, function (item) {
                                        return {
                                            id: parseFloat(item[0]),
                                            label: item[2] + ' ' + item[1] + ' (' + item[4] + ')',
                                            value: item[2] + ' ' + item[1] + ' (' + item[4] + ')',
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
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.CityObject));

                tableN6Row.append('<td style="padding-left: 10px;">буд.</td>');
                self.fields.Citizen.HouseNumber = $('<input type="text" style="width: 30px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.HouseNumber : '');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.HouseNumber));

                tableN6Row.append('<td style="padding-left: 10px;">корп.</td>');
                self.fields.Citizen.Corps = $('<input type="text" style="width: 30px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Corps : '');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.Corps));

                tableN6Row.append('<td style="padding-left: 10px;">кв.</td>');
                self.fields.Citizen.ApartmentNumber = $('<input type="text" style="width: 60px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.ApartmentNumber : '');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.ApartmentNumber));

                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN6)));
                /*--------------------------------------------------------*/

                self.fields.Citizen.Work = $('<input type="text" style="width: 468px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Work : '');
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Місце роботи:</td></tr>').append($('<td></td>').append(self.fields.Citizen.Work))))));

                self.fields.Citizen.Sex = $('<select><option value="0">Не визанчено</option><option value="1">Чоловіча</option><option value="2">Жіноча</option></select>')
                    .val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Sex : 0);
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Стать:</td></tr>').append($('<td></td>').append(self.fields.Citizen.Sex))))));
                /*--------------------------------------------------------*/

                self.fields.Citizen.SocialStatus = comboBox({ items: dicts.socialStatuses, selected: thisDoc.Document.Source.Citizen.SocialStatusID });
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Соц. статус:</td></tr>').append($('<td></td>').append(self.fields.Citizen.SocialStatus))))));


                var triggerSoc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                var socialCategories = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px;" class="social-categories ui-state-default"></div>');
                if (dicts.socialCategories) {
                    var scTable = $('<table></table>');
                    var scs = dicts.socialCategories;
                    for (var cs in scs) {
                        scTable.append('<tr><td><label><input type="checkbox" value="' + scs[cs].id + '" ' + ($.inArray(scs[cs].id, thisDoc.Document.Source.Citizen.SocialCategories) > -1 ? 'checked="checked"' : '') + '/>' + scs[cs].name + '</label></td></tr>');
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
                self.fields.Citizen.SocialCategories = socialCategories;
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Соц. категорії:</td></tr>').append($('<td></td>').append(socialCategoriesContainer))))));


                self.fields.uploadButton = $('<div></div>');
                table.append(row('Прикріплені файли:', self.fields.uploadButton));
                recreateFileUploader();


                //Buttons
                self.buttons.buttonCreate = $('<input type="button" value="Зберегти">').button().click(function () {
                    sendDocData({ close: true });
                    return false;
                });

                self.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                    if (ops.onCancel instanceof Function) {
                        ops.onCancel();
                    }
                    return false;
                });


                self.blank = $('<div class="form-font-big"></div>').append(table);
                self.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(self.buttons.buttonCreate.add(self.buttons.buttonCancel))));
                self.blank.append(self.actionPanel);

                self.blank.appendTo(appendTo);

            }

            function createReadForm() {

                var tableN1 = $('<table></table>');
                var tableN2 = $('<table style="border-left-style: dashed; border-left-width: 1px; border-left-color: #C0C0C0;"></table>');

                self.fields.IsReception = thisDoc.IsReception;
                self.fields.ID = thisDoc.ID;
                self.fields.DocumentID = thisDoc.DocumentID;

                self.fields.Document = {};
                self.fields.Document.ID = thisDoc.Document.ID;
                self.fields.Document.CodeID = thisDoc.Document.CodeID;
                self.fields.Document.DocStatusID = thisDoc.Document.DocStatusID;
                self.fields.Document.Files = thisDoc.Document.Files;
                self.fields.Document.Notes = thisDoc.Document.Notes;
                self.fields.Document.DepartmentID = thisDoc.Document.DepartmentID;

                self.fields.Document.Number = $('<input type="text" style="width: 138px;">').val(thisDoc.Document.Number).attr('disabled', 'disabled');
                tableN1.append(row('Вхідний номер:', self.fields.Document.Number));

                self.fields.Document.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                if (thisDoc.Document.CreationDate) {
                    var creationDate = new Date(+thisDoc.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    self.fields.Document.CreationDate.datepicker("setDate", creationDate);
                }
                tableN1.append(row('Дата надходження:', self.fields.Document.CreationDate));

                self.fields.Head = $('<input type="text" valueid="0" style="width: 138px;">')
                    .val(thisDoc.Head ? formatFullName(thisDoc.Head.LastName, thisDoc.Head.FirstName, thisDoc.Head.MiddleName) : "")
                    .attr('valueid', thisDoc.HeadID).attr('disabled', 'disabled');
                var headRow = row('Керівник прийому:', self.fields.Head);
                tableN1.append(headRow);
                if (fields.Head != undefined && fields.Head.hidden)
                    headRow.hide();

                var triggerBc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                var branches = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute;" class="branches ui-state-default"></div>');
                if (dicts.branchTypes) {
                    var btTable = $('<table></table>');
                    var bts = dicts.branchTypes;
                    for (var bt in bts) {
                        btTable.append('<tr><td><label><input disabled="disabled" type="checkbox" value="' + bts[bt].id + '" ' + ($.inArray(bts[bt].id, thisDoc.Branches) > -1 ? 'checked="checked"' : '') + '/>' + bts[bt].name + '</label></td></tr>');
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
                self.fields.Branches = branches;
                tableN1.append(row('Галузі заяви:', branchesContainer));
                
                var depLabels = getLabels();
                if (depLabels.length || thisDoc.Document.Labels.length) {
                    var triggerLabels = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                    var labelsWrap = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px; position: absolute;" class="branches ui-state-default"></div>');
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
                    
                    labelsWrap.append($('<input type="button" value="Зберегти" style="float: right;">').button().click(function (e) {
                        var that = $(this);
                        that.css('background-image', '').css('background-color', '');
                        var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=addlbl' + '&dep=' + userdata.departmentId + '&documentId=' + thisDoc.Document.ID;
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
                    tableN1.append(row('Вибіркові мітки:', labelsContainer));
                }


                self.fields.InputDocType = comboBox({ items: dicts.inputDocTypes, selected: thisDoc.InputDocTypeID }).attr('disabled', 'disabled');
                tableN2.append(row('Вид звернення:', self.fields.InputDocType));

                self.fields.InputMethod = comboBox({ items: dicts.inputMethods, selected: thisDoc.InputMethodID }).attr('disabled', 'disabled');
                tableN2.append(row('Тип звернення:', self.fields.InputMethod));

                self.fields.InputSubjectType = comboBox({ items: dicts.inputSubjectTypes, selected: thisDoc.InputSubjectTypeID }).attr('disabled', 'disabled');
                tableN2.append(row("Суб'єкт:", self.fields.InputSubjectType));

                self.fields.InputSign = comboBox({ items: dicts.inputSigns, selected: thisDoc.InputSignID }).attr('disabled', 'disabled');
                tableN2.append(row('Ознака надходження:', self.fields.InputSign));

                self.fields.DeliveryType = comboBox({ items: dicts.deliveryTypes, selected: thisDoc.DeliveryTypeID }).attr('disabled', 'disabled');
                tableN2.append(row('Тип надходження:', self.fields.DeliveryType));

                table.append($('<tr></tr>').append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN1)).append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN2)));


                self.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>').attr('disabled', 'disabled')
                    .height((isMiniMode ? 38 : 38))
                    .attr('rows', (isMiniMode ? '2' : '2'))
                    .val(thisDoc.Content);
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append('Короткий зміст документа:').append(self.fields.Content)));

                /*--------------------------------------------------------*/
                var tableN3 = $('<table></table>');

                self.fields.Document.Source = {};
                self.fields.Document.Source.ID = thisDoc.Document.Source ? thisDoc.Document.Source.ID : 0;
                self.fields.Document.Source.Organization = $('<input type="text" style="width: 300px;">').val(thisDoc.Document.Source ? thisDoc.Document.Source.OrganizationName : '')
                    .attr('valueid', thisDoc.Document.Source ? thisDoc.Document.Source.OrganizationID : 0).attr('disabled', 'disabled');

                tableN3.append(row('Організація:', self.fields.Document.Source.Organization));

                self.fields.Document.Source.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true }).attr('disabled', 'disabled');
                if (thisDoc.Document.Source && thisDoc.Document.Source.CreationDate) {
                    var externalCreationDate = new Date(+thisDoc.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    if (externalCreationDate > testTime) {
                        self.fields.Document.Source.CreationDate.datepicker("setDate", externalCreationDate);
                    }
                }
                tableN3.append(row('Дата складання документа:', self.fields.Document.Source.CreationDate));

                self.fields.Document.Source.Number = $('<input type="text" style="width: 138px;">').val(thisDoc.Document.Source ? thisDoc.Document.Source.Number : '').attr('disabled', 'disabled');
                tableN3.append(row('Номер документа в організації:', self.fields.Document.Source.Number));

                self.fields.IsNeedAnswer = $('<input type="checkbox">').attr('checked', thisDoc.IsNeedAnswer).attr('disabled', 'disabled');
                tableN3.append(row('Потреба у відповіді:', self.fields.IsNeedAnswer));

                if (fields.Source != undefined && fields.Source.hidden)
                    tableN3.hide();

                table.append($('<tr></tr>').append($('<td colspan="2" style="background-color: #EFEFEF; border-bottom: 1px dashed #C0C0C0; border-top: 1px dashed #C0C0C0;"></td>').append(tableN3)));


                /*--------------------------------------------------------*/
                self.fields.CitizenID = thisDoc.CitizenID;
                self.fields.Citizen = {};
                self.fields.Citizen.ID = thisDoc.Document.Source.Citizen.ID;
                var tableN4 = $('<table></table>');
                var tableN4Row = $('<tr></tr>').appendTo(tableN4);

                tableN4Row.append('<td style="width: 94px;">Прізвище:</td>');
                self.fields.Citizen.LastName = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.LastName : '').attr('disabled', 'disabled');
                tableN4Row.append($('<td></td>').append(self.fields.Citizen.LastName));

                tableN4Row.append('<td style="padding-left: 10px;">Ім\'я:</td>');
                self.fields.Citizen.FirstName = $('<input type="text" style="width: 100px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.FirstName : '').attr('disabled', 'disabled');
                tableN4Row.append($('<td></td>').append(self.fields.Citizen.FirstName));

                tableN4Row.append('<td style="padding-left: 10px;">По батькові:</td>');
                self.fields.Citizen.MiddleName = $('<input type="text" style="width: 100px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.MiddleName : '').attr('disabled', 'disabled');
                tableN4Row.append($('<td></td>').append(self.fields.Citizen.MiddleName));

                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN4)));
                /*--------------------------------------------------------*/

                var tableN5 = $('<table></table>');
                var tableN5Row = $('<tr></tr>').appendTo(tableN5);

                tableN5Row.append('<td style="width: 94px;">Адреса:</td>');
                self.fields.Citizen.Address = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Address : '').attr('disabled', 'disabled');
                tableN5Row.append($('<td></td>').append(self.fields.Citizen.Address));

                tableN5Row.append('<td style="padding-left: 10px;">Телефон:</td>');
                self.fields.Citizen.PhoneNumber = $('<input type="text" style="width: 200px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.PhoneNumber : '').attr('disabled', 'disabled');
                tableN5Row.append($('<td></td>').append(self.fields.Citizen.PhoneNumber));

                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN5)));
                /*--------------------------------------------------------*/

                var tableN6 = $('<table></table>');
                var tableN6Row = $('<tr></tr>').appendTo(tableN6);

                self.fields.Citizen.CityObjectTypeShortName = $('<span style="padding-left: 67px;">вул.</span>').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.CityObjectTypeShortName : '');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.CityObjectTypeShortName));
                self.fields.Citizen.CityObject = $('<input type="text" style="width: 204px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.CityObjectName : '')
                    .attr('valueid', thisDoc.Document.Source.Citizen.CityObjectID).attr('disabled', 'disabled');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.CityObject));

                tableN6Row.append('<td style="padding-left: 10px;">буд.</td>');
                self.fields.Citizen.HouseNumber = $('<input type="text" style="width: 30px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.HouseNumber : '').attr('disabled', 'disabled');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.HouseNumber));

                tableN6Row.append('<td style="padding-left: 10px;">корп.</td>');
                self.fields.Citizen.Corps = $('<input type="text" style="width: 30px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Corps : '').attr('disabled', 'disabled');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.Corps));

                tableN6Row.append('<td style="padding-left: 10px;">кв.</td>');
                self.fields.Citizen.ApartmentNumber = $('<input type="text" style="width: 60px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.ApartmentNumber : '').attr('disabled', 'disabled');
                tableN6Row.append($('<td></td>').append(self.fields.Citizen.ApartmentNumber));

                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN6)));
                /*--------------------------------------------------------*/

                self.fields.Citizen.Work = $('<input type="text" style="width: 468px;">').val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Work : '').attr('disabled', 'disabled');
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Місце роботи:</td></tr>').append($('<td></td>').append(self.fields.Citizen.Work))))));

                self.fields.Citizen.Sex = $('<select><option value="0">Не визанчено</option><option value="1">Чоловіча</option><option value="2">Жіноча</option></select>')
                    .val(thisDoc.Document.Source.Citizen ? thisDoc.Document.Source.Citizen.Sex : 0).attr('disabled', 'disabled');
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Стать:</td></tr>').append($('<td></td>').append(self.fields.Citizen.Sex))))));
                /*--------------------------------------------------------*/

                self.fields.Citizen.SocialStatus = comboBox({ items: dicts.socialStatuses, selected: thisDoc.Document.Source.Citizen.SocialStatusID }).attr('disabled', 'disabled');
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Соц. статус:</td></tr>').append($('<td></td>').append(self.fields.Citizen.SocialStatus))))));


                var triggerSoc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
                var socialCategories = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px;" class="social-categories ui-state-default"></div>');
                if (dicts.socialCategories) {
                    var scTable = $('<table></table>');
                    var scs = dicts.socialCategories;
                    for (var cs in scs) {
                        scTable.append('<tr><td><label><input disabled="disabled" type="checkbox" value="' + scs[cs].id + '" ' + ($.inArray(scs[cs].id, thisDoc.Document.Source.Citizen.SocialCategories) > -1 ? 'checked="checked"' : '') + '/>' + scs[cs].name + '</label></td></tr>');
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
                self.fields.Citizen.SocialCategories = socialCategories;
                table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                    .append($('<tr><td style="width: 94px;">Соц. категорії:</td></tr>').append($('<td></td>').append(socialCategoriesContainer))))));



                self.fields.uploadButton = $('<div></div>');
                table.append(row('Прикріплені файли:', self.fields.uploadButton));
                recreateFileUploader();


                
                var cards = thisDoc.ControlCards,
                    parentControlCardId = 0,
                    lastChildrenControlCardNumber = 0,
                    headId = userdata.worker.ID,
                    hasBaseCard = false;

                cards.forEach(function (rCard) {
                    if (userdata.departmentId === rCard.DepartmentID) {
                        return true;
                    }
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
                    }

                    parentControlCardId = rCard.ID;
                    headId = rCard.HeadID;
                    head = (head && rCard.HeadID > 0) ? documentUI.formatFullName(head.LastName, head.FirstName, head.MiddleName) : '';
                    worker = (worker && rCard.WorkerID > 0) ? documentUI.formatFullName(worker.LastName, worker.FirstName, worker.MiddleName) : '';
                    fixedWorker = (fixedWorker && rCard.FixedWorkerID > 0) ? documentUI.formatFullName(fixedWorker.LastName, fixedWorker.FirstName, fixedWorker.MiddleName) : '';


                    table.append(row('&nbsp;'));
                    table.append(row(rCard.CardNumber + '. Виконавець:', worker + '&nbsp;&nbsp;&nbsp;&nbsp; Закріплена за: ' + fixedWorker));
                    table.append(row('&nbsp; Резолюція:', rCard.Resolution));


                    var cardStatus = rCard.CardStatus ? rCard.CardStatus.Name : '';

                    var endDate = '';
                    if (rCard.EndDate) {
                        endDate = new Date(+rCard.EndDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                        if (endDate > testTime) {
                            endDate = endDate;
                        }
                        else {
                            endDate = '';
                        }
                    }
                    var cardRow = $('<div> Стан: <span class="card-status-' + rCard.CardStatusID + '">' + cardStatus + '</span> </div>')
                        .prepend($('<input class="card-enddate' + (rCard.CardStatusID != 1 ? ' card-close' : '') + '" type="text" disabled="disabled">').datepicker({ changeMonth: true, changeYear: true }).datepicker("setDate", endDate))
                        .prepend(' Термін: ');
                    table.append(row('&nbsp; ' + head, cardRow));

                    table.append(row('&nbsp;'));

                    if (isBaseCard) {
                        self.fields.InnerNumber = $('<input type="text" style="width: 200px;">').val(rCard.InnerNumber)
                            .add($('<input type="button" value="Зберегти">').button().click(function (e) {
                                var that = $(this);
                                that.css('background-image', '').css('background-color', '');
                                rCard.InnerNumber = self.fields.InnerNumber.val().trim();

                                //var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=upd' + '&dep=' + userdata.departmentId;
                                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=card&type=innernumber' +
                                                '&dep=' + userdata.departmentId + '&cardid=' + rCard.ID;

                                $.ajax({
                                    url: urlRequest,
                                    type: "POST",
                                    cache: false,
                                    /*data: { 'jdata': JSON.stringify(rCard) },*/
                                    data: { 'jdata': rCard.InnerNumber },
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
                    }
                    
                    self.buttons.buttonResponse = $('<input type="button" value="Відповісти">').button().click(function (e) {
                        //rCard.ControlResponseDate = self.fields.ControlResponseDate.val();
                        rCard.InnerNumber = self.fields.InnerNumber.val().trim();
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
                    self.fields.ControlResponse = $('<textarea style="height:57px;width:98%;font-size:15px;" cols="81" rows="3" ' + (foreignCard ? 'disabled="disabled"' : '') + '></textarea>')
                        .val(rCard.ControlResponse);
                    table.append($('<tr class="blank-row"></tr>').append($('<td colspan="2" class="blank-row-title"></td>')
                        .append('Контрольна відповідь:').append(self.fields.ControlResponse.add(self.buttons.buttonResponse))));


                    table.append(row('Виконавці:'));
                    var ccc = rCard.ChildrenControlCards;
                    if (ccc.length > 0) {
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
                    table.append(row(p.CardNumber + '. ' + p.worker, resp));
                }

                //Buttons
                self.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                    if (ops.onCancel instanceof Function) {
                        ops.onCancel();
                    }
                    return false;
                });


                self.blank = $('<div class="form-font-big"></div>').append(table);
                self.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(self.buttons.buttonCancel)));
                self.blank.append(self.actionPanel);

                self.blank.appendTo(appendTo);

            }

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
                    onComplete: function (id, fileName, response) {
                        self.fields.Document.Files.push({ DocumentID: thisDoc.DocumentID, FileID: response.fileID, FileName: fileName });
                    },
                    onRemove: function (id, file) {
                        if (confirm('Ви дійсно бажаєте видалити файл ' + file.name + ' ?')) {
                            $.ajax({
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=del&dep=' + documentUI.departmentID + '&fileID=' + file.id + '&documentid=' + self.fields.DocumentID,
                                type: 'GET',
                                dataType: 'json',
                                success: function () {
                                    for (var j in self.fields.Document.Files)
                                        if (self.fields.Document.Files[j].FileID == file.id)
                                            self.fields.Document.Files.splice(j, 1);
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
            
            setFields(thisDoc);
            initForm(thisDoc);

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
                docObj.Document.Source.DepartmentID = 0;

                docObj.Document.Source.CreationDate = self.fields.Document.Source.CreationDate.val();
                docObj.Document.Source.Number = self.fields.Document.Source.Number.val();
                docObj.Document.Source.CitizenID = self.fields.CitizenID;

                docObj.Document.Destination.CreationDate = docObj.Document.CreationDate;
                docObj.Document.Destination.DepartmentID = userdata.departmentId;
                docObj.Document.Destination.Number = self.fields.Document.Number.val();

                var checkedBranches = [];
                self.fields.Branches.find('input:checked').each(function () { checkedBranches.push($(this).val()); });
                docObj.Branches = checkedBranches;
                
                var checkedLabels = [];
                if (self.fields.Labels) {
                    self.fields.Labels.find('input:checked:enabled').each(function() {
                        checkedLabels.push($(this).val());
                    });
                }
                docObj.Document.Labels = checkedLabels;

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

                self.fields.IsReception = data.IsReception;
                self.fields.ID = data.ID;
                self.fields.DocumentID = data.DocumentID;

                self.fields.Document.ID = data.Document.ID;
                self.fields.Document.CodeID = data.Document.CodeID;
                self.fields.Document.DocStatusID = data.Document.DocStatusID;
                self.fields.Document.Files = data.Document.Files;
                self.fields.Document.Notes = data.Document.Notes;
                self.fields.Document.DepartmentID = data.Document.DepartmentID;

                self.fields.Document.Number.val(data.Document.Number);

                if (data.Document.CreationDate) {
                    var creationDate = new Date(+data.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    self.fields.Document.CreationDate.datepicker("setDate", creationDate);
                }

                self.fields.Head.val(data.Head ? formatFullName(data.Head.LastName, data.Head.FirstName, data.Head.MiddleName) : '')
                    .attr('valueid', data.HeadID);

                var branches = self.fields.Branches;
                if (dicts.branchTypes) {
                    branches.empty();
                    var btTable = $('<table></table>');
                    var bts = dicts.branchTypes;
                    for (var bt in bts) {
                        btTable.append('<tr><td><label><input ' + (isReadOnly ? 'disabled="disabled" ' : '') + 'type="checkbox" value="' + bts[bt].id + '" ' + ($.inArray(bts[bt].id, data.Branches) > -1 ? 'checked="checked"' : '') + '/>' + bts[bt].name + '</label></td></tr>');
                    }
                    branches.append(btTable);
                }

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


                self.fields.InputDocType.val(data.InputDocTypeID);
                self.fields.InputMethod.val(data.InputMethodID);
                self.fields.InputSubjectType.val(data.InputSubjectTypeID);
                self.fields.InputSign.val(data.InputSignID);
                self.fields.DeliveryType.val(data.DeliveryTypeID);

                self.fields.Content.val(data.Content);

                self.fields.Document.Source.ID = data.Document.Source ? data.Document.Source.ID : 0;
                self.fields.Document.Source.Organization.val(data.Document.Source ? data.Document.Source.OrganizationName : '')
                    .attr('valueid', data.Document.Source ? data.Document.Source.OrganizationID : 0);

                if (data.Document.Source && data.Document.Source.CreationDate) {
                    var externalCreationDate = new Date(+data.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                    if (externalCreationDate > testTime) {
                        self.fields.Document.Source.CreationDate.datepicker("setDate", externalCreationDate);
                    }
                }

                self.fields.Document.Source.Number.val(data.Document.Source ? data.Document.Source.Number : '');
                self.fields.IsNeedAnswer.attr('checked', data.IsNeedAnswer);

                /*--------------------------------------------------------*/
                self.fields.CitizenID = data.CitizenID;
                self.fields.Citizen.ID = data.Document.Source.Citizen.ID;
                self.fields.Citizen.LastName.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.LastName : '');
                self.fields.Citizen.FirstName.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.FirstName : '');
                self.fields.Citizen.MiddleName.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.MiddleName : '');
                /*--------------------------------------------------------*/
                self.fields.Citizen.Address.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.Address : '');
                self.fields.Citizen.PhoneNumber.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.PhoneNumber : '');
                /*--------------------------------------------------------*/
                self.fields.Citizen.CityObjectTypeShortName.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.CityObjectTypeShortName : '');
                self.fields.Citizen.CityObject.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.CityObjectName : '')
                    .attr('valueid', data.Document.Source.Citizen.CityObjectID);
                self.fields.Citizen.HouseNumber.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.HouseNumber : '');
                self.fields.Citizen.Corps.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.Corps : '');
                self.fields.Citizen.ApartmentNumber.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.ApartmentNumber : '');
                /*--------------------------------------------------------*/
                self.fields.Citizen.Work.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.Work : '');
                self.fields.Citizen.Sex.val(data.Document.Source.Citizen ? data.Document.Source.Citizen.Sex : 0);
                /*--------------------------------------------------------*/
                self.fields.Citizen.SocialStatus.val(data.Document.Source.Citizen.SocialStatusID);


                var socialCategories = self.fields.Citizen.SocialCategories;
                if (dicts.socialCategories) {
                    socialCategories.empty();
                    var scTable = $('<table></table>');
                    var scs = dicts.socialCategories;
                    for (var cs in scs) {
                        scTable.append('<tr><td><label><input ' + (isReadOnly ? 'disabled="disabled" ' : '') + 'type="checkbox" value="' + scs[cs].id + '" ' + ($.inArray(scs[cs].id, data.Document.Source.Citizen.SocialCategories) > -1 ? 'checked="checked"' : '') + '/>' + scs[cs].name + '</label></td></tr>');
                    }
                    socialCategories.append(scTable);
                }

                recreateFileUploader();
            }


            function sendDocData(p) {
                var docObj = getDocData();
                if (ops.onSave instanceof Function) {
                    ops.onSave(docObj);
                }

                return false;
            }
            
            return self.blank;
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

    window.StatementBlankB = statementBlankB;

})(window, jQuery);
