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
        Player.instance.actorEvents.onTryPlayCard += OnPlayCard;
        Enemy.instance.actorEvents.onTryPlayCard += OnPlayCard;
        Player.instance.actorEvents.onCardDamaged += OnDamage;
        Enemy.instance.actorEvents.onCardDamaged += OnDamage;
        Player.instance.actorEvents.onPlayCard += OnCardPlayed;
        Enemy.instance.actorEvents.onPlayCard += OnCardPlayed;
        Player.instance.targetEvents.onGainStatus += OnGainStatus;
        Enemy.instance.targetEvents.onGainStatus += OnGainStatus;
        Player.instance.actorEvents.onCardGainedStatus += OnGainStatus;
        Enemy.instance.actorEvents.onCardGainedStatus += OnGainStatus;

        GameEvents.current.onCardDestroyed += OnCardDestroyed;

        GameEvents.current.onStartTurn += OnStartTurn;
    }

    private void OnCardPlayed(Card card)
    {
    }
    private void OnDamage(DamageData damage)
    {
        
        if (damage.damage > 0)
        {
            string txt = "";
            if (damage.target is Player)
            {
                txt += "You";
            } else {
                txt += damage.target.name;
            }
            
            txt += " took " + damage.damage + " " + Icons.Get(damage.type) + " damage";
            if (damage.source != null)
            {
                txt += " from " + damage.source.name;
            } else
            {
                txt += ".";
            }
            _window.Add(txt);
        }
    }
    private void OnPlayCard(Card card, Attempt attempt)
    {
        string txt = "\n";
        if (card.playerControlled)
        {
            txt += "You played " + card.name + ".";
        } else
        {
            txt += Enemy.instance.name + " played " + card.name + ".";
        }
        _window.Add(txt);
    }
    private void OnStartTurn(Actor actor)
    {
        string txt = "\n";
        if (actor is Player)
        {
            txt += "<b>Start Player Turn.</b>\n";
        } else
        {
            txt += "<b>Start " + actor.name + " Turn.</b>\n";
        }
        _window.Add(txt);
    }
    private void OnCardDestroyed(Card card)
    {
        _window.Add(card.name + " was destroyed.");
    }

    private void OnGainStatus(StatusEffect status, int stacks)
    {
        if (status.target is Player)
        {
            _window.Add("You gained " + stacks + " stacks of " + status.id);
        } else
        {
            _window.Add(status.target.name + " gained " + stacks + " stacks of " + status.id);
        }
    }
}
