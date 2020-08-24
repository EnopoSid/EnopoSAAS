(function () {
    'use strict';

    angular
        .module('app')
        .factory('app', app);

    app.$inject = ['$http'];

    function app($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() { }
    }
})();