//10. Mail service
(function (app) {

    var headers = {};

    //token is stored in sessionStorage in AccountController.js
    //headers.Authorization = sessionStorage.getItem('token');

    var app = angular.module("MyMail");

    var mailService = function ($http, ApiMailUrl) {
        var getAll = function (folder) {         
            return $http.get(ApiMailUrl + '?folder=' + folder)
        };
        var getById = function (params) {          
            return $http.get(ApiMailUrl + '?folder=' + params.folderId + '&id=' + params.mailId)
        };
        var update = function (mail) {
            return $http.put(ApiMailUrl, mail)
        };
        var create = function (mail) {
            return $http.post(ApiMailUrl, mail)
        };
        var destroy = function (params) {          
            return $http.delete(ApiMailUrl + '?folder=' + params.folderId + '&id=' + params.mailId)
        };
        return {
            getAll: getAll,
            getById: getById,
            update: update,
            create: create,
            delete: destroy
        };
    };

    var accountService = function ($http, $rootScope, ApiAccountUrl) {
        var register = function (account) { return $http.post(ApiAccountUrl + 'register', account) };
        var login = function (account) {
            return $http({
                method: 'post',
                url: ApiAccountUrl + 'login',
                data: account
            })
        };

        var logout = function () {         
            return $http({
                method: 'POST',
                url: ApiAccountUrl + 'logout',
                //data : ""
            })
        };

        var setCredentials = function (account, token) {
            $http.defaults.headers.common.Authorization = token;

            sessionStorage.setItem('token', token);
            sessionStorage.setItem('username', account.username);
            sessionStorage.setItem('email', account.email);
        }

        var clearCredentials = function () {
            $http.defaults.headers.common.Authorization = '';

            sessionStorage.removeItem('token');
            sessionStorage.removeItem('username');
            sessionStorage.removeItem('email');
        };

        var isLoggedIn = function () {
            return sessionStorage.getItem('token') == null ? false : true;
        };

        return {
            register: register,
            login: login,
            logout: logout,
            setCredentials: setCredentials,
            clearCredentials: clearCredentials,
            isLoggedIn: isLoggedIn,
        };
    };

    app.factory("MailService", mailService);
    app.factory("AccountService", accountService);
}());