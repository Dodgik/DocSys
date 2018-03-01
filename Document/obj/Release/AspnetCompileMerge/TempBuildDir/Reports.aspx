<%@ Page Title="Звіти" Language="C#" MasterPageFile="~/MasterPages/MainMasterPage.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="Document.Pages.Reports" %>

<asp:Content ID="contentOfHead" ContentPlaceHolderID="cphHead" runat="server">
    <script src="<%=RootURL%>js/navigation-1.0.2.js" type="text/javascript"></script>

    <script type="text/javascript">
        
        $(function () {
            $.datepicker.setDefaults($.datepicker.regional["uk"]);
            $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });

            $('#txtStartDate').datepicker({ changeMonth: true, changeYear: true });
            $('#txtEndDate').datepicker({ changeMonth: true, changeYear: true });

            $("#btnDownloadReportControledDocs").button({ icons: { primary: "ui-icon-ms-word"} }).click(function () {
                var reportType = $('#hdnReportType').val();
                var cd = $('#txtStartDate').val();
                var ed = $('#txtEndDate').val();
                var dep = $('#hdnSelDep').val();
                var tmp = parseFloat($('#hdnSelTmp').val());

                var params = '';

                if (tmp == 1 || tmp == 2) {
                    params = '?obj=docstrep&dep=' + dep + '&type=' + reportType + '&cd=' + cd + '&ed=' + ed;

                    if (reportType === "ssc") {
                        var sc = $('#pDsReportStatistics_SocialCategory select').val();
                        params = params + '&sc=' + sc;
                    }
                    if (reportType === "sbt") {
                        var bt = $('#pDsReportStatistics_BranchType select').val();
                        params = params + '&bt=' + bt;
                    }
                    if (reportType === "srh" || reportType === "hd") {
                        var hd = $('#hdnSelectedHeaderId').val();
                        params = params + '&hd=' + hd;
                    }
                    if (reportType === "wk") {
                        var wk = $('#hdnSelectedWorkerId').val();
                        params = params + '&wk=' + wk;
                    }
                    if (reportType === "sorg") {
                        params = params + '&org=' + $("#txtOrganization").attr('valueid');
                    }
                }
                else if (tmp == 3) {
                    params = '?t=3&dep=' + dep + '&type=rep&r=' + reportType + '&cd=' + cd + '&ed=' + ed;
                    
                    if (reportType === "cpw")
                        params = params + '&wk=' + $('#hdnSelectedWorkerId').val();
                    if (reportType != "dv")
                        params = params +
                            '&edid=' + $("#txtExecutiveDepartment").attr('valueid') +
                            '&dcid=' + $("#txtDocumentCode").attr('valueid') +
                            '&dtid=' + $("#txtDocType").attr('valueid') +
                            '&qtid=' + $("#txtQuestionType").attr('valueid');
                }

                var reportUrl = 'Handlers/DataPoint.ashx' + params;
                window.location.href = window.appSettings.rootUrl + reportUrl;
                return false;
            });

            if (navigationSettings.menuList.length > 0) {
                var menuList = [];
                for (var i = 0, item; item = navigationSettings.menuList[i]; i++) {
                    if (navigationSettings.menuList.length > 1)
                        menuList.push({ name: item.name, dep: item.id });

                    for (var j = 0, templ; templ = navigationSettings.templateList[j]; j++) {
                        //menuList.push({ name: templ.name, href: templ.systemName + '-' + item.id, dep: item.id });
                        if (templ.id == 1) {
                            $.datepicker.setDefaults({ dateFormat: 'yy-mm-dd' });
                            if (navigationSettings.templateList.length > 2)
                                menuList.push({ name: "  - " + templ.name, dep: item.id });
                            menuList.push({ name: 'Документи на контролі', href: 'c-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Кількість зареєстрованих', href: 'fs-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Звіт по категоріях', href: 'ssc-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Звіт по видах питань', href: 'sbt-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Повторні звернення', href: 'sis-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Колективні звернення', href: 'sist-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Особистий прийом', href: 'srh-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Звернення з організацій', href: 'sorg-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Особливий контроль', href: 'sc-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Дані розгляду звернень', href: 'hd-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Звіт по виконавцю', href: 'wk-' + item.id, dep: item.id, tmp: templ.id });
                        } else if (templ.id == 3) {
                            $.datepicker.setDefaults({ dateFormat: 'dd.mm.yy' });
                            if (navigationSettings.templateList.length > 2)
                                menuList.push({ name: "  - " + templ.name, dep: item.id });
                            menuList.push({ name: 'Документи на контролі', href: 'dc-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Не виконано', href: 'nd-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Виконано із запізненням', href: 'ml-' + item.id, dep: item.id, tmp: templ.id });
                            /*menuList.push({ name: 'План на виконавця', href: 'cpw-' + item.id, dep: item.id, tmp: templ.id });*/
                            menuList.push({ name: 'Кількість надходжень', href: 'qs-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Звіт про обсяг документообігу', href: 'dv-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Зведення про виконання документів', href: 'dcn-' + item.id, dep: item.id, tmp: templ.id });
                            menuList.push({ name: 'Перелік невиконаних документів', href: 'do-' + item.id, dep: item.id, tmp: templ.id });
                        }
                    }
                }

                navigation.create({
                    appendTo: '#navigation-menu',
                    list: menuList,
                    onSelect: function (e) {
                        var tagName = e.target.getAttribute('href').match((/#([\w\-]+)?/))[1];
                        var type = tagName.substr(0, tagName.indexOf('-'));
                        var dep = parseFloat(e.target.getAttribute('dep'));
                        var tmp = parseFloat(e.target.getAttribute('tmp'));

                        documentUI.departmentID = dep;
                        $('#hdnSelDep').val(dep);
                        $('#hdnSelTmp').val(tmp);
                        $('#hdnReportType').val(type);

                        function renderParamsForm() {
                            $('#reportParams h3 a').text(e.target.text);

                            $("#tblDateRange tr").each(function () {
                                $(this).show();
                                $($(this).children("td")[0]).addClass('ui-helper-hidden');
                                $($(this).children("td")[1]).removeClass('ui-helper-hidden');
                            });
                            $('#paragraf').hide();
                            $('#pDsReportStatistics_SocialCategory').hide();
                            $('#pDsReportStatistics_BranchType').hide();
                            $('#pDsReportStatistics_Header').hide();
                            $('#pDsReportStatistics_Organization').hide();
                            $('#pDsReportStatistics_Worker').hide();
                            $('#pDtReportStatistics_DocumentCode').hide();
                            $('#pDtReportStatistics_DocType').hide();
                            $('#pDtReportStatistics_QuestionType').hide();
                            $('#pDtReportStatistics_ExecutiveDepartment').hide();

                            $("#txtHeader").val('');
                            $("#hdnSelectedHeaderId").val('');
                            $("#txtWorkers").val('');
                            $("#hdnSelectedWorkerId").val('');
                            $("#txtDocumentCode").val('').attr('valueid', '0');
                            $("#txtDocType").val('').attr('valueid', '0');
                            $("#txtQuestionType").val('').attr('valueid', '0');
                            $("#txtExecutiveDepartment").val('').attr('valueid', '0');
                            

                            switch (tagName) {
                                case 'c-' + dep:
                                    $("#tblDateRange tr").each(function () {
                                        $($(this).children("td")[0]).removeClass('ui-helper-hidden');
                                        $($(this).children("td")[1]).addClass('ui-helper-hidden');
                                    });
                                    break;
                                case 'dc-' + dep:
                                    $('#pDtReportStatistics_ExecutiveDepartment').show();
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    $('#pDtReportStatistics_DocType').show();
                                    $('#pDtReportStatistics_QuestionType').show();
                                    $("#tblDateRange tr").each(function () {
                                        $($(this).children("td")[0]).removeClass('ui-helper-hidden');
                                        $($(this).children("td")[1]).addClass('ui-helper-hidden');
                                    });
                                    break;
                                case 'nd-' +dep:
                                    $('#pDtReportStatistics_ExecutiveDepartment').show();
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    $('#pDtReportStatistics_DocType').show();
                                    $('#pDtReportStatistics_QuestionType').show();
                                    $("#tblDateRange tr").each(function (n) {
                                        if (n == 1)
                                            $(this).hide();
                                        $($(this).children("td")[0]).removeClass('ui-helper-hidden');
                                        $($(this).children("td")[1]).addClass('ui-helper-hidden');
                                    });
                                    break;
                                case 'ml-' +dep:
                                    $('#pDtReportStatistics_ExecutiveDepartment').show();
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    $('#pDtReportStatistics_DocType').show();
                                    $('#pDtReportStatistics_QuestionType').show();
                                    break;
                                case 'fs-' + dep:
                                    $('#paragraf').show();
                                    break;
                                case 'ssc-' + dep:
                                    $('#pDsReportStatistics_SocialCategory').show();
                                    break;
                                case 'sbt-' + dep:
                                    $('#pDsReportStatistics_BranchType').show();
                                    break;
                                case 'srh-' + dep:
                                case 'hd-' + dep:
                                    $('#pDsReportStatistics_Header').show();
                                    break;
                                case 'sorg-' + dep:
                                    $('#pDsReportStatistics_Organization').show();
                                    break;
                                case 'wk-' + dep:
                                    $('#pDsReportStatistics_Worker').show();
                                    break;
                                case 'qs-' + dep:
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    break;
                                    
                                case 'cpw-' + dep:
                                    $('#pDtReportStatistics_ExecutiveDepartment').show();
                                    $("#tblDateRange tr").each(function (i) {
                                        if (i == 1)
                                            $(this).hide();
                                        $($(this).children("td")[0]).removeClass('ui-helper-hidden');
                                        $($(this).children("td")[1]).addClass('ui-helper-hidden');
                                    });
                                    $('#pDsReportStatistics_Worker').show();
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    $('#pDtReportStatistics_DocType').show();
                                    $('#pDtReportStatistics_QuestionType').show();
                                    break;
                                case 'dcn-' +dep:
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    $("#tblDateRange tr").each(function (i) {
                                        if (i == 1)
                                            $(this).hide();
                                        $($(this).children("td")[0]).removeClass('ui-helper-hidden');
                                        $($(this).children("td")[1]).addClass('ui-helper-hidden');
                                    });
                                    break;
                                case 'do-' +dep:
                                    $('#pDtReportStatistics_DocumentCode').show();
                                    $('#pDtReportStatistics_ExecutiveDepartment').show();
                                    $("#tblDateRange tr").each(function (i) {
                                        if (i == 1)
                                            $(this).hide();
                                        $($(this).children("td")[0]).removeClass('ui-helper-hidden');
                                        $($(this).children("td")[1]).addClass('ui-helper-hidden');
                                    });
                                    break;
                            }
                        }

                        if ($("#reportParams").is(':visible')) {
                            $("#reportParams").hide("slide", {}, 500, function () {
                                renderParamsForm();
                                $("#reportParams").show("slide", {}, 1000);
                            });
                        }
                        else {
                            renderParamsForm();
                            $("#reportParams").show("slide", {}, 1000);
                        }
                    }
                });

                navigation.init();


                $("#txtHeader").autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $("#hdnSelectedHeaderId").val(ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtHeader");

                $("#txtWorkers").autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=worker&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $("#hdnSelectedWorkerId").val(ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtWorkers");

                $('#txtExecutiveDepartment').autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=dep&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $("#txtExecutiveDepartment").attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtExecutiveDepartment");
                
                $("#txtDocumentCode").autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=documentcode&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[0] + '. ' + item[1],
                                        value: item[0] + '. ' + item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $("#txtDocumentCode").attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtDocumentCode");
                
                $('#txtDocType').autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=doctype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term + '&code=' + $("#txtDocumentCode").attr('valueid'),
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtDocType");

                $("#txtQuestionType").autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=questiontype&type=search&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $(this).attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtQuestionType");

                $('#txtOrganization').autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: function (request, response) {
                        $.ajax({
                            url: appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=organization&type=search&orgtype=1&dep=' + documentUI.departmentID + '&term=' + request.term,
                            type: "GET",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data, function (item) {
                                    return {
                                        id: parseInt(item[0]),
                                        label: item[1],
                                        value: item[1],
                                        option: this
                                    };
                                }));
                            }
                        });
                    },
                    select: function (event, ui) {
                        $("#txtOrganization").attr('valueid', ui.item.id);
                    }
                }).addClass("ui-widget ui-widget-content ui-corner-left");
                documentUI.addButtonToAutocomplete("#txtOrganization");
                
            }
        });
        
    </script>
</asp:Content>
<asp:Content ID="contentOfBody" ContentPlaceHolderID="cphBodyMain" runat="server">
<table>
    <tr>
        <td style="vertical-align: top;">
            <div id="navigation-menu" class="ui-widget-content ui-corner-top ui-corner-bottom">
            </div>
        </td>
        <td style="vertical-align: top;">
            <div id="contentArea" >

                <div class="content-st">
                    <div id="demo-frame-wrapper">
                        <div id="reportParams" class="ui-helper-hidden ui-accordion ui-widget ui-helper-reset ui-accordion-icons">
                            <input id="hdnSelDep" type="hidden" value="0" />
                            <input id="hdnSelTmp" type="hidden" value="0" />
                            <input id="hdnReportType" type="hidden" value="" />
	                        <h3 class="ui-accordion-header ui-helper-reset ui-state-default ui-state-active ui-corner-top"><a href="#">Документи на контролі</a></h3>
	                        <div class="ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active">
                                <p id="pDsReportStatistics_SocialCategory" class="ui-helper-hidden">
                                    Категорія:
                                    <asp:DropDownList ID="ddlSocialCategory" ClientIDMode="Static" runat="server" DataTextField="Name" DataValueField="SocialCategoryID">
                                    </asp:DropDownList>
                                </p>
                                <p id="pDsReportStatistics_BranchType" class="ui-helper-hidden">
                                    Вид питання:
                                    <asp:DropDownList ID="ddlBranchType" ClientIDMode="Static" runat="server" DataTextField="Name" DataValueField="BranchTypeID">
                                    </asp:DropDownList>
                                </p>
                                <p id="pDsReportStatistics_Header" class="ui-helper-hidden">
                                    Керівник прийому:
                                    <input id="txtHeader" type="text" />
                                    <input id="hdnSelectedHeaderId" type="hidden" />
                                </p>
                                <p id="pDsReportStatistics_Organization" class="ui-helper-hidden">
                                    Організація:
                                    <br/>
                                    <input id="txtOrganization" type="text" valueid="0" style="width: 526px;" />
                                </p>
                                <p id="pDsReportStatistics_Worker" class="ui-helper-hidden">
                                    Виконавець:
                                    <input id="txtWorkers" type="text" />
                                    <input id="hdnSelectedWorkerId" type="hidden" />
                                </p>
                                <p id="pDtReportStatistics_DocumentCode" class="ui-helper-hidden">
                                    Шифр:
                                    <input id="txtDocumentCode" type="text" valueid="0" style="width: 260px;" />
                                </p>
                                <p id="pDtReportStatistics_DocType" class="ui-helper-hidden">
                                    Найменування документа:
                                    <input id="txtDocType" type="text" valueid="0" style="width: 260px;" />
                                </p>
                                <p id="pDtReportStatistics_QuestionType" class="ui-helper-hidden">
                                    Категорія питання:
                                    <input id="txtQuestionType" type="text" valueid="0" style="width: 260px;" />
                                </p>
                                <p id="pDtReportStatistics_ExecutiveDepartment" class="ui-helper-hidden">
                                    Відомство:
                                    <input id="txtExecutiveDepartment" type="text" valueid="0" style="width: 260px;" />
                                </p>
                                <p id="paragraf" class="ui-helper-hidden">
                                    Заяви зареєстровані за період:
                                </p>
                                <table id="tblDateRange" style="width: 100%;">
                                    <tr>
                                        <td>
                                            Станом на:
                                        </td>
                                        <td class="ui-helper-hidden">
                                            з:
                                        </td>
                                        <td>
                                            <input id="txtStartDate" type="text" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Виконати до:
                                        </td>
                                        <td class="ui-helper-hidden">
                                            по:
                                        </td>
                                        <td>
                                            <input id="txtEndDate" type="text" />
                                        </td>
                                    </tr>
                                </table>
                                <button id="btnDownloadReportControledDocs" title="Завантажити звіт">Завантажити звіт</button>
	                        </div>
                        </div>
	                </div>
                </div>

            </div>
        </td>
    </tr>
</table>
</asp:Content>