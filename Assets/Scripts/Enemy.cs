using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : Actor
{
    public static Enemy instance;

    [SerializeField] private EnemyData _data;
    [SerializeField] private TextMeshProUGUI _nameText;

    public new string name
    {
        get { return _data.name; }
    }

    public void DrawRandom()
    {
        StartCoroutine(DoDrawRandom());
    }
    public IEnumerator DoDrawRandom()
    {
        //int n = Random.Range(Dungeon.gameParams.enemyDrawMin, Dungeon.gameParams.enemyDrawMax + 1);
        int n = Random.Range(1, 7);
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
        Init(_data);
    }
    public void Init(EnemyData data)
    {
        //set enemy UI to be active
        _data = data;
        _nameText.text = data.name;
        StatusDisplay[] displays = _statusDisplays.GetComponentsInChildren<StatusDisplay>();
        foreach (StatusDisplay tf in displays) { tf.gameObject.SetActive(false); }
        foreach (StatusDisplay tf in displays) { tf.gameObject.SetActive(false); }

        health.baseValue = data.maxHealth;
        maxHealth.baseValue = health.value;

        _weapon.Equip(data.weapon, this);
        _armor.Equip(data.armor, this);
        _relic = null;

        _deck.Init(data.cardPool);
    }
}
