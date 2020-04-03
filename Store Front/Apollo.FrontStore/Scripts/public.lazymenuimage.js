(function () {
$('#nav-header .menu-top .menu-middle li').hover(
    function () {
        let images = $(this).find('.later');

        images.each(function () {
            var src = $(this).data('original');
            $(this).attr('src', src);
            $(this).removeAttr('data-original');
        });
    }
)})();