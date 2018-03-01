<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="Service.aspx.cs" Inherits="Document.Pages.Service" %>

<asp:Content ID="contentOfHead" ContentPlaceHolderID="cphHead" runat="server">
    
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/hrm-ui-1.0.8.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/admin/adminOrg-1.0.1.js" type="text/javascript"></script>

</asp:Content>

<asp:Content ID="contentOfBody" ContentPlaceHolderID="cphBodyMain" runat="server">
<table>
    <tr>
        <td style="vertical-align: top;">
            <div id="navigation-menu" class="ui-widget-content ui-corner-top ui-corner-bottom">
            </div>
        </td>
        <td style="vertical-align: top;">
            <div id="contentArea">
                
            </div>
        </td>
    </tr>
</table>

<script type="text/javascript">
    $(function () {

        if (navigationSettings.menuList.length > 0) {
            var menuList = [];
            for (var i = 0, item; item = navigationSettings.menuList[i]; i++) {
                if (navigationSettings.menuList.length > 1) {
                    menuList.push({ name: item.name, dep: item.id });

                }
                menuList.push({ name: 'Працівники', href: 'workers-' + item.id, dep: item.id });
                menuList.push({ name: 'Посади працівників', href: 'post-' + item.id, dep: item.id });
                menuList.push({ name: 'Відомства', href: 'departments-' + item.id, dep: item.id });
                menuList.push({ name: 'Організації', href: 'organizations-' + item.id, dep: item.id });
            }
            navigation.create({
                appendTo: '#navigation-menu',
                list: menuList,
                onSelect: function (e) {
                    var tagName = e.target.getAttribute('href').match((/#([\w\-]+)?/))[1];
                    var dep = parseInt(e.target.getAttribute('dep'));
                    switch (tagName) {
                        case 'workers-' + dep:
                            documentUI.departmentID = dep;
                            
                            var wManager = null;
                            var wManagerDlg = $('<div title="Працівники"></div>').dialog({
                                //appendTo: '#contentArea',
                                autoOpen: true,
                                draggable: true,
                                modal: true,
                                position: [0, 0],
                                resizable: true,
                                minWidth: 500,
                                width: 860,
                                open: function (event, ui) {
                                    //if ($.ui.dialog.overlay.instances.length > 0)
                                    //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                                },
                                close: function (event, ui) {
                                    wManagerDlg.remove();
                                    navigation.deselect();
                                    wManager.dispose();
                                }
                            });
                            //wManagerDlg.parent().appendTo('div[id=contentArea]');
                            wManager = new WorkerUi({ appendTo: wManagerDlg, departmentID: dep, templateID: 3 });
                            
                            break;
                        case 'post-' + dep:
                            documentUI.departmentID = dep;
                            
                            var postManager = null;
                            var postManagerDlg = $('<div title="Посади"></div>').dialog({
                                //appendTo: '#contentArea',
                                autoOpen: true,
                                draggable: true,
                                modal: true,
                                position: [0, 0],
                                resizable: true,
                                minWidth: 500,
                                width: 860,
                                open: function (event, ui) {
                                    //if ($.ui.dialog.overlay.instances.length > 0)
                                    //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                                },
                                close: function (event, ui) {
                                    postManagerDlg.remove();
                                    navigation.deselect();
                                    postManager.dispose();
                                }
                            });
                            //postManagerDlg.parent().appendTo('div[id=contentArea]');
                            postManager = new PostUi({ appendTo: postManagerDlg, departmentID: dep, templateID: 3 });
                            break;
                        case 'departments-' +dep:
                            documentUI.departmentID = dep;

                            var depManager = null;
                            var depManagerDlg = $('<div title="Відомства"></div>').dialog({
                                //appendTo: '#contentArea',
                                autoOpen: true,
                                draggable: true,
                                modal: true,
                                position: [0, 0],
                                resizable: true,
                                minWidth: 500,
                                width: 860,
                                open: function (event, ui) {
                                    //if ($.ui.dialog.overlay.instances.length > 0)
                                    //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                                },
                                close: function (event, ui) {
                                    depManagerDlg.remove();
                                    navigation.deselect();
                                    depManager.dispose();
                                }
                            });
                            //depManagerDlg.parent().appendTo('div[id=contentArea]');
                            depManager = new DepartmentUi({ appendTo: depManagerDlg, departmentID: dep, templateID: 3 });
                            break;
                        case 'organizations-' +dep:
                            documentUI.departmentID = dep;

                            var orgManager = null;
                            var orgManagerDlg = $('<div title="Організації"></div>').dialog({
                                //appendTo: '#contentArea',
                                autoOpen: true,
                                draggable: true,
                                modal: true,
                                position: [0, 0],
                                resizable: true,
                                minWidth: 600,
                                width: 860,
                                open: function (event, ui) {
                                    //if ($.ui.dialog.overlay.instances.length > 0)
                                    //    $($.ui.dialog.overlay.instances[$.ui.dialog.overlay.instances.length - 1]).css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                                },
                                close: function (event, ui) {
                                    orgManagerDlg.remove();
                                    navigation.deselect();
                                    orgManager.dispose();
                                }
                            });
                            /*
                            var orgsEl = $('<div>').appendTo(orgManagerDlg);
                            orgManager = new Organizations({
                                serviceUrl: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=org',
                                element: orgsEl,
                                organizationTypeID: 2,
                                onSelect: function (sRow) {
                                    //self.fields.Document.ExternalSource.Organization.attr('valueid', sRow.id).val(sRow.name);
                                    orgManagerDlg.dialog("close");
                                },
                                onClose: function () {
                                    orgManagerDlg.dialog("close");
                                }
                            });
                            */
                            orgManager = new OrganizationsUi({ appendTo: orgManagerDlg, departmentID: dep, templateID: 3, organizationTypeID: 2 });
                            break;
                    }
                }
            });

            navigation.init();
        }
    });
</script>
</asp:Content>