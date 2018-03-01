$(function () {
    var contentFrame = $(".content-frame"),
    helpList = $(".navigate"),
    currentItem = location.hash.substring(1);

    contentFrame.attr('src', appSettings.rootUrl + 'resources/default.html')
        .css({ width: $(window).width() - 200, height: $(window).height() - 120 });

    helpList.find("a").each(function () {
        if ($(this).attr('href').substr(0, 4) !== 'http') {
            $(this).attr('href', appSettings.rootUrl + $(this).attr('href'));
        }
    });

    helpList.on("click", "a", function (event) {
        event.preventDefault();
        var filename = "/" + event.target.pathname.replace(/^\//, ""),
        parts = filename.split("/"),
        demo = parts[parts.length - 1].substring(0, parts[parts.length - 1].length - 5);
       
        contentFrame.attr("src", event.target.href);
        
        helpList.find(".navigation-on").removeClass("navigation-on");
        $(this).parent().addClass("navigation-on");
        location.hash = "#" + demo;
    });

    if (currentItem) {
        helpList.find("a").filter(function () {
            return this.pathname.split("/")[4] === (currentItem + ".html");
        }).click();
    }
});