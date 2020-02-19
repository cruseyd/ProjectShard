using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageManager : MonoBehaviour
{
    [SerializeField] private MessageWindow _window;

    private void Start()
    {
        Player.instance.events.onReceiveDamage += OnDamage;
        Enemy.instance.events.onReceiveDamage += OnDamage;
    }

    private void OnDamage(DamageData damage)
    {
        
        if (damage.damage > 0)
        {
            {
                if (damage.target is Actor)
                {
                    string txt = " took " + damage.damage + " " + Keywords.Parse(damage.type) + " damage.";
                    if (((Actor)damage.target).isPlayer)
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
}
