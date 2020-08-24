//lazy-loader in all pages
app.directive('lazyLoader', function () {
    return {
        templateUrl: 'app/common/templates/LazyLoader.html'
    };

});


app.directive('calculateKpoints', function () {
    return {
        require: 'ngModel',
        scope: { 'maxLength': '=' },
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    if (text.match(/^(?=.*\d)\d*[\.]?\d*$/) == null) {
                        transformedInput = text.slice(0, -1);
                    }
                    else {
                        if (!!text) {
                            if (parseFloat(text) > scope.maxLength) {
                                transformedInput = text.slice(0, -1);
                            }
                            else {
                                transformedInput = text;
                            }
                        }
                        //if (text.length > scope.maxLength) {
                        //    transformedInput = text.slice(0, -1);
                        //}
                        //else {
                        //    transformedInput = text;
                        //}
                    }
                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return "";
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});

app.directive('checkPattern', function () {
    return {
        require: 'ngModel',
        scope: {
            //'maxLength': '=',
            'checkPattern': '=',
        },
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    if (text.match(scope.checkPattern) == null) {
                        transformedInput = text.slice(0, -1);
                    }
                    else {
                        transformedInput = text;
                    }
                    //}
                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return "";
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});