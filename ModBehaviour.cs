using Duckov.Options.UI;
using Duckov.Utilities;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using Utilities;

namespace ModConfig
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private static bool createdModTab = false;

        private static OptionsPanel_TabButton? modTabButton = null;
        private static GameObject? modContent = null;

        //从游戏里克隆一个设置分辨率的游戏对象
        private static GameObject? dropdownListPrefab;
        //从游戏里克隆一个鼠标灵敏度的游戏对象
        //OptionsUIEntry_Slider
        private static GameObject? inputWithSliderPrefab;

        //待添加到modContent子节点的所有操作
        private static Queue<Action> pendingConfigActions = new Queue<Action>();

        /// <summary>
        /// 添加配置项的通用方法（支持延迟初始化）
        /// </summary>
        public static void AddConfig(Action configAction)
        {
            if (modTabButton != null && modContent != null)
            {
                // 确保在正确的主线程中执行
                configAction.Invoke();
            }
            else
            {
                pendingConfigActions.Enqueue(configAction);
                Debug.Log($"配置项已加入队列，等待mod菜单初始化。当前队列长度: {pendingConfigActions.Count}");
            }
        }

        /// <summary>
        /// 处理所有等待的配置项
        /// </summary>
        private void ProcessPendingConfigs()
        {
            if (modTabButton == null || modContent == null)
            {
                Debug.Log("modTabButton 或 modContent 尚未初始化，暂不处理配置项");
                return;
            }

            if (pendingConfigActions.Count > 0)
            {
                Debug.Log($"开始处理等待的配置项，剩余数量: {pendingConfigActions.Count}");

                // 安全地处理所有等待的配置项
                while (pendingConfigActions.Count > 0)
                {
                    var configAction = pendingConfigActions.Dequeue();
                    try
                    {
                        configAction.Invoke();
                        Debug.Log("配置项执行成功");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"执行配置项时出错: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
        }

        /// <summary>
        /// 下拉选项, 类似分辨率选择
        /// </summary>
        /// <param name="modName">模组名称</param>
        /// <param name="key"></param>
        /// <param name="description"></param>
        /// <param name="options"></param>
        /// <param name="valueType"></param>
        /// <param name="defaultValue"></param>
        public static void AddDropdownList(string modName, string key, string description, SortedDictionary<string, object> options, Type valueType, object defaultValue)
        {
            AddConfig(() => {
                // 安全检查
                if (dropdownListPrefab == null)
                {
                    Debug.LogError("dropdownListPrefab 为 null，无法创建下拉列表");
                    return;
                }

                if (modContent == null)
                {
                    Debug.LogError("modContent 为 null，无法添加下拉列表");
                    return;
                }

                try
                {
                    GameObject dropdownListPrefabClone = Instantiate(ModBehaviour.dropdownListPrefab);
                    dropdownListPrefabClone.SetActive(true);

                    // 设置描述
                    OptionsUIEntry_Dropdown dropdownUIEntry = dropdownListPrefabClone.GetComponent<OptionsUIEntry_Dropdown>();
                    if (dropdownUIEntry == null)
                    {
                        Debug.LogError("无法获取 OptionsUIEntry_Dropdown 组件");
                        Destroy(dropdownListPrefabClone);
                        return;
                    }

                    var label = ReflectionHelper.GetFieldValue<TextMeshProUGUI>(dropdownUIEntry, "label");
                    if (label != null)
                    {
                        label.SetText(description, true);
                    }

                    // 修改provider
                    DropDownOptionsProvider provider = dropdownUIEntry.AddComponent<DropDownOptionsProvider>();
                    provider.init(key, description, options, valueType, defaultValue);

                    ReflectionHelper.SetFieldValue(dropdownUIEntry, "provider", provider);

                    // 创建或查找mod标题
                    Transform modTitleTransform = modContent.transform.Find(modName);
                    if (modTitleTransform == null)
                    {
                        GameObject modNameTitleClone = Instantiate(ModBehaviour.dropdownListPrefab, modContent.transform);
                        modNameTitleClone.name = modName;
                        modNameTitleClone.transform.DestroyAllChildren();

                        // 创建标题文本
                        GameObject titleTextObject = new GameObject("TitleText");
                        titleTextObject.transform.SetParent(modNameTitleClone.transform);

                        TextMeshProUGUI titleText = titleTextObject.AddComponent<TextMeshProUGUI>();
                        titleText.SetText(modName);
                        titleText.margin = new Vector4(10, 10, 10, 10);

                        modNameTitleClone.SetActive(true);

                        modTitleTransform = modNameTitleClone.transform;
                    }

                    // 设置父级和顺序
                    dropdownListPrefabClone.transform.SetParent(modContent.transform, false);

                    int titleIndex = modTitleTransform.GetSiblingIndex();
                    dropdownListPrefabClone.transform.SetSiblingIndex(titleIndex + 1);
                    dropdownListPrefabClone.name = "UI_" + modName + "_" + key;

                    Debug.Log($"已成功添加下拉选项: {description}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建下拉列表时出错: {ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        /// <summary>
        /// 添加一个输入框, 可选是否带滑条
        /// </summary>
        /// <param name="modName">模组名称</param>
        /// <param name="key"></param>
        /// <param name="description"></param>
        /// <param name="valueType"></param>
        /// <param name="sliderRange">滑条范围, 如果不需要则填null</param>
        public static void AddInputWithSlider(string modName, string key, string description, Type valueType, Vector2? sliderRange = null)
        {
            //TODO: 待实现
            AddConfig(() => {
                // 安全检查
                if (inputWithSliderPrefab == null)
                {
                    Debug.LogError("inputWithSliderPrefab 为 null，无法创建滑条");
                    return;
                }

                if (dropdownListPrefab == null)
                {
                    Debug.LogError("dropdownListPrefab 为 null，无法创建滑条");
                    return;
                }

                if (modContent == null)
                {
                    Debug.LogError("modContent 为 null，无法添加下拉列表");
                    return;
                }

                try
                {
                    GameObject inputWithSliderPrefabClone = Instantiate(ModBehaviour.inputWithSliderPrefab);
                    inputWithSliderPrefabClone.SetActive(true);

                    // 设置描述
                    OptionsUIEntry_Slider_Mod uIEntry_Slider_Mod = inputWithSliderPrefabClone.GetComponent<OptionsUIEntry_Slider_Mod>();
                    if (uIEntry_Slider_Mod == null)
                    {
                        Debug.LogError("无法获取 OptionsUIEntry_Slider_Mod 组件");
                        Destroy(inputWithSliderPrefabClone);
                        return;
                    }

                    //初始化数据
                    uIEntry_Slider_Mod.Init(key, description, valueType, sliderRange);

                    // 创建或查找mod标题
                    Transform modTitleTransform = modContent.transform.Find(modName);
                    if (modTitleTransform == null)
                    {
                        GameObject modNameTitleClone = Instantiate(ModBehaviour.dropdownListPrefab, modContent.transform);
                        modNameTitleClone.name = modName;
                        modNameTitleClone.transform.DestroyAllChildren();

                        // 创建标题文本
                        GameObject titleTextObject = new GameObject("TitleText");
                        titleTextObject.transform.SetParent(modNameTitleClone.transform);

                        TextMeshProUGUI titleText = titleTextObject.AddComponent<TextMeshProUGUI>();
                        titleText.SetText(modName);
                        titleText.margin = new Vector4(10, 10, 10, 10);

                        modNameTitleClone.SetActive(true);

                        modTitleTransform = modNameTitleClone.transform;
                    }

                    // 设置父级和顺序
                    inputWithSliderPrefabClone.transform.SetParent(modContent.transform, false);

                    int titleIndex = modTitleTransform.GetSiblingIndex();
                    inputWithSliderPrefabClone.transform.SetSiblingIndex(titleIndex + 1);
                    inputWithSliderPrefabClone.name = "UI_" + modName + "_" + key;


                    Debug.Log($"已成功添加滑条选项: {description}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建滑条时出错: {ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        void Awake()
        {
            TestAddDropDownlist();
            Debug.Log("ModConfig Mod Loaded!!!");
        }

        void OnDestroy()
        {
            // 清理静态变量
            modTabButton = null;
            modContent = null;
            dropdownListPrefab = null;
            pendingConfigActions.Clear();
        }

        void OnEnable()
        {
            Debug.Log("ModConfig Enabled");
        }

        void OnDisable()
        {
            Debug.Log("ModConfig Disabled");
        }

        private void Update()
        {
            ProcessPendingConfigs();
        }

        private void OnMenuAwake()
        {
            Debug.Log("ModConfig: OnMenuAwake");
            TryCreatingModSettingTab();
        }

        private void TryCreatingModSettingTab()
        {
            if (!ModBehaviour.createdModTab)
            {
                CreateModSettingTab();
                ModBehaviour.createdModTab = true;
            }
        }

        /// <summary>
        /// 在设置菜单中标签页追加ModSetting标签
        /// 1. 复制OptionsPanel/Tabs其中一个, 作为Tabs标签的子对象, 命名为modTab
        /// 2. 复制OptionsPanel/ScrollView/Viewport/Content其中一个, 作为Content的子对象, 命名为modContent
        /// 3. 清空modContent中内容
        /// 4. 获取modTab的OptionsPanel_TabButton实例命名为modTab_OptionsPanel_TabButton
        /// 5. modTab_OptionsPanel_TabButton.tab = (GameObject)modContent
        /// 6. 查找OptionsPanel并对其(List<OptionsPanel_TabButton>)tabButtons添加modTab_OptionsPanel_TabButton
        /// 7. 调用OptionsPanel的private void Setup()方法
        /// 8. 至此, 已经创建完毕, 后面就是需要修改下tab名称, 以及处理modContent, 添加各种选项了
        /// </summary>
        private void CreateModSettingTab()
        {
            Debug.Log("开始创建Mod设置标签页");

            // 获取MainMenu场景中的OptionsPanel
            OptionsPanel optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "DontDestroyOnLoad");

            if (optionsPanel == null)
            {
                Debug.LogError("OptionsPanel Not Found!!!");
                return;
            }

            Debug.Log("OptionsPanel Found");

            // 使用反射获取tabButtons
            var tabButtons = ReflectionHelper.GetFieldValue<List<OptionsPanel_TabButton>>(optionsPanel, "tabButtons");
            if (tabButtons == null)
            {
                Debug.LogError("无法反射获取tabButtons!!!!");
                return;
            }

            Debug.Log("已反射获取tabButtons");

            // 复制一个tabButton的游戏对象
            GameObject tabButtonGameObjectClone = Instantiate(tabButtons[0].gameObject, tabButtons[0].gameObject.transform.parent);
            tabButtonGameObjectClone.name = "modTab";

            OptionsPanel_TabButton modTabButton = tabButtonGameObjectClone.GetComponent<OptionsPanel_TabButton>();
            if (modTabButton == null)
            {
                Debug.LogError("无法获取克隆的GameObject的OptionsPanel_TabButton组件");
                Destroy(tabButtonGameObjectClone);
                return;
            }

            ModBehaviour.modTabButton = modTabButton;

            // 获取原始tab并克隆
            var tab = ReflectionHelper.GetFieldValue<GameObject>(modTabButton, "tab");
            if (tab == null)
            {
                Debug.LogError("无法反射获取modTabButton的tab成员");
                Destroy(tabButtonGameObjectClone);
                return;
            }

            GameObject tabClone = Instantiate(tab, tab.transform.parent);
            tabClone.name = "modContent";
            ModBehaviour.modContent = tabClone;

            // 设置克隆的tab到tabButton            
            bool result = ReflectionHelper.SetFieldValue(modTabButton, "tab", tabClone);
            if (!result)
            {
                Debug.LogError("反射修改tab成员失败!!");
                Destroy(tabButtonGameObjectClone);
                Destroy(tabClone);
                return;
            }

            // 添加到tabButtons列表
            tabButtons.Add(modTabButton);

            // 调用Setup更新UI
            ReflectionHelper.InvokeMethod(optionsPanel, "Setup");

            // 修改标签页名称
            TextMeshProUGUI? tabName = modTabButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tabName != null)
            {
                // 移除本地化组件
                TextLocalizor localizor = modTabButton.GetComponentInChildren<TextLocalizor>(true);
                if (localizor != null)
                    Destroy(localizor);

                tabName.SetText("Mod Settings");
            }

            // 清空内容并创建下拉列表预制体
            if (modContent != null)
            {
                modContent.transform.DestroyAllChildren();

                // 查找分辨率下拉列表作为模板
                OptionsUIEntry_Dropdown resolutionDropDown = tabClone.transform.parent.GetComponentsInChildren<OptionsUIEntry_Dropdown>(true)
                    .FirstOrDefault(dropdown => dropdown.gameObject.name == "UI_Resolution");

                if (resolutionDropDown != null)
                {
                    GameObject dropdownListPrefab = Instantiate(resolutionDropDown.gameObject, modContent.transform);
                    dropdownListPrefab.name = "dropDownPrefab";
                    dropdownListPrefab.SetActive(false);
                    ModBehaviour.dropdownListPrefab = dropdownListPrefab;

                    Debug.Log("成功创建下拉列表预制体");
                }
                else
                {
                    Debug.LogError("未找到分辨率下拉列表作为模板");
                }
            }

            MakeInputWithSliderPrefab();

            Debug.Log("Mod设置标签页创建完成");

            // 立即处理等待的配置项
            ProcessPendingConfigs();
        }

        private void MakeInputWithSliderPrefab()
        {
            if (modContent == null)
            {
                Debug.LogError("modContent为空! MakeInputWithSliderPrefab failed!");
            }
      
            GameObject? UI_MouseSensitivity = FindObjectsOfType<OptionsUIEntry_Slider>().First(component => component.gameObject.name == "UI_MouseSensitivity")?.gameObject;

            if (UI_MouseSensitivity == null)
            {
                Debug.LogError("无法找到鼠标灵敏度设置选项, 将无法使用InputWithSlider类型config");
                return;
            }

            GameObject UI_MouseSensitivity_Clone = Instantiate(UI_MouseSensitivity);
            UI_MouseSensitivity_Clone.SetActive(true);
            DontDestroyOnLoad(UI_MouseSensitivity_Clone);

            OptionsUIEntry_Slider optionsUIEntry_Slider = UI_MouseSensitivity_Clone.GetComponent<OptionsUIEntry_Slider>();
            //获取这三个引用
            //private TextMeshProUGUI label;
            //private Slider slider;
            //private TMP_InputField valueField;
            var label = ReflectionHelper.GetFieldValue<TextMeshProUGUI>(optionsUIEntry_Slider, "label");
            var slider = ReflectionHelper.GetFieldValue<UnityEngine.UI.Slider>(optionsUIEntry_Slider, "slider");
            var valueField = ReflectionHelper.GetFieldValue<TMP_InputField>(optionsUIEntry_Slider, "valueField");

            Destroy(optionsUIEntry_Slider);

            OptionsUIEntry_Slider_Mod UIEntry_Slider_Mod = UI_MouseSensitivity_Clone.AddComponent<OptionsUIEntry_Slider_Mod>();

            //将旧的3个组件加到新的OptionsUIEntry_Slider_Mod
            UIEntry_Slider_Mod.label = label;
            UIEntry_Slider_Mod.slider = slider;
            UIEntry_Slider_Mod.valueField = valueField;

            UIEntry_Slider_Mod.label.SetText("OptionsUIEntry_Slider_Mod");

            ModBehaviour.inputWithSliderPrefab = UI_MouseSensitivity_Clone;
        }

        private void TestAddDropDownlist()
        {
            SortedDictionary<string, object> dropDownOptions = new SortedDictionary<string, object>()
            {
                { "选项1", 1 },
                { "选项2", 2 },
                { "选项3", 3 },
                { "选项4", 4 },
            };

            AddDropdownList("模组A", "testA1", "测试选项1", dropDownOptions, typeof(int), 0);
            AddDropdownList("模组B", "testB1", "测试选项1", dropDownOptions, typeof(int), 0);
            AddDropdownList("模组A", "testA2", "测试选项2", dropDownOptions, typeof(int), 0);
            AddDropdownList("模组A", "testA3", "测试选项3", dropDownOptions, typeof(int), 0);
            AddDropdownList("模组B", "testB2", "测试选项2", dropDownOptions, typeof(int), 0);
            AddInputWithSlider("模组C", "testSlider1", "测试滑条float", typeof(float), new Vector2( 0.0f, 1.0f));
        }

        override protected void OnAfterSetup()
        {
            Debug.Log("已添加MainMenuAwake事件");
            MainMenu.OnMainMenuAwake += OnMenuAwake;

            // 检查是否已经存在MainMenu
            if (FindAnyObjectByType(typeof(MainMenu)) != null)
            {
                Debug.Log("MainMenu 已存在，尝试创建设置标签页");
                TryCreatingModSettingTab();
            }
        }

        override protected void OnBeforeDeactivate()
        {
            MainMenu.OnMainMenuAwake -= OnMenuAwake;
        }
    }
}