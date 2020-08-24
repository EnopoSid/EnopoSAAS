app.config(function ($stateProvider, $urlRouterProvider, $locationProvider) {

    $locationProvider.hashPrefix('');
    $urlRouterProvider.otherwise('/');
    $locationProvider.html5Mode(true);

    $stateProvider

        // HOME STATES AND NESTED VIEWS ========================================
        //.state('home', {
        //    url: '/gourmetproducts',
        //    //templateUrl: 'app/components/home/Home.html',
        //    //controller: 'HomeController as vm'
        //    templateUrl: 'app/components/gourmetProducts/GourmetProducts.html',
        //    controller: 'GourmetProductsController as vm'
        //})

        .state('registration', {
            url: '',
            templateUrl: 'app/components/registration/Registration.html',
            controller: 'RegistrationController as vm'
        })
        .state('disclaimer', {
            url: '/disclaimer',
            templateUrl: 'app/components/disclaimer/Disclaimer.html',
            controller: 'DisclaimerController as vm'
        })
        .state('termsAndConditions', {
            url: '/termsAndConditions',
            templateUrl: 'app/components/terms/TermsAndConditions.html',
            controller: 'TermsAndConditionsController as vm'
        })
});