// Wheel axis lock: on a trackpad a vertical gesture carries some sideways movement (and vice versa), so a row of cards
// moves while the page scrolls. The start of the gesture picks the axis, and the other one is ignored until the gesture
// ends. Only acts with the pointer over a row: elsewhere the browser does what it always does.
(function () {
    var GESTURE_END_MS = 150; // the macOS momentum keeps sending events, so a gesture ends when they stop
    var axis = null, timer = null;

    // Which element scrolls the page changes with the layout, so the one that moves is the right one.
    function scrollPage(element, delta) {
        var candidates = [];
        for (var node = element.parentElement; node; node = node.parentElement) {
            if (/(auto|scroll)/.test(getComputedStyle(node).overflowY)) {
                candidates.push(node); // an element with overflow hidden would also move, and hide content
            }
        }
        candidates.push(document.scrollingElement);
        for (var i = 0; i < candidates.length; i++) {
            var before = candidates[i].scrollTop;
            candidates[i].scrollTop = before + delta;
            if (candidates[i].scrollTop !== before) {
                return;
            }
        }
    }

    window.addEventListener('wheel', function (event) {
        if (event.ctrlKey) {
            return; // pinch to zoom
        }
        var row = event.target && event.target.closest ? event.target.closest('.emby-scroller') : null;
        if (!row) {
            return;
        }
        var sideways = Math.abs(event.deltaX), down = Math.abs(event.deltaY);
        if (axis === null) {
            axis = sideways > down ? 'x' : 'y';
        }
        clearTimeout(timer);
        timer = setTimeout(function () {
            axis = null;
        }, GESTURE_END_MS);

        if (axis === 'x' && down > 0) {
            event.preventDefault(); // the row still gets the event and moves; the page does not
        } else if (axis === 'y' && sideways > 0) {
            // The row reads deltaX by itself, so it must not get this event: the page is scrolled here instead.
            event.preventDefault();
            event.stopImmediatePropagation();
            scrollPage(row, event.deltaMode === 1 ? event.deltaY * 16 : event.deltaY);
        }
    }, { capture: true, passive: false });
})();
