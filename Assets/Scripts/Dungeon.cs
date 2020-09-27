using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

/*
public enum CardZone
{
    DEFAULT,
    PLAYER_HAND,
    PLAYER_ACTIVE,
    PLAYER_DISCARD,
    DUNGEON_HAND,
    DUNGEON_ACTIVE,
    DUNGEON_DISCARD,
    MAGNIFY,
    DROP,
    BURN,
}
*/
public class Dungeon : MonoBehaviour
{
    public static Dungeon instance;
    private GameParams _params;
    [SerializeField] private GameObject _targeterPrefab;
    //[SerializeField] private Enemy _enemy;
    [SerializeField] private GamePhase _phase;

    [SerializeField] private Canvas _mainCanvas;

    [SerializeField] private GameObject _combatUI;
    [SerializeField] private GameObject _draftUI;
    
    // General UI Handles
    [SerializeField] private GameObject _confirmButton;
    [SerializeField] private TextMeshProUGUI _confirmButtonText;
    [SerializeField] private GameObject _tooltipWindow;
    [SerializeField] private GameObject _endgameWindow;
    [SerializeField] private TextMeshProUGUI _endgameHeader;
    [SerializeField] private GameObject _endgameContinueButton;

    // Draft UI Handles
    [SerializeField] private CardZone _draftZone;
    [SerializeField] private CardZone _deckbuilderZone;
    [SerializeField] private CardZone _previewZone;

    // Combat UI Handles
    [SerializeField] private CardZone _playerHandZone;
    [SerializeField] private CardZone _enemyHandZone;
    [SerializeField] private CardZone _playerActiveZone;
    [SerializeField] private CardZone _enemyActiveZone;
    [SerializeField] private CardZone _playerDiscardZone;
    [SerializeField] private CardZone _enemyDiscardZone;
    [SerializeField] private CardZone _magnifyZone;
    [SerializeField] private CardZone _tributeZone;
    [SerializeField] private CardZone _playZone;

    [SerializeField] private RectTransform _playerDeckZone;
    [SerializeField] private RectTransform _enemyDeckZone;

    [SerializeField] private ParticleSystem _playerParticleUnderlay;
    [SerializeField] private ParticleSystem _enemyParticleUnderlay;

    [SerializeField] private Stack<Targeter> _targeters;
    
    public static List<TemplateModifier> modifiers;
    public static List<JSONCardData> jsonCards;
    public static GamePhase phase
    {
        get
        {
            return instance._phase;
        }
        set
        {
            instance.StartCoroutine(instance.DoSwitchPhase(value));
        }
    }
    public static GameObject tooltipWindow
    {
        get
        {
            return instance._tooltipWindow;
        }
    }
    public static Card magnifiedCard
    {
        get
        {
            List<Card> cards = instance._magnifyZone.Cards();
            if (cards.Count == 1) { return cards[0]; }
            else if (cards.Count == 0) { return null; }
            else { Debug.Log("There should not be more than 1 card magnified"); }
            return null;
        }
    }
    public static bool priority
    {
        get
        {
            if (phase != null)
            {
                return phase.priority;
            }
            else
            {
                return false;
            }

        }
    }
    public static Targeter targeter
    {
        get
        {
            if (instance._targeters.Count <= 0)
            { return null; }
            else
            {
                return instance._targeters.Peek();
            }
        }
    }
    public static bool targeting
    {
        get
        {
            if (targeter != null && targeter.interactive)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }

    public void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
        modifiers = new List<TemplateModifier>();
        _targeters = new Stack<Targeter>();
        _endgameWindow.SetActive(false);
    }
    public void Start()
    {
        jsonCards = new List<JSONCardData>();
        string path = Application.dataPath + "/Resources/Cards/Set_1";
        string json = File.ReadAllText (path + "/Raiz/cards.json");
        json = "{ \"data\":" + json + "}";
        JSONCardArray cardArray = JsonUtility.FromJson<JSONCardArray>(json);
        foreach (JSONCardData data in cardArray.data)
        {
            Debug.Log(data.name + " | " + data.level + " | " + (Keyword)System.Enum.Parse(typeof(Keyword), data.key1) + " | " + data.text);
        }
        if (GameData.instance.startEncounter)
        {
            StartEncounter();
        } else if (GameData.instance.startDraft)
        {
            StartDraft();
        }
    }
    public void Update()
    {

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (targeting) { ClearTargeter(); }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Confirm();
        }
    }
    
    public static List<Card> GetCards(CardZone.Type zone, bool player = true)
    {
        return GetZone(zone, player).Cards();
    }
    public static CardZone GetZone(CardZone.Type zone, bool player = true)
    {
        switch (zone)
        {
            case CardZone.Type.HAND:
                if (player) { return instance._playerHandZone; }
                else { return instance._enemyHandZone; }
            case CardZone.Type.ACTIVE:
                if (player) { return instance._playerActiveZone; }
                else { return instance._enemyActiveZone; }
            case CardZone.Type.DISCARD:
                if (player) { return instance._playerDiscardZone; }
                else { return instance._enemyDiscardZone; }
            case CardZone.Type.MAGNIFY: return instance._magnifyZone;
            case CardZone.Type.TRIBUTE: return instance._tributeZone;
            case CardZone.Type.PLAY: return instance._playZone;
            case CardZone.Type.DRAFT: return instance._draftZone;
            case CardZone.Type.DECKBUILDER: return instance._deckbuilderZone;
            case CardZone.Type.PREVIEW: return instance._previewZone;
            default: return null;
        }
    }

    public static bool AddModifier(TemplateModifier mod)
    {
        if (modifiers.Contains(mod)) { return false; }
        else
        {
            modifiers.Add(mod);
            GameEvents.current.Refresh();
            return true;
        }
    }
    public static bool RemoveModifier(TemplateModifier mod)
    {
        if (modifiers.Contains(mod))
        {
            modifiers.Remove(mod);
            GameEvents.current.Refresh();
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SetParticleUnderlay(bool playerSide)
    {
        if (playerSide)
        {
            instance._playerParticleUnderlay.Play();
            instance._enemyParticleUnderlay.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            instance._enemyParticleUnderlay.Play();
            instance._playerParticleUnderlay.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public static void FixLayering()
    {
        instance._magnifyZone.transform.SetAsLastSibling();
        instance._tooltipWindow.transform.SetAsLastSibling();
    }

    public static void EnableDropZones(bool flag)
    {
        instance._deckbuilderZone.gameObject?.SetActive(flag);
        instance._playZone.gameObject?.SetActive(flag);
        instance._tributeZone.gameObject?.SetActive(flag);
        DecklistDisplay.transferZone?.SetActive(flag);
    }
    public void Confirm() { _phase.Confirm(); }
    public static void SetConfirmButtonText(string text)
    {
        instance._confirmButtonText.text = text;
    }
    public static void EnableConfirmButton(bool flag)
    {
        instance._confirmButton.GetComponent<Button>().enabled = flag;
    }

    public void StartEncounter()
    {

        _combatUI.SetActive(true);
        _draftUI.SetActive(false);
        StartCoroutine(DoStartEncounter());
    }
    private IEnumerator DoStartEncounter()
    {
        Player.instance.StartEncounter();
        Enemy.instance.StartEncounter();
        yield return new WaitForSeconds(1.0f);
        yield return Enemy.instance.DoDrawRandom();
        Dungeon.phase = GamePhase.player;
    }
    private IEnumerator DoSwitchPhase(GamePhase newPhase)
    {
        instance._phase?.Exit();
        yield return new WaitForSeconds(GameData.instance.cardAnimationRate);
        instance._phase = newPhase;
        yield return new WaitForSeconds(GameData.instance.cardAnimationRate);
        instance._phase.Enter();
        yield return new WaitForSeconds(GameData.instance.cardAnimationRate);
        GameEvents.current.Refresh();
    }

    public void Victory()
    {
        _endgameHeader.text = "Victory!";
        _endgameContinueButton.SetActive(true);
        _endgameWindow.SetActive(true);
    }

    public void Defeat()
    {
        _endgameHeader.text = "Defeat...";
        _endgameContinueButton.SetActive(false);
        _endgameWindow.SetActive(true);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void StartDraft()
    {
        _combatUI.SetActive(false);
        _draftUI.SetActive(true);
        Drafter.instance.StartDraft();
    }

    // Targeting
    public static void ClearTargeter()
    {
        if (instance._targeters.Count > 0)
        {
            Targeter t = instance._targeters.Pop();
            Destroy(t.gameObject);
        }
    }

    public static void SetTargeter(ITargetable src, Ability.Mode mode)
    {
        Targeter t = AddTargeter();
        t.Set(src, mode);
        Debug.Log(instance._targeters.Count + " targeters.");
    }
    public static void ShowTargeter(ITargetable src, ITargetable trg)
    {
        Targeter t = AddTargeter();
        t.Show(src, trg);
    }

    public static void ShowTargeter(Transform src, Transform trg)
    {
        Targeter t = AddTargeter();
        t.Show(src, trg);
    }

    private static Targeter AddTargeter()
    {
        Targeter t = Instantiate(instance._targeterPrefab).GetComponent<Targeter>();
        t.transform.parent = instance._mainCanvas.transform;
        instance._targeters.Push(t);
        return t;
    }


}
