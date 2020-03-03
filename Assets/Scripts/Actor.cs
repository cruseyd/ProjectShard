using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour, ITargetable, IDamageable
{
    [SerializeField] protected ValueDisplay _healthDisplay;
    [SerializeField] protected GameObject _statusDisplays;
    [SerializeField] protected Deck _deck;
    [SerializeField] protected bool _playerControlled;

    [SerializeField] protected Equipment _weapon;
    [SerializeField] protected Equipment _armor;
    [SerializeField] protected Equipment _relic;

    protected List<ITargetable> _validTargets;
    protected CardZone _handZone;
    protected CardZone _activeZone;
    protected CardZone _discardZone;
    
    public Actor opponent
    {
        get
        {
            if (playerControlled) { return Enemy.instance; }
            else { return Player.instance; }
        }
    }
    public Actor controller
    {
        get
        {
            if (playerControlled) { return Player.instance; }
            else { return Enemy.instance; }
        }
    }
    public ActorEvents events;
    public Equipment weapon { get { return _weapon; } }
    public Equipment armor { get { return _armor; } }
    public Equipment relic { get { return _relic; } }
    
    protected Dictionary<StatusName, StatusEffect> _statusEffects;

    public ActorParticles particles;

    public bool playerControlled { get { return (this is Player); } }

    public Stat health;
    public Stat maxHealth;
    public Deck deck { get { return _deck; } }
    public Card[] hand {
        get {
            if (playerControlled)
            {
                return Dungeon.GetCards(CardZone.PLAYER_HAND);
            } else
            {
                return Dungeon.GetCards(CardZone.DUNGEON_HAND);
            }
        }
    }
    public Card[] active
    {
        get
        {
            if (playerControlled)
            {
                return Dungeon.GetCards(CardZone.PLAYER_ACTIVE);
            }
            else
            {
                return Dungeon.GetCards(CardZone.DUNGEON_ACTIVE);
            }
        }
    }
    public Card[] discard
    {
        get
        {
            if (playerControlled)
            {
                return Dungeon.GetCards(CardZone.PLAYER_DISCARD);
            }
            else
            {
                return Dungeon.GetCards(CardZone.DUNGEON_DISCARD);
            }
        }
    }

    public virtual void Awake()
    {
        _statusEffects = new Dictionary<StatusName, StatusEffect>();
        StatusDisplay[] displays = _statusDisplays.GetComponentsInChildren<StatusDisplay>();
        foreach (StatusDisplay tf in displays) { tf.gameObject.SetActive(false); }

        if (playerControlled)
        {
            _handZone = CardZone.PLAYER_HAND;
            _activeZone = CardZone.PLAYER_ACTIVE;
            _discardZone = CardZone.PLAYER_DISCARD;
        } else
        {
            _handZone = CardZone.DUNGEON_HAND;
            _activeZone = CardZone.DUNGEON_ACTIVE;
            _discardZone = CardZone.DUNGEON_DISCARD;
        }
        _validTargets = new List<ITargetable>();
        health = new Stat(0);
        maxHealth = new Stat(0);
        events = new ActorEvents(this);
    }
    public virtual void Start()
    {
        GameEvents.current.onQueryTarget += MarkTarget;
        GameEvents.current.onRefresh += Refresh;
    }

    public virtual void OnDestroy()
    {
        GameEvents.current.onQueryTarget -= MarkTarget;
        GameEvents.current.onRefresh -= Refresh;
    }
    public virtual void Discard(Card card)
    {
        if (card.inPlay)
        {
            ((CardEvents)card.events).Destroy();
        }
        card.FaceUp(true, false);
        Dungeon.MoveCard(card, _discardZone);
        card.particles.Clear();
    }

    public virtual void DiscardRandom()
    {
        if (hand.Length > 0)
        {
            int choice = Random.Range(0, hand.Length);
            Discard(hand[choice]);
        }
    }
    public virtual void PutInPlay(Card card)
    {
        Dungeon.MoveCard(card, _activeZone);
    }
    public virtual void Draw()
    {
        Card card = _deck.Draw();
        card.FaceUp(playerControlled, true);
        Dungeon.MoveCard(card, _handZone);
        card.Refresh();
        ((ActorEvents)events).DrawCard(card);
    }
    public virtual void Draw(int n) { StartCoroutine(DoDraw(n)); }
    public virtual void DiscardAll() { StartCoroutine(DoDiscardAll()); }
    public virtual IEnumerator DoDraw(int n)
    {
        float duration = Dungeon.gameParams.cardAnimationRate;
        for (int ii = 0; ii < n; ii++)
        {
            Draw();
            yield return new WaitForSeconds(duration);
        }
    }
    public virtual IEnumerator DoDiscardAll()
    {
        float duration = Dungeon.gameParams.cardAnimationRate;
        Card[] handCards = hand;
        foreach (Card card in handCards)
        {
            Discard(card);
            yield return new WaitForSeconds(duration);
        }
    }
    public virtual void Refresh()
    {
        _healthDisplay.value = health.value;
        _healthDisplay.baseValue = maxHealth.value;
        particles.Clear();
    }
    
    public virtual void StartEncounter()
    {
        health.baseValue = maxHealth.value;
    }
    public virtual void Damage(DamageData data)
    {
        if (data == null) { return; }
        if (data.source is Card)
        {
            Card src = ((Card)data.source);
            src.events.DealRawDamage(data);
            if (src.type != Card.Type.THRALL) { src.owner.events.DealRawDamage(data); }
            src.events.DealModifiedDamage(data);
            if (src.type != Card.Type.THRALL) { src.owner.events.DealModifiedDamage(data); }
            src.events.DealDamage(data);
            if (src.type != Card.Type.THRALL) { src.owner.events.DealDamage(data); }
        }
        else if (data.source is Actor)
        {
            Actor act = ((Actor)data.source);
            act.events.DealRawDamage(data);
            act.events.DealModifiedDamage(data);
            act.events.DealDamage(data);
        }
        events.TakeRawDamage(data);
        events.TakeModifiedDamage(data);
        events.TakeDamage(data);
        health.baseValue -= data.damage;
        
    }

    public virtual void ResolveDamage(DamageData data)
    {

    }
    // ITargetable Interface
    public Actor Controller() { return this; }
    public void AddTarget(ITargetable target)
    {
        _validTargets.Add(target);
    }
    public void FindTargets(Ability.Mode mode, int n, bool show = false)
    {
        _validTargets.Clear();
        GameEvents.current.QueryTarget(GetQuery(mode, n), this, show);
    }
    public void MarkTarget(TargetTemplate query, ITargetable source, bool show)
    {
        if (Compare(query, source.Controller()) && this != (Object)source)
        {
            if (show) { particles.MarkValidTarget(true); }
            source.AddTarget(this);
        } else if (this == (Object)source)
        {
            if (show) { particles.MarkSource(true); }
        } else 
        {
            particles.Clear();
        }
    }
    public bool Compare(TargetTemplate query, Actor self)
    {
        bool flag = true;
        // automatic disqualifiers
        if (query.cardColor != Card.Color.DEFAULT) { return false; }
        if (query.cardType != Card.Type.DEFAULT) { return false; }
        if (query.templateParams.Count > 0) { return false; }

        // actual checks
        if (query.isOpposing)
        {
            flag &= (this != self);
        }
        if (query.isSelf)
        {
            flag &= (this == self);
        }
        if (query.isAttackable)
        {
            foreach (Card card in active)
            {
                if (card.type == Card.Type.THRALL) { return false; }
            }
        }
        return flag;
    }
    public TargetTemplate GetQuery(Ability.Mode mode, int n)
    {
        Debug.Assert(mode == Ability.Mode.INFUSE);
        Debug.Assert(n == 0);
        TargetTemplate t = new TargetTemplate();
        t.cardType = Card.Type.THRALL;
        t.inPlay = true;
        t.isSelf = true;
        return t;
    }
    public virtual bool Resolve(Ability.Mode mode, List<ITargetable> targets)
    {
        return true;
    }
    public virtual List<ICommand> FindMoves()
    {
        List<ICommand> moves = new List<ICommand>();
        return moves;
    }
    public virtual void AddStatus(StatusName id, int stacks = 1)
    {
        if (_statusEffects.ContainsKey(id))
        {
            _statusEffects[id].stacks += stacks;
            events.GainStatus(_statusEffects[id], stacks);
        } else
        {
            int n = _statusEffects.Count;
            StatusDisplay display = _statusDisplays.transform.GetChild(n).GetComponent<StatusDisplay>();
            StatusEffect s = new StatusEffect(id, this, display, stacks);
            _statusEffects[id] = s;
            events.GainStatus(_statusEffects[id], stacks);
        }
    }
    public virtual void RemoveStatus(StatusName id, int stacks = 1)
    {
        if (_statusEffects.ContainsKey(id))
        {
            StatusEffect s = _statusEffects[id];
            events.RemoveStatus(s, stacks);
            if (stacks >= s.stacks)
            {
                _statusEffects[id].Remove();
                _statusEffects.Remove(id);
            } else
            {
                s.stacks -= stacks;
            }
        }
    }
    public virtual int GetStatus(StatusName id)
    {
        if (_statusEffects.ContainsKey(id))
        {
            return _statusEffects[id].stacks;
        }
        else
        {
            return 0;
        }
    }
}
