var app = angular.module('listSaveUserApp', [])
    .config(['$httpProvider', function ($httpProvider) {
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
        $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
    }]);

app.controller('listSaveUserController', function ($scope, $http, $window) {
    $scope.correoExist = false;
    $scope.sexo_input = "";

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
            $scope.Correo = localStorage.getItem("emailUpdate");
            localStorage.removeItem("emailUpdate");
            if ($scope.Correo) {
                $scope.correoExist = true;
                $scope.getUsuario();
            }
        }, function (error) {
            if (error.status == 401) {
                $window.location.href = "../Home/Index";
            }
            if (!error.data) {
                $scope.NotificationMessage("Error", "Error en el sistema", "error");
            }
        });
    };

    $scope.getUsuario = function () {
        var address = $scope.route + "apiUsuario/DataUsuarioByEmail/" + $scope.Correo;
        $http({
            method: 'GET',
            url: address,
            headers: {
                'dataType': "json",
                'contentType': "application/json",
                'Authorization': "Bearer " + $window.localStorage.getItem('AT')
            }
        }).then(function (success) {
            if (!success.data) {
                $scope.NotificationMessage("Error", "No existe informacion para este usuario", "error");
                return;
            }
            const usuario = success.data;
            $scope.correo_input = usuario.correo;
            $scope.user_input = usuario.usuario;
            $scope.sexo_input = usuario.sexo;
        }, function (error) {
            $scope.NotificationMessage("Error", "Error en el sistema", "error");
        });
    };

    $scope.btnFinalizarMethod = function () {
        var address = $scope.route + "apiUsuario/saveUsuario";
        var correoElectronico = $scope.correo_input;
        var nombreUsuario = $scope.user_input;
        var sexo = $scope.sexo_input;
        var password = $scope.pass_input;
        var passwordTwo = $scope.passTwo_input;

        if (!correoElectronico) {
            $scope.NotificationMessage("Informacion", "Verifique su correo electronico", "info");
            return;
            
        }

        if (nombreUsuario && sexo) {
            if (!$scope.correoExist && !password) {
                $scope.NotificationMessage("Informacion", "Complete los campos de contraseña", "info");
                return;
            }

            if (password != passwordTwo) {
                $scope.NotificationMessage("Informacion", "las contraseñas no coinciden", "info");
                return;
            }

            if ($scope.correoExist && (password == passwordTwo) && password != undefined) {
                if (password.length < 10) {
                    $scope.NotificationMessage("Informacion", "La contraseña debe contener al menos 10 caracteres", "info");
                    return;
                }

                if (!$scope.validarPass(password)) {
                    $scope.NotificationMessage("Informacion", "La contrasena debe contener Mayuscula Minuscula Digito Simbolo", "info");
                    return;
                }

                $scope.savePassword(password);
            }
            else {
                $scope.saveUser(address);
            }            
        } else {
            $scope.NotificationMessage("Informacion", "Por favor complete la informacion necesaria", "info");
        }
    };

    $scope.savePassword = function (data) {
        var address = $scope.route + "apiUsuario/updatePasswordUsuario";
        $http({
            method: 'PUT',
            url: address,
            data: JSON.stringify({
                userName: $scope.Correo,
                password: data
            }),
            headers: {
                'dataType': "json",
                'contentType': "application/json",
                'Authorization': "Bearer " + $window.localStorage.getItem('AT'),
                'userName': $scope.userName
            }
        }).then(function (success) {
            if (success.status > 230) {
                $scope.NotificationMessage("Error", "Ocurrio un error el intentar actualizar la contrasena", "error");
                return;
            }

            $scope.saveUser(address);
        }, function (error) {
                $scope.NotificationMessage("Error", "Error en el sistema", "error");
        });
    };

    $scope.saveUser = function (data) {
        var correoElectronico = $scope.correo_input;
        var nombreUsuario = $scope.user_input;
        var sexo = $scope.sexo_input;
        var password = $scope.pass_input;

        if (!$scope.validarEmail(correoElectronico)) {
            $scope.NotificationMessage("Informacion", "El correo no posee un formato correcto", "info");
            return;
        }

        if (nombreUsuario.length < 7) {
            $scope.NotificationMessage("Informacion", "El usuario debe contener al menos 7 caracteres", "info");
            return;
        }

        if (!$scope.validarAlfaNumerico(nombreUsuario)) {
            $scope.NotificationMessage("Informacion", "El usuario solo puede poseer caracteres alfanumericos", "info");
            return;
        }

        $http({
            method: 'POST',
            url: data,
            data: JSON.stringify({
                correo: correoElectronico,
                usuario: nombreUsuario,
                usuarioUpd: $scope.Correo, 
                sexo: sexo,
                contrasenia: password,
                isSave: $scope.correoExist ? false : true
            }),
            headers: {
                'dataType': "json",
                'contentType': "application/json",
                'Authorization': "Bearer " + $window.localStorage.getItem('AT')
            }
        }).then(function (success) {
            if (success.status == 202) {
                $scope.NotificationMessage("Informacion", "El usuario ya existe en el sistema", "info");
                return;
            }
            if (success.status == 200 || success.status == 201) {
                Swal.fire({
                    title: 'Bien hecho',
                    text: 'Ha guardado correctamente los datos',
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'Si',
                }).then((result) => {
                    if (result.value) {
                        location.href = "/Usuarios/";
                    }
                });
            }
        }, function (error) {
            $scope.NotificationMessage("Error", "Error en el sistema", "error");
        });
    };

    $scope.validarEmail = function (valor) {
        if (/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/.test(valor)) {
            return true;
        } else {
            return false;
        }
    }

    $scope.validarPass = function (valor) {
        if (/^(?=.*\d)(?=.*[\u0021-\u002b\u003c-\u0040])(?=.*[A-Z])(?=.*[a-z])\S{10,70}$/.test(valor)) {
            return true;
        } else {
            return false;
        }
    }

    var letras = "abcdefghyjklmnñopqrstuvwxyz0123456789";

    $scope.validarAlfaNumerico = function (texto) {
        texto = texto.toLowerCase();
        for (i = 0; i < texto.length; i++) {
            if (letras.indexOf(texto.charAt(i), 0) != -1) {
                return true;
            }
        }
        return false;
    }

    $scope.NotificationMessage = function (status, message, type) {
        Swal.fire({
            title: status,
            text: message,
            icon: type
        });
    };
});