﻿var API_URL = (window.location.hostname == 'localhost' ? 'http://localhost:49668' : 'http://dummy.pt') + '/api';
var APP_URL = (window.location.hostname == 'localhost' ? 'http://localhost:49668' : 'http://dummy.pt');

/*****************************************************************************************************/
/**************************************** Helper Functions********************************************/
/*****************************************************************************************************/
function isFunction(functionToCheck) {
    var getType = {};
    return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
}

function goBack() {
    window.history.back();
}

function isDefined(str) {
    return (typeof (str) !== "undefined");
}

function getFileExtension(str) {
    return str.split('.').pop();
}

function getFileNameWithoutExtension(str) {
    return str.split('.')[0];
}

function getFileName(str) {
    return str.substring(str.lastIndexOf('/') + 1);
}

function getBase64ByInput(file, callback) {
    var reader = new FileReader();
    reader.onloadend = function () {
        callback(reader.result.substring(reader.result.indexOf(',') + 1));
    }
    reader.readAsDataURL(file);
}

/*****************************************************************************************************/
/******************************************* Plugins *************************************************/
/*****************************************************************************************************/
var angApp = angular.module('dft_ang_app', []);


angApp.controller('mainController', ['$scope', function ($scope) {
    $scope.isContentLoading = false;
}]);


var App = new (function () {

    /* private functions */
    function _notify(message, type) {
        $.growl({
            message: message
        }, {
            type: type,
            allow_dismiss: false,
            label: 'Cancel',
            className: 'btn-xs btn-inverse',
            placement: {
                from: 'top',
                align: 'center'
            },
            delay: 2500,
            animate: {
                enter: 'animated bounceIn',
                exit: 'animated bounceOut'
            },
            offset: {
                y: 10
            }
        });
    };

    var that = this;


    that.Init = function (element) {
        that.Tooltips(element);
        that.Selects(element);
        that.Popovers(element);
        that.DatePickers(element);
        that.TimePickers(element);
        that.Suggest(element);
        that.Tags(element);
    };

    that.ShowMessage = function (title, text, type, fn) {
        swal({
            title: title,
            text: text,
            type: type,
        }, function () {
            if (isFunction(fn))
                fn();
        });
        return false;
    }

    that.DatePickers = function (element) {
        if ($(element).find('[data-plugin="datepicker"]').length) {
            $(element).find('[data-plugin="datepicker"]').datetimepicker({
                format: 'DD/MM/YYYY'
            });
        }
    }

    that.TimePickers = function (element) {
        if ($(element).find('[data-plugin="timepicker"]').length) {
            $(element).find('[data-plugin="timepicker"]').datetimepicker({
                format: 'HH:mm:ss'
            });
        }
    }

    that.Tooltips = function (element) {
        if ($(element).find('[data-plugin="tooltip"]').length) {
            $('[data-plugin="tooltip"]').tooltip();
        }
    }


    that.Suggest = function (element) {
        if ($(element).find('[data-plugin="suggest"]').length) {
            $('[data-plugin="suggest"]').each(function () {
                $(this).suggest({ url: $(this).attr('data-suggest-url') });
            });
        }

        if ($(element).find('[data-plugin="suggest-local"]').length) {
            $('[data-plugin="suggest-local"]').each(function () {
                $(this).suggest({ url: null, values: $(this).attr('data-suggest-values').split(','), address: false });
            });
        }
    }

    that.Selects = function (element) {
        if ($(element).find('[data-plugin="select"]').length) {
            $('[data-plugin="select"]').chosen({
                width: '100%',
                allow_single_deselect: false,
            });
            $(element).find('.chosen-search').attr("style", "display:none");
        }
        if ($(element).find('[data-plugin="select-nullable"]').length) {
            $('[data-plugin="select-nullable"]').chosen({
                width: '100%',
                allow_single_deselect: true,
            });
        }
    }

    that.Popovers = function (element) {
        if ($(element).find('[data-plugin="popover"]').length) {
            $('[data-plugin="popover"]').tooltip();
        }
    }

    that.Tags = function (element) {
        if ($(element).find('[data-plugin="tags-selectize"]').length) {
            $('[data-plugin="tags-selectize"]').selectize({
                delimiter: ' ',
                persist: false,
                createOnBlur: true,
                duplicates:true,
                create: function (input) {
                    return {
                        value: input,
                        text: input
                    }
                }
            });
        }
    }

    that.ShowSuccessMessage = function (Message) {
        _notify(Message, 'success');
    }

    that.ShowErrorMessage = function (Message) {
        _notify(Message, 'danger');
    }

    that.ShowInfoMessage = function (Message) {
        _notify(Message, 'inverse');
    }

});

App.Init("body");