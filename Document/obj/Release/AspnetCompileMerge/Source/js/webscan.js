function WebToolkit(config) {
    $.extend(this, {
        //host
        host: '127.0.0.1',
        //port
        port: 57632,
        // path - should have "/"" at start and end
        path: "/Scan/json/",
        //request timeout
        timeout: 10000,
        //application name
        applicationName: "",
        //client uid
        clientUid: "",
        //interval to pool scan or export job
        poolingInterval: 2000,
        // global error handler
        errorHandler: {
            error: null,
            scope: null
        }
    }, config);

    this._serviceUrl = "http://" + this.host + ":" + this.port + this.path;
}

WebToolkit.JobStatus = {
    InProgress: 0,
    JobComplete: 1,
    Canceling: 2,
    Cancelled: 3,
    Error: 4,
    JobNotFound: 5,
    WaitUserInput: 6
};

WebToolkit.Messages = {
    NotAvailable: "SharedScanner is not available. Please check that it is installed, started and you are connecting to correct IP address."
};

WebToolkit.prototype._callServer = function (serviceMethod, data, callback) {
    var url = this._serviceUrl + serviceMethod;
    $.ajax(url, {
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "jsonp",
        jsonp: "method",
        timeout: this.timeout,
        cache: false,
        data: data,
        context: this
    }).success(function (data) {
        this._parseResponse(data, callback);
    }).error(function (jqXHR, textStatus) {
        this._parseResponse({ Status: 501, Error: { Description: WebToolkit.Messages.NotAvailable } }, callback);
    });
};

WebToolkit.prototype._parseResponse = function (data, callback) {
    callback = callback || {};

    var code = data.Status;
    if (code < WebToolkit.JobStatus.Error) {
        this._invoke(callback.success, callback.scope, [data]);
    } else {
        this._invokeFail(callback, [data]);
    }
};

WebToolkit.prototype._invoke = function (target, scope, params) {
    if (target) {
        target.apply(scope, params);
    }
};

WebToolkit.prototype._invokeFail = function (callback, params) {
    if (callback.fail) {
        this._invoke(callback.fail, callback.scope, params);
    } else {
        this._invoke(this.errorHandler.fail, this.errorHandler.scope, params);
    }
};

//public

/*
callback: {
    success: function(data)
    progress: function(data)
    fail: function(errorData)
    scope: scope to call success, fail
    }

    data: {"Status": int, ... }
    errorData: {"Status": code, "Error": { "Description" : message} }
 */


WebToolkit.prototype.getScannersList = function (callback) {
    this._callServer("service/scanners", null, callback);
};

/*
scanner: scanner name
*/
WebToolkit.prototype.getScannerCapabilities = function (scanner, callback) {
    var params = {
        scanner: scanner
    };
    this._callServer("scan/getscannercapabilities", params, callback);
};

WebToolkit.prototype.startJob = function (jobid, scanSettings, callback) {
    var params = $.extend({
        ticket: jobid || "",
        owner: this.clientUid,
        application: this.applicationName
    }, {
        'scanner': '',
        'colormode': '',
        'resolution': 0,
        'orientation': 0,
        'papersize': 0,
        'papersource': 0,
        'duplex': false,
        'showui': false,
        'deskew': false,
        'crop': false,
        'punches': false
    }, scanSettings);

    this._callServer("scan/start", params, callback);
};

WebToolkit.prototype.getJobImageCount = function (jobid, callback) {
    var params = {
        jobid: jobid
    };
    this._callServer("repository/getjobimagecount", params, callback);
};

WebToolkit.prototype.scan = function (jobid, scanSettings, callback) {
    var statusCallback = {
        success: function (data) {
            var status = data.Status;
            //add ticket to response
            data.Ticket = statusCallback.ticket;
            //check status and fire events
            if (status == WebToolkit.JobStatus.JobComplete || status == WebToolkit.JobStatus.Cancelled) {
                this._invoke(callback.success, callback.scope, [data]);
            } else {
                if (data.ImageCount > statusCallback.imagesScanned) {
                    statusCallback.imagesScanned = data.ImageCount;
                    this._invoke(callback.progress, callback.scope, [data]);
                }
                //call pooling with delay
                var self = this;
                setTimeout(function () {
                    self.getJobImageCount(data.Ticket, statusCallback);
                }, this.poolingInterval);
            }
        },
        fail: function (e) {
            this._invoke(callback.fail, callback.scope, [e]);
        },
        scope: this
    };

    this.startJob(jobid, scanSettings, {
        success: function (data) {
            statusCallback.ticket = data.Ticket;
            statusCallback.imagesScanned = 0;
            data.ImageCount = 0;
            this._invoke(callback.progress, callback.scope, [data]);
            this.getJobImageCount(data.Ticket, statusCallback);
        },
        fail: function (e) {
            this._invoke(callback.fail, callback.scope, [e])
        },
        scope: this
    });
};

WebToolkit.prototype.getServiceStatus = function (ticket, callback) {
    var params = {
        ticket: ticket
    };
    this._callServer("service/getstatus", params, callback);
};

WebToolkit.prototype.upgrade = function (ticket, callback) {
    var params = {
        owner: this.clientUid,
        app: this.applicationName
    };

    if (ticket) {
        params.Ticket = ticket;
    }

    this._callServer("service/upgrade", params, callback);
};

WebToolkit.prototype.removeJob = function (jobid, callback) {
    var params = {
        jobid: jobid
    };
    this._callServer("repository/removejob", params, callback);
};

WebToolkit.prototype.removeJobItem = function (jobid, imageId, callback) {
    var params = {
        jobid: jobid,
        jobitem: imageId
    };
    this._callServer("repository/removejobitem", params, callback);
};

WebToolkit.prototype.getImageInfo = function (jobid, imagenumber, thumb, callback) {
    var params = {
        jobid: jobid,
        imagenumber: imagenumber,
        thumb: thumb
    };
    this._callServer("repository/getjobimageinfo", params, callback);
};

WebToolkit.prototype.rotateImage = function (jobid, imageid, rotate, callback) {
    var params = {
        jobid: jobid,
        imageid: imageid,
        rotate: rotate
    };
    this._callServer("repository/rotateimage", params, callback);
};

WebToolkit.prototype.moveImage = function (jobid, imageid, pos, callback) {
    var params = {
        jobid: jobid,
        imageid: imageid,
        pos: pos
    };
    this._callServer("repository/moveimage", params, callback);
};

WebToolkit.prototype.getImageUrl = function (jobid, imageid, thumb, lowres, rotate) {
    return this._serviceUrl + "repository/getjobimage" +
            "?jobid=" + jobid +
            "&imageid=" + imageid +
            "&thumb=" + thumb +
            "&lowres=" + lowres +
            "&rotate=" + rotate;
};

WebToolkit.prototype.getImageTilesUrl = function (jobid, imageid, rotate) {
    return this._serviceUrl + "repository/getjobimagetile" +
            "?jobid=" + jobid +
            "&imageid=" + imageid +
            "&rotate=" + rotate;
};

WebToolkit.prototype.startExport = function (jobid, exportSettings, callback) {
    var params = $.extend({
        scanticket: jobid || "",
        owner: this.clientUid,
        application: this.applicationName
    }, exportSettings);
    this._callServer("export/export", params, callback);
};

WebToolkit.prototype.getExportStatus = function (exportJobId, callback) {
    var params = {
        ticket: exportJobId
    };
    this._callServer("export/getexportresult", params, callback);
};

WebToolkit.prototype.exportJob = function (jobid, exportSettings, callback) {
    var statusCallback = {
        success: function (data) {
            var status = data.ExportResult.Status;
            //add ticket to response
            data.Ticket = statusCallback.ticket;
            //check status and fire events
            if (status == WebToolkit.JobStatus.JobComplete || status == WebToolkit.JobStatus.Cancelled) {
                this._invoke(callback.success, callback.scope, [data]);
            } else if (status == WebToolkit.JobStatus.Error) {
                var e = { Status: status, Error: { Description: data.ExportResult.Text } };
                this._invoke(callback.fail, callback.scope, [e]);
            } else {
                this._invoke(callback.progress, callback.scope, [data]);
                //call pooling with delay
                var self = this;
                setTimeout(function () {
                    self.getExportStatus(data.Ticket, statusCallback);
                }, this.poolingInterval);
            }
        },
        fail: function (e) {
            this._invoke(callback.fail, callback.scope, [e]);
        },
        scope: this
    };

    this.startExport(jobid, exportSettings, {
        success: function (data) {
            statusCallback.ticket = data.Ticket;
            this.getExportStatus(data.Ticket, statusCallback);
        },
        fail: function (e) {
            this._invoke(callback.fail, callback.scope, [e]);
        },
        scope: this
    })
};

WebToolkit.prototype.getExportResultFileUrl = function (exportJobId, filetype) {
    return this._serviceUrl + "export/getexportresultfile?ticket=" + exportJobId + "&filetype=" + filetype;
};

WebToolkit.prototype.getPrinters = function (callback) {
    this._callServer("service/printers", null, callback);
};

WebToolkit.prototype.getKey = function (callback) {
    this._callServer("service/key", null, callback);
};

WebToolkit.prototype.register = function (apptype, email, serial, callback) {
    var params = {
        owner: this.clientUid,
        apptype: apptype,
        email: email,
        serial: serial
    };

    this._callServer("service/register", params, callback);
};

WebToolkit.prototype.getLicense = function (apptype, callback) {
    var params = {
        owner: this.clientUid,
        apptype: apptype
    };

    this._callServer("service/license", params, callback);
};

WebToolkit.prototype.getVersion = function (callback) {
    this._callServer("service/version", null, callback);
};

WebToolkit.prototype.checkUpdate = function (callback) {
    var baseUrl = document.URL.substr(0, document.URL.lastIndexOf("/") + 1);

    this.getVersion({
        success: function (data) {
            $.ajax(baseUrl + 'autoupdate/autoupdate.xml', {
                type: "GET",
                dataType: "xml",
                timeout: this.timeout,
                cache: false,
                context: this
            }).success(function (updateData) {
                var newVersion, description;
                try {
                    var node = $(updateData.childNodes[0]);
                    newVersion = node.attr('location').replace('.zip', '');
                    description = node.text();
                } catch (e) { }


                if (!newVersion || this.compareVersions(newVersion, data.ProductVersion) <= 0) {
                    this._invoke(callback.success, callback.scope, [{ UpdateAvailable: false, CurrentVersion: data.ProductVersion, UI: data.InterfaceVersion }]);
                    return;
                }

                this._invoke(callback.success, callback.scope, [{
                    UpdateAvailable: true,
                    CurrentVersion: data.ProductVersion,
                    NewVersion: newVersion,
                    UI: data.InterfaceVersion,
                    Description: description
                }]);
            }).error(function (jqXHR, textStatus) {
                this._parseResponse({ Status: 501, Error: { Description: WebToolkit.Messages.NotAvailable } }, callback);
            });
        },
        fail: function (e) {
            this._invoke(callback.fail, callback.scope, [e]);
        },
        scope: this
    });
};

WebToolkit.prototype.removeExportJob = function (ticket, callback) {
    var params = {
        ticket: ticket
    };
    this._callServer("export/removeexportjob", params, callback);
};

WebToolkit.prototype.sendUserInput = function (ticket, value, text, callback) {
    var params = {
        ticket: ticket,
        value: value,
        text: text
    };
    this._callServer("export/senduserinput", params, callback);
};


WebToolkit.prototype.compareVersions = function (a, b) {
    if (a === b) {
        return 0;
    }

    var a_components = a.split(".");
    var b_components = b.split(".");

    var len = Math.min(a_components.length, b_components.length);

    for (var i = 0; i < len; i++) {
        if (parseInt(a_components[i]) > parseInt(b_components[i])) {
            return 1;
        }
        if (parseInt(a_components[i]) < parseInt(b_components[i])) {
            return -1;
        }
    }

    if (a_components.length > b_components.length) {
        return 1;
    }

    if (a_components.length < b_components.length) {
        return -1;
    }

    return 0;
};







function Session(config) {
    this._storage = localStorage || new InMemoryStorage();
}

Session.prototype.getClientUID = function () {
    var clientUID = this._storage.getItem('ClientUID');
    if (!clientUID) {
        var chars = '0123456789abcdef'.split('');

        var uuid = [], rnd = Math.random, r;
        uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
        uuid[14] = '4'; // version 4

        for (var i = 0; i < 36; i++) {
            if (!uuid[i]) {
                r = 0 | rnd() * 16;
                uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r & 0xf];
            }
        }

        clientUID = uuid.join('');
        this._saveItem('ClientUID', clientUID);
    }

    return clientUID;
},

Session.prototype.getServerUrl = function () {
    return this._storage.getItem("server") || '127.0.0.1';
}

Session.prototype.setServerUrl = function (value) {
    this._saveItem("server", value);
}

Session.prototype.getJobId = function () {
    return this._storage.getItem("JobId");
}

Session.prototype.setJobId = function (value) {
    this._saveItem("JobId", value);
}

Session.prototype.getSelectedScanner = function () {
    return this._storage.getItem("SelectedScanner");
}

Session.prototype.setSelectedScanner = function (value) {
    this._saveItem("SelectedScanner", value);
}

Session.prototype.reset = function () {
    this.setJobId(null);
    this._tempData = {};
    //this.setJobSettings(null);
    //this.imageCache.flush();
}

Session.prototype.getScanParams = function () {
    return this._getObject('ScanParams') || {};
}

Session.prototype.setScanParams = function (value) {
    this._saveItem("ScanParams", value);
}

Session.prototype.getLicenseKey = function () {
    return this._licenseKey || "";
}

Session.prototype.setLicenseKey = function (value) {
    this._licenseKey = value;
}

Session.prototype.getLicenseValid = function () {
    return this._licenseValid || false;
}

Session.prototype.setLicenseValid = function (value) {
    this._licenseValid = value;
}

Session.prototype.setLastScanMode = function (value) {
    this._lastScanMode = value;
}

Session.prototype.getLastScanMode = function () {
    return this._lastScanMode || "color";
}

Session.prototype.setLastExportMode = function (value) {
    this._lastExportMode = value;
}

Session.prototype.getLastExportMode = function () {
    return this._lastExportMode;
}

Session.prototype.getExporterSettings = function (name) {
    return this._getObject('exporter-' + name) || {};
}

Session.prototype.getTempData = function () {
    if (!this._tempData) {
        this._tempData = {};
    }

    return this._tempData;
}

Session.prototype.setExporterSettings = function (name, value) {
    this._saveItem('exporter-' + name, value);
}

Session.prototype._getObject = function (key) {
    try {
        var storedValue = eval('(' + this._storage.getItem(key) + ')');
        return storedValue;
    } catch (ex) {
        return null;
    }
}

Session.prototype._saveItem = function (key, value) {
    if (value) {
        if (typeof value == 'object') {
            value = JSON.stringify(value);
        }

        this._storage.setItem(key, value);
    } else {
        this._storage.removeItem(key);
    }
}


function InMemoryStorage() {
    this._data = {};
}

InMemoryStorage.prototype.getItem = function (key) {
    var value = this._data[key];
    if (!value) {
        return null;
    }
    return value;
}

InMemoryStorage.prototype.setItem = function (key, value) {
    this._data[key] = value;
}

InMemoryStorage.prototype.removeItem = function (key) {
    delete this._data[key];
}










var _toolkit;
var _session = new Session();
var _statistics = new Statistics(_session.getClientUID());
var _viewer;

function connectToSharedScanner(host) {
    _toolkit = new WebToolkit({
        host: host,
        applicationName: "cloudscan html",
        clientUid: _session.getClientUID()
    });

    showWaiting("Connecting to Scanner. Please wait...");
    _toolkit.getScannersList({
        success: function (data) {
            hideWaiting();
            var options = $('#ribbon-home-listScanners');
            $.each(data.Scanners, function () {
                options.append($('<option />').val(this).text(this));
            });
            setScanDisabled(data.Scanners.length == 0);

            var selectedScanner = _session.getSelectedScanner();
            if (selectedScanner) {
                options.find('option[value="' + selectedScanner + '"]').attr("selected", true);
            }

            ////////////////////Redirect to Silverlight CloudScan if old version of SharedScanner used////////////
            _toolkit.getVersion(
                {
                    success: function (data) {
                        if (_toolkit.compareVersions("3.3.20.0", data.ProductVersion) > 0) {
                            window.open("http://www.scanworkssoftware.com/products/cloudscan/cloudscan.html", "_self");
                            return;
                        }
                    },
                    fail: function (data) {
                        return;
                    },
                    scope: this
                }
                );
            ///////////////////////////////////////////////////

            _toolkit.checkUpdate({
                success: function (data) {
                    hideWaiting();
                    if (data.UpdateAvailable) {
                        _session.getTempData().NewVersion = data.NewVersion;
                        showUpdateDialog(data.CurrentVersion, data.NewVersion, data.Description);
                    } else {
                        startNewSession();
                    }

                    _statistics.logSuccessfulConnect(data.UI);
                },
                fail: function (e) {
                    startNewSession();
                }
            });
        },
        fail: function (e) {
            hideWaiting();
            _statistics.logErrorConnect("Failed connect to " + host);
            _statistics.logErrorAvailableScanners(e.Error.Description);

            $('#connect-dialog').dialog('open');
        }
    });
}

function startNewSession() {
    loadScannerCapabilities();
    restoreScanJob();

    _toolkit.getKey({
        success: function (data) {
            _session.setLicenseKey(data.Key);
            $('#ribbon-premium-textKey').val(data.Key);
        }
    });

    var email = getURLParameter("email");
    var serial = getURLParameter("serial");

    if (!(email && serial)) {
        _toolkit.getLicense("CloudScanPremium", {
            success: function (data) {
                _session.setLicenseValid(data.ValidLicense);
                if (data.ValidLicense) {
                    $("#ribbon-premium-textEmail").val(data.eMail)
                    $("#ribbon-premium-textSerial").val(data.Serial);
                    $('#advert').advertisement({ baseUrl: getBaseUrl(), premiumUser: true });
                } else {
                    $("#ribbon-premium-textEmail").val("")
                    $("#ribbon-premium-textSerial").val("");
                    $('#advert').advertisement({ baseUrl: getBaseUrl(), premiumUser: false });
                }
            }
        });
    } else {
        $("#ribbon-premium-textEmail").val(email)
        $("#ribbon-premium-textSerial").val(serial);
        register();
    }
}

function onUpdateYes() {
    showWaiting("SharedScanner is downloading the new version...");
    _toolkit.upgrade(null, {
        success: function (data) {
            monitorUpdateStatus(data.Ticket);
        },
        fail: function (e) {
            _statistics.logErrorUpgrade(e.Error.Description);
            showError(e, startNewSession);
        }
    })
}

function monitorUpdateStatus(ticket) {
    showWaiting("SharedScanner is downloading the new version...");
    var update_status_callback = {
        success: function (data) {
            setTimeout(function () {
                _toolkit.getServiceStatus(ticket, update_status_callback);
            }, 3000);
        },
        fail: function (e) {
            hideWaiting();
            if (e.Status == WebToolkit.JobStatus.WaitUserInput) {
                $('#update-completed-dialog').dialog('open');
                _session.getTempData().UpdateTicket = ticket;
            } else {
                _statistics.logErrorUpgrade(e.Error.Description);
                $('#update-help-dialog').dialog('open');
            }
        }
    };

    _toolkit.getServiceStatus(ticket, update_status_callback);
}

function onUpdateRestart() {
    var startStamp = new Date().getTime();
    showWaiting("SharedScanner is restarting...");

    var update_status_callback = {
        success: function (data) {
            hideWaiting();
            if (data.ProductVersion == _session.getTempData().NewVersion) {
                _statistics.logSuccessfulUpgrade(data.ProductVersion);
                startNewSession();
            } else {
                _statistics.logErrorUpgrade("Version didn't changed after restart: " + data.ProductVersion);
                $('#update-help-dialog').dialog('open');
            }
        },
        fail: function (e) {
            if (e.Status == 501) {
                var nowStamp = new Date().getTime();
                //wait one minute for SS restart
                if (nowStamp - startStamp > 60 * 1000) {
                    e.Error.Description = "SharedScanner cannot be automatically restarted for some reasons. Please run it from Start menu.";
                    _statistics.logErrorUpgrade(e.Error.Description);
                    showError(e, startNewSession);
                } else {
                    _toolkit.getVersion(update_status_callback);
                }
            } else {
                _statistics.logErrorUpgrade(e.Error.Description);
                showError(e, startNewSession);
            }
        }
    };

    _toolkit.upgrade(_session.getTempData().UpdateTicket, {
        success: function () {
            setTimeout(function () {
                _toolkit.getVersion(update_status_callback);
            }, 5000);
        },
        fail: function (e) {
            if (e.Status == 501) {
                setTimeout(function () {
                    _toolkit.getVersion(update_status_callback);
                }, 1000);
            } else {
                showError(e, startNewSession);
            }
        }
    })
}



function onUpdateNo() {
    startNewSession();
}

function loadScannerCapabilities() {
    var selectedScanner = $('#ribbon-home-listScanners').val();
    if (!selectedScanner) return;

    _session.setSelectedScanner(selectedScanner);

    _statistics.logCallScanParams(selectedScanner);
    _toolkit.getScannerCapabilities(selectedScanner, {
        success: function (data) {
            hideWaiting();

            var lastSettings = _session.getScanParams();

            var options = $('#ribbon-sp-listPapperSize');
            options.find('option').remove();
            $.each(data.SupportedScanParameters.PaperSizes, function () {
                options.append($('<option />').val(this.Value).text(this.Description));
            });
            //select best paper size by priority: US Letter, A4, first
            if (lastSettings.papersize) {
                options.find('option[value="' + lastSettings.papersize + '"]').attr("selected", true);
            } else if ($.inArray(3, $.map(data.SupportedScanParameters.PaperSizes, function (item) { return item.Value })) > -1) {
                options.find('option[value="3"]').attr("selected", true);
            } else if ($.inArray(1, $.map(data.SupportedScanParameters.PaperSizes, function (item) { return item.Value })) > -1) {
                options.find('option[value="1"]').attr("selected", true);
            }

            options = $('#ribbon-sp-listResolution');
            options.find('option').remove();
            $.each(data.SupportedScanParameters.ResolutionsNormalized, function () {
                options.append($('<option />').val(this).text(this));
            });
            //select best resoultion by priority: last used, 200, 300, first
            if (lastSettings.resolution) {
                options.find('option[value="' + lastSettings.resolution + '"]').attr("selected", true);
            } else if ($.inArray('200', data.SupportedScanParameters.ResolutionsNormalized) > -1) {
                options.find('option[value="200"]').attr("selected", true);
            } else if ($.inArray('300', data.SupportedScanParameters.ResolutionsNormalized) > -1) {
                options.find('option[value="300"]').attr("selected", true);
            }

            options = $('#ribbon-sp-listSource');
            options.find('option').remove();
            $.each(data.SupportedScanParameters.PaperSources, function () {
                options.append($('<option />').val(this.Value).text(this.Description));
            });
            options.find('option[value="' + lastSettings.papersource + '"]').attr("selected", true);

            options = $('#ribbon-sp-listOrientation');
            options.find('option').remove();
            $.each(data.SupportedScanParameters.Orientations, function () {
                options.append($('<option />').val(this.Value).text(this.Description));
            });
            options.find('option[value="' + lastSettings.orientation + '"]').attr("selected", true);

            if (data.SupportedScanParameters.DuplexSupport) {
                $('#ribbon-sp-buttonDuplex').attr('checked', lastSettings.duplex).parent().show();
            } else {
                $('#ribbon-sp-buttonDuplex').attr('checked', false).parent().hide();
            }
        },
        fail: function (e) {
            $('#ribbon-sp-listPapperSize').find('option').remove();
            $('#ribbon-sp-listResolution').find('option').remove();
            $('#ribbon-sp-listSource').find('option').remove();
            $('#ribbon-sp-listOrientation').find('option').remove();
            $('#ribbon-sp-buttonDuplex').parent().hide();
            _statistics.logErrorScanParams(e.Error.Description);
            showError(e);
        }
    });
}

function startScanJob(colorMode) {
    showWaiting('Scanning. Please wait...');

    var settings = {
        scanner: $('#ribbon-home-listScanners').val(),
        colormode: colorMode,
        resolution: $('#ribbon-sp-listResolution').val(),
        orientation: $('#ribbon-sp-listOrientation').val(),
        papersize: $('#ribbon-sp-listPapperSize').val(),
        papersource: $('#ribbon-sp-listSource').val(),
        duplex: $('#ribbon-sp-buttonDuplex').is(':checked'),
        showui: $('#ribbon-home-buttonShowScannerUI').hasClass('ui-button-checked'),
        deskew: $('#ribbon-ie-buttonAutoDeskew').hasClass('ui-button-checked'),
        crop: $('#ribbon-ie-buttonBlackBorders').hasClass('ui-button-checked'),
        punches: $('#ribbon-ie-buttonPunches').hasClass('ui-button-checked')
    };

    _session.setScanParams({
        resolution: settings.resolution,
        orientation: settings.orientation,
        papersize: settings.papersize,
        papersource: settings.papersource,
        duplex: settings.duplex,
        deskew: settings.deskew,
        crop: settings.crop,
        punches: settings.punches
    });

    if (settings.deskew || settings.crop || settings.punches) {
        var msg = [settings.deskew ? "AutoDeskew" : "", settings.crop ? "AutoCrop" : "", settings.punches ? "PunchRemoval" : ""].join(" ");
        _statistics.logOtherMessage(104, msg);
    }

    var scanImpl = function () {
        _toolkit.scan(_session.getJobId(), settings, {
            success: function (data) {
                hideWaiting();
                _statistics.logSuccessfulScan();
                _session.setJobId(data.Ticket);
                updatePreviewsLayout(data, true);
                $("#scancompleted-dialog").dialog("open");
            },
            progress: function (data) {
                _session.setJobId(data.Ticket);
                updatePreviewsLayout(data);
            },
            fail: function (e) {
                _statistics.logErrorScan(e.Error.Description);
                showError(e);
            }
        });
    }

    if (_session.getLicenseValid() || !(settings.deskew || settings.crop || settings.punches)) {
        scanImpl();
    } else {
        showPremiumDialog('Image enhancement during scanning', scanImpl);
    }
}

function restoreScanJob() {
    if (!_session.getJobId()) {
        return;
    }
    _toolkit.getJobImageCount(_session.getJobId(), {
        success: function (data) {
            data.Ticket = _session.getJobId();
            updatePreviewsLayout(data);
        },
        fail: function (e) {
            if (e.Error.Description.indexOf('not found') != -1) {
                _session.reset();
            } else {
                showError(e);
            }
        }
    });
}

function updatePreviewsLayout(data, selectLastItem) {
    var existingItems = $('#preview-list').find('.preview-item');

    for (var i = existingItems.length; i < data.ImageCount; i++) {
        var needSelect = selectLastItem && (i == data.ImageCount - 1);

        $('#preview-list').append(
			$('<div/>')
				.addClass('preview-item')
				.append($('<img/>').attr('src', 'css/images/loading.gif'))
				.append($('<span/>').text('Page ' + (i + 1)))
		);

        var addedItem = $('#preview-list .preview-item:last').get(0);

        //load image info and create preview
        var jobId = data.Ticket;
        _toolkit.getImageInfo(data.Ticket, i, false, {
            success: function (data) {
                jQuery.data(this, "ImageInfo", data);
                $(this).attr('id', data.ImageId);
                if (needSelect) {
                    onPreviewSelected($(this));
                }
                updatePreviewImage(data);
            },
            fail: function (e) {
            },
            scope: addedItem
        });
    }

    setExportDisabled(data.ImageCount == 0);
}

function updatePreviewNumberes() {
    $('#preview-list div').each(function (index, elem) {
        $(elem).find('span').text('Page ' + (index + 1));
    });
}

function onScanItemRotate(item, step) {
    var elem = $('#' + item).get(0);
    var data = jQuery.data(elem, "ImageInfo");
    var rotate = (4 + data.Rotation + step) % 4;

    data = {
        ImageId: data.ImageId,
        ImageHeight: data.ImageWidth,
        ImageWidth: data.ImageHeight,
        Rotation: rotate
    };

    _toolkit.rotateImage(_session.getJobId(), item, rotate, {
        success: function () {
            updatePreviewImage(data);

            jQuery.data(elem, "ImageInfo", data);
            if (isItemSelected(item)) {
                _viewer.close();
                onPreviewSelected(elem);
            }
        },
        error: function (e) {
            showError(e);
        }
    });
}

function onEditScanItem(item) {
    _statistics.logStartExporter(-106);

    var url = ["http://pixlr.com/editor/?s=c&credentials=true&method=post&image=",
				escape(_toolkit.getImageUrl(_session.getJobId(), item, false, false)),
				"&title=Scanned image"].join("");
    window.open(url);
}

function onRemoveScanItem(item) {
    _toolkit.removeJobItem(_session.getJobId(), item, {
        success: function () {
            if (isItemSelected(item)) {
                _viewer.close();
            }

            $('#' + item).remove();
            setExportDisabled($('#preview-list .preview-item').length == 0);
            updatePreviewNumberes();
        },
        fail: function (e) {
            showError(e);
        }
    });
}

function onExportPdf(pdfa) {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(pdfa == true ? -108 : -101);
    showWaiting('Saving. Please wait...');

    var settings = {
        exporter: "MultiPageExporter",
        filetype: pdfa ? "Pdfa" : "Pdf"
    };

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            var url = _toolkit.getExportResultFileUrl(data.Ticket, "pdf");
            showExportCompleted('Message', 'PDF document has been successfully created. To save it on local disk click right mouse button on link below and select \"Save target as...\" or \"Save link as...\".', url);
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function onExportTiff() {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(-102);
    showWaiting('Saving. Please wait...');

    var settings = {
        exporter: "MultiPageExporter",
        filetype: "Tif"
    };

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            var url = _toolkit.getExportResultFileUrl(data.Ticket, "tif");
            showExportCompleted('Message', 'Tiff document has been successfully created. To save it on local disk click right mouse button on link below and select \"Save target as...\" or \"Save link as...\".', url);
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function onExportPrinter(printer) {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(-105);
    showWaiting('Printing document. Please wait...');

    var settings = {
        exporter: "PrinterExporter",
        recipient: printer
    };

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            showExportCompleted('Message', 'Document has been successfully printed.');
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function onExportGoogleDocs(documentName, email, password) {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(-100);
    showWaiting('Exporting to Google Docs. Please wait...');

    var settings = {
        exporter: "GoogleDocsExporter",
        filetype: "Pdf",
        document: documentName,
        login: email,
        pass: "swpass" + password
    };

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            var url = data.ExportResult.Text;
            showExportCompleted('Message', 'Document has been successfully saved to Google Docs. Link:', url);
        },
        progress: function (data) {
            var status = data.ExportResult.Text;
            if (status) {
                showWaiting(status);
            }
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function onExportSharePoint(server, library, filename, user, password) {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(-107);
    showWaiting('Exporting to SharePoint. Please wait...');

    var settings = {
        exporter: "SPExporter",
        filetype: "Pdf",
        login: filename,
        pass: "swpass" + password,
        tags: ["SHarePointUrl=" + server, "DocumentLibrary=" + library, "TargetFileName=" + filename].join(',')
    };

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            var url = data.ExportResult.Text;
            showExportCompleted('Message', 'Document has been successfully saved to SharePoint. Link:', url);
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function onExportFax(name, password, phone) {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(-103);
    showWaiting('Sending by fax. Please wait...');

    var settings = {
        exporter: "PamFaxExporter",
        login: name,
        pass: "swpass" + password,
        recipient: phone
    };

    var faxConfirmShown = false;
    var canceled = false;

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            if (!canceled) {
                showExportCompleted('Message', 'Fax has been successfully sent.');
            }
        },
        progress: function (data) {
            if (faxConfirmShown || data.ExportResult.Status != WebToolkit.JobStatus.WaitUserInput) return;
            faxConfirmShown = true;
            hideWaiting();

            $('#export-fax-confirm-dialog').dialog('open');
            $('#export-fax-confirm-dialog p').html(data.ExportResult.Text.replace(/\n/g, "<br/>"));

            $("#export-fax-confirm-dialog").dialog({
                buttons: {
                    Yes: function () {
                        showWaiting('Sending by fax. Please wait...');
                        _toolkit.sendUserInput(data.Ticket, "1", "Yes", {
                            fail: function () { faxConfirmShown = false; }
                        });
                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        canceled = true;
                        showWaiting('Stop sending. Please wait...');
                        _toolkit.sendUserInput(data.Ticket, "0", "No", {
                            fail: function () { faxConfirmShown = false; }
                        });
                        $(this).dialog("close");
                    }
                }
            });
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function onExportEmail(recipients, subject, body) {
    if (!_session.getJobId()) {
        return;
    }

    _statistics.logStartExporter(-104);
    showWaiting('Sending an e-mail. Please wait...');

    var settings = {
        exporter: "EmailExporter",
        tags: ["to=" + recipients, "subject=" + subject, "body=" + body].join(',')
    };

    _toolkit.exportJob(_session.getJobId(), settings, {
        success: function (data) {
            _statistics.logSuccessfulExporter();
            hideWaiting();
            showExportCompleted('Message', 'Document has been successfully sent by e-mail.');
        },
        fail: function (e) {
            _statistics.logErrorExporter(e.Error.Description);
            showError(e);
        }
    });
}

function deleteJob() {
    if (!_session.getJobId()) {
        return;
    }

    _toolkit.removeJob(_session.getJobId(), {
        success: function () {
            $('#preview-list').find('.preview-item').remove();
            _session.reset();
            _viewer.close();
            setExportDisabled(true);
        },
        fail: function (e) {
            showError(e);
            $('#preview-list').find('.preview-item').remove();
            _session.reset();
            _viewer.close();
            setExportDisabled(true);
        }
    })
}

function register() {
    var email = $("#ribbon-premium-textEmail").val();
    var serial = $("#ribbon-premium-textSerial").val();

    _toolkit.register("CloudScanPremium", email, serial, {
        success: function (data) {
            if (data.Registered) {
                _session.setLicenseValid(true);
                $('#advert').advertisement({ baseUrl: getBaseUrl(), premiumUser: true });
            } else {
                $("#ribbon-premium-textEmail").val("")
                $("#ribbon-premium-textSerial").val("");
                $('#advert').advertisement({ baseUrl: getBaseUrl(), premiumUser: false });
            }
        },
        fail: function (e) {
            $('#advert').advertisement({ baseUrl: getBaseUrl(), premiumUser: false });
            showError(e);
        }
    });
}

function onPreviewSelected(item) {
    $('#preview-list .preview-item').removeClass('selected');
    $(item).addClass('selected');

    var data = jQuery.data(item, "ImageInfo");
    if (!data) return;

    var config = {
        tilesUrl: _toolkit.getImageTilesUrl(_session.getJobId(), data.ImageId, data.Rotation),
        width: data.ImageWidth,
        height: data.ImageHeight,
        tileSize: 256,
        tileOverlap: 1,
        tileFormat: "jpg"
    };

    _viewer.openDzi(config);
}

function isItemSelected(item) {
    var selectedItem = $('#preview-list .preview-item.selected');
    if (selectedItem.length > 0) {
        return $(selectedItem[0]).attr('id') == item;
    }
    return false;
}

function updatePreviewImage(data) {
    var img = $('#' + data.ImageId + ' img').get(0);
    $(img).attr('src', _toolkit.getImageUrl(_session.getJobId(), data.ImageId, true, false, data.Rotation));
    if (data.ImageHeight < data.ImageWidth) {
        $(img).width(200);
        $(img).height(data.ImageHeight / data.ImageWidth * 200);
    } else {
        $(img).width(data.ImageWidth / data.ImageHeight * 133);
        $(img).height(133);
    }
}

function setScanDisabled(disabled) {
    $('#ribbon-home-buttonScan,#ribbon-sp-buttonScanColor,#ribbon-sp-buttonScanGray,#ribbon-sp-buttonScanBlackWhite').button({ disabled: disabled });
}

function setExportDisabled(disabled) {
    $('#ribbon-home-buttonSave,#ribbon-home-buttonDelete,#ribbon-home-buttonFax,#ribbon-home-buttonEmail,#ribbon-home-buttonPrint').button({ disabled: disabled });
}








$().ready(function () {

    //disable context menu for document with chrome workaround
    $(document).on('mousedown', function (e) {
        window.disableContextMenu = $(e.target).hasClass('preview-item') || $(e.target).parent().hasClass('preview-item');
    });

    $(document).on('contextmenu', function (e) {
        return !window.disableContextMenu;
    });

    Seadragon.Config.imagePath = "css/seadragon/";
    _viewer = new Seadragon.Viewer("image-view");

    $('#ribbon-home-buttonScan,#ribbon-home-buttonSave').button();
    $('#ribbon-sp-buttonScanColor,#ribbon-sp-buttonScanGray,#ribbon-sp-buttonScanBlackWhite').button();
    $('#ribbon-home-buttonDelete,#ribbon-home-buttonFax,#ribbon-home-buttonEmail,#ribbon-home-buttonPrint,#ribbon-home-buttonHelp').button({});
    $('#ribbon-home-buttonShowScannerUI').button();
    $('#ribbon-home-buttonSettings').button({});

    $('#ribbon-ie-buttonAutoDeskew,#ribbon-ie-buttonBlackBorders,#ribbon-ie-buttonPunches').button();

    $('#ribbon-premium-buttonRegister,#ribbon-premium-button3Month,#ribbon-premium-button6Month,#ribbon-premium-button12Month').button({});


    setScanDisabled(true);
    setExportDisabled(true);

    //fix issues with focus
    $('button.ui-button').bind('mouseleave.button', function () {
        $(this).blur();
    });

    onColorModeChanged(_session.getLastScanMode());

    $('#ribbon-msoffice').ribbon({
        collapsible: false
    });
    $('span.ui-button-text').addClass('ui-button-door-left');


    $('#ribbon-home-menuScan, #ribbon-home-menuSave, #scanItem-contextMenu').menu();

    $('html').bind('mousedown', function () {
        $('#ribbon-home-menuScan').hide();
        $('#ribbon-home-menuSave').hide();
        $('#scanItem-contextMenu').hide();
    });

    var setViewportSize = function () {
        var height = $(window).height() - 170;
        $('#preview-list').height(height);
        $('#image-view').height(height).width($(window).width() - 278);
    }

    $(window).bind('resize', setViewportSize);

    $('#wait-dialog').dialog({ autoOpen: false, modal: true, width: "auto", height: 70, resizable: false, dialogClass: 'ui-dialog-no-title' });
    $('#wait-dialog-bar').progressbar({ value: 100 });
    $('#connect-dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 250,
        width: 530,
        resizable: false,
        dialogClass: 'ui-dialog-no-close',
        closeOnEscape: false,
        open: function () {
            setDialogState(this.id, ['server'], { server: _session.getServerUrl() });
        }
    });

    $('#exportcompleted-dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 'auto',
        width: 350,
        resizable: false,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#buypremium-dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 'auto',
        width: 350,
        resizable: false,
        dialogClass: 'ui-dialog-helpbutton ui-dialog-no-close'
    });

    $('#update-dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 'auto',
        width: 350,
        resizable: false,
        dialogClass: 'ui-dialog-no-close',
        buttons: {
            "Upgrade": function () {
                $(this).dialog("close");
                onUpdateYes();
            },
            "Skip upgrade": function () {
                $(this).dialog("close");
                onUpdateNo();
            }
        }
    });

    $('#update-help-dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 'auto',
        width: 350,
        resizable: false,
        dialogClass: 'ui-dialog-no-close',
        buttons: {
            "Help": function () {
                window.open("cloudscanhelp/troubleshooting.htm");
            },
            "Close": function () {
                $(this).dialog("close");
                startNewSession();
            }
        }
    });

    $('#update-completed-dialog').dialog({
        autoOpen: false,
        modal: true,
        height: 'auto',
        width: 350,
        resizable: false,
        dialogClass: 'ui-dialog-no-close',
        buttons: {
            "Restart": function () {
                $(this).dialog("close");
                onUpdateRestart();
            },
            "Cancel": function () {
                $(this).dialog("close");
                startNewSession();
            }
        }
    });


    $('#ribbon-home-listScanners').bind('change', function () {
        loadScannerCapabilities();
    });

    $('#connect-dialog button[name="install"]').bind('click.button', function () {
        window.open('http://www.scanworkssoftware.com/ssinstall.aspx');
    });

    $('#connect-dialog button[name="connect"]').bind('click.button', function () {
        var state = getDialogState('connect-dialog', ['server']);
        _session.setServerUrl(state.server);
        $('#connect-dialog').dialog('close');
        connectToSharedScanner(_session.getServerUrl());
    });

    $('#connect-dialog button[name="ask"]').bind('click.button', function () {
        window.open('http://www.scanworkssoftware.com/contacts.aspx');
    });

    $('#connect-dialog button[name="help"]').bind('click.button', function () {
        window.open('cloudscanhelp/ssnotavailable.htm');
    });

    $('#ribbon-home-buttonScan').bind('click.button', function () {
        if (arguments[0].pageY - $(this).offset().top < 53) {
            startScanJob(_session.getLastScanMode());
        } else {
            var menuX = $(this).offset().left;
            var menuY = $(this).offset().top + $(this).height();

            $('#ribbon-home-menuScan').css({
                left: menuX + "px",
                top: menuY + "px"
            });

            setTimeout(function () {
                $('#ribbon-home-menuScan').show();
            }, 1);
        }
    });

    $('#ribbon-home-buttonScanColor, #ribbon-sp-buttonScanColor').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuScan').hide();
        _session.setLastScanMode('color');
        onColorModeChanged('color');
        startScanJob('color');
    });

    $('#ribbon-home-buttonScanGray, #ribbon-sp-buttonScanGray').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuScan').hide();
        _session.setLastScanMode('8bitsgray');
        onColorModeChanged('8bitsgray');
        startScanJob('8bitsgray');
    });

    $('#ribbon-home-buttonScanBlackWhite, #ribbon-sp-buttonScanBlackWhite').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuScan').hide();
        _session.setLastScanMode('binary');
        onColorModeChanged('binary');
        startScanJob('binary');
    });

    $('#ribbon-home-buttonSavePdf').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuSave').hide();
        _session.setLastExportMode($(this).attr('id'));
        onExportPdf(false);
    });

    $('#ribbon-home-buttonSavePdfA').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuSave').hide();
        _session.setLastExportMode($(this).attr('id'));
        if (_session.getLicenseValid()) {
            onExportPdf(true);
        } else {
            showPremiumDialog('Saving to PDF/A', function () {
                onExportPdf(true);
            });
        }
    });

    $('#ribbon-home-buttonSaveTiff').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuSave').hide();
        _session.setLastExportMode($(this).attr('id'));
        onExportTiff();
    });

    $('#ribbon-home-buttonSaveGDocs').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuSave').hide();
        _session.setLastExportMode($(this).attr('id'));

        if (_session.getLicenseValid()) {
            $("#export-gdocs-dialog").dialog("open");
        } else {
            showPremiumDialog('Sending to Google Drive', function () {
                $("#export-gdocs-dialog").dialog("open");
            });
        }
    });

    $('#ribbon-home-buttonSaveSharePoint').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#ribbon-home-menuSave').hide();
        _session.setLastExportMode($(this).attr('id'));

        if (_session.getLicenseValid()) {
            $("#export-sharepoint-dialog").dialog("open");
        } else {
            showPremiumDialog('Sending to SharePoint', function () {
                $("#export-sharepoint-dialog").dialog("open");
            });
        }
    });


    $('#ribbon-home-buttonSave').bind('click.button', function () {
        if (arguments[0].pageY - $(this).offset().top < 53) {
            var buttonId = _session.getLastExportMode() || 'ribbon-home-buttonSavePdf';
            $('#' + buttonId).trigger('mousedown');
        } else {
            var menuX = $(this).offset().left;
            var menuY = $(this).offset().top + $(this).height();

            $('#ribbon-home-menuSave').css({
                left: menuX + "px",
                top: menuY + "px"
            });

            setTimeout(function () {
                $('#ribbon-home-menuSave').show();
            }, 1);
        }
    });

    $('#ribbon-home-buttonDelete').bind('click.button', function () {
        $("#deleteConfirm-dialog").dialog("open");
    });

    $('#ribbon-home-buttonFax').bind('click.button', function () {
        $("#export-fax-dialog").dialog("open");
    });

    $('#ribbon-home-buttonEmail').bind('click.button', function () {
        if (_session.getLicenseValid()) {
            $("#export-email-dialog").dialog("open");
        } else {
            showPremiumDialog('Sending by eMail', function () {
                $("#export-email-dialog").dialog("open");
            });
        }
    });

    $('#ribbon-home-buttonPrint').bind('click.button', function () {
        if (_session.getLicenseValid()) {
            $("#export-print-dialog").dialog("open");
        } else {
            showPremiumDialog('Printing', function () {
                $("#export-print-dialog").dialog("open");
            });
        }
    });

    $('#ribbon-home-buttonSettings').bind('click.button', function () {
        $("#settings-dialog").dialog("open");
    });

    $('#ribbon-home-buttonHelp').bind('click.button', function () {
        window.open('cloudscanhelp/cloudscan.htm');
    });

    $('#ribbon-premium-buttonRegister').bind('click.button', function () {
        register();
    });

    $('#ribbon-premium-button3Month, #buypremium-dialog [name="button3Month"]').bind('click.button', function () {
        window.location = 'https://www.plimus.com/jsp/buynow.jsp?contractId=3041902&custom1=' + _session.getLicenseKey();
    });

    $('#ribbon-premium-button6Month, #buypremium-dialog [name="button6Month"]').bind('click.button', function () {
        window.location = 'https://www.plimus.com/jsp/buynow.jsp?contractId=3043176&custom1=' + _session.getLicenseKey();
    });

    $('#ribbon-premium-button12Month, #buypremium-dialog [name="button12Month"]').bind('click.button', function () {
        window.location = 'https://www.plimus.com/jsp/buynow.jsp?contractId=3043178&custom1=' + _session.getLicenseKey();
    });

    $(document).on('click', '#preview-list .preview-item', function () {
        onPreviewSelected(this);
    });

    $('#ribbon-home-buttonShowScannerUI,#ribbon-ie-buttonAutoDeskew,#ribbon-ie-buttonBlackBorders,#ribbon-ie-buttonPunches').bind("click.button", function () {
        $(this).toggleClass('ui-button-checked');
    });

    $(document).on('contextmenu', '#preview-list .preview-item', function (e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
    });

    $('#scanItem-rotateLeft').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#scanItem-contextMenu').hide();
        var targetId = $('#scanItem-contextMenu').attr('target-id');
        onScanItemRotate(targetId, -1);
    });

    $('#scanItem-rotateRight').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#scanItem-contextMenu').hide();
        var targetId = $('#scanItem-contextMenu').attr('target-id');
        onScanItemRotate(targetId, +1);
    });

    $('#scanItem-menuEdit').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#scanItem-contextMenu').hide();
        var targetId = $('#scanItem-contextMenu').attr('target-id');
        onEditScanItem(targetId);
    });

    $('#scanItem-menuDelete').bind('mousedown', function (e) {
        e.stopPropagation();
        $('#scanItem-contextMenu').hide();
        var targetId = $('#scanItem-contextMenu').attr('target-id');
        onRemoveScanItem(targetId);
    });

    $(document).on('mousedown', '#preview-list .preview-item', function (e) {
        if (e.button == 2) {
            $('#scanItem-contextMenu').css({
                left: e.pageX + "px",
                top: e.pageY + "px"
            });
            $('#scanItem-contextMenu').attr('target-id', this.id);
            setTimeout(function () {
                $('#scanItem-contextMenu').attr('');
                $('#scanItem-contextMenu').show();
            }, 1);
        }
    });

    $("#deleteConfirm-dialog").dialog({
        autoOpen: false,
        height: 180,
        width: 350,
        modal: true,
        resizable: false,
        buttons: {
            "Yes": function () {
                deleteJob();
                $(this).dialog("close");
            },
            No: function () {
                $(this).dialog("close");
            }
        }
    });


    $("#export-gdocs-dialog").dialog({
        autoOpen: false,
        height: 340,
        width: 350,
        modal: true,
        dialogClass: 'ui-dialog-helpbutton',
        open: function () {
            setDialogState(this.id, ['document', 'name', 'password', 'remember'], _session.getExporterSettings('gdocs'));
        },
        buttons: {
            Help: function () {
                window.open('cloudscanhelp/savingtogoogle.htm');
            },
            OK: function () {
                var state = getDialogState(this.id, ['document', 'name', 'password', 'remember']);
                onExportGoogleDocs(state.document, state.name, state.password);
                if (!state.remember) {
                    state.name = state.password = null;
                }

                _session.setExporterSettings('gdocs', state);

                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#export-sharepoint-dialog").dialog({
        autoOpen: false,
        height: 420,
        width: 350,
        modal: true,
        dialogClass: 'ui-dialog-helpbutton',
        open: function () {
            setDialogState(this.id, ['server', 'library', 'filename', 'name', 'password', 'remember'], _session.getExporterSettings('sharepoint'));
        },
        buttons: {
            Help: function () {
                window.open('cloudscanhelp/sharepoint.htm');
            },
            OK: function () {
                var state = getDialogState(this.id, ['server', 'library', 'filename', 'name', 'password', 'remember']);
                onExportSharePoint(state.server, state.library, state.filename, state.name, state.password);
                if (!state.remember) {
                    state.name = state.password = null;
                }

                _session.setExporterSettings('sharepoint', state);

                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#export-fax-dialog").dialog({
        autoOpen: false,
        height: 340,
        width: 350,
        modal: true,
        dialogClass: 'ui-dialog-helpbutton',
        open: function () {
            setDialogState(this.id, ['name', 'password', 'phone', 'remember'], _session.getExporterSettings('fax'));
        },
        buttons: {
            Help: function () {
                window.open('cloudscanhelp/faxservice.htm');
            },
            OK: function () {
                var state = getDialogState(this.id, ['name', 'password', 'phone', 'remember']);
                onExportFax(state.name, state.password, state.phone);
                if (!state.remember) {
                    state.name = state.password = null;
                }

                _session.setExporterSettings('fax', state);

                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#export-fax-confirm-dialog").dialog({
        autoOpen: false,
        height: 'auto',
        width: 350,
        modal: true,
        open: function () {
        }
    });

    $("#export-email-dialog").dialog({
        autoOpen: false,
        height: 'auto',
        width: 350,
        modal: true,
        dialogClass: 'ui-dialog-helpbutton',
        open: function () {
            setDialogState(this.id, ['recipients', 'subject', 'body'], _session.getExporterSettings('email'));
        },
        buttons: {
            Help: function () {
                window.open('cloudscanhelp/sendingbyemail.htm');
            },
            OK: function () {
                var state = getDialogState(this.id, ['recipients', 'subject', 'body']);
                onExportEmail(state.recipients, state.subject, state.body);
                _session.setExporterSettings('email', state);
                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#export-email-dialog .adv-options").on('click', function () {
        var el = $('#export-email-dialog span');
        el.toggle();
        if (el.is(':visible')) {
            $('#export-email-dialog .adv-options').text('<<Hide');
        } else {
            $('#export-email-dialog .adv-options').text('Advanced...');
        }

        return false;
    });

    $("#export-print-dialog").dialog({
        autoOpen: false,
        height: 300,
        width: 350,
        modal: true,
        open: function () {
            $("#export-print-dialog [name='printers']").find('option').remove();
            _toolkit.getPrinters({
                success: function (data) {
                    var options = $("#export-print-dialog [name='printers']");
                    $.each(data.Printers, function () {
                        options.append($('<option />').val(this).text(this));
                    });
                }
            });
        },
        buttons: {
            "Print": function () {
                var state = getDialogState(this.id, ['printers']);
                onExportPrinter(state.printers);
                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#scancompleted-dialog").dialog({
        autoOpen: false,
        height: 160,
        width: 400,
        modal: true,
        resizable: false
    });

    $('#scancompleted-dialog button[name="gotodoc"]').bind('click.button', function () {
        $("#scancompleted-dialog").dialog("close");
    });

    $('#scancompleted-dialog button[name="scan"]').bind('click.button', function () {
        $("#scancompleted-dialog").dialog("close");
        startScanJob(_session.getLastScanMode());
    });

    $("#settings-dialog").dialog({
        autoOpen: false,
        height: 300,
        width: 350,
        modal: true,
        open: function () {
            setDialogState(this.id, ['server'], { server: _session.getServerUrl() });
        },
        buttons: {
            "OK": function () {
                $(this).dialog("close");
                _session.setServerUrl($('#settings-dialog [name="server"]').val());
                connectToSharedScanner(_session.getServerUrl());
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $('#error-dialog').dialog({
        autoOpen: false,
        modal: true
    });

    connectToSharedScanner(_session.getServerUrl());
    setInterval(function () {
        setViewportSize();
    }, 1);

    //
    $('#link-register-palmfax').on('click', function () {
        _statistics.logOtherMessage(103, 'Fax register')
    })

    $('#preview-list').sortable({
        revert: true,
        start: function (e, ui) {
            $(this).attr('data-previndex', ui.item.index());
        },
        stop: function (e, ui) {
            var previndex = $(this).attr('data-previndex');
            var newindex = ui.item.index();
            if (previndex == newindex) {
                return;
            }

            if (_session.getLicenseValid()) {
                updatePreviewNumberes();
                _toolkit.moveImage(_session.getJobId(), ui.item.attr('id'), newindex);
            } else {
                $(this).sortable('cancel');
                showPremiumDialog('Moving pages', null);
            }
        }
    });

});
