using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EVENT_TYPE
{
    DEFAULT,
    KILL
};

public class EventManager : MonoBehaviour {
    // реализация синглтона
	public static EventManager Instance { get { return instance; } set { } }

    private static EventManager instance = null;

    // Тип делегата обробатывающего событие
    public delegate void OnEvent(EVENT_TYPE Event_type,GameObject Sender, object Param = null);

    // Массив получателей
    private Dictionary<EVENT_TYPE, List<OnEvent>> Listeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();

    void Awake()
    {
        // реализация синглтона
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void AddListener(EVENT_TYPE Event_type, OnEvent Listener)
    {
        List<OnEvent> ListenList = null;
        if(Listeners.TryGetValue(Event_type, out ListenList))
        {
            ListenList.Add(Listener);
            return;
        }

        ListenList = new List<OnEvent>();
        ListenList.Add(Listener);
        Listeners.Add(Event_type, ListenList);
    }
    
    public void PostNotification(EVENT_TYPE Event_type,GameObject Sender = null, object Param = null)
    {
        List<OnEvent> ListenList = null;
        if (!Listeners.TryGetValue(Event_type, out ListenList))
            return;
        

        for(int i = 0; i < ListenList.Count; i++)
        {
            if (!ListenList[i].Equals(null))
                ListenList[i](Event_type,Sender,Param);
        }
    }

    public void RemoveEvent(EVENT_TYPE Event_type)
    {
        Listeners.Remove(Event_type);
    }

    public void RemoveRedundancies()
    {
        Dictionary<EVENT_TYPE, List<OnEvent>> TmpListeners = new Dictionary<EVENT_TYPE,List<OnEvent>>();

        foreach(KeyValuePair<EVENT_TYPE,List<OnEvent>> item in Listeners)
        {
            for(int i = item.Value.Count - 1; i>=0; i--)
            {
                if (item.Value[i].Equals(null))
                    item.Value.RemoveAt(i);
            }

            if(item.Value.Count > 0)
            {
                TmpListeners.Add(item.Key, item.Value);
            }
        }

        Listeners = TmpListeners;
    }

    void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
}
