/// <reference path="../Html5Framework/UiBase.js" />
/// <reference path="../Html5Framework/GeometryHelper.js" />
/// <reference path="NodeControl.js" />


// -----------------------------------------
// ConectorControl

ConnectorControl = function (canvas, context) {
    UserControl.prototype.constructor.call(this, canvas, context);

    this.X1 = 0;
    this.Y1 = 0;
    this.X2 = 0;
    this.Y2 = 0;
    this.Node1 = null;
    this.Node2 = null;
    this.Creating = false;

    this.updatePosition1 = function (xp, yp) {
        if (this.Node1 == null)
            return;

        var c1 = this.Node1.getConnectCircle();
        var points = GeometryHelper.getJoinedPointsOfCircleAndLine1(c1[0], c1[1], c1[2], xp, yp);
        if (Math.abs(points[0] - xp) < Math.abs(points[2] - xp)) {
            this.X1 = points[0];
            this.Y1 = points[1];
        }
        else {
            this.X1 = points[2];
            this.Y1 = points[3];
        }
        this.X2 = xp;
        this.Y2 = yp;
    };

    this.updatePosition = function () {
        if (this.Node1 == null)
            return;
        if (this.Node2 == null)
            return;
        var c1 = this.Node1.getConnectCircle();
        var c2 = this.Node2.getConnectCircle();
        var points = GeometryHelper.getJoinedPointsOf2Circles(c1[0], c1[1], c1[2], c2[0], c2[1], c2[2]);

        var minx = Math.min(c1[0], c2[0]);
        var maxx = Math.max(c1[0], c2[0]);
        if (points[0][0] > minx && points[0][0] < maxx) {
            this.X1 = points[0][0];
            this.Y1 = points[0][1];
        }
        else {
            this.X1 = points[0][2];
            this.Y1 = points[0][3];
        }

        if (points[1][0] > minx && points[1][0] < maxx) {
            this.X2 = points[1][0];
            this.Y2 = points[1][1];
        }
        else {
            this.X2 = points[1][2];
            this.Y2 = points[1][3];
        }
    };
}

ConnectorControl.prototype = new UserControl();

ConnectorControl.prototype.draw = function () {
    UserControl.prototype.draw.call(this);

    this.Canvas.drawLine({
        strokeStyle: "violet",
        strokeWidth: 3,
        x1: this.X1, y1: this.Y1,
        x2: this.X2, y2: this.Y2
    });

    this.Canvas.drawEllipse({
        fillStyle: "violet",
        x: this.X1,
        y: this.Y1,
        width: 5 * 2,
        height: 5 * 2
    });

    this.Canvas.drawEllipse({
        fillStyle: "violet",
        x: this.X2,
        y: this.Y2,
        width: 5 * 2,
        height: 5 * 2
    });


    if (this.Creating) {
        this.Canvas.drawEllipse({
            strokeStyle: "violet",
            strokeWidth: 1,
            x: this.X2,
            y: this.Y2,
            width: 8 * 2,
            height: 8 * 2
        });
    }
}