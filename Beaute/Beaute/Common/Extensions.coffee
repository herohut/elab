# must have
ko.bindingHandlers.winJsUserRating = 
	init:(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)->
		element.winControl.onchange = ()->
			valueAccessor() element.winControl.userRating


	update:(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)->
		element.winControl.userRating =  ko.utils.unwrapObservable valueAccessor()

