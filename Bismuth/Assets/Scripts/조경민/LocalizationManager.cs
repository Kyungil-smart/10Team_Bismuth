using UnityEngine;
using System.Collections.Generic;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private SheetData _sheet;
    [SerializeField] private Language _currentLanguage = Language.English;
    public Language CurrentLanguage => _currentLanguage;

    public event Action OnLocalizationLoaded;

    private Dictionary<string, string> _table = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(_sheet.Load(SetLanguage));
    }

    private void SetLanguage(char splitSymbol, string[] lines)
    {
        _table.Clear();

        int langIndex = (int)_currentLanguage + 1;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(splitSymbol);

            if (cols.Length <= langIndex) continue;

            string key = cols[0];
            string value = cols[langIndex];

            _table[key] = value;
        }

        OnLocalizationLoaded?.Invoke(); // 변경 후 받아오기용
    }

    public string Get(string key)
    {
        return _table.TryGetValue(key, out var value) ? value : key;
    }

    public void ChangeLanguage(Language language)
    {
        _currentLanguage = language;
        StartCoroutine(_sheet.Load(SetLanguage));
    }
}

public enum Language
{
    Korean,
    English
}