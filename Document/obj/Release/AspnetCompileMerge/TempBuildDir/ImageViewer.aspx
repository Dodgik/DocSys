<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageViewer.aspx.cs" Inherits="Document.ImageViewer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="content-type" /> 
    <title>Переглядач</title>
    <script src="js/lib/jquery-1.9.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        var getParameterByName = function(name) {
            name = name.replace( /[\[]/ , "\\\[").replace( /[\]]/ , "\\\]");
            var regexS = "[\\?&]" + name + "=([^&#]*)";
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.search);
            if (results == null)
                return "";
            else
                return decodeURIComponent(results[1].replace( /\+/g , " "));
        };

        $(function () {
            var documentID = getParameterByName('documentID');
            if (documentID) {
                var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=file&type=getlist&documentid=' + documentID;

                $.ajax({
                    type: "GET",
                    cache: false,
                    url: urlRequest,
                    dataType: "json",
                    success: function(data) {
                        for (var i in data) {
                            if ($('#imgSelected').attr('src') == '')
                                $('#imgSelected').attr('src', appSettings.rootUrl + 'File.ashx?id=' + data[i].FileID);

                            $('#imgList').append($('<img style="width:110px; height:170px; padding: 0px 0px 5px 0px; cursor: pointer;" />').attr('src', appSettings.rootUrl + 'File.ashx?id=' + data[i].FileID)
                                    .click(function () {
                                $('#imgSelected').attr('src', $(this).attr('src'));
                            }));
                        }
                    }
                });
            }
        });
    </script>
</head>
<body style="background-color: #C5E6F9;">
    <div>
    <table cellspacing="0" cellpadding="0" border="0" width="100%" style="margin: 0px 0px 0px 0px; background-color:#FFFFEE;">
        <tr>
            <td id="imgList" valign="top" align="center" style="width:110px; padding: 0px 10px 0px 5px;">            

            </td>
            <td width="1010px" valign="top" align="center">
                <table cellspacing="0" cellpadding="0" border="0" width="100%" style="margin: 0px 0px 0px 0px;">
                    <tr>
                        <td valign="top" align="center">
                            <img id="imgSelected" border="1px" width="1000" style="border-color:#9fbdff;" alt="" src="" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </div>
</body>
</html>