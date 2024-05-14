using System.Text.Json;
using Skills.Models.Enums;

namespace Skills.Services;

public class LocalizationManager
{
    public string[] SupportedLanguages { get; set; } = { "fr-FR", "en-US" };
    public string SelectedLanguage { get; set; }
    private Dictionary<string, string> _translations = new();
    public delegate void LanguageChanged();
    public event LanguageChanged? OnLanguageChanged;

    public LocalizationManager()
    {
        SelectedLanguage = SupportedLanguages[0];   
    }
    
    public Task InitAsync()
    {
        var stream = File.OpenRead($".\\Languages\\{SelectedLanguage}.json");
        stream.Seek(0, SeekOrigin.Begin);
        _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(stream) ?? [];
        OnLanguageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public async Task SetLanguageAsync(string lang)
    {
        SelectedLanguage = SupportedLanguages.Contains(lang) ? lang : SupportedLanguages[0]; 
        await InitAsync();
    }
    
    public string Get(string key)
    {
        _translations.TryGetValue(key, out string? val);
        return string.IsNullOrWhiteSpace(val) ? "undefined" : val;
    }
}