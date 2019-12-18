/// <reference path="../vue/dist/vue.js" />
/// <reference path="../signalr/dist/browser/signalr.js" />


var DEFAULT_MAX_HEALTH = 100;

//////////////////////////////////////////

new Vue({
    el: '#cnt-main',

    /**
     *  
     **/
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

        isGameSessionFound() {
            return this.gameClient && this.gameClient.getSessionId();
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
        isWinner() {
            return this.gameClient.isWinner();
        },
        isGameOver() {
            return this.gameClient && this.gameClient.isGameover();
        },
        score() {
            return this.gameClient.getCurrentScore();
        }

    },
    created: function () {

    },
    methods: {

        tryFindNewGameSession() {

            let client = new GameClient();

            client.build();

            client.on(EVENT_ON_GAMESESSION_FOUND, (gsInfo) => {

                this.mainBoard.size = gsInfo.boardSize;
                this.mainBoard.selectedShipId = null;
                this.mainBoard.positioned = [];
                this.mainBoard.availableShips = gsInfo.playerFleet.map(function (s) {
                    return Object.assign({
                        X: 0,
                        Y: 0,
                        health: DEFAULT_MAX_HEALTH,
                        destroyed: false
                    }, s);
                });

                this.opponentBoard = {
                    size: gsInfo.boardSize,
                    positioned: [],
                    ships: [],
                    shots: []
                };
            });

            client.on(EVENT_ON_OPPONENT_ATTACK, (attackInfo) => {

                this.updateBoardWithAttackInfo(attackInfo, this.mainBoard);

                if (attackInfo.success) {

                    var ship = this.mainBoard.availableShips.find(function (s) {
                        return s.id === attackInfo.target.ID;
                    });

                    if (ship) {
                        ship.destroyed = true;
                    }
                }
            });

            client.on(EVENT_ON_FINISHED, state => {

            });

            client.findGameSession();

            this.gameClient = client;
        },

        selectShipForPosition(ship) {

            if (!this.isPreparingBoard()) {
                return;
            }

            if (this.mainBoard.positioning) {
                return;
            }

            if (this.mainBoard.selectedShip === ship) {
                ship = null;
            }

            this.mainBoard.selectedShip = ship;
        },

        setShipPosition(cell) {
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

        playerIsReady() {
            if (!this.isAllShipsPositioned) {
                return;
            }

            this.gameClient.setReady();
        },

        shootOpponent(cell) {

            if (!this.isMyTurn) {
                return;
            }

            this.gameClient.shootOpponent(cell).then(attackInfo => {
                this.updateBoardWithAttackInfo(attackInfo, this.opponentBoard);
            });
        },

        updateBoardWithAttackInfo(attackInfo, board) {
            
            if (attackInfo.success) {
                attackInfo.target.cells.forEach(c => {
                    board.shots.push({ X: c.x, Y: c.y });
                });
                return;
            }

            board.shots.push({
                X: attackInfo.position.x,
                Y: attackInfo.position.y
            });
        }
    }
});

