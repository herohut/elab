gomoku = function () {
    var hub = $.connection.gomoku;

    hub.notifyPlayersStatus = function (pList) {
        var $pls = $('#players');
        $pls.html('');
        for (var i = 0; i < pList.length; i++) {
            var p = pList[i];
            var name = p.Name == undefined || p.Name == '' ? 'Anynomous' : p.Name;
            $pls.append('<li data-ClientContextId="{ClientContextId}">'.replace("{ClientContextId}", p.ClientContextId) + 'Name:' + name + ' ClientContextId:' + p.ClientContextId + '</li>');
        }

        $pls.children('li').dblclick(function () {
            var vsId = $(this).attr('data-ClientContextId');
            if (vsId == clientId)
                return;

            hub.requestMatch(vsId);
            $('#matchStatus').html('Math: Waiting');
        });
    };

    var invitingPlayer = undefined;
    var clientId = undefined;
    hub.onMatchRequest = function (player) {
        invitingPlayer = player;
        var name = player.Name == undefined || player.Name == '' ? 'Anynomous' : player.Name;
        var txt = 'Player ' + name + ' is inviting you to a match. Do you accept?';
        $('#matchRequestText').html(txt);

        $('#matchConfirm').css('visibility', 'visible');
    };
    hub.onReceiveId = function(id) {
        clientId = id;
    };

    $('#btnYesMatch').click(function () {
        $('#matchConfirm').css('visibility', 'hidden');
        hub.answerMatch(invitingPlayer.ClientContextId, 'yes');

    });

    $('#btnNoMatch').click(function () {
        $('#matchConfirm').css('visibility', 'hidden');
        hub.answerMatch(invitingPlayer.ClientContextId, 'no');
    });

    var currentMatch = undefined;

    hub.onMatchAnswered = function (match, result) {
        if (result != 'yes') {
            
            $('#matchStatus').html('Match: No');
            return;
        }
        
        currentMatch = match;
        $('#matchStatus').html('Match: Yes');
        var name = invitingPlayer.Name == undefined || invitingPlayer.Name == '' ? 'Anynomous' : invitingPlayer.Name;
        alert('Start:' + name + ' AND ' + 'YOU');
    };

    $('#btnSetPlayerName').click(function () {
        hub.setPlayerName($('#playerName').val());
    });

    $('#testTick').click(function () {
        hub.tick({ X: 0, Y: 0 }, currentMatch.Id);
    });

    $.connection.hub.start().fail(function (evt) {
        throw "Fail:" + evt;
    });
};