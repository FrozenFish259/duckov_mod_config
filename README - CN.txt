[h1]中文说明[/h1]
[list]
[*][i]ModConfig[/i] 是一个为其他Mod提供便捷的游戏内配置参数调整的框架。玩家能够直接在游戏内通过设置菜单来调整其他Mod的各个配置参数。
[*]Mod设置菜单需要进入游戏场景后再打开设置才能看到, 主菜单中设置里暂不可见。
[*]第一次启用或重新开关本mod后可能需要重启游戏以生效。
[/list]

[img]https://images.steamusercontent.com/ugc/18308003542399829868/2FA2DF264BAF657E99EB727C0372E10BE4669CBC/?imw=5000&imh=5000&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=false[/img]
[h2]支持的mod[/h2]
[list]
[*][url=https://steamcommunity.com/sharedfiles/filedetails/?id=3588412062]KillFeed[/url] 一款显示击杀记录的mod
[/list]

[img]./20251020163234_1.jpg[/img]

[h2]配置文件位置[/h2]

如果你想重置mod配置, 可以删除下面的文件
[quote]
%USERPROFILE%\AppData\LocalLow\TeamSoda\Duckov\Saves\ModConfig.ES3
[/quote]

[h1]开发指南[/h1]

[h2]步骤[/h2]
[olist]
[*]从[url=https://github.com/FrozenFish259/duckov_mod_config]Github Project[/url]下载并拷贝 [i]ModConfigApi.cs[/i] 至你的项目中, 方便调用 [i]ModConfig[/i] 函数
[*]务必在配置读写相关逻辑前, 务必调用一次[i]ModConfigAPI.Initialize()[/i]以验证是否能够正常与 [i]ModConfig[/i] 通信
[*]参考我的[url=https://github.com/FrozenFish259/duckov_mod_config_example]演示项目[/url]
[/olist]

[h2]支持的数据类型[/h2]
[list]
[*]bool
[*]int
[*]float
[*]string
[/list]

[h2]注意事项[/h2]
[list]
[*]配置项数值是通过 [i]OptionsManager_Mod.Load<T>(string key, T defaultV)[/i] 读取的
[*][i]ModConfig[/i]实现逻辑较复杂, 不排除未来游戏某次更新后会导致其失效并需要重新维护的可能性, 因为有了下面这条注意事项
[*]禁止将你的mod配置读写逻辑与 [i]ModConfig[/i] 建立强依赖关系, 配置读写即使没有 [i]ModConfig[/i] 也必须满足正常功能
[/list]

[h2]Mod发布[/h2]

在上传你的mod后, 在创意工坊页面中点击 [i]添加/移除必需物品[/i] 后添加对 [i]ModConfig[/i] 的依赖即可

[h2]源码参考[/h2]

[url=https://github.com/FrozenFish259/duckov_mod_config]Github源码[/url]
[url=https://github.com/FrozenFish259/duckov_mod_config_example]Github演示项目[/url]