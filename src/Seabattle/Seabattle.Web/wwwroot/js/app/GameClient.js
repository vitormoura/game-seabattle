﻿

function prepareGameClient(options) {

    var SESSION_STATE_NO_GAME = 0;
    var SESSION_STATE_WAITING_FOR_PLAYERS = 1;
    var SESSION_STATE_WAITING_PLAYERS_CONFIRMATION = 2;
    var SESSION_STATE_PLAYING = 3;
    var SESSION_STATE_CANCELLED = 4;
    var SESSION_STATE_FINISHED = 5;

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/game-sessions")
        .configureLogging(signalR.LogLevel.Debug)
        .build();

    var client = {

        session: {
            id: null,
            state: SESSION_STATE_NO_GAME,
            playerId: null,
            playerReady: false,
            currentPlayerId: null,
            points: {
            }
        },

        findGameSession: function () {
            return connection.start().then(function (res) {
                return connection.invoke('FindNewGameSession');
            });
        },

        positionShip: function (shipId, pos) {

            if (!client.isSettingGameBoard()) {
                return Promise.reject('invalid game state');
            }

            return connection.invoke('PositionShip', {
                sessionID: client.getSessionId(),
                shipID: shipId,
                position: pos
            });
        },

        setReady: function () {
            return connection.invoke('SetPlayerIsReady', {
                sessionID: client.getSessionId()
            });
        },

        shootOpponent: function (pos) {

            if (!client.isInGameplay()) {
                return Promise.reject('invalid game state');
            }
                        
            return connection.invoke('ShootOpponent', {
                sessionID: client.getSessionId(),
                position: pos
            });
        },

        getSessionId: function () {
            return client.session.id;
        },

        getPlayerId: function () {
            return client.session.playerId;
        },

        getCurrentScore: function () {
            return client.session.points[client.session.playerId] || 0;
        },

        ///////////////////////

        isNotInGame: function () {
            return client.session.state === SESSION_STATE_NO_GAME;
        },

        isWaitingForPlayers: function () {
            return client.session.state === SESSION_STATE_WAITING_FOR_PLAYERS;
        },

        isSettingGameBoard: function () {
            return client.session.state === SESSION_STATE_WAITING_PLAYERS_CONFIRMATION;
        },

        isInGameplay: function () {
            return client.session.state === SESSION_STATE_PLAYING;
        },
                
        isWaitingYourPlay: function () {
            return client.session.currentPlayerId === client.session.playerId;
        }
    };

    connection.on('GameSessionFound', function (gsInfo) {
        console.log('OnGameSessionFound', gsInfo);

        client.session.id = gsInfo.id;
        client.session.state = gsInfo.state;
        client.session.playerId = gsInfo.playerID;

        if (options.onGameSessionFound) {
            options.onGameSessionFound(gsInfo);
        }
    });

    connection.on('OpponentAttack', function (attackInfo) {
        console.log('OnOpponentAttack', attackInfo);

        if (options.onOpponentAttack) {
            options.onOpponentAttack(attackInfo);
        }
    });

    connection.on('GameSessionStateChanged', function (gpState) {
        console.log('OnGameSessionStateChanged', gpState);

        //Opponent found
        if (client.session.state === SESSION_STATE_WAITING_FOR_PLAYERS &&
            gpState.state === SESSION_STATE_WAITING_PLAYERS_CONFIRMATION) {

            if (options.onOpponentFound) {
                options.onOpponentFound();
            }
        }

        //Both players confirmed and the game started
        if (client.session.state === SESSION_STATE_WAITING_PLAYERS_CONFIRMATION &&
            gpState.state === SESSION_STATE_PLAYING) {

            if (options.onGameStart) {
                options.onGameStart();
            }
        }

        client.session.state = gpState.state;
        client.session.currentPlayerId = gpState.currentPlayerTurn;

        switch (gpState.state) {

            case SESSION_STATE_WAITING_FOR_PLAYERS:
                options.onWaitingForPlayers && options.onWaitingForPlayers(gpState);
                break;

            case SESSION_STATE_WAITING_PLAYERS_CONFIRMATION:
                options.onWaitingPlayerConfirmation && options.onWaitingPlayerConfirmation(gpState);
                break;

            case SESSION_STATE_PLAYING:
                client.session.points = gpState.playerScore;

                options.onPlaying && options.onPlaying(gpState);
                break;

            case SESSION_STATE_FINISHED:
                options.onFinished && options.onFinished(gpState);
                break;
        }
    });

    return client;
}