<%@ Page Title="Головна" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="MainOld.aspx.cs" Inherits="Document.Pages.MainOld" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/lib/fileuploader-1.1.4.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/documents-1.0.8.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/documentBlank-1.1.1.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/statements-1.0.5.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/statementBlank-1.0.7.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/asDocuments-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/asDocumentBlank-1.0.1.js" type="text/javascript"></script>
    
    <script src="<%=RootURL%>js/controlCardBlankA-1.0.3.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/controlCardBlankD-1.0.8.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/org-ui-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/remoteScanner-1.0.0.js" type="text/javascript"></script>
    
    <script type="text/javascript">

        $(function () {
            var mList = window.navigationSettings.menuList,
                tList = window.navigationSettings.templateList;
            if (mList.length > 0) {
                var menuList = [];
                for (var i = 0, item; item = mList[i]; i++) {
                    if (mList.length > 1) {
                        menuList.push({ name: item.name, dep: item.id });

                    }
                    for (var j = 0, templ; templ = tList[j]; j++) {
                        menuList.push({ name: templ.name, href: templ.systemName + '-' + item.id, dep: item.id });
                    }
                }
                navigation.create({
                    appendTo: '#navigation-menu',
                    list: menuList,
                    onSelect: function (e) {
                        var tagName = e.target.getAttribute('href').match((/#([\w\-]+)?/))[1];
                        var dep = parseInt(e.target.getAttribute('dep'));
                        documentUI.departmentID = dep;
                        var documentsTestUi = null;
                        switch (tagName) {
                            case 'statements-' + dep:
                                $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
                                $("#tabsStatementAndReception").empty();
                                
                                var sdialog = $('<div></div>').dialog({
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
                                        documentsTestUi.dispose();
                                    }
                                });
                                //sdialog.parent().appendTo($("#tabsStatementAndReception"));
                                
                                documentsTestUi = new Statements('documentsTest', { appendTo: sdialog, departmentID: dep, templateID: 1, isReception: false });
                                break;
                            case 'receptions-' + dep:
                                $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
                                $("#tabsStatementAndReception").empty();

                                var rdialog = $('<div></div>').dialog({
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
                                        documentsTestUi.dispose();
                                    }
                                });
                                //rdialog.parent().appendTo($("#tabsStatementAndReception"));

                                documentsTestUi = new Statements('documentsTest', { appendTo: rdialog, departmentID: dep, templateID: 2, isReception: true });
                                break;
                            case 'documents-' + dep:
                                $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                                $("#tabsStatementAndReception").empty();

                                var cdialog = $('<div></div>').dialog({
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
                                        documentsTestUi.dispose();
                                    }
                                });
                                //cdialog.parent().appendTo($("#tabsStatementAndReception"));

                                documentsTestUi = new Documents('documentsTest', { appendTo: cdialog, departmentID: dep, templateID: 3 });
                                break;
                            case 'asdocuments-' + dep:
                                $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                                $("#tabsStatementAndReception").empty();

                                var asDialog = $('<div></div>').dialog({
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
                                        asDocumentsTestUi.dispose();
                                    }
                                });
                                //asDialog.parent().appendTo($("#tabsStatementAndReception"));

                                asDocumentsTestUi = new AsDocuments('documentsTest', { appendTo: asDialog, departmentID: dep, templateID: 4 });
                                break;
                        }
                    }
                });

                navigation.init();
            }
        });

        var loadBinaryResource = function(url) {
            var req = new XMLHttpRequest();
            req.open('GET', url, false);

            if (req.overrideMimeType)
                req.overrideMimeType('text/plain; charset=x-user-defined');
            req.send(null);
            if (req.status != 200)
                return '';
            if (typeof(req.responseBody) !== 'undefined')
                return BinaryArrayToAscCSV(req.responseBody);
            return req.responseText;
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyMain" runat="server">
    <table>
        <tr>
            <td style="vertical-align: top;">
                <div id="navigation-menu" class="ui-widget-content ui-corner-top ui-corner-bottom">
                </div>
            </td>
            <td style="vertical-align: top;">
                <div id="contentArea" >
                
                    <div id="tabsStatementAndReception" style="width: 96%;">
                    </div>

                </div>
            </td>
        </tr>
    </table>
</asp:Content>