(function (window) {

    var appSettings = window.appSettings;
    var outputDocumentBlank = function (options) {
        var self = this,
            thisDoc = undefined;

        this.success = null;
        this.document = {};
        this.departmentID = 0;
        this.templateID = 0;

        if (options) {
            if (options.success)
                this.success = options.success;
            if (options.departmentID)
                this.departmentID = options.departmentID;
            if (options.templateID)
                this.templateID = options.templateID;
        }


        this.blank = null,
        this.form = null,
        this.dialog = null,
        this.fields = {},
        this.buttons = {},
        this.actionPanel = null,

        this.createForm = function (docData) {
            this.dispose();

            var row = function (name, obj) {
                return $('<tr class="blank-row"></tr>').append($('<td class="blank-row-title">' + name + '</td>')).append($('<td></td>').append(obj));
            };
            var table = $('<table></table>');

            table.append(row('', 'Вихідний'));
            table.append(row('Шифр:', docData.Document.CodeName ? docData.Document.CodeID + '. ' + docData.Document.CodeName : ''));
            table.append(row('Найменування документа:', docData.DocType ? docData.DocType.Name : ''));
            table.append(row('Категорія питання:', (docData.QuestionType && docData.QuestionTypeID > 0) ? docData.QuestionType.Name : ''));
            table.append(row('&nbsp;'));

            table.append(row('Адресат:'));
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Організація:', docData.Document.Destination.OrganizationID > 0 ? docData.Document.Destination.OrganizationName : ''));
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Підрозділ:', docData.Document.Destination.DepartmentID > 0 ? docData.Document.Destination.DepartmentName : ''));

            var sWorker = docData.Document.Destination.Worker;
            var sWorkerName = docData.Document.Destination.WorkerID > 0 ? documentUI.formatFullName(sWorker.LastName, sWorker.FirstName, sWorker.MiddleName) : '';
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Працівник:', sWorkerName));
            table.append(row('&nbsp;'));
            
            var ecDateStr = '';
            if (docData.Document.Source.CreationDate) {
                var ecDate = new Date(+docData.Document.Source.CreationDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                ecDateStr = (ecDate.getDate() <= 9 ? '0' : '') + ecDate.getDate() + '.' + (ecDate.getMonth() < 9 ? 0 : '') + (ecDate.getMonth() + 1) + '.' + ecDate.getFullYear();
            }
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Дата складання:', ecDateStr));
            table.append(row('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Номер документа:', docData.Document.Source.Number));
            table.append(row('&nbsp;'));

            table.append(row('Короткий зміст документа:', docData.Content));


            table.append(row('Підготовив:', (docData.Worker && docData.WorkerID > 0) ? documentUI.formatFullName(docData.Worker.LastName, docData.Worker.FirstName, docData.Worker.MiddleName) : ''));
            table.append(row('За підписом:', (docData.Head && docData.HeadID > 0) ? documentUI.formatFullName(docData.Head.LastName, docData.Head.FirstName, docData.Head.MiddleName) : ''));



            var files = docData.Document.Files;
            var uploadButton = $('<div></div>');
            table.append(row('Прикріплені файли:', uploadButton));
            var uploadList = $('<ul class="qq-upload-list"></ul>').appendTo(uploadButton);
            for (var i in files) {
                var fileUrl = appSettings.rootUrl + 'File.ashx?id=' + files[i].FileID,
                openUrl = appSettings.rootUrl + 'ImageViewer.aspx?documentID=' + docData.DocumentID + '&?id=' + files[i].FileID;
                uploadList.append('<li><a target="_blank" href="' + fileUrl + '">' + files[i].FileName + '</a> <a target="_blank" href="' + openUrl + '">Перегляд</a></li>');
            }


            this.blank = $('<div style="border: 1px dashed #C0C0C0; font-weight: bold;" class="form-font-big"></div>').append(table);
            this.form = $('<div title="Документ" style="display:none;"></div>').append(this.blank);
            $('body').append(this.form);


            this.dialog = $(this.form).dialog({
                autoOpen: false,
                draggable: true,
                modal: true,
                position: ["top"],
                resizable: true,
                width: 860,
                close: function () {
                    self.dispose();
                },
                open: function () {
                    $(".ui-widget-overlay").css({ 'background-image': 'url("")', 'background-color': '#000', 'opacity': '0.99' });
                }
            });
        },

        this.showViewForm = function (documentId) {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?t=' + this.templateID + '&type=getblank&jdata=' + documentId;

            $.ajax({
                type: "GET",
                cache: false,
                url: urlRequest,
                dataType: "json",
                success: function (data) {
                    thisDoc = data;
                    self.createForm(data);
                    self.dialog.dialog("open");
                }
            });
        },

        this.dispose = function () {
            if (this.form)
                this.form.remove();
            if (this.dialog)
                this.dialog.remove();
        };
    };

    window.OutputDocumentBlank = outputDocumentBlank;

})(window);