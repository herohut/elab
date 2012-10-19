SET coffee=C:\Users\HERO\node_modules\coffee-script\bin

SET dir=%~dp0

CD %coffee%


node coffee -c -b %dir%EntryRenderer.coffee
node coffee -c -b %dir%HomePresenter.coffee