/// <reference path="../vue/dist/vue.js" />

var ORIENTATION_H = 0;
var ORIENTATION_V = 1;

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
            cells: prepareGridData(this.width)
        };
    },

    created: function () {
        console.log('my-game-board component created');
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
            <caption>{{width}} : {{ships.length}}</caption>
            <tr>
                <td>&nbsp;</td>
                <td v-for="(c, index) in cells">
                    {{index}}
                </td>
            </tr>
            <tr v-for="(row,index) of cells">
                <td>{{index}}</td>
                <td v-for="c of row" v-on:click="$emit('cell-clicked', c)" v-bind:class="getCellStyle(c)">
                    &nbsp;
                </td>
            </tr>
        </table>
    `
});