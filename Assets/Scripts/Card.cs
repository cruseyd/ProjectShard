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
        GOLD,
        RED,
        GREEN,
        BLUE,
        VIOLET,
        INDIGO,
        TAN
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
        LEGENDARY,
        TOKEN
    }

    private static List<CardData> _allCardData;

    private static List<CardData> _redCardData;
    private static List<CardData> _blueCardData;
    private static List<CardData> _greenCardData;

    private static List<CardData> _allSpells;
    private static List<CardData> _allTechniques;
    private static List<CardData> _allThralls;

    private static List<CardData> _commonData;
    private static List<CardData> _scarceData;
    private static List<CardData> _rareData;
    private static List<CardData> _legendaryData;

    private static GameObject _cardPrefab;
    private static GameObject _enemyCardPrefab;

    [SerializeField] private GameObject _cardFront;
    [SerializeField] private GameObject _cardBack;
    [SerializeField] private GameObject _affinity;
    [SerializeField] private Transform _statusDisplays;

    [SerializeField] private Image _border;
    [SerializeField] private Image _rarity;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _keywordText;
    [SerializeField] private TextMeshProUGUI _abilityText;
    
    [SerializeField] private ValueDisplay _costDisplay;
    [SerializeField] private ValueDisplay _perceptionDisplay;
    [SerializeField] private ValueDisplay _finesseDisplay;
    [SerializeField] private ValueDisplay _strengthDisplay;
    [SerializeField] private ValueDisplay _powerDisplay;
    [SerializeField] private ValueDisplay _enduranceDisplay;
    [SerializeField] private ValueDisplay _upkeepDisplay;

    public CardParticles particles;

    private CardData _data;
    private Ability _ability;
    private List<KeywordAbility> _kAbilities;
    private List<KeywordAbility.Key> _kAbilityKeys;
    private CardEvents _cardEvents;
    private TargetEvents _targetEvents;
    private Dictionary<StatusEffect.ID, StatusEffect> _statusEffects;
    private List<ITargetable> _validTargets;

    private bool _graphic = false;
    private bool _translating = false;
    private bool _followingCursor = false;
    private bool _faceUp;
    private bool _needsUpkeep;

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
    public Stat endurance;
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
                flag &= (ability != null && ability.react);
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

    public static void Load()
    {
        _allCardData = new List<CardData>();
        _redCardData = new List<CardData>();
        _blueCardData = new List<CardData>();
        _greenCardData = new List<CardData>();
        _allSpells = new List<CardData>();
        _allTechniques = new List<CardData>();
        _allThralls = new List<CardData>();
        _commonData = new List<CardData>();
        _scarceData = new List<CardData>();
        _rareData = new List<CardData>();
        _legendaryData = new List<CardData>();

        CardData[] cards = Resources.LoadAll<CardData>("Cards/Set_1");
        foreach (CardData data in cards)
        {
            _allCardData.Add(data);
            switch (data.color)
            {
                case Card.Color.RED: _redCardData.Add(data); break;
                case Card.Color.BLUE: _blueCardData.Add(data); break;
                case Card.Color.GREEN: _greenCardData.Add(data); break;
                default: break;
            }
            switch (data.type)
            {
                case Card.Type.SPELL: _allSpells.Add(data); break;
                case Card.Type.TECHNIQUE: _allTechniques.Add(data); break;
                case Card.Type.THRALL: _allThralls.Add(data); break;
            }
            switch (data.rarity)
            {
                case Card.Rarity.COMMON: _commonData.Add(data); break;
                case Card.Rarity.SCARCE: _scarceData.Add(data); break;
                case Card.Rarity.RARE: _rareData.Add(data); break;
                case Card.Rarity.LEGENDARY: _legendaryData.Add(data); break;
            }
        }
    }

    public static CardData Rand(List<CardData> _list = null)
    {
        while(true)
        {
            CardData data;
            if (_list == null)
            {
                data = _allCardData[Random.Range(0, _allCardData.Count)];
            } else
            {
                data = _list[Random.Range(0, _list.Count)];
            }
            
            if (data.type != Card.Type.AFFLICTION && !data.abilityKeywords.Contains(KeywordAbility.Key.EPHEMERAL))
            {
                return data;
            }
        }
    }
    public static CardData Rand(TargetTemplate template, List<CardData> _list = null)
    {
        List<CardData> matches = new List<CardData>();
        if (_list == null)
        {
            foreach (CardData data in _allCardData)
            {
                if (data.Compare(template))
                {
                    matches.Add(data);
                }
            }
        } else
        {
            foreach (CardData data in _list)
            {
                if (data.Compare(template))
                {
                    matches.Add(data);
                }
            }
        }
        while (true)
        {
            CardData data = matches[Random.Range(0, matches.Count)];
            if (data.type != Card.Type.AFFLICTION && !data.abilityKeywords.Contains(KeywordAbility.Key.EPHEMERAL))
            {
                return data;
            }
        }
    }

    public static CardData Rand(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.COMMON: return Rand(_commonData);
            case Rarity.SCARCE: return Rand(_scarceData);
            case Rarity.RARE: return Rand(_rareData);
            case Rarity.LEGENDARY: return Rand(_legendaryData);
            default: return null;
        }
    }
    public static Card Spawn(CardData data, bool isPlayerCard, Vector3 spawnPoint, bool graphic = false)
    {
        if (Card._cardPrefab == null)
        {
            _cardPrefab = Resources.Load("Prefabs/Card") as GameObject;
        }
        GameObject cardGO = Instantiate(_cardPrefab,spawnPoint, Quaternion.identity);
        Card card = cardGO.GetComponent<Card>();

        // DATA AND STATE
        card._data = data;
        card._graphic = graphic;
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
        
        // TEXT
        card._nameText.text = data.name;
        card._keywordText.text = "";
        foreach (Keyword word in data.keywords)
        {
            card._keywordText.text += (Keywords.Parse(word).ToUpper() + " ");
        }
        if (data.type != Card.Type.THRALL)
        {
            card._keywordText.text += Keywords.Parse(data.type).ToUpper();
        }

        // DISPLAYS AND VISUALS
        card._strengthDisplay.valueName = Icons.strength;
        card._finesseDisplay.valueName = Icons.finesse;
        card._perceptionDisplay.valueName = Icons.perception;
        card._powerDisplay.valueName = Icons.power;
        card._enduranceDisplay.valueName = Icons.endurance;
        card._upkeepDisplay.valueName = Icons.upkeep;

        card._costDisplay.baseValue = data.level;
        card._strengthDisplay.baseValue = data.strength;
        card._finesseDisplay.baseValue = data.finesse;
        card._perceptionDisplay.baseValue = data.perception;
        card._powerDisplay.baseValue = data.power;
        card._enduranceDisplay.baseValue = data.endurance;
        card._upkeepDisplay.baseValue = data.upkeep;

        card._border.color = GameData.GetColor(data.color);
        card._rarity.color = GameData.GetColor(data.rarity);
        Image[] pips = card._affinity.GetComponentsInChildren<Image>();
        int pipNum = 0;
        if (isPlayerCard)
        {
            for (int ii = 0; ii < data.violetAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = GameData.GetColor(Card.Color.VIOLET);
                pipNum++;
            }
            for (int ii = 0; ii < data.redAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = GameData.GetColor(Card.Color.RED);
                pipNum++;
            }
            for (int ii = 0; ii < data.goldAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = GameData.GetColor(Card.Color.GOLD);
                pipNum++;
            }
            for (int ii = 0; ii < data.greenAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = GameData.GetColor(Card.Color.GREEN);
                pipNum++;
            }
            for (int ii = 0; ii < data.blueAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = GameData.GetColor(Card.Color.BLUE);
                pipNum++;
            }
            for (int ii = 0; ii < data.indigoAffinity; ii++)
            {
                pips[pipNum].enabled = true;
                pips[pipNum].color = GameData.GetColor(Card.Color.INDIGO);
                pipNum++;
            }
        }
        while (pipNum < pips.Length)
        {
            pips[pipNum].enabled = false;
            pipNum++;
        }

        card._powerDisplay.transform.parent.gameObject.SetActive(false);
        card._strengthDisplay.transform.parent.gameObject.SetActive(false);
        if (card.data.type == Type.THRALL)
        {
            card._powerDisplay.transform.parent.gameObject.SetActive(true);

            card._strengthDisplay.transform.parent.gameObject.SetActive(false);

        } else if (card.data.type == Type.CONSTANT)
        {
            card._powerDisplay.transform.parent.gameObject.SetActive(true);
            card._powerDisplay.gameObject.SetActive(false);
            card._strengthDisplay.transform.parent.gameObject.SetActive(false);
        } else
        {
            card._powerDisplay.transform.parent.gameObject.SetActive(false);
            card._strengthDisplay.transform.parent.gameObject.SetActive(true);
        }
        
        card.particles.Clear();

        // EVENTS
        GameEvents.current.onAddGlobalModifier += card.AddGlobalModifier;
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
        card._abilityText.text = "";
        card._kAbilities = new List<KeywordAbility>();
        card._kAbilityKeys = new List<KeywordAbility.Key>();
        foreach (KeywordAbility.Key key in data.abilityKeywords)
        {
            card._kAbilityKeys.Add(key);
            card._kAbilities.Add(KeywordAbility.Get(key, card));
            card._abilityText.text += Keywords.Parse(key).ToLower() + "\n";
        }
        card._ability = AbilityIndex.Get(data.id, card);
        card._abilityText.text += card._ability.Text();

        card._abilityText.text += card.data.flavorText;

        card.FaceUp(graphic);
        card.RefreshText();
        GameEvents.current.SpawnCard(card);
        return card;
    }
    public void OnDestroy()
    {
        GameEvents.current.onAddGlobalModifier -= AddGlobalModifier;
        //GameEvents.current.onRemoveGlobalModifier -= RemoveGlobalModifier;
        GameEvents.current.onRefresh -= Refresh;
        GameEvents.current.onQueryTarget -= MarkTarget;
        controller.targetEvents.onTakeDamage -= DamageConstant;
        if (!_graphic)
        {
            controller.actorEvents.onStartTurn -= ResetFlags;
        }
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

        if (playable || canAttack || activatable)
        {
            particles.ShimmerGold();
        }
        else if (canDefend)
        {
            particles.ShimmerBlue();
        }
        else if (needsUpkeep)
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
        _costDisplay.value = cost.value;


        if (type == Type.THRALL)
        {
            _powerDisplay.value = power.value;
            _enduranceDisplay.value = endurance.value;
            _upkeepDisplay.value = upkeep.value;
        }
        else if (type == Type.CONSTANT)
        {
            _enduranceDisplay.value = endurance.value;
            _upkeepDisplay.value = upkeep.value;
        }
        else
        {
            _finesseDisplay.value = finesse.value;
            _perceptionDisplay.value = perception.value;
            _strengthDisplay.value = strength.value;
        }
        _abilityText.text = "";
        foreach (KeywordAbility.Key key in _kAbilityKeys)
        {
            _abilityText.text += key.ToString() + "\n";
        }
        _abilityText.text += Icons.Parse(_ability.Text()) + "\n";
        _abilityText.text += "<i>" + data.flavorText + "</i>";
    }
    public void AddGlobalModifier(TemplateModifier mod)
    {
        
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
            if ((type == Type.THRALL || type == Type.CONSTANT) && !interrupted) { controller.PutInPlay(this); }
            else { controller.Discard(this); }
        } else if (mode == Ability.Mode.ACTIVATE)
        {
            if (!interrupted) { controller.actorEvents.ActivateCard(this); }
            activationAvailable = false;
        }
        if (!interrupted) { _ability?.Use(mode, targets); }
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
            targetEvents.LoseHealth(data.damage);
        }
        GameEvents.current.CardDamaged(data);
        controller.actorEvents.CardDamaged(data);
        ResolveDamage(data);
    }
    public void DamageConstant(DamageData data)
    {
        Debug.Assert(type == Card.Type.CONSTANT);
        Damage(new DamageData(1, Keyword.DEFAULT, data.source, this));
    }
    public bool Upkeep()
    {
        if (!needsUpkeep) { return false; }
        if (((Player)controller).focus.value < upkeep.value && !GameData.instance.ignoreResources) { return false; }
        ((Player)controller).focus.baseValue -= upkeep.value;
        _needsUpkeep = false;
        if (type == Type.CONSTANT)
        {
            _ability.Use(Ability.Mode.ACTIVATE, null);
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
            RemoveAllStatus();
        }
    }
    public void IncrementHealth(int value)
    {
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
            if (needsTarget)
            {
                Dungeon.SetTargeter(this, Ability.Mode.ACTIVATE);
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
        //Refresh();
    }

    public void SwitchController()
    {
        _playerControlled = !_playerControlled;
    }
    public IEnumerator Flip(bool faceUp)
    {
        float duration = GameData.instance.cardAnimationRate;
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
        Refresh();
    }
    public IEnumerator Translate(Vector2 targetPos, bool blocking = true, float duration = 0)
    {
        if (duration == 0)
        {
            duration = GameData.instance.cardAnimationRate;
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
        Refresh();
    }
    public IEnumerator Zoom(bool flag, float factor = 1.5f, float duration = 0.1f)
    {
        if (zone == null) { yield return null; }
        else
        {
            RectTransform zoneTF = zone.GetComponent<RectTransform>();
            Vector3 targetScale = Vector3.one * zoneTF.rect.height / GetComponent<RectTransform>().rect.height;
            Vector3 start = transform.localScale;
            if (flag) { targetScale = targetScale * factor; }
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.localScale = Vector3.Lerp(start, targetScale, t);
                yield return null;
            }
            foreach (Transform status in _statusDisplays)
            {
                status.localScale = Vector3.one;
            }
        }
    }
    public Vector2 GetPosition()
    {
        RectTransform zonetf = zone.GetComponent<RectTransform>();
        switch (zone.type)
        {
            case CardZone.Type.DISCARD:
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
        if (_translating || Dungeon.targeting) { return; }

        Vector2 zoomDir = Vector2.zero;
        if (zone.type == CardZone.Type.ACTIVE
            || zone.type == CardZone.Type.HAND
            || zone.type == CardZone.Type.DRAFT)
        {
            zoomDir = Vector2.up; 
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
        if (_translating || Dungeon.targeting) { return; }
        StopAllCoroutines();
        StartCoroutine(Zoom(false));
        StartCoroutine(Translate(GetPosition(), false, 0.1f));
        transform.SetSiblingIndex(zoneIndex);
        Dungeon.FixLayering();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_translating || !playerControlled) { return; }
        Dungeon.EnableDropZones(true);
        if (needsTarget)
        {
            zone.Organize();
            if (playable) { Dungeon.SetTargeter(this, Ability.Mode.PLAY); }
            else if (canAttack)
            {
                Dungeon.SetTargeter(this, Ability.Mode.ATTACK);
            }
        }
        //StopAllCoroutines();
        transform.SetAsLastSibling();
        transform.parent.SetAsLastSibling();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!playerControlled) { return; }
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
                StartCoroutine(Zoom(false));
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
            /*
            bool valid = false;
            if (hovered != null && Dungeon.targeting)
            {
                valid = hovered.Compare(Dungeon.targeter.query, Dungeon.targeter.source.controller);
            }
            if (valid)
            {
                Dungeon.targeter.AddTarget(hovered);
            } else
            {
                Dungeon.ClearTargeter();
            }
            */
        }
        zone.Organize();
        particles.ClearGlow();
        _followingCursor = false;
        Dungeon.EnableDropZones(false);
        GameEvents.current.Refresh();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_translating || !playerControlled) { return; }

        if (needsTarget)
        {
            if (OverZone(eventData, Dungeon.GetZone(CardZone.Type.TRIBUTE)))
            {
                if (!_followingCursor)
                {
                    _followingCursor = true;
                    Dungeon.targeter?.Hide(true);
                }

                transform.position = eventData.position;
            } else
            {
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
        zone = cardZone;
        RectTransform newTF = cardZone.GetComponent<RectTransform>();
        transform.SetParent(cardZone.transform);
        transform.localScale = Vector3.one * newTF.rect.height / GetComponent<RectTransform>().rect.height;
        cardZone.Organize();
        prevZone?.Organize();
        if (prevZone == null) { return; }
        if (prevZone.type == CardZone.Type.ACTIVE && cardZone.type != CardZone.Type.ACTIVE)
        {
            cardEvents.LeavePlay();
        }
        if (prevZone.type != CardZone.Type.ACTIVE && cardZone.type == CardZone.Type.ACTIVE)
        {
            cardEvents.EnterPlay();
        }
        if (cardZone.type == CardZone.Type.DISCARD && prevZone.type != CardZone.Type.DISCARD)
        {
            cardEvents.EnterDiscard();
            if (prevZone.type == CardZone.Type.HAND)
            {
                cardEvents.Discard();
            }
        }
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
        
        if (Compare(query, source.controller) && this != (Object)source)
        {
            Attempt attempt = new Attempt();
            source.controller.actorEvents.TryMarkTarget(source, this, attempt);
            cardEvents.TryMarkTarget(source, attempt);
            if (attempt.success)
            {
                if (show) { particles.ShimmerBlue(); }
                source.AddTarget(this);
            }
        } else if (this == (Object)source)
        {
            if (show) { } //particles.GlowGold(); }
        } else
        {
            particles.Clear();
        }
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
        if (query.keyword.Count > 0)
        {
            foreach (Keyword key in query.keyword)
            {
                flag &= HasKeyword(key);
            }
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
        Debug.Log("(REAL) Trying to add status " + id + " to " + name);
        if (gameObject == null) { return; }
        Attempt attempt = new Attempt();
        targetEvents.TryGainStatus(id, stacks, attempt);
        if (!attempt.success)
        {
            Debug.Log("Status " + id + " was prevented");
            return;
        }
        if (GetStatus(id) > 0 && _statusEffects[id].stackable)
        {
            _statusEffects[id].stacks += stacks;
        }
        else
        {
            StatusEffect effect = StatusEffect.Spawn(id, this, stacks);
            effect.transform.parent = _statusDisplays;
            _statusEffects[id] = effect;
            //int n = _statusEffects.Count;
            //StatusDisplay display = _statusDisplays.transform.GetChild(n).GetComponent<StatusDisplay>();
            //StatusEffect s = new StatusEffect(id, this, display, stacks);
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
            case Card.Color.RED: return redAffinity;
            case Card.Color.GREEN: return greenAffinity;
            case Card.Color.BLUE: return blueAffinity;
            case Card.Color.VIOLET: return violetAffinity;
            case Card.Color.GOLD: return goldAffinity;
            case Card.Color.INDIGO: return indigoAffinity;
            default: return 0;
        }
    }
}
