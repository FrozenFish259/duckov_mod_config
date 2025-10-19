using Duckov.Options.UI;
using Duckov.Utilities;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

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
                Debug.Log("modTabButton 或 modContent 尚未初始化，跳过处理配置项");
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

                    Debug.Log($"已成功添加下拉选项: {description}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建下拉列表时出错: {ex.Message}\n{ex.StackTrace}");
                }
            });
        }

        void Awake()
        {
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
            // 延迟测试，确保UI已初始化
            Invoke(nameof(DelayedTest), 1f);
        }

        void OnDisable()
        {
            Debug.Log("ModConfig Disabled");
        }

        private void DelayedTest()
        {
            TestAddDropDownlist();
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
            List<OptionsPanel_TabButton>? tabButtons = GetTabButtons(optionsPanel);
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
            GameObject? tab = GetTabFromOptionsPanel_TabButton(modTabButton);
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
            bool result = SetTabForOptionsPanel_TabButton(modTabButton, tabClone);
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
            InvokeSetup(optionsPanel);

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

            Debug.Log("Mod设置标签页创建完成");

            // 立即处理等待的配置项
            ProcessPendingConfigs();
        }

        private void InvokeSetup(OptionsPanel optionsPanel)
        {
            if (optionsPanel == null)
            {
                Debug.LogWarning("OptionsPanel 实例为 null");
                return;
            }

            Type optionsPanelType = typeof(OptionsPanel);
            MethodInfo setupMethod = optionsPanelType.GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Instance);

            if (setupMethod != null)
            {
                try
                {
                    setupMethod.Invoke(optionsPanel, null);
                    Debug.Log("成功调用 Setup 方法");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"调用 Setup 方法失败: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else
            {
                Debug.LogWarning("未找到 Setup 方法");
            }
        }

        private List<OptionsPanel_TabButton>? GetTabButtons(OptionsPanel optionsPanel)
        {
            if (optionsPanel == null) return null;

            Type optionsPanelType = typeof(OptionsPanel);
            FieldInfo tabButtonsField = optionsPanelType.GetField("tabButtons", BindingFlags.NonPublic | BindingFlags.Instance);

            if (tabButtonsField != null)
            {
                return tabButtonsField.GetValue(optionsPanel) as List<OptionsPanel_TabButton>;
            }

            return null;
        }

        private GameObject? GetTabFromOptionsPanel_TabButton(OptionsPanel_TabButton tabButton)
        {
            if (tabButton == null) return null;

            Type OptionsPanel_TabButtonType = typeof(OptionsPanel_TabButton);
            FieldInfo tabField = OptionsPanel_TabButtonType.GetField("tab", BindingFlags.NonPublic | BindingFlags.Instance);

            if (tabField != null)
            {
                return tabField.GetValue(tabButton) as GameObject;
            }

            return null;
        }

        private bool SetTabForOptionsPanel_TabButton(OptionsPanel_TabButton tabButton, GameObject newTab)
        {
            if (tabButton == null) return false;

            Type optionsPanelTabButtonType = typeof(OptionsPanel_TabButton);
            FieldInfo tabField = optionsPanelTabButtonType.GetField("tab", BindingFlags.NonPublic | BindingFlags.Instance);

            if (tabField != null)
            {
                try
                {
                    tabField.SetValue(tabButton, newTab);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"设置 tab 字段失败: {ex.Message}");
                    return false;
                }
            }

            Debug.LogWarning("未找到 tab 字段");
            return false;
        }

        private void TestAddDropDownlist()
        {
            if (modContent == null)
            {
                Debug.LogWarning("modContent 尚未初始化，延迟测试添加下拉列表");
                return;
            }

            SortedDictionary<string, object> options = new SortedDictionary<string, object>()
            {
                { "选项1", 1 },
                { "选项2", 2 },
                { "选项3", 3 },
                { "选项4", 4 },
            };

            AddDropdownList("模组A", "test", "测试选项1", options, typeof(int), 0);
            AddDropdownList("模组B", "test", "测试选项1", options, typeof(int), 0);
            AddDropdownList("模组A", "test", "测试选项2", options, typeof(int), 0);
            AddDropdownList("模组A", "test", "测试选项3", options, typeof(int), 0);
            AddDropdownList("模组B", "test", "测试选项2", options, typeof(int), 0);
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