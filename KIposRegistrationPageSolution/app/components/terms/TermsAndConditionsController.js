(function () {
    'use strict';

    angular
        .module('app')
        .controller('TermsAndConditionsController', TermsAndConditionsController);

    TermsAndConditionsController.$inject = ['$location'];

    function TermsAndConditionsController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'TermsAndConditionsController';

        activate();

        function activate() { }
    }
})();
