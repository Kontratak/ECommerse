$(document).ready(function () {
    var splide = new Splide('.splide', {
        perPage: 3,
        rewind: true,
    });
    splide.mount();
});
function changeMain(image) {
    $('#mainImage').attr('src', image);
}