//18. Edit controller
(function (app) {
    var app = angular.module("MyMail");

    var EditController = function ($scope, $routeParams, mailService) {

        $scope.result;
        $scope.errors = [];

        var folderId = $routeParams.folderId;

        //alert(folderId);
        //Only draft messages are allowed to update
        if (folderId != 'draft')
            $scope.hideUpdateButton = true;


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

        $scope.isEditable = function () {
            return $scope.edit && $scope.edit.mail;
        };

        $scope.cancel = function () {
            $scope.edit.mail = null;
        };

        //Listen to event from DetailsController
        $scope.$on('EditEvent', function (event, data) {
            //alert(folderId);
            $scope.edit.mail = data;
            //{
            //    'ID': data.ID,
            //    'To': folderId == "inbox" ? data.FromAddress : data.To,
            //    'Subject': data.Subject,
            //    'Body': data.Body
            //};

            //alert($scope.newmail.To);
            //alert($scope.newmail.Subject);
            //alert($scope.newmail.Body);
        });

        $scope.updateMessage = function () {
            mailService
                //$scope.edit.mail.ID is used for searching an existed message in database
                //then update the founded message with $scope.edit.mail.To, $scope.edit.mail.Subject, $scope.edit.mail.Body
                .update($scope.edit.mail)
                .then(function (response) {                  
                    //angular.extend($scope.mail, $scope.edit.mail);
                    //alert('id: ' + $scope.edit.mail.ID);
                    //alert(response.data.resultMsg.To);

                    //dispatch update upward to DetailsController
                    //so that DetailsController can update its view immmediately.
                    if(response.data.resultMsg != null)
                        $scope.$emit('MailUpdated', response.data.resultMsg);
                })
                .catch(showError);
        };

        $scope.postMessage = function (draft) {
            $scope.edit.mail.isDraft = draft;

            //alert('Is draft :' + draft);
            //alert('mail is draft :' + $scope.edit.mail.isDraft);

            mailService
                .create($scope.edit.mail)
                .then(function (response) {
                    //angular.extend($scope.mail, $scope.edit.mail);
                    $scope.edit.message = response.data;
                })
                .catch(showError);
        };


    }
    EditController.$inject = ["$scope", "$routeParams", "MailService"];

    app.controller("EditController", EditController);
}());