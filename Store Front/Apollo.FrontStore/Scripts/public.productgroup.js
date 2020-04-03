$(document).ready(function () {
    $('.owl-carousel').owlCarousel({
        margin: 0,
        nav: true,
        navClass: ['owl-prev', 'owl-next'],
        lazyLoad: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 4
            }
        }
    })
});