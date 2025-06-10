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
        // ... �߰������� �ʿ��� �Ŵ��� ���
    }

    void AddManager<T>(T manager) where T : IManager
    {
        _managers[typeof(T)] = manager;
        manager.Init(); // ���� �ʱ�ȭ
    }

    public static T Get<T>() where T : class, IManager
    {
        return Instance._managers[typeof(T)] as T;
    }

    // �߰�� ���� (�ʿ��� ���)
    public void RunCoroutine(System.Collections.IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
