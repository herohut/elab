# HomePresenter
class HomePresenter
	constructor: ->				

		$('.screen').resize =>
    	    arrange()
    	

	    installGroupPage = ()->
	    	arrange()
    		installjScrollPane()
	    	$('.moreButton').click ()->	    		
	    		$this = $(this)
	    		groupId = $this.parent().attr 'data-groupid'
	    		page = parseInt($this.attr 'data-nextPage')

	    		$.ajax
	    			url:"/Nouvelle/GetEntries?groupId=#{groupId}&page=#{page}"
	    			success: (evt)->    	
	    				if evt is ''
	    					$this.children().html 'FIN'
	    					return

		    			$this.before evt	    			
		    			$this.parent().find('.entries').each (index,item) =>
		    				arrangeEntries item
		    			$this.attr 'data-nextPage', page+1
		    			$this.parent().find('.nailthumb-container').nailthumb()
		    		error:(evt)->
		    			alert "Error:#{evt}"
		    		fail:(evt)->
		    			alert "Fail:#{evt}"	   

		    $('.nailthumb-container').nailthumb()

	    cached = {}
	    $(window).bind 'hashchange',(e)->
	    	url = $.param.fragment()
	    	
		    frags = $.deparam.fragment()
		    view = frags.view
		    id=frags.id
		    
		    # HOME URL
		    if url is ''
		    	if cached[url]
		    		$('#main').html cached[url]		    		
		    		installGroupPage()

		    		return

		    	$.ajax
		    		url:"/Nouvelle/GroupsView"
		    		success: (evt)->    	
		    			$('#main').html evt
		    			cached[url] = evt
		    			installGroupPage()

		    		error:(evt)->
		    			alert "Error:#{evt}"
		    		fail:(evt)->
		    			alert "Fail:#{evt}"
		    	
		    	return

		    if view is undefined
		    	throw 'Unknown view'
		   
		    # GROUP URL
		    if view is 'group'
		    	if cached[url]
		    		$('#main').html cached[url]
		    		installGroupPage()
		    		return

		    	$.ajax
		    		url:"/Nouvelle/GroupsView?groupIds=#{id}"
		    		success: (evt)->    	
		    			$('#main').html evt
		    			cached[url] = evt
		    			installGroupPage()

		    		error:(evt)->
		    			alert "Error:#{evt}"
		    		fail:(evt)->
		    			alert "Fail:#{evt}"

		     # ENTRY URL
		    if view is 'entry'
		    	if cached[url]
		    		$('#main').html cached[url]
		    		installjScrollPane()
		    		new BreadCrumManager({breadItem: '.entryBreadCrumb'})
		    		return

		    	id = parseInt(id)
	    		$.ajax
		    		url:"/Nouvelle/EntryView?id=#{id}"
		    		success: (evt)->    	
		    			$('#main').html evt
		    			installjScrollPane()
		    			new BreadCrumManager({breadItem: '.entryBreadCrumb'})
		    			cached[url] = evt		    			

		    		error:(evt)->
		    			alert "Error:#{evt}"
		    		fail:(evt)->
		    			alert "Fail:#{evt}"


		$(window).trigger 'hashchange'
    
	installjScrollPane = ()->				
		$('.jScrollPane').jScrollPane 
	    	showArrows: true
	    	autoReinitialise: true
	    	hideFocus:true

	arrange = ()->
		$('.entries').each (index, item) =>
			arrangeEntries item		

	arrangeEntries = (entries) ->
	    renderer = new EntryRenderer
            container: entries
        renderer.arrange()

class BreadCrumManager
	$bread = undefined
	constructor:(@attrs)->
		$bread = $(@attrs.breadItem)

		children = $bread.children()
		for i in [0..children.length]
			handleChild($(children[i]),{breadItemCount: children.length, index: i})
		


	handleChild = (child,opts)->
		text = child.html()
		child.html(
			"<span class='breadcrumbIcon breadcrumbIconLeft'></span>
			<span class='breadcrumbText'>#{text}</span>
			<span class='breadcrumbIcon breadcrumbIconRight'></span>
			")
		child.mouseenter (evt)->

			tmp = child.find '.breadcrumbIconLeft'			
			tmp.removeClass()			
			tmp.addClass 'breadcrumbIcon breadcrumbIconLeft breadcrumbIconLeftHover'
			
			tmp = child.find '.breadcrumbIconRight'
			tmp.removeClass()
			tmp.addClass 'breadcrumbIcon breadcrumbIconRight breadcrumbIconRightHover'

			tmp = child.find('.breadcrumbText')
			tmp.removeClass()
			tmp.addClass 'breadcrumbText breadcrumbTextHover'

		child.mouseleave (evt)->
			child.find('*').removeClass 'breadcrumbIconLeftHover breadcrumbIconRightHover breadcrumbTextHover'