'use strict';

angular.module('myApp.temperature', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/temperature', {
        templateUrl: 'temperature/temperature.html',
        controller: 'TemperatureCtrl',
    });
}])

.controller('TemperatureCtrl', ['$scope', '$', 'temperatureService', 'cService', function ($scope, $, temperatureService, counterService) {
    $scope.title = "Temperature";
    $scope.temperature = "Loading...";
    $scope.startTemperature = "Loading...";
    $scope.version = "...";

    $scope.title = counterService.addOne();

    var promise = temperatureService.getTemperature();

    promise.then(
		function (payload) {
		    $scope.startTemperature = payload.data;
		},
		function (errorPayload) {
		    $scope.startTemperature = "Error";
		}
	);

    // Proxy created on the fly
    var myHub = $.connection.iOHub;

    $.connection.hub.url = '/signalr/hubs'
    $.connection.hub.logging = true;

    myHub.client.getVersion = function (version) {
        $scope.version = version;
        $scope.$apply();
    };

    myHub.client.temperatureChanged = function (data) {
        $scope.temperature = data;
        $scope.$apply();
    };

    $.connection.hub.error(function (err) {
        // IE doesn't have console
        //console.log("Error " + err);
    });

    $.connection.hub.start()
        .done(function () {
            $scope.version = "Loading...";
            myHub.server.getVersion();
            $scope.$apply();
        })
        .fail(function () {
            $scope.version = "Error";
            $scope.$apply();
        });

}])

.service('temperatureService', ['$http', TempService]);

function TempService($http) {

    var self = this;

    var info = "Hello from Temperature Service";
    self.count = 0;
    
    var service = {
        info: info,
        getTemperature: getTemperature,
        initCount: initCount
    };

    return service;

    //////////////

    function getTemperature() {
        return $http({ method: 'GET', url: '/r/temperature' });
    };

    function initCount() {
        self.count = self.count + 1;
        return self.count;
    };
}