var app = angular.module('app', ['ui.router', 'ngStorage', '720kb.datepicker', 'ui.bootstrap', 'toaster', 'ngSanitize']);

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authenticateInterceptorService');
});

//Modified by surakshith to clear cache issue after build movement start 
app.run(function (constants, $rootScope, $localStorage, mainService, $templateCache, $state) {
//Modified by surakshith to clear cache issue after build movement end 
    //Added by surakshith to clear cache issue after build movement start 
    $rootScope.$on('$viewContentLoaded', function () {
        $templateCache.removeAll();
    });
    //Added by surakshith to clear cache issue after build movement end 
    $state.go("registration");
    $rootScope.myConstants = constants;
    $rootScope.globalRequestCount = 0;
    $rootScope.globalResponseCount = 0;
    $rootScope.globalRequestedServices = [];
    $rootScope.globalServiceInfoWithoutCustomerDetails = [];
    //$rootScope.categoryNav = $localStorage.categoryNavigation;

    $rootScope.nameOfLoggedInUser = !$localStorage.authorizationData ? null : $localStorage.authorizationData.userName;

    $rootScope.userData = {};
    $rootScope.globalCustomerDetails = {};
    $rootScope.globalCategories = undefined;
    $rootScope.globalCartInformation = {};
    var promiseGet = mainService.crudService('api/NOP/Category/GetParentCategories', $rootScope.myConstants.HTTPGet, {}, true);
    promiseGet.then(function (success) {
        if (success.status == 200) {
            $rootScope.globalCategories = success.data;
        }
    }, function (error) {
        console.log(error);
    });

    if(!$localStorage.IsAuthenticated && !$localStorage.authorizationData)
    {
        $rootScope.$broadcast('broadCastLogout');
    }

    if (!!$localStorage.IsAuthenticated && !!$localStorage.authorizationData) {
        if(!$localStorage.IsPOSUser){
            var promiseGet = mainService.crudService('api/MemberLogin?id=' + $localStorage.authorizationData.UserId, 'GET', {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.globalMemberDetails = success.data;
                    $localStorage.CustomerMobileNumber = success.data.MobileNumber;
                }
            }, function (error) {
                $rootScope.globalMemberDetails = {};
                console.log(error);
                });
        }else if(!!$localStorage.IsPOSUser){
            var promiseGet = mainService.crudService('api/POSUser/GetPOSUserById?id=' + $localStorage.authorizationData.UserId, 'GET', {}, true);
            promiseGet.then(function (success) {
                if (success.status == 200) {
                    $rootScope.posUser= $localStorage.IsPOSUser;
                    var tempObj={};
                    tempObj.FullName="";
                            tempObj=success.data;
                            tempObj.FullName=success.data.FirstName+""+success.data.LastName;
                            $rootScope.globalMemberDetails = tempObj;
                }
            }, function (error) {
                $rootScope.globalPOSUserDetails = {};
            });
        }

        
        var promiseGet = mainService.crudService('api/Customer/Details?emailId=' + $localStorage.authorizationData.EmailId + '', 'GET', {}, true);
        promiseGet.then(function (success) {
            if (success.status == 200) {
               // if (success.data.length > 0) {
                    $rootScope.globalCustomerDetails = success.data;

                    $rootScope.userData.CustomerGUID = $rootScope.globalCustomerDetails.CustomerGuid;
                    var promiseGet = mainService.crudService("Api/Client/Info", $rootScope.myConstants.HTTPPost, $rootScope.userData);
                    promiseGet.then(function (success) {
                        if (success.status == 200) {
                            $rootScope.globalUserInformation = {};
                            if (!!success.data) {
                                $rootScope.globalUserInformation = success.data;
                            }
                            var requestobj = {
                                "CustomerGUID": $rootScope.globalCustomerDetails.CustomerGuid,
                                "CurrencyId": "1"
                            };
                            var promiseGet = mainService.crudService('Api/Client/OnlineCart', $rootScope.myConstants.HTTPPost, requestobj);
                            promiseGet.then(function (success) {
                                if (success.data.Items[0].ParentCategoryId == $rootScope.globalCategories.Fresh) {
                                    var requestObj = {};
                                    var promiseGet1 = mainService.crudService("Api/Client/FreshCart", $rootScope.myConstants.HTTPPost, requestObj);
                                    promiseGet1.then(function (success) {
                                        $rootScope.globalCartInformation = success.data;
                                        $rootScope.globalCartInformation_Fresh = $rootScope.globalCartInformation.mealPlanModels;
                                        $rootScope.globalCartInformation_Fresh_SubTotal = 0;
                                        $rootScope.globalCartInformation_Fresh_Savings = 0;
                                        for (var j = 0; j < $rootScope.globalCartInformation_Fresh.length; j++) {

                                            $rootScope.globalCartInformation_Fresh_SubTotal = $rootScope.globalCartInformation_Fresh_SubTotal + $.sum($.map($rootScope.globalCartInformation_Fresh[j].Items, function (e) { return (e.SubTotal).split('$')[1] }));

                                            $rootScope.globalCartInformation_Fresh_Savings = $rootScope.globalCartInformation_Fresh_Savings + $.sum($.map($rootScope.globalCartInformation_Fresh[j].Items, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));
                                        }

                                        $rootScope.globalCartInformation_Fresh_SubTotalWithoutDiscount = ($rootScope.globalCartInformation_Fresh_SubTotal + $rootScope.globalCartInformation_Fresh_Savings);
                                        for (var i = 0; i < $rootScope.globalCartInformation.mealPlanModels.length; i++) {
                                            var tempCartInformation = $rootScope.globalCartInformation.mealPlanModels[i];
                                            $rootScope.globalCartInformation.mealPlanModels[i].MealPlanPrice = $.sum($.map(tempCartInformation.Items, function (e) { return (e.UnitPrice).split('$')[1] }));
                                            for (var k = 0; k < tempCartInformation.Items.length; k++) {
                                                // var text = tempCartInformation.Items[k].AttributeInfo.replace("<br />", " ,");
                                                // text = text.replace("<br />", " ,");
                                                // text = text.replace("BASE:", "");
                                                // text = text.replace("PROTEIN:", "");
                                                // text =  text.replace("&#233;", "é"); 
                                                // var mealText = text.replace("GREENS:", "");
                                                var text = tempCartInformation.Items[k].AttributeInfo.split("<br />").join(" ");
                                                //text = text.replace("<br />", " ,");
                                                //text = text.replace("BASE:", "");
                                                //text = text.replace("PROTEIN:", "");
                                                //text = text.replace("&#233;", "é");
                                                //vm.mealText = text.replace("GREENS:", "");
                                                var mealText = text;
                                                var mealDate = tempCartInformation.Items[k].MealDate.replace("T00:00:00", "");
                                                var TotalPrice = tempCartInformation.Items[k].UnitPrice;
                                                tempCartInformation.Items[k].AttributeInfo = mealText;
                                                tempCartInformation.Items[k].MealDate = mealDate;
                                            }

                                        }
                                    });
                                }
                                 else {
                                    $rootScope.globalCartInformation = success.data;
                                    $rootScope.globalCartInformation.ProductId=success.data.Items[0].ProductId;
                                    if (!!$rootScope.globalCategories) {
                                        $rootScope.globalCartInformation_Grocer =
                                            $.grep($rootScope.globalCartInformation.Items, function (e) { return e.ParentCategoryId == $rootScope.globalCategories.Grocer });


                                        $rootScope.globalCartInformation_Grocer_SubTotal = $.sum($.map($rootScope.globalCartInformation_Grocer, function (e) { return (e.SubTotal).split('$')[1] }));

                                        $rootScope.globalCartInformation_Grocer_Savings = $.sum($.map($rootScope.globalCartInformation_Grocer, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));

                                        $rootScope.globalCartInformation_Grocer_SubTotalWithoutDiscount = ($rootScope.globalCartInformation_Grocer_SubTotal + $rootScope.globalCartInformation_Grocer_Savings);

                                        $rootScope.globalCartInformation_Gourmet =
                                            $.grep($rootScope.globalCartInformation.Items, function (e) { return e.ParentCategoryId == $rootScope.globalCategories.Gourmet });

                                        $rootScope.globalCartInformation_Gourmet_Savings = $.sum($.map($rootScope.globalCartInformation_Gourmet, function (e) { return !!e.Discount ? ((e.Discount).split('$')[1]) : 0 }));


                                        $rootScope.globalCartInformation_Gourmet_SubTotalWithoutDiscount = ($rootScope.globalCartInformation_Gourmet_SubTotal + $rootScope.globalCartInformation_Gourmet_Savings);

                                        $rootScope.globalCartInformation_Gourmet_SubTotal = $.sum($.map($rootScope.globalCartInformation_Gourmet, function (e) { return (e.SubTotal).split('$')[1] }));
                                        }
                                }

                                $rootScope.$broadcast('getTotalOrderAmount');
                            });
                        }
                    }, function (error) {
                        console.log(error);
                    });

                //}
                //else {
                //    $rootScope.globalCustomerDetails = {};
                //}
            }
        }, function (error) {
            $rootScope.globalCustomerDetails = {};
            console.log(error);
        });
    }
    else {
        $rootScope.globalMemberDetails = {};
    }
    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        // if (toState.name == 'home') {
        $rootScope.currentState = toState.name;
        // }
    });

    $rootScope.globalPreviousState;
    $rootScope.globalCurrentState;
    $rootScope.$on('$stateChangeSuccess', function (ev, to, toParams, from, fromParams) {
        $rootScope.globalEmailForNewsLetter = '';
        $rootScope.isGlobalNewsLetterClicked  = false;
        $('#emailNewsLetter').val('');
        $rootScope.globalPreviousState = from.name;
        $rootScope.globalCurrentState = to.name;
        console.log('Previous state:' + $rootScope.globalPreviousState);
        console.log('Current state:' + $rootScope.globalCurrentState);
    });

});