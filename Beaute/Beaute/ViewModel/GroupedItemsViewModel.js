// Generated by CoffeeScript 1.4.0
var GroupedItemsViewModel;

GroupedItemsViewModel = (function() {

  function GroupedItemsViewModel(opts) {
    this.opts = opts;
    this.FirstName = ko.observable();
    this.Age = ko.observable();
    this.UserRating = ko.observable(2);
    this.ChangeSomething = function() {
      this.Age = 27;
      return this.FirstName((new Date()).toString());
    };
    this.Examine = function() {
      return this.UserRating(this.UserRating() + 1);
    };
  }

  return GroupedItemsViewModel;

})();
