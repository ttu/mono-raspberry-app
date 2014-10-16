angular.module('jquery', [])

.factory('$', ['$window', function ($window) {
    return $window.jQuery; // jQuery must already be loaded on the page 
}]);