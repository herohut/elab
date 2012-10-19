/// <reference path="UiBase.js" />
/// <reference path="Utils.js" />

ButtonBase = function (canvas, context) {
    UserControl.prototype.constructor.call(this, canvas, context);
}
ButtonBase.prototype = new UserControl();

ButtonBase.prototype.draw = function () {
    UserControl.prototype.draw.call(this);
    this.Context.fillStyle = "yellow";
    this.Context.roundedRect(this.Bound.Pos.X, this.Bound.Pos.Y, this.Bound.Size.X, this.Bound.Size.Y, 3, true, true);

    // text-center
    this.Context.fillStyle = "black";
    var tWidth = this.Context.measureText(this.Text);
    this.Context.textBaseline = "middle";
    this.Context.fillText(this.Text, this.Bound.Pos.X + this.Bound.Size.X / 2 - tWidth.width / 2, this.Bound.Pos.Y + this.Bound.Size.Y / 2);
}
