// TODO

angular.module('underscore', [])

.factory('_', ['$window', function($window) { 
    return $window._; // Underscore must already be loaded on the page 
});