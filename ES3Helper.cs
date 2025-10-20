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
        private static Dictionary<string, Type> cachedDict = new Dictionary<string, Type>();
        private static ES3Settings SaveSettings
        {
            get
            {
                return OptionsManager_Mod.SaveSettings;
            }
        }

        public static void DeleteKey(string key)
        {
            ES3.DeleteKey(key, SaveSettings);
        }

        public static Type? getOldTypeOfKeyCached(string key)
        {
            if (cachedDict.ContainsKey(key))
            {
                return cachedDict[key];
            }

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

            cachedDict.Add(key, type);

            return type;
        }        
    }
}
