/// <reference path="../vue/dist/vue.js" />
/// <reference path="../signalr/dist/browser/signalr.js" />


var DEFAULT_MAX_HEALTH = 100;

//////////////////////////////////////////

var page = new Vue({
    el: '#cnt-main',
    data: {
        gameClient: null,
        
        mainBoard: {
            size: 0,
            availableShips: [],
            selectedShip: null,
            ships: [],
            positioning: false,
            shots: []
        },
        opponentBoard: {
            size: 0,
            ships: [],
            shots: []
        }
    },
    computed: {

        isInGameSession() {
            return this.gameClient && !this.gameClient.isNotInGame();
        },
        isFindingOpponent() {
            return this.gameClient.isWaitingForPlayers();
        },
        isPreparingBoard() {
            return this.gameClient.isSettingGameBoard();
        },
        isAllShipsPositioned() {
            return this.isPreparingBoard && this.mainBoard.ships.length === this.mainBoard.availableShips.length;
        },
        isPlaying() {
            return this.gameClient.isInGameplay();
        },
        isMyTurn() {
            return this.gameClient.isWaitingYourPlay();
        },

        score() {
            return this.gameClient.getCurrentScore();
        }
       
    },
    created: function () {

    },
    methods: {

        selectShipForPosition: function (ship) {

            if (this.mainBoard.positioning) {
                return;
            }

            if (this.mainBoard.selectedShip === ship) {
                ship = null;
            }

            this.mainBoard.selectedShip = ship;
        },
                       
        setShipPosition: function (cell) {
            if (!this.isPreparingBoard) {
                return;
            }
            
            var board = this.mainBoard;

            if (!board.selectedShip || board.positioning) {
                return;
            }

            board.positioning = true;

            this.gameClient.positionShip(board.selectedShip.id, cell).then(function (posState) {
                
                var shipId = posState.shipID;
                var pos = posState.position;

                var selectedShip = board.availableShips.find(function (x) { return x.id === shipId; });
                selectedShip.X = pos.x;
                selectedShip.Y = pos.y;

                if (!selectedShip.positioned) {
                    selectedShip.positioned = true;

                    board.ships.push(selectedShip);
                }

                board.positioning = false;
            });
        },

        playerIsReady: function () {
            if (!this.isAllShipsPositioned) {
                return;
            }
                        
            this.gameClient.setReady();
        },

        shootOpponent: function (cell) {
            var self = this;

            if (this.isPlaying && this.isMyTurn) {
                self.gameClient.shootOpponent(cell).then(function () {
                    self.opponentBoard.shots.push(cell);
                });
            }
        },

        tryFindNewGameSession: function () {
            var self = this;

            self.gameClient = prepareGameClient({

                onGameSessionFound: function (gsInfo) {
                    console.log(gsInfo);

                    self.mainBoard.size = gsInfo.boardSize;
                    self.mainBoard.selectedShipId = null;
                    self.mainBoard.positioned = [];
                    self.mainBoard.availableShips = gsInfo.playerFleet.map(function (s) {
                        return Object.assign({
                            X: 0,
                            Y: 0,
                            health: DEFAULT_MAX_HEALTH,
                            destroyed: false
                        }, s);
                    });

                    self.opponentBoard = {
                        size: gsInfo.boardSize,
                        positioned: [],
                        ships: [],
                        shots: []
                    };
                },

                onWaitingForPlayers: function () {

                },

                onOpponentFound: function () {

                },

                onOpponentAttack: function (attackInfo) {
                    console.log('page onOpponentAttack', attackInfo);

                    self.mainBoard.shots.push({
                        X: attackInfo.position.x,
                        Y: attackInfo.position.y
                    });

                    if (attackInfo.targetID) {
                        var ship = self.mainBoard.availableShips.find(function (s) {
                            return s.id === attackInfo.targetID;
                        });

                        if (ship) {
                            ship.destroyed = true;
                        }
                    }
                },

                onWaitingPlayerConfirmation: function () {

                },

                onGameStart: function () {

                },

                onGameOver: function () {

                },

                onPlaying: function () {

                },

                onFinished: function () {

                }
            });
            self.gameClient.findGameSession();
        }
    }
});

