(function () {
    'use strict';

    angular
        .module('app')
        .controller('DisclaimerController', DisclaimerController);

    DisclaimerController.$inject = ['$location'];

    function DisclaimerController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'DisclaimerController';

        activate();

        function activate() { }
    }
})();
