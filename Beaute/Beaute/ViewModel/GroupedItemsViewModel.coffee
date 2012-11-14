# must have
class GroupedItemsViewModel
	constructor: (@opts) ->		
		@FirstName = ko.observable()	
		@Age = ko.observable()
		@UserRating = ko.observable 2
		@ChangeSomething = ->
			@Age = 27
			@FirstName (new Date()).toString()
		@Examine = ->
			@UserRating @UserRating()+1