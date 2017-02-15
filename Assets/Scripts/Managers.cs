using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(GameController))]
[RequireComponent(typeof(UtilityClass))]
// TODO: Добавить менеджер фонового звука.

public class Managers : MonoBehaviour {

    public static GameController GameManager { get; private set; }
    public static UtilityClass UtilityManager { get; private set; }

    private List<IGameManager> _startSequence;

    void Awake()
    {
        GameManager = GetComponent<GameController>();
        UtilityManager = GetComponent<UtilityClass>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(GameManager);
        _startSequence.Add(UtilityManager);
        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        foreach(IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numberModules = _startSequence.Count;
        int numberIsReady = 0;

        while (numberIsReady < numberModules)
        {
            int last = numberIsReady;
            numberIsReady = 0;

            foreach(IGameManager manager in _startSequence)
            {
                if(manager.status == ManagerStatus.Started)
                {
                    numberIsReady++;
                }
            }

            if (numberIsReady > last)
                Debug.Log("Прогресс: " + numberIsReady + "/" + numberModules);

            yield return null;
        }

        Debug.Log("Все контроллеры запущены!");
        
    }

    
}
