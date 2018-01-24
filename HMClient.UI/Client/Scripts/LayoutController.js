(function (app) {
    var LayoutController = function ($scope, accountService) {
        $scope.username = '';
        if (accountService.isLoggedIn()) {
            $scope.showLogInRegister = false;
            $scope.showNameLogout = true;
        }
        else {
            $scope.showLogInRegister = true;
            $scope.showNameLogout = false;
        }
        $scope.$watch(
            function () {
                return sessionStorage.getItem('token')
            },
            function () {
                alert(sessionStorage.getItem('token'));
                //$scope.$apply();
                $scope.showLogInRegister
            });

        $scope.logout = function () {
            accountService.logout()
                .then(function (response) {
                    $scope.message = response.data;
                });
            //.catch(showError);
            accountService.clearCredentials();
        };
    }
        
    LayoutController.$inject = ["$scope", "AccountService"];

    app.controller("LayoutController", LayoutController);
}(angular.module("MyMail")));