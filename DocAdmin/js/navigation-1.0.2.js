var navigation = {
    appendTo: 'body',
    onSelect: null,

    create: function (options) {
        var list = [];
        if (options) {
            if (options.appendTo) {
                navigation.appendTo = options.appendTo;
            }
            if (options.list) {
                list = options.list;
            }
            if (options.onSelect) {
                navigation.onSelect = options.onSelect;
            }
        }

        $(navigation.appendTo).remove('div.navigate');
        $(navigation.appendTo).append('<div class="navigate"><h4>Меню</h4><dl></dl></div>');

        for (var i = 0; i < list.length; ++i) {
            var el = list[i];
            if (el.href) {
                var link = $('<a href="#' + el.href + '" dep="' + el.dep + '" target="demo-frame">' + el.name + '</a>').click(function (e) {
                    if (!$(this).parent().hasClass('navigation-on')) {
                        if (navigation.onSelect instanceof Function) {
                            navigation.onSelect(e, el);
                        }
                    }
                    $(this).parents('dl').find('dd').removeClass('navigation-on');
                    $(this).parent().addClass('navigation-on');

                    window.location.hash = this.getAttribute('href').match((/(#[\w\-]+)?/))[1];
                    e.preventDefault();
                });
                if (el.tmp) {
                    link.attr("tmp", el.tmp);
                }
                $(navigation.appendTo + ' div.navigate dl').append($('<dd></dd>').append(link));
            } else {
                $(navigation.appendTo + ' div.navigate dl').append($('<dt>' + el.name + '</dt>'));
            }
        }
    },

    select: function (target) {
        $(navigation.appendTo + ' div.navigate a[href="' + target + '"]').click();
    },

    deselect: function () {
        $('div.navigate dl').find('dd').removeClass('navigation-on');
    },

    init: function () {
        if (window.location.hash != '') {
            navigation.select(window.location.hash);
        }
    }
};
