(function () {
    'use strict';

    angular
        .module('app')
        .factory('mainService', app);

    app.$inject = ['$http', '$rootScope', '$localStorage', '$q'];

    function app($http, $rootScope, $localStorage, $q) {
        var service = {
            crudService: crudService
        };

        return service;

        function crudService(api, method, inputObj, isFromMembership, customerGuid) {

            if (isFromMembership === "") {
                isFromMembership = false;
            }

            if (customerGuid === "") {
                customerGuid = false;
            }

            var deferred = $q.defer();

            if (method == $rootScope.myConstants.HTTPPost && !isFromMembership) {
                inputObj.ApiSecretKey = $rootScope.myConstants.apiKey;
                inputObj.StoreId = $rootScope.myConstants.StoreId;
                inputObj.LanguageId = $rootScope.myConstants.LanguageId;
                inputObj.CurrencyId = $rootScope.myConstants.CurrencyId;
            }

            if (!!$localStorage.authorizationData) {
                if (customerGuid != null) {
                    inputObj.CustomerGUID = customerGuid;

                    apiCall(api, method, inputObj, isFromMembership, deferred);
                }
                else {
                    if (!!$rootScope.globalCustomerDetails.CustomerGuid) {
                        inputObj.CustomerGUID = $rootScope.globalCustomerDetails.CustomerGuid;

                        apiCall(api, method, inputObj, isFromMembership, deferred);
                    }
                    else {
                        //$rootScope.globalRequestCount++;
                        $http({
                            method: 'GET',
                            url: $rootScope.myConstants.membershipModuleApiURL + '/' + 'api/Customer/Details?emailId=' + $localStorage.authorizationData.EmailId + '',
                            data: {}
                        }).then(function (success) {
                            if (success.status == 200) {
                                $rootScope.globalCustomerDetails = success.data;
                                inputObj.CustomerGUID = $rootScope.globalCustomerDetails.CustomerGuid;

                                apiCall(api, method, inputObj, isFromMembership, deferred);

                            }
                        }, function (error) {
                            $rootScope.globalCustomerDetails = {};
                            console.log(error);
                        });
                    }
                }
            }
            else {
                //inputObj.CustomerGuid = $rootScope.myConstants.customerGUID;
                if (!!$rootScope.globalCustomerDetails.CustomerGuid) {
                    //if ($rootScope.globalCustomerDetails.CustomerGuid === "00000000-0000-0000-0000-000000000000") {
                    //    $localStorage.CustomerType = '0';
                    //}
                    inputObj.CustomerGuid = $rootScope.globalCustomerDetails.CustomerGuid;
                    // if ($localStorage.CustomerType === '0') {
                    $localStorage.CustomerGuid = $rootScope.globalCustomerDetails.CustomerGuid;
                    //}
                }
                else if (!!$localStorage.CustomerGuid) {

                    $rootScope.globalCustomerDetails.CustomerGuid = $localStorage.CustomerGuid;
                }
                else if (!$localStorage.CustomerGuid) {
                    inputObj.CustomerGuid = $rootScope.myConstants.customerGUID;
                }
                apiCall(api, method, inputObj, isFromMembership, deferred);
            }

            return deferred.promise;
        }


        function apiCall(api, method, inputObj, isFromMembership, deferred) {
            var apiUrl = '';
            if (!isFromMembership) {
                apiUrl = $rootScope.myConstants.apiURL;
            }
            else {
                apiUrl = $rootScope.myConstants.membershipModuleApiURL;
            }
            // $rootScope.globalRequestCount++;
            var request = $http({
                method: method,
                url: apiUrl + '/' + api,
                data: inputObj,
                headers : {
                    'Content-Type': 'application/json;charset=utf-8;'
                }
            });

            deferred.resolve(request);
        }
    }
})();