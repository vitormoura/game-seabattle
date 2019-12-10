/// <reference path="../vue/dist/vue.js" />
/// <reference path="../signalr/dist/browser/signalr.js" />


var STATE_NO_GAME = 0;
var STATE_FINDING_OPPONENT = 1;
var STATE_PREPARING_BOARD = 2;
var STATE_WAITING_YOUR_MOVE = 80;
var STATE_WAITING_OTHER_MOVE = 81;
var STATE_GAMEOVER = 99;

//////////////////////////////////////////

Vue.component('my-game-board', {
    props: [
        'width'
    ],
    data: function () {

        cells = [];

        for (var i = 0; i < this.width; i++) {
            var row = [];

            for (var j = 0; j < this.width; j++) {
                row.push({
                    X: j,
                    Y: i,
                    Ship: null,
                    State: null
                });
            }

            cells.push(row);
        }

        return {
            cells: cells
        };
    },
    
    template: `
        <table class="tb-board">
            <tr v-for="row of cells">
                <td v-for="c of row">
                    &nbsp;
                </td>
            </tr>
        </table>
    `
});

var page = new Vue({
    el: '#cnt-main',
    data: {
        gameSession: {
            conn: null,
            id: null,
            playerId: null,
            state: STATE_NO_GAME
        },
        board: {
            size: 0,
            cells:[]
        }
    },
    computed: {
        isInGame() {
            return this.gameSession.state !== STATE_NO_GAME;
        },
        isNoGame() {
            return this.gameSession.state === STATE_NO_GAME;
        },
        isFindingOpponent() {
            return this.gameSession.state === STATE_FINDING_OPPONENT;
        },
        isPreparingBoard() {
            return this.gameSession.state === STATE_PREPARING_BOARD;
        },
        isWaitingForYou() {
            return this.gameSession.state === STATE_WAITING_YOUR_MOVE;
        },
        isWaitingYourOpponent() {
            return this.gameSession.state === STATE_WAITING_OTHER_MOVE;
        },
        isGameOver() {
            return this.gameSession.state === STATE_GAMEOVER;
        }
    },
    created: function () {
        
    },
    methods: {
        handleGameSessionFound: function (session) {
            console.log('handleGameSessionFound', session);

            this.gameSession.id = session.id;
            this.gameSession.playerId = session.player;
        },

        handleBeginBoardConfiguration: function (board) {

            this.gameSession.state = STATE_PREPARING_BOARD;
            
            for (var i = 0; i < board.size; i++) {
                for (var j = 0; i < board.size; j++) {
                    this.board.cells.push({
                        X: j,
                        Y: i,
                        Ship: null,
                        State: null
                    });
                }   
            }

            this.board = {
                size: board.size,
                cells: cells
            };
        },

        handleOpponentPlay: function () {

        },

        handleGameError: function () {

        },

        findNewGameSession: function () {
            console.log('findNewGameSession');

            var self = this;

            var conn = new signalR.HubConnectionBuilder().withUrl("/game-sessions").build();

            conn.on('GameSessionFound', this.handleGameSessionFound.bind(self));
            conn.on('BeginBoardConfiguration', this.handleBeginBoardConfiguration.bind(self));
            conn.on('OpponentPlay', this.handleOpponentPlay.bind(self));
            conn.on('GameError', this.handleGameError.bind(self));

            conn.start().then(function () {
                self.gameSession.state = STATE_FINDING_OPPONENT;

                self.gameSession.conn.invoke('FindNewGameSession').catch(function (err) {
                    console.error(err);
                });

            }).catch(function (err) {
                console.error(err);
            });

            self.gameSession.conn = conn;
        }
    }
});

