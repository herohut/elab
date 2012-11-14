cd %coffeePath%

"%nodejs%\node" coffee -c -b %dir%Common\Global.coffee
"%nodejs%\node" coffee -c -b %dir%ViewModel\GroupedItemsViewModel.coffee
"%nodejs%\node" coffee -c -b %dir%Common\Extensions.coffee