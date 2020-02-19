using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Actor, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    public enum Stats
    {
        HEALTH,
        ENDURANCE,
        FOCUS,
        TEMPFOCUS
    }

    public static Player instance;

    [SerializeField] private ValueDisplay _enduranceDisplay;
    [SerializeField] private ValueDisplay _focusDisplay;

    [SerializeField] private ValueDisplay _violetDisplay;
    [SerializeField] private ValueDisplay _redDisplay;
    [SerializeField] private ValueDisplay _blueDisplay;
    [SerializeField] private ValueDisplay _goldDisplay;
    [SerializeField] private ValueDisplay _greenDisplay;
    [SerializeField] private ValueDisplay _indigoDisplay;

    private List<ValueDisplay> _affinityDisplay;

    //private int _endurance;
    //private int _focus;
    //private int _maxFocus;

    public Stat endurance;
    public Stat focus;
    public Stat maxFocus;

    public Stat violetAffinity;
    public Stat redAffinity;
    public Stat blueAffinity;
    public Stat goldAffinity;
    public Stat greenAffinity;
    public Stat indigoAffinity;

    public List<Stat> affinity;
    /*
    public static int endurance
    {
        get { return instance._endurance; }
        set
        {
            instance._endurance = value;
            instance._enduranceDisplay.value = endurance;
        }
    }
    public int focus
    {
        get { return instance._focus; }
        set { instance._focus = value; instance._focusDisplay.value = focus; }
    }
    public int maxFocus
    {
        get { return instance._maxFocus; }
        set { instance._maxFocus = value; instance._focusDisplay.baseValue = maxFocus; }
    }


    public int violetAffinity
    {
        set { instance._violetAffinity.value = value; }
        get
        {
            return instance._violetAffinity.value;
        }
    }
    public int redAffinity
    {
        set { instance._redAffinity.value = value; }
        get
        {
            return instance._redAffinity.value;
        }
    }
    public int goldAffinity
    {
        set { instance._goldAffinity.value = value; }
        get
        {
            return instance._goldAffinity.value;
        }
    }
    public int greenAffinity
    {
        set { instance._greenAffinity.value = value; }
        get
        {
            return instance._greenAffinity.value;
        }
    }
    public int blueAffinity
    {
        set { instance._blueAffinity.value = value; }
        get
        {
            return instance._blueAffinity.value;
        }
    }
    public int purpleAffinity
    {
        set { instance._purpleAffinity.value = value; }
        get
        {
            return instance._purpleAffinity.value;
        }
    }

    */

    public override void Awake()
    {
        base.Awake();
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        _affinityDisplay = new List<ValueDisplay>();
        _affinityDisplay.Add(_violetDisplay);
        _affinityDisplay.Add(_redDisplay);
        _affinityDisplay.Add(_goldDisplay);
        _affinityDisplay.Add(_greenDisplay);
        _affinityDisplay.Add(_blueDisplay);
        _affinityDisplay.Add(_indigoDisplay);

        violetAffinity = new Stat(0);
        redAffinity = new Stat(0);
        goldAffinity = new Stat(0);
        greenAffinity = new Stat(0);
        blueAffinity = new Stat(0);
        indigoAffinity = new Stat(0);

        affinity = new List<Stat>();
        affinity.Add(violetAffinity);
        affinity.Add(redAffinity);
        affinity.Add(goldAffinity);
        affinity.Add(greenAffinity);
        affinity.Add(blueAffinity);
        affinity.Add(indigoAffinity);

        endurance = new Stat(100);
        health = new Stat(0);
        maxHealth = new Stat(20);
        focus = new Stat(0);
        maxFocus = new Stat(3);
    }
    public override void Start()
    {
        base.Start();
        PlayerData data = Resources.Load("PlayerData") as PlayerData;
        _deck.Init(data.decklist);
        _weapon.Equip(data.weapon, this);
        _armor.Equip(data.armor, this);
        _relic.Equip(data.relic, this);

        StatusCondition[] buffs = _statusDisplays.GetComponentsInChildren<StatusCondition>();
        foreach (StatusCondition s in buffs)
        {
            s.gameObject.SetActive(false);
        }
        Refresh();
    }
    public override void Refresh()
    {
        base.Refresh();
        _focusDisplay.value = focus.value;
        _focusDisplay.baseValue = maxFocus.value;

        _enduranceDisplay.value = endurance.value;

        for (int ii = 0; ii < affinity.Count; ii++)
        {
            _affinityDisplay[ii].value = affinity[ii].value;
        }
    }
    public override void StartEncounter()
    {
        base.StartEncounter();
        focus.baseValue = maxFocus.value;
        //StartCoroutine(DoDraw(Dungeon.gameParams.playerHandSize));
    }
    public void Redraw()
    {
        StartCoroutine(DoRedraw());
    }
    public IEnumerator DoRedraw()
    {
        foreach (Card card in hand)
        {
            card.TriggerPassive();
        }
        yield return DoDiscardAll();
        yield return DoDraw(Dungeon.gameParams.playerHandSize);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ITargetable hovered = Targeter.HoveredTarget(eventData);
        if (hovered != null && hovered is Card)
        {
            Targeter.AddTarget(hovered);
        }
        Targeter.Clear();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Targeter.SetSource(this, Ability.Mode.INFUSE);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public override bool Resolve(Ability.Mode mode, List<ITargetable> targets)
    {
        Debug.Assert(mode == Ability.Mode.INFUSE);
        Ability.Infuse(this, (Card)targets[0]);
        return true;
    }


}
