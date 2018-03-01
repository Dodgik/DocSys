<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.ascx.cs" Inherits="Document.Controls.Master.UserLogin" %>

<asp:Login ID="LoginForm" VisibleWhenLoggedIn="false" runat="server" >
    <LayoutTemplate>
        <table>
            <tr>
                <td colspan="2" style="background-color:#0e71bd; padding:6px; text-align: center;">
                    Авторизація
                </td>
            </tr>
            <tr>
                <td style="padding:5px 5px 0 5px">
                    Логін:
                </td>
                <td  style="padding:3px 0 3px 0">
                    <asp:TextBox ID="UserName" style="width:150px" runat="server" />
                    <asp:RequiredFieldValidator ID="UsernameValidator" ControlToValidate="UserName" runat="server" ErrorMessage="Невірний логін" Text="*" />                    
                </td>                                                                                
            </tr>
            <tr>
                <td style="padding:5px 5px 0 5px">
                    Пароль:
                </td>
                <td  style="padding:3px 0 3px 0">
                    <asp:TextBox ID="Password" TextMode="Password" style="width:150px" runat="server" />
                    <asp:RequiredFieldValidator ID="PasswordValid" ControlToValidate="Password" runat="server" ErrorMessage="Невірний Пароль" Text="*" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="padding:5px">

                </td>
            </tr>
            <tr>
                <td colspan="2" style="padding:5px">
                    <asp:Button ID="Login" runat="server" Text="Увійти" CommandName="Login" style="width:220px; padding:3px" onclick="Login_Click" />
                </td>
            </tr>
        </table>
    </LayoutTemplate>
</asp:Login>