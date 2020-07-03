using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour, ITargetable
{
    [SerializeField] protected ValueDisplay _healthDisplay;
    [SerializeField] protected Transform _statusDisplays;
    [SerializeField] protected Deck _deck;
    [SerializeField] protected bool _playerControlled;

    [SerializeField] protected Equipment _weapon;
    [SerializeField] protected Equipment _armor;
    [SerializeField] protected Equipment _relic;

    protected List<ITargetable> _validTargets;
    protected ActorEvents _actorEvents;
    protected TargetEvents _targetEvents;
    protected List<Card> _playedThisTurn;

    CardZone handZone;
    CardZone activeZone;
    CardZone discardZone;

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
    public bool inPlay { get { return true; } }
    public ActorEvents actorEvents { get { return _actorEvents; } }
    public TargetEvents targetEvents { get { return _targetEvents; } }
    public Equipment weapon { get { return _weapon; } }
    public Equipment armor { get { return _armor; } }
    public Equipment relic { get { return _relic; } }
    
    protected Dictionary<StatusEffect.ID, StatusEffect> _statusEffects;

    public ActorParticles particles;

    public bool playerControlled { get { return (this is Player); } }

    public Stat health;
    public Stat maxHealth;
    public Deck deck { get { return _deck; } }
    public List<Card> hand {
        get {
            return Dungeon.GetCards(CardZone.Type.HAND, playerControlled);
        }
    }
    public List<Card> active
    {
        get
        {
            return Dungeon.GetCards(CardZone.Type.ACTIVE, playerControlled);
        }
    }
    public List<Card> discard
    {
        get
        {
            return Dungeon.GetCards(CardZone.Type.DISCARD, playerControlled);
        }
    }

    public virtual void Awake()
    {
        _statusEffects = new Dictionary<StatusEffect.ID, StatusEffect>();
        //StatusDisplay[] displays = _statusDisplays.GetComponentsInChildren<StatusDisplay>();
        //foreach (StatusDisplay tf in displays) { tf.gameObject.SetActive(false); }

        
        _playedThisTurn = new List<Card>();
        _validTargets = new List<ITargetable>();
        health = new Stat(0);
        maxHealth = new Stat(0);
        _actorEvents = new ActorEvents(this);
        _targetEvents = new TargetEvents(this);
    }
    public virtual void Start()
    {
        handZone = Dungeon.GetZone(CardZone.Type.HAND, playerControlled);
        activeZone = Dungeon.GetZone(CardZone.Type.ACTIVE, playerControlled);
        discardZone = Dungeon.GetZone(CardZone.Type.DISCARD, playerControlled);

        GameEvents.current.onQueryTarget += MarkTarget;
        GameEvents.current.onRefresh += Refresh;
        actorEvents.onPlayCard += AddToPlayed;
        actorEvents.onPostTurn += ClearPlayed;
    }

    public virtual void OnDestroy()
    {
        GameEvents.current.onQueryTarget -= MarkTarget;
        GameEvents.current.onRefresh -= Refresh;
    }
    public virtual void Discard(Card card)
    {
        card.FaceUp(true, false);
        card.Move(discardZone);
        card.particles.Clear();
    }
    public virtual void DiscardRandom()
    {
        if (hand.Count > 0)
        {
            int choice = Random.Range(0, hand.Count);
            Discard(hand[choice]);
        }
    }
    public virtual void PutInPlay(Card card, bool selfControl = true)
    {
        card.FaceUp(true, true);
        if (selfControl) { card.Move(activeZone); }
        else {
            card.Move(opponent.activeZone);
            card.SwitchController();
        }
    }
    public virtual void AddToHand(Card card)
    {
        card.FaceUp(playerControlled, true);
        card.Move(handZone);
        card.Refresh();
    }
    public virtual void Draw()
    {
        Card card = _deck.Draw();
        if (card != null)
        {
            AddToHand(card);
            actorEvents.DrawCard(card);
            card.cardEvents.Draw();
        }
    }
    public virtual void Draw(int n) { StartCoroutine(DoDraw(n)); }
    //public virtual void DiscardAll() { StartCoroutine(DoDiscardAll()); }
    public virtual IEnumerator DoDraw(int n)
    {
        float duration = GameData.instance.cardAnimationRate;
        for (int ii = 0; ii < n; ii++)
        {
            Draw();
            yield return new WaitForSeconds(duration);
        }
    }
    public virtual IEnumerator DoDiscardAll()
    {
        float duration = GameData.instance.cardAnimationRate;
        List<Card> handCards = hand;
        foreach (Card card in handCards)
        {
            Attempt attempt = new Attempt();
            card.cardEvents.TryCycle(attempt);
            if (!attempt.success) { continue; }
            /*
            if (card.GetStatus(StatusEffect.ID.MEMORIZED) > 0)
            {
                card.RemoveStatus(StatusEffect.ID.MEMORIZED);
                continue;
            }
            */
            Discard(card);
            card.cardEvents.Cycle();
            yield return new WaitForSeconds(duration);
        }
    }
    public virtual void Refresh()
    {
        _healthDisplay.value = health.value;
        _healthDisplay.baseValue = maxHealth.value;
        particles.Clear();
        targetEvents.Refresh();
    }
    
    public virtual void StartEncounter()
    {
        health.baseValue = maxHealth.value;
    }

    public List<Card> GetCardsWithKeyword(Keyword key, CardZone.Type zone)
    {
        List<Card> matches = new List<Card>();
        List<Card> cards = new List<Card>();
        switch (zone)
        {
            case CardZone.Type.HAND: cards = hand; break;
            case CardZone.Type.ACTIVE: cards = active; break;
            case CardZone.Type.DISCARD: cards = discard; break;
        }
        foreach (Card card in cards)
        {
            if (card.HasKeyword(key)) { matches.Add(card); }
        }
        return matches;
    }
    public virtual void IncrementHealth(int value)
    {
        health.baseValue += value;
        if (value > 0)
        {
            targetEvents.GainHealth(value);
        } else if (value < 0)
        {
            targetEvents.LoseHealth(-value);
        }
    }
    public virtual void Damage(DamageData data)
    {
        if (data == null) { return; }
        if (data.source != null)
        {
            data.source.targetEvents.DealRawDamage(data);
            if (!(data.source is Actor)) { data.source.controller.targetEvents.DealRawDamage(data); }
            data.source.targetEvents.DealModifiedDamage(data);
            if (!(data.source is Actor)) { data.source.controller.targetEvents.DealModifiedDamage(data); }
            
        }

        targetEvents.TakeRawDamage(data);
        targetEvents.TakeModifiedDamage(data);

        health.baseValue -= data.damage;
        
        if (data.source != null)
        {
            data.source.targetEvents.DealDamage(data);
            if (!(data.source is Actor)) { data.source.controller.targetEvents.DealDamage(data); }
        }
        targetEvents.TakeDamage(data);

        if (data.damage > 0)
        {
            targetEvents.LoseHealth(data.damage);
        }
        ResolveDamage(data);
    }
    public virtual void ResolveDamage(DamageData data)
    {
        if (health.value <= 0)
        {
            if (!GameData.instance.invincible)
            {
                if (!playerControlled) { Dungeon.instance.Victory(); }
                else { Dungeon.instance.Defeat(); }
            }
        }
    }
    // ITargetable Interface
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
        if (Compare(query, source.controller) && this != (Object)source)
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
        if (query.isNot != null && (query.isNot.Equals(this))) { return false; }
        if (query.cardColor.Count > 0) { return false; }
        if (query.cardType.Count > 0) { return false; }
        if (query.keyword.Count > 0) { return false; }
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
                if (card.type == Card.Type.THRALL && card.canBlock) { return false; }
            }
        }
        return flag;
    }
    public TargetTemplate GetQuery(Ability.Mode mode, int n)
    {
        //Debug.Assert(mode == Ability.Mode.INFUSE);
        Debug.Assert(n == 0);
        TargetTemplate t = new TargetTemplate();
        t.cardType.Add(Card.Type.THRALL);
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
    public virtual void AddStatus(StatusEffect.ID id, int stacks = 1)
    {
        Attempt attempt = new Attempt();
        targetEvents.TryGainStatus(id, stacks, attempt);
        if (!attempt.success)
        {
            Debug.Log("Prevented " + id.ToString());
            return;
        }
        if (_statusEffects.ContainsKey(id))
        {
            _statusEffects[id].stacks += stacks;
            targetEvents.GainStatus(_statusEffects[id], stacks);
        } else
        {
            StatusEffect effect = StatusEffect.Spawn(id, this, stacks);
            effect.transform.parent = _statusDisplays;
            _statusEffects[id] = effect;
        }
    }
    public virtual void RemoveStatus(StatusEffect.ID a_id, int a_stacks = 9999)
    {
        Debug.Log("Removing " + a_stacks + " stacks of " + a_id.ToString());
        if (_statusEffects.ContainsKey(a_id))
        {
            StatusEffect s = _statusEffects[a_id];
            
            targetEvents.RemoveStatus(s, a_stacks);
            if (a_stacks >= s.stacks)
            {
                _statusEffects[a_id].Remove();
                _statusEffects.Remove(a_id);
            } else
            {
                s.stacks -= a_stacks;
            }
        }
    }
    public virtual int GetStatus(StatusEffect.ID id)
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

    public int NumPlayedThisTurn(TargetTemplate template)
    {
        int n = 0;
        foreach (Card card in _playedThisTurn)
        {
            if (card.Compare(template, this)) { n++; }
        }
        return n;
    }

    public bool PlayedThisTurn(Card card)
    {
        return _playedThisTurn.Contains(card);
    }

    protected void AddToPlayed(Card card)
    {
        _playedThisTurn.Add(card);
    }

    protected void ClearPlayed(Actor actor) { _playedThisTurn.Clear(); }
}
