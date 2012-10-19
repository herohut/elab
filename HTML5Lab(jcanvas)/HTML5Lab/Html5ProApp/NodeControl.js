/// <reference path="../Html5Framework/UiBase.js" />
/// <reference path="../Html5Framework/GeometryHelper.js" />


// ---------------------------------------
// NodeControl
NodeControl = function (canvas) {
    UserControl.prototype.constructor.call(this, canvas);
    this.circle1R = 50;
    this.circle2R = 35;
    this.circle3R = 10;
    this.borderStroke = "violet";


    this.getConnectCircle = function () {
        var center = this.Bound.Center();
        return [center.X, center.Y, this.circle1R];
    }
    this.checkPoint = function (xp, yp) {
        var center = this.Bound.Center();
        // create relation
        if (GeometryHelper.isPointInsideCircle(center.X, center.Y, this.circle1R, xp, yp) && !GeometryHelper.isPointInsideCircle(center.X, center.Y, this.circle2R, xp, yp))
            return "in_connector";
        if (GeometryHelper.isPointInsideRect(this.Bound.X(), this.Bound.Y(), this.Bound.Right(), this.Bound.Bottom(), xp, yp))
            return "in_normal";

        return "out";
    }
}

NodeControl.prototype = new UserControl();

NodeControl.prototype.onmousemove = function (a, e) {
    var isInside = GeometryHelper.isPointInsideBound(this.Bound, a.offsetX, a.offsetY);
    e.Cancel = isInside;
    this.borderStroke = isInside ? "red" : "violet";
}



NodeControl.prototype.draw = function () {
    UserControl.prototype.draw.call(this);
    var canvas = $(this.Canvas);

    canvas.drawRect({
        fromCenter: false,
        fillStyle: "white",
        strokeStyle: this.borderStroke,
        strokeWidth: 1,
        x: this.Bound.Pos.X,
        y: this.Bound.Pos.Y,
        width: this.Bound.Size.X,
        height: this.Bound.Size.Y,
        cornerRadius: 3
    });

    canvas.drawEllipse({
        strokeStyle: "gray",
        x: this.Bound.Pos.X + this.Bound.Size.X / 2,
        y: this.Bound.Pos.Y + this.Bound.Size.Y / 2,
        width: this.circle1R * 2,
        height: this.circle1R * 2
    });

    canvas.drawEllipse({
        strokeStyle: "violet",
        x: this.Bound.Pos.X + this.Bound.Size.X / 2,
        y: this.Bound.Pos.Y + this.Bound.Size.Y / 2,
        width: this.circle2R * 2,
        height: this.circle2R * 2
    });

    canvas.drawEllipse({
        fillStyle: "violet",
        x: this.Bound.Pos.X + this.Bound.Size.X / 2,
        y: this.Bound.Pos.Y + this.Bound.Size.Y / 2,
        width: this.circle3R * 2,
        height: this.circle3R * 2
    });

    canvas.drawText({
        fillStyle: "white",
        x: this.Bound.Pos.X + this.Bound.Size.X / 2,
        y: this.Bound.Pos.Y + this.Bound.Size.Y / 2,
        text: this.Text,
        align: "center",
        baseline: "middle"
    });
}
