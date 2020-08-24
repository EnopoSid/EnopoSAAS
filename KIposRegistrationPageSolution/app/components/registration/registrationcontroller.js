(function () {
    'use strict';

    angular
        .module('app')
        .controller('RegistrationController', RegistrationController);

    RegistrationController.$inject = ['$location', 'authenticateService', '$state', '$localStorage', '$rootScope', 'mainService', '$scope', 'toaster', '$window'];

    function RegistrationController($location, authenticateService, $state, $localStorage, $rootScope, mainService, $scope, toaster, $window) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'RegistrationController';
        vm.globalCountryCodes = [
            { 'Id': 1, 'Code': '+65', 'Value': 'SIN +65', 'maxlength': '8' }];
        vm.selectedCountryCode = fnGetCountryCode(vm.globalCountryCodes[0].Code);

        activate();

        function activate() {
               if(!!$localStorage.IsAuthenticated)
               {
                 $rootScope.$broadcast('broadCastLogout');
               }
         }

        vm.Register=function(ngForm){
            if (ngForm.$invalid) {
                var tempInputs = angular.element('input.ng-invalid,select.ng-invalid');
                var inputs = $.grep(tempInputs, function (e) { return e.form.id == 'vm.RegistrationForm' });

                inputs[0].focus();

                return false;
            }
            fnIsCustomerExists(ngForm);
        }
        function fnGetCountryCode(code) {
            var selectedCode;
            for (var i = 0; i < vm.globalCountryCodes.length; i++) {
                if (code == vm.globalCountryCodes[i].Code) {
                    selectedCode = vm.globalCountryCodes[i];
                    vm.maxdigits = vm.globalCountryCodes[i].maxlength;
                }
            }
            return selectedCode;
        } 
        function fnIsCustomerExists(ngForm) {
            var promiseGet = mainService.crudService("api/Customer/IsUserExists?emailId=" + vm.userData.EmailId + '&mobileNumber=' + vm.userData.PhoneNumber, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                        fnsignUp(ngForm);
                }
            }, function (error) {
                if (error.status == 409 || error.status == 500) {
                    toaster.pop('error', "", "Email already exists");
                }
                console.log(error);
            });
        }

        function fnGenerateRandomPassword() { 
            var pass = ''; 
            var str = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' +  
                    'abcdefghijklmnopqrstuvwxyz0123456789@#$'; 
              
            for (var i = 1; i <= 8; i++) { 
                var char = Math.floor(Math.random() 
                            * str.length + 1); 
                  
                pass += str.charAt(char) 
            } 
              
            return pass; 
        }
        function fnsignUp(ngForm) {
            var password= fnGenerateRandomPassword();
            vm.userData.CustomerGUID = "";
            vm.userData.FirstName = vm.userData.UserName;
            vm.userData.LastName = ".";
            vm.userData.MobileNumber = vm.selectedCountryCode.Code + '-' + vm.userData.PhoneNumber;
            vm.userData.IsGuestUser = false;
            vm.userData.Password=password;

                var promiseGet = mainService.crudService('Api/Client/Register', 'Post', vm.userData);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        subscribetoNewsletter(vm.userData.EmailId);
                        var authData = $localStorage.authorizationData;
                        var apiUrl = $rootScope.myConstants.membershipModuleApiURL;
                        var requestObj = {
                            "MemberBasicDetails": {
                                "FullName": vm.userData.UserName
                            },
                            "MemberLoginDetails": {
                                "Email": vm.userData.EmailId,
                                "MobileNumber": vm.selectedCountryCode.Code + '-' + vm.userData.PhoneNumber,
                                "Password": vm.userData.Password,
                                "CustomerGuid": success.data.CustomerGuid,
                                "CustomerId": success.data.CustomerId,
                                "IsPasswordChanged": false,
                                "IsGuestUser": false,
                                "IsLoyalityMember": true,
                            },
                            "IsFromCampain":true
                        };
                        var pagename = $state.current.name;
                        var promisePost = mainService.crudService('api/MemberDetails/PostDetails', 'Post', requestObj, true);
                        promisePost.then(function (success) {
                            toaster.pop('success', "", "Registered Succesfully");
                            vm.userData={};
                            vm.RegistrationForm.$submitted=false;
                            vm.RegistrationForm.fullname.$touched = false;
                            vm.RegistrationForm.email.$touched = false;
                            vm.RegistrationForm.telephone.$touched = false;
                        });

                    }

                }, function (error) {
                        toaster.pop('error', "", error.data.ErrorMessage);
                });
            }
        
            function subscribetoNewsletter(email) {
                var promiseGet = mainService.crudService("api/NewsLetterSubscription/Subscribe?email=" + email, $rootScope.myConstants.HTTPPost, {}, true);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        // toaster.pop('success', "", "you have Subscribed to Newsletter sucessfully");
                    }
                }, function (error) {
                    console.log(error);
                });
            }

            vm.validateMobileNumber = function (selectedCountry, phonenumber) {
                vm.patternError = false;
                if (!!phonenumber) {
                    for (var i = 0; i < vm.globalCountryCodes.length; i++) {
                        if (selectedCountry.Code == vm.globalCountryCodes[0].Code) {
                            if (phonenumber.length != vm.globalCountryCodes[i].maxlength) {
                                vm.patternError = true;
                                vm.maxdigits = vm.globalCountryCodes[i].maxlength;
                                //vm.IsMobileNumberExists = false;
                            }
                        }
                    }
                }
                else {
                    vm.patternError = true;
                    for (var j = 0; j < vm.globalCountryCodes.length; j++) {
                        if (selectedCountry.Code == vm.globalCountryCodes[j].Code) {
                            vm.maxdigits = vm.globalCountryCodes[j].maxlength;
                        }
                    }
                }
            };
    }
})();
