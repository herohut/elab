/// <reference path="../Lib/datetime.1-3.js" />
/// <reference path="../../Lib/datetime.1-3.js" />


ChatMessageViewModel = function (from, to, content, time) {
    this.From = from;
    this.To = to;
    this.Content = content;
    this.Time = time;
}

var ChatMessageViewModelStatic = new TickDataViewModel();