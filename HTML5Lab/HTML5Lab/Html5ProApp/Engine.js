/// <reference path="../Html5Framework/UiBase.js" />
/// <reference path="../Html5Framework/Utils.js" />
/// <reference path="../Html5Framework/GeometryHelper.js" />
/// <reference path="Objects.js" />
/// <reference path="../Html5Framework/ButtonBase.js" />

// ENGINE
Engine = function (canvas) {
    UserControl.prototype.constructor.call(this, canvas);

    this.objects = [];
    this.connectors = [];
    this.draggingObj = EmptyControl;

    var dragging = false;
    var creatingConnector = false;
    this.newConnector = null;
    this.currentX = 0;
    this.currentY = 0;

    // INIT SAMPLE DATA
    var n = new NodeControl(this.Canvas, this.Context);
    n.Bound.Pos = new Vector2D(100, 150);
    n.Bound.Size = new Vector2D(160, 120);
    n.Text = "0";
    this.objects.push(n);

    n = new NodeControl(this.Canvas, this.Context);
    n.Bound.Pos = new Vector2D(250, 10);
    n.Bound.Size = new Vector2D(160, 120);
    n.Text = "1";
    this.objects.push(n);

    n = new NodeControl(this.Canvas, this.Context);
    n.Bound.Pos = new Vector2D(200, 300);
    n.Bound.Size = new Vector2D(160, 120);
    n.Text = "2";
    this.objects.push(n);


    this.connectors.push(new ConnectorControl(this.Canvas, this.Context));
    this.connectors[0].Node1 = this.objects[0];
    this.connectors[0].Node2 = this.objects[1];

    this.connectors.push(new ConnectorControl(this.Canvas, this.Context));
    this.connectors[1].Node1 = this.objects[0];
    this.connectors[1].Node2 = this.objects[2];


    var btn = new ButtonBase(this.Canvas, this.Context);
    btn.Bound.Pos = new Vector2D(20, 40);
    btn.Bound.Size = new Vector2D(100, 40);
    btn.Text = "BaseButton";
    this.Controls.push(btn);

    var ref = this;


    $(canvas).mousedown(function (a) {
        ref.mousedown(a);
    });
    $(canvas).mousemove(this, function (a) {
        ref.mousemove(a);
    });

    $(canvas).mouseup(function (a) {
        ref.mouseup(a);
    });


    this.mousedown = function (a) {
        this.currentX = a.offsetX;
        this.currentY = a.offsetY;
        var checkDrag = EmptyControl;

        for (var i = 0; i < this.objects.length; i++) {
            var obj = this.objects[i];
            if (checkDrag != EmptyControl) {
                this.objects[i].dragging = false;
                continue;
            }

            var cp = this.objects[i].checkPoint(a.offsetX, a.offsetY);
            if (cp == "in_connector") {
                this.newConnector = new ConnectorControl(this.Canvas, this.Context);
                this.newConnector.Node1 = this.objects[i];
                this.newConnector.Creating = true;
                this.connectors.push(this.newConnector);
                this.Canvas.style.cursor = "pointer";
            }

            else if (cp == "in_normal") {
                this.objects[i].dragging = true;
                this.Canvas.style.cursor = "move";
            }
            if (this.objects[i].dragging) {
                checkDrag = this.objects[i];
            }
        }

        this.draggingObj = checkDrag;
    }

    this.mousemove = function (a) {
        if (this.draggingObj != EmptyControl) {
            var deltaX = a.offsetX - this.currentX;
            var deltaY = a.offsetY - this.currentY;
            this.currentX = a.offsetX;
            this.currentY = a.offsetY;
            this.draggingObj.Bound.Pos.X = this.draggingObj.Bound.Pos.X + deltaX;
            this.draggingObj.Bound.Pos.Y = this.draggingObj.Bound.Pos.Y + deltaY;

            this.draw();
            return;
        }
        if (this.newConnector != null) {
            this.newConnector.updatePosition1(a.offsetX, a.offsetY);
            this.draw();
            return;
        }

        var eventArgs = EventArgs;
        eventArgs.Cancel = false;
        for (var i = 0; i < this.objects.length; i++) {
            this.objects[i].onmousemove(a, eventArgs);
        }

        this.draw();
    }


    this.mouseup = function (a) {
        canvas.style.cursor = "default";
        if (this.draggingObj != undefined) {
            this.draggingObj.dragging = false;
            this.draggingObj = EmptyControl;
            this.draw();
        }
        if (this.newConnector != null) {

            this.newConnector.Creating = false;
            for (var i = 0; i < this.objects.length; i++) {
                if (this.objects[i].checkPoint(a.offsetX, a.offsetY) != "out"
                 && this.newConnector.Node1 != this.objects[i]
                && !this.existConnector(this.newConnector.Node1, this.objects[i])) {
                    this.newConnector.Node2 = this.objects[i];
                    break;
                }
            }

            if (this.newConnector.Node2 == null) {
                this.connectors.remove(this.newConnector);
            }

            this.newConnector = null;
            this.draw();
        }
    }

    this.existConnector = function (node1, node2) {
        for (var i = 0; i < this.connectors.length; i++) {
            var n1 = this.connectors[i].Node1;
            var n2 = this.connectors[i].Node2;
            if ((n1 == node1 && n2 == node2) || (n1 == node2 && n2 == node1))
                return true;
        }
        return false;
    }

    this.addObjectRandom = function () {
        var n = new NodeControl(this.Canvas, this.Context);

        n.Bound.Pos.X = Math.random() * (this.Canvas.width - 50);
        n.Bound.Pos.Y = Math.random() * (this.Canvas.height - 50);
        n.Bound.Size = new Vector2D(160, 120);
        n.Text = (this.objects.length + 1).toString();
        this.objects.push(n);
        this.draw();
    };

}

Engine.prototype = new UserControl();

Engine.prototype.draw = function () {    
    this.Context.font = "bold 10pt Tahoma";
    this.Context.clearRect(0, 0, canvas.width, canvas.height);

    this.Context.strokeStyle = "gray";
    this.Context.lineWidth = 2;
    this.Context.strokeRect(1, 1, canvas.width - 2, canvas.height - 2);
    this.Context.strokeStyle = "violet";

    this.Context.fillStyle = "red";
    this.Context.fillText("HERO'S LAB: HTML5", 7, 14);

    UserControl.prototype.draw.call(this);

    for (var i = 0; i < this.connectors.length; i++) {
        this.connectors[i].updatePosition();
    }

    for (var i = 0; i < this.objects.length; i++) {
        this.objects[i].draw();
    }
    for (var i = 0; i < this.connectors.length; i++) {
        this.connectors[i].draw();
    }
}
