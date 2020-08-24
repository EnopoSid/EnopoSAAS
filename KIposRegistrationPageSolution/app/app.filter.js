app.filter('PercentageCalculation', function () {
    return function (actualValue, percentage) {
        var tempValue = actualValue;
        if (!jQuery.isNumeric(actualValue)) {
            tempValue = parseFloat(actualValue.split('$')[1]);
        }
        return (tempValue * ((100 - percentage) / 100)).toFixed(2);
    };
});

app.filter('DiscountedAmountCalculation', function () {
    return function (actualValue, percentage) {
        var tempValue = actualValue;
        if (!jQuery.isNumeric(actualValue)) {
            tempValue = parseFloat(actualValue.split('$')[1]);
        }
        return (tempValue * ((percentage) / 100)).toFixed(2);
    };
});


app.filter('PercentageCalculationAddition', function () {
    return function (actualValue, percentage) {
        var tempValue = actualValue;
        if (!jQuery.isNumeric(actualValue)) {
            tempValue = parseFloat(actualValue.split('$')[1]);
        }
        return (tempValue * (100 / (100 - percentage))).toFixed(2);
    };
});

app.filter('PercentageCalculationByDiscountWithActualValue', function () {
    return function (DiscountedPrice, ActualPrice) {
        return ((100 - ((DiscountedPrice / ActualPrice) * 100))).toFixed(0);
    };
});


app.filter('StringtoDecimal', function () {
    return function (stringValue) {
        if (!!stringValue)
            return parseFloat(stringValue.split('$')[1]);
        else
            return 0;
    }
});

app.filter('ToFixed', function () {
    return function (value) {
        return !!value ? value.toFixed(2) : '0';
    }
});

app.filter('titleCase', function () {
    return function (input) {
        input = input || ''; return input.replace(/\w\S*/g, function (txt) {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        });
    };
});
app.filter('highlight', function($sce) {
    return function(text, phrase) {
      if (phrase) text = text.replace(new RegExp('('+phrase+')', 'gi'),
        '<span class="highlighted">$1</span>')

      return $sce.trustAsHtml(text)
    }
  })

