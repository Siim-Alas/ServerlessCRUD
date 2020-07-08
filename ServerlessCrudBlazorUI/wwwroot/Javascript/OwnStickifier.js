// Workaround, since position: sticky; doesn't work with the overflow property set.

window.OwnStickifier = {
    stickify: function (element, elementContainingAllAbove, dummyElement) {
        document.addEventListener("scroll", function () {
            if (window.pageYOffset > elementContainingAllAbove.offsetHeight) {
                dummyElement.style.height = `${element.offsetHeight}px`;
                element.classList.add("fixed-top");
            } else {
                element.classList.remove("fixed-top");
                dummyElement.style.height = "0";
            }
        });
    }
};