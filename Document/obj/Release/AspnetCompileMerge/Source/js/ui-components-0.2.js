(function (window, $, undefined) {
    var appSettings = window.appSettings;
    var testTime = new Date(1, 0, 1, 0, 0, 0);

    function formatDateTime(date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes;
        return date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate() + '  ' + strTime;
    }

    function cardBlock(o) {
        o = o || {};
        var card = o.card,
            head = card.Head,
            worker = card.Worker,
            fixedWorker = card.FixedWorker,
            isBaseCard = false,
            foreignCard = (userdata.departmentId !== card.ExecutiveDepartmentID);

        head = (head && card.HeadID > 0) ? documentUI.formatFullName(head.LastName, head.FirstName, head.MiddleName) : '';
        worker = (worker && card.WorkerID > 0) ? documentUI.formatFullName(worker.LastName, worker.FirstName, worker.MiddleName) : '';
        var executiveDepartment = (card.ExecutiveDepartment && card.ExecutiveDepartmentID > 0) ? card.ExecutiveDepartment.ShortName : '';
        fixedWorker = (fixedWorker && card.FixedWorkerID > 0) ? documentUI.formatFullName(fixedWorker.LastName, fixedWorker.FirstName, fixedWorker.MiddleName) : '';
        var fix = '&nbsp;&nbsp;&nbsp;&nbsp; Закріплена за: ' + fixedWorker;

        var cardStatus = card.CardStatus ? card.CardStatus.Name : '';

        var endDate = '';
        if (card.EndDate) {
            endDate = new Date(+card.EndDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
            if (endDate > testTime) {
                endDate = endDate;
            } else {
                endDate = '';
            }
        }
        var startDate = '';
        if (card.StartDate) {
            startDate = new Date(+card.StartDate.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
            if (startDate > testTime) {
                startDate = startDate;
            } else {
                startDate = '';
            }
        }
        var wrap = $('<div class="card-wrap' + (o.cssClass ? (' ' + o.cssClass) : '') + '"><span class="fw-n"> Стан: </span><span class="card-status-' + card.CardStatusID + '">' + cardStatus + '</span> </div>')
            .prepend($('<input class="card-enddate' + (card.CardStatusID != 1 ? ' card-close' : '') + '" type="text" disabled="disabled">').datepicker({ changeMonth: true, changeYear: true }).datepicker('setDate', endDate))
            .prepend('<span class="fw-n"> Термін: </span>')
            .prepend('<span>' + card.CardNumber + '. ' + executiveDepartment + ', ' + (worker ? '<span class="fs-i">' + worker + '</span>,' : '') + ' </span>');
        if (o.resolutionVisibility) {
            wrap.append('<span> &nbsp; Резолюція: </span><span class="fw-n card-resolution">' + card.Resolution + '</span>');
        }
        $('<span class="info-button" data-title="Info">i</span>').appendTo(wrap).on('click', function () {
            if (o.documentBlank && o.documentBlank.comments) {
                cardComments.empty();
                commentsReadonlyBlock({
                    appendTo: cardComments,
                    comments: o.documentBlank.comments.getComments({ controlCardId: card.ID })
                });
            }
            infoTooltip.show();
        });
        var infoTooltip = $('<div class="info-tooltip" style="display:none;"></div>').appendTo(wrap);
        $('<div class="tooltip-clouse">x</div>').appendTo(infoTooltip).on('click', function () {
            infoTooltip.hide();
        });
        $('<div>' + 'Виконавець: ' + executiveDepartment + (worker ? ', <span class="fs-i">' + worker + '</span>,' : '') + '</div>').appendTo(infoTooltip);
        $('<div>' + 'Термін: з' + '</div>').append($('<input class="card-startDate' + (card.CardStatusID != 1 ? ' card-close' : '') + '" type="text" disabled="disabled">').datepicker({ changeMonth: true, changeYear: true }).datepicker('setDate', startDate))
            .append(' по ')
            .append($('<input class="card-enddate' + (card.CardStatusID != 1 ? ' card-close' : '') + '" type="text" disabled="disabled">').datepicker({ changeMonth: true, changeYear: true }).datepicker('setDate', endDate)).appendTo(infoTooltip);
        $('<div>' + 'Стан:' + '</div>').append($('<span class="card-status-' + card.CardStatusID + '">' + cardStatus + '</span>')).appendTo(infoTooltip);
        var cardComments = $('<div class="card-comments"></div>').appendTo(infoTooltip);

        return wrap;
    }

    function commentsReadonlyBlock(o) {
        o = o || {};
        var cHeader = $('<div class="comments-header">Коментарі:</div>').appendTo(o.appendTo);
        var cBlock = $('<div class="comments-block"></div>').appendTo(o.appendTo);
        o.comments.forEach(function (c) {
            var commentItem = $('<div class="comment-item"></div>').appendTo(cBlock);
            var commentTitle = $('<span class="comment-title"></span>').appendTo(commentItem);
            var cDate = new Date(+c.CreateDate.replace(/\/Date\((-?\d+)\)\//gi, '$1'));
            $('<span class="comment-time">' + formatDateTime(cDate) + ' </span>').appendTo(commentTitle);
            var workerName = documentUI.formatFullName(c.Worker.LastName, c.Worker.FirstName, c.Worker.MiddleName);
            $('<span class="comment-worker">' + workerName + ' </span>').appendTo(commentTitle);
            $('<div class="comment-content"></div>').append(c.Content).appendTo(commentItem);
        });
    }

    function commentsBlock(o) {
        o = o || {};
        var lastComments = [];
        var cHeader = $('<div class="comments-header">Коментарі:</div>').appendTo(o.appendTo);
        var cBlock = $('<div class="comments-block"></div>').appendTo(o.appendTo);
        var cButtons = $('<div class="comments-buttons"></div>').appendTo(o.appendTo);

        var textarea = $('<textarea style="width:98%;height:54px;" cols="81" rows="3"></textarea>').appendTo(cButtons);
        $('<input type="button" value="Надіслати">').button().click(function () {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=coms&type=ins' + '&dep=' + userdata.departmentId + '&documentId=' + o.DocumentID + '&controlCardId=' + o.ControlCardID;
            var data = textarea.val();
            //var data = { comment: textarea.text() };
            //data = JSON.stringify(data);
            if (data) {
                $.ajax({
                    url: urlRequest,
                    type: "POST",
                    cache: false,
                    data: { 'data': data },
                    dataType: "json",
                    success: function(msg) {
                        textarea.val('');
                        updateComments();
                    },
                    error: function(xhr, status, error) {
                        alert(xhr.responseText);
                    }
                });
            } else {
                alert('Текст коментаря до документа пустий!');
            }
        }).appendTo(cButtons);


        function updateComments() {
            var urlRequest = appSettings.rootUrl + 'Handlers/DataPoint.ashx?obj=coms&type=list' + '&dep=' + userdata.departmentId + '&documentId=' + o.DocumentID;
            $.ajax({
                url: urlRequest,
                type: 'GET',
                cache: false,
                dataType: 'json',
                success: function (response) {
                    if (response.Data) {
                        cBlock.empty();
                        lastComments = response.Data;
                        response.Data.forEach(function (c) {
                            var commentItem = $('<div class="comment-item"></div>').appendTo(cBlock);
                            var commentTitle = $('<span class="comment-title"></span>').appendTo(commentItem);
                            var cDate = new Date(+c.CreateDate.replace(/\/Date\((-?\d+)\)\//gi, '$1'));
                            $('<span class="comment-time">' + formatDateTime(cDate) + ' </span>').appendTo(commentTitle);
                            var workerName = documentUI.formatFullName(c.Worker.LastName, c.Worker.FirstName, c.Worker.MiddleName);
                            $('<span class="comment-worker">' + workerName + ' </span>').appendTo(commentTitle);
                            $('<div class="comment-content"></div>').append(c.Content).appendTo(commentItem);
                        });
                    }
                },
                error: function (xhr, status, error) {
                }
            });
        }
        updateComments();

        function getComments(p) {
            p = p || {};
            if (p.controlCardId) {
                return lastComments.filter(function (c) {
                    return c.ControlCardID === p.controlCardId;
                });
            }
            return lastComments;
        }

        return {
            getComments: getComments
        };
    }

    window.formParts = {
        cardBlock: cardBlock,
        commentsBlock: commentsBlock
    };
})(window, jQuery);
