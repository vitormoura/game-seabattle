/// <reference path="../eventemitter3/umd/eventemitter3.js" />

const EVENT_ON_GAMESESSION_FOUND = 'EVENT_ON_GAMESESSION_FOUND';
const EVENT_ON_OPPONENT_ATTACK = 'EVENT_ON_OPPONENT_ATTACK';
const EVENT_ON_OPPONENT_FOUND = 'EVENT_ON_OPPONENT_FOUND';
const EVENT_ON_GAMESTART = 'EVENT_ON_GAMESTART';
const EVENT_ON_WAITING_FOR_PLAYERS = 'EVENT_ON_WAITING_FOR_PLAYERS';
const EVENT_ON_WAITING_PLAYERS_CONFIRMATION = 'EVENT_ON_WAITING_PLAYERS_CONFIRMATION';
const EVENT_ON_PLAYING = 'EVENT_ON_PLAYING';
const EVENT_ON_FINISHED = 'EVENT_ON_FINISHED';

const GameSessionStates = {
    NO_GAME: 0,
    WAITING_FOR_PLAYERS: 1,
    WAITING_PLAYERS_CONFIRMATION: 2,
    PLAYING: 3,
    CANCELLED: 4,
    FINISHED: 5
};

class GameClient extends EventEmitter3 {

    connection = null
    session = {
        id: null,
        state: GameSessionStates.NO_GAME,
        playerId: null,
        playerReady: false,
        currentPlayerId: null,
        winnerId: null,
        points: {
        }
    }

    constructor() {
        super();

    }

    isNotInGame() {
        return this.session.state === GameSessionStates.NO_GAME;
    }

    isWaitingForPlayers() {
        return this.session.state === GameSessionStates.WAITING_FOR_PLAYERS;
    }

    isSettingGameBoard() {
        return this.session.state === GameSessionStates.WAITING_PLAYERS_CONFIRMATION;
    }

    isInGameplay() {
        return this.session.state === GameSessionStates.PLAYING;
    }

    isGameover() {
        return this.session.state === GameSessionStates.FINISHED;
    }

    isWinner() {
        return this.isGameover() && this.session.winnerId === this.session.playerId;
    }

    isWaitingYourPlay() {
        return this.isInGameplay() && this.session.currentPlayerId === this.session.playerId;
    }

    ////////////////////////////////////////////

    build() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/game-sessions")
            .configureLogging(signalR.LogLevel.Debug)
            .build();

        this.connection.on('GameSessionFound', (gsInfo) => {
            console.log('OnGameSessionFound', gsInfo);

            this.session.id = gsInfo.id;
            this.session.state = gsInfo.state;
            this.session.playerId = gsInfo.playerID;

            this.emit(EVENT_ON_GAMESESSION_FOUND, gsInfo);
        });

        this.connection.on('OpponentAttack', (attackInfo) => {
            this.emit(EVENT_ON_OPPONENT_ATTACK, attackInfo);
        });

        this.connection.on('GameSessionStateChanged', (gpState) => {
            console.log('OnGameSessionStateChanged', gpState);

            //Opponent found
            if (this.session.state === GameSessionStates.WAITING_FOR_PLAYERS && gpState.state === GameSessionStates.EVENT_ON_WAITING_FOR_PLAYERS) {
                this.emit(EVENT_ON_OPPONENT_FOUND);
            }

            //Both players confirmed and the game started
            if (this.session.state === GameSessionStates.WAITING_PLAYERS_CONFIRMATION && gpState.state === GameSessionStates.PLAYING) {
                this.emit(EVENT_ON_GAMESTART);
            }

            this.session.state = gpState.state;
            this.session.currentPlayerId = gpState.currentPlayerTurn;

            switch (gpState.state) {

                case GameSessionStates.WAITING_FOR_PLAYERS:
                    this.emit(EVENT_ON_WAITING_FOR_PLAYERS, gpState);
                    break;

                case GameSessionStates.WAITING_PLAYERS_CONFIRMATION:
                    this.emit(EVENT_ON_WAITING_PLAYERS_CONFIRMATION, gpState);
                    break;

                case GameSessionStates.PLAYING:
                    this.session.points = gpState.playerScore;

                    this.emit(EVENT_ON_PLAYING, gpState);
                    break;
            }
        });

        this.connection.on('GameOver', state => {
            this.session.state = GameSessionStates.FINISHED;
            this.emit(EVENT_ON_FINISHED, state);
        });
    }

    findGameSession() {

        return this.connection.start().then((res) => {
            return this.connection.invoke('FindNewGameSession');
        });
    }

    positionShip(shipId, pos) {

        if (!this.isSettingGameBoard()) {
            return Promise.reject('invalid game state');
        }

        return this.connection.invoke('PositionShip', {
            sessionID: this.getSessionId(),
            shipID: shipId,
            position: pos
        });
    }

    setReady() {
        return this.connection.invoke('SetPlayerIsReady', {
            sessionID: this.getSessionId()
        });
    }

    shootOpponent(pos) {

        if (!this.isInGameplay()) {
            return Promise.reject('invalid game state');
        }

        return this.connection.invoke('ShootOpponent', {
            sessionID: this.getSessionId(),
            position: pos
        });
    }

    getSessionId() {
        return this.session.id;
    }

    getPlayerId() {
        return this.session.playerId;
    }

    getCurrentScore() {
        return this.session.points[this.session.playerId] || 0;
    }

       
}
