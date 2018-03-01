<%@ Page Title="Адмін-Екрани" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="AdminScreens.aspx.cs" Inherits="Document.Pages.AdminScreens" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>
    
    <script src="<%=RootURL%>js/menu-1.0.1.js" type="text/javascript"></script>
    <script src="<%=RootURL%>js/templateHelper-1.0.0.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyMain" runat="server">
    <table>
        <tr>
            <td style="vertical-align: top;">
                <div id="navigation-menu" class="ui-widget-content ui-corner-top ui-corner-bottom">
                </div>
            </td>
            <td style="vertical-align: top;">
                <div id="contentArea">
                
                    <div id="tabsStatementAndReception" style="width: 96%;">
                    </div>

                </div>
            </td>
        </tr>
    </table>
</asp:Content>
