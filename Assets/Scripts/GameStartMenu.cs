﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameStartMenu : MonoBehaviour
{
    private Decklist[] _playerDecklists;
    private EnemyData[] _enemies;

    [SerializeField] private TMP_Dropdown _playerDeckDropdown;
    [SerializeField] private TMP_Dropdown _enemyDropdown;

    private void Awake()
    {
        Card.Load();
        _playerDecklists = Resources.LoadAll<Decklist>("DeckLists/Player");
        Array.Sort(_playerDecklists);
        _playerDeckDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> playerOptions = new List<TMP_Dropdown.OptionData>();
        foreach (Decklist dlist in _playerDecklists)
        {
            playerOptions.Add(new TMP_Dropdown.OptionData(dlist.name));
        }
        _playerDeckDropdown.AddOptions(playerOptions);

        _enemies = Resources.LoadAll<EnemyData>("EnemyData");
        Array.Sort(_enemies);
        _enemyDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> enemyOptions = new List<TMP_Dropdown.OptionData>();
        foreach (EnemyData enemy in _enemies)
        {
            enemyOptions.Add(new TMP_Dropdown.OptionData(enemy.name));
        }
        _enemyDropdown.AddOptions(enemyOptions);

    }

    public void ApplySettings()
    {
        GameData.playerDecklist = _playerDecklists[_playerDeckDropdown.value];
        GameData.enemy = _enemies[_enemyDropdown.value];
    }

    public void StartGame()
    {
        ApplySettings();
        GameData.instance.startEncounter = true;
        SceneManager.LoadScene(1);
    }

    public void StartDraft()
    {
        ApplySettings();
        GameData.instance.startDraft = true;
        SceneManager.LoadScene(1);
    }
}