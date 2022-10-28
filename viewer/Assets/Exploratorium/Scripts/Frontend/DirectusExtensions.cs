using System.Linq;
using Directus.Connect.v9;
using UnityEngine.Localization;

namespace Exploratorium.Frontend
{
    public static class DirectusExtensions
    {
        public static bool TryGetTranslation<TTranslation>(
            Locale locale, Translations<TTranslation> translations,
            out TTranslation text
        )
            where TTranslation : DbRecord, ITranslationRecord
        {
            text = null;
            if (translations == null || !translations.Any())
                return false;
            return translations.TryGetBestAvailableCode(
                locale.Identifier.Code,
                out var code) && translations.TryGetTranslation(code, out text);
        }
    }
}