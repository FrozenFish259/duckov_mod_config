using Duckov.Options.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace ModConfig
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private static bool createdModTab = false;

        public static OptionsPanel_TabButton modTabButton;
        public static GameObject modContent;

        /// <summary>
        /// 下拉选项, 类似分辨率选择
        /// 
        /// name: 选项名称
        /// options: key为显示的字符串, value表示该选项对应的要使用的值
        /// 
        /// 例: 添加分辨率选项AddDropdownList("分辨率", {"1980, 1080":0, "1280, 720":1});
        /// </summary>
        /// <param name="options"></param>
        public static void AddDropdownList(string name, SortedDictionary<string, object> options, Type valueType)
        {
            // Canvas/MainMenuContainer/Menu/OptionsPanel
            // --Tabs 标签组
            // --ScrollView 选项区域


            Debug.Log("已添加下拉选项config:" + name);
        }
      
        void Awake()
        {
            Debug.Log("ModConfig Mod Loaded!!!");

        }

        void OnDestroy()
        {
          
        }

        void OnEnable()
        {
            TestAddDropDownlist();
        }



        void OnDisable()
        {
           
        }


        private void Update()
        {

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
            //1. 复制OptionsPanel/Tabs其中一个, 作为Tabs标签的子对象, 命名为modTab
            //2. 复制OptionsPanel/ScrollView/Viewport/Content其中一个, 作为Content的子对象, 命名为modContent
            //3. 清空modContent中内容
            //4. 获取modTab的OptionsPanel_TabButton实例命名为modTab_OptionsPanel_TabButton
            //5. modTab_OptionsPanel_TabButton.tab = (GameObject)modContent
            //6. 查找OptionsPanel并对其(List<OptionsPanel_TabButton>)tabButtons添加modTab_OptionsPanel_TabButton
            //7. 调用OptionsPanel的private void Setup()方法
            //8. 至此, 已经创建完毕, 后面就是需要修改下tab名称, 以及处理modContent, 添加各个选项了
            Debug.Log("开始创建Mod设置标签页");

            // 获取MainMenu场景中的OptionsPanel
            OptionsPanel optionsPanel = FindObjectsOfType<OptionsPanel>(true)
                .FirstOrDefault(panel => panel.gameObject.scene.name == "MainMenu");

            if (optionsPanel == null)
            {
                Debug.Log("OptionsPanel Not Found!!!");
                return;
            }

            Debug.Log("OptionsPanel Found");

            //使用反射获取tabButtons
            List<OptionsPanel_TabButton>? tabButtons = null;
            do
            {
                tabButtons = GetTabButtons(optionsPanel);

                if(tabButtons == null)
                {
                    Debug.Log("无法反射获取tabButtons!!!!");
                    return;
                }

                Debug.Log("已反射获取tabButtons");
            } while (false);

            //获取OptionsPanel/Tabs游戏对象
            //实际上就是tabButtons中任一GameObject父节点
            //GameObject? Tabs_GameObject = tabButtons[0].gameObject.transform.parent?.gameObject;

            //if (Tabs_GameObject == null) {
            //    Debug.Log("未找到OptionsPanel/Tabs");
            //    return;
            //}

            //复制一个tabButton的游戏对象到OptionsPanel/Tabs节点下
            GameObject tabButtonGameObjectClone = Instantiate(tabButtons[0].gameObject, tabButtons[0].gameObject.transform.parent);
            tabButtonGameObjectClone.name = "modTab";


            OptionsPanel_TabButton modTabButton = tabButtonGameObjectClone.GetComponent<OptionsPanel_TabButton>();

            ModBehaviour.modTabButton = modTabButton;

            tabButtons.Add(modTabButton);

            if (modTabButton == null) {
                Debug.Log("无法获取克隆的GameObject的OptionsPanel_TabButton组件");
                return;
            }

            //这时候需要修改下tabButton的tab, 因为这个克隆的引用的还是旧的tab
            //但是首先我们需要先克隆一个tab再说
            //所以这里先克隆tab
            GameObject? tab = GetTabFromOptionsPanel_TabButton(modTabButton);

            if (tab == null)
            {
                Debug.Log("无法反射获取modTabButton的tab成员");
                return;
            }
            
            GameObject tabClone = Instantiate(tab, tab.transform.parent);
            tabClone.name = "modContent";

            ModBehaviour.modContent = tabClone;

            //TODO:清空tabClone里面没用的选项
            var TODO = 1;

            //这里把克隆的tab设置为tabButton的tab成员
            bool result = SetTabForOptionsPanel_TabButton(modTabButton, tabClone);

            if (!result)
            {
                Debug.Log("反射修改tab成员失败!!");
                return;
            }

            //这里调用下OptionsPanel的Setup来更新之前刚插入的克隆出来的mod标签页
            InvokeSetup(optionsPanel);

            /////////////从这里开始已经成功创建了一个能够正常工作的标签页了/////////////
            //接下来需要修改下标签页的名称, 然后清空tabClone里面没用的选项
        }

        private void InvokeSetup(OptionsPanel optionsPanel)
        {
            if (optionsPanel == null)
            {
                Debug.LogWarning("OptionsPanel 实例为 null");
                return;
            }

            // 获取类型信息
            Type optionsPanelType = typeof(OptionsPanel);

            // 获取私有方法 "Setup"
            MethodInfo setupMethod = optionsPanelType.GetMethod("Setup",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (setupMethod != null)
            {
                try
                {
                    setupMethod.Invoke(optionsPanel, null);
                    Debug.Log("成功调用 Setup 方法");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"调用 Setup 方法失败: {ex.Message}");
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

            // 获取类型信息
            Type optionsPanelType = typeof(OptionsPanel);

            // 获取私有字段 "tabButtons"
            FieldInfo tabButtonsField = optionsPanelType.GetField("tabButtons",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (tabButtonsField != null)
            {
                return tabButtonsField.GetValue(optionsPanel) as List<OptionsPanel_TabButton>;
            }

            return null;
        }

        private GameObject? GetTabFromOptionsPanel_TabButton(OptionsPanel_TabButton tabButton)
        {
            if (tabButton == null) return null;

            // 获取类型信息
            Type OptionsPanel_TabButtonType = typeof(OptionsPanel_TabButton);

            // 获取私有字段 "tab"
            FieldInfo tabField = OptionsPanel_TabButtonType.GetField("tab",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (tabField != null)
            {
                return tabField.GetValue(tabButton) as GameObject;
            }

            return null;
        }

        private bool SetTabForOptionsPanel_TabButton(OptionsPanel_TabButton tabButton, GameObject newTab)
        {
            if (tabButton == null) return false;

            // 获取类型信息
            Type optionsPanelTabButtonType = typeof(OptionsPanel_TabButton);

            // 获取私有字段 "tab"
            FieldInfo tabField = optionsPanelTabButtonType.GetField("tab",
                BindingFlags.NonPublic | BindingFlags.Instance);

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
            string name = "测试";
            SortedDictionary<string, object> options = new SortedDictionary<string, object>()
            {
                { "选项1", 1},
                { "选项2", 2},
                { "选项3", 3},
                { "选项4", 4},
            };

            AddDropdownList(name, options, typeof(int));
        }

        override protected void OnAfterSetup()
        {
            Debug.Log("已添加MainMenuAwake事件");
            //添加事件
            MainMenu.OnMainMenuAwake += OnMenuAwake;

            //这里是防止该mod是后启用的, 此时MainMenu的Awake事件已经被触发过, 所以需要单独判断下
            if (FindAnyObjectByType(typeof(MainMenu)) != null)
            {
                TryCreatingModSettingTab();
            }
        }

        override protected void OnBeforeDeactivate()
        {
            MainMenu.OnMainMenuAwake -= OnMenuAwake;
        }
    }
}