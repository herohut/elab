chat = function () {
    var hub = $.connection.chat;

    hub.receiveText = function (txt) {
        var toapp = '<li>' + '<b>' + 'server' + '</b>' + ' (' + $.format.date(Date(), "hh:MM:ss") + '): ' + txt + '</li>';
        $('#chatMsgs').append(toapp);
    };

    hub.receiveMsg = function (msg) {
        var from = msg.FromUser != undefined && msg.FromUser != '' ? msg.FromUser : "Anonymous";
        var toapp = '<li>' + '<b>' + from + '</b>' + ' (' + $.format.date(Date(), "hh:MM:ss") + '): ' + msg.Text + '</li>';
        $('#chatMsgs').append(toapp);
    };

    hub.notifyUsers = function (users) {
        var txt = "";
        for (var i = 0; i < users.length; i++) {
            txt += users[i] + "; ";
        }

        $('#inRoom').html(txt);
    };

    $.connection.hub.start().fail(function (evt) {
        throw "Fail:" + evt;
    });

    $(document).ready(function () {
        $('#btnLogin').click(function () {
            hub.loginChat($('#loginName').val()).fail(function (evt) {
                $('#chatMsgs').append(evt);
            });
        });

        $('#btnSendChat').click(function () {
            hub.broadcastChatMsg({ FromUser: $('#loginName').val(), Text: $('#textToChat').val() });
            $('#textToChat').val('');
        });
    });
};