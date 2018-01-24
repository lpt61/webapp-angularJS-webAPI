(function () {
    //1. Create app module
    //first parameter: name of the module
    //second parameter: an array of dependencies for the module
    //var app = angular.module("MyMail", []);	//first parameter: name of the module

    //6. Register routing module
    var app = angular.module("MyMail", ["ngRoute"]);
    var config = function ($routeProvider) {
        $routeProvider
            .when(
                "/register",
                { templateUrl: "/client/views/register.html"})
            .when(
                "/login",
                { templateUrl: "/client/views/login.html" })
            .when(
                "/logout",
                { templateUrl: "/client/views/logout.html" })
            .when(
                "/list",
                { templateUrl: "/client/views/list.html" })
            .when(
                "/list/:folderId",
                { templateUrl: "/client/views/list.html" })           
            .when(
                "/details/:folderId/:mailId",
                { templateUrl: "/client/views/details.html"})
            .when(
                "/create",
                { templateUrl: "/client/views/create.html"})
            .otherwise({ redirectTo: "/list" });
    };

    //Only providers and constants can be injected into configuration blocks. 
    //This is to prevent accidental instantiation of services before they have been fully configured.
    app.config(config);


    //11. Register mailApiUrl
    app.constant("ApiMailUrl", "api/mail");
    app.constant("ApiAccountUrl", "api/account/");


    //Only instances and constants can be injected into run blocks. 
    //This is to prevent further system configuration during application run time.
    app.run(['$route', '$http', '$rootScope', '$location', 'AccountService', function ($route, $http, $rootScope, $location, accountService) {
        //$route.reload();

        // keep user logged in after page refresh
        var token = sessionStorage.getItem('token') || {};     
        var postLogInRoute;

        //console.log(accountService.isLoggedIn());


        if (token) {
            //console.log(token);
            //console.log(sessionStorage.getItem('username'));
            //console.log(sessionStorage.getItem('email'));
            $http.defaults.headers.common.Authorization = token; // jshint ignore:line           
            //console.log($http.defaults.headers.common.Authorization);

        };

        $rootScope.$on('$locationChangeStart', function (event, nextRoute, currentRoute) {
            // redirect to login page if not logged in
            if ($location.path() !== '/login'
                && $location.path() !== '/register'
                && !accountService.isLoggedIn()) {
                    postLogInRoute = $location.path();
                    $location.path('/login');
            }
            else if (postLogInRoute && accountService.isLoggedIn()) {               
                $location.path(postLogInRoute).replace();              
                postLogInRoute = null;
            }
        });
    }]);
}());