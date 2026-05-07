function AlertMessage(message, status) {
    toastr.options = {
        "closeButton": true,              // Show X close button
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,              // Add progress bar
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "10000",
        "hideDuration": "10000",
        "timeOut": "10000",                // Toast auto-close time (5 sec)
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false
    };
    if (status === 'success') toastr.success(message);
    else if (status === 'info') toastr.info(message);
    else if (status === 'warning') toastr.warning(message);
    else toastr.error(message);
}