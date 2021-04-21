using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IEndDragHandler, IDragHandler, ITargetable
{
    public enum Type
    {
        DEFAULT,
        SPELL,
        TECHNIQUE,
        THRALL, 
        CONSTANT,
        ITEM,
        AFFLICTION,
        IDEAL
    }
    public enum Color
    {
        DEFAULT,
        ORA,
        RAIZ,
        FEN,
        IRI,
        LIS,
        VAEL,
        NEUTRAL
    }
    public enum Attribute
    {
        STRENGTH,
        FINESSE,
        PERCEPTION
    }

    public enum Rarity
    {
        COMMON,
        SCARCE,
        RARE,
        MYTHIC,
        TOKEN
    }

    private static GameObject _cardPrefab;

    [SerializeField] private Transform _statusDisplays;

    public CardParticles particles;
    public CardGraphic graphic;

    private Ability _ability;
    private List<KeywordAbility> _kAbilities;
    private List<KeywordAbility.Key> _kAbilityKeys;
    private CardEvents _cardEvents;
    private TargetEvents _targetEvents;
    private Dictionary<StatusEffect.ID, StatusEffect> _statusEffects;
    private List<ITargetable> _validTargets;

    private bool _followingCursor = false;
    private bool _needsUpkeep;

    public bool activationAvailable;
    public bool attackAvailable;
    public bool blockAvailable;
    [SerializeField] private bool _playerControlled;
    public bool playerControlled { get { return _playerControlled; } }
    public int zoneIndex;
    public CardZone zone { get { return graphic.zone; } }

    public CardData data { get { return graphic.data; } }
    public Actor controller
    {
        get
        {
            if (playerControlled) { return Player.instance; }
            else { return Enemy.instance; }
        }
    }
    public Actor opponent
    {
        get
        {
            if (!playerControlled) { return Player.instance; }
            else { return Enemy.instance; }
        }
    }
    public Ability ability
    {
        get
        {
            return _ability;
        }
    }
    public TargetEvents targetEvents { get { return _targetEvents; } }
    public CardEvents cardEvents { get { return _cardEvents; } }
    public new string name { get { return data.name; } }
    public Card.Type type { get { return data.type; } }


    public Stat cost;

    public Stat strength;
    public Stat finesse;
    public Stat perception;

    public Stat power;
    public Stat health { get { return endurance; } }
    public Stat endurance;
    public Stat upkeep;

    public List<Stat> values;

    public int violetAffinity { get { return data.violetAffinity; } }
    public int redAffinity    { get { return data.redAffinity; } }
    public int goldAffinity   { get { return data.goldAffinity; } }
    public int greenAffinity  { get { return data.greenAffinity; } }
    public int blueAffinity   { get { return data.blueAffinity; } }
    public int indigoAffinity { get { return data.indigoAffinity; } }
    public Keyword damageType { get { return data.damageType; } }

    public bool resourcesAvailable
    {
        get
        {
            bool flag = true;
            if (GameData.instance.ignoreResources) { return true; }
            if (playerControlled)
            {
                flag &= (cost.value <= Player.instance.focus.value);

                flag &= (violetAffinity <= Player.instance.violetAffinity.value);
                flag &= (redAffinity <= Player.instance.redAffinity.value);
                flag &= (goldAffinity <= Player.instance.goldAffinity.value);
                flag &= (greenAffinity <= Player.instance.greenAffinity.value);
                flag &= (blueAffinity <= Player.instance.blueAffinity.value);
                flag &= (indigoAffinity <= Player.instance.indigoAffinity.value);
            }
            return flag;
        }
    }
    public bool playable
    {
        get
        {
            if (zone == null) { return false; }
            bool flag = true;
            flag &= (zone.type == CardZone.Type.HAND);
            if (type == Type.IDEAL)
            {
                TargetTemplate template = new TargetTemplate();
                template.cardType.Add(Type.IDEAL);
                if (controller.NumPlayedThisTurn(template) > 0) { return false; }
            }

            if (playerControlled && (Dungeon.phase == GamePhase.player))
            {
                flag &= Dungeon.priority;
                flag &= resourcesAvailable;
                
            } else if (playerControlled && (Dungeon.phase == GamePhase.enemy))
            {
                flag &= Dungeon.priority;
                flag &= (ability != null && react);
                flag &= resourcesAvailable;
            }
            else if (!playerControlled && (Dungeon.phase == GamePhase.enemy))
            {
                flag &= (zone.type == CardZone.Type.HAND);
            } else
            {
                flag = false;
            }
            return flag;
        }
        
    }
    public bool activatable
    {
        get
        {
            bool flag = true;
            flag &= (!needsUpkeep);
            flag &= activationAvailable;
            flag &= inPlay;
            flag &= ability.ActivationAvailable();
            if (playerControlled)
            {
                flag &= (Dungeon.phase == GamePhase.player);
            } else
            {
                flag &= (Dungeon.phase == GamePhase.enemy);
            }
            return flag;
        }
    }
    public bool canAttack
    {
        get
        {
            bool flag = true;
            flag &= (!needsUpkeep);
            flag &= (!HasKeyword(KeywordAbility.Key.PASSIVE));
            flag &= attackAvailable;
            flag &= (type == Type.THRALL);
            flag &= (zone.type == CardZone.Type.ACTIVE);
            if (playerControlled)
            {
                flag &= (Dungeon.phase != GamePhase.enemy);
            }
            else
            {
                flag &= (Dungeon.phase == GamePhase.enemy);
            }
            return flag;

        }
    }
    public bool canDefend
    {
        get
        {
            Card attackingCard = Dungeon.magnifiedCard;
            if (attackingCard == null) { return false; }
            List<Attribute> attributes = attackingCard.maxAttributes();

            bool flag = false;
            foreach (Attribute a in attributes)
            {
                flag |= (GetAttribute(a) >= attackingCard.GetAttribute(a));
            }
            flag &= resourcesAvailable;
            flag &= (zone.type == CardZone.Type.HAND);
            flag &= (Dungeon.phase == GamePhase.enemy);
            flag &= playerControlled;
            flag &= (attackingCard.counterable);
            return flag;
        }
    }
    public bool canBlock
    {
        get
        {
            bool flag = true;
            flag &= inPlay;
            flag &= (type == Card.Type.THRALL);
            flag &= blockAvailable;
            return flag;
        }
    }
    public bool react
    {
        get
        {
            return HasKeyword(KeywordAbility.Key.QUICK);
        }
    }
    public bool counterable
    {
        get
        {
            return (data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
        }
    }

    public bool attackable
    {
        get
        {
            bool flag = true;
            flag &= (type == Card.Type.THRALL);
            /*
            if (!HasKeyword(KeywordAbility.Key.GUARDIAN))
            {
                List<Card> cards = controller.active;
                foreach (Card card in cards)
                {
                    if (card == this) { continue; }
                    if (card.HasKeyword(KeywordAbility.Key.GUARDIAN) && card.canBlock) { return false; }
                }
            }
            if (HasKeyword(KeywordAbility.Key.ELUSIVE))
            {
                List<Card> cards = controller.active;
                foreach (Card card in cards)
                {
                    if (card == this) { continue; }
                    if (!card.HasKeyword(KeywordAbility.Key.ELUSIVE) && card.canBlock) { return false; }
                }
            }
            */
            return flag;
        }
    }
    public bool needsTarget
    {
        get
        {
            if (inHand && _ability.NumTargets(Ability.Mode.PLAY) > 0) { return true; }
            else if (inPlay && _ability.NumTargets(Ability.Mode.ACTIVATE) > 0) { return true; }
            else if (canAttack) { return true; }
            else { return false; }
        }
    }
    public bool inPlay
    {
        get
        {
            return (zone.type == CardZone.Type.ACTIVE);
        }
    }
    public bool inHand
    {
        get
        {
            return (zone.type == CardZone.Type.HAND);
        }
    }
    public bool inDiscard
    {
        get
        {
            return (zone.type == CardZone.Type.DISCARD);
        }
    }
    public bool needsUpkeep
    {
        get
        {
            bool flag = true;
            flag &= (playerControlled);
            flag &= (type == Card.Type.THRALL || type == Card.Type.CONSTANT);
            flag &= (upkeep != null && upkeep.value > 0);
            flag &= (_needsUpkeep);
            return flag;
        }
    }
    public bool playedThisTurn
    {
        get
        {
            bool flag = true;
            flag &= inDiscard;
            flag &= controller.PlayedThisTurn(this);
            return flag;
        }
    }

    public bool validTarget
    {
        get
        {
            if (Dungeon.targeter != null && Dungeon.targeter.source is Card)
            {
                Card source = Dungeon.targeter.source as Card;
                if (source._validTargets.Contains(this)) { return true; }
            }
            return false;
        }
    }

    public static Card Spawn(string id, bool isPlayerCard, Vector3 spawnPoint)
    {
        CardData data = CardIndex.Get(id);
        if (data != null) { return Spawn(data, isPlayerCard, spawnPoint); }
        return null;
    }
    public static Card Spawn(CardData data, bool isPlayerCard, Vector3 spawnPoint)
    {
        if (Card._cardPrefab == null)
        {
            _cardPrefab = Resources.Load("Prefabs/LayeredCard") as GameObject;
        }
        GameObject cardGO = Instantiate(_cardPrefab,spawnPoint, Quaternion.identity);
        Card card = cardGO.GetComponent<Card>();
        card.graphic = cardGO.GetComponent<CardGraphic>();
        card.graphic.Initialize(data);
        card.FaceUp(false);
        card.particles = cardGO.GetComponent<CardParticles>();

        // DATA AND STATE
        card._validTargets = new List<ITargetable>();
        card._statusEffects = new Dictionary<StatusEffect.ID, StatusEffect>();
        card._cardEvents = new CardEvents(card);
        card._targetEvents = new TargetEvents(card);
        card._playerControlled = isPlayerCard;
        card.attackAvailable = false;
        card.activationAvailable = true;
        card._needsUpkeep = false;
        card.blockAvailable = true;

        // STAT BASE VALUES
        card.cost = new Stat(data.level);
        card.strength = new Stat(data.strength);
        card.finesse = new Stat(data.finesse);
        card.perception = new Stat(data.perception);
        if (data.type == Type.THRALL)
        {
            card.power = new Stat(data.power);
            card.endurance = new Stat(data.endurance);
            card.upkeep = new Stat(data.upkeep);
        }
        if (data.type == Type.CONSTANT)
        {
            card.endurance = new Stat(data.endurance);
            card.upkeep = new Stat(data.upkeep);
        }
        card.values = new List<Stat>();
        foreach (int val in data.values)
        {
            card.values.Add(new Stat(val));
        }
        // EVENTS
        if (card.controller != null)
        {
            card.controller.actorEvents.onStartTurn += card.ResetFlags;
            if (card.type == Card.Type.CONSTANT)
            {
                card.controller.targetEvents.onTakeDamage += card.DamageConstant;
            }
        }
        GameEvents.current.onQueryTarget += card.MarkTarget;
        GameEvents.current.onRefresh += card.Refresh;
        card.targetEvents.onDealRawDamage += (damageData) => { card.controller.targetEvents.DealRawDamage(damageData); };
        card.targetEvents.onDealModifiedDamage += (damageData) => { card.controller.targetEvents.DealModifiedDamage(damageData); };
        card.targetEvents.onDealDamage += (damageData) => { card.controller.targetEvents.DealDamage(damageData); };
        card.targetEvents.onDealOverflowDamage += (damageData) => { card.controller.targetEvents.DealOverFlowDamage(damageData); };

        // ABILITY
        card._kAbilities = new List<KeywordAbility>();
        card._kAbilityKeys = new List<KeywordAbility.Key>();
        foreach (KeywordAbility.Key key in data.abilityKeywords)
        {
            card._kAbilityKeys.Add(key);
            card._kAbilities.Add(KeywordAbility.Get(key, card));
        }
        card._ability = AbilityIndex.Get(data.id, card);

        GameEvents.current.SpawnCard(card);
        return card;
    }
    public void OnDestroy()
    {
        GameEvents.current.onRefresh -= Refresh;
        GameEvents.current.onQueryTarget -= MarkTarget;
        controller.targetEvents.onTakeDamage -= DamageConstant;
        controller.actorEvents.onStartTurn -= ResetFlags;
    }
    public int GetAttribute(Attribute a)
    {
        switch (a)
        {
            case Attribute.STRENGTH: return strength.value;
            case Attribute.FINESSE: return finesse.value;
            case Attribute.PERCEPTION: return perception.value;
            default: return -1;
        }
    }
    public List<Attribute> maxAttributes()
    {
        List<Card.Attribute> attr = new List<Card.Attribute>();
        if (strength.value >= finesse.value && strength.value >= perception.value)
        { attr.Add(Attribute.STRENGTH); }
        if (finesse.value >= strength.value && finesse.value >= perception.value)
        { attr.Add(Attribute.FINESSE); }
        if (perception.value >= strength.value && perception.value >= finesse.value)
        { attr.Add(Attribute.PERCEPTION); }
        return attr;
    }
    public void ResetFlags(Actor actor)
    {
        // input is due to event requirement
        if (inPlay)
        {
            blockAvailable = true;
            _needsUpkeep = true;
            activationAvailable = true;
            attackAvailable = true;
        } 
    }
    public void Refresh()
    {
        targetEvents.Refresh();
        foreach (TemplateModifier mod in Dungeon.modifiers)
        {
            if (mod.Compare(this))
            {
                mod.SetTarget(this);
            } else
            {
                mod.RemoveTarget(this);
            }
        }

        if (playable)
        {
            if (!Dungeon.targeting || (Dungeon.targeting && react))
            {
                particles.ShimmerGold();
            }
        }
        else if ((activatable || canAttack) && !Dungeon.targeting)
        {
            particles.ShimmerGold();
        }
        else if (canDefend || validTarget)
        {
            particles.ShimmerBlue();
        }
        else if (needsUpkeep && !Dungeon.targeting)
        {
            particles.ShimmerRed();
        }
        else
        {
            particles.ClearShimmer();
        }
        RefreshText();
        
    }
    public void RefreshText()
    {
        graphic.SetStat(Stat.Name.COST, cost.value);

        if (type == Type.THRALL)
        {
            graphic.SetStat(Stat.Name.POWER, power.value);
        }
        if (type == Type.CONSTANT || type == Type.THRALL)
        {
            graphic.SetStat(Stat.Name.ENDURANCE, endurance.value);
            graphic.SetStat(Stat.Name.UPKEEP, upkeep.value);
        }
        if (type == Type.SPELL || type == Type.TECHNIQUE)
        {
            graphic.SetStat(Stat.Name.STRENGTH, strength.value);
            graphic.SetStat(Stat.Name.PERCEPTION, perception.value);
            graphic.SetStat(Stat.Name.FINESSE, finesse.value);
        }
        graphic.ParseText(data, values, _kAbilityKeys);
    }
    public bool AddModifier(StatModifier mod)
    {
        switch (mod.statName)
        {
            case Stat.Name.COST: 
                if (cost == null) { return false; }
                else 
                {
                    return cost.AddModifier(mod);
                }
            case Stat.Name.POWER:
                if (power == null) { return false; }
                else
                {
                    return power.AddModifier(mod);
                }
            case Stat.Name.ENDURANCE:
                if (endurance == null) { return false; }
                else
                {
                    return endurance.AddModifier(mod);
                }
            case Stat.Name.UPKEEP:
                if (upkeep == null) { return false; }
                else
                {
                    return upkeep.AddModifier(mod);
                }
            case Stat.Name.STRENGTH:
                if (strength == null) { return false; }
                else
                {
                    return strength.AddModifier(mod);
                }
            case Stat.Name.PERCEPTION:
                if (perception == null) { return false; }
                else
                {
                    return perception.AddModifier(mod);
                }
            case Stat.Name.FINESSE:
                if (finesse == null) { return false; }
                else
                {
                    return finesse.AddModifier(mod);
                }
            default: return false;
        }
    }
    public bool RemoveModifier(StatModifier mod)
    {
        switch (mod.statName)
        {
            case Stat.Name.COST:
                if (cost == null) { return false; }
                else
                {
                    return cost.RemoveModifier(mod);
                }
            case Stat.Name.POWER:
                if (power == null) { return false; }
                else
                {
                    return power.RemoveModifier(mod);
                }
            case Stat.Name.ENDURANCE:
                if (endurance == null) { return false; }
                else
                {
                    return endurance.RemoveModifier(mod);
                }
            case Stat.Name.UPKEEP:
                if (upkeep == null) { return false; }
                else
                {
                    return upkeep.RemoveModifier(mod);
                }
            case Stat.Name.STRENGTH:
                if (strength == null) { return false; }
                else
                {
                    return strength.RemoveModifier(mod);
                }
            case Stat.Name.PERCEPTION:
                if (perception == null) { return false; }
                else
                {
                    return perception.RemoveModifier(mod);
                }
            case Stat.Name.FINESSE:
                if (finesse == null) { return false; }
                else
                {
                    return finesse.RemoveModifier(mod);
                }
            default: return false;
        }
    }
    public void AddKeywordAbility(KeywordAbility.Key key)
    {
        if (_kAbilityKeys.Contains(key)) { return; }
        _kAbilityKeys.Add(key);
        _kAbilities.Add(KeywordAbility.Get(key, this));
        RefreshText();
    }
    public void RemoveKeywordAbility(KeywordAbility.Key key)
    {
        if (!_kAbilityKeys.Contains(key)) { return; }
        foreach (KeywordAbility.Key k in _kAbilityKeys)
        {
            if (k == key)
            {
                _kAbilityKeys.Remove(k);
                _kAbilities.Remove(KeywordAbility.Get(key, this));
            }
        }
        RefreshText();
    }
    public bool Resolve(Ability.Mode mode, List<ITargetable> targets)
    {
        bool interrupted = false;
        if (_ability.NumTargets(mode) > 0)
        {
            if (targets == null || targets.Count < _ability.NumTargets(mode)) { return false; }
        }
        if (mode == Ability.Mode.PLAY)
        {
            Attempt attempt = new Attempt();
            controller.actorEvents.TryPlayCard(this, attempt);
            if (!attempt.success) { interrupted = true; }
            if (playerControlled)
            {
                Player.instance.focus.baseValue -= cost.value;
            }
        }
        if (targets != null)
        {
            foreach (ITargetable target in targets)
            {
                controller.targetEvents.DeclareTarget(this, target);
                targetEvents.DeclareTarget(this, target);
            }
        }
        if (mode == Ability.Mode.PLAY)
        {
            if (!interrupted) { controller.actorEvents.PlayCard(this); }
            if ((type == Type.THRALL || type == Type.CONSTANT) && !interrupted)
            {
                controller.PutInPlay(this);
                if (type == Type.CONSTANT)
                {
                    _ability?.Use(Ability.Mode.ACTIVATE, targets);
                    activationAvailable = false;
                }
            } else
            {
                _ability?.Use(Ability.Mode.PLAY, targets);
                controller.Discard(this);
            }
        } else if (mode == Ability.Mode.ACTIVATE)
        {
            if (!interrupted)
            {
                controller.actorEvents.ActivateCard(this);
                _ability?.Use(Ability.Mode.ACTIVATE, targets);
            }
            activationAvailable = false;
        } else if (mode == Ability.Mode.ATTACK)
        {
            _ability?.Use(Ability.Mode.ATTACK, targets);
        }
        Dungeon.ClearTargeter();
        GameEvents.current.Refresh();
        return true;
    }
    public void Defend()
    {
        if (!canDefend) { return; }
        Player.instance.focus.baseValue -= cost.value;
        ((EnemyTurnPhase)Dungeon.phase).Interrupt();
        Player.instance.Discard(this);
    }
    public void Damage(DamageData data)
    {
        if (data == null || !inPlay) { return; }
        Debug.Assert(type == Type.THRALL || type == Type.CONSTANT);

        if (data.source != null)
        {
            data.source.targetEvents.DealRawDamage(data);
            data.source.targetEvents.DealModifiedDamage(data);
        }

        targetEvents.TakeRawDamage(data);
        targetEvents.TakeModifiedDamage(data);

        int overflowDamage = data.damage - endurance.value;
        DamageData overflow = new DamageData(overflowDamage, data.type, data.source, data.target, data.isAttackDamage);
       
        endurance.baseValue -= data.damage;

        if (data.damage > 0)
        {
            if (data.source != null)
            {
                data.source.targetEvents.DealDamage(data);
                if (overflowDamage > 0)
                {
                    data.source.targetEvents.DealOverFlowDamage(overflow);
                }
            }
            targetEvents.TakeDamage(data);
            if (type == Card.Type.THRALL)
            {
                targetEvents.LoseHealth(data.damage);
            }
        }
        GameEvents.current.CardDamaged(data);
        controller.actorEvents.CardDamaged(data);
        ResolveDamage(data);
    }
    public void DamageConstant(DamageData data)
    {
        Debug.Assert(type == Card.Type.CONSTANT);
        Damage(new DamageData(1, Keyword.DEFAULT, data.source, this));
        if (endurance.value <= 0) { Delete(); }
    }
    public bool Upkeep()
    {
        if (!needsUpkeep) { return false; }
        if (((Player)controller).focus.value < upkeep.value && !GameData.instance.ignoreResources) { return false; }
        ((Player)controller).focus.baseValue -= upkeep.value;
        _needsUpkeep = false;
        if (type == Type.CONSTANT)
        {
            _ability?.Use(Ability.Mode.ACTIVATE, null);
            activationAvailable = false;
        }
        GameEvents.current.Refresh();
        return true;
    }

    public void ResolveDamage(DamageData data)
    {
        if (type == Card.Type.THRALL && endurance.value <= 0 && inPlay)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        if (inPlay)
        {
            controller.Discard(this);
            GameEvents.current.CardDestroyed(this);
            cardEvents.Destroy();
            IncrementHealth(9999);
            RemoveAllStatus();
        }
    }
    public void IncrementHealth(int value)
    {
        if (type != Card.Type.THRALL) { return; }
        int prev = endurance.value;
        endurance.baseValue = Mathf.Clamp(endurance.baseValue + value, 0, data.endurance);
        if (endurance.value > prev)
        {
            targetEvents.GainHealth(value);
        }
        else if (endurance.value < prev)
        {
            targetEvents.LoseHealth(-value);
        }
    }

    // Motion
    public void DoubleClick()
    {
        Debug.Log("activatable: " + activatable);
        if (canDefend)
        {
            Defend();
        }
        else if (Dungeon.targeting)
        {
            bool valid = Compare(Dungeon.targeter.query, Dungeon.targeter.source.controller);
            if (valid)
            {
                Dungeon.targeter.AddTarget(this);
            }
        }
        else if (needsUpkeep)
        {
            Upkeep();
        }
        else if (activatable)
        {
            if (_ability.NumTargets(Ability.Mode.ACTIVATE) > 0)
            {
                Dungeon.SetTargeter(this, Ability.Mode.ACTIVATE);
            } else
            {
                ability?.Use(Ability.Mode.ACTIVATE, null);
                GameEvents.current.Refresh();
                Debug.Log("activatable: " + activatable);
            }
        }
    }

    public void FaceUp(bool flag, bool animate = false) { graphic.FaceUp(flag, animate); }
    public void SwitchController() { _playerControlled = !_playerControlled; }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_followingCursor) { return; }
        //Debug.Log("PointerEnter: " + data.name);
        if (zone.type == CardZone.Type.ACTIVE || zone.type == CardZone.Type.HAND || zone.type == CardZone.Type.MAGNIFY)
        {
            graphic.Zoom(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_followingCursor) { return; }
        //Debug.Log("PointerExit: " + data.name);
        if (zone.type == CardZone.Type.ACTIVE || zone.type == CardZone.Type.HAND || zone.type == CardZone.Type.MAGNIFY)
        {
            graphic.Zoom(false);
        }
        //Dungeon.FixLayering();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!playerControlled) { return; }
        //Debug.Log("BeginDrag: " + data.name);
        Dungeon.EnableDropZones(true);
        if (needsTarget)
        {
            if (playable)
            {
                Dungeon.SetTargeter(this, Ability.Mode.PLAY);
            }
            else if (canAttack)
            {
                Dungeon.SetTargeter(this, Ability.Mode.ATTACK);
            }
        } else
        {
            _followingCursor = true;
        }
        transform.SetAsLastSibling();
        transform.parent.SetAsLastSibling();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!playerControlled) { return; }
        //Debug.Log("EndDrag: " + data.name);
        if (OverZone(eventData, Dungeon.GetZone(CardZone.Type.TRIBUTE)) && Player.instance.burnAvailable)
        {
            Player.instance.Burn(this);
        } else if (OverZone(eventData, Dungeon.GetZone(CardZone.Type.DECKBUILDER)) && (zone.type == CardZone.Type.DRAFT))
        {
            Dungeon.EnableDropZones(false);
            Drafter.instance.Draft(data, 0);
            Delete();
            return;
        }
        if (!needsTarget)
        {
            if (playable && OverZone(eventData, Dungeon.GetZone(CardZone.Type.PLAY)))
            {
                Resolve(Ability.Mode.PLAY, null);
            }
            else {
                transform.localScale = Vector3.one;
                zone.Organize();
            }
        } else
        {
            ITargetable hovered = Targeter.HoveredTarget(eventData);
            if (_validTargets.Contains(hovered))
            {
                Dungeon.targeter.AddTarget(hovered);
            } else
            {
                Dungeon.ClearTargeter();
            }
        }
        zone.Organize();
        particles.ClearGlow();
        _followingCursor = false;
        Dungeon.EnableDropZones(false);
        GameEvents.current.Refresh();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!playerControlled) { return; }
        //Debug.Log("Drag: " + data.name);
        if (needsTarget)
        {
            if (OverZone(eventData, Dungeon.GetZone(CardZone.Type.TRIBUTE)))
            {
                if (!_followingCursor)
                {
                    _followingCursor = true;
                    Dungeon.targeter?.Hide(true);
                }
                if (Player.instance.burnAvailable)
                {
                    particles.GlowRed();
                } else
                {
                    particles.ClearGlow();
                }
                transform.position = eventData.position;
            } else
            {
                particles.ClearGlow();
                if (_followingCursor)
                {
                    _followingCursor = false;
                    Dungeon.targeter?.Hide(false);
                    zone.Organize();
                }
            }
        } else
        {
            _followingCursor = true;
            transform.position = eventData.position;
            if (playable && OverZone(eventData, Dungeon.GetZone(CardZone.Type.PLAY)))
            {
                particles.GlowGold();
            }
            else if (OverZone(eventData, Dungeon.GetZone(CardZone.Type.TRIBUTE)) && Player.instance.burnAvailable)
            {
                particles.GlowRed();
            } else
            {
                particles.ClearGlow();
            }
        }
    }
    public void Move(CardZone cardZone)
    {
        CardZone prevZone = zone;
        graphic.Move(cardZone);
        if (prevZone == null)
        {
            if (cardZone.type == CardZone.Type.ACTIVE)
            {
                cardEvents.EnterPlay();
            }
            if (cardZone.type == CardZone.Type.DISCARD)
            {
                cardEvents.EnterDiscard();
            }
        } else {
            if (prevZone.type == CardZone.Type.ACTIVE && cardZone.type != CardZone.Type.ACTIVE)
            {
                cardEvents.LeavePlay();
            }
            if (prevZone.type != CardZone.Type.ACTIVE && cardZone.type == CardZone.Type.ACTIVE)
            {
                cardEvents.EnterPlay();
            }
            if (prevZone.type != CardZone.Type.DISCARD && cardZone.type == CardZone.Type.DISCARD)
            {
                cardEvents.EnterDiscard();
                if (prevZone.type == CardZone.Type.HAND)
                {
                    cardEvents.Discard();
                }
            }
        }
        Refresh();
    }
    private bool OverZone(PointerEventData eventData, CardZone cardZone)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);
        foreach (RaycastResult hit in hits)
        {
            if (hit.gameObject == cardZone.gameObject) { return true; }
        }
        return false;
    }
    public void AddTarget(ITargetable target)
    {
        _validTargets.Add(target);
    }
    public void FindTargets(Ability.Mode mode, int n, bool show = false)
    {
        _validTargets.Clear();
        TargetTemplate t = GetQuery(mode, n);
        if (t != null)
        {
            GameEvents.current.QueryTarget(GetQuery(mode, n), this, show);
        }
    }
    public void MarkTarget(TargetTemplate query, ITargetable source, bool show)
    {
        
        if (Compare(query, source.controller) && this != (UnityEngine.Object)source)
        {
            Attempt attempt = new Attempt();
            source.controller.actorEvents.TryMarkTarget(source, this, attempt);
            cardEvents.TryMarkTarget(source, attempt);
            if (attempt.success)
            {
                if (show) { particles.ShimmerBlue(); }
                source.AddTarget(this);
            }
        } else if (this == (UnityEngine.Object)source)
        {
            if (show) { } //particles.GlowGold(); }
        } else
        {
            particles.Clear();
        }
    }
    public bool IsTargeting(ITargetable target)
    {
        return _validTargets.Contains(target);
    }
    public bool Compare(TargetTemplate query, Actor self)
    {
        if (query is null) { return false; }
        bool flag = true;
        if (query.isNot != null && (query.isNot.Equals(this))) { return false; }
        if (query.isActor) { return false; }
        if (query.inHand) { flag &= (zone.type == CardZone.Type.HAND); }
        if (query.inPlay) { flag &= (zone.type == CardZone.Type.ACTIVE); }
        if (query.isDamageable) { flag &= (type == Type.THRALL); }
        if (query.isAttackable) { flag &= attackable; }
        if (query.isOpposing) { flag &= (controller != self); }
        if (query.isSelf) { flag &= (controller == self); }
        if (query.cardType.Count > 0)
        {
            bool success = false;
            foreach (Card.Type t in query.cardType)
            {
                if (t == this.type) { success = true; break; }
            }
            if (!success) { return false; }
        }
        if (query.cardColor.Count > 0)
        {
            bool success = false;
            foreach (Card.Color c in query.cardColor)
            {
                if (c == data.color) { success = true; break; }
            }
            if (!success) { return false; }
        }
        if (query.keywordAnd.Count > 0)
        {
            foreach (Keyword key in query.keywordAnd)
            {
                flag &= HasKeyword(key);
            }
        }
        if (query.keywordOr.Count > 0)
        {
            bool success = false;
            foreach (Keyword key in query.keywordOr)
            {
                if (HasKeyword(key)) { success = true; break; }
            }
            flag &= success;
        }
        if (query.notKeyword.Count > 0)
        {
            foreach (Keyword key in query.notKeyword)
            {
                if (HasKeyword(key)) { return false; }
            }
        }
        if (query.keywordAbility.Count > 0)
        {
            foreach (KeywordAbility.Key key in query.keywordAbility)
            {
                flag &= HasKeyword(key);
            }
        }
        if (query.notKeywordAbility.Count > 0)
        {
            foreach (KeywordAbility.Key key in query.notKeywordAbility)
            {
                if (HasKeyword(key)) { return false; }
            }
        }
        if (query.rarity.Count > 0)
        {
            bool success = false;
            foreach (Card.Rarity item in query.rarity)
            {
                if (item == data.rarity) { success = true; break; }
            }
            if (!success) { return false; }
        }
        if (query.zone.Count > 0)
        {
            bool success = false;
            foreach (CardZone.Type cardZone in query.zone)
            {
                if (zone.type == cardZone) { success = true; break; }
            }
            if (!success) { return false; }
        }
        foreach (TemplateParam _param in query.templateParams)
        {
            switch (_param.param)
            {
                case TargetTemplate.Param.LEVEL:
                    flag &= TargetTemplate.EvalOp(_param.op, cost.value, _param.value); break;
                case TargetTemplate.Param.POWER:
                    flag &= TargetTemplate.EvalOp(_param.op, power.value, _param.value); break;
                case TargetTemplate.Param.HEALTH:
                    flag &= TargetTemplate.EvalOp(_param.op, endurance.value, _param.value); break;
            }
        }
        return flag;
    }
    public TargetTemplate GetQuery(Ability.Mode mode, int n)
    {
        return _ability.GetQuery(mode, n);
    }
    public List<ICommand> FindMoves()
    {
        List<ICommand> moves = new List<ICommand>();
        if (playable)
        {
            if (needsTarget)
            {
                FindTargets(Ability.Mode.PLAY, 0);
                foreach (ITargetable target in _validTargets)
                {
                    List<ITargetable> targets = new List<ITargetable>();
                    targets.Add(target);
                    moves.Add(new AbilityCommand(_ability, Ability.Mode.PLAY, this, targets));
                }
            } else
            {
                moves.Add(new AbilityCommand(_ability, Ability.Mode.PLAY, this, null));
            }
        }
        if (canAttack) {
            FindTargets(Ability.Mode.ATTACK, 0);
            foreach (ITargetable target in _validTargets)
            {
                List<ITargetable> targets = new List<ITargetable>();
                targets.Add(target);
                moves.Add(new AbilityCommand(_ability, Ability.Mode.ATTACK, this, targets));
            }
        }
        if (activatable)
        {
            if (needsTarget)
            {
                FindTargets(Ability.Mode.ACTIVATE, 0);
                foreach (ITargetable target in _validTargets)
                {
                    List<ITargetable> targets = new List<ITargetable>();
                    targets.Add(target);
                    moves.Add(new AbilityCommand(_ability, Ability.Mode.ACTIVATE, this, targets));
                }
            }
            else
            {
                moves.Add(new AbilityCommand(_ability, Ability.Mode.ACTIVATE, this, null));
            }
        }
        return moves;
    }
    public virtual void AddStatus(StatusEffect.ID id, int stacks = 1)
    {
        if (gameObject == null) { return; }
        Attempt attempt = new Attempt();
        targetEvents.TryGainStatus(id, stacks, attempt);
        if (!attempt.success)
        {
            return;
        }
        if (GetStatus(id) > 0)
        {
            if (_statusEffects[id].stackable)
            {
                _statusEffects[id].stacks += stacks;
            } else
            {
                return;
            }
        }
        else
        {
            StatusEffect effect = StatusEffect.Spawn(id, this, stacks);
            effect.transform.parent = _statusDisplays;
            _statusEffects[id] = effect;
        }
        controller.actorEvents.CardGainedStatus(_statusEffects[id], stacks);
        targetEvents.GainStatus(_statusEffects[id], stacks);
    }
    public virtual void RemoveStatus(StatusEffect.ID id, int stacks = 9999)
    {
        if (_statusEffects.ContainsKey(id))
        {
            StatusEffect s = _statusEffects[id];
            targetEvents.RemoveStatus(s, stacks);
            if (stacks >= s.stacks)
            {
                _statusEffects[id].Remove();
                _statusEffects.Remove(id);
            }
            else
            {
                s.stacks -= stacks;
            }
        }
    }

    public List<StatusEffect.ID> GetAllStatus()
    {
        List<StatusEffect.ID> list = new List<StatusEffect.ID>();
        foreach (StatusEffect.ID key in _statusEffects.Keys)
        {
            list.Add(key);
        }
        return list;
    }

    public void RemoveAllStatus()
    {
        List<StatusEffect.ID> status = new List<StatusEffect.ID>();
        foreach (StatusEffect.ID s in _statusEffects.Keys)
        {
            status.Add(s);
        }
        foreach (StatusEffect.ID s in status)
        {
            RemoveStatus(s, 9999);
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

    public bool HasKeyword(Keyword key)
    {
        foreach (Keyword k in data.keywords)
        {
            if (key == k) { return true; }
        }
        return false;
    }

    public bool HasKeyword(KeywordAbility.Key key)
    {
        foreach (KeywordAbility.Key k in data.abilityKeywords)
        {
            if (key == k) { return true; }
        }
        return false;
    }

    public void Delete()
    {
        RemoveAllStatus();
        transform.parent = null;
        zone?.Organize();
        Destroy(this.gameObject);
    }

    public int Affinity(Card.Color color)
    {
        switch (color)
        {
            case Card.Color.RAIZ: return redAffinity;
            case Card.Color.FEN: return greenAffinity;
            case Card.Color.IRI: return blueAffinity;
            case Card.Color.LIS: return violetAffinity;
            case Card.Color.ORA: return goldAffinity;
            case Card.Color.VAEL: return indigoAffinity;
            default: return 0;
        }
    }
}
