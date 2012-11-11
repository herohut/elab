# HomeViewModel


class HomeViewModel
	constructor: () ->
		list = new WinJS.Binding.List()

		#@Items = @generateSampe()
		@ItemsSource = list.createGrouped(
			(item)=>item.GroupId,
			(item)=>item.Items)


		
	generateSampe: =>
		[
			{
				"GroupName":"Group1"
				"GroupId":"01"
				"Items":[
					{
						"Id":"01"
						"Title":"PBB"
						"Vote":"4"
					}
					{
						"Id":"02"
						"Title":"PB2"
						"Vote":"5"
					}
				]

			}

			{
				"GroupName":"Group2"
				"GroupId":"02"
				"Items":[
					{
						"Id":"01"
						"Title":"TV"
						"Vote":"0"
					}
					{
						"Id":"02"
						"Title":"TV02"
						"Vote":"5"
					}
				]

			}
		]
