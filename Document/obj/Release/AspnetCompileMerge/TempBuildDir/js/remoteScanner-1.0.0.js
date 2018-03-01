function RemoteScanner(selector) {
    
    this._imgUrl = null;
    this._scanBtn = null;
    this.init = function (sss) {
        var self = this;
        var cont = $('<div></div>').appendTo(sss);
    
        self._imgUrl = $('<input type="text" value="http://192.168.1.33:57632/Scan/json/repository/getjobimage?jobid=398536aa-e4ad-4b8c-be2e-6221d60d9f28&imageid=04f5d999-6790-4de2-af66-39a3f9876306&lowres=false&rotate=0" style="width: 700px;" />').appendTo(cont);
        self._scanBtn = $('<input type="button" value="scan" />').appendTo(cont)
        .click(function() {
            self._scanBtnClick();
        });
    };

    this.init(selector);
};


RemoteScanner.prototype._scanBtnClick = function () {
    this.showScanImg(this._imgUrl.val());
};

RemoteScanner.prototype._loadToCanvas = function (dataUrl, canvas) {
    var context = canvas.getContext('2d'),
        imageObj = new Image();
    
    imageObj.crossOrigin = 'Anonymous';
    imageObj.addEventListener('load', function () {
        canvas.width = imageObj.width;
        canvas.height = imageObj.height;
        context.drawImage(this, 0, 0);
    });
    imageObj.src = dataUrl;
    context.drawImage(imageObj, 0, 0);
};
RemoteScanner.prototype._loadToCanvas2 = function (dataUrl, canvas) {
    var context = canvas.getContext('2d');
    /*
    var imageObj = document.createElement('img');
    imageObj.crossOrigin = 'Anonymous';
    $(imageObj).load(function () {
        canvas.width = $(this)[0].width;
        canvas.height = $(this)[0].height;
        context.drawImage($(this)[0], 0, 0);
        console.log(canvas.toDataURL());
        
        $('<a target="_blank">test scan</a>').attr('href', canvas.toDataURL("image/jpeg")).appendTo('body');
    });
    $(imageObj)[0].src = dataUrl;
    context.drawImage($(imageObj)[0], 0, 0);
    */
    var imageObj = $('<img alt="" />').appendTo('body').load(function () {
        canvas.width = $(this)[0].width;
        canvas.height = $(this)[0].height;
        context.drawImage($(this)[0], 0, 0);
        console.log(canvas.toDataURL());
        
        $('<a target="_blank">test scan</a>').attr('href', canvas.toDataURL("image/jpeg")).appendTo('body');
    });
    $(imageObj)[0].src = dataUrl;
    context.drawImage($(imageObj)[0], 0, 0);
    
};

RemoteScanner.prototype._getBase64Image = function(img) {
    var canvas = document.createElement("canvas");
    //canvas.width = img.width;
    //canvas.height = img.height;

    // Copy the image contents to the canvas
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);

    // Get the data-URL formatted image
    // Firefox supports PNG and JPEG. You could check img.src to
    // guess the original format, but be aware the using "image/jpg"
    // will re-encode the image.
    var dataUrl = canvas.toDataURL("image/png");

    return dataUrl.replace(/^data:image\/(png|jpg);base64,/, "");
};

RemoteScanner.prototype.showScanImg = function (imgUrl) {
    var imageViewer = document.createElement("div");
    imageViewer.style.position = "absolute";
    imageViewer.style.top = "20px";
    imageViewer.style.left = "20px";
    imageViewer.style.zIndex = "200";

    var canvas = document.createElement("canvas");
    imageViewer.appendChild(canvas);
    document.body.appendChild(imageViewer);

    //this._loadToCanvas2(imgUrl, canvas);
    
    var ctx = canvas.getContext("2d");
    /*
    var img = document.createElement('img');
    imageViewer.appendChild(img);

    img.onload = function() {
        canvas.width = img.width;
        canvas.height = img.height;
        ctx.drawImage(img, 0, 0);
        console.log(canvas.toDataURL());
    };
    img.crossOrigin = 'anonymous';
    img.src = imgUrl;
    img.width = 700;
    img.height = 700;
    */
    canvas.width = 200;
    canvas.height = 200;
    
    var imageObj = $('<img id="scimg" alt="anonymous" style="width: 400px; height: 400px;" />').appendTo('body').load(function () {
        ctx.drawImage($(this)[0], 0, 0);
        console.log(canvas.toDataURL());
        //$('<img alt="" />').attr('src', canvas.toDataURL("image/jpeg")).appendTo('body');
        $('<a target="_blank">test scan</a>').attr('href', canvas.toDataURL("image/jpeg")).appendTo('body');
    });
    var scimg = document.getElementById('scimg');

    //imageObj.attr('crossorigin', 'anonymous');
    imageObj.attr('src', imgUrl);

    //setTimeout(function() {
        ctx.drawImage(scimg, 0, 0);
        var imgdata = canvas.toDataURL("image/jpeg");
        console.log(imgdata);
        console.log(imgdata.length);
        
        $('<a target="_blank">test scan</a>').attr('href', canvas.toDataURL("image/jpeg")).appendTo('body');
    //}, 3000);

};
/*
$(function() {
    var rews = new RemoteScanner('body');
});
*/