[h1]English Documentation[/h1]
[list]
[*][i]ModConfig[/i] is a framework that provides in-game configuration parameter adjustment for other mods.It allows players to directly adjust various configuration parameters of other mods through the in-game settings menu.
[*][i]Mod Settings[/i] menu currently can only be accessed from ingame scenes, not from MainMenu
[*]You may need to restart game after enabling [i]ModConfig[/i]
[/list]

[img]https://images.steamusercontent.com/ugc/18308003542399829868/2FA2DF264BAF657E99EB727C0372E10BE4669CBC/?imw=5000&imh=5000&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=false[/img]
[h2]Supported mods[/h2]
[list]
[*][url=https://steamcommunity.com/sharedfiles/filedetails/?id=3588412062]KillFeed[/url] shows killfeed
[/list]

[h2]Mod Settings File Location[/h2]

Delete following file if you wanna reset ModConfig's mod settings
[quote]
%USERPROFILE%\AppData\LocalLow\TeamSoda\Duckov\Saves\ModConfig.ES3
[/quote]

[h1]Development Guide[/h1]

[h2]Steps[/h2]
[olist]
[*]Download [i]ModConfigApi.cs[/i] from [url=https://github.com/FrozenFish259/duckov_mod_config]Github Project[/url] and copy it to your project for calling [i]ModConfig[/i] functions
[*]Before configuration read/write logic, be sure to call [i]ModConfigAPI.Initialize()[/i] once to verify normal communication with [i]ModConfig[/i]
[*]Check out my [url=https://github.com/FrozenFish259/duckov_mod_config_example]example project[/url]
[/olist]

[h2]Supported Data Types[/h2]
[list]
[*]bool
[*]int
[*]float
[*]string
[/list]

[h2]Important Notes[/h2]
[list]
[*]Configuration values are read through [i]OptionsManager_Mod.Load<T>(string key, T defaultV)[/i]
[*]The implementation logic of [i]ModConfig[/i] is relatively complex. There is a possibility that future game updates may broke [i]ModConfig[/i] and require maintenance. Therefore, please note the following important tip
[*]Do not create strong dependencies between your mod's configuration read/write logic and [i]ModConfig[/i]. Your configuration read/write must function normally even without [i]ModConfig[/i]
[/list]

[h2]Mod Publishing[/h2]

After uploading your mod, go to workshop item page and click [i]Add/Remove Required Items[/i] , add [i]ModConfig[/i] as its dependency

[h2]Source Code Reference[/h2]

[url=https://github.com/FrozenFish259/duckov_mod_config]Github Source[/url]
[url=https://github.com/FrozenFish259/duckov_mod_config_example]Github Example Project[/url]
