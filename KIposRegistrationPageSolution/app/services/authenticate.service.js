angular
    .module('app')
    .factory('authenticateService', ['$http', '$q', '$localStorage', 'constants', '$rootScope', function ($http, $q, $localStorage, constants, $rootScope) {

        var authServiceFactory = {};
        
        var _login = function (loginData, IsFromPOS) {

            if (IsFromPOS === "") {
                IsFromPOS = false;
            }

            var clientType = null;
            if (!!IsFromPOS) {
                clientType = $rootScope.myConstants.POSClientType;
            } else if (!IsFromPOS) {
                clientType = $rootScope.myConstants.StoreClientType;
            }
            var data = "grant_type=password&username=" + loginData.EmailId + "&password=" + loginData.Password + "&ClientType=" + clientType;

            var deferred = $q.defer();

            $http.post(constants.membershipModuleApiURL + '/api/authtoken'  , data, {
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                })
                .success(function (response) {

                    $localStorage.authorizationData = {
                        token: response.access_token,
                        EmailId: response.UserName,
                        MemberId: response.MemberId,
                        UserId: parseInt(response.id)
                    };
                    $localStorage.IsAuthenticated = true;
                   
                    deferred.resolve(response);

                }).error(function (err, status) {
                    console.log(err);
                    _logOut();
                    deferred.reject(err);
                });

            return deferred.promise;

        };

        var _logOut = function () {
            $localStorage.$reset();
            $rootScope.nameOfLoggedInUser = !$localStorage.authorizationData ? null : $localStorage.authorizationData.userName;

            // _authentication.IsAuthenticated = false;
            // _authentication.email = "";
            // _authentication.userId = 0;
            // _authentication.customerName = "";

        };

        var userData = function () {
            return {
                IsAuthenticated: $localStorage.IsAuthenticated,
                UserId: $localStorage.userId,
                Name: $localStorage.customerName
            };
        }

        authServiceFactory.login = _login;
        authServiceFactory.logout = _logOut;
        authServiceFactory.userData = userData;

        return authServiceFactory;
    }]);