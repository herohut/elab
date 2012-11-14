# must have
class GroupedItemsViewModel
	constructor: (@opts) ->
		# ...
		@FirstName = ko.observable()	
		@Age = ko.observable()
		@ChangeSomething = ()->
			@Age = 27
			@FirstName (new Date()).toString()
		@Examine = ()->
			@FirstName "Dang Thai Hung"