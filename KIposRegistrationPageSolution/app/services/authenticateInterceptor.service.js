angular
    .module('app')
    .factory('authenticateInterceptorService',
        ['$q', '$location', '$localStorage', '$rootScope', function ($q, $location, $localStorage, $rootScope) {
            var authInterceptorServiceFactory = {};
            var count = 0;
            var _request = function (config) {
                $rootScope.globalRequestCount++;
                config.headers = config.headers || {};
                var authData = $localStorage.authorizationData;
                if (authData) {
                    config.headers.Authorization = 'Bearer ' + authData.token;
                }
                
                //config.headers['Content-Type'] = 'application/json;charset=utf-8;';
                $('#lazyLoaderRoot').removeClass('hide');

                return config;
            };

            var _responseError = function (rejection) {
                if (rejection.status === 401) {
                    $rootScope.user = {};
                    //   $location.path('/login');
                }
                // $('#lazyLoaderRoot').addClass('hide');
                $rootScope.globalResponseCount++;
                if ($rootScope.globalRequestCount == $rootScope.globalResponseCount) {
                    $('#lazyLoaderRoot').addClass('hide');
                }
                return $q.reject(rejection);
            };

            var _responseSuccess = function (success) {
                //$('#lazyLoaderRoot').addClass('hide');
                $rootScope.globalResponseCount++;
                if ($rootScope.globalRequestCount == $rootScope.globalResponseCount) {
                    $('#lazyLoaderRoot').addClass('hide');
                }
                return success;
            };

            authInterceptorServiceFactory.request = _request;
            authInterceptorServiceFactory.response = _responseSuccess;
            authInterceptorServiceFactory.responseError = _responseError;

            return authInterceptorServiceFactory;
        }]);