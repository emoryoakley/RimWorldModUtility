using Verse;

namespace CustomTranslator
{
    /// <summary>
    /// 提供自定義翻譯功能的靜態類別。
    /// </summary>
    public static class CustomTranslator
    {
        /// <summary>
        /// 嘗試翻譯指定的 key，若翻譯不存在則回退到預設語言或返回 key 本身。
        /// </summary>
        /// <param name="key">要翻譯的 key。</param>
        /// <returns>翻譯後的字串，或在翻譯不存在時返回 key 本身。</returns>
        public static string TranslateOrFallback(this string key)
        {
            // 調用重載方法，傳遞 null 參數
            return TranslateOrFallback(key, null);
        }

        /// <summary>
        /// 嘗試翻譯指定的 key，若翻譯不存在則回退到預設語言或返回 key 本身。
        /// </summary>
        /// <param name="key">要翻譯的 key。</param>
        /// <param name="args">格式化參數。</param>
        /// <returns>翻譯後的字串，或在翻譯不存在時返回 key 本身。</returns>
        public static string TranslateOrFallback(this string key, params object[] args)
        {
            // 嘗試獲取目前語系的翻譯
            if (LanguageDatabase.activeLanguage.TryGetTextFromKey(key, out TaggedString translation))
            {
                // 如果存在翻譯，替換參數並回傳
                return args == null ? string.Format(translation) : string.Format(translation, args);
            }

            // RimWorld 使用英文當作預設語言，若有手動覆寫 LanguageDatabase.defaultLanguage 才會是其他語言
            // 如果沒有找到目前語言的翻譯，切換成備用的英文翻譯
            if (LanguageDatabase.defaultLanguage.TryGetTextFromKey(key, out TaggedString defaultTranslation))
            {
                // 如果存在備用的英文翻譯，替換參數並回傳
                return args == null ? string.Format(defaultTranslation) : string.Format(defaultTranslation, args);
            }

            // 如果也沒有備用的英文翻譯，則回傳 key 本身
            return key;
        }
    }
}