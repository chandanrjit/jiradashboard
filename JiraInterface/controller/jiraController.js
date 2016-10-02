var app = angular.module('jiraapp', []);
app.config(['$httpProvider', function ($httpProvider) {
    // enable http caching
    $httpProvider.defaults.cache = true;
}])
app.controller('jiraCtrl', function ($scope, $http) {
    url = "http://localhost:8081/api/jira/JiraDetails";
    $http.get(url)
   .then(function (response) {
       $scope.results = response.data;
   });
       
});