/// <reference path="GeometryHelper.js" />


Vector2D = function (x, y) {
    this.X = x;
    this.Y = y;
}
Vector2D.prototype.toString = function () {
    return this.X.toString() + ":" + this.Y.toString()
}

EventArgs = function () {
    this.Cancel = false;
}

Bound = function () {
    this.X = function () {
        return this.Pos.X;
    }
    this.Y = function () {
        return this.Pos.Y;
    }
    this.Width = function () {
        return this.Size.X;
    }
    this.Height = function () {
        return this.Size.Y;
    }
    this.Pos = new Vector2D();
    this.Size = new Vector2D();


    this.Right = function () {
        return this.X() + this.Width();
    }
    this.Bottom = function () {
        return this.Y() + this.Height();
    }
    this.Center = function () {
        return new Vector2D(this.X() + this.Width() / 2, this.Y() + this.Height() / 2);
    }
}
Bound.prototype.toString = function () {
    return this.X.toString() + ":" + this.Y.toString()
}


// UserControl
UserControl = function (canvas) {
    this.Bound = new Bound();
    this.Name = undefined;
    this.Canvas = canvas;
    this.Context = undefined;
    this.Text = undefined;
    this.dragging = false;
    this.EnableTooltip = false;
    this.ShowTooltip = false;
    this.Controls = [];
}

EmptyControl = new UserControl();

UserControl.prototype.onmousemove = function (a, eventArgs) {
    eventArgs.Cancel = false;

    // Tooltip
    if (this.EnableTooltip && GeometryHelper.isPointInsideBound(this.Bound, a.offsetX, a.offsetY))
        this.ShowTooltip = true;
    else
        this.ShowTooltip = false;

    // Raise children
    for (var i = 0; i < this.Controls.length; i++) {
        this.Controls[i].onmousemove(a, eventArgs);
    }
}

UserControl.prototype.onmousedown = function (a, eventArgs) {
    eventArgs.Cancel = false;
    for (var i = 0; i < this.Controls.length; i++) {
        this.Controls[i].onmousedown(a, eventArgs);
    }
}

UserControl.prototype.onmouseup = function (a, eventArgs) {
    eventArgs.Cancel = false;
    for (var i = 0; i < this.Controls.length; i++) {
        this.Controls[i].onmouseup(a, eventArgs);
    }
}

UserControl.prototype.onmouseleave = function (a, eventArgs) {
    eventArgs.Cancel = false;
    this.ShowTooltip = false;
    for (var i = 0; i < this.Controls.length; i++) {
        this.Controls[i].onmouseleave(a, eventArgs);
    }
}

UserControl.prototype.onmouseenter = function (a, eventArgs) {
    eventArgs.Cancel = false;
    for (var i = 0; i < this.Controls.length; i++) {
        this.Controls[i].onmouseenter(a, eventArgs);
    }
}

UserControl.prototype.draw = function (args) {
    for (var i = 0; i < this.Controls.length; i++) {
        this.Controls[i].draw(args);
    }

    for (var i = 0; i < this.Controls.length; i++) {
        if (!this.Controls[i].EnableTooltip || !this.Controls[i].ShowTooltip)
            continue;
        this.Controls[i].drawTooltip(args);
    }
}

UserControl.prototype.drawTooltip = function (args) {

}
