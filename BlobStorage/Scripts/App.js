'use strict';
angular.module('angularUploadApp', [
    'angularFileUpload'
])


.factory('SonService', function ($http, $q) {
    return {
        getWeather: function () {
            // the $http API is based on the deferred/promise APIs exposed by the $q service
            // so it returns a promise for us by default
            return $http.get('http://fishing-weather-api.com/sunday/afternoon')
                .then(function (response) {
                    if (typeof response.data === 'object') {
                        return response.data;
                    } else {
                        // invalid response
                        return $q.reject(response.data);
                    }

                }, function (response) {
                    // something went wrong
                    return $q.reject(response.data);
                });
        }
    };
})

.controller('UploadCtrl', function ($scope, $http, $timeout, $upload) {

        $scope.upload = [];
        $scope.fileUploadObj = { testString1: "Test string 1", testString2: "Test string 2" };

        $scope.onFileSelect = function ($files) {
            //$files: an array of files selected, each file has name, size, and type.
            for (var i = 0; i < $files.length; i++) {
                var $file = $files[i];
                (function (index) {
                    $scope.upload[index] = $upload.upload({
                        url: "https://microsoft-apiapp55759bba47b74474bffa45d9538d840b.azurewebsites.net/api/files/upload", // webapi url
                        method: "POST",
                        data: { fileUploadObj: $scope.fileUploadObj },
                        file: $file
                    }).progress(function (evt) {
                        // get upload percentage
                        console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                    }).success(function (data, status, headers, config) {
                        // file is uploaded successfully
                        console.log(data);
                    }).error(function (data, status, headers, config) {
                        // file failed to upload
                        console.log(data);
                    });
                })(i);
            }
        }

        $scope.abortUpload = function (index) {
            $scope.upload[index].abort();
        }
    })

.controller('ListController', function ($scope, $http, $timeout, $upload, SonService) {

    var makePromiseWithSon = function () {
        // This service's function returns a promise, but we'll deal with that shortly
        SonService.getWeather()
            // then() called when son gets back
            .then(function (data) {
                // promise fulfilled
                if (data.forecast === 'good') {
                    prepareFishingTrip();
                } else {
                    prepareSundayRoastDinner();
                }
            }, function (error) {
                // promise rejected, could log the error with: console.log('error', error);
                prepareSundayRoastDinner();
            });
    };


    $scope.getList = function () {
        $http({
            method: 'GET',
            url: 'https://microsoft-apiapp55759bba47b74474bffa45d9538d840b.azurewebsites.net/api/ringtones'
        }).then(function successCallback(response) {
            console.log(response)
            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }
});