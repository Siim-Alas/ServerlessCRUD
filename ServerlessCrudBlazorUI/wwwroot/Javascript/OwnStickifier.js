// Workaround, since position: sticky; doesn't work with the overflow property set.

window.OwnStickifier = {
    stickify: function (element, elementContainingAllAbove) {
        document.addEventListener("scroll", function () {
            if (window.pageYOffset > elementContainingAllAbove.offsetHeight) {
                element.classList.add("fixed-top");
            } else {
                element.classList.remove("fixed-top");
            }
        });
    }
}