/// <reference path="../vue/dist/vue.js" />
/// <reference path="../signalr/dist/browser/signalr.js" />


var STATE_NO_GAME = 0;
var STATE_FINDING_OPPONENT = 1;
var STATE_PREPARING_BOARD = 2;
var STATE_WAITING_YOUR_MOVE = 80;
var STATE_WAITING_OTHER_MOVE = 81;
var STATE_GAMEOVER = 99;

var ORIENTATION_H = '0';
var ORIENTATION_V = '1';

//////////////////////////////////////////

Vue.component('my-game-board', {
    props: {
        public: {
            required: true,
            type: Boolean
        },
        width: {
            required: true,
            type: Number
        },
        ships: {
            required: true,
            type: Array,
            default: function () {
                return [];
            }
        }
    },
    data: function () {

        cells = [];

        for (var i = 0; i < this.width; i++) {
            var row = [];

            for (var j = 0; j < this.width; j++) {
                row.push({
                    X: j,
                    Y: i
                });
            }

            cells.push(row);
        }

        return {
            cells: cells
        };
    },

    created: function() {
        console.log('component created');
    },

    methods: {
        boardCellClick: function (cell) {
            console.log(cell);
        },

        getCellStyle(c) {
            
            var ship = this.findShipInCell(c);

            return {
                visible: this.public && !!ship,
                active: true
            };
        },


        findShipInCell(cell) {
            return this.ships.find(s =>
                (s.Orientation === ORIENTATION_H && cell.X >= s.X && cell.X < (s.X + s.Size) && s.Y === cell.Y) ||
                (s.Orientation === ORIENTATION_V && cell.Y >= s.Y && cell.Y < (s.Y + s.Size) && s.X === cell.X)
            );
        }
    },
    
    template: `
        <table class="tb-board">
            <caption>{{ships.length}}</caption>
            <tr v-for="row of cells">
                <td v-for="c of row" v-on:click="boardCellClick(c)" v-bind:class="getCellStyle(c)">
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
        board1: {
            size: 10,
            ships:[]
        },
        board2: {
            size: 10,
            ships: []
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

            this.board1.ships.push({
                id: '1234',
                X: 5,
                Y: 5,
                Size: 3,
                Orientation: ORIENTATION_V,
                Health: 0
            });

            /*
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
            */
        }
    }
});

