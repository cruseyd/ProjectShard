using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusName
{
    DEFAULT,
    POISON,
    BURN,
    STUN,
    ELDER_KNOWLEDGE,
    CHILL,
    DAZE,
    IMPALE
}

[CreateAssetMenu(fileName = "NewStatusData", menuName = "StatusData")]
public class StatusData : ScriptableObject
{
    public StatusName id;
    public bool stackable;
    public string tooltipHeader;
    [TextArea(minLines: 10, maxLines: 20)]
    public string tooltipContent;
    public Color iconColor;
    public Color backgroundColor;
}
