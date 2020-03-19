using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IEndDragHandler, IDragHandler, ITargetable
{
    public enum Type
    {
        DEFAULT,
        SPELL,
        CANTRIP,
        ABILITY,
        STRATEGY,
        ITEM,
        THRALL, 
        CONSTANT,
        IDEAL
    }
    public enum Color
    {
        DEFAULT,
        VIOLET,
        RED,
        GOLD,
        GREEN,
        BLUE,
        INDIGO,
        TAN
    }
    public enum Attribute
    {
        STRENGTH,
        FINESSE,
        PERCEPTION
    }
    
    
    private static GameObject _cardPrefab;
    private static GameObject _enemyCardPrefab;

    [SerializeField] private GameObject _cardFront;
    [SerializeField] private GameObject _cardBack;
    [SerializeField] private GameObject _affinity;
    [SerializeField] private GameObject _statusDisplays;

    [SerializeField] private Image _border;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _keywordText;
    [SerializeField] private TextMeshProUGUI _abilityText;
    
    [SerializeField] private ValueDisplay _costDisplay;
    [SerializeField] private ValueDisplay _perceptionDisplay;
    [SerializeField] private ValueDisplay _finesseDisplay;
    [SerializeField] private ValueDisplay _strengthDisplay;
    [SerializeField] private ValueDisplay _powerDisplay;
    [SerializeField] private ValueDisplay _healthDisplay;
    [SerializeField] private ValueDisplay _upkeepDisplay;

    public CardParticles particles;

    private CardData _data;
    private Ability _ability;
    private CardEvents _cardEvents;
    private TargetEvents _targetEvents;
    private Dictionary<StatusName, StatusEffect> _statusEffects;
    private List<ITargetable> _validTargets;

    private bool _translating = false;
    private bool _followingCursor = false;
    private bool _faceUp;

    public bool _needsUpkeep;
    public bool activationAvailable;
    public bool attackAvailable;
    public bool blockAvailable;
    [SerializeField] private bool _playerControlled;
    public bool playerControlled { get { return _playerControlled; } }
    public int zoneIndex;
    public CardZone zone;

    public CardData data { get { return _data; } }
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
    public new string name { get { return _data.name; } }
    public Card.Type type { get { return data.type; } }


    public Stat cost;
    public Stat strength;
    public Stat finesse;
    public Stat perception;
    public Stat power;
    public Stat health;
    public Stat upkeep;

    public int violetAffinity { get { return _data.violetAffinity; } }
    public int redAffinity    { get { return _data.redAffinity; } }
    public int goldAffinity   { get { return _data.goldAffinity; } }
    public int greenAffinity  { get { return _data.greenAffinity; } }
    public int blueAffinity   { get { return _data.blueAffinity; } }
    public int indigoAffinity { get { return _data.indigoAffinity; } }

    public Keyword damageType { get { return _data.damageType; } }

    public bool resourcesAvailable
    {
        get
        {
            bool flag = true;
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
            bool flag = true;
            
            if (playerControlled && (Dungeon.phase == GamePhase.player))
            {
                flag &= (zone == CardZone.PLAYER_HAND);
                flag &= resourcesAvailable;
                
            } else if (playerControlled && (Dungeon.phase == GamePhase.enemy))
            {
                // reaction ability logic
                flag = false;
            }
            else if (!playerControlled && (Dungeon.phase == GamePhase.enemy))
            {
                flag &= (zone == CardZone.DUNGEON_HAND);
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
            flag &= attackAvailable;
            flag &= (type == Type.THRALL);
            if (playerControlled)
            {
                flag &= (zone == CardZone.PLAYER_ACTIVE);
                flag &= (Dungeon.phase != GamePhase.enemy);
            }
            else
            {
                flag &= (zone == CardZone.DUNGEON_ACTIVE);
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
            flag &= (zone == CardZone.PLAYER_HAND);
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
    public bool counterable
    {
        get
        {
            return (data.type == Card.Type.SPELL || data.type == Card.Type.ABILITY);
        }
    }
    public bool needsTarget
    {
        get
        {
            if (playable && _ability.NumTargets(Ability.Mode.PLAY) > 0) { return true; }
            else if (activatable && _ability.NumTargets(Ability.Mode.ACTIVATE) > 0) { return true; }
            else if (canAttack) { return true; }
            else { return false; }
        }
    }
    public bool inPlay
    {
        get
        {
            return (zone == CardZone.DUNGEON_ACTIVE || zone == CardZone.PLAYER_ACTIVE);
        }
    }
    public bool needsUpkeep
    {
        get
        {
            bool flag = true;
            flag &= (playerControlled);
            flag &= (type == Card.Type.THRALL);
            flag &= (data.upkeep > 0);
            flag &= (_needsUpkeep);
            return flag;
        }
    }

    public static Card Spawn(CardData data, bool isPlayerCard, Vector3 spawnPoint)
    {
        if (Card._cardPrefab == null)
        {
            _cardPrefab = Resources.Load("Prefabs/Card") as GameObject;
        }
        GameObject cardGO = Instantiate(_cardPrefab,spawnPoint, Quaternion.identity);
        Card card = cardGO.GetComponent<Card>();

        // DATA AND STATE
        card._data = data;
        card._validTargets = new List<ITargetable>();
        card._statusEffects = new Dictionary<StatusName, StatusEffect>();
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
            card.health = new Stat(data.health);
            card.upkeep = new Stat(data.upkeep);
        }

        // TEXT
        card._nameText.text = data.name;
        card._keywordText.text = "";
        foreach (Keyword word in data.keywords)
        {
            card._keywordText.text += (Keywords.Parse(word) + " ");
        }
        if (data.type != Card.Type.THRALL)
        {
            card._keywordText.text += Keywords.Parse(data.type);
        }

        // DISPLAYS AND VISUALS
        card._strengthDisplay.valueName = Icons.strength;
        card._finesseDisplay.valueName = Icons.finesse;
        card._perceptionDisplay.valueName = Icons.perception;
        card._border.color = Dungeon.gameParams.GetColor(data.color);
        Image[] pips = card._affinity.GetComponentsInChildren<Image>();
        int pipNum = 0;
        if (isPlayerCard)
        {
            for (int ii = 0; ii < data.violetAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = Dungeon.gameParams.GetColor(Card.Color.VIOLET);
                pipNum++;
            }
            for (int ii = 0; ii < data.redAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = Dungeon.gameParams.GetColor(Card.Color.RED);
                pipNum++;
            }
            for (int ii = 0; ii < data.goldAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = Dungeon.gameParams.GetColor(Card.Color.GOLD);
                pipNum++;
            }
            for (int ii = 0; ii < data.greenAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = Dungeon.gameParams.GetColor(Card.Color.GREEN);
                pipNum++;
            }
            for (int ii = 0; ii < data.blueAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = Dungeon.gameParams.GetColor(Card.Color.BLUE);
                pipNum++;
            }
            for (int ii = 0; ii < data.indigoAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = Dungeon.gameParams.GetColor(Card.Color.INDIGO);
                pipNum++;
            }
        }
        while (pipNum < pips.Length)
        {
            pips[pipNum].enabled = false;
            pipNum++;
        }
        if (card.data.type == Type.THRALL)
        {
            card._healthDisplay.gameObject.SetActive(true);
            card._healthDisplay.GetComponent<Image>().color = Dungeon.gameParams.GetColor(data.color);
            card._powerDisplay.gameObject.SetActive(true);
            card._powerDisplay.GetComponent<Image>().color = Dungeon.gameParams.GetColor(data.color);
            card._upkeepDisplay.gameObject.SetActive(true);
            card._upkeepDisplay.GetComponent<Image>().color = Dungeon.gameParams.GetColor(data.color);

            card._strengthDisplay.gameObject.SetActive(false);
            card._finesseDisplay.gameObject.SetActive(false);
            card._perceptionDisplay.gameObject.SetActive(false);

        } else
        {
            card._healthDisplay.gameObject.SetActive(false);
            card._powerDisplay.gameObject.SetActive(false);
            card._upkeepDisplay.gameObject.SetActive(false);
        }
        StatusDisplay[] displays = card._statusDisplays.GetComponentsInChildren<StatusDisplay>();
        foreach (StatusDisplay tf in displays) { tf.gameObject.SetActive(false); }
        card.particles.Clear();

        // EVENTS
        GameEvents.current.onAddGlobalModifier += card.AddGlobalModifier;
        GameEvents.current.onRemoveGlobalModifier += card.RemoveGlobalModifier;
        card.controller.actorEvents.onStartTurn += card.ResetFlags;
        GameEvents.current.onQueryTarget += card.MarkTarget;
        GameEvents.current.onRefresh += card.Refresh;

        // ABILITY
        card._abilityText.text = "";
        foreach (KeywordAbility.Key key in data.abilityKeywords)
        {
            KeywordAbility.Parse(key, card);
            card._abilityText.text += key.ToString() + "\n";
        }
        card._ability = AbilityIndex.Get(data.id, card);
        card._abilityText.text += card._ability.Text();

        

        card.FaceUp(false);

        return card;
    }
    public void OnDestroy()
    {
        GameEvents.current.onAddGlobalModifier -= AddGlobalModifier;
        GameEvents.current.onRemoveGlobalModifier -= RemoveGlobalModifier;
        GameEvents.current.onRefresh -= Refresh;
        GameEvents.current.onQueryTarget -= MarkTarget;
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

        foreach (TemplateModifier mod in Dungeon.modifiers)
        {
            if (mod != null && Compare(mod.template, controller))
            {
                AddModifier(mod.modifier, mod.statName);
            }
        }

        _costDisplay.value = cost.value;
        

        if (type == Type.THRALL)
        {
            _powerDisplay.value = power.value;
            _healthDisplay.value = health.value;
            _upkeepDisplay.value = upkeep.value;
        } else
        {
            _finesseDisplay.value = finesse.value;
            _perceptionDisplay.value = perception.value;
            _strengthDisplay.value = strength.value;
        }

        if (playable || canAttack || activatable)
        {
            particles.MarkActive();
        } else if (canDefend)
        {
            particles.MarkValidTarget();
        } else if (needsUpkeep)
        {
            particles.MarkNeedsUpkeep();
        } else
        {
            particles.Clear();
        }
    }
    
    public void AddGlobalModifier(TemplateModifier mod)
    {
        if (Compare(mod.template, controller))
        {
            AddModifier(mod.modifier, mod.statName);
        }
    }

    public void RemoveGlobalModifier(TemplateModifier mod)
    {
        RemoveModifier(mod.modifier, mod.statName);
    }

    public bool AddModifier(StatModifier mod, Stat.Name stat)
    {
        switch (stat)
        {
            case Stat.Name.COST: return cost.AddModifier(mod);
            case Stat.Name.POWER: return power.AddModifier(mod);
            case Stat.Name.HEALTH: return health.AddModifier(mod);
            case Stat.Name.STRENGTH: return strength.AddModifier(mod);
            case Stat.Name.PERCEPTION: return perception.AddModifier(mod);
            case Stat.Name.FINESSE: return finesse.AddModifier(mod);
            case Stat.Name.UPKEEP: return upkeep.AddModifier(mod);
            default: return false;
        }
    }

    public bool RemoveModifier(StatModifier mod, Stat.Name stat)
    {
        switch (stat)
        {
            case Stat.Name.COST: return cost.RemoveModifier(mod);
            case Stat.Name.POWER: return power.RemoveModifier(mod);
            case Stat.Name.HEALTH: return health.RemoveModifier(mod);
            case Stat.Name.STRENGTH: return strength.RemoveModifier(mod);
            case Stat.Name.PERCEPTION: return perception.RemoveModifier(mod);
            case Stat.Name.FINESSE: return finesse.RemoveModifier(mod);
            case Stat.Name.UPKEEP: return upkeep.RemoveModifier(mod);
            default: return false;
        }
    }

    public bool Resolve(Ability.Mode mode, List<ITargetable> targets)
    {
        if (_ability.NumTargets(mode) > 0)
        {
            if (targets == null || targets.Count < _ability.NumTargets(mode)) { return false; }
        }
        if (playerControlled && mode == Ability.Mode.PLAY)
        {
            Player.instance.focus.baseValue -= cost.value;
        }
        if (targets != null)
        {
            foreach (ITargetable target in targets)
            {
                controller.targetEvents.DeclareTarget(this, target);
                targetEvents.DeclareTarget(this, target);
            }
        }
        _ability?.Use(mode, targets);
        if (mode == Ability.Mode.PLAY)
        {
            controller.actorEvents.PlayCard(this);
            if (type == Type.THRALL || type == Type.CONSTANT) { controller.PutInPlay(this); }
            else { controller.Discard(this); }
        } else if (mode == Ability.Mode.ACTIVATE)
        {
            controller.actorEvents.ActivateCard(this);
            activationAvailable = false;
        }
        Targeter.Clear();
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
    public void TriggerPassive()
    {
        _ability.Use(Ability.Mode.PASSIVE, null);
    }
    public void Damage(DamageData data)
    {
        if (data == null) { return; }
        Debug.Assert(type == Type.THRALL);

        data.source.targetEvents.DealRawDamage(data);
        if (!(data.source is Actor)) { data.source.controller.targetEvents.DealRawDamage(data); }
        data.source.targetEvents.DealModifiedDamage(data);
        if (!(data.source is Actor)) { data.source.controller.targetEvents.DealModifiedDamage(data); }
        data.source.targetEvents.DealDamage(data);
        if (!(data.source is Actor)) { data.source.controller.targetEvents.DealDamage(data); }

        targetEvents.TakeRawDamage(data);
        targetEvents.TakeModifiedDamage(data);
        health.baseValue -= data.damage;
        targetEvents.TakeDamage(data);

        if (data.damage > 0)
        {
            targetEvents.LoseHealth(data.damage);
        }
    }
    public bool Upkeep()
    {
        if (!needsUpkeep) { return false; }
        if (((Player)controller).focus.value < data.upkeep) { return false; }
        ((Player)controller).focus.baseValue -= data.upkeep;
        _needsUpkeep = false;
        GameEvents.current.Refresh();
        return true;
    }
    public virtual void ResolveDamage(DamageData data)
    {
        if (type == Card.Type.THRALL && health.value <= 0 && inPlay)
        {
            controller.Discard(this);
        }
    }
    public virtual void IncrementHealth(int value)
    {
        int prev = health.value;
        health.baseValue = Mathf.Clamp(health.baseValue + value, 0, data.health);
        if (health.value > prev)
        {
            targetEvents.GainHealth(value);
        }
        else if (health.value < prev)
        {
            targetEvents.LoseHealth(-value);
        }
    }
    // Motion
    public void DoubleClick()
    {
        if (canDefend) { Defend(); }
        else if (Targeter.active)
        {
            bool valid = Compare(Targeter.currentQuery, Targeter.source.controller);
            if (valid)
            {
                Targeter.AddTarget(this);
            }
            Targeter.Clear();
        }
        else if (needsUpkeep)
        {
            Upkeep();
        }
        else if (activatable)
        {
            if (needsTarget)
            {
                Targeter.SetSource(this, Ability.Mode.ACTIVATE);
            } else
            {
                ability?.Use(Ability.Mode.ACTIVATE, null);
                GameEvents.current.Refresh();
            }
        }
    }
    public void FaceUp(bool flag, bool animate = false)
    {
        _faceUp = flag;
        particles.Clear();
        if (animate)
        {
            StartCoroutine(Flip(flag));
        }
        else
        {
            _cardFront.SetActive(flag);
            _cardBack.SetActive(!flag);
        }
        Refresh();
    }

    public void SwitchController()
    {
        _playerControlled = !_playerControlled;
    }
    public IEnumerator Flip(bool faceUp)
    {
        float duration = Dungeon.gameParams.cardAnimationRate;
        float t = 0.0f;
        float halfDur = duration / 2.0f;
        Vector2 startScale = transform.localScale;
        Vector2 midScale = new Vector2(0, startScale.y);
        while (t < 1)
        {
            t += Time.deltaTime / halfDur;
            transform.localScale = Vector2.Lerp(startScale, midScale, t);
            yield return null;
        }
        FaceUp(faceUp, false);
        t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / halfDur;
            transform.localScale = Vector2.Lerp(midScale, startScale, t);
            yield return null;
        }
    }
    public IEnumerator Translate(Vector2 targetPos, bool blocking = true, float duration = 0)
    {
        if (duration == 0)
        {
            duration = Dungeon.gameParams.cardAnimationRate;
        }
        if (blocking) { _translating = true; }
        Vector2 startPos = transform.position;
        float t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        if (blocking) { _translating = false; }
    }
    public IEnumerator Zoom(bool flag, float factor = 1.5f, float duration = 0.1f)
    {
        Vector3 targetScale = Vector3.one * Dungeon.GetZone(zone).rect.height / GetComponent<RectTransform>().rect.height;
        Vector3 start = transform.localScale;
        if (flag) { targetScale = targetScale * factor; }
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(start, targetScale, t);
            yield return null;
        }
        
    }
    public Vector2 GetPosition()
    {
        RectTransform zonetf = Dungeon.GetZone(zone);
        switch (zone)
        {
            case CardZone.PLAYER_DISCARD:
                return zonetf.TransformPoint(0, 0, 0);
            default:
                float width = zonetf.rect.width;
                float spacing = width / (1.0f * transform.parent.childCount);
                float xpos = -width / 2.0f + spacing / 2.0f;

                xpos += zoneIndex * spacing;
                return zonetf.TransformPoint(xpos, 0, 0);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_translating) { return; }
        if (zone == CardZone.DUNGEON_DISCARD || zone == CardZone.PLAYER_DISCARD)
        {
            //Dungeon.GetZone(zone).SetAsLastSibling();
            //Dungeon.GetZone(zone).sizeDelta = new Vector2(500, 150);
            //Dungeon.Organize(zone);
        }
        Vector2 zoomDir = Vector2.zero;
        switch (zone)
        {
            case CardZone.PLAYER_ACTIVE:
            case CardZone.PLAYER_HAND:
                zoomDir = Vector2.up; break;
            case CardZone.DUNGEON_ACTIVE:
                zoomDir = Vector2.up; break;
            case CardZone.PLAYER_DISCARD:
            case CardZone.DUNGEON_DISCARD:
                return;
        }
        transform.SetAsLastSibling();
        transform.parent.SetAsLastSibling();

        Vector2 pos = transform.position;
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 delta = center - pos;
        delta = delta * zoomDir;
        StartCoroutine(Zoom(true, 2.0f));
        StartCoroutine(Translate(pos + delta * 0.3f, false, 0.1f));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_translating) { return; }
        if (zone == CardZone.DUNGEON_DISCARD || zone == CardZone.PLAYER_DISCARD)
        {
            //Dungeon.GetZone(zone).sizeDelta = new Vector2(100, 150);
            //Dungeon.Organize(zone);
        }
        StopAllCoroutines();
        StartCoroutine(Zoom(false));
        StartCoroutine(Translate(GetPosition(), false, 0.1f));
        transform.SetSiblingIndex(zoneIndex);
        Dungeon.FixLayering();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_translating || !playerControlled) { return; }
        if (needsTarget)
        {
            if (playable) { Targeter.SetSource(this, Ability.Mode.PLAY); }
            else if (canAttack)
            {
                Targeter.SetSource(this, Ability.Mode.ATTACK);
            }
        }
        StopAllCoroutines();
        transform.SetAsLastSibling();
        transform.parent.SetAsLastSibling();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!playerControlled) { return; }
        if (OverZone(eventData, CardZone.BURN) && Player.instance.burnAvailable)
        {
            controller.Discard(this);
            Player.instance.addAffinity(data.color, 1);
            Player.instance.burnAvailable = false;
            GameEvents.current.Refresh();
            return;
        }
        if (!needsTarget)
        {
            if (playable && OverZone(eventData, CardZone.DROP))
            {
                Resolve(Ability.Mode.PLAY, null);
            }
            else {
                StartCoroutine(Zoom(false));
                Dungeon.Organize(zone);
            }
        } else
        {
            ITargetable hovered = Targeter.HoveredTarget(eventData);
            bool valid = false;
            if (hovered != null)
            {
                valid = hovered.Compare(Targeter.currentQuery, Targeter.source.controller);
            }
            if (valid)
            {
                Targeter.AddTarget(hovered);
            }
            Targeter.Clear();
        }
        _followingCursor = false;
        GameEvents.current.Refresh();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_translating || !playerControlled) { return; }

        if (needsTarget)
        {
            if (!(OverZone(eventData, CardZone.PLAYER_HAND) || OverZone(eventData, CardZone.BURN)))
            {
                if (_followingCursor)
                {
                    _followingCursor = false;
                    Dungeon.Organize(zone);
                        
                }
                Targeter.ShowTarget(transform.position, eventData.position);
            } else
            {
                if (!_followingCursor)
                {
                    _followingCursor = true;
                    Targeter.HideTargeter();
                }
                    
                transform.position = eventData.position;
                    
            }
                
        }
        if (!needsTarget)
        {
            _followingCursor = true;
            transform.position = eventData.position;
            if (playable && OverZone(eventData, CardZone.DROP))
            {
                particles.Glow(true);
            }
            else
            {
                particles.Glow(false);
            }
        }
        if (OverZone(eventData, CardZone.BURN) && Player.instance.burnAvailable)
        {
            particles.RedGlow(true);
        }
        else
        {
            particles.RedGlow(false);
        }
    }

    private bool OverZone(PointerEventData eventData, CardZone zone)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);
        GameObject zoneObject = Dungeon.GetZone(zone).gameObject;
        foreach (RaycastResult hit in hits)
        {
            if (hit.gameObject == zoneObject) { return true; }
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
        if (Compare(query, source.controller) && this != (Object)source)
        {
            if (show) { particles.MarkValidTarget(); }
            source.AddTarget(this);
        } else if (this == (Object)source)
        {
            if (show) { particles.MarkSource(); }
        } else
        {
            particles.Clear();
        }
    }
    public bool Compare(TargetTemplate query, Actor self)
    {
        bool flag = true;
        if (query.isNot != null && (query.isNot.Equals(this))) { return false; }
        if (query.isActor) { return false; }
        if (query.inHand) { flag &= (zone == CardZone.DUNGEON_HAND || zone == CardZone.PLAYER_HAND); }
        if (query.inPlay) { flag &= (zone == CardZone.DUNGEON_ACTIVE || zone == CardZone.PLAYER_ACTIVE); }
        if (query.isDamageable) { flag &= (type == Type.THRALL); }
        if (query.isAttackable) { flag &= (type == Type.THRALL); }
        if (query.isOpposing) { flag &= (controller != self); }
        if (query.isSelf) { flag &= (controller == self); }
        if (query.cardType != Type.DEFAULT) { flag &= (type == query.cardType); }
        if (query.cardColor != Color.DEFAULT) { flag &= (data.color == query.cardColor); }
        if (query.keyword != Keyword.DEFAULT)
        {
            bool tmpFlag = false;
            foreach (Keyword key in data.keywords)
            {
                tmpFlag |= (key == query.keyword);
            }
            flag &= tmpFlag;
        }
        foreach (TemplateParam _param in query.templateParams)
        {
            switch (_param.param)
            {
                case TargetTemplate.Param.LEVEL:
                    flag &= TargetTemplate.EvalOp(_param.op, cost.value, _param.value); break;
                case TargetTemplate.Param.POWER:
                    flag &= TargetTemplate.EvalOp(_param.op, power.value, _param.value); break;
                case TargetTemplate.Param.ALLEGIANCE:
                    flag &= TargetTemplate.EvalOp(_param.op, health.value, _param.value); break;
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
        } else if(canAttack) {
            FindTargets(Ability.Mode.ATTACK, 0);
            foreach (ITargetable target in _validTargets)
            {
                List<ITargetable> targets = new List<ITargetable>();
                targets.Add(target);
                moves.Add(new AbilityCommand(_ability, Ability.Mode.ATTACK, this, targets));
            }
        }
        return moves;
    }
    public virtual void AddStatus(StatusName id, int stacks = 1)
    {
        if (_statusEffects.ContainsKey(id))
        {
            _statusEffects[id].stacks += stacks;
            targetEvents.GainStatus(_statusEffects[id], stacks);
        }
        else
        {
            int n = _statusEffects.Count;
            StatusDisplay display = _statusDisplays.transform.GetChild(n).GetComponent<StatusDisplay>();
            StatusEffect s = new StatusEffect(id, this, display, stacks);
            _statusEffects[id] = s;
            targetEvents.GainStatus(_statusEffects[id], stacks);
        }
    }
    public virtual void RemoveStatus(StatusName id, int stacks = 1)
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

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
