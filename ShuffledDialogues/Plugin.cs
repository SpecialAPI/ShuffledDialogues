using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace ShuffledDialogues
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.inscryption.shuffleddialogue";
        public const string NAME = "Shuffled Dialogue";
        public const string VERSION = "1.0.0";
        public static Dictionary<string, string> text = new();

        public void Awake()
        {
            Localization.TryLoadLanguage(Language.Russian);
            List<Localization.Translation> list = Localization.translations;
            List<Localization.Translation> l2 = new(list);
            foreach(Localization.Translation str in list)
            {
                if (!text.ContainsKey(str.englishStringFormatted))
                {
                    List<Localization.Translation> l3 = new(l2);
                    l3.Remove(str);
                    if (l3.Count > 0)
                    {
                        int rind = UnityEngine.Random.Range(0, l3.Count);
                        Localization.Translation elem = l3[rind];
                        l2.Remove(elem);
                        text.Add(str.englishStringFormatted, elem.englishString);
                    }
                }
            }
            new Harmony(GUID).PatchAll();
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.TranslateToLanguage))]
        [HarmonyPrefix]
        public static bool MessUp(ref string __result, Language language, string englishText)
        {
            if(language == Language.English && !string.IsNullOrEmpty(englishText) && text.ContainsKey(Localization.FormatString(englishText)))
            {
                __result = text[Localization.FormatString(englishText)].Replace("<br>", "\n");
                return false;
            }
            return true;
        }
    }
}
