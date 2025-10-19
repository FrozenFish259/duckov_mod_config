using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Sprite = UnityEngine.Sprite;

namespace ModConfig
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
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
            CreateModSettingTab();
            TestAddDropDownlist();
        }



        void OnDisable()
        {
           
        }


        private void Update()
        {

        }
        /// <summary>
        /// 在设置菜单中标签页追加ModSetting标签
        /// </summary>
        private void CreateModSettingTab()
        {
            //TODO:
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
    }
}