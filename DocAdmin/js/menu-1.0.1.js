$(function () {
    var mList = window.navigationSettings.menuList,
        tList = window.navigationSettings.templateList;
    if (mList.length > 0) {
        var menuList = [];
        for (var i = 0, mItem; mItem = mList[i]; i++) {
            if (mList.length > 1) {
                menuList.push({ name: mItem.name, dep: mItem.id });

            }
            for (var j = 0, t; t = tList[j]; j++) {
                menuList.push({ name: t.name, href: t.systemName + '-' + mItem.id, dep: mItem.id, templateId: t.id });
            }
        }
        navigation.create({
            appendTo: '#navigation-menu',
            list: menuList,
            onSelect: function (e, item) {
                var tagName = e.target.getAttribute('href').match((/#([\w\-]+)?/))[1],
                    dep = parseInt(e.target.getAttribute('dep')),
                    documentsTable = null,
                    currentDialog;
                documentUI.departmentID = dep;
                
                $("#tabsStatementAndReception").empty();

                currentDialog = $('<div></div>').dialog({
                    //appendTo: '#tabsStatementAndReception',
                    autoOpen: true,
                    draggable: true,
                    modal: true,
                    position: [0, 0],
                    resizable: true,
                    minWidth: 800,
                    width: $(window).width() - 10,
                    close: function () {
                        $(this).dialog("destroy");
                        $(this).remove();
                        navigation.deselect();
                        documentsTable.dispose();
                    }
                });
                //currentDialog.parent().appendTo($("#tabsStatementAndReception"));

                switch (tagName) {
                    case 'statements-' + dep:
                        $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
                        documentsTable = new Statements(tagName, { appendTo: currentDialog, departmentID: dep, templateID: item.templateId, isReception: false });
                        break;
                    case 'receptions-' + dep:
                        $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
                        documentsTable = new Statements(tagName, { appendTo: currentDialog, departmentID: dep, templateID: item.templateId, isReception: true });
                        break;
                    case 'documents-' + dep:
                        $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                        documentsTable = new Documents(tagName, { appendTo: currentDialog, departmentID: dep, templateID: item.templateId });
                        break;
                    case 'asdocuments-' + dep:
                        $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                        documentsTable = new AsDocuments(tagName, { appendTo: currentDialog, departmentID: dep, templateID: item.templateId });
                        break;
                    case 'senatedecisions-' +dep:
                        $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                        documentsTable = new DocumentsTable(tagName, {
                            appendTo: currentDialog,
                            departmentID: dep,
                            templateID: item.templateId,
                            templateName: 'DecisionBlank',
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + item.templateId + '&type=getlist&isReception=true&departmentID=' + dep
                        });
                        break;
                }
            }
        });

        navigation.init();
    }
});
