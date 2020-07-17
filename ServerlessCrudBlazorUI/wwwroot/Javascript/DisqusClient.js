
// Reference: https://blazor-blog.disqus.com/admin/install/platforms/universalcode/

window.DisqusClient = {
    init: function (pageUrl, pageIdentifier) {
        var disqus_config = function () {
            this.page.url = pageUrl;  // The page's canonical URL variable
            this.page.identifier = pageIdentifier; // The page's unique identifier variable
        };

        (function () {
            var d = document, s = d.createElement('script');
            s.src = 'https://blazor-blog.disqus.com/embed.js';
            s.setAttribute('data-timestamp', +new Date());
            (d.head || d.body).appendChild(s);
        })();
    }
};