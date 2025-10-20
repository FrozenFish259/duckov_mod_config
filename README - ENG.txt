[h1]English Documentation[/h1]
[list]
[*][i]ModConfig[/i] is a framework that provides in-game configuration parameter adjustment for other mods.It allows players to directly adjust various configuration parameters of other mods through the in-game settings menu.
[*][i]Mod Settings[/i] menu currently can only be accessed from ingame scenes, not from MainMenu
[*]You may need to restart game after enabling [i]ModConfig[/i]
[/list]

[h2]Development Guide[/h2]

[h3]Steps[/h3]
[olist]
[*]Download [i]ModConfigApi.cs[/i] from [url=https://github.com/FrozenFish259/duckov_mod_config]Github Project[/url] and copy it to your project for calling [i]ModConfig[/i] functions
[*]Before configuration read/write logic, be sure to call [i]ModConfigAPI.Initialize()[/i] once to verify normal communication with [i]ModConfig[/i]
[/olist]

[h3]Important Notes[/h3]
[list]
[*]Configuration values are read through [i]OptionsManager.Load<T>(string key, T defaultV)[/i]
[*]The implementation logic of [i]ModConfig[/i] is relatively complex. There is a possibility that future game updates may broke [i]ModConfig[/i] and require maintenance. Therefore, please note the following important tip
[*]Do not create strong dependencies between your mod's configuration read/write logic and [i]ModConfig[/i]. Your configuration read/write must function normally even without [i]ModConfig[/i]
[/list]

[h3]Mod Publishing[/h3]

After uploading your mod, go to workshop item page and click [i]Add/Remove Required Items[/i] , add [i]ModConfig[/i] as its dependency

[h3]Source Code Reference[/h3]

[url=https://github.com/FrozenFish259/duckov_mod_config]Github Project[/url]
