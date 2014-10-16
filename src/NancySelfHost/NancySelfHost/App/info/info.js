'use strict';

angular.module('myApp.info', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/info', {
        templateUrl: 'info/info.html',
        controller: 'InfoCtrl',
        controllerAs: 'info'
    });
}])

.controller('InfoCtrl', ['infoService', 'cService', InfoControl])

.service('infoService', ['$http', InfoService]);

function InfoControl(infoService, counterService) {
    var self = this;

    self.title = "Info";
    self.count = counterService.addOne();
    self.os = "Unknown";

    var promise = infoService.getOS();

    promise.then(
		function (payload) {
		    self.os = payload.data;
		},
		function (errorPayload) {
		    self.os = "Error";
		}
	);
}

function InfoService($http) {
    return {
        getOS: function () {
            return $http({ method: 'GET', url: '/os' });
        }
    };
}