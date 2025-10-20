using Duckov.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ExampleUsingModConfig
{
    private static string MOD_NAME = "EXAMPLE_MOD_NAME";

    //int with no range
    private static int posX = 0;

    //int with range 0-10000
    private static int money = 0;

    //float with no range
    private static float posY = 220.12f;
    //float with range 0.0f-100.0f
    private static float time = 100.0f;

    //bool
    private static bool win = false;

    //dropdown list
    //"None", "Red", "Blue", "Green"
    // 0, 1, 2, 3
    private string color = "None";

    public static void OnActive()
    {

        OptionsManager.OnOptionsChanged += OnOptionsChanged;

        bool checkModConfig = ModConfigAPI.Initialize();

        if (!checkModConfig)
            return;

        //no range, no slider
        //pass null to sliderRange so it's disabled
        ModConfigAPI.SafeAddInputWithSlider(
            MOD_NAME, $"{MOD_NAME}_{nameof(posX)}", "position X", typeof(int), posX, null);

        //ranged with slider
        ModConfigAPI.SafeAddInputWithSlider(
                    MOD_NAME, $"{MOD_NAME}_{nameof(money)}", "your money", typeof(int), money, new Vector2(0f, 10000f));

        ModConfigAPI.SafeAddInputWithSlider(
                    MOD_NAME, $"{MOD_NAME}_{nameof(posY)}", "position Y", typeof(float), posY, null);

        ModConfigAPI.SafeAddInputWithSlider(
                           MOD_NAME, $"{MOD_NAME}_{nameof(time)}", "time left", typeof(float), time, new Vector2(0.0f, 100.0f));

        //bool dropdown
        ModConfigAPI.SafeAddBoolDropdownList(
            MOD_NAME, $"{MOD_NAME}_{nameof(win)}", "won?", win);

        //custom dropdown list
        var colorsDict = new SortedDictionary<string, object>()
        {
            { "None", 0 },
            { "Red", 1 },
            { "Blue", 2 },
            { "Green", 3 },
        };

        // use reversed dictionary
        var reversedColorsDict = getReversedSortedDictionary<string, object>(colorsDict);

        //bool dropdown
        ModConfigAPI.SafeAddDropdownList(
            MOD_NAME, $"{MOD_NAME}_{nameof(color)}", "Color", reversedColorsDict, typeof(int), 0);
    }

    private static SortedDictionary<T1, T2> getReversedSortedDictionary<T1, T2>(SortedDictionary<T1, T2> dict)
    {       
        var reversedDict = new SortedDictionary<T1, T2>();

        foreach (var x in dict.Reverse())
        {
            reversedDict.Add(x.Key, x.Value);
        }

        return reversedDict;
    }

    public static void OnDeactivate()
    {
        OptionsManager.OnOptionsChanged -= OnOptionsChanged;
    }

    private static void OnOptionsChanged(string key)
    {
        UpdateModLogic();
    }

    private static void UpdateModLogic()
    {
        //Update your mod's configs...
        money = OptionsManager.Load<int>($"{MOD_NAME}_{nameof(money)}", 0);

        Debug.Log($"my money left={money}");

        //other updated configs...
    }
}

