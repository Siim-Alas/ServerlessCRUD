// Workaround, since position: sticky; doesn't work with the overflow property set.

window.OwnStickifier = {
    stickify: function (element) {
        let originalOffsetTop = element.offsetTop;
        window.addEventListener("resize", function () {
            originalOffsetTop = element.offsetTop;
        });
        document.addEventListener("scroll", function () {
            if (window.pageYOffset > originalOffsetTop) {
                element.classList.add("fixed-top");
            } else {
                element.classList.remove("fixed-top");
            }
        });
    }
}