using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Display : MonoBehaviour
{
    protected bool _active = true;
    public void SetActive(bool flag)
    {
        _active = flag;
        Refresh();
    }
    public virtual void Refresh() { }
}

