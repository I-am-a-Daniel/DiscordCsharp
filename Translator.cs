using GoogleTranslateFreeApi;

public class Translator
{
    public static async Task<string> Translate(string langcode, string text)
    {
        var translator = new GoogleTranslator();
        Language from = Language.Auto;
        try
        {
            Language to = GoogleTranslator.GetLanguageByISO(langcode);
            TranslationResult tr = await translator.TranslateAsync(text, from, to);
            string response = tr.MergedTranslation;
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new string("Ismeretlen nyelv.");
        }
    }
}