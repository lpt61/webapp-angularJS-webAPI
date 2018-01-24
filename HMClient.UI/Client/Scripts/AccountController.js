(function (app) {
    var app = angular.module("MyMail");
    var AccountController = function ($scope, $location, accountService) {

        $scope.result;
        $scope.errors = [];

        $scope.showLogInRegister = true;
        $scope.showNameLogout = false;

        if (accountService.isLoggedIn()) {
            updateUserMenu();
        };

        $scope.cancel = function () {
            $scope.register.account = null;
            $scope.login.account = null;
        };

        $scope.register = function () {
            register();
        };

        $scope.login = function () {
            login();
        };

        function updateUserMenu () {
            $scope.showLogInRegister = false;
            $scope.showNameLogout = true;
            $scope.userName = sessionStorage.getItem('username');
        };

        var showError = function (e) {
            $scope.result = '';
            $scope.errors.length = 0;

            //show info of the event
            $scope.result = e.status + ' '
                + (e.statusText ? e.statusText : '');

            //show info of the returned object
            var result = e.data;
            if (result) {
                if (result.Message)
                    $scope.errors.push(result.Message);
                if (result.ModelState) {
                    var modelState = result.ModelState;
                    for (var prop in modelState) {
                        if (modelState.hasOwnProperty(prop)) {
                            var msgArr = modelState[prop]; // expect array here
                            if (msgArr.length) {
                                for (var i = 0; i < msgArr.length; ++i)
                                    $scope.errors.push(msgArr[i]);
                            }
                        }
                    }
                }
                if (result.error)
                    $scope.errors.push(e.error);
                if (result.error_description)
                    $scope.errors.push(e.error_description);
            }
            for (var i in $scope.errors) {
                $scope.result += '\n' + $scope.errors[i];
            }
            alert($scope.result);
        }

        var register = function () {
            accountService
                .register($scope.register.account)
                .then(function (response) {
                    $scope.message = response.data;
                    $scope.register.account = null;
                });
        }

        var login = function () {
            if (accountService.isLoggedIn())
                alert('please log out first')
            else {                
                accountService
                    .login($scope.login.account)
                    .then(
                        function (response) {
                            $scope.message = response.data;
                            //$scope.login.account = null;

                            var token = response.data.token_type + ' ' + response.data.access_token;

                            accountService.setCredentials($scope.login.account, token);

                            updateUserMenu();

                            //redirect to /list
                            //window.location.href = "#!/list";
                            //$location.path('/list');
                        })
                    .catch(showError);
            }
        }

        $scope.logout = function () {
            accountService.logout()
               .then(function (response) {
                   //Show message, for debugging
                   //$scope.message = response.data;

                   //refresh the browser
                   location.reload();
               })
           .catch(showError);
            accountService.clearCredentials();
        }

    };
    AccountController.$inject = ["$scope", "$location", "AccountService"];

    app.controller("AccountController", AccountController);
}());
