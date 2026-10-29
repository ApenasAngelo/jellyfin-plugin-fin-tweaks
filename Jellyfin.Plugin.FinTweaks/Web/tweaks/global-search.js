// S1, global search: the search button of the modern layout links to `#/search?parentId=<library>` while a library
// is open, so the search only covers that library. Send it to the plain search page instead. The legacy/TV layouts
// already open the global search. One capturing click listener, no DOM observers.
(function () {
    function isScopedSearchLink(link) {
        var href = link.getAttribute('href') || '';
        return /^#\/search\?/.test(href) && /(^|[?&])(parentId|topParentId|collectionType)=/.test(href.split('?')[1] || '');
    }

    window.addEventListener('click', function (event) {
        if (event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) {
            return;
        }
        var link = event.target && event.target.closest ? event.target.closest('a[href^="#/search"]') : null;
        if (!link || !isScopedSearchLink(link)) {
            return;
        }
        event.preventDefault();
        event.stopPropagation();
        window.location.hash = '#/search';
    }, true);
})();
