// Slider
$("#slider").lightSlider({
    item: 1,
    slideMargin: 0,
    mode: "fade",
    useCSS: true,
    cssEasing: 'ease', //'cubic-bezier(0.25, 0, 0.25, 1)',//
    easing: 'linear', //'for jquery animation',////
    speed: 400, //ms'
    pause: 4000,
    pauseOnHover: true,
    auto: true,
    loop: true,
    controls: false,
    pager: false,
    rtl: true,
    onSliderLoad: function () {
        $('#slider').removeClass('cS-hidden');
    }
});