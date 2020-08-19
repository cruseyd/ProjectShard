using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : Actor
{
    public static Enemy instance;

    [SerializeField] private EnemyData _data;
    [SerializeField] private TextMeshProUGUI _nameText;

    private int _drawRangeMin = 1;
    private int _drawRangeMax = 6;

    public new string name
    {
        get { return _data.name; }
    }
    public override void CycleDeck()
    {
        _drawRangeMin++;
        _drawRangeMax++;
    }
    public void DrawRandom()
    {
        StartCoroutine(DoDrawRandom());
    }
    public IEnumerator DoDrawRandom()
    {
        int n = Random.Range(_drawRangeMin, _drawRangeMax);
        n = (int)((float)n / 2.0f);
        n = Mathf.Max(n, 1);
        yield return DoDraw(n);
    }
    public override void Awake()
    {
        base.Awake();
        if (instance == null) { instance = this; }
        else { Destroy(this); }
        
    }
    public override void Start()
    {
        base.Start();
        _data = GameData.enemy;
        Debug.Log("Initializing enemy as: " + GameData.enemy.name);
        Init(_data);
    }
    public void Init(EnemyData data)
    {
        //set enemy UI to be active
        _data = data;
        _nameText.text = data.name;

        health.baseValue = data.maxHealth;
        maxHealth.baseValue = health.value;

        if (_data.cardPool != null && _data.cardPool.pool.Count > 0)
        {
            _deck.Init(data.cardPool);
        } else if (_data.decklist != null)
        {
            _deck.Init(_data.decklist);
        } else
        {
            Debug.Log("Could not seed cards for " + name);
        }
        _drawRangeMin = 2;
        _drawRangeMax = 7;
    }
}
