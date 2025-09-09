const defaultPopup = Swal.mixin({
    customClass: {
        confirmButton: "btn btn-lg btn-success",
        cancelButton: "btn btn-lg btn-default me-2"
    },
    buttonsStyling: false,
    focusConfirm: true,
    confirmButtonText: jsRes(`OK`),
    cancelButtonText: jsRes(`Cancel`),
    showCancelButton: false,
    reverseButtons: true
});

const Toast = Swal.mixin({
    toast: true,
    position: "top-end",
    icon: "success",
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.onmouseenter = defaultPopup.fire.stopTimer;
        toast.onmouseleave = defaultPopup.fire.resumeTimer;
    }
});

let popupQueue = Promise.resolve();
function notifyAlerts(type, message, title) {
    if (type == "success") {
        Toast.fire({
            title: message ? message : jsRes("Success") + "!",
        });
    }
    if (type == "warning") {
        Toast.fire({
            title: message ? message : jsRes("Warning") + "!",
            icon: 'warning'
        });
    }
    if (type == "error") {
        Toast.fire({
            title: message ? message : jsRes("Error") + "!",
            icon: 'error'
        });
    }
    if (type == "info") {
        Toast.fire({
            title: message,
            icon: 'info'
        });
    }
    if (type == "popup-success") {
        defaultPopup.fire({
            title: title ? title : jsRes('Success') + '!',
            text: message,
            icon: 'success',
        });
    }
    if (type == "popup-info") {
        defaultPopup.fire({
            title: title ? title : jsRes('Info'),
            text: message,
            icon: 'info',
        });
    }

    if (type == "popup-warning") {
        defaultPopup.fire({
            title: title ? title : jsRes('Warning') + '!',
            text: message,
            icon: 'warning',
            //customClass: {
            //    confirmButton: "btn btn-lg btn-warning"
            //},
        });
    }
    if (type == "popup-error") {
        defaultPopup.fire({
            title: title ? title : jsRes('Error'),
            text: message,
            icon: 'error',
            //customClass: {
            //    confirmButton: "btn btn-lg btn-danger"
            //},
        });
    }

    if (type == "popup-confirm") {
        return defaultPopup.fire({
            title: title ? title : jsRes("Info"),
            text: jsRes(message),
            icon: 'info',
            showCancelButton: true,
        }).then(function (result) {
            if (result.isConfirmed) {
                return true;
            } else {
                return false;
            }
        });
    }

    if (type == "popup-info-confirm") {
        return defaultPopup.fire({
            title: title ? title : jsRes("Info"),
            text: jsRes(message),
            icon: 'info',
        }).then(function (result) {
            if (result.isConfirmed) {
                return true;
            } else {
                return false;
            }
        });
    }

    if (type == "popup-info-confirm-multiple") {
        popupQueue = popupQueue.then(() => {
            return defaultPopup.fire({
                title: title ? title : jsRes("Info"),
                text: jsRes(message),
                icon: 'info',
            }).then(function (result) {
                return !!result.isConfirmed;
            });
        });
        return popupQueue;
    }

    if (type == "popup-warning-delete") {
        //TODO fix this when calling from another page! promise is pending
        return defaultPopup.fire({
            title: title ? title : jsRes('Delete') + '?',
            text: message ? message : jsRes(`This action can't be undone!`),
            icon: 'warning',
            customClass: {
                confirmButton: "btn btn-lg btn-danger",
                cancelButton: "btn btn-lg btn-default me-2"
            },
            confirmButtonText: jsRes(`Yes`),
            cancelButtonText: jsRes(`No`),
            showCancelButton: true,
            focusConfirm: false,
        }).then((confirmed) => {
            if (confirmed) {
                return true;
            }
            else return false;
        })
    }
}

function ajaxErrorHandlingAlert(req = "", status = "") {
    if (status == "401") {
        defaultPopup.fire({
            title: jsRes("Your login session has expired."),
            text: jsRes("You will be redirected to the login page."),
            icon: 'warning',
            //customClass: {
            //    confirmButton: "btn btn-lg btn-danger"
            //},
        }).then(function (result) {
            if (result.isConfirmed) {
                window.location.reload();
            }
        });
    }
    else if (status == "403") {
        defaultPopup.fire({
            title: jsRes("You do not have authorization to view this page."),
            icon: 'warning',
            //customClass: {
            //    confirmButton: "btn btn-lg btn-danger"
            //},
        })
    }
    else if (status == "66") {
        defaultPopup.fire({
            title: jsRes("This Name already exists.."),
            icon: 'warning',
            //customClass: {
            //    confirmButton: "btn btn-lg btn-danger"
            //},
        })
    }
    else
        defaultPopup.fire({
            title: jsRes("Error occured!"),
            icon: 'warning',
            confirmButtonText: jsRes(`Reload`),
            showCancelButton: true,
            focusConfirm: false,
        }).then(function (result) {
            if (result.isConfirmed) {
                window.location.reload();
            }
        });
}