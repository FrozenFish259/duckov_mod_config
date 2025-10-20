using Duckov.Options;
using SodaCraft.Localizations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModConfig
{
    class OptionsUIEntry_Slider_Mod: MonoBehaviour
    {
        // Token: 0x17000367 RID: 871
        // (get) Token: 0x060012C2 RID: 4802 RVA: 0x0004673A File Offset: 0x0004493A
        // (set) Token: 0x060012C3 RID: 4803 RVA: 0x0004674C File Offset: 0x0004494C
        //[LocalizationKey("Options")]
        //private string labelKey
        //{
        //    get
        //    {
        //        return "Options_" + this.key;
        //    }
        //    set
        //    {
        //    }
        //}

        // Token: 0x17000368 RID: 872
        // (get) Token: 0x060012C4 RID: 4804 RVA: 0x0004674E File Offset: 0x0004494E
        // (set) Token: 0x060012C5 RID: 4805 RVA: 0x00046761 File Offset: 0x00044961
        public float Value
        {
            get
            {
                return OptionsManager.Load<float>(this.key, this.defaultValue);
            }
            set
            {
                OptionsManager.Save<float>(this.key, value);
            }
        }

        // Token: 0x060012C6 RID: 4806 RVA: 0x00046770 File Offset: 0x00044970
        private void Awake()
        {
            this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
            this.valueField.onEndEdit.AddListener(new UnityAction<string>(this.OnFieldEndEdit));
            this.RefreshLable();
            LocalizationManager.OnSetLanguage += this.OnLanguageChanged;
        }

        // Token: 0x060012C7 RID: 4807 RVA: 0x000467CC File Offset: 0x000449CC
        private void OnDestroy()
        {
            LocalizationManager.OnSetLanguage -= this.OnLanguageChanged;
        }

        // Token: 0x060012C8 RID: 4808 RVA: 0x000467DF File Offset: 0x000449DF
        private void OnLanguageChanged(SystemLanguage language)
        {
            this.RefreshLable();
        }

        // Token: 0x060012C9 RID: 4809 RVA: 0x000467E7 File Offset: 0x000449E7
        private void RefreshLable()
        {
            //if (this.label)
            //{
            //    this.label.text = this.labelKey.ToPlainText();
            //}
        }

        // Token: 0x060012CA RID: 4810 RVA: 0x0004680C File Offset: 0x00044A0C
        private void OnFieldEndEdit(string arg0)
        {
            float value;
            if (float.TryParse(arg0, out value))
            {
                value = Mathf.Clamp(value, this.slider.minValue, this.slider.maxValue);
                this.Value = value;
            }
            this.RefreshValues();
        }

        // Token: 0x060012CB RID: 4811 RVA: 0x0004684D File Offset: 0x00044A4D
        private void OnEnable()
        {
            this.RefreshValues();
        }

        // Token: 0x060012CC RID: 4812 RVA: 0x00046855 File Offset: 0x00044A55
        private void OnSliderValueChanged(float value)
        {
            this.Value = value;
            this.RefreshValues();
        }

        // Token: 0x060012CD RID: 4813 RVA: 0x00046864 File Offset: 0x00044A64
        private void RefreshValues()
        {
            this.valueField.SetTextWithoutNotify(this.Value.ToString(this.valueFormat));
            this.slider.SetValueWithoutNotify(this.Value);
        }

        // Token: 0x060012CE RID: 4814 RVA: 0x000468A1 File Offset: 0x00044AA1
        //private void OnValidate()
        //{
        //    this.RefreshLable();
        //}

        // Token: 0x04000E1D RID: 3613
        private string key;

        // Token: 0x04000E1E RID: 3614
        [Space]
        [SerializeField]
        private float defaultValue;

        // Token: 0x04000E1F RID: 3615
        [SerializeField]
        public TextMeshProUGUI label;

        // Token: 0x04000E20 RID: 3616
        [SerializeField]
        public Slider slider;

        // Token: 0x04000E21 RID: 3617
        [SerializeField]
        public TMP_InputField valueField;

        // Token: 0x04000E22 RID: 3618
        [SerializeField]
        private string valueFormat = "0";

        public void Init(string key, string description, Type valueType, Vector2? sliderRange)
        {
            this.key = key;
            this.label.SetText(description);

            //TODO: 处理不同数据类型
            
        }
    }
}
