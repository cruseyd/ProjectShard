using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targeter : MonoBehaviour
{
    [SerializeField] private ParticleLineRenderer _line;
    private List<ITargetable> _targets;
    private Ability.Mode _mode;
    public bool interactive;
    public ITargetable source;
    public TargetTemplate query
    {
        get
        {
            return source.GetQuery(_mode, _targets.Count);
        }
    }
    public static ITargetable HoveredTarget(PointerEventData eventData)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);
        foreach (RaycastResult hit in hits)
        {
            //Debug.Log("Checking " + hit.gameObject.name);
            ITargetable node = hit.gameObject.GetComponent<ITargetable>();
            if (node != null)
            {
                //Debug.Log("Targeter sees an ITargetable");
                return node;
            }
        }
        return null;
    }
    public void Update()
    {
        if (interactive)
        {
            _line.SetTarget(Input.mousePosition);
        }
    }
    public void Set(ITargetable src, Ability.Mode mode, bool show = true)
    {
        interactive = true;
        source = src;
        _mode = mode;
        _targets = new List<ITargetable>();
        _line.gameObject.SetActive(true);
        _line.transform.position = source.transform.position;
        source.FindTargets(_mode, 0, show);
    }
    public void Show(ITargetable src, ITargetable target)
    {
        interactive = false;
        source = null;
        _line.gameObject.SetActive(true);
        _line.transform.position = src.transform.position;
        _line.SetTarget(target.transform.position);
    }

    public void Show(Transform src, Transform target)
    {
        interactive = false;
        source = null;
        _line.gameObject.SetActive(true);
        _line.transform.position = src.position;
        _line.SetTarget(target.position);
    }

    public void AddTarget(ITargetable target)
    {
        Debug.Assert(source != null);
        if (target.Compare(query, source.controller))
        {
            _targets.Add(target);
            if (target is Card)
            {
                Card card = target as Card;
                card.particles.Burst();
                //card.particles.GlowGold();
            }
            Resolve();
        }
    }
    public void Resolve()
    {
        Debug.Assert(source != null);
        if (source.Resolve(_mode, _targets))
        {
            Destroy(this.gameObject);
        }
        else
        {
            source.FindTargets(_mode, _targets.Count, true);
        }
    }

    public void Hide(bool flag)
    {
        _line.gameObject.SetActive(!flag);
    }

}
