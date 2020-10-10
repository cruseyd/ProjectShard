using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DecklistIndex : MonoBehaviour
{
    public static DecklistIndex instance;

    private static List<Decklist> _playerDecks;
    private static List<EnemyData> _enemyDecks;
    private static List<string> _deletePlayerDecks;
    private static List<string> _deleteEnemyDecks;
    private static List<Decklist> _newPlayerDecks;
    private static List<EnemyData> _newEnemyDecks;

    public static List<Decklist> player
    {
        get { return _playerDecks; }
    }
    public static List<EnemyData> enemy
    {
        get { return _enemyDecks; }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Load();
            _deletePlayerDecks = new List<string>();
            _deleteEnemyDecks = new List<string>();
            _newPlayerDecks = new List<Decklist>();
            _newEnemyDecks = new List<EnemyData>();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void Load()
    {
        string path = Application.dataPath + "/Resources/DeckLists";

        string player_json = File.ReadAllText(path + "/player_decks.json");
        Debug.Log(player_json);
        DecklistArray playerDeckArray = JsonUtility.FromJson<DecklistArray>(player_json);
        _playerDecks = playerDeckArray.decks;

        string enemy_json = File.ReadAllText(path + "/enemy_decks.json");
        EnemyDataArray enemyDeckArray = JsonUtility.FromJson<EnemyDataArray>(enemy_json);
        _enemyDecks = enemyDeckArray.enemies;

        _playerDecks.Sort();
        _enemyDecks.Sort();
    }

    public static void Save()
    {
        string path = Application.dataPath + "/Resources/DeckLists";

        DecklistArray playerDecks = new DecklistArray();
        playerDecks.decks = new List<Decklist>();

        foreach (Decklist list in _playerDecks)
        {
            if (!(_deletePlayerDecks.Contains(list.name)))
            {
                playerDecks.decks.Add(list);
            }
        }
        foreach (Decklist list in _newPlayerDecks)
        {
            playerDecks.decks.Add(list);
        }
        string playerDeckJSON = JsonUtility.ToJson(playerDecks);
        File.WriteAllText(path + "/player_decks.json", playerDeckJSON);


        EnemyDataArray enemyDecks = new EnemyDataArray();
        enemyDecks.enemies = new List<EnemyData>();
        foreach (EnemyData list in _enemyDecks)
        {
            if (!(_deleteEnemyDecks.Contains(list.name)))
            {
                enemyDecks.enemies.Add(list);
            }
        }
        foreach (EnemyData list in _newEnemyDecks)
        {
            enemyDecks.enemies.Add(list);
        }
        string enemyDeckJSON = JsonUtility.ToJson(enemyDecks);
        File.WriteAllText(path + "/enemy_decks.json", enemyDeckJSON);
    }
}
