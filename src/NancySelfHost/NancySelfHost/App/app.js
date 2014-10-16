'use strict';

// Declare app level module which depends on views, and components
angular.module('myApp', [
  'ngRoute',
  'counter',
  'jquery',
  'myApp.info',
  'myApp.led_control',
  'myApp.temperature',
  'myApp.version'
])

//.factory('$', ['$window', function ($window) {
//    return $window.jQuery;
//}])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.otherwise({ redirectTo: '/info' });
}]);
