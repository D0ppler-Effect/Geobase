'use strict';

(function() {
    function init() {
        var router = new Router([
            new Route('cityLocations', 'cityLocations.html', true),
            new Route('ipLocation', 'ipLocation.html')
        ]);
    }

    init();
}());