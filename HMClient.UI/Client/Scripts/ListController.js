//2. Reference to the module
/*
(function (app) {
}(angular.module("MyMail")));

//Similar to
(function (app) {
    var app = angular.module("MyMail");
}());
*/

//3.Create and register a controller (service)
//(function (app) {
//    //Specify the app module
//    var app = angular.module("MyMail");
//    //Create a constructor
//    var ListController = function () { };     //the constructor is empty for now
//    //Register the controller and its constructor to the app module
//    app.controller("ListController", ListController);   //the second param is the constructor
//}());

//4. Passing the model to the constructor, $scope is the MODEL consumed by the view
//(function (app) {
//    var app = angular.module("MyMail");
//    var ListController = function ($scope) {
//        $scope.message = "Hello";
//    };
//    app.controller("ListController", ListController);
//}());

//5. Modify the Controller to consume http service (by using $http parameter) to communicate 
//with Web API endpoints server and retrieves the data
//(function (app) {
//    var app = angular.module("MyMail");
//    //$scope and $http are 2 dependencies that ListController needs, so LC specifies their names as parameters
//    var ListController = function ($scope, $http) {
//        $scope.message = "Mail list";
//        //Make the request then put the responsive data into property mails of the MODEL
//        $http.get("api/mail").then(function (response) {
//            $scope.mails = response.data;
//        });
//    };
//    //Prevent JavaScript minifiers to manipulate the names of parameters that the controller needs
//    //without injection, the code may not work.
//    ListController.$inject = ["$scope", "$http"];

//    app.controller("ListController", ListController);
//}());

//13. Modify ListController to use mailService instead of $http
(function (app) {
    var app = angular.module("MyMail");
    var ListController = function ($scope, $rootScope, $location, $routeParams, mailService) {
        //if ($rootScope.globals.currentUser)
        //$scope.message = 'Token: ' + $rootScope.globals.currentUser.token;

        var hideFrom = false;
        var hideTo = false;
        var hideStatus = false;
        var hideRemoveButton = false;
        var hideDeleteButton = false;

        //With the help of $routeParams service, I need not to use $rootScope
        var folderId = $routeParams.folderId;
        var mailId = $routeParams.mailId;

        //In case the folderId is not specified
        if (typeof (folderId) == 'undefined') {
            $scope.type = 'inbox';
        }
        else{
            $scope.type = folderId;
        };

        //Show/hide elements
        if ($scope.type == 'outbox') {
            hideFrom = true;
            hideDeleteButton = true;
        }
        if ($scope.type == 'draft'){
            hideFrom = true;
            hideDeleteButton = true;
            hideStatus = true;
        }
        if ($scope.type == 'inbox') {
            hideTo = true;
            hideDeleteButton = true;
            hideStatus = true;
        }
        if ($scope.type == 'trashbin') {
            hideRemoveButton = true;
            hideStatus = true;
        };
        
        $scope.hide = {
            'from': hideFrom,
            'to': hideTo,
            'status': hideStatus,
            'removeButton': hideRemoveButton,
            'deleteButton': hideDeleteButton
        };

        //alert('from '+ $scope.hide.from + '\n'+ 'to' + $scope.hide.to + '\n' + 'status' + $scope.hide.status + '\n' + 'removeButton ' + $scope.hide.removeButton + '\n' + 'deleteButton '+ $scope.hide.deleteButton);

        mailService
            .getAll($scope.type)
            .then(function (response) {
                $scope.mails = response.data;
                //Make the mail state readable
                for (var i = 0; i < $scope.mails.length; i++) {
                    if ($scope.mails[i].State == 2) {
                        $scope.mails[i].Status = "Sent";
                    }
                }
            });

        //15. Modify ListController to handle the event called by the delete button in list.html
        $scope.delete = function (mailid) {
            var params = {
                'folderId': $scope.type,
                'mailId': mailid
            };
            mailService
                .delete(params)
                .then(function(){
                    removeById(params.mailId);
                });
        };
        
        $scope.moveToTrashBin = function (mailid) {
            var params = {
                'folderId': $scope.type,
                'mailId': mailid
            };
            mailService
                .delete(params)
                .then(function () {
                    removeById(params.mailId);
                });
        };

        var removeById = function (id) {
            for (var i = 0; i < $scope.mails.length; i++) {
                if ($scope.mails[i].ID == id) {
                    $scope.mails.splice(i, 1);
                    break;
                }
            }
        };

        //16. Create a new empty mail, pass it to EditController 
        $scope.create = function () {
            $scope.edit = {
                mail: {}
            }
        };
    };
    ListController.$inject = ["$scope", "$rootScope", "$location", "$routeParams", "MailService"];

    app.controller("ListController", ListController);
}());