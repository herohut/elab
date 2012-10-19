// HELPER
CanvasRenderingContext2D.prototype.fillCircle = function (x, y, r) {
    this.beginPath();
    this.arc(x, y, r, 0, (Math.PI / 180) * 360);
    this.closePath();
    this.fill();
}

CanvasRenderingContext2D.prototype.strokeCircle = function (x, y, r) {
    this.beginPath();
    this.arc(x, y, r, 0, (Math.PI / 180) * 360);
    this.closePath();
    this.stroke();
}

CanvasRenderingContext2D.prototype.strokeLine = function (x1, y1, x2, y2) {
    this.beginPath();
    this.moveTo(x1, y1);
    this.lineTo(x2, y2);
    this.closePath();
    this.stroke();
}

CanvasRenderingContext2D.prototype.fillLine = function (x1, y1, x2, y2) {


}

CanvasRenderingContext2D.prototype.roundedRect = function (x, y, width, height, radius, fill, stroke) {
    if (typeof stroke == "undefined") {
        stroke = true;
    }
    if (typeof radius === "undefined") {
        radius = 5;
    }
    this.beginPath();
    this.moveTo(x + radius, y);
    this.lineTo(x + width - radius, y);
    this.quadraticCurveTo(x + width, y, x + width, y + radius);
    this.lineTo(x + width, y + height - radius);
    this.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
    this.lineTo(x + radius, y + height);
    this.quadraticCurveTo(x, y + height, x, y + height - radius);
    this.lineTo(x, y + radius);
    this.quadraticCurveTo(x, y, x + radius, y);
    this.closePath();
    if (stroke) {
        this.stroke();
    }
    if (fill) {
        this.fill();
    }
}

Array.prototype.remove = function (s) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == s)
            this.splice(i, 1);
    }
}

Utils = function () {
    this.log = function (text) {
        console.log(text);
    }
}

var Utils = new Utils();