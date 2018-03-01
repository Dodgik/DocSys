<%@ Page Title="Головна" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Document.Pages.Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/lib/fileuploader-1.1.4.js" type="text/javascript"></script>
    
    <script src="<%=RootURL%>js/models/models-1.0.3.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/ui-components-0.2.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/admin/adminDocuments-1.1.6.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/admin/adminDocumentBlank-1.1.3.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/admin/statements-1.1.5.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/admin/statementBlank-1.1.4.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/admin/controlCardBlankA-1.0.7.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/admin/statementControlCardBlank-1.1.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/admin/controlCardBlankD-1.1.4.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/admin/controlCardGroupDocument-1.1.4.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/org-ui-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/remoteScanner-1.0.0.js" type="text/javascript"></script>
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
                        var adminDocsTable = null;
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
                                        adminDocsTable.dispose();
                                    }
                                });
                                //sdialog.parent().appendTo($("#tabsStatementAndReception"));
                                
                                adminDocsTable = new Statements('documentsTest', { appendTo: sdialog, departmentID: dep, templateID: 1, isReception: false });
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
                                        adminDocsTable.dispose();
                                    }
                                });
                                //rdialog.parent().appendTo($("#tabsStatementAndReception"));

                                adminDocsTable = new Statements('documentsTest', { appendTo: rdialog, departmentID: dep, templateID: 2, isReception: true });
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
                                        adminDocsTable.dispose();
                                    }
                                });
                                //cdialog.parent().appendTo($("#tabsStatementAndReception"));

                                adminDocsTable = new AdminDocuments('adminDocsTable', {
                                    appendTo: cdialog,
                                    departmentID: dep,
                                    templateID: 3,
                                    url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=adocs&type=getpage&departmentID=' + userdata.departmentId
                                });
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
                                        adminDocsTable.dispose();
                                    }
                                });
                                //asDialog.parent().appendTo($("#tabsStatementAndReception"));

                                adminDocsTable = new AsDocuments('documentsTest', { appendTo: asDialog, departmentID: dep, templateID: 4 });
                                break;
                        }
                    }
                });

                navigation.init();
            }
            $('.gear').click(function() {
                $('.settings').dialog('open');
            });
            $('.settings').dialog({
                autoOpen: false,
                draggable: true,
                modal: true,
                resizable: true,
                width: 500,
                open: function () {
                    $('.settings input.filter-option').each(function () {
                        var $this = $(this);
                        if ($this.attr('type') == 'checkbox') {
                            $this.prop('checked', localStorage[$this.attr('name')] == 'true');
                        }
                    });
                },
                close: function () {

                }
            });
            $('.settings input.filter-option').change(function () {
                localStorage[$(this).attr('name')] = $(this).is(':checked');
            });
            
        });
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
    <div class="settings" style="display: none;" title="Налаштування">
        <p>
            <label><input class="filter-option" type="checkbox" name="filter_organizations_by_department" value="false">Фільтрувати списки організацій по підрозділу</label><br>
        </p>
    </div>
</asp:Content>