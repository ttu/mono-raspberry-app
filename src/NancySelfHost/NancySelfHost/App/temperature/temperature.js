'use strict';

angular.
	module('myApp.temperature', ['ngRoute']).
		config(['$routeProvider', function($routeProvider) {
			$routeProvider.when('/temperature', {
				templateUrl: 'temperature/temperature.html',
				controller: 'TemperatureCtrl'
			});
		}]).
		controller('TemperatureCtrl', ['$scope', 'temperatureService', function($scope, temperatureService){
			$scope.title = "Temperature";
		}]).
		factory('temperatureService', ['$http', TempService]);
		
function TempService($http){
	// TODO
}