using System;
using System.Collections;
using UnityEngine;

public class InputUpdater : MonoBehaviour
{
    private static InputUpdater instance;
    public static InputUpdater Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputUpdater>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BoolManager");
                    instance = obj.AddComponent<InputUpdater>();
                }
            }
            return instance;
        }
    }

    private bool[] bools = new bool[64];

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBool(int key, bool state)
    {
        bools[key] = state;
    }

    public void SetTimer(Action<bool[]> callback, float frequencyHz)
    {
        StartCoroutine(TimerCoroutine(callback, frequencyHz));
    }

    private IEnumerator TimerCoroutine(Action<bool[]> callback, float frequencyHz)
    {
        while (true)
        {
            callback.Invoke(bools);
            yield return new WaitForSeconds(1 / frequencyHz);
        }
    }
}