<%@ Page Title="Головна" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Document.Pages.Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/org-ui-1.0.0.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/remoteScanner-1.0.0.js" type="text/javascript"></script>

    <script src="<%=RootURL%>js/users-0.0.1.js" type="text/javascript"></script>
    
    <script type="text/javascript">

        $(function () {

            var menuList = [];
            menuList.push({ name: 'Користувачі', href: 'users' + '-' + 0, dep: 0 });
            
            navigation.create({
                appendTo: '#navigation-menu',
                list: menuList,
                onSelect: function (e, item) {
                    var tagName = e.target.getAttribute('href').match((/#([\w\-]+)?/))[1];
                    var dep = parseInt(e.target.getAttribute('dep')),
                    currentTable = null,
                    currentDialog;
                    

                    $("#windowPlace").empty();
                    currentDialog = $('<div></div>').dialog({
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
                            currentTable.dispose();
                        }
                    });

                    switch (tagName) {
                        case 'users-' +dep:
                           $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                            currentTable = new Users(tagName, {
                                appendTo: currentDialog,
                                departmentID: dep,
                                templateID: item.templateId,
                                templateName: 'UserBlank',
                                url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=users&type=getpage&departmentID=' + dep
                            });
                            break;
                    }
                }
            });

            navigation.init();
            
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
                
                    <div id="windowPlace" style="width: 96%;">
                    </div>

                </div>
            </td>
        </tr>
    </table>
</asp:Content>