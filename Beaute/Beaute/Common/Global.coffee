# Must have
class Global
	log:(text)->

	TestSmth:()->
		data = {Name:"Hero"}
		#control = document.getElementById "test"
		control = $('#test')[0]
		WinJS.Binding.processAll control, data

		control = $('#txtValue')[0]
		WinJS.Binding.processAll control, data


		bindingsource = WinJS.Binding.as data

		$('#btn').click (evt) =>
			bindingsource.Name = "HUT"

			


Global = new Global()


WinJS.Namespace.define "Binding.Mode", {
                twoway: WinJS.Binding.initializer  (source, sourceProps, dest, destProps) =>
                    WinJS.Binding.defaultBind(source, sourceProps, dest, destProps)
                    dest.onchange = () =>
                        d = dest[destProps[0]]
                        s = source[sourceProps[0]]
                        if s isnt d
                        	source[sourceProps[0]] = d
                        return
            }