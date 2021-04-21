using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardGraphic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static GameObject _cardPrefab;
    private static Sprite[] _topSprites;
    private static Sprite[] _midSprites;
    private static Sprite[] _botSprites;

    private static GameObject _affinityPipPrefab;

    [SerializeField] private GameObject _cardFront;
    [SerializeField] private GameObject _cardBack;
    [SerializeField] private GameObject _affinity;

    [SerializeField] private Image _topSprite;
    [SerializeField] private Image _midSprite;
    [SerializeField] private Image _botSprite;
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
    public CardZone zone { get { return _zone; } }
    public CardData data { get { return _data; } }
    public bool draggable
    {
        get
        {
            return _draggable;
        } set
        {
            _draggable = value;
        }
    }
    public int zoneIndex;

    private CardData _data;
    private bool _faceUp;
    private bool _translating = false;
    private bool _scaling = false;
    private bool _draggable = false;
    private CardZone _zone;

    public static CardGraphic Spawn(CardData data, Vector3 spawnPoint)
    {
        if (_cardPrefab == null)
        {
            _cardPrefab = Resources.Load("Prefabs/CardGraphic") as GameObject;
        }
        GameObject cardGO = Instantiate(_cardPrefab, spawnPoint, Quaternion.identity);
        CardGraphic card = cardGO.GetComponent<CardGraphic>();
        card.Initialize(data);
        return card;
    }
    public void Initialize(CardData data)
    {
        if (_affinityPipPrefab == null)
        {
            _affinityPipPrefab = Resources.Load("Prefabs/affinityPip") as GameObject;
            _topSprites = Resources.LoadAll<Sprite>("Art/card_top") as Sprite[];
            _midSprites = Resources.LoadAll<Sprite>("Art/card_mid") as Sprite[];
            _botSprites = Resources.LoadAll<Sprite>("Art/card_lower") as Sprite[];
        }
        _data = data;
        // Name
        _nameText.text = data.name;

        // Colors
        //_border.color = GameData.GetColor(data.color);
        int spriteIndex = 0;
        switch(data.color)
        {
            case Card.Color.RAIZ: spriteIndex = 0; break;
            case Card.Color.IRI: spriteIndex = 1; break;
            case Card.Color.FEN: spriteIndex = 2; break;
            case Card.Color.LIS: spriteIndex = 3; break;
            case Card.Color.ORA: spriteIndex = 4; break;
            case Card.Color.VAEL: spriteIndex = 5; break;
        }
        _topSprite.sprite = _topSprites[spriteIndex];
        _midSprite.sprite = _midSprites[spriteIndex];
        _botSprite.sprite = _botSprites[spriteIndex];

        _rarity.color = GameData.GetColor(data.rarity);

        // Keywords
        _keywordText.text = "";
        foreach (Keyword word in data.keywords)
        {
            _keywordText.text += (Keywords.Parse(word).ToUpper() + " ");
        }
        if (data.type != Card.Type.THRALL)
        {
            _keywordText.text += Keywords.Parse(data.type).ToUpper();
        }

        // ABILITY TEXT
        ParseText(data);

        // Value Displays
        if (data.type == Card.Type.THRALL)
        {
            _powerDisplay.valueName = Icons.power;
            _enduranceDisplay.valueName = Icons.health;
            _upkeepDisplay.valueName = Icons.upkeep;
        } else if (data.type == Card.Type.CONSTANT)
        {
            _powerDisplay.valueName = Icons.power;
            _enduranceDisplay.valueName = Icons.endurance;
            _upkeepDisplay.valueName = Icons.upkeep;
        } else
        {
            _strengthDisplay.valueName = Icons.strength;
            _finesseDisplay.valueName = Icons.finesse;
            _perceptionDisplay.valueName = Icons.perception;
        }
        SetStat(Stat.Name.COST, data.level);
        SetBaseStat(Stat.Name.COST, data.level);
        if (data.type == Card.Type.THRALL)
        {
            SetStat(Stat.Name.POWER, data.power);
            SetBaseStat(Stat.Name.POWER, data.power);
        }
        if (data.type == Card.Type.THRALL || data.type == Card.Type.CONSTANT)
        {
            if (data.type == Card.Type.CONSTANT)
            {
                _powerDisplay.gameObject.SetActive(false);
            }
            SetStat(Stat.Name.ENDURANCE, data.endurance);
            SetBaseStat(Stat.Name.ENDURANCE, data.endurance);
            SetStat(Stat.Name.UPKEEP, data.upkeep);
            SetBaseStat(Stat.Name.UPKEEP, data.upkeep);
        }
        if (data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE)
        {
            SetStat(Stat.Name.STRENGTH, data.strength);
            SetBaseStat(Stat.Name.STRENGTH, data.strength);
            SetStat(Stat.Name.PERCEPTION, data.perception);
            SetBaseStat(Stat.Name.PERCEPTION, data.perception);
            SetStat(Stat.Name.FINESSE, data.finesse);
            SetBaseStat(Stat.Name.FINESSE, data.finesse);
        }

        // Affinity
        for (int ii = 0; ii < data.redAffinity; ii++)
        {
            GameObject pipGO = Instantiate(_affinityPipPrefab, _affinity.transform.position, Quaternion.identity);
            pipGO.transform.parent = _affinity.transform;
            pipGO.GetComponent<Image>().color = GameData.GetColor(Card.Color.RAIZ);
        }
        for (int ii = 0; ii < data.greenAffinity; ii++)
        {
            GameObject pipGO = Instantiate(_affinityPipPrefab, _affinity.transform.position, Quaternion.identity);
            pipGO.transform.parent = _affinity.transform;
            pipGO.GetComponent<Image>().color = GameData.GetColor(Card.Color.FEN);
        }
        for (int ii = 0; ii < data.blueAffinity; ii++)
        {
            GameObject pipGO = Instantiate(_affinityPipPrefab, _affinity.transform.position, Quaternion.identity);
            pipGO.transform.parent = _affinity.transform;
            pipGO.GetComponent<Image>().color = GameData.GetColor(Card.Color.IRI);
        }
        for (int ii = 0; ii < data.violetAffinity; ii++)
        {
            GameObject pipGO = Instantiate(_affinityPipPrefab, _affinity.transform.position, Quaternion.identity);
            pipGO.transform.parent = _affinity.transform;
            pipGO.GetComponent<Image>().color = GameData.GetColor(Card.Color.LIS);
        }
        for (int ii = 0; ii < data.goldAffinity; ii++)
        {
            GameObject pipGO = Instantiate(_affinityPipPrefab, _affinity.transform.position, Quaternion.identity);
            pipGO.transform.parent = _affinity.transform;
            pipGO.GetComponent<Image>().color = GameData.GetColor(Card.Color.ORA);
        }
        for (int ii = 0; ii < data.indigoAffinity; ii++)
        {
            GameObject pipGO = Instantiate(_affinityPipPrefab, _affinity.transform.position, Quaternion.identity);
            pipGO.transform.parent = _affinity.transform;
            pipGO.GetComponent<Image>().color = GameData.GetColor(Card.Color.VAEL);
        }

        particles.Clear();
        FaceUp(true);
    }
    
    public void ParseText(CardData data, List<Stat> values = null, List<KeywordAbility.Key> keywords = null)
    {
        _abilityText.text = "";
        if (keywords == null)
        {
            keywords = data.abilityKeywords;
        }
        for (int ii = 0; ii < keywords.Count; ii++)
        {
            _abilityText.text += Keywords.Parse(keywords[ii]).ToLower();
            if (ii < keywords.Count - 1)
            {
                _abilityText.text += ", ";
            }
            else
            {
                _abilityText.text += "\n";
            }
        }
        string[] textArray = data.text.Split('#');
        _abilityText.text += textArray[0];
        for (int ii = 1; ii < textArray.Length; ii++)
        {
            if (values == null)
            {
                _abilityText.text += data.values[ii - 1] + textArray[ii];
            } else
            {
                _abilityText.text += values[ii - 1].value + textArray[ii];
            }
        }
        _abilityText.text = Icons.Parse(StatusEffect.Parse(Keywords.Parse(_abilityText.text)));
    }
    // VALUE UPDATES
    public void SetStat(Stat.Name stat, int value)
    {
        switch (stat)
        {
            case Stat.Name.COST: _costDisplay.value = value; break;
            case Stat.Name.POWER:
                Debug.Assert(data.type == Card.Type.THRALL);
                _powerDisplay.value = value; break;
            case Stat.Name.ENDURANCE:
                Debug.Assert(data.type == Card.Type.THRALL || data.type == Card.Type.CONSTANT);
                _enduranceDisplay.value = value; break;
            case Stat.Name.UPKEEP:
                Debug.Assert(data.type == Card.Type.THRALL || data.type == Card.Type.CONSTANT);
                _upkeepDisplay.value = value; break;
            case Stat.Name.STRENGTH:
                Debug.Assert(data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
                _strengthDisplay.value = value; break;
            case Stat.Name.PERCEPTION:
                Debug.Assert(data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
                _perceptionDisplay.value = value; break;
            case Stat.Name.FINESSE:
                Debug.Assert(data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
                _finesseDisplay.value = value; break;
            default:
                Debug.LogError("Stat " + stat + " invalid for card of type " + data.type); break;
        }
    }
    public void SetBaseStat(Stat.Name stat, int value)
    {
        switch (stat)
        {
            case Stat.Name.COST: _costDisplay.baseValue = value; break;
            case Stat.Name.POWER:
                Debug.Assert(data.type == Card.Type.THRALL);
                _powerDisplay.baseValue = value; break;
            case Stat.Name.ENDURANCE:
                Debug.Assert(data.type == Card.Type.THRALL || data.type == Card.Type.CONSTANT);
                _enduranceDisplay.baseValue = value; break;
            case Stat.Name.UPKEEP:
                Debug.Assert(data.type == Card.Type.THRALL || data.type == Card.Type.CONSTANT);
                _upkeepDisplay.baseValue = value; break;
            case Stat.Name.STRENGTH:
                Debug.Assert(data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
                _strengthDisplay.baseValue = value; break;
            case Stat.Name.PERCEPTION:
                Debug.Assert(data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
                _perceptionDisplay.baseValue = value; break;
            case Stat.Name.FINESSE:
                Debug.Assert(data.type == Card.Type.SPELL || data.type == Card.Type.TECHNIQUE);
                _finesseDisplay.baseValue = value; break;
            default:
                Debug.LogError("Stat " + stat + " invalid for card of type " + data.type); break;
        }
    }

    // MOTION
    public void FaceUp(bool flag, bool animate = false)
    {
        _faceUp = flag;
        particles.Clear();
        if (animate)
        {
            StartCoroutine(DoFlip(flag));
        }
        else
        {
            _cardFront.SetActive(flag);
            _cardBack.SetActive(!flag);
        }
    }
    public IEnumerator DoFlip(bool faceUp)
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
    }
    public void Translate(Vector2 targetPos, float duration = 0)
    {
        if (duration == 0) { duration = GameData.instance.cardAnimationRate; }
        StartCoroutine(DoTranslate(targetPos, false, duration));
        StartCoroutine(DoZoom(false, 1.0f, duration));
    }
    public IEnumerator DoTranslate(Vector2 targetPos, bool blocking = true, float duration = 0)
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
    }
    public void Zoom(bool flag, float factor = 1.5f)
    {
        if (_translating) { return; }

        Vector2 basePosition = Vector2.zero;
        Vector2 translation = Vector2.zero;
        CardZone zone = transform.parent.GetComponent<CardZone>();
        if (zone)
        {
            basePosition = new Vector2(zone.Position(zoneIndex) + transform.parent.position.x, transform.parent.position.y);
        }
        else
        {
            basePosition = new Vector2(transform.parent.position.x, transform.parent.position.y);
        }
        if (flag)
        {
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
            RectTransform rect = GetComponent<RectTransform>();
            float x = transform.position.x / Screen.width;
            float y = transform.position.y / Screen.height;
            float sgn_x = 1f; float sgn_y = 1f;
            if (x > 0.5f ^ !flag) { sgn_x = -1f; }
            if (y > 0.5f ^ !flag) { sgn_y = -1f; }
            float x_damping = Mathf.Abs(2.0f * (x - 0.5f));
            float y_damping = Mathf.Abs(2.0f * (y - 0.5f));
            translation = new Vector2(
                basePosition.x + (sgn_x * rect.rect.width / 2.0f * (factor - 1.0f) * x_damping * zone.scale),
                basePosition.y + (sgn_y * rect.rect.height / 2.0f * (factor - 1.0f) * y_damping * zone.scale));
        }
        else
        {
            transform.SetSiblingIndex(zoneIndex);
            translation = basePosition;
        }
        StartCoroutine(DoTranslate(translation, false, 0.1f));
        StartCoroutine(DoZoom(flag, factor, 0.1f));
    }
    public IEnumerator DoZoom(bool flag, float factor = 1.5f, float duration = 0.1f)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * zone.scale;
        Vector3 startTextPos = _midSprite.transform.position;
        Vector3 targetTextPos = Vector3.down * 100;
        if (flag)
        {
            targetScale = targetScale * factor;
            targetTextPos = Vector3.zero;
        }
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            _midSprite.transform.localPosition = Vector3.Lerp(startTextPos, targetTextPos, t);
            yield return null;
        }
        transform.localScale = targetScale;
        _midSprite.transform.localPosition = targetTextPos;
    }
    public void Move(CardZone cardZone)
    {
        CardZone prevZone = _zone;
        _zone = cardZone;
        transform.SetParent(cardZone.transform);
        cardZone.Organize();
        prevZone?.Organize();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (zone.type == CardZone.Type.DRAFT)
        {
            CardZone previewZone = Dungeon.GetZone(CardZone.Type.PREVIEW);
            CardGraphic clone = Spawn(data, previewZone.transform.position);
            clone.Move(previewZone);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        List<CardGraphic> cards = Dungeon.GetCardGraphics(CardZone.Type.PREVIEW);
        for (int ii = cards.Count - 1; ii >= 0; ii--)
        {
            Destroy(cards[ii].gameObject);
        }
    }
}