'use strict';

angular.module('myApp.led_control', ['ngRoute'])

.config(['$routeProvider', function($routeProvider) {
  $routeProvider.when('/led_control', {
    templateUrl: 'led_control/led_control.html',
    controller: 'LedControlCtrl'
  });
}])

.controller('LedControlCtrl', ['$http', LedControl]);

function LedControl($http){
	this.title = "Led Control";
}

