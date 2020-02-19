using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarDisplay : ValueDisplay
{

    [SerializeField] private Image _img;
    // Start is called before the first frame update

    public override void Refresh()
    {
        base.Refresh();
        _img.fillAmount = Mathf.Clamp01(((float)value) / ((float)baseValue));
    }
}
