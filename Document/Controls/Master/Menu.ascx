<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Document.Controls.Master.Menu" %>
<div id="menu">
    <ul>
        <li>
            <a href="Home.aspx" class="header">Кабінет</a>
        </li>
    </ul>
    <ul>
        <li>
            <a id="linkHomePage" href="Main.aspx" class="header">Головна</a>
        </li>
    </ul>
    <ul>
        <li>
            <a href="Reports.aspx" class="header">Звіти</a>
        </li>
    </ul>
    <ul>
        <li>
            <a href="Service.aspx" class="header">Сервіс</a>
        </li>
    </ul>
    <ul>
        <li>
            <a href="AdminScreens.aspx" class="header">Адмін-Екрани</a>
        </li>
    </ul>
    <ul style="float: right;">
        <li>
            <a href="Help.aspx" class="header">Довідка</a>
        </li>
    </ul>
    <ul style="float: right;">
        <li class="gear">
           <img src="resources/images/setting.png" width="20" height="20" alt="gear"></a>
        </li>
    </ul>
</div>
