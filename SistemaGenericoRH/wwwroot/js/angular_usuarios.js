var app = angular.module('listUsersApp', [])
    .config(['$httpProvider', function ($httpProvider) {
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
        $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
    }]);

app.controller('listUsersController', function ($scope, $http, $window) {

    $scope.page = 1;
    $scope.itemsPage = 10;

    angular.element(document).ready(function () {
        $scope.GetExternalRoutes();
    });

    $scope.GetExternalRoutes = function () {
        $http({
            method: 'GET',
            url: "/Usuarios/GetExternalRoutes",
        }).then(function (success) {
            var route = success.data.value.data;
            $scope.route = route;
            $scope.getUsuarios();
            $scope.Datatable();
        }, function (error) {
            if (error.status == 401) {
                $window.location.href = "../Home/Index";
            }
            if (!error.data) {
                $scope.NotificationMessage("Error", "Error en el sistema", "error");
            }
        });
    };

    $scope.getUsuarios = function () {
        var address = $scope.route + "apiUsuario/ListDataUsuarios";
        $http({
            method: 'GET',
            url: address,
            headers: {
                'dataType': "json",
                'contentType': "application/json",
                'Authorization': "Bearer " + $window.localStorage.getItem('AT')
            }
        }).then(function (success) {
            $scope.fullList = success.data;
        }, function (error) {
            $scope.NotificationMessage("Error", "Error en el sistema", "error");
        });


    };

    $scope.saveUsuario = function (data) {
        if (data == null) {
            data = "";
        }
        localStorage.setItem("emailUpdate", data);
        location.href = "/Usuarios/Save";
    }

    $scope.modalDeleteUsuario = function (data) {
        Swal.fire({
            title: 'Cuidado',
            text: "Desea eliminar el usuario?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Si',
            cancelButtonText: 'No',
        }).then((result) => {
            if (result.value) {
                $scope.deleteUsuario(data);
            }
        });
    };

    $scope.Datatable = function () {
        $('#datatableUsuarios').DataTable({
            "scrollY": "400px",
            "scrollCollapse": false,
            "paging": false,
            "searching": false,
            "lengthChange": false,
            "info": false,
            "ordering": false,
            "sScrollXInner": "100%",
            "language": {
                "emptyTable": " "
            }
        });
    };

    $scope.deleteUsuario = function (data) {
        var address = $scope.route + "apiUsuario/deleteUsuario";
        $http({
            method: 'POST',
            url: address,
            data: JSON.stringify({
                correo: data
            }),
            headers: {
                'dataType': "json",
                'contentType': "application/json",
                'Authorization': "Bearer " + $window.localStorage.getItem('AT')
            }
        }).then(function (success) {
            if (success.status == 202) {
                $scope.NotificationMessage("Alerta", "Error al intenter desactivar este usuario", "warning");
                return;
            }
            if (success.status == 200) {
                $scope.getUsuarios();
                $scope.NotificationMessage("Bien hecho", "Ha guardado correctamente los datos", "success");
            }
        }, function (error) {
            $scope.NotificationMessage("Error", "Error en el sistema", "error");
        });
    };

    $scope.NotificationMessage = function (status, message, type) {
        Swal.fire({
            title: status,
            text: message,
            icon: type
        });
    };
});