using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonToKey : MonoBehaviour
{

    public static event Action buttonDidChange;
    private int _insideColliderCount = 0;
    private bool isFollowing = false;
    private int keyCode;


    void Start()
    {
        keyCode = (int)Enum.Parse(typeof(Buttons), gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        _insideColliderCount += 1;
        //InputUpdater.Instance.SetBool(keyCode, true);
        UDPClient.instance.SendInputState(1, keyCode, true);
        buttonDidChange?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        _insideColliderCount -= 1;
        _insideColliderCount = Mathf.Max(0, _insideColliderCount);

        if (_insideColliderCount == 0)
        {
            //InputUpdater.Instance.SetBool(keyCode, false);
            UDPClient.instance.SendInputState(1, keyCode, false);
            buttonDidChange?.Invoke();
        }
    }
    enum Buttons
    {
        //Key_Q=52,Key_W=53, Key_E = 54, Key_D = 55, Key_C = 56,Key_X=57,Key_Z=58,Key_A=59
        //Key_Q = 0, Key_W = 1, Key_E = 2, Key_D = 3, Key_C = 4, Key_X = 5, Key_Z = 6, Key_A = 7,Select= 8,Enter=9,Back=10,Home=11
        Key_Q = 81,
        Key_W = 87,
        Key_E = 69,
        Key_D = 68,
        Key_C = 67,
        Key_X = 88,
        Key_Z = 90,
        Key_A = 65,
        Key_3 = 51,
        GameTest = 145,
        xxx = 13,
        Pause = 19,
    }
}
