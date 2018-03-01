<%@ Page Title="Головна" Language="C#" MasterPageFile="~/MasterPages/WorkerMasterPage.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Document.Pages.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/lib/fileuploader-1.1.4.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/models/models-1.0.3.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/ui-components-0.2.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/worker/workerInputDocuments-1.0.3.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/worker/workerOutputDocuments-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/worker/workerDraftDocuments-1.0.0.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/worker/inputDocumentBlank-1.0.4.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/worker/outputDocumentBlank-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/worker/draftDocumentBlank-1.0.1.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/documents-1.0.8.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/documentBlank-1.1.1.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/controlCardBlankA-1.0.3.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/controlCardBlankD-1.0.8.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/org-ui-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/remoteScanner-1.0.0.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/worker/workerStatements-1.0.8.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/worker/workerStatementBlank-1.0.8.js" type="text/javascript"></script>
    
    <script type="text/javascript">
        window.appData = window.appData || {};
        window.appData.resolutions = [
            '{виконавець} - До відома',
            '{виконавець} - До відома та відповідного реагування',
            '{виконавець} - До відома та врахування в роботі',
            '{виконавець} - До виконання',
            '{виконавець} - Для використання в роботі',
            '{виконавець} - Для вирішення',
            '{виконавець} - Зайдіть',
            '{виконавець} - Пропозиції',
            '{виконавець} - Прошу вирішити',
            '{виконавець} - Прошу взяти участь',
            '{виконавець} - Прошу надати інформацію',
            '{виконавець} - Прошу розглянути',
            '{виконавець} - Прошу розглянути та внести пропозиції',
            '{виконавець} - Прошу розглянути. Інформуйте МВК',
            '{виконавець} - Прошу підготувати відповідь',
            '{виконавець} - Прошу підготувати інформацію',
            '{виконавець} - Прошу підготувати необхідні документи',
            '{виконавець} - Прошу підготувати рішення',
            '{виконавець} - Прошу проінформувати',
            '{виконавець} - Прошу надати копії необхідних документів',
            'Контроль'
        ];
    </script>

    <script type="text/javascript">

        function updateCountDocs() {
            var urlRequest = window.appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getdtcount&dep=' + userdata.departmentId;
            $.ajax({
                url: urlRequest,
                type: 'GET',
                cache: false,
                dataType: "json",
                success: function (msg) {
                    $('.count-111').text('(' + msg + ')');
                }
            });

            window.setTimeout(function () {
                var urlRequest2 = window.appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getdtconcount&dep=' + userdata.departmentId;
                $.ajax({
                    url: urlRequest2,
                    type: 'GET',
                    cache: false,
                    dataType: "json",
                    success: function (msg) {
                        $('.count-11').text('(' + msg + ')');
                    }
                });
            }, 2000);

            window.setTimeout(function () {
                var urlRequest2 = window.appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getstcount&dep=' + userdata.departmentId;
                $.ajax({
                    url: urlRequest2,
                    type: 'GET',
                    cache: false,
                    dataType: "json",
                    success: function (msg) {
                        $('.count-444').text('(' + msg + ')');
                    }
                });
            }, 4000);

            window.setTimeout(function () {
                var urlRequest2 = window.appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getstconcount&dep=' + userdata.departmentId;
                $.ajax({
                    url: urlRequest2,
                    type: 'GET',
                    cache: false,
                    dataType: "json",
                    success: function (msg) {
                        $('.count-44').text('(' + msg + ')');
                    }
                });
            }, 6000);
        }

        $(function () {
            $('#btn-create').click(function() {
                var currentDocument = new DraftDocumentBlank({
                    departmentID: userdata.departmentId,
                    templateID: 3,
                    success: function (msg) {
                        
                    }
                });
                currentDocument.showInsertForm();
                return false;
            });

            navigation.create({
                appendTo: '#navigation-menu',
                list: [{ name: 'Документи', count: false },
                    { name: 'Нові', href: '111', dep: 111, count: true },
                    { name: 'На контролі', href: '11', dep: 11, count: true },
                    { name: 'Всі', href: '1', dep: 1, count: false },
                    /*{ name: 'Вихідні', href: '2', dep: 2 }, { name: 'Чернетки', href: '3', dep: 3 },*/
                    { name: 'Заяви та звернення', count: false },
                    { name: 'Нові', href: '444', dep: 444, count: true },
                    { name: 'На контролі', href: '44', dep: 44, count: true },
                    { name: 'Всі', href: '4', dep: 4, count: false }],
                onSelect: function (e) {
                    var tagName = e.target.getAttribute('href').match((/#([\w\-]+)?/))[1];
                    var dep = parseInt(e.target.getAttribute('dep'));
                    documentUI.departmentID = dep;
                    var documentsTable = null;
                    switch (tagName) {
                        case '111':
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //asDialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerInputDocuments('inputDocuments', {
                                appendTo: asDialog,
                                departmentID: userdata.departmentId,
                                templateID: 3,
                                isOpenWorker: false,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '11':
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //asDialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerInputDocuments('inputDocuments', {
                                appendTo: asDialog,
                                departmentID: userdata.departmentId,
                                templateID: 3,
                                controlled: true,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '1':
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //asDialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerInputDocuments('inputDocuments', {
                                appendTo: asDialog,
                                departmentID: userdata.departmentId,
                                templateID: 3,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '2':
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //cdialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerOutputDocuments('outputDocuments', {
                                appendTo: cdialog,
                                departmentID: userdata.departmentId,
                                templateID: 3,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getoutputpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '3':
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //cdialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerDraftDocuments('outputDocuments', {
                                appendTo: cdialog,
                                departmentID: userdata.departmentId,
                                templateID: 3,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getdraftpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '444':
                            $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //sdialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerStatements('documentsTest', {
                                appendTo: cdialog,
                                departmentID: userdata.departmentId,
                                templateID: 1,
                                isReception: false,
                                isOpenWorker: false,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getstpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '44':
                            $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //sdialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerStatements('documentsTest', {
                                appendTo: cdialog,
                                departmentID: userdata.departmentId,
                                templateID: 1,
                                isReception: false,
                                controlled: true,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getstpage&departmentID=' + userdata.departmentId
                            });
                            break;
                        case '4':
                            $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
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
                                    documentsTable.dispose();
                                    updateCountDocs();
                                }
                            });
                            //sdialog.parent().appendTo($("#tabsStatementAndReception"));

                            documentsTable = new WorkerStatements('documentsTest', {
                                appendTo: cdialog,
                                departmentID: userdata.departmentId,
                                templateID: 1,
                                isReception: false,
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=wdocs&type=getstpage&departmentID=' + userdata.departmentId
                            });
                            break;
                    }
                }
            });

            navigation.init();
            updateCountDocs();
            setInterval(function() {
                updateCountDocs();
            }, 60000);
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyMain" runat="server">
    <table class="content-table">
        <tr>
            <td colspan="2" style="vertical-align: top;">
                <div class="top-bar ui-widget-content ui-corner-top ui-corner-bottom">
                    <button id="btn-create" class="ui-button ui-widget ui-state-default ui-corner-all">Створити</button>
                </div>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top;">
                <div id="navigation-menu" class="ui-widget-content ui-corner-top ui-corner-bottom"></div>
            </td>
            <td style="vertical-align: top;">
                <div id="contentArea">
                    <div id="tabsStatementAndReception" style="width: 96%;"></div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>