(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['$location', 'authenticateService', '$state', '$localStorage', '$rootScope', 'mainService', '$scope', 'toaster', '$window'];

    function HomeController($location, authenticateService, $state, $localStorage, $rootScope, mainService, $scope, toaster, $window) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'HomeController';
        vm.userData = {};
        vm.formStage = 1;
        vm.userData.EOTP = '';
        vm.userData.DTOP = '';
        vm.forgotFlow = 0;
        vm.showProducts = false;
        vm.navigationOpen = false;
        vm.IsUserLogin = false;
        vm.IsOTPValidated = false;
        vm.noData = false;
        vm.processing = false;
        vm.gourmetAndGrocery = true;
        vm.freshItems = false;
        vm.isfromcheckout=false;
        vm.membershipModuleApiURL = $rootScope.myConstants.membershipModuleApiURL
        vm.globalCountryCodes = [
            //{ 'Id': 1, 'Code': '+61', 'Value': 'AUS +61' },
        //{ 'Id': 2, 'Code': '+91', 'Value': 'IND +91' },
        { 'Id': 3, 'Code': '+65', 'Value': 'SIN +65' }];
        vm.selectedCountryCode = vm.globalCountryCodes[0];
        vm.isPOSRoute = $state.current.name;
        vm.selectedProductAttributeInfo = [];

        $rootScope.globalPickedPlanId = 0;

        function activate() {
            setTimeout(function () {
                startSlick();
            }, 1000);

            $rootScope.posUser = $localStorage.IsPOSUser;
            if (!!$localStorage.captureEmail || !!$localStorage.captureEmailMobile) {
                $scope.captureEmail = $localStorage.captureEmail;
                $scope.captureEmailMobile = $localStorage.captureEmailMobile;
            }
            vm.IsUserLogin = $localStorage.IsAuthenticated;
            $rootScope.IsAuthenticated = $localStorage.IsAuthenticated;
            
            fnGetTopNSiteReviews($rootScope.myConstants.NumberofReviewstobeLoaded);
            AboutUsByIsActiveIsCanview();
            GetLandingPageBannerImages();
            GetSponsersImages();
            GettrendingImages();
            if ($rootScope.globalPreviousState == 'commonCheckout') {
                fnRemoveAppliedDiscounts();
            }
            /*
             *Commented By Praveen - 01-12-2018
            var promiseGet = mainService.crudService('api/Plans', 'GET', {}, true);
            promiseGet.then(function (success) {

            }, function (error) {
                console.log(error);
                });
                
            */

            //if ($state.current.name == 'home' || $state.current.name == '') {

            //}
            //else {
            fnGetAllDiscounts();
            //}

            //getAllUserPurchaseInfoIdAndUserId('CART');
            if (!$rootScope.globalPreviousState) {
                getAllUserPurchaseInfoIdAndUserId('NOREDIRECT');
                fnGetShippingLimit();
            }
        }
        

    // function myFunction(x) {
    //   if (x.matches) { // If media query matches
    //     var IDLE_TIMEOUT = $rootScope.myConstants.SessionTimeOutTimeForMobile; //seconds
    //     localStorage.setItem("setTimeOut", "0");

    //     document.onclick = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     document.onmousemove = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     document.onkeypress = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     document.onfocus = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     setInterval(CheckIdleTime, 1000);
    //     //  console.log('Here is te POS status: '+localStorage.getItem("ngStorage-IsPOSUser"));
    //     // if (localStorage.getItem("ngStorage-IsPOSUser") == '""' || localStorage.getItem("ngStorage-IsPOSUser") == 'false' || localStorage.getItem("ngStorage-IsPOSUser") === false) {
    //     //     // console.log('It will navigates to login page');
    //     //     window.setInterval(CheckIdleTime, 1000);
    //     // }
    //     function CheckIdleTime() {
    //         localStorage.setItem("setTimeOut", parseInt(localStorage.getItem("setTimeOut")) + 1);
    //         if (localStorage.getItem("setTimeOut") >= IDLE_TIMEOUT) {
    //             // alert('Times up!, You are idle for about 1 minute, Please login to continue');
    //             localStorage.clear();
    //             // window.location.reload();
    //             window.location.href = $rootScope.myConstants.SiteUrl;
    //             if (localStorage.length > 0) {
    //                 if (!$localStorage.IsAuthenticated && !$localStorage.authorizationData) {
    //                     $rootScope.$broadcast('broadCastLogout');
    //                 }
    //             }

    //         }
    //     }
    //   } else {
    //     var IDLE_TIMEOUT = $rootScope.myConstants.SessionTimeOutTimeForDesktop; //seconds
    //     localStorage.setItem("setTimeOut", "0");

    //     document.onclick = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     document.onmousemove = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     document.onkeypress = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     document.onfocus = function () {
    //         localStorage.setItem("setTimeOut", "0");
    //     };

    //     window.setInterval(CheckIdleTime, 1000);
    //     //  console.log('Here is te POS status: '+localStorage.getItem("ngStorage-IsPOSUser"));
    //     // if (localStorage.getItem("ngStorage-IsPOSUser") == '""' || localStorage.getItem("ngStorage-IsPOSUser") == 'false' || localStorage.getItem("ngStorage-IsPOSUser") === false) {
    //     //     // console.log('It will navigates to login page');
    //     //     window.setInterval(CheckIdleTime, 1000);
    //     // }
    //     function CheckIdleTime() {
    //         localStorage.setItem("setTimeOut", parseInt(localStorage.getItem("setTimeOut")) + 1);
    //         if (localStorage.getItem("setTimeOut") >= IDLE_TIMEOUT) {
    //             // alert('Times up!, You are idle for about 1 minute, Please login to continue');
    //             localStorage.clear();
    //             // window.location.reload();
    //             window.location.href = $rootScope.myConstants.SiteUrl;
    //             if (localStorage.length > 0) {
    //                 if (!$localStorage.IsAuthenticated && !$localStorage.authorizationData) {
    //                     $rootScope.$broadcast('broadCastLogout');
    //                 }
    //             }

    //         }
    //     }
    //   }
    // }
    
    //var x = window.matchMedia("(max-width: 700px)")
    //myFunction(x) // Call listener function at run time
    //x.addListener(myFunction) // Attach listener function on state changes
    
        vm.showPages = function () {
            if (!!$localStorage.IsPOSUser) {
                $state.go('registration');
                if ($state.current.name == "registration") {
                    window.location.reload();
                }
            }
            else {
                // $state.go('gourmetproducts');
                $state.go('registration');
            }
        }
        $scope.$on('broadCastGotoShop', function () {
            vm.showPages();
        });
        vm.getConfigData = function () {
            var promiseGet = mainService.crudService("api/Admin/Configuration/Get", $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    let LeadTimeforGourmetDelivery = success.data.filter(x => x.Key == 'LeadTimeforGourmetDelivery');
                    $rootScope.LeadTimeforGourmetDelivery = parseInt(LeadTimeforGourmetDelivery[0].Value);
                    localStorage.setItem("LeadTimeforGourmetDelivery", LeadTimeforGourmetDelivery);

                    let LeadTimeforGourmetPickUp = success.data.filter(x => x.Key == 'LeadTimeforGourmetPickUp');
                    $rootScope.LeadTimeforGourmetPickUp = parseInt(LeadTimeforGourmetPickUp[0].Value);
                    localStorage.setItem("LeadTimeforGourmetPickUp", LeadTimeforGourmetPickUp);

                    let StoreOpeningTimeOnWeekDays = success.data.filter(x => x.Key == 'STORE MON TO FRI  OPEN TIME');
                    $rootScope.StoreOpeningTimeOnWeekDays = StoreOpeningTimeOnWeekDays[0].Value;
                    localStorage.setItem("StoreOpeningTimeOnWeekDays", StoreOpeningTimeOnWeekDays);

                    let StoreClosingTimeOnWeekDays = success.data.filter(x => x.Key == 'STORE MON TO FRI CLOSE TIME');
                    $rootScope.StoreClosingTimeOnWeekDays = StoreClosingTimeOnWeekDays[0].Value;
                    localStorage.setItem("StoreClosingTimeOnWeekDays", StoreClosingTimeOnWeekDays);

                    let StoreOpeningTimeOnWeekend = success.data.filter(x => x.Key == 'STORE SAT TO SUN OPEN TIME');
                    $rootScope.StoreOpeningTimeOnWeekend = StoreOpeningTimeOnWeekend[0].Value;
                    localStorage.setItem("StoreOpeningTimeOnWeekend", StoreOpeningTimeOnWeekend);

                    let StoreClosingTimeOnWeekend = success.data.filter(x => x.Key == 'STORE SAT TO SUN CLOSE TIME');
                    $rootScope.StoreClosingTimeOnWeekend = StoreClosingTimeOnWeekend[0].Value;
                    localStorage.setItem("StoreClosingTimeOnWeekend", StoreClosingTimeOnWeekend);
                }
            }, function (error) {
                console.log(error);
            });
        }

        $scope.$on('broadCastGetConfigData', function () {
            vm.getConfigData();
        });

        activate();

        $scope.$on('userauthentication', function () {
            activate();

        });
        vm.search = function (param) {
            if (param == "show") {
                vm.showProducts = false;
            }
        }
        vm.globalSearchDivClose = function () {

            vm.showProducts = false;

        }
        vm.clikGlobalSearch = function (searchValue) {
            vm.noData = false;
            vm.searchData = searchValue;
            if (searchValue.length >= 3) {
                vm.processing = true;
                vm.requestdata = {
                    "KeyWord": searchValue
                };
                var promiseGet = mainService.crudService('Api/Client/GlobalSearchForGourmet', $rootScope.myConstants.HTTPPost, vm.requestdata);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        vm.allSearchProducts = success.data;
                        if (vm.allSearchProducts.length == 0) {
                            vm.showProducts = false;
                            vm.noData = true;
                            vm.processing = false;
                        } else {
                            vm.showProducts = true;
                            vm.noData = false;
                            vm.processing = false;
                        }
                    }
                }, function (error) {
                    console.log(error);
                });
            } else {
                vm.showProducts = false;
            }

        };

        vm.clk_SearchProduct = function (product_Id) {
            vm.showProducts = false;
            $state.go('gourmetProductDetails', { productId: product_Id, productInfoId: 0 });
            document.getElementById("exampleInputsearch").value = "";
        };

        vm.setFlag = function () {
            vm.navigationOpen = true;
        };

        vm.clkLogin = function (ngForm) {
            if (ngForm.$invalid) {
                var tempInputs = angular.element('input.ng-invalid,select.ng-invalid');
                var inputs = $.grep(tempInputs, function (e) { return e.form.id == 'vm.frmLogin' });

                inputs[0].focus();

                return false;
            }
            var pagename = $state.current.name;
            vm.getCustomerGuid(vm.loginData, pagename);
        };

        vm.getCustomerGuid = function (signInData, pagename) {
            var promiseGet = mainService.crudService('api/Customer/Details?emailId=' + signInData.EmailId + '', 'GET', {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {

                    $rootScope.globalCustomerDetails = success.data;
                    if (!!$rootScope.globalCartInformation.CustomerGuid) {
                        $rootScope.globalCartInformation.CustomerGuid = $rootScope.globalCustomerDetails.CustomerGuid;
                    }

                    var authenticateObj = {
                        EmailId: signInData.EmailId,
                        Password: signInData.Password
                    };
                    authenticateUser(authenticateObj, false, pagename);
                }
            }, function (error) {
                $rootScope.globalCustomerDetails = {};
                toaster.pop('error', "", "User doesn't exists");
            });
        };


        vm.update_Cart = function (pagename) {

            var JsonCartList = new Array();
            if (!!$rootScope.globalCartInformation.Items) {
                for (var i = 0, len = $rootScope.globalCartInformation.Items.length; i < len; i++) {
                    var jsonCart = new Object();
                    jsonCart.ItemId = $rootScope.globalCartInformation.Items[i].ProductId;
                    jsonCart.Quantity = $rootScope.globalCartInformation.Items[i].Quantity;
                    jsonCart.ParentCategoryId = $rootScope.globalCartInformation.Items[i].ParentCategoryId;
                    jsonCart.ShoppingCartTypeId = "1";
                    jsonCart.AttributeControlIds = $rootScope.globalCartInformation.Items[i].AttributeInfoAsArrayList;
                    JsonCartList.push(jsonCart);
                }

                var jsonCartArray = JSON.parse(JSON.stringify(JsonCartList));

                var requestObj = {

                    "CustomerGUID": $rootScope.globalCustomerDetails.CustomerGuid,
                    "CartItems": jsonCartArray,
                    "ShoppingCartTypeId": "1"
                };

                var promiseGet = mainService.crudService("Api/Client/UpdateCartWithMultipleItems", $rootScope.myConstants.HTTPPost, requestObj);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        $rootScope.globalOrderSummary = {};
                        $rootScope.globalCartInformation = {};
                        toaster.pop('success', "", "Product has been added to Cart Successfully");
                        if (pagename == "commonCheckout") {
                            $rootScope.$emit('EmitLoadProductsAfterLogin');
                        } else {
                            $rootScope.$broadcast('broadCastGetAllUserPurchaseInfo', $rootScope.myConstants.PAGE_NAMES.NOREDIRECT);
                        }
                        $('#modal-signin').modal('hide');
                        vm.isProductClicked = false;
                    }
                }, function (error) {
                    console.log(error);
                });
            } else {
                var MealPlanModelArray = [];
                for (var a = 0; a < $rootScope.globalCartInformation.mealPlanModels.length; a++) {
                    var tempArray = [];
                    for (var aa = 0; aa < $rootScope.globalCartInformation.mealPlanModels[a].Items.length; aa++) {
                        var obj = {
                            "MealNumber": $rootScope.globalCartInformation.mealPlanModels[a].Items[aa].MealNumber,
                            "ProductId": $rootScope.globalCartInformation.mealPlanModels[a].Items[aa].ProductId,
                            "Quantity": $rootScope.globalCartInformation.mealPlanModels[a].Items[aa].Quantity,
                            "MealDate": $rootScope.globalCartInformation.mealPlanModels[a].Items[aa].MealDate,
                            "MealTime": $rootScope.globalCartInformation.mealPlanModels[a].Items[aa].MealTime,
                            "AttributeControlIds": $rootScope.globalCartInformation.mealPlanModels[a].Items[aa].AttributeInfoAsArrayList
                        };
                        tempArray.push(obj);
                    }
                    var tempObj = {
                        "MealPlanName": $rootScope.globalCartInformation.mealPlanModels[a].MealPlanName,
                        "MealPlanId": $rootScope.globalCartInformation.mealPlanModels[a].MealPlanId,
                        "Items": tempArray
                    };
                    MealPlanModelArray.push(tempObj);
                }

                var freshRequestObj = {
                    "ShoppingCartTypeId": "1",
                    "ParentCategoryId": $rootScope.myConstants.CategoryId_Fresh,
                    "mealDetails": MealPlanModelArray
                };

                var freshUpadteGet = mainService.crudService("Api/Client/UpdateFreshCartWithMultipleItems", $rootScope.myConstants.HTTPPost, freshRequestObj);
                freshUpadteGet.then(function (success) {
                    if (success.status == 200) {
                        $rootScope.globalOrderSummary = {};
                        $rootScope.globalCartInformation = {};
                        toaster.pop('success', "", "Product has been added to Cart Successfully");
                        //$rootScope.$broadcast('broadCastGetAllUserPurchaseInfo', $rootScope.myConstants.PAGE_NAMES.NOREDIRECT);
                        //vm.isProductClicked = false;
                        if (pagename == "freshShipping") {
                            $rootScope.$emit('EmitLoadProductsAfterLogin');
                        } else {
                            $rootScope.$broadcast('broadCastGetAllUserPurchaseInfo', $rootScope.myConstants.PAGE_NAMES.NOREDIRECT);
                        }
                        $('#modal-signin').modal('hide');
                    }
                }, function (error) {
                    console.log(error);
                });
            }

        };

        function validateNewLetter() {
            if ($localStorage.IsAuthenticated) {
                return true;
            } else {
                if (screen.width > 767) {
                    if ($('#captureEmail').val() === '') {
                        $('#captureEmail').focus();
                        angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                        $(".tooltip").show();
                        return false;
                    }
                } else if (screen.width <= 767) {
                    if ($('#captureEmailMobile').val() === '') {
                        $('#captureEmailMobile').focus();
                        $(".tooltip").show();
                        return false;
                    }
                }
            }
        }

        vm.getnewsletter = function (ngForm) {

            validateNewLetter();

            if (ngForm.$invalid) {
                angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                return false;
            }

            $rootScope.globalEmailForNewsLetter = vm.globalEmailForNewsLetter;
            vm.getdata = {
                "Email": vm.globalEmailForNewsLetter
            };

            if (!!vm.globalEmailForNewsLetter) {
                var promiseGet = mainService.crudService("Api/Client/SubscribeNewsletter", $rootScope.myConstants.HTTPPost, vm.getdata);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        vm.getnewsletterData = success.data;
                        toaster.pop('success', "", success.data);
                        $rootScope.globalEmailForNewsLetter = "";
                        vm.globalEmailForNewsLetter = "";
                        $('#emailNewsLetter').val('');
                    }
                }, function (error) {
                    toaster.pop('error', "", error.data.ErrorMessage);
                    $rootScope.globalEmailForNewsLetter = "";
                    $('#emailNewsLetter').val('');
                    //console.log(error);
                });
            }
        };
        $scope.$on('broadCastgetnewsletter', function () {
            getnewsletter();
        });
        function LoginService(signInObj, pagename) {
            vm.loginData.StoreId = $rootScope.myConstants.StoreId;
            vm.loginData.ApiSecretKey = $rootScope.myConstants.apiKey;
            vm.loginData.CustomerGUID = $rootScope.globalCustomerDetails.CustomerGuid;
            vm.loginData.UserName = signInObj.EmailId;
            vm.loginData.EmailId = signInObj.EmailId;
            vm.loginData.Password = signInObj.Password;

            var promiseGet = mainService.crudService('Api/Client/Login', 'Post', vm.loginData);

            promiseGet.then(function (success) {
                if (success.status == 200) {
                    if (!!signInObj.IsPasswordChanged) {
                        $state.go('setPassword');
                        $('#modal-signin').modal('hide');
                    }
                    if (!!$rootScope.globalCartInformation.CustomerGuid) {
                        vm.update_Cart(pagename);
                    } else {
                        $rootScope.$broadcast('broadCastGetAllUserPurchaseInfo', $rootScope.myConstants.PAGE_NAMES.NOREDIRECT);

                        $rootScope.$emit('EmitLoadProductsAfterLogin');

                        $('#modal-signin').modal('hide');
                    }
                    $scope.cartInfor = '';
                    $localStorage.IsAuthenticated = true;
                    vm.IsUserLogin = $localStorage.IsAuthenticated;
                    $rootScope.IsAuthenticated = $localStorage.IsAuthenticated;
                    $rootScope.nameOfLoggedInUser = !success.data ? null : success.data.UserName;
                    $rootScope.globalOrderSummary = {};
                    $rootScope.globalCartInformation = {};
                    $localStorage.captureEmail = null;
                    $localStorage.captureEmailMobile = null;

                }

            }, function (error) {

            });


        }

        function fnsignUp(ngForm) {
            vm.userData.CustomerGUID = "";
            vm.userData.FirstName = vm.userData.UserName;
            vm.userData.LastName = ".";
            vm.userData.MobileNumber = vm.selectedCountryCode.Code + '-' + vm.userData.PhoneNumber;
            vm.userData.IsGuestUser = false;

                var promiseGet = mainService.crudService('Api/Client/Register', 'Post', vm.userData);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        $rootScope.globalGuestInformation = success.data
                        subscribetoNewsletter(vm.userData.EmailId);

                        toaster.pop('success', "", "Registered Successfully");
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
                                "IsLoyalityMember": true
                            }
                        };

                        //$(document).ready(function () {
                        //    $.ajax({
                        //        url: apiUrl + "api/MemberDetails/PostDetails",
                        //        type: $rootScope.myConstants.HTTPPost,
                        //        headers: {
                        //            "Accept": "application/json; charset=utf-8",
                        //            "Content-Type": "application/x-www-form-urlencoded",
                        //        },
                        //        data: requestObj,
                        //        dataType: "json",
                        //        success: function (response) {
                        //            var authenticateObj = {
                        //                EmailId: vm.userData.EmailId,
                        //                Password: vm.userData.Password
                        //            };
                        //            $("#modal-signup").modal('hide');
                        //            vm.getCustomerGuid(authenticateObj,pagename);
                        //        },
                        //        failure: function (result) { console.log('Error', 'Some problem occurred'); }

                        //    });
                        //});
                        var pagename = $state.current.name;
                        var promisePost = mainService.crudService('api/MemberDetails/PostDetails', 'Post', requestObj, true);
                        promisePost.then(function (success) {
                            var authenticateObj = {
                                EmailId: vm.userData.EmailId,
                                Password: vm.userData.Password
                            };
                            $("#modal-signup").modal('hide');
                            vm.getCustomerGuid(authenticateObj, pagename);
                        });

                    }

                }, function (error) {
                    if (error.data.Password == "The Password field is required.") {
                        toaster.pop('error', "", "Please Click Send Otp Button to generate Otp");
                    } else {
                        toaster.pop('error', "", error.data.ErrorMessage);
                    }
                });
            }

        function fnIsCustomerExists(ngForm, flag) {
            var promiseGet = mainService.crudService("api/Customer/IsUserExists?emailId=" + vm.userData.EmailId + '&mobileNumber=' + vm.userData.PhoneNumber, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    if (flag == 1) {
                        fnGenerateOTP();
                    } else {
                        fnsignUp(ngForm);
                    }
                }
            }, function (error) {
                if (error.status == 409 || error.status == 500) {
                    toaster.pop('error', "", "Email already exists");
                }

                console.log(error);
            });
        }


        $scope.$on('broadCastRegister', function (event, ngForm) {
            vm.signup(ngForm);
        });


        vm.signup = function (ngForm) {
            if (ngForm.$invalid) {
                var tempInputs = angular.element('input.ng-invalid,select.ng-invalid');
                var inputs = $.grep(tempInputs, function (e) { return e.form.id == 'vm.frmSignup' });

                inputs[0].focus();

                return false;
            }
            if (vm.invalidPasswordMatch) {
                toaster.pop('warning', "", "Password and Confirm Password should be same");
                return false;
            }
            if (vm.userData.NewPassword != vm.userData.Password) {
                toaster.pop('warning', "", "Password and Confirm Password should be same");
                return false;
            }
            fnIsCustomerExists(ngForm, 2);

        };

        vm.clkGenerateOTP = function (ngForm) {
            if (ngForm.$invalid) {
                var tempInputs = angular.element('input.ng-invalid,select.ng-invalid');
                var inputs = $.grep(tempInputs, function (e) { return e.form.id == 'vm.frmSignup' });
                inputs[0].focus();

                return false;
                // angular.element('input#telephone.ng-invalid').first().focus();
            }

            fnIsCustomerExists(ngForm, 1);
        };

        function fnGenerateOTP() {
            vm.userData.OTP = "";
            // var promiseGet = mainService.crudService("api/OTP/Send?mobileNumber=" + vm.userData.PhoneNumber, $rootScope.myConstants.HTTPGet, {}, true);
            var promiseGet = mainService.crudService("api/OTP/Send?mobileNumber=" + vm.selectedCountryCode.Code + vm.userData.PhoneNumber, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    if (JSON.parse(success.data).Status == "Success") {
                        vm.IsOTPButtonClicked = true;
                        vm.tempOTPSessionKey = JSON.parse(success.data).Details;
                    }
                    else {
                        toaster.pop("error", "", "Please enter valid Phone Number");

                        vm.IsOTPButtonClicked = false;
                        vm.tempOTPSessionKey = '';
                    }

                }
            }, function (error) {
                console.log(error);
                vm.IsOTPButtonClicked = false;
                vm.tempOTPSessionKey = '';
            });
        }

        vm.ValidateOTP = function fnValidateOTP(ngForm) {
            if (!!vm.userData.PhoneNumber && !!vm.tempOTPSessionKey && !!vm.userData.OTP) {
                if (vm.userData.OTP.toString().length == 6) {
                    var promiseGet = mainService.crudService("api/OTP/Validate?mobileNumber=" + vm.userData.PhoneNumber + "&sessionKey=" + vm.tempOTPSessionKey + "&otp=" + vm.userData.OTP, $rootScope.myConstants.HTTPGet, {}, true);
                    promiseGet.then(function (success) {
                        if (success.status == 200) {
                            vm.userData.OTP = '';
                            if (JSON.parse(success.data).Details == "OTP Expired") {
                                toaster.pop("error", "", "OTP has expired, Please try again");

                                return false;
                            }
                            else if (JSON.parse(success.data).Details == 'OTP Matched') {
                                // fnsignUp(ngForm);
                                vm.IsOTPValidated = true;
                                //return true;
                            }
                            else if (JSON.parse(success.data).Details == 'OTP Mismatch') {
                                toaster.pop("error", "", "Invalid OTP");
                                return false;
                            }
                            else {
                                return false;
                            }
                        }
                    }, function (error) {
                        toaster.pop("error", "", 'Invalid OTP');
                        console.log(error);
                    });
                } else {
                    toaster.pop("error", "", 'Invalid OTP');
                }
            }
            else {
                toaster.pop("error", "", 'OTP is mandatory');
                return false;
            }
        };

        vm.clkSignupButton = function () {
            $('#modal-signin').modal('hide');
            vm.IsOTPButtonClicked = false;
            vm.tempOTPSessionKey = '';
            vm.clkResetAllModals();
        };

        vm.clkResetAllModals = function () {
            vm.userData = {};
            vm.loginData = {};
            vm.fortgotPasswordData = {};
            vm.selectedCountryCode = vm.globalCountryCodes[0];

            angular.extend(vm.userData, {
                UserName: '', EmailId: '', Password: '', PhoneNumber: '', OTP: '',
                OldPassword: '', UserPassword: '', NewPassword: ''
            });

            angular.extend(vm.loginData, {
                Email: '', Password: ''
            });

            angular.extend(vm.fortgotPasswordData, {
                EmailId: ''
            });

            vm.frmSignup.$setPristine();
            vm.frmSignup.$setUntouched();


            vm.frmLogin.$setPristine();
            vm.frmLogin.$setUntouched();


            vm.frmForgotPassword.$setPristine();
            vm.frmForgotPassword.$setUntouched();


            vm.frmChangepwd.$setPristine();
            vm.frmChangepwd.$setUntouched();
            vm.loginData.EmailId = $localStorage.captureEmail;
            vm.userData.EmailId = $localStorage.captureEmail;
        };

        $scope.$on('broadcastPickPlan', function (event, planId, checkOutCount, IsFromFresh) {

            if (IsFromFresh === "") {
                IsFromFresh = false;
            }

            if (checkOutCount === 0) {
                vm.clkPlanPick(planId, checkOutCount, IsFromFresh);
            }
        });

        vm.clkPlanPick = function (planId, checkOutCount, IsFromFresh) {

            if (!!$localStorage.authorizationData) {
                $rootScope.globalPickedPlanId = planId;
                if ($state.current.name != 'profile') {
                    toaster.pop('success', "", "You have choosen membership plan successully");
                    $rootScope.globalIsMember = true;
                    if (planId != 6) {
                        fnApplyDefaultMemberDiscounts(IsFromFresh);
                    }
                    checkOutCount = checkOutCount + 1;
                    //$rootScope.$broadcast('broadcastApplyDefaultMemberDiscounts'); 
                    //$('#checkout-membershipment').modal('hide');
                }
                else {
                    var promiseGet = mainService.crudService("api/MemberPlans/ChangePlan?memberId=" + $localStorage.authorizationData.MemberId + "&planId=" + planId, $rootScope.myConstants.HTTPPost, {}, true);
                    promiseGet.then(function (success) {
                        if (success.status == 200) {
                            $('#checkout-membershipment').modal('hide');
                            isMembershipPlanExists();
                            toaster.pop('success', "", "Plan changed successfully");
                        }
                    }, function (error) {
                        console.log(error);
                    });
                }
            }
            else {
                console.log("Not registered in MemberDetails Module");
            }
        };

        $scope.$on('broadCastPlanActivate', function (event, planId) {
            var requestObj = {
                "MemberId": !!$localStorage.authorizationData ? $localStorage.authorizationData.MemberId : '',
                "PlanId": planId
            };
            var IsLoyalityMember = false;
            var promisePost = mainService.crudService('api/MemberPlans?IsLoyalityMember=' + IsLoyalityMember, 'Post', requestObj, true);
            promisePost.then(function (success) {
                console.log(success);
            }, function (error) {
                console.log(error);
            });
            //$(document).ready(function () {
            //    $.ajax({
            //        url: apiUrl + "api/MemberPlans",
            //        type: $rootScope.myConstants.HTTPPost,
            //        headers: {
            //            "Accept": "application/json; charset=utf-8",
            //            "Content-Type": "application/x-www-form-urlencoded",
            //            "Authorization": 'Bearer ' + authData.token
            //        },
            //        data: requestObj,
            //        dataType: "json",
            //        success: function (response) {
            //            console.log('Success');
            //        },// function called on success
            //        failure: function (result) { console.log('Error', 'Some problem occurred'); }

            //    });
            //});
        });

        function authenticateUser(userData, isFromRegister, pagename) {
            authenticateService.login(userData).then(function (data) {
                if (authenticateService.userData().IsAuthenticated) {

                    var promiseGet = mainService.crudService('api/MemberLogin?id=' + $localStorage.authorizationData.UserId, 'GET', {}, true);
                    promiseGet.then(function (success) {
                        if (success.status == 200) {
                            $rootScope.globalMemberDetails = success.data;
                            $localStorage.CustomerMobileNumber = success.data.MobileNumber;
                            userData.IsPasswordChanged = $rootScope.globalMemberDetails.IsPasswordChanged;
                            LoginService(userData, pagename);
                        }
                    }, function (error) {
                        $rootScope.globalMemberDetails = {};
                    });

                } else {
                    toaster.pop('warning', "", "Invalid credentials");
                }
            }, function (error) {
                toaster.pop('warning', "", "Invalid credentials");
            });
        }

        vm.validatepassword = function () {
            vm.invalidPasswordMatch = false;
            if (!!vm.userData.ConfirmPassword && !!vm.userData.UserPassword) {
                if (vm.userData.ConfirmPassword != vm.userData.UserPassword) {
                    vm.invalidPasswordMatch = true;
                }
            }
        };

        vm.forgotPassword = function (ngForm) {
            if (ngForm.emailForgot.$invalid) {
                angular.element('input#emailForgot.ng-invalid').first().focus();

                return false;
            }

            vm.fortgotPasswordData.LanguageId = $rootScope.myConstants.LanguageId;

            var promiseGet = mainService.crudService('Api/Client/PasswordRecovery', $rootScope.myConstants.HTTPPost, vm.fortgotPasswordData);
            promiseGet.then(function (success) {
                if (success.status === 200) {
                    toaster.pop('success', "", $rootScope.myConstants.ValidationMessages.ForgotPasswordSuccess);
                    window.location.reload();
                }
            }, function (error) {
                console.log(error);
                if (error.data.Status === 404) {
                    toaster.pop('warning', "", $rootScope.myConstants.ValidationMessages.ForgotPasswordMailNotFound);
                }
            });


        };
        vm.backToLogin = function () {
            vm.loginData = {};
            vm.formStage = 1;
            vm.forgotFlow = 0;
        };

        vm.gotoForgotPassword = function () {
            //vm.formStage = 2;
            //vm.forgotFlow = 1;
            vm.fortgotPasswordData = {};

            vm.fortgotPasswordData.UserId = 0;
            vm.fortgotPasswordData.NewPassword = '';
            vm.fortgotPasswordData.UserPhoneNumber = '';
            vm.fortgotPasswordData.EOTP = "";
            vm.fortgotPasswordData.DOTP = "";

            vm.clkResetAllModals();
        };


        vm.changepassword = function (ngForm) {
            if (ngForm.$invalid) {
                angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                return false;
            }
            if (vm.invalidPasswordMatch) {
                toaster.pop('warning', "", "Password and Confirm Password should be same");
                return false;
            }
            if (vm.userData.NewPassword != vm.userData.UserPassword) {
                toaster.pop('warning', "", "Password and Confirm Password should be same");
                return false;
            }

            var requestObj = {
                OldPassword: vm.userData.OldPassword,
                //EmailId: $rootScope.globalUserInformation.Email,
                EmailId: $localStorage.authorizationData.EmailId,
                NewPassword: vm.userData.UserPassword,
                ApiSecretKey: $rootScope.myConstants.apiKey
            };
            var promiseGet = mainService.crudService("Api/Client/ChangePassword", $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    var promisePost = mainService.crudService('api/Customer/ChangePassword?emailId=' + requestObj.EmailId + '&NewPassword=' + requestObj.NewPassword, $rootScope.myConstants.HTTPPost, {}, true);
                    promisePost.then(function (success) {
                        if (success.status == 200) {
                            toaster.pop('success', "", "Your password has been changed successfully. Please Re-Login");
                            $('#modal-changePassword').modal('hide');
                            $rootScope.$broadcast('broadCastLogout');
                        }
                    }, function (error) {
                        console.log(error);
                    });
                }

            }, function (error) {
                toaster.pop('error', "", error.data.ErrorMessage);
            });
        };

        vm.validatepassword = function () {
            vm.invalidPasswordMatch = false;
            if (!!vm.userData.ConfirmPassword && !!vm.userData.UserPassword) {
                if (vm.userData.ConfirmPassword != vm.userData.UserPassword) {
                    vm.invalidPasswordMatch = true;
                }
            }
        };


        vm.signout = function () {
            if ($localStorage.IsPOSUser) {
                $state.go('POSLogin');
                authenticateService.logout();
                $rootScope.$broadcast('userauthentication');
                $rootScope.globalCartInformation = {};
                $rootScope.globalCartInformation_Fresh = {};
                $rootScope.globalTotalShippingAmount = 0;
                $rootScope.globalDeliveryCharges = 0;
                $rootScope.globalDeliveryChargeMinLimit = 0;

                $rootScope.globalCheckoutInformation = [];
                $rootScope.globalCheckoutTotalShippingAmount = 0;
                $rootScope.globalCustomerDetails = {};
                $rootScope.IsAuthenticated = false;
                $rootScope.RedeemedAmount = 0;
                $rootScope.globalAppliedDiscountCoupons = [];
                $rootScope.globalAllAppliedDiscountCoupons = [];
                $rootScope.globalMemberdiscounts = [];
                $localStorage.CustomerGuid = undefined;
                $rootScope.globalMemberDetails = [];
                $rootScope.globalIsMember = false;
                $localStorage.IsPOSUser = false;
                localStorage.setItem("ngStorage-IsPOSUser", false);
                //$localStorage.IsPOSUser=false;  
            } else {
                authenticateService.logout();


                $rootScope.$broadcast('userauthentication');

                $rootScope.globalCartInformation = {};
                $rootScope.globalCartInformation_Fresh = {};
                $rootScope.globalTotalShippingAmount = 0;
                $rootScope.globalDeliveryCharges = 0;
                $rootScope.globalDeliveryChargeMinLimit = 0;

                $rootScope.globalCheckoutInformation = [];
                $rootScope.globalCheckoutTotalShippingAmount = 0;
                $rootScope.globalCustomerDetails = {};
                $rootScope.IsAuthenticated = false;
                $rootScope.RedeemedAmount = 0;
                $rootScope.globalAppliedDiscountCoupons = [];
                $rootScope.globalAllAppliedDiscountCoupons = [];
                $rootScope.globalMemberdiscounts = [];
                $localStorage.CustomerGuid = undefined;
                $rootScope.globalMemberDetails = [];
                $rootScope.globalIsMember = false;

                if ($state.current.name != "home") {
                    $state.go('gourmetproducts');
                    //  window.location.reload();
                }
            }


        };

        $scope.$on('broadCastLogout', function (event) {
            vm.signout();
        });

        function fnAddToCartBroadcast(productData, quantity) {
            if (!quantity) {
                alert("Quantity must be greater than zero");
                return false;
            }

            var parentCategoryId = 0;

            if ($state.current.name == 'products' || $state.current.name == 'productDetails' || $state.current.name == 'wishlist' || $state.current.name == 'product') {
                parentCategoryId = $rootScope.globalCategories.Grocer;
            }

            var cartInfo = {};
            var requestObj = {
                "ProductId": productData.ProductId,
                "ShoppingCartTypeId": productData.ShoppingCartTypeId,
                "AttributeControlIds": productData.AttributeControlIds,
                "Quantity": quantity,
                "ParentCategoryId": productData.parentCategoryId,
            };
            if (!!$localStorage.IsAuthenticated) {
                var promiseGet = mainService.crudService("Api/Client/DetailAddProductToCart", $rootScope.myConstants.HTTPPost, requestObj);
                promiseGet.then(function (success) {
                    if (success.status === 200) {
                        if (!!success.data.Messages) {
                            toaster.pop('warning', "", success.data.Messages);
                        } else {
                            $rootScope.globalOrderSummary = {};
                            if (productData.ShoppingCartTypeId == 1)
                                toaster.pop('success', "", "Product has been added to Cart successfully");
                            else if (productData.ShoppingCartTypeId == 2)
                                toaster.pop('success', "", "Product has been added to Wishlist successfully");
                            getAllUserPurchaseInfoIdAndUserId($rootScope.myConstants.PAGE_NAMES.NOREDIRECT);
                            return cartInfo;
                        }
                    }
                }, function (error) {
                    console.log(error);
                });
            }
            else {
                // toaster.pop('error', "", "Please LogIn");

                if (productData.ShoppingCartTypeId == 2) {
                    toaster.pop('error', "", "Please LogIn");
                } else if (productData.ShoppingCartTypeId == 1) {
                    if ($rootScope.globalCustomerDetails.CustomerGuid === undefined) {
                        $rootScope.globalCustomerDetails.CustomerGuid = "00000000-0000-0000-0000-000000000000";
                    }
                    var promiseGet1 = mainService.crudService("Api/Client/DetailAddProductToCart", $rootScope.myConstants.HTTPPost, requestObj);
                    promiseGet1.then(function (success) {
                        if (success.status == 200) {
                            $rootScope.globalCustomerDetails.CustomerGuid = success.data.CustomerGuid;
                            $rootScope.globalOrderSummary = {};
                            if (productData.ShoppingCartTypeId == 1)
                                toaster.pop('success', "", "Product has been added to Cart successfully");
                            else if (productData.ShoppingCartTypeId == 2)
                                toaster.pop('success', "", "Product has been added to Wishlist successfully");
                            getAllUserPurchaseInfoIdAndUserId($rootScope.myConstants.PAGE_NAMES.NOREDIRECT);
                            return cartInfo;
                        }
                    }, function (error) {
                        console.log(error);
                    });
                }
            }
        }


        $scope.$on('addtocartbroadcast', function (event, productData, quantity, redirectURL) {

            //if (productData.ShoppingCartTypeId.toString() === "1") {
            //    if ($rootScope.globalCartInformation_Grocer.length > 0 && productData.parentCategoryId === 50) {
            //        if ($window.confirm("Grocery iteams already there in cart, remove them?")) {
            //            for (var i = 0; i < $rootScope.globalCartInformation_Grocer.length; i++) {
            //                $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Grocer[i].Id, $rootScope.myConstants.PAGE_NAMES.CART);
            //            }
            //            $scope.Message = "You clicked YES.";
            //        } else {
            //            $scope.Message = "You clicked NO.";
            //            return false;
            //        }

            //        //alert('Grocery iteams already there in cart');
            //        //return;
            //    }
            //    if ($rootScope.globalCartInformation_Gourmet.length > 0 && productData.parentCategoryId === 49) {
            //        if ($window.confirm("Gourment iteams already there in cart, remove them?")) {
            //            for (var j = 0; j < $rootScope.globalCartInformation_Gourmet.length; j++) {
            //                $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Gourmet[j].Id, $rootScope.myConstants.PAGE_NAMES.CART);
            //            }
            //            $scope.Message = "You clicked YES.";
            //        } else {
            //            $scope.Message = "You clicked NO.";
            //            return false;
            //        }
            //        //alert('Gourment iteams already there in cart');
            //        //return;
            //    }
            //}

            if (!!$rootScope.globalCategories) {
                fnAddToCartBroadcast(productData, quantity, redirectURL);
            }
            else {
                $rootScope.$broadcast('broadcastGetParentCategories');

                $rootScope.$on('emitGetParentCategories', function () {
                    fnAddToCartBroadcast(productData, quantity, redirectURL);
                });
            }


        });
        $scope.$on('broadCastApplyGrocerDiscountCoupon', function (event, discountCode, IsFromFresh,isfromcheckout) {

            if (IsFromFresh === "") {
                IsFromFresh = false;
            }
            if (isfromcheckout === "") {
                isfromcheckout = false;
            }
            if(discountCode==null)
            {
                FnGetOrderSummaryFinal(null,IsFromFresh,null,isfromcheckout);
            }
            if(!!discountCode)
            {
                FnApplyGrocerDiscountCoupon(discountCode, IsFromFresh,isfromcheckout);
            }
        });

        function FnApplyGrocerDiscountCoupon(discountCode, IsFromFresh,isfromcheckout) {

            var requestObj = {
                "DiscountCouponCode": discountCode,
                "OrderTotal": $rootScope.globalOrderSummary.SubTotalWithDiscount
            };

            var promiseGet = mainService.crudService('Api/Client/ApplyDiscount', $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    //toaster.pop('success', "", success.data.Message);
                    if (!!IsFromFresh) {
                        FnGetFreshOrderSummaryFinal(success.data.Message);
                    }
                    else if (!IsFromFresh) {
                        FnGetOrderSummaryFinal(discountCode, IsFromFresh, success.data.Message,isfromcheckout);
                    }
                    if (!!$localStorage.authorizationData) {
                        fnGetAppliedDiscountCoupons();
                    }
                    else {
                        fnGetGuestDiscountCoupons();
                    }

                    getTotal();
                }
            }, function (error) {
                toaster.pop('error', "", error.data.ErrorMessage);
            });
        }

        $scope.$on('broadCastGetOrderSummary', function (event) {
            FnGetOrderSummary();
        });

        // $scope.$on('broadCastGetOrderSummaryForTotalTax', function (event,isfromcheckoutpage) {
        //     FnGetOrderSummaryFinal('', '', '');
        // });

        function FnGetOrderSummaryFinal(discountCode, IsFromFresh, successMsg,isfromcheckout) {
            if(isfromcheckout=='')
            {
                isfromcheckout=false;
            }
            $rootScope.globalOrderSummary = {};
            var requestObj = {

            };

            var promiseGet = mainService.crudService('Api/Client/OrderSummary', $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalOrderSummary = success.data;
                    console.log(success.data);
                    if (!!$rootScope.globalOrderSummary.SubTotal) {
                        $rootScope.globalOrderSummary.SubTotalWithDiscount = (
                            parseFloat($rootScope.globalOrderSummary.SubTotal.split('$')[1]) -
                            (!!$rootScope.globalOrderSummary.SubTotalDiscount ?
                                (parseFloat($rootScope.globalOrderSummary.SubTotalDiscount)) : 0));

                        //   $rootScope.globalOrderSummary.SubTotalWithDiscount = parseFloat($rootScope.globalOrderSummary.SubTotal.split('$')[1]);
                    }

                    $rootScope.globalOrderSummary.SubTotal=parseFloat($rootScope.globalOrderSummary.SubTotal.split('$')[1]);
                      
                    $rootScope.globalOrderSummary.Tax = (
                        (!!$rootScope.globalOrderSummary.Tax ?
                            (parseFloat($rootScope.globalOrderSummary.Tax.split('$')[1])) :
                            0
                        )
                    );

                    if(!!isfromcheckout)
                    {
                        vm.isfromcheckout=isfromcheckout;
                    }
                    else
                    {
                        vm.isfromcheckout=isfromcheckout;
                    }

                    if(!!vm.isfromcheckout)
                    {
                        var temptax=($rootScope.globalOrderSummary.SubTotalWithDiscount+
                            $rootScope.globalPaidShippingCharges)* ((parseInt($rootScope.globalOrderSummary.TaxRates[0].Rate))*0.01);
                            $rootScope.globalOrderSummary.Tax=temptax;
                    }
                     else
                     {
                        var temptax=($rootScope.globalOrderSummary.SubTotalWithDiscount+
                            $rootScope.globalShippingChargesForPickup)* ((parseInt($rootScope.globalOrderSummary.TaxRates[0].Rate))*0.01);
                            $rootScope.globalOrderSummary.Tax=temptax;
                     }
                    if ($rootScope.globalOrderSummary.Items.length > 0) {
                        $rootScope.globalOrderSavings =
                            $.sum($.map($rootScope.globalOrderSummary.Items, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));


                        $rootScope.globalOrderSummary.SubTotalWithoutDiscount =
                            ($rootScope.globalOrderSummary.SubTotalWithDiscount +
                                (!!$rootScope.globalOrderSavings ? $rootScope.globalOrderSavings : 0)
                            );

                    }

                    // if (($rootScope.globalOrderSummary.SubTotalWithDiscount - $rootScope.RedeemedAmount) < 2) {
                    //     fnRemoveDiscountCoupon(discountCode, IsFromFresh,isfromcheckout);
                    //     toaster.pop("error", '', "Sub-Total amount should be greater than SG$2");
                    //     return false;
                    // }
                   // else
                     if (!!successMsg) {
                        //fnRemoveDiscountCoupon(discountCode, IsFromFresh,isfromcheckout);
                        if (successMsg.length > 0)
                        toaster.pop('success', "", successMsg);
                    }
                }
            }, function (error) {
                console.log(error);
            });
        }

        function FnGetOrderSummary() {

            if (!!$rootScope.globalOrderSummary) {
                if (Object.keys($rootScope.globalOrderSummary).length == 0) {
                    FnGetOrderSummaryFinal('', '', '','');
                }
            }
            else {
                FnGetOrderSummaryFinal('', '', '','');
            }

        }

        $scope.$on('broadCastGetFreshOrderSummary', function (event) {
            FnGetFreshOrderSummary();
        });

        function FnGetFreshOrderSummary() {

            if (!!$rootScope.globalFreshOrderSummary) {
                if (Object.keys($rootScope.globalFreshOrderSummary).length == 0) {
                    FnGetFreshOrderSummaryFinal('');
                }
            }
            else {
                FnGetFreshOrderSummaryFinal('');
            }

        }

        function FnGetFreshOrderSummaryFinal(successMsg) {
            $rootScope.globalOrderSummary = {};
            var requestObj = {

            };

            var promiseGet = mainService.crudService('Api/Client/OrderSummaryFresh', $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalFreshOrderSummary = success.data;
                    if (!!$rootScope.globalFreshOrderSummary.SubTotal) {
                        $rootScope.globalFreshOrderSummary.SubTotalWithDiscount = (
                            parseFloat($rootScope.globalFreshOrderSummary.SubTotal.split('$')[1]) -
                            (!!$rootScope.globalFreshOrderSummary.SubTotalDiscount ?
                                (parseFloat($rootScope.globalFreshOrderSummary.SubTotalDiscount.split('$')[1].split(')')[0])) :
                                0
                            )
                        );

                        //   $rootScope.globalOrderSummary.SubTotalWithDiscount = parseFloat($rootScope.globalOrderSummary.SubTotal.split('$')[1]);
                    }


                    if ($rootScope.globalFreshOrderSummary.mealPlanModels.length > 0) {
                        $rootScope.globalFreshOrderSavings = 0;
                        for (var i = 0; i < $rootScope.globalFreshOrderSummary.mealPlanModels.length; i++) {
                            $rootScope.globalFreshOrderSavings = $rootScope.globalFreshOrderSavings +
                                $.sum($.map($rootScope.globalFreshOrderSummary.mealPlanModels[i].Items, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));
                        }

                        $rootScope.globalFreshOrderSummary.SubTotalWithoutDiscount =
                            ($rootScope.globalFreshOrderSummary.SubTotalWithDiscount +
                                (!!$rootScope.globalFreshOrderSavings ? $rootScope.globalFreshOrderSavings : 0)
                            );

                    }

                    if (($rootScope.globalFreshOrderSummary.SubTotalWithDiscount - $rootScope.RedeemedAmount) < 2) {
                        fnRemoveDiscountCoupon(discountCode, true,false);
                        toaster.pop("error", '', "Sub-Total amount should be greater than SG$2");
                        return false;
                    }
                    else if (successMsg.length > 0) {
                        toaster.pop('success', "", successMsg);
                    }
                }
            }, function (error) {
                console.log(error);
            });
        }

        $.sum = function (arr) {
            var r = 0;
            $.each(arr, function (i, v) {
                r += +v;
            });
            return r;
        };


        vm.clk_RemoveFromCart = function (purchaseId, pageName, IsFromRemoveFunction, eventValue) {

            if (IsFromRemoveFunction === "") {
                IsFromRemoveFunction = false;
            }

            if (eventValue === "") {
                eventValue = false;
            }

            var requestObj = {
                "ItemIds": purchaseId
            };
            var promiseGet = mainService.crudService("Api/Client/RemoveFromCart", $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    getAllUserPurchaseInfoIdAndUserId(pageName);
                    if (!!IsFromRemoveFunction) {
                        fnRemoveCartItems(eventValue);
                    }
                }
            }, function (error) {
                console.log(error);
            });

        };

        $scope.$on('removefromcartbroadcast', function (event, purchaseId, pageName) {

            vm.clk_RemoveFromCart(purchaseId, pageName);

        });
        $scope.$on('cartcount', function (event, pageName) {
            vm.gourmetAndGrocery = false;
            vm.freshItems = true;
        });

        function getAllUserPurchaseInfoIdAndUserId(pageName) {
            var requestobj = {
            };
            var promiseGet = mainService.crudService('Api/Client/OnlineCart', $rootScope.myConstants.HTTPPost, requestobj);
            promiseGet.then(function (success) {
                if (success.data.Items[0].ParentCategoryId == $rootScope.globalCategories.Fresh) {
                    var requestObj = {};
                    var promiseGet1 = mainService.crudService("Api/Client/FreshCart", $rootScope.myConstants.HTTPPost, requestObj);
                    promiseGet1.then(function (success) {
                        $rootScope.globalCartInformation = success.data;
                        tempFunFresh(pageName);
                        vm.gourmetAndGrocery = false;
                        vm.freshItems = true;
                        if (pageName == 'freshCart' || pageName == $rootScope.myConstants.PAGE_NAMES.CART)
                            $state.go("freshCart");
                    });

                } else {
                    $rootScope.globalCartInformation = success.data;
                    if ($rootScope.globalCartInformation.Items[0].ProductId == 1236) {
                        $rootScope.PromotionsFlag = true;
                    }
                    else {
                        $rootScope.PromotionsFlag = false;
                    }
                    tempFunOld(pageName);
                    vm.gourmetAndGrocery = true;
                    vm.freshItems = false;
                }
            }, function (error) {
                if (error.status = 400) {
                    $rootScope.globalCartInformation = {};
                    $rootScope.globalCartInformation_Grocer = [];
                    $rootScope.globalCartInformation_Gourmet = [];
                    $rootScope.globalCartInformation_Fresh = [];

                    if (pageName == $rootScope.myConstants.PAGE_NAMES.CHECKOUT) {
                        $state.go("gourmetproducts");
                    } else if (pageName == $rootScope.myConstants.PAGE_NAMES.POSCHECKOUT) {
                        $state.go("gourmetproducts");
                    }

                    // if (pageName != $rootScope.myConstants.PAGE_NAMES.NOREDIRECT) {
                    // //toaster.pop('info', "", "Your Cart is empty, Please add Products!");
                    // // $scope.$emit('emitRedirectToProducts');

                    // if (!!$rootScope.globalPreviousState) {
                    // if ($rootScope.globalPreviousState == "checkout" || $rootScope.globalPreviousState == "cart") {
                    // //$state.go("home");
                    // }
                    // else {
                    // //$state.go($rootScope.globalPreviousState);
                    // }
                    // }
                    // else {
                    // //$state.go("home");
                    // }
                    // }
                }
            });

        }
        function tempFunFresh(pageName) {
            $rootScope.globalCartInformation_Grocer = [];
            $rootScope.globalCartInformation_Gourmet = [];
            $rootScope.globalCartInformation_Fresh = [];
            if (!!$rootScope.globalCategories) {
                $rootScope.globalCartInformation_Fresh = $rootScope.globalCartInformation.mealPlanModels;
                $rootScope.globalCartInformation_Fresh_SubTotal = 0;
                $rootScope.globalCartInformation_Fresh_Savings = 0;
                for (var j = 0; j < $rootScope.globalCartInformation_Fresh.length; j++) {

                    $rootScope.globalCartInformation_Fresh_SubTotal = $rootScope.globalCartInformation_Fresh_SubTotal + $.sum($.map($rootScope.globalCartInformation_Fresh[j].Items, function (e) { return (e.SubTotal).split('$')[1] }));

                    $rootScope.globalCartInformation_Fresh_Savings = $rootScope.globalCartInformation_Fresh_Savings + $.sum($.map($rootScope.globalCartInformation_Fresh[j].Items, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));
                }

                $rootScope.globalCartInformation_Fresh_SubTotalWithoutDiscount = ($rootScope.globalCartInformation_Fresh_SubTotal + $rootScope.globalCartInformation_Fresh_Savings);
            }

            for (var i = 0; i < $rootScope.globalCartInformation.mealPlanModels.length; i++) {
                var tempCartInformation = $rootScope.globalCartInformation.mealPlanModels[i];
                $rootScope.globalCartInformation.mealPlanModels[i].MealPlanPrice = $.sum($.map(tempCartInformation.Items, function (e) { return (e.UnitPrice).split('$')[1] }));
                for (var k = 0; k < tempCartInformation.Items.length; k++) {
                    // var text = tempCartInformation.Items[k].AttributeInfo.replace("<br />", " ,");
                    // text = text.replace("<br />", " ,");
                    // text = text.replace("BASE:", "");
                    // text = text.replace("PROTEIN:", "");
                    // text = text.replace("&#233;", "é");
                    // vm.mealText = text.replace("GREENS:", "");
                    var text = tempCartInformation.Items[k].AttributeInfo.split("<br />").join(" ");
                    //text = text.replace("<br />", " ,");
                    //text = text.replace("BASE:", "");
                    //text = text.replace("PROTEIN:", "");
                    //text = text.replace("&#233;", "é");
                    //vm.mealText = text.replace("GREENS:", "");
                    var mealText = text;
                    vm.mealDate = tempCartInformation.Items[k].MealDate.replace("T00:00:00", "");
                    vm.TotalPrice = tempCartInformation.Items[k].UnitPrice;
                    //$rootScope.globalPerPlanPrice = $.sum($.map(tempCartInformation.Items, function (e) { return (e.UnitPrice).split('$')[1] }));
                    tempCartInformation.Items[k].AttributeInfo = mealText;
                    tempCartInformation.Items[k].MealDate = vm.mealDate;
                }

            }
            $rootScope.globalFreshOrderSummary = {};
            $rootScope.$broadcast('getTotalOrderAmount');
            FnGetFreshOrderSummary();
            $rootScope.$broadcast('broadCastShippingMethods');
            // $localStorage.globalCartInformation_Fresh_length = $rootScope.globalCartInformation_Fresh.length;


            $scope.$emit('emitCheckoutfunctionality');
            if (pageName == $rootScope.myConstants.PAGE_NAMES.FRESHSHIPPING) {
                $scope.$emit('emitSavingsoneachorder');
            }

        }
        function tempFunOld(pageName) {
            $rootScope.globalCartInformation_Grocer = [];
            $rootScope.globalCartInformation_Gourmet = [];
            $rootScope.globalCartInformation_Fresh = [];
            if (!!$rootScope.globalCategories) {
                $rootScope.globalCartInformation_Grocer =
                    $.grep($rootScope.globalCartInformation.Items, function (e) { return e.ParentCategoryId == $rootScope.globalCategories.Grocer });


                $rootScope.globalCartInformation_Grocer_SubTotal = $.sum($.map($rootScope.globalCartInformation_Grocer, function (e) { return (e.SubTotal).split('$')[1] }));

                $rootScope.globalCartInformation_Grocer_Savings = $.sum($.map($rootScope.globalCartInformation_Grocer, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));

                $rootScope.globalCartInformation_Grocer_SubTotalWithoutDiscount = ($rootScope.globalCartInformation_Grocer_SubTotal + $rootScope.globalCartInformation_Grocer_Savings);

                $rootScope.globalCartInformation_Gourmet =
                    $.grep($rootScope.globalCartInformation.Items, function (e) { return e.ParentCategoryId == $rootScope.globalCategories.Gourmet });

                $rootScope.globalCartInformation_Gourmet_SubTotal = $.sum($.map($rootScope.globalCartInformation_Gourmet, function (e) { return (e.SubTotal).split('$')[1] }));

                $rootScope.globalCartInformation_Gourmet_Savings = $.sum($.map($rootScope.globalCartInformation_Gourmet, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));


                $rootScope.globalCartInformation_Gourmet_SubTotalWithoutDiscount = ($rootScope.globalCartInformation_Gourmet_SubTotal + $rootScope.globalCartInformation_Gourmet_Savings);

            }

            $rootScope.globalOrderSummary = {};
            $rootScope.$broadcast('getTotalOrderAmount');
            $rootScope.$broadcast('broadCastGetOrderSummary');
            $rootScope.$broadcast('broadCastShippingMethods');

            // $localStorage.globalCartInformation_Grocer_length = $rootScope.globalCartInformation_Grocer.length;
            // $localStorage.globalCartInformation_Gourmet_length = $rootScope.globalCartInformation_Gourmet.length;

            $scope.$emit('emitCheckoutfunctionality');
            if (pageName == $rootScope.myConstants.PAGE_NAMES.CHECKOUT) {
                $scope.$emit('emitSavingsoneachorder');
            }
        }

        $scope.$on('broadCastGetAllUserPurchaseInfo', function (event, pageName) {
            getAllUserPurchaseInfoIdAndUserId(pageName);
        });

        function getTotal() {
            vm.oderData = {};
            vm.oderData.CurrencyId = 1;
            vm.oderData.IsEditable = "true";
            var promiseGet = mainService.crudService("Api/Client/OrderTotal", $rootScope.myConstants.HTTPPost, vm.oderData);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalCheckoutInformation = success.data;
                    if (!!success.data.SubTotal) {
                        $rootScope.globalCheckoutTotalShippingAmount = parseFloat(success.data.SubTotal.split('$')[1]) - (!!success.data.SubTotalDiscount ? parseFloat(success.data.SubTotalDiscount.replace('($', '').split(')')[0]) : 0);
                    }
                }
            });
        }

        $scope.$on('getTotalOrderAmount', function () {
            getTotal();
        });


        $scope.$on('broadCastAddItemToCheckout', function () {
            if (!!$localStorage.IsPOSUser) {
                $state.go('POSCheckout');
            } else if (!$localStorage.IsPOSUser) {
                $state.go('commonCheckout');
            }
        });

        vm.clk_Checkout = function () {
            $rootScope.$broadcast('broadCastAddItemToCheckout');
        };

        $scope.$on('broadCastIsMembershipPlanExists', function (event, IsFromFresh) {

            if (IsFromFresh === "") {
                IsFromFresh = false;
            }

            if (!!$rootScope.globalMemberPlanDetails) {
                if (Object.keys($rootScope.globalMemberPlanDetails).length == 0 || $state.current.name == "profile" || IsFromFresh) {
                    isMembershipPlanExists(IsFromFresh);
                }
            }
            else {
                isMembershipPlanExists(IsFromFresh);
            }
        });

        function isMembershipPlanExists(IsFromFresh) {
            if ($localStorage.authorizationData !== undefined) {
                var promiseGet = mainService.crudService('api/MemberPlans?memberId=' + $localStorage.authorizationData.MemberId, 'GET', {}, true);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        if (success.data.KiposMemberPlan.Kipos_M_Plans.PlanName == "Loyalty Member") {
                            $rootScope.globalIsMember = false;
                        }
                        else {
                            $rootScope.globalIsMember = true;
                        }
                        $rootScope.globalMemberPlanDetails = success.data;
                        $rootScope.globalKPointsToDollarAmount = 0;
                        if (!!$rootScope.globalMemberPlanDetails.KiposMemberPlan && !!$rootScope.globalMemberPlanDetails.ConversionDetails) {
                            $rootScope.globalKPointsToDollarAmount = parseFloat($rootScope.globalMemberPlanDetails.KiposMemberPlan.TotalRewardPoints / $rootScope.globalMemberPlanDetails.ConversionDetails.KPoints);
                        }
                        if (success.data.KiposMemberPlan.Kipos_M_Plans.PlanName != "Loyalty Member") {
                            fnApplyDefaultMemberDiscounts(IsFromFresh);
                        }
                    }
                }, function (error) {
                    if (error.status == 404) {
                        if ($state.current.name != 'profile') {
                            $rootScope.globalIsMember = false;
                        }
                    }
                    else {
                        console.log(error);
                    }
                });
            }

        }
        

        $rootScope.$on('broadcastApplyDefaultMemberDiscounts', function () {
            fnApplyDefaultMemberDiscounts();
        });

        function fnApplyDefaultMemberDiscounts(IsFromFresh) {
            var ArrayofDiscounts = [{
                "DiscountCouponCode": 'DISCOUNT_GOURMET',
                "OrderTotal": 0
            }, {
                "DiscountCouponCode": 'DISCOUNT_GROCER',
                "OrderTotal": 0
            },
            {
                "DiscountCouponCode": 'DISCOUNT_FRESH',
                "OrderTotal": 0
            }
            ];

            var requestObj = {
                'DiscountsInfo': ArrayofDiscounts
            };
            var ApplyDiscountsRequest = mainService.crudService('api/client/ApplyMultipleDiscounts', $rootScope.myConstants.HTTPPost, requestObj);
            ApplyDiscountsRequest.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalIsMember = true;
                    if (!IsFromFresh) {
                        FnGetOrderSummaryFinal('', '', '','');
                        if ($state.current.name == 'commonCheckout') {
                            getAllUserPurchaseInfoIdAndUserId($rootScope.myConstants.PAGE_NAMES.CHECKOUT);
                        }
                    } else if (!!IsFromFresh) {
                        FnGetFreshOrderSummaryFinal('');
                        getAllUserPurchaseInfoIdAndUserId($rootScope.myConstants.PAGE_NAMES.FRESHSHIPPING);
                    }
                    getTotal();
                }
            }, function (error) {

            });
        }

        $scope.$on('broadCastShippingMethods', function (event) {
            shippingMethods();
        });


        function shippingMethods() {
            var requestObject = {
                "currencyId": "1"
            };
            var promiseGet = mainService.crudService('Api/Client/EstimateShipping', $rootScope.myConstants.HTTPPost, requestObject);
            promiseGet.then(function (success) {
                $rootScope.globalShippingOptions = success.data.ShippingOptions;
                $rootScope.globalPaidShippingCharges = parseFloat($.grep($rootScope.globalShippingOptions, function (e) { return e.Name == 'Paid' })[0].Price.split('$')[1]);
                $rootScope.globalShippingChargesForPickup = parseFloat($.grep($rootScope.globalShippingOptions, function (e) { return e.Name == 'Free' })[0].Price.split('$')[1]);
            });
        }

        /*
        function fnDiscountsByModule(moduleName) {
            var promiseGet = mainService.crudService('api/Configuration/GetDiscounts?ConfigKey=' + moduleName, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        $rootScope.globalDiscountPercentage = parseFloat(success.data);

                    };
                },
                function (error) { });
        };

        $rootScope.$on('broadCastDiscountsByModule', function (event, moduleName) {
            if (!$rootScope.globalDiscountPercentage) {
                fnDiscountsByModule(moduleName);
            }
        });
        */


        function fnGetAllDiscounts() {
            var promiseGet = mainService.crudService('api/NOP/Discount/GetAllDiscounts', $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        $rootScope.globalDiscountPercentages = success.data;
                    }
                },
                function (error) { });
        }

        $rootScope.$on('broadCastGetAllDiscounts', function () {
            if (!$rootScope.globalDiscountPercentages) {
                fnGetAllDiscounts();
            }

        });


        function fnGetAppliedDiscountCoupons() {
            var promiseGet = mainService.crudService('api/NOP/Discount/GetApplied', $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        var tempdiscounts = [];
                        var tempmemberdiscounts = [];
                        $rootScope.globalAllAppliedDiscountCoupons = success.data;
                        // updated by Phani for corp_domain couponCode on 30-05-2020 start
                        // updated by sree for couponCode issue for guest on 01-06-2020 start
                        if (!!$rootScope.globalCustomerDetails.DomainCouponCode) {
                            var strDomainCCode = $rootScope.globalCustomerDetails.DomainCouponCode.toLowerCase();
                            if (strDomainCCode === undefined) {
                                tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' });
                            } else {
                                // updated by Phani & surakshith on 17-06-2020 for  corp_domain couponCode start
                                if ($rootScope.globalCustomerDetails.IsDomainCouponCode == true) {
                                    tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' });
                                }
                                else  if ($rootScope.globalCustomerDetails.IsDomainCouponCode == false) {
                                tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' && e != strDomainCCode });}
                                for (var i = 0; i < tempdiscounts.length; i++) {
                                    if (tempdiscounts[i] === strDomainCCode) {
                                        tempdiscounts[i] = 'Corporate discount'; 
                                    }
                                }
                                // updated by Phani & surakshith on 17-06-2020 for  corp_domain couponCode end
                            }
                        }
                        else {
                            tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' });
                        }
                        
                        //if (strDomainCCode === undefined) {
                        //    tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' });// && e != strDomainCCode });
                        //} else {
                        //    tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' && e != strDomainCCode });
                        //}
                        //updated by sree for couponCode issue for guest on 01-06-2020 end
                        // updated by Phani for corp_domain couponCode on 30-05-2020 end
                        
                        $rootScope.globalAppliedDiscountCoupons = tempdiscounts;
                        tempmemberdiscounts = $.grep(success.data, function (e) { return e == 'discount_gourmet' || e == 'discount_grocer' || e == 'discount_fresh' });
                        $rootScope.globalMemberdiscounts = tempmemberdiscounts;
                         // Commented by Phani & surakshith on 17-06-2020 for  corp_domain couponCode start
                        //Added by sree for applying corp discounts when no discounts applied in the checkout page 16_06_2020
                        // if($rootScope.globalAppliedDiscountCoupons.length==0){
                        //     $rootScope.$emit('ApplyDomainDiscountsForCheckout');
                        // }
                        //Added by sree for applying corp discounts when no discounts applied in the checkout page 16_06_2020
                        // Commented by Phani & surakshith on 17-06-2020 for  corp_domain couponCode end
                    }
                },
                function (error) {
                    console.log(error);
                });
        }
        function fnGetGuestDiscountCoupons() {
            // console.log($rootScope.globalGuestInformation.CustomerId);
            if (!!$rootScope.globalGuestInformation) {
                var promiseGet = mainService.crudService('api/NOP/Discount/GetAppliedByList?id=' + $rootScope.globalGuestInformation.CustomerId, $rootScope.myConstants.HTTPGet, {}, true);
                promiseGet.then(
                    function (success) {
                        if (success.status == 200) {
                            var tempdiscounts = [];
                            var tempmemberdiscounts = [];
                            $rootScope.globalAllAppliedDiscountCoupons = success.data;
                            // updated by Phani for corp_domain couponCode on 30-05-2020 start
                            //updated by sree 01-06-2020 for display of couponcode guest start
                            if (!!$rootScope.globalCustomerDetails.DomainCouponCode) {
                                var strDomainCCode = $rootScope.globalCustomerDetails.DomainCouponCode.toLowerCase();
                                if (strDomainCCode != undefined) {
                                    tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' && e != strDomainCCode });
                                }
                            }
                            else{
                                tempdiscounts = $.grep(success.data, function (e) { return e != 'discount_gourmet' && e != 'discount_grocer' && e != 'discount_fresh' });
                            }
                            //updated by sree 01-06-2020 for display of couponcode guest end
                            // updated by Phani for corp_domain couponCode on 30-05-2020 end
                            $rootScope.globalAppliedDiscountCoupons = tempdiscounts;
                            tempmemberdiscounts = $.grep(success.data, function (e) { return e == 'discount_gourmet' || e == 'discount_grocer' || e == 'discount_fresh' });
                            $rootScope.globalMemberdiscounts = tempmemberdiscounts;
                        }
                    },
                    function (error) {
                        console.log(error);
                    });
            }

        }

        $rootScope.$on('broadCastGetAppliedDiscountCoupons', function (event) {
            if (!!$localStorage.authorizationData) {
                fnGetAppliedDiscountCoupons();
            }
            else {
                fnGetGuestDiscountCoupons();
            }
        });


        function fnRemoveDiscountCoupon(couponcode, IsFromFresh,isfromcheckout) {
            var requestObj = {
                "DiscountCouponCode": couponcode
            };
            var promiseGet = mainService.crudService('Api/Client/RemoveDiscount', $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        fnGetAppliedDiscountCoupons();
                        $rootScope.globalOrderSummary = {};
                        if (!!IsFromFresh) {
                            FnGetFreshOrderSummaryFinal('');
                        } else if (!IsFromFresh) {
                            FnGetOrderSummaryFinal('', '', '',isfromcheckout);
                        }
                        getTotal();
                    }
                },
                function (error) { });

        }

        $rootScope.$on('broadCastRemoveDiscountCoupon', function (event, couponcode, IsFromFresh,isfromcheckout) {

            if (IsFromFresh === "") {
                IsFromFresh = false;
            }
            if (isfromcheckout === "") {
                isfromcheckout = false;
            }
            fnRemoveDiscountCoupon(couponcode, IsFromFresh,isfromcheckout);
        });

        function fnDeleteKPDiscountCoupons() {
            var promiseGet = mainService.crudService('api/NOP/Discount/DeleteKPCoupon', $rootScope.myConstants.HTTPPost, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        fnGetAppliedDiscountCoupons();
                        $rootScope.globalOrderSummary = {};
                        FnGetOrderSummary();
                    };
                },
                function (error) { });
        }

        $rootScope.$on('broadCastDeleteKPDiscountCoupons', function (event) {
            if (!!$localStorage.authorizationData) {
                fnDeleteKPDiscountCoupons();
            }
        });

        function fnAddKPDiscountCoupon(KPointstoRedeem, KPointsWorth, CustomerGuid) {
            //var promiseGet = mainService.crudService('api/NOP/Discount/AddKPCoupon?kpointsToRedeem=' + KPointstoRedeem + '&kpointsWorth=' + KPointsWorth, $rootScope.myConstants.HTTPPost, {}, true);
            //promiseGet.then(
            //    function (success) {
            //        if (success.status == 200) {
            //            FnApplyGrocerDiscountCoupon(success.data.data);
            //            FnGetOrderSummary();
            //        };
            //    },
            //    function (error) { });
            var requestObj = {
                "DiscountAmount": KPointsWorth,
                "KPointsToRedeem": KPointstoRedeem,
                "CustomerGUID": CustomerGuid
            };

            var promiseGet = mainService.crudService('api/client/InsertDiscountForOnlineForOnline', $rootScope.myConstants.HTTPPost, requestObj);
            promiseGet.then(function (success) {
                $rootScope.globalOrderSummary = {};
                FnGetOrderSummary();
            }), function (error) {
                console.log(error);
            };
        }

        $rootScope.$on('broadCastAddKPDiscountCoupon', function (event, KPointstoRedeem, KPointsWorth, CustomerGuid) {
            fnAddKPDiscountCoupon(KPointstoRedeem, KPointsWorth, CustomerGuid);
        });

        function fnGetTopNSiteReviews(reviewCount) {
            var promiseGet = mainService.crudService('api/SiteReviews/GetTopN?returnCount=' + reviewCount, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        vm.SiteReviewData = success.data;
                    }
                },
                function (error) { });
        }
        function AboutUsByIsActiveIsCanview() {
            var promiseGet = mainService.crudService('api/AboutUs/AboutUsByIsActiveIsCanview', $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        vm.AboutUs = success.data;
                        vm.AboutUs[0].ImagePath = $rootScope.myConstants.membershipModuleApiURL + success.data[0].ImagePath;
                    }
                },
                function (error) { });
        }
        function GetLandingPageBannerImages() {
            var PageName = $rootScope.myConstants.PageName
            var promiseGet = mainService.crudService('api/KiposBannerForLandingPage/GetLandingPageBannerImages?PageName=' + PageName, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        // vm.LandingPage = success.data[0];
                        vm.LandingPage = $rootScope.myConstants.membershipModuleApiURL + success.data[0].ImagePath;

                    }
                },
                function (error) { });
        }
        function GetSponsersImages() {
            var PageName = $rootScope.myConstants.PageName
            var promiseGet = mainService.crudService('api/Sponsor/SponsorByIsActiveIsCanview', $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        vm.Sponsers = success.data;

                    }
                },
                function (error) { });
        }
        function GettrendingImages() {
            var PageName = $rootScope.myConstants.PageName
            var promiseGet = mainService.crudService('api/KiposInstagramPageImage/GetallActiveimages', $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(
                function (success) {
                    if (success.status == 200) {
                        vm.TrendingImage = success.data;

                    }
                },
                function (error) { });
        }
        vm.locationhref = function (eventValue) {
            $window.location.href = window.location.href + eventValue;
        };



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


        $rootScope.$on('broadcastGetParentCategories', function () {
            var promiseGet = mainService.crudService('api/NOP/Category/GetParentCategories', $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalCategories = success.data;
                    $scope.$emit('emitGetParentCategories');
                }
            }, function (error) {
                console.log(error);
            });
        });

        vm.fnModalSignup = function () {
            $('#modal-signup').modal('show');
        };


        $rootScope.$on('broadcastGetShippingLimit', function () {
            fnGetShippingLimit();
        });


        function fnGetShippingLimit() {
            var promiseGet = mainService.crudService('api/configuration?ConfigKey=' + $rootScope.myConstants.FREE_SHIPPING_LIMIT, $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalShippingMinLimit = parseFloat(success.data);
                }
            }, function (error) {
                console.log(error);
            });
        }

        function validateMobile(eventValue) {
            if (!$localStorage.IsAuthenticated) {
                if (!vm.emailValidationMobile.captureEmailMobile.$valid) {
                    angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                    return false;
                }
                if ($('#captureEmailMobile').val() === '') {
                    $('#captureEmailMobile').focus();
                }
                else {
                    var promiseGet = mainService.crudService("api/CaptureEmail?email=" + vm.emailValidationMobile.captureEmailMobile.$modelValue, $rootScope.myConstants.HTTPPost, {}, true);
                    promiseGet.then(function (success) {
                        if (success.status == 200) {
                            $localStorage.captureEmailMobile = vm.emailValidationMobile.captureEmailMobile.$modelValue;
                            $(".tooltip").hide();
                            if (eventValue === 'ModalSignup') {
                                vm.fnModalSignup();
                            } else {
                                $state.go(eventValue);
                            }
                        }
                    }, function (error) {
                        console.log(error);
                    });

                }
            }
        }

        vm.ValidateEmailEmpty = function () {

            if (screen.width > 767) {
                if ($('#captureEmail').val() === '') {
                    $(".tooltip").show();
                    $('#captureEmail').focus();
                }
                else {
                    $(".tooltip").hide();

                }
            } else if (screen.width <= 767) {
                if ($('#captureEmailMobile').val() === '') {
                    $(".tooltip").show();
                    $('#captureEmailMobile').focus();
                }
                else {
                    $(".tooltip").hide();

                }
            }
        };

        vm.validate = function (eventValue) {
            if (!!$localStorage.IsPOSUser) {
                if (eventValue === 'gourmetproducts') {
                    if ($localStorage.IsAuthenticated) {
                        if (eventValue === 'ModalSignup') {
                            vm.fnModalSignup();
                        } else {
                            fnRemoveWithoutDateTimeFCartItems(eventValue);
                        }
                    }
                    // else if (eventValue === 'cart') {
                    //     if (screen.width > 767) {
                    //         if ($('#captureEmail').val() === '') {
                    //             $('#captureEmail').focus();
                    //             angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                    //             $(".tooltip").show();
                    //             return false;
                    //         }
                    //         else {
                    //             fnRemoveWithoutDateTimeFCartItems(eventValue);
                    //         }

                    //     } else if (screen.width <= 767) {
                    //         if ($('#captureEmailMobile').val() === '') {
                    //             $('#captureEmailMobile').focus();
                    //             $(".tooltip").show();
                    //             return false;
                    //         }
                    //         else {
                    //             fnRemoveWithoutDateTimeFCartItems(eventValue);
                    //         }
                    //     }


                    // }
                    // else if (!$localStorage.IsAuthenticated) {
                    //     if (ngForm.$invalid) {
                    //         if (vm.emailValidationMobile.captureEmailMobile.$valid) {
                    //             validateMobile(eventValue);
                    //         }

                    //         if (screen.width > 767) {
                    //             angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                    //             $(".tooltip").show();
                    //             return false;

                    //         } else if (screen.width <= 767) {
                    //             $('#captureEmailMobile').focus();
                    //             $(".tooltip").show();
                    //             return false;
                    //         }
                    //     }
                    //     if ($('#captureEmail').val() === '') {
                    //         $('#captureEmail').focus();
                    //     }
                    //     else {
                    //         var promiseGet = mainService.crudService("api/CaptureEmail?email=" + ngForm.$modelValue, $rootScope.myConstants.HTTPPost, {}, true);
                    //         promiseGet.then(function (success) {
                    //             if (success.status == 200) {
                    //                 $localStorage.captureEmail = ngForm.$modelValue;
                    //                 $(".tooltip").hide();
                    //                 if (eventValue === 'ModalSignup') {
                    //                     vm.fnModalSignup();
                    //                 } else {
                    //                     fnRemoveWithoutDateTimeFCartItems(eventValue);
                    //                 }
                    //             }
                    //         }, function (error) {
                    //             console.log(error);
                    //         });

                    //     }
                    // }
                } else {
                    toaster.pop('warning', "", "POS User Logged In!!");
                }

            } else if (!$localStorage.IsPOSUser) {
                // if ($localStorage.IsAuthenticated) {
                if (eventValue === 'ModalSignup') {
                    vm.fnModalSignup();
                } else {
                    fnRemoveWithoutDateTimeFCartItems(eventValue);
                }
                //  } 
                // else if (eventValue === 'cart') {
                //     if (screen.width > 767) {
                //         if ($('#captureEmail').val() === '') {
                //             $('#captureEmail').focus();
                //             angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                //             $(".tooltip").show();
                //             return false;
                //         }
                //         else {
                //             fnRemoveWithoutDateTimeFCartItems(eventValue);
                //         }

                //     } else if (screen.width <= 767) {
                //         if ($('#captureEmailMobile').val() === '') {
                //             $('#captureEmailMobile').focus();
                //             $(".tooltip").show();
                //             return false;
                //         }
                //         else {
                //             fnRemoveWithoutDateTimeFCartItems(eventValue);
                //         }
                //     }


                // }
                // else if (!$localStorage.IsAuthenticated) {
                //     if (ngForm.$invalid) {
                //         if (vm.emailValidationMobile.captureEmailMobile.$valid) {
                //             validateMobile(eventValue);
                //         }

                //         if (screen.width > 767) {
                //             angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                //             $(".tooltip").show();
                //             return false;

                //         } else if (screen.width <= 767) {
                //             $('#captureEmailMobile').focus();
                //             $(".tooltip").show();
                //             return false;
                //         }
                //     }
                //     if ($('#captureEmail').val() === '') {
                //         $('#captureEmail').focus();
                //     }
                //     else {
                //         var promiseGet = mainService.crudService("api/CaptureEmail?email=" + ngForm.$modelValue, $rootScope.myConstants.HTTPPost, {}, true);
                //         promiseGet.then(function (success) {
                //             if (success.status == 200) {
                //                 $localStorage.captureEmail = ngForm.$modelValue;
                //                 $(".tooltip").hide();
                //                 if (eventValue === 'ModalSignup') {
                //                     vm.fnModalSignup();
                //                 } else {
                //                     fnRemoveWithoutDateTimeFCartItems(eventValue);
                //                 }
                //             }
                //         }, function (error) {
                //             console.log(error);
                //         });

                //     }
                // }
            }




            //$localStorage.globalCartInformation_Grocer_length = $rootScope.globalCartInformation_Grocer.length;
            //$localStorage.globalCartInformation_Gourmet_length = $rootScope.globalCartInformation_Gourmet.length;
            //$localStorage.globalCartInformation_Fresh_length = $rootScope.globalCartInformation_Fresh.length;

        };


        function fnRemoveCartItems(eventValue) {

            var requestobj = {
            };
            var promiseGet = mainService.crudService('Api/Client/OnlineCart', $rootScope.myConstants.HTTPPost, requestobj);
            promiseGet.then(function (success) {
                $rootScope.globalCartInformation_Fresh = $.grep(success.data.Items, function (e) { return e.ParentCategoryId == $rootScope.myConstants.CategoryId_Fresh; });
                $rootScope.globalCartInformation_Fresh_length = $rootScope.globalCartInformation_Fresh.length;

                $rootScope.globalCartInformation_Promotions = $.grep(success.data.Items, function (e) { return e.ProductId == $rootScope.myConstants.Promotions_productId; });
                $rootScope.globalCartInformation_Promotions_length = $rootScope.globalCartInformation_Promotions.length;

                $rootScope.globalCartInformation_Grocer = $.grep(success.data.Items, function (e) { return e.ParentCategoryId == $rootScope.myConstants.CategoryId_Grocer; });
                $rootScope.globalCartInformation_Grocer_length = $rootScope.globalCartInformation_Grocer.length;

                $rootScope.globalCartInformation_Gourmet = $.grep(success.data.Items, function (e) { return e.ParentCategoryId == $rootScope.myConstants.CategoryId_Gourment; });
                $rootScope.globalCartInformation_Gourmet_length = $rootScope.globalCartInformation_Gourmet.length;

                var confirmMsg1 = "";
                if ($rootScope.globalCartInformation_Grocer_length > 0)
                    confirmMsg1 = "Grocery items already there in cart, remove them?";

                else if ($rootScope.globalCartInformation_Gourmet_length > 0) {
                    if ($rootScope.globalCartInformation_Promotions_length > 0)
                        confirmMsg1 = "Promotion items already there in cart, remove them?";
                    else
                        confirmMsg1 = "Gourmet items already there in cart, remove them?";
                }
                else if ($rootScope.globalCartInformation_Fresh_length > 0)
                    confirmMsg1 = "Fresh items already there in cart, remove them?";

                if (eventValue === 'gourmetproducts') {
                    if (($rootScope.globalCartInformation_Grocer_length <= 0 && $rootScope.globalCartInformation_Fresh_length <= 0) || $rootScope.globalCartInformation_Gourmet_length > 0) {
                        if ($rootScope.globalCartInformation_Promotions_length <= 0)
                            $state.go(eventValue);

                        else {
                            if ($window.confirm(confirmMsg1)) {
                                if ($rootScope.globalCartInformation_Promotions_length > 0) {
                                    $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Promotions[0].Id, $rootScope.myConstants.PAGE_NAMES.CART);
                                }
                            }

                        }
                    } else if ($rootScope.globalCartInformation_Grocer_length === undefined || $rootScope.globalCartInformation_Fresh_length === undefined || $rootScope.globalCartInformation_Promotions_length === undefined) {
                        $state.go(eventValue);
                    } else {

                        if ($window.confirm(confirmMsg1)) {
                            if ($rootScope.globalCartInformation_Grocer_length > 0) {
                                for (var i = 0; i < $rootScope.globalCartInformation_Grocer_length; i++) {
                                    $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Grocer[i].Id, $rootScope.myConstants.PAGE_NAMES.CART);
                                }
                            }
                            else if ($rootScope.globalCartInformation_Promotions_length > 0) {
                                $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Promotions.ProductId, $rootScope.myConstants.PAGE_NAMES.CART);
                            }
                            else if ($rootScope.globalCartInformation_Fresh_length > 0) {
                                var temp_str1 = "";
                                for (var k = 0; k < $rootScope.globalCartInformation_Fresh_length; k++) {
                                    vm.itemIds = $rootScope.globalCartInformation_Fresh[k].Id;
                                    temp_str1 = temp_str1 + vm.itemIds + ",";
                                }
                                var purchaseId1 = temp_str1.replace(/,+$/, '');
                                console.log(purchaseId1);

                                $rootScope.$broadcast('removefromcartbroadcast', purchaseId1, $rootScope.myConstants.PAGE_NAMES.FRESHCART);
                            }
                            $state.go(eventValue);
                        }
                    }
                } else if (eventValue === 'product') {
                    if (($rootScope.globalCartInformation_Gourmet_length <= 0 && $localStorage.globalCartInformation_Fresh_length <= 0 && $rootScope.globalCartInformation_Promotions_length <= 0) || $rootScope.globalCartInformation_Grocer_length > 0) {
                        $state.go(eventValue);
                    }
                    else if ($rootScope.globalCartInformation_Gourmet_length === undefined || $rootScope.globalCartInformation_Fresh_length === undefined) {
                        $state.go(eventValue);
                    }
                    else {

                        if ($window.confirm(confirmMsg1)) {
                            if ($rootScope.globalCartInformation_Gourmet_length > 0) {
                                for (var j = 0; j < $rootScope.globalCartInformation_Gourmet_length; j++) {
                                    $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Gourmet[j].Id, $rootScope.myConstants.PAGE_NAMES.CART);
                                }
                            } else if ($rootScope.globalCartInformation_Fresh_length > 0) {
                                var temp_str1 = "";
                                for (var kk = 0; kk < $rootScope.globalCartInformation_Fresh_length; kk++) {
                                    vm.itemIds = $rootScope.globalCartInformation_Fresh[kk].Id;
                                    temp_str1 = temp_str1 + vm.itemIds + ",";
                                }
                                var purchaseId1 = temp_str1.replace(/,+$/, '');
                                console.log(purchaseId1);

                                $rootScope.$broadcast('removefromcartbroadcast', purchaseId1, $rootScope.myConstants.PAGE_NAMES.FRESHCART);
                            }
                            $state.go(eventValue);
                        }
                    }
                }
                else if (eventValue === 'freshMeal') {
                    if (($rootScope.globalCartInformation_Gourmet_length <= 0 && $rootScope.globalCartInformation_Grocer_length <= 0) || $rootScope.globalCartInformation_Fresh_length > 0) {
                        $state.go(eventValue);
                    }
                    else if ($rootScope.globalCartInformation_Grocer_length === undefined || $rootScope.globalCartInformation_Gourmet_length === undefined) {
                        $state.go(eventValue);
                    }
                    else {

                        if ($window.confirm(confirmMsg1)) {

                            if ($rootScope.globalCartInformation_Gourmet_length > 0) {
                                for (var jj = 0; jj < $rootScope.globalCartInformation_Gourmet_length; jj++) {
                                    $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Gourmet[jj].Id, $rootScope.myConstants.PAGE_NAMES.CART);
                                }
                            } else if ($rootScope.globalCartInformation_Grocer_length > 0) {
                                for (var ii = 0; ii < $rootScope.globalCartInformation_Grocer_length; ii++) {
                                    $rootScope.$broadcast('removefromcartbroadcast', $rootScope.globalCartInformation_Grocer[ii].Id, $rootScope.myConstants.PAGE_NAMES.CART);
                                }
                            }
                            $state.go(eventValue);
                        }
                    }

                }
                else if (eventValue !== 'gourmetproducts' || eventValue !== 'product' || eventValue !== 'freshMeal') {
                    $state.go(eventValue);
                }
            }, function (error) {
                if (eventValue !== 'gourmetproducts' || eventValue !== 'product' || eventValue !== 'freshMeal') {
                    $state.go(eventValue);
                }
            });
        }

        function fnRemoveWithoutDateTimeFCartItems(eventValue) {
            var requestobj = {};
            var promiseGet = mainService.crudService('Api/Client/OnlineCart', $rootScope.myConstants.HTTPPost, requestobj);
            promiseGet.then(function (success) {
                vm.CartInformation = success.data.Items;
                if (success.data.Items[0].ParentCategoryId == $rootScope.globalCategories.Fresh) {
                    var requestobjOne = {};
                    var promiseGet1 = mainService.crudService("Api/Client/FreshCart", $rootScope.myConstants.HTTPPost, requestobjOne);
                    promiseGet1.then(function (response) {
                        if (response.data.mealPlanModels.length != 0) {
                            fnRemoveCartItems(eventValue);
                        } else {
                            var temp_str1 = "";
                            for (var k = 0; k < vm.CartInformation.length; k++) {
                                vm.itemIds = vm.CartInformation[k].Id;
                                temp_str1 = temp_str1 + vm.itemIds + ",";
                            }
                            var purchaseId1 = temp_str1.replace(/,+$/, '');
                            console.log(purchaseId1);

                            //  $rootScope.$broadcast('removefromcartbroadcast', purchaseId1, $rootScope.myConstants.PAGE_NAMES.FRESHCART);
                            vm.clk_RemoveFromCart(purchaseId1, $rootScope.myConstants.PAGE_NAMES.FRESHCART, true, eventValue);
                        }

                    });
                } else {
                    fnRemoveCartItems(eventValue);
                }
            }, function (error) {
                if (eventValue !== 'gourmetproducts' || eventValue !== 'product' || eventValue !== 'freshMeal') {
                    $state.go(eventValue);
                }
            });
        }

        vm.checkEmail = function (ngForm) {
            if (ngForm.$invalid) {
                if (screen.width > 767) {
                    angular.element('input.ng-invalid,select.ng-invalid').first().focus();
                    toaster.pop('error', "", "Please enter email");
                    $(".tooltip").show();
                    return false;

                } else if (screen.width <= 767) {
                    $('#captureEmailMobile').focus();
                    toaster.pop('error', "", "Please enter email");
                    $(".tooltip").show();
                    return false;
                }
            }
            else {
                var capEmail = '';
                if (screen.width > 767) {
                    capEmail = ngForm.captureEmail.$viewValue;
                } else if (screen.width <= 767) {
                    capEmail = ngForm.captureEmailMobile.$viewValue;
                }

                var promiseGet = mainService.crudService("api/CaptureEmail?email=" + capEmail, $rootScope.myConstants.HTTPPost, {}, true);
                promiseGet.then(function (success) {
                    if (success.status == 200) {
                        $localStorage.captureEmail = capEmail;
                        $("html, body").animate({ scrollTop: 320 }, 1000);
                        $(".tooltip").hide();
                    }
                }, function (error) {
                    console.log(error);
                });
            }
        };

        $rootScope.$on('broadcastSignUp', function () {
            fnRegister();
        });

        function fnRegister() {
            vm.clkSignupButton();
            $("#modal-signup").modal('show');
        }

        $rootScope.$on('broadcastLogIn', function () {
            fnLogIn();
        });

        function fnLogIn() {
            $("#modal-signin").modal('show');
        }

        $rootScope.$on('broadCastRemoveAppliedDiscounts', function () {
            fnRemoveAppliedDiscounts();
        });

        function fnRemoveAppliedDiscounts() {
            var membershipObj = {
                "CustomerGUID": $rootScope.globalCustomerDetails.CustomerGuid
            };
            var membershipRequest = mainService.crudService("Api/Client/UnSubscribeMember", $rootScope.myConstants.HTTPPost, membershipObj);
            membershipRequest.then(function (response) {
                $rootScope.globalAppliedDiscountCoupons = [];
                $rootScope.globalAllAppliedDiscountCoupons = [];
                $rootScope.globalMemberdiscounts = []; 
                //Added by sree for applying corp discounts in cart page 16_06_2020
                $rootScope.$emit('ApplyDomainDiscounts');
                //Added by sree for applying corp discounts in cart page 16_06_2020
            }, function (error) {

            });
        }

        $rootScope.$on('broadCastFreshMealPlans', function () {
            GetMealsPlans();
        });

        function GetMealsPlans() {
            var promiseGet = mainService.crudService("api/mealsplans/get", $rootScope.myConstants.HTTPGet, {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalFreshMealPlanData = success.data;
                    vm.freshPlan1 = success.data[0];
                    vm.freshPlan2 = success.data[1];
                    console.log(vm.freshPlan);
                }
            }, function (error) {
                console.log(error);
            });
        }
        function startSlick() {
            $('.slider').slick({
                slidesToShow: 3,
                slidesToScroll: 1,
                speed: 500,
                autoplay: true,
                dots: true,
                infinite: true,
                cssEase: 'linear',
                centerPadding: '40px',

                responsive: [
                    {
                        breakpoint: 1024,
                        settings: {
                            arrows: false,
                            centerMode: true,
                            centerPadding: '40px',
                            slidesToShow: 3
                        }
                    },
                    {
                        breakpoint: 991,
                        settings: {
                            arrows: false,
                            centerMode: true,
                            centerPadding: '40px',
                            slidesToShow: 2
                        }
                    },
                    {
                        breakpoint: 600,
                        settings: {
                            arrows: false,
                            centerMode: true,
                            centerPadding: '0px',
                            slidesToShow: 1
                        }
                    }
                ]
            })

        }


        function fnGetCountryCode(code) {
            var selectedCode;
            for (var i = 0; i < $rootScope.myConstants.globalCountryCodes.length; i++) {
                if (code == $rootScope.myConstants.globalCountryCodes[i].Code) {
                    selectedCode = $rootScope.myConstants.globalCountryCodes[i];
                    vm.maxdigits = $rootScope.myConstants.globalCountryCodes[i].maxlength;
                }
            }
            return selectedCode;
        }

        vm.validateMobileNumber = function (selectedCountry, phonenumber) {
            vm.patternError = false;
            if (!!phonenumber) {
                for (var i = 0; i < $rootScope.myConstants.globalCountryCodes.length; i++) {
                    if (selectedCountry.Code == $rootScope.myConstants.globalCountryCodes[i].Code) {
                        if (phonenumber.length != $rootScope.myConstants.globalCountryCodes[i].maxlength) {
                            vm.patternError = true;
                            vm.maxdigits = $rootScope.myConstants.globalCountryCodes[i].maxlength;
                            vm.IsMobileNumberExists = false;
                        }
                    }
                }
            }
            else {
                vm.patternError = true;
                for (var i = 0; i < $rootScope.myConstants.globalCountryCodes.length; i++) {
                    if (selectedCountry.Code == $rootScope.myConstants.globalCountryCodes[i].Code) {
                        vm.maxdigits = $rootScope.myConstants.globalCountryCodes[i].maxlength;
                    }
                }
            }
        };


    }
})();
