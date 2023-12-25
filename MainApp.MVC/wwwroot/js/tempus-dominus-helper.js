var DefaultDateFormat = "YYYY-MM-DD";
var DefaultDisplayDateFormat = "DD.MM.YYYY";

var DefaultDateTimeFormat = "YYYY-MM-DD HH:mm:ss";
var DefaultDisplayDateTimeFormat = "DD.MM.YYYY HH:mm:ss";


function createDatePicker(selector, format = DefaultDisplayDateFormat) {
    $(selector).datetimepicker({
        format: format,
        icons: {
            time: 'fa fa-clock'
        },
    });
}

function createDateTimePicker(selector, format = DefaultDisplayDateTimeFormat) {

    $(selector).datetimepicker({
        format: format,
        icons: {
            time: 'fa fa-clock',
        },
    });
}

function getDefaultFormatDate(selector, format = DefaultDateFormat) {
    var result = null;
    if ($(selector).datetimepicker("date") != null) {
        result = $(selector).datetimepicker("date").format(format);
    }
    return result;
}

function getDefaultFormatDateTime(selector, format = DefaultDateTimeFormat) {
    var result = null;
    if ($(selector).datetimepicker("date") != null) {
        result = $(selector).datetimepicker("date").format(format);
    }
    return result;
}

function setDefaultFormatDateTime(selector, date, format = DefaultDisplayDateTimeFormat) {
    $(selector).datetimepicker('date', date);
}


