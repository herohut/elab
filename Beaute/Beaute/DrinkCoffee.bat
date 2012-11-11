SET coffeePath=C:\Users\Thai\AppData\Roaming\npm\node_modules\coffee-script\bin

cd %coffeePath%

SET dir=%~dp0

node coffee -c -b %dir%Common\Global.coffee
node coffee -c -b %dir%ViewModel\HomeViewModel.coffee