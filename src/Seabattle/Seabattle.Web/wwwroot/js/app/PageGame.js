/// <reference path="../vue/dist/vue.js" />
/// <reference path="../signalr/dist/browser/signalr.js" />


var STATE_NO_GAME = 0;
var STATE_FINDING_OPPONENT = 1;
var STATE_PREPARING_BOARD = 2;
var STATE_WAITING_YOUR_MOVE = 80;
var STATE_WAITING_OTHER_MOVE = 81;
var STATE_GAMEOVER = 99;

var ORIENTATION_H = 0;
var ORIENTATION_V = 1;

var DEFAULT_MAX_HEALTH = 100;

//////////////////////////////////////////

function prepareGridData(width) {
    var cells = [];

    for (var i = 0; i < width; i++) {
        var row = [];

        for (var j = 0; j < width; j++) {
            row.push({
                X: j,
                Y: i
            });
        }

        cells.push(row);
    }

    return cells;
}

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
    watch: {
        width: function () {
            this.cells = prepareGridData(this.width);
        }
    },
    data: function () {
        return {
            cells: null
        };
    },

    created: function () {
        console.log('component created');
    },

    methods: {

        getCellStyle(c) {
            var ship = this.findShipInCell(c);

            return {
                visible: this.public && !!ship,
                active: true
            };
        },

        findShipInCell(cell) {
            return this.ships.find(s =>
                (s.orientation === ORIENTATION_H && cell.X >= s.X && cell.X < (s.X + s.size) && s.Y === cell.Y) ||
                (s.orientation === ORIENTATION_V && cell.Y >= s.Y && cell.Y < (s.Y + s.size) && s.X === cell.X)
            );
        }
    },

    template: `
        <table class="tb-board">
            <caption>{{ships.length}}</caption>
            <tr v-for="row of cells">
                <td v-for="c of row" v-on:click="$emit('cell-clicked', c)" v-bind:class="getCellStyle(c)">
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
            playerId: '',
            state: STATE_NO_GAME
        },
        board1: {
            size: 0,
            availableShips: [],
            selectedShip: null,
            positioned: [],
            positioning: false
        },
        board2: {
            size: 0,
            positioned: []
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
            console.log('handleBeginBoardConfiguration', board);

            this.board1.size = board.size;
            this.board1.availableShips = board.fleet.map(function (s) {
                return Object.assign({ X: 0, Y: 0, health: DEFAULT_MAX_HEALTH }, s);
            });
            this.board1.selectedShipId = null;
            this.board1.positioned = [];

            this.board2 = {
                size: board.size,
                positioned: []
            };

            this.gameSession.state = STATE_PREPARING_BOARD;
        },

        handleShipPositionConfirmed: function (posState) {
            
            console.log('handleShipPositionConfirmed', posState);

            var shipId = posState.shipID;
            var pos = posState.position;

            var selectedShip = this.board1.availableShips.find(function (x) { return x.id === shipId; });
            selectedShip.X = pos.x;
            selectedShip.Y = pos.y;

            if (!selectedShip.positioned) {
                selectedShip.positioned = true;

                this.board1.positioned.push(selectedShip);
            }

            this.board1.positioning = false;
        },

        handleOpponentPlay: function () {

        },

        handleGameError: function (msg) {
            console.error(msg);
            this.board1.positioning = false;
        },

        

        positionShip: function (cell) {
            var board = this.board1;

            if (!board.selectedShip || board.positioning) {
                return;
            }

            board.positioning = true;

            this.gameSession.conn.invoke('SetPositionPlayerShip', this.gameSession.id, board.selectedShip.id, cell).then(function (result) {
                console.log('result', result)
            }).catch(function (err) {
                console.error(err);
                board.positioning = false;
            });
        },

        selectShipForPosition: function (ship) {

            if (this.board1.positioning) {
                return;
            }

            if (this.board1.selectedShip === ship) {
                ship = null;
            }

            this.board1.selectedShip = ship;
        },

        findNewGameSession: function () {

            console.log('findNewGameSession');

            var self = this;

            ///*
            var conn = new signalR.HubConnectionBuilder().withUrl("/game-sessions").build();

            conn.on('GameSessionFound', this.handleGameSessionFound.bind(self));
            conn.on('BeginBoardConfiguration', this.handleBeginBoardConfiguration.bind(self));
            conn.on('ShipPositionConfirmed', this.handleShipPositionConfirmed.bind(self));
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
            //*/
        }
    }
});

