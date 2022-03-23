var app = angular.module('loginApp', [])
    .config(['$httpProvider', function ($httpProvider) {
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
        $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
    }]);


app.controller('loginController', function ($scope, $http, $window) {

    angular.element(document).ready(function () {
        $scope.getExternalRoutes();
    });

    $scope.getExternalRoutes = function () {
        $http({
            method: 'GET',
            url: "/Login/GetExternalRoutes",
        }).then(function (success) {
            var route = success.data.value.data;
            $scope.route = route;
        }, function (error) {
            if (error.status == 401) {
                $window.location.href = "../Home/Index";
            }
            if (!error.data) {
                $scope.NotificationMessage("Error", "Error en el sistema", "error");
            }
        });
    };

    $scope.login = function () {
        var address = $scope.route + "apiLogin/login/";
        var username = $scope.email;
        var password = $scope.password;

        if (username && password) {
            $http({
                method: 'POST',
                url: address,
                data: JSON.stringify({
                    usuario: username,
                    contrasenia: password
                }),
                headers: {
                    'dataType': "json",
                    'contentType': "application/json"
                }
            }).then(function (success) {
                if (success.data) {
                    var accessList = success.data;
                    $window.localStorage.setItem("AT", accessList.tokenAcceso);
                    
                    $scope.gotoHome();
                }
                if (!success.data) {
                    $scope.NotificationMessage("Cuidado!", "Usuario/Contrasena Incorrectos.", "warning");
                }
            }, function (error) {
                $scope.NotificationMessage("Error", "Error en el sistema", "error");
            });
        } else {
            $scope.NotificationMessage("Error", "Debe ingresar los datos correspondientes", "info");
        }
    };

    $scope.gotoHome = function () {
        $window.location.href = "/Usuarios/Index";
    };

    $scope.NotificationMessage = function (status, message, type) {
        Swal.fire({
            title: status,
            text: message,
            icon: type
        });
    };

});