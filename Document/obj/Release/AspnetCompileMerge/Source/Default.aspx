<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Document.Pages.Default" %>

<%@ Register src="~/Controls/Master/UserLogin.ascx" tagname="UserLogin" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyMain" runat="server">
    <table style="width:100%; text-align:center;">
        <tr>
            <td align="center" >
                <uc1:UserLogin ID="ucUserLogin" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>