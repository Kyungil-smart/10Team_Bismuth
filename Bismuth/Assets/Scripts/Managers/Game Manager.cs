using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] private bool _debugAllOn = true;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    private void Init()
    {
        GenerateManager<GameSceneManager>();
        GenerateManager<AudioManager>();
        GenerateManager<TimeScaleController>();
        
        DebugTool.DebugPrintAll(_debugAllOn);
    }

    private void Start()
    {
        DebugTool.Log("게임 시작", DebugType.Game, this);
    }

    private void GenerateManager<T>() where T : Component
    {
        if (FindAnyObjectByType<T>() != null) return;
        
        var go = new GameObject(typeof(T).Name);
        go.AddComponent<T>();
        DontDestroyOnLoad(go);
    }

}

