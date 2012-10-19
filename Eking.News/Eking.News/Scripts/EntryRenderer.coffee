# EntryRenderer


class EntryRenderer
	rectW = 124
	rectH = 86
	
	###

	###
	
	isVertical = true

	constructor: (@options)->
		# ...
		@container = $(@options.container)
		@items = []

		for i in @container.children '.entry'
			$i = $(i)
			@items.push 
				element: $i
				renderSize: $i.attr 'data-renderSize'		
				id: $i.attr 'data-Id'

	arrange:->
		@smartArrange()
		#@simpleArrange()
	simpleArrange:->
		x = 0
		y = 0
		maxY = y
		yRange = 0
		width = @container.width()
		for item in @items
			size = getSize 2,4			
			if x + size.x > width
				x = 0
				y = size.y + maxY	
			yRange = y+size.y	

			bound = {
				w:size.x
				h:size.y
				x:x
				y:y	
			}		
			
			item.element.css 'width',"#{bound.w}px"
			item.element.css 'height',"#{bound.h}px"
			item.element.css 'left',"#{bound.x}px"
			item.element.css 'top',"#{bound.y}px"			

			x = x + size.x

			if y > maxY
				maxY = y 		
				yRange = maxY+size.y

		@container.css 'height',  "#{yRange}px"
	resetcellMap=(arr)->
		for i in [0..arr.x-1]
			for j in [0..arr.y-1]
				arr[i][j] = false

	printCellMap = (map)=>
		output = ""
		output = output.appendLine(map.x)
		output = output.appendLine(map.y)

		output = output.appendLine("")

		for i in [0..map.x-1]
			output = output.appendWithTab(i)

		for y in [0..map.y-1]
			output = output.appendLine("")
			output = output.append(y)
			for x in [0..map.x-1]				
				txt = if map[x][y] then "X" else "-"
				output = output.appendWithTab(txt)

		return output
		

	smartArrange:->
		x = 0
		y = 0
		width = @container.width()
		
		colCount = Math.floor width/rectW
		rowCount = 4

		cellMap = create2DArray colCount, rowCount, false
		blockBottom = 0

		for item in @items
			txt = printCellMap(cellMap)

			coeff = getSizeCoeff item.renderSize
			pos = findPositionMarkTrue cellMap, coeff

			if pos is undefined				
				resetcellMap cellMap
				pos = findPositionMarkTrue cellMap, coeff
				y = blockBottom			

			bound = {
				w:coeff.x*rectW
				h:coeff.y*rectH
				x:x+pos.x*rectW
				y:y+pos.y*rectH	
			}			
			
			item.element.css 'width',"#{bound.w}px"
			item.element.css 'height',"#{bound.h}px"
			item.element.css 'left',"#{bound.x}px"
			item.element.css 'top',"#{bound.y}px"	

			chk = bound.y+bound.h

			if chk>blockBottom
				blockBottom = chk		

		@container.css 'height',  "#{blockBottom}px"	

	

	create2DArray = (x,y, defaultVal)->
		output = []
		for i in [0..x-1]
			rows = []
			for j in [0..y-1]
				rows.push defaultVal
			output.push rows
		output.x = x
		output.y = y

		return output		


	findPosition = (cellMap, itemCoeff)->		
		for i in [0..cellMap.x-1]
			for j in [0..cellMap.y-1]

				# CHECK FREE CELL
				if cellMap[i][j] is true
					continue
				if i+itemCoeff.x>cellMap.x
					continue
				if j+itemCoeff.y>cellMap.y
					continue

				# CHECK RECT THAT CAN STORE ITEM
				breaki1 = false
				for i1 in [i..i+itemCoeff.x-1]															
					for j1 in [j..j+itemCoeff.y-1]						
						if cellMap[i1][j1] is true
							breaki1 = true
							break

					if breaki1
						break

				if breaki1 is false
					return {
						x:i
						y:j
					}
		return undefined
	findPositionMarkTrue = (cellMap, itemCoeff)->
		found = findPosition cellMap, itemCoeff
		if found is undefined
			return undefined

		for i in [found.x..found.x+itemCoeff.x-1]
			for j in [found.y..found.y+itemCoeff.y-1]				
				cellMap[i][j] = true


		return found

	getSize = (x,y)->
		return {
			x: rectW*x
			y: rectH*y
		}



	getItemSize = (input)->
		if input.renderSize is undefined
			return	{
				x: rectW
				y: rectH
			}
		
		coeff = getSizeCoeff input.renderSize
		getSize coeff.x,coeff.y

	getSizeCoeff = (txt)->
		arr = txt.split 'x'
		return {
			x:parseInt(arr[0])
			y:parseInt(arr[1])
		}

# in Util later
String.prototype.appendLine = (txt)->
	@+"\r\n"+txt
String.prototype.append = (txt)->
	@+txt
String.prototype.appendWithTab =(txt)->
	@+"\t"+txt