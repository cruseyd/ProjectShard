using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageManager : MonoBehaviour
{
    [SerializeField] private MessageWindow _window;

    private void Start()
    {
        Player.instance.targetEvents.onTakeDamage += OnDamage;
        Enemy.instance.targetEvents.onTakeDamage += OnDamage;
        GameEvents.current.onStartTurn += OnStartTurn;
    }

    private void OnDamage(DamageData damage)
    {
        
        if (damage.damage > 0)
        {
            {
                if (damage.target is Actor)
                {
                    string txt = " took " + damage.damage + " " + Keywords.Parse(damage.type) + " damage from " + damage.source.name;
                    if (damage.target.playerControlled)
                    {
                        _window.Add("You" + txt);
                    }
                    else
                    {
                        _window.Add(Enemy.instance.name + txt);
                    }
                } else if (damage.target is Card)
                {

                }
            }
        }
    }
    private void OnStartTurn(Actor actor)
    {
        string txt = "__________________________________\n";
        if (actor is Player)
        {
            txt += "Start Player Turn.";
        } else
        {
            txt += "Start " + actor.name + " Turn.";
        }
        _window.Add(txt);
    }
}
