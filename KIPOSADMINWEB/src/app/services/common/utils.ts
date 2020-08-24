
export default class Utils {

    static getObjectByFilter(arrayToFilter: any, key: string, id: any) {
        return arrayToFilter.filter(x => x[key] === id);
    }

    static getObjectByFind(arrayToFilter: any, key: string, id: any) {
        return arrayToFilter.find(x => x[key] === id);
    }

    static getByIndex(customObject, index) {
        return customObject[Object.keys(customObject)[index]];
    }

    static getObjectNameByIndex(customObject, index) {
        return Object.keys(customObject)[index];
    }

    static logMessage(title, message) {
       
    }

    static stringify(input) {
        try {
         
        } catch (e) {
        }
    };

    static convertStringToDate(dateStr) {
        let parts = dateStr.split("/");
        return new Date(parts[2], parts[1] - 1, parts[0]);
    };

    static capitalizeFirstLetter(string) {
        if(string) {
            return string.charAt(0).toUpperCase() + string.slice(1);
        }
    }

    
    static convertToYandM(months): string {

        if(!months || months == '-1' || months == '0') {

            return ("");

        } else {

            let yearCalculated: any = (months / 12 | 0) ;
            let monthCalculated: any = (months % 12 | 0) ;

            yearCalculated =  (yearCalculated == 0 || yearCalculated == -1) ? "" : yearCalculated + " y  " ;
            monthCalculated = (monthCalculated == 0 || monthCalculated == -1) ? "" : monthCalculated + " m ";

            return (yearCalculated + monthCalculated);
        }
    }


    static convertServerDateToAccpetedDate(dateStringInRange) {
        var dateStr;
        if (dateStringInRange instanceof Date) {
            var now = dateStringInRange;
            var year = "" + now.getFullYear();
            var month = "" + (now.getMonth() + 1); if (month.length == 1) { month = "0" + month; }
            var day = "" + now.getDate(); if (day.length == 1) { day = "0" + day; }
            var hour = "" + now.getHours(); if (hour.length == 1) { hour = "0" + hour; }
            var minute = "" + now.getMinutes(); if (minute.length == 1) { minute = "0" + minute; }
            var second = "" + now.getSeconds(); if (second.length == 1) { second = "0" + second; }
            dateStr = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
        } else {
            if (dateStringInRange.indexOf(":") == -1) {
                dateStringInRange = dateStringInRange.trim();
                dateStringInRange = dateStringInRange + " 00:00:00";
            }
            dateStr = dateStringInRange;
        }
        var a = dateStr.split(" ");
        var d = a[0].split("-");
        var t = a[1].split(":");
        var date = new Date(d[0], (d[1] - 1), d[2], t[0], t[1], t[2]);
        return date;
    };

    static convertStringToBoolean(input) {
        if (Utils.isValidInput(input)) {
            if (input == "Y" || input == "y") {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    };

    static convertBooleanToString(input) {
        if (Utils.isValidInput(input)) {
            if (input == true || input == "true") {
                return "Y";
            } else {
                return "N";
            }
        } else {
            return "N";
        }
    };

    static formattingMobileNumber(cid) {
        var nomask = false;
        var cid = cid.replace(/(\d{3})(\d{3})(\d{4})/, "$1-$2-$3");
        return cid;
    };

    static unformattingMobileNumber(cid) {
        var cid = Utils.replaceAll(cid, "-", "");
        return cid;
    };

    static replaceAll(str, find, replace) {
        return this.replaceText(str, new RegExp(find, 'g'), replace);
    };

    static replaceText(str, find, replace) {
        if (this.isValidInput(str)) {
            return str.replace(find, replace);
        }
        else {
            return str;
        }
    };

    static toDisallowCharacters(event, charCodeToDisallow) {
        var charCode = event.which || event.keyCode;
        if (charCode == charCodeToDisallow) {
            event.preventDefault();
        }
    };


    static passwordValidator(str) {
        var pattern = /^(?=.*[a-zA-Z])(?=.*[\d])(?!.*([A-Za-z0-9])\1{2})[a-zA-Z\d,._]+$/;
        var result = pattern.test(str);
        if (result) {
            return true;
        } else {
            return false;
        }
    };

    static datecompare(date1, sign, date2) {
        var day1 = date1.getDate();
        var mon1 = date1.getMonth();
        var year1 = date1.getFullYear();
        var day2 = date2.getDate();
        var mon2 = date2.getMonth();
        var year2 = date2.getFullYear();
        if (sign === '===') {
            if (day1 === day2 && mon1 === mon2 && year1 === year2) {
                return true;
            } else {
                return false;
            }
        } else if (sign === '>') {
            if (year1 > year2) { return true; }
            else if (year1 === year2 && mon1 > mon2) { return true; }
            else if (year1 === year2 && mon1 === mon2 && day1 > day2) { return true; }
            else { return false; }
        } else if (sign === '<') {
            if (year1 < year2) { return true; }
            else if (year1 === year2 && mon1 < mon2) { return true; }
            else if (year1 === year2 && mon1 === mon2 && day1 < day2) { return true; }
            else { return false; }
        }
    };

    static formatCurrencyValue(input) {
        return parseFloat(input).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    };

    static isEmpty(input) {
        if (typeof input == "undefined") {
            return true;
        } else {
            var lstrTempstring = new String(input);
            lstrTempstring = lstrTempstring.trim(); //this is needed
            if (lstrTempstring == "" || lstrTempstring == "undefined") {
                return true;
            } else {
                return false;
            }
        }
    };

    static isUndefined(input) {
        if (typeof input == "undefined") {
            return true;
        } else {
            return false;
        }
    };

    static isNull(input) {
        if (input != null) {
            return false;
        } else {
            return true;
        }
    };

    static isValidInput(input) {
        if (Utils.isNull(input) || Utils.isUndefined(input) || Utils.isEmpty(input)) {
            return false;
        } else {
            return true;
        }
    };

    static truncate(input, truncatetext) {
        if (Utils.isValidInput(input)) {
            var res = input.replace(truncatetext, "");
            return res;
        } else {
            return input;
        }
    };

    static removeUnwantedSpaces(input) {
        return input
            .replace(/^\s\s*/, '')     // Remove Preceding white space
            .replace(/\s\s*$/, '');    // Remove Trailing white space
    }

    static escapeInput(input) {
        var output = input;
        output = this.replaceAll(output, '"', '\"');
        output = this.replaceAll(output, "'", "\'");
        return output;
    }

    static globalException(err) {
    }


    static Log(method, message, type) {
        
    }

    static refreshTranslations() {
        try {
        } catch (e) {
        }
    }

    static returnEmptyStringForInvalidInput(input) {
        if (this.isValidInput(input)) {
            return input;
        }
        else {
            return "";
        }
    }

    static setFocus(element) {
       
    }

    static translateContent(name, params) {
        try {
            if (params) {
            } else {
            }
        } catch (e) {
        }
    }

    static genericPreShow() {
        try {
         
        } catch (e) {
        }
    }

    static genericPostShow() {
        try {
          
        } catch (e) {
        }
    }

    static hideLoading() {
        try {
           
        } catch (e) {
        }
    }

    static showLoading() {
        try {
          
        } catch (e) {
        }
    }

}
