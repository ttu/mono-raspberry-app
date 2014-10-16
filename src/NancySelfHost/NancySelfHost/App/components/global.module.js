'use strict';

angular.module('counter', [])

.service('cService', CounterService)
.factory('counterService', CounterFactory);

function CounterService() {
    var count = 0;

    this.addOne = function() {
        count = count + 1;
        return count;
    }
}

function CounterFactory() {
    var count = 0;

    var service = {
        addOne : addOne
    };

    return service;

    function addOne()
    {
        count = count + 1;
        return count;
    }
}