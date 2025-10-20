# 中文说明
*  `ModConfig` 是一个为其他Mod提供便捷的游戏内配置参数调整的框架。玩家能够直接在游戏内通过设置菜单来调整其他Mod的各个配置参数。  
* Mod设置菜单需要进入游戏场景后再打开设置才能看到, 主菜单中设置里暂不可见。  
* 第一次启用或重新开关本mod后可能需要重启游戏以生效。

## 开发指南

### 步骤
1. 从[Github Project](https://github.com/FrozenFish259/duckov_mod_config)下载并拷贝 `ModConfigApi.cs` 至你的项目中, 方便调用 `ModConfig` 函数
2. 务必在配置读写相关逻辑前, 务必调用一次`ModConfigAPI.Initialize()`以验证是否能够正常与 `ModConfig` 通信

### 支持的数据类型
* bool
* int
* float
* string

### 注意事项
* 配置项数值是通过 `OptionsManager_Mod.Load<T>(string key, T defaultV)` 读取的
* `ModConfig`实现逻辑较复杂, 不排除未来游戏某次更新后会导致其失效并需要重新维护的可能性, 因为有了下面这条注意事项
* 禁止将你的mod配置读写逻辑与 `ModConfig` 建立强依赖关系, 配置读写即使没有 `ModConfig` 也必须满足正常功能

### Mod发布
在上传你的mod后, 在创意工坊页面中点击 `添加/移除必需物品` 后添加对 `ModConfig` 的依赖即可

### 源码参考
[Github项目](https://github.com/FrozenFish259/duckov_mod_config)  


# English Documentation

* `ModConfig` is a framework that provides in-game configuration parameter adjustment for other mods.It allows players to directly adjust various configuration parameters of other mods through the in-game settings menu.  
* `Mod Settings` menu currently can only be accessed from ingame scenes, not from MainMenu  
* You may need to restart game after enabling `ModConfig`

## Development Guide

### Steps
1. Download `ModConfigApi.cs` from [Github Project](https://github.com/FrozenFish259/duckov_mod_config) and copy it to your project for calling `ModConfig` functions
2. Before configuration read/write logic, be sure to call `ModConfigAPI.Initialize()` once to verify normal communication with `ModConfig`

### Supported Data Types
* bool
* int
* float
* string

### Important Notes
* Configuration values are read through `OptionsManager_Mod.Load<T>(string key, T defaultV)`
* The implementation logic of `ModConfig` is relatively complex. There is a possibility that future game updates may broke `ModConfig` and require maintenance. Therefore, please note the following important tip
* Do not create strong dependencies between your mod's configuration read/write logic and `ModConfig`. Your configuration read/write must function normally even without `ModConfig`

### Mod Publishing
After uploading your mod, go to workshop item page and click `Add/Remove Required Items` , add `ModConfig` as its dependency

### Source Code Reference
[Github Project](https://github.com/FrozenFish259/duckov_mod_config)