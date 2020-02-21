using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targeter : MonoBehaviour
{
    public static Targeter current;

    [SerializeField] private ParticleLineRenderer _targetBeam;
    private static ITargetable _source;
    private static List<ITargetable> _targets;
    private static Ability.Mode _abilityMode;
    public static bool active = false;
    public static int numSelected { get { return _targets.Count; } }
    public static ITargetable source { get { return _source; } }
    public static TargetTemplate currentQuery
    {
        get
        {
            return source.GetQuery(_abilityMode, _targets.Count);
        }
    }
    public void Awake()
    {
        if (current == null)
        {
            current = this;
            _targets = new List<ITargetable>();
            _source = null;
        }
        else { Destroy(this.gameObject); }
    }
    public static void SetSource(ITargetable src, Ability.Mode mode)
    {
        active = true;
        _source = src;
        _abilityMode = mode;
        /*
        current._targetBeam.transform.position = _source.transform.position;
        current._targetBeam.followCursor = true;
        current._targetBeam.gameObject.SetActive(true);
        */
        src.FindTargets(_abilityMode, 0, true);
    }
    public static int AddTarget(ITargetable node)
    {
        if (node.Compare(currentQuery, source.Controller()))
        {
            _targets.Add(node);
            Resolve();
        }
        return _targets.Count;
    }
    public static void Resolve()
    {
        if (!source.Resolve(_abilityMode, _targets))
        {
            _source.FindTargets(_abilityMode, _targets.Count, true);
        }
    }
    public static void Clear()
    {
        _source = null;
        active = false;
        current._targetBeam.gameObject.SetActive(false);
        _targets.Clear();
        GameEvents.current.Refresh();
    }
    public static ITargetable HoveredTarget(PointerEventData eventData)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);
        foreach (RaycastResult hit in hits)
        {
            ITargetable node = hit.gameObject.GetComponent<ITargetable>();
            if (node != null) { return node; }
        }
        return null;
    }

    public static void ShowTarget(ITargetable source, ITargetable target)
    {
        current._targetBeam.transform.position = source.transform.position;
        current._targetBeam.SetTarget(target.transform.position);
        current._targetBeam.gameObject.SetActive(true);
    }
    public static void ShowTarget(Vector3 source, Vector3 target)
    {
        current._targetBeam.transform.position = source;
        current._targetBeam.SetTarget(target);
        current._targetBeam.gameObject.SetActive(true);
    }

    public static void HideTargeter()
    {
        current._targetBeam.gameObject.SetActive(false);
    }
}