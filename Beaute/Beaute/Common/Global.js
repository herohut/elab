// Generated by CoffeeScript 1.4.0
var Global,
  _this = this;

Global = (function() {

  function Global() {}

  Global.prototype.log = function(text) {};

  Global.prototype.TestSmth = function() {
    var bindingsource, control, data,
      _this = this;
    data = {
      Name: "Hero"
    };
    control = $('#test')[0];
    WinJS.Binding.processAll(control, data);
    control = $('#txtValue')[0];
    WinJS.Binding.processAll(control, data);
    bindingsource = WinJS.Binding.as(data);
    return $('#btn').click(function(evt) {
      return bindingsource.Name = "HUT";
    });
  };

  return Global;

})();

Global = new Global();

WinJS.Namespace.define("Binding.Mode", {
  twoway: WinJS.Binding.initializer(function(source, sourceProps, dest, destProps) {
    WinJS.Binding.defaultBind(source, sourceProps, dest, destProps);
    return dest.onchange = function() {
      var d, s;
      d = dest[destProps[0]];
      s = source[sourceProps[0]];
      if (s !== d) {
        source[sourceProps[0]] = d;
      }
    };
  })
});
