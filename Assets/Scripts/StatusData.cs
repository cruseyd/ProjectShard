using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusData", menuName = "StatusData")]
public class StatusData : ScriptableObject
{

    //public StatusName id;
    public StatusEffect.ID id;
    public bool stackable;
    public bool isDebuff;
    public bool isBuff;
    public string tooltipHeader;
    [TextArea(minLines: 10, maxLines: 20)]
    public string tooltipContent;
    public Color iconColor;
    public Color backgroundColor;

    public static StatusData Get(StatusEffect.ID _id)
    {
        return Resources.Load<StatusData>("StatusData/" + _id.ToString()) as StatusData; ;
    }
}
