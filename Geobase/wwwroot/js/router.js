'use strict';

function Router(routes) {
    try {
        if (!routes) {
            throw 'Error: routes parameter is mandatory';
        }
        this.constructor(routes);
    } catch (e) {
        console.error(e);
    }
}

Router.prototype = {
    routes: undefined,
    rootElem: undefined,
    constructor: function(routes) {
        this.routes = routes;
        this.rootElem = document.getElementById('content');
    },
    init: function() {
        var r = this.routes;
        (function(scope, r) {
            window.addEventListener('hashChange', function(e) {
                scope.hasChanged(scope, r);
            });
        })(this, r);
    },
    hasChanged: function(scope, routes) {
        if (window.location.hash.length > 0) {
            for (var i = 0, length = routes.length; i < length; i++) {
                var route = routes[i];
                if (route.isActiveRoute(window.location.hash.substr(1))) {
                    scope.goToRoute(route.htmlName);
                }
            }
        } else {
            for (var i = 0, length = routes.length; i < length; i++) {
                var route = routes[i];
                if (route.isDefaultRoute) {
                    scope.goToRoute(route.htmlName);
                }
            }
        }
    },
    goToRoute: function(htmlName) {
        (function(scope) {
            var contentUrl = 'views/' + htmlName;

            var httpRequest = new XMLHttpRequest();
            httpRequest.onreadystatechange = function() {
                if (this.readyState === 4 && this.status === 200) {
                    scope.rootElem.innerHTML = this.responseText;
                }
            };

            httpRequest.open('GET', contentUrl, true);
            httpRequest.send();
        })(this);
    }
}