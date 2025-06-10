using System;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private Dictionary<System.Type, IManager> _managers = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        RegisterManagers();
    }

    void RegisterManagers()
    {
        AddManager(new TurnManager());
        // ... 추가적으로 필요한 매니저 등록
    }

    void AddManager<T>(T manager) where T : IManager
    {
        _managers[typeof(T)] = manager;
        manager.Init(); // 직접 초기화
    }

    public static T Get<T>() where T : class, IManager
    {
        return Instance._managers[typeof(T)] as T;
    }

    // 중계기 역할 (필요한 경우)
    public void RunCoroutine(System.Collections.IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
