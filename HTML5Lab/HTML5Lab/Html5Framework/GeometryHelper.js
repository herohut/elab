/// <reference path="UiBase.js" />

function GeometryHelper () {
    this.isPointInsideRect = function (x1, y1, x2, y2, x, y) {
        var xmax = Math.max(x1, x2);
        var xmin = Math.min(x1, x2);
        var ymax = Math.max(y1, y2);
        var ymin = Math.min(y1, y2);
        return x >= xmin && x <= xmax && y >= ymin && y <= ymax;
    };

    this.isPointInsideBound = function (bound, xp, yp) {
        return this.isPointInsideRect(bound.Pos.X, bound.Pos.Y, bound.Right(), bound.Bottom(), xp, yp);
    }

    this.isPointInsideCircle = function (x1, y1, r, xp, yp) {
        return Math.pow((xp - x1), 2) + Math.pow((yp - y1), 2) <= r * r;

    };

    this.getJoinedPointsOf2Circles = function (x1, y1, r1, x2, y2, r2) {
        if (x1 == x2) {
            return [[x1, y1 - r1, x1, y1 + r1], [x2, y2 + r2, x2, y2 - r2]];
        }


        var a = (y2 - y1) / (x2 - x1);
        var b = -(y2 - y1) / (x2 - x1) * x1 + y1;

        return [this.getJoinedPointsOfCircleAndLine(x1, y1, r1, a, b), this.getJoinedPointsOfCircleAndLine(x2, y2, r2, a, b)];
    };

    // (x-x1)^2 + (y-y1)^2 = r1^2
    // y=ax+b;
    this.getJoinedPointsOfCircleAndLine = function (x1, y1, r, a, b) {
        var a1 = a * a + 1;
        var b1 = 2 * ((b - y1) * a - x1);
        var c1 = (b - y1) * (b - y1) + x1 * x1 - r * r;


        var x1 = ((-b1 + Math.sqrt(b1 * b1 - 4 * a1 * c1))) / (2 * a1);
        var x2 = ((-b1 - Math.sqrt(b1 * b1 - 4 * a1 * c1))) / (2 * a1);
        var y1 = a * x1 + b;
        var y2 = a * x2 + b;
        return [x1, y1, x2, y2];
    };

    this.getJoinedPointsOfCircleAndLine1 = function (x1, y1, r, x2, y2) {
        var a = (y2 - y1) / (x2 - x1);
        var b = -(y2 - y1) / (x2 - x1) * x1 + y1;

        return this.getJoinedPointsOfCircleAndLine(x1, y1, r, a, b);

    };
};

var GeometryHelper = new GeometryHelper();