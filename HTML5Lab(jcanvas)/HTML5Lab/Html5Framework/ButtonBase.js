/// <reference path="UiBase.js" />
/// <reference path="Utils.js" />

ButtonBase = function (canvas, context) {
    UserControl.prototype.constructor.call(this, canvas, context);
}
ButtonBase.prototype = new UserControl();

ButtonBase.prototype.draw = function () {
    UserControl.prototype.draw.call(this);

    this.Canvas.drawRect({
        fromCenter: false,
        fillStyle: "yellow",
        strokeStyle: "violet",
        strokeWidth: 1,                        
        x: this.Bound.Pos.X,
        y: this.Bound.Pos.Y,
        width: this.Bound.Size.X,
        height: this.Bound.Size.Y,
        cornerRadius: 3
    });

    this.Canvas.drawText({
        fillStyle: "black",
        x: this.Bound.Pos.X + this.Bound.Size.X / 2,
        y: this.Bound.Pos.Y + this.Bound.Size.Y / 2,
        text: this.Text,
        align: "center",
        baseline: "middle"
    });

}
