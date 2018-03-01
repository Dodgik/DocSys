(function (window, $, undefined) {

    var appSettings = window.appSettings;
    var statementBlankB = function (options) {
        var self = this;

        var o = {
            dictionaries: {}
        },
        clearDoc = {
            ID: 0,
            DocumentID: 0,
            Branches: [],
            CitizenID: 0,
            Content: '',
            DeliveryTypeID: 0,
            HeadID: 0,
            InputDocTypeID: 0,
            InputMethodID: 0,
            InputSignID: 0,
            InputSubjectTypeID: 0,
            IsNeedAnswer: false,
            IsReception: false,
            Document: {
                ID: 0,
                CodeID: 0,
                CodeName: '',
                DepartmentID: self.departmentID,
                DocStatusID: 0,
                Files: [],
                Notes: '',
                Number: '',
                CreationDate: '',
                ExternalSource: {
                    ID: 0,
                    OrganizationID: 0,
                    OrganizationName: '',
                    CreationDate: '',
                    Number: ''
                }
            },
            Citizen: {
                ID: 0,
                LastName: '',
                FirstName: '',
                MiddleName: '',
                Address: '',
                PhoneNumber: '',
                CityObjectTypeShortName: '',
                CityObjectID: 0,
                CityObjectName: '',
                HouseNumber: '',
                Corps: '',
                ApartmentNumber: '',
                Work: '',
                Sex: 0,
                SocialStatusID: 0,
                SocialCategories: []
            }
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

            var appendTo = ops.appendTo || 'body',
                docData = $.extend(clearDoc, ops.docData),
                fields = ops.fields || {},
                dicts = o.dictionaries;
            var isMiniMode = ($(window).height() < 750);
            
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
            var tableN1 = $('<table></table>');
            var tableN2 = $('<table style="border-left-style: dashed; border-left-width: 1px; border-left-color: #C0C0C0;"></table>');
            
            this.fields.IsReception = docData.IsReception;
            this.fields.ID = docData.ID;
            this.fields.DocumentID = docData.DocumentID;
            
            this.fields.Document = {};
            this.fields.Document.ID = docData.Document.ID;
            this.fields.Document.CodeID = docData.Document.CodeID;
            this.fields.Document.DocStatusID = docData.Document.DocStatusID;
            this.fields.Document.Files = docData.Document.Files;
            this.fields.Document.Notes = docData.Document.Notes;
            this.fields.Document.DepartmentID = docData.Document.DepartmentID;
            
            this.fields.Document.Number = $('<input type="text" style="width: 138px;">').val(docData.Document.Number);
            tableN1.append(row('Вхідний номер:', this.fields.Document.Number));

            this.fields.Document.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.Document.CreationDate) {
                var creationDate = new Date(+docData.Document.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                this.fields.Document.CreationDate.datepicker("setDate", creationDate);
            }
            tableN1.append(row('Дата надходження:', this.fields.Document.CreationDate));

            this.fields.Head = $('<input type="text" valueid="0" style="width: 138px;">')
                .val(docData.Head ? formatFullName(docData.Head.LastName, docData.Head.FirstName, docData.Head.MiddleName) : "")
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
                    btTable.append('<tr><td><label><input type="checkbox" value="' + bts[bt].id + '" ' + ($.inArray(bts[bt].id, docData.Branches) > -1 ? 'checked="checked"' : '') + '/>' + bts[bt].name + '</label></td></tr>');
                }
                branches.append(btTable);
            }
            var branchesContainer = $('<div></div>')
                .append($('<div></div>').append(triggerBc)
                .click(function() {
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


            this.fields.InputDocType = comboBox({ items: dicts.inputDocTypes, selected: docData.InputDocTypeID });
            tableN2.append(row('Вид звернення:', this.fields.InputDocType));

            this.fields.InputMethod = comboBox({ items: dicts.inputMethods, selected: docData.InputMethodID });
            tableN2.append(row('Тип звернення:', this.fields.InputMethod));

            this.fields.InputSubjectType = comboBox({ items: dicts.inputSubjectTypes, selected: docData.InputSubjectTypeID });
            tableN2.append(row("Суб'єкт:", this.fields.InputSubjectType));

            this.fields.InputSign = comboBox({ items: dicts.inputSigns, selected: docData.InputSignID });
            tableN2.append(row('Ознака надходження:', this.fields.InputSign));

            this.fields.DeliveryType = comboBox({ items: dicts.deliveryTypes, selected: docData.DeliveryTypeID });
            tableN2.append(row('Тип надходження:', this.fields.DeliveryType));

            table.append($('<tr></tr>').append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN1)).append($('<td style="background-color: #EFEFEF; vertical-align: top;"></td>').append(tableN2)));


            this.fields.Content = $('<textarea style="width:98%;" cols="81"></textarea>')
                .height((isMiniMode ? 38 : 38))
                .attr('rows', (isMiniMode ? '2' : '2'))
                .val(docData.Content);
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append('Короткий зміст документа:').append(this.fields.Content)));

            /*--------------------------------------------------------*/
            var tableN3 = $('<table></table>');
            
            this.fields.Document.ExternalSource = {};
            this.fields.Document.ExternalSource.ID = docData.Document.ExternalSource ? docData.Document.ExternalSource.ID : 0;
            this.fields.Document.ExternalSource.Organization = $('<input type="text" style="width: 300px;">').val(docData.Document.ExternalSource ? docData.Document.ExternalSource.OrganizationName : '')
                .attr('valueid', docData.Document.ExternalSource ? docData.Document.ExternalSource.OrganizationID : 0)
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

            tableN3.append(row('Організація:', this.fields.Document.ExternalSource.Organization
                    .add(autocompleteButton(this.fields.Document.ExternalSource.Organization))
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
					            onSelect: function(sRow) {
					                self.fields.Document.ExternalSource.Organization.attr('valueid', sRow.id).val(sRow.name);
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
					        self.fields.Document.ExternalSource.Organization.attr('valueid', 0);
					        self.fields.Document.ExternalSource.Organization.val('');
					    }))
            ));

            this.fields.Document.ExternalSource.CreationDate = $('<input type="text" style="width: 138px;">').datepicker({ changeMonth: true, changeYear: true });
            if (docData.Document.ExternalSource && docData.Document.ExternalSource.CreationDate) {
                var externalCreationDate = new Date(+docData.Document.ExternalSource.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                this.fields.Document.ExternalSource.CreationDate.datepicker("setDate", externalCreationDate);
            }
            tableN3.append(row('Дата складання документа:', this.fields.Document.ExternalSource.CreationDate));

            this.fields.Document.ExternalSource.Number = $('<input type="text" style="width: 138px;">').val(docData.Document.ExternalSource ? docData.Document.ExternalSource.Number : '');
            tableN3.append(row('Номер документа в організації:', this.fields.Document.ExternalSource.Number));

            this.fields.IsNeedAnswer = $('<input type="checkbox">').attr('checked', docData.IsNeedAnswer);
            tableN3.append(row('Потреба у відповіді:', this.fields.IsNeedAnswer));

            if (fields.ExternalSource != undefined && fields.ExternalSource.hidden)
                tableN3.hide();
            
            table.append($('<tr></tr>').append($('<td colspan="2" style="background-color: #EFEFEF; border-bottom: 1px dashed #C0C0C0; border-top: 1px dashed #C0C0C0;"></td>').append(tableN3)));


            /*--------------------------------------------------------*/
            this.fields.CitizenID = docData.CitizenID;
            this.fields.Citizen = {};
            this.fields.Citizen.ID = docData.Citizen.ID;
            var tableN4 = $('<table></table>');
            var tableN4Row = $('<tr></tr>').appendTo(tableN4);

            tableN4Row.append('<td style="width: 94px;">Прізвище:</td>');
            this.fields.Citizen.LastName = $('<input type="text" style="width: 200px;">').val(docData.Citizen ? docData.Citizen.LastName : '');
            tableN4Row.append($('<td></td>').append(this.fields.Citizen.LastName));
            
            tableN4Row.append('<td style="padding-left: 10px;">Ім\'я:</td>');
            this.fields.Citizen.FirstName = $('<input type="text" style="width: 100px;">').val(docData.Citizen ? docData.Citizen.FirstName : '');
            tableN4Row.append($('<td></td>').append(this.fields.Citizen.FirstName));
            
            tableN4Row.append('<td style="padding-left: 10px;">По батькові:</td>');
            this.fields.Citizen.MiddleName = $('<input type="text" style="width: 100px;">').val(docData.Citizen ? docData.Citizen.MiddleName : '');
            tableN4Row.append($('<td></td>').append(this.fields.Citizen.MiddleName));
            
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN4)));
            /*--------------------------------------------------------*/

            var tableN5 = $('<table></table>');
            var tableN5Row = $('<tr></tr>').appendTo(tableN5);

            tableN5Row.append('<td style="width: 94px;">Адреса:</td>');
            this.fields.Citizen.Address = $('<input type="text" style="width: 200px;">').val(docData.Citizen ? docData.Citizen.Address : '');
            tableN5Row.append($('<td></td>').append(this.fields.Citizen.Address));

            tableN5Row.append('<td style="padding-left: 10px;">Телефон:</td>');
            this.fields.Citizen.PhoneNumber = $('<input type="text" style="width: 200px;">').val(docData.Citizen ? docData.Citizen.PhoneNumber : '');
            tableN5Row.append($('<td></td>').append(this.fields.Citizen.PhoneNumber));

            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN5)));
            /*--------------------------------------------------------*/

            var tableN6 = $('<table></table>');
            var tableN6Row = $('<tr></tr>').appendTo(tableN6);

            this.fields.Citizen.CityObjectTypeShortName = $('<span style="padding-left: 67px;">вул.</span>').val(docData.Citizen ? docData.Citizen.CityObjectTypeShortName : '');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.CityObjectTypeShortName));
            this.fields.Citizen.CityObject = $('<input type="text" style="width: 204px;">').val(docData.Citizen ? docData.Citizen.CityObjectName : '')
                .attr('valueid', docData.Citizen.CityObjectID)
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
                                        label: item[2] + ' ' + item[1],
                                        value: item[2] + ' ' + item[1],
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
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.CityObject));

            tableN6Row.append('<td style="padding-left: 10px;">буд.</td>');
            this.fields.Citizen.HouseNumber = $('<input type="text" style="width: 30px;">').val(docData.Citizen ? docData.Citizen.HouseNumber : '');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.HouseNumber));

            tableN6Row.append('<td style="padding-left: 10px;">корп.</td>');
            this.fields.Citizen.Corps = $('<input type="text" style="width: 30px;">').val(docData.Citizen ? docData.Citizen.Corps : '');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.Corps));

            tableN6Row.append('<td style="padding-left: 10px;">кв.</td>');
            this.fields.Citizen.ApartmentNumber = $('<input type="text" style="width: 60px;">').val(docData.Citizen ? docData.Citizen.ApartmentNumber : '');
            tableN6Row.append($('<td></td>').append(this.fields.Citizen.ApartmentNumber));
            
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append(tableN6)));
            /*--------------------------------------------------------*/

            this.fields.Citizen.Work = $('<input type="text" style="width: 468px;">').val(docData.Citizen ? docData.Citizen.Work : '');
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                .append($('<tr><td style="width: 94px;">Місце роботи:</td></tr>').append($('<td></td>').append(this.fields.Citizen.Work))))));

            this.fields.Citizen.Sex = $('<select><option value="0">Не визанчено</option><option value="1">Чоловіча</option><option value="2">Жіноча</option></select>')
                .val(docData.Citizen ? docData.Citizen.Sex : 0);
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                .append($('<tr><td style="width: 94px;">Стать:</td></tr>').append($('<td></td>').append(this.fields.Citizen.Sex))))));
            /*--------------------------------------------------------*/

            this.fields.Citizen.SocialStatus = comboBox({ items: dicts.socialStatuses, selected: docData.Citizen.SocialStatusID });
            table.append($('<tr></tr>').append($('<td colspan="2"></td>').append($('<table></table>')
                .append($('<tr><td style="width: 94px;">Соц. статус:</td></tr>').append($('<td></td>').append(this.fields.Citizen.SocialStatus))))));
            

            var triggerSoc = $('<h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-corner-all"><span class="ui-icon ui-icon-circle-arrow-e" style="float:left;"></span><a href="#">Розгорнути</a></h3>');
            var socialCategories = $('<div style="display:none; float:left; color:#222222; font-weight: normal; clear: both; overflow:scroll; height:180px;" class="ui-state-default"></div>');
            if (dicts.socialCategories) {
                var scTable = $('<table></table>');
                var scs = dicts.socialCategories;
                for (var cs in scs) {
                    scTable.append('<tr><td><label><input type="checkbox" value="' + scs[cs].id + '" ' + ($.inArray(scs[cs].id, docData.Citizen.SocialCategories) > -1 ? 'checked="checked"' : '') + '/>' + scs[cs].name + '</label></td></tr>');
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

            

            //Buttons
            this.buttons.buttonCreate = $('<input type="button" value="Зберегти">').button().click(function () {
                var docObj = {};
                
                docObj.IsReception = self.fields.IsReception;
                docObj.ID = self.fields.ID;
                docObj.DocumentID = self.fields.DocumentID;
                docObj.Document = {};
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

                docObj.Document.ExternalSource = {};
                docObj.Document.ExternalSource.ID = self.fields.Document.ExternalSource.ID;

                docObj.Document.ExternalSource.OrganizationName = self.fields.Document.ExternalSource.Organization.val();
                if (docObj.Document.ExternalSource.OrganizationName)
                    docObj.Document.ExternalSource.OrganizationID = parseFloat(self.fields.Document.ExternalSource.Organization.attr('valueid'));
                else
                    docObj.Document.ExternalSource.OrganizationID = 0;

                //docObj.Document.ExternalSource.CreationDate = $.datepicker.formatDate('yy-mm-dd', $.datepicker.parseDate('dd.mm.yy', self.fields.Document.ExternalSource.CreationDate.val()));
                docObj.Document.ExternalSource.CreationDate = self.fields.Document.ExternalSource.CreationDate.val();
                docObj.Document.ExternalSource.Number = self.fields.Document.ExternalSource.Number.val();


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

                if (ops.onSave instanceof Function) {
                    ops.onSave(docObj);
                }
            });
            
            this.buttons.buttonCancel = $('<input type="button" value="Відмінити">').button().click(function () {
                if (ops.onCancel instanceof Function) {
                    ops.onCancel();
                }
            });


            this.blank = $('<div class="form-font-big"></div>').append(table);
            this.actionPanel = $('<table style="width:100%;"></table>').append($('<tr></tr>').append($('<td align="center"></td>').append(this.buttons.buttonCreate.add(this.buttons.buttonCancel))));
            this.blank.append(this.actionPanel);

            this.blank.appendTo(appendTo);
            
            return this.blank;
        },

        this.dispose = function () {
            if (this.blank) {
                this.blank.remove();
            }
        };

        this.createForm(options);
    };

    window.StatementBlankB = statementBlankB;

})(window, jQuery);