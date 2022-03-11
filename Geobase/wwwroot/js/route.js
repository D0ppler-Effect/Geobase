'use strict';

function Route(name, htmlName, isDefaultRoute) {
    try {
        if (!name || !htmlName) {
            throw 'Error: name and htmlName parameters are mandatory';
        }
        this.constructor(name, htmlName, isDefaultRoute);
    } catch (e) {
        console.error(e);
    }
}

Route.prototype = {
    name: undefined,
    htmlName: undefined,
    isDefaultRoute: undefined,
    constructor: function(name, htmlName, isDefaultRoute) {
        this.name = name;
        this.htmlName = htmlName;
        this.isDefaultRoute = isDefaultRoute;
    },
    isActiveRoute: function(hashedPath) {
        return hashedPath.replace('#', '') === this.name;
    }
}