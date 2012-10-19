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

    this.Context.lineWidth = 2;
    this.Context.lineStyle = "red";
    this.Context.fillStyle = "white";
    this.Context.strokeStyle = this.borderStroke;
    this.Context.roundedRect(this.Bound.Pos.X, this.Bound.Pos.Y, this.Bound.Size.X, this.Bound.Size.Y, 6);

    // circle-1
    this.Context.strokeStyle = "gray";
    this.Context.lineWidth = 1;
    this.Context.strokeCircle(this.Bound.Pos.X + this.Bound.Size.X / 2, this.Bound.Pos.Y + this.Bound.Size.Y / 2, this.circle1R);

    // circle-2
    this.Context.strokeStyle = "violet";
    this.Context.lineWidth = 2;
    this.Context.strokeCircle(this.Bound.Pos.X + this.Bound.Size.X / 2, this.Bound.Pos.Y + this.Bound.Size.Y / 2, this.circle2R);

    // circle-3
    this.Context.fillStyle = "violet";
    this.Context.fillCircle(this.Bound.Pos.X + this.Bound.Size.X / 2, this.Bound.Pos.Y + this.Bound.Size.Y / 2, this.circle3R);

    // text-center
    this.Context.fillStyle = "white";
    var tWidth = this.Context.measureText(this.Text);
    this.Context.textBaseline = "middle";
    this.Context.fillText(this.Text, this.Bound.Pos.X + this.Bound.Size.X / 2 - tWidth.width / 2, this.Bound.Pos.Y + this.Bound.Size.Y / 2);
}
