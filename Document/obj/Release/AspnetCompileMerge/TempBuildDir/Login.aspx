<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Document.Login" Theme="Default" %>

<%@ Register src="~/Controls/Master/UserLogin.ascx" tagname="UserLogin" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" /> 
    <title>login page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width:100%; text-align:center; height: 600px; vertical-align: middle;">
                <tr>
                    <td align="center" >
                        <uc1:UserLogin ID="ucUserLogin" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>