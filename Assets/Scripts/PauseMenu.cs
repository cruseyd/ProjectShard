using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] private GameObject _menu;

    [SerializeField] private Toggle _invincible;
    [SerializeField] private Toggle _ignoreResources;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
        _menu.SetActive(false);
    }

    private void Start()
    {
        _invincible.isOn = GameData.instance.invincible;
        _ignoreResources.isOn = GameData.instance.ignoreResources;
        _invincible.onValueChanged.AddListener(delegate { ToggleInvincible(_invincible); });
        _ignoreResources.onValueChanged.AddListener(delegate { ToggleIgnoreResources(_ignoreResources); });
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menu.SetActive(true);
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
        ToResume();
    }

    public void ToResume()
    {
        _menu.SetActive(false);
    }

    private void ToggleInvincible(Toggle toggle)
    {
        GameData.instance.invincible = _invincible.isOn;
    }
    private void ToggleIgnoreResources(Toggle toggle)
    {
        GameData.instance.ignoreResources = _ignoreResources.isOn;
    }
}
