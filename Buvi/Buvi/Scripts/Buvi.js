/// <reference path="jquery-1.6.2.js" />
$(document).ready(function () {
    $('#refreshData').click(function () {
        $.ajax('/Buvi/DownloadData',
            {
               success: function() {
                   alert('OK');
               },
               error: function() {
                   alert('error');
               },
               fail: function() {
                   alert('fail');
               }
            });
    });
});