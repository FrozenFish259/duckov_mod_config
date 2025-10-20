using Duckov.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using Utilities;

namespace ModConfig
{
    class ES3Helper
    {
        private static ES3Settings? _SaveSettings = null;
        private static ES3Settings SaveSettings
        {
            get
            {
                if(_SaveSettings == null)
                {
                    _SaveSettings = ReflectionHelper.GetStaticPropertyValue<ES3Settings>(typeof(OptionsManager_Mod), "SaveSettings");
                }
                return _SaveSettings;
            }
        }

        public static void DeleteKey(string key)
        {
            ES3.DeleteKey(key, SaveSettings);
        }

        public static Type? getType(string key)
        {
            using ES3Reader eS3Reader = ES3Reader.Create(SaveSettings);
            if (eS3Reader == null)
            {
                throw new FileNotFoundException("File \"" + SaveSettings.FullPath + "\" could not be found.");
            }

            
            //if (!eS3Reader.Goto(key))
            if(!(bool)ReflectionHelper.InvokeInstanceMethod(eS3Reader, "Goto", new object[] { key }))
            {
                return null;
            }

            //Type type = eS3Reader.ReadKeyPrefix();            
            Type? type = (Type?)ReflectionHelper.InvokeInstanceMethod(eS3Reader, "ReadKeyPrefix", new object[] { false });

            return type;
        }        
    }
}
