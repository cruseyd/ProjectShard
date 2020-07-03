using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColumnDisplay : MonoBehaviour
{
    private List<TextMeshProUGUI> _labels;
    private int _currentIndex;
    void Awake()
    {
        _labels = new List<TextMeshProUGUI>();
        TextMeshProUGUI[] text = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI txt in text)
        {
            _labels.Add(txt);
        }
        ShowLabel(0);
    }

    public void ShowLabel(int _labelIndex)
    {
        if (_labelIndex >= _labels.Count)
        {
            _labels[_currentIndex].gameObject.SetActive(false);
            return;
        }
        _currentIndex = _labelIndex;
        for (int ii = 0; ii < _labels.Count; ii++)
        {
            if (ii == _labelIndex)
            {
                _labels[ii].gameObject.SetActive(true);
            } else
            {
                _labels[ii].gameObject.SetActive(false);
            }
        }
    }
}
