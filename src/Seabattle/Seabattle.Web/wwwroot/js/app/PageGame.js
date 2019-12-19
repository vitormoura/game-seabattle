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

            if (!this.isPreparingBoard) {
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

            }).catch(err => {
                shake(this.$refs.mainboardref);
            }).then(() => {
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

//////////////////////////////////////////

var shakingElements = [];

var shake = function (element, magnitude = 16, angular = false) {
    //First set the initial tilt angle to the right (+1) 
    var tiltAngle = 1;

    //A counter to count the number of shakes
    var counter = 1;

    //The total number of shakes (there will be 1 shake per frame)
    var numberOfShakes = 15;

    //Capture the element's position and angle so you can
    //restore them after the shaking has finished
    var startX = 0,
        startY = 0,
        startAngle = 0;

    // Divide the magnitude into 10 units so that you can 
    // reduce the amount of shake by 10 percent each frame
    var magnitudeUnit = magnitude / numberOfShakes;

    //The `randomInt` helper function
    var randomInt = (min, max) => {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    };

    //Add the element to the `shakingElements` array if it
    //isn't already there
    if (shakingElements.indexOf(element) === -1) {
        //console.log("added")
        shakingElements.push(element);

        //Add an `updateShake` method to the element.
        //The `updateShake` method will be called each frame
        //in the game loop. The shake effect type can be either
        //up and down (x/y shaking) or angular (rotational shaking).
        if (angular) {
            angularShake();
        } else {
            upAndDownShake();
        }
    }

    //The `upAndDownShake` function
    function upAndDownShake() {

        //Shake the element while the `counter` is less than 
        //the `numberOfShakes`
        if (counter < numberOfShakes) {

            //Reset the element's position at the start of each shake
            element.style.transform = 'translate(' + startX + 'px, ' + startY + 'px)';

            //Reduce the magnitude
            magnitude -= magnitudeUnit;

            //Randomly change the element's position
            var randomX = randomInt(-magnitude, magnitude);
            var randomY = randomInt(-magnitude, magnitude);

            element.style.transform = 'translate(' + randomX + 'px, ' + randomY + 'px)';

            //Add 1 to the counter
            counter += 1;

            requestAnimationFrame(upAndDownShake);
        }

        //When the shaking is finished, restore the element to its original 
        //position and remove it from the `shakingElements` array
        if (counter >= numberOfShakes) {
            element.style.transform = 'translate(' + startX + ', ' + startY + ')';
            shakingElements.splice(shakingElements.indexOf(element), 1);
        }
    }

    //The `angularShake` function
    function angularShake() {
        if (counter < numberOfShakes) {
            console.log(tiltAngle);
            //Reset the element's rotation
            element.style.transform = 'rotate(' + startAngle + 'deg)';

            //Reduce the magnitude
            magnitude -= magnitudeUnit;

            //Rotate the element left or right, depending on the direction,
            //by an amount in radians that matches the magnitude
            var angle = Number(magnitude * tiltAngle).toFixed(2);
            console.log(angle);
            element.style.transform = 'rotate(' + angle + 'deg)';
            counter += 1;

            //Reverse the tilt angle so that the element is tilted
            //in the opposite direction for the next shake
            tiltAngle *= -1;

            requestAnimationFrame(angularShake);
        }

        //When the shaking is finished, reset the element's angle and
        //remove it from the `shakingElements` array
        if (counter >= numberOfShakes) {
            element.style.transform = 'rotate(' + startAngle + 'deg)';
            shakingElements.splice(shakingElements.indexOf(element), 1);
            //console.log("removed")
        }
    }

};