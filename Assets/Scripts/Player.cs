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

    public bool burnAvailable = true;
    
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
        Debug.Log("Initializing player with decklist: " + GameData.playerDecklist.name);
        _deck.Init(GameData.playerDecklist);
        
        //_weapon.Equip(data.weapon, this);
        //_armor.Equip(data.armor, this);
        //_relic.Equip(data.relic, this);
        
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
        if (GameData.instance.playerDeck != null)
        {
            _deck.Init(GameData.instance.playerDeck);
        }
        _deck.Shuffle();
        focus.baseValue = maxFocus.value;
    }
    public void StartTurn()
    {
        StartCoroutine(DoStartTurn());
    }
    public void EndTurn()
    {
        List<Card> cards = Player.instance.active;
        foreach (Card card in cards)
        {
            if (card.needsUpkeep)
            {
                Player.instance.Discard(card);
            }
        }
    }
    public IEnumerator DoStartTurn()
    {
        burnAvailable = true;
        focus.baseValue = maxFocus.value;
        yield return DoRedraw();
        GameEvents.current.StartTurn(this);
        actorEvents.StartTurn();
        actorEvents.BeginTurn();

        Dungeon.SetConfirmButtonText("End Turn");
        Dungeon.EnableConfirmButton(true);
        Dungeon.SetParticleUnderlay(true);
        GameEvents.current.Refresh();
    }
    public void Redraw()
    {
        StartCoroutine(DoRedraw());
    }
    public IEnumerator DoRedraw()
    {
        /*
        foreach (Card card in hand)
        {
            if (card.GetStatus(StatusEffect.ID.MEMORIZED) <= 0)
            {
                card.TriggerPassive();
            }
        }
        */
        yield return DoDiscardAll();
        yield return DoDraw(GameData.instance.playerHandSize);
    }
    public override void AddFocus(int a_focus, int a_maxFocus = 0)
    {
        focus.baseValue += a_focus;
        maxFocus.baseValue += a_maxFocus;
    }
    public override void CycleDeck()
    {
        maxFocus.baseValue++;
        focus.baseValue++;
    }
    public void Burn(Card card)
    {
        Discard(card);
        addAffinity(card.data.color, 1);
        burnAvailable = false;
        Dungeon.ClearTargeter();
        GameEvents.current.Refresh();
    }
    public void addAffinity(Card.Color color, int delta)
    {
        switch (color)
        {
            case Card.Color.LIS: violetAffinity.baseValue += delta; break;
            case Card.Color.RAIZ: redAffinity.baseValue += delta; break;
            case Card.Color.ORA: goldAffinity.baseValue += delta; break;
            case Card.Color.FEN: greenAffinity.baseValue += delta; break;
            case Card.Color.IRI: blueAffinity.baseValue += delta; break;
            case Card.Color.VAEL: indigoAffinity.baseValue += delta; break;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ITargetable hovered = Targeter.HoveredTarget(eventData);
        if (hovered != null && hovered is Card)
        {
            Dungeon.targeter.AddTarget(hovered);
        }
        Dungeon.ClearTargeter();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Targeter.SetSource(this, Ability.Mode.INFUSE);
    }
    public void OnDrag(PointerEventData eventData)
    {
    }
    public int Affinity(Card.Color color)
    {
        switch (color)
        {
            case Card.Color.RAIZ: return redAffinity.value;
            case Card.Color.IRI: return blueAffinity.value;
            case Card.Color.FEN: return greenAffinity.value;
            case Card.Color.LIS: return violetAffinity.value;
            case Card.Color.ORA: return goldAffinity.value;
            case Card.Color.VAEL: return indigoAffinity.value;
            default: return 0;
        }
    }
    public override bool Resolve(Ability.Mode mode, List<ITargetable> targets)
    {
        //Debug.Assert(mode == Ability.Mode.INFUSE);
        //Ability.Infuse((Card)targets[0]);
        return true;
    }

 
}
