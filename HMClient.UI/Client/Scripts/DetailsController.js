//9. DetailsController
//(function (app) {
//    var app = angular.module("MyMail");

//    var DetailsController = function ($scope, $http, $reouteParams) {
//        var id = $routeParams.id;      //$routeParams takes parameters extracted from URL 
//        $http.get("/api/mail/" + id).then(function (result) {
//            $scope.mail = result.data;
//        });
//    };
//    app.controller("DetailsController", DetailsController);
//}());

//14. Modify DetailsController to use mailService instead of $http
(function (app) {
    var app = angular.module("MyMail");
    var DetailsController = function ($rootScope, $scope, $routeParams, mailService) {

        var params = {
            'folderId': $routeParams.folderId,
            'mailId': $routeParams.mailId
        };

        mailService
            .getById(params)
            .then(function (response) {
                $scope.mail = response.data;               
                if (params.folderId == "inbox") {
                    $scope.mail.To = "Me";

                }
                if (params.folderId == "outbox") {
                    $scope.mail.From = "Me";
                }
                
            });

        $scope.$on('MailUpdated', function (event, data) {
            //alert(data.To);
            $scope.mail = data;
        });

        /*17. Edit mail
            - angular.copy() : copy the original detailed movie into a new one
			- if user cancel editing: throw the copied movie
			- if user edit and successfully save, the new info will be updated into the original movie
        */
        $scope.edit = function () {
            $scope.edit.mail = angular.copy($scope.mail);

            //Notify the EditController with data
            //DetailsController pass down email to EditController => use $broadcast
            $scope.$broadcast('EditEvent', $scope.mail);
        };
    };
    DetailsController.$inject = ["$rootScope", "$scope", "$routeParams", "MailService"];

    app.controller("DetailsController", DetailsController);
}());