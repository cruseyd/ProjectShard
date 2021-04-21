using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{

    private ActorState _self;
    private ActorState _opponent;
    public GameState(Actor self)
    {
        _self = new ActorState(self);
        _opponent = new ActorState(self.opponent);
    }
    private CardState GetCardState(Card card)
    {
        CardState state = _self.GetCardState(card);
        if (state == null) { state = _opponent.GetCardState(card); }
        return state;
    }
    public void AddTemplateModifier(TemplateModifier mod, bool undo)
    {
        foreach (CardState state in _self.cards)
        {
            if (mod.Compare(state.source))
            {
                AddStatModifier(state.source, mod, undo);
            }
        }
    }
    public void RemoveTemplateModifier(TemplateModifier mod, bool undo)
    {
        foreach (CardState state in _self.cards)
        {
            if (mod.Compare(state.source))
            {
                RemoveStatModifier(state.source, mod, undo);
            }
        }
    }
    public void AddStatModifier(Card card, StatModifier mod, bool undo)
    {
        int scale = 1;
        if (undo) { scale = -1; }
        switch (mod.statName)
        {
            case Stat.Name.POWER: GetCardState(card).power += scale * mod.value; break;
            case Stat.Name.HEALTH: GetCardState(card).health += scale * mod.value; break;
        }
    }
    public void RemoveStatModifier(Card card, StatModifier mod, bool undo)
    {
        int scale = 1;
        if (undo) { scale = -1; }
        switch (mod.statName)
        {
            case Stat.Name.POWER: GetCardState(card).power -= scale * mod.value; break;
            case Stat.Name.HEALTH: GetCardState(card).health -= scale * mod.value; break;
        }
    }
    public void Heal(Actor actor, int value, bool undo)
    {
        int scale = 1;
        if (undo) { scale = -1; }
        if (_self.actor == actor)
        {
            _self.health += scale * value;
        } else
        {
            _opponent.health += scale * value;
        }
    }
    public void Heal(Card card, int value, bool undo)
    {
        CardState state = GetCardState(card);
        if (state != null)
        {
            int scale = 1;
            if (undo) { scale = -1; }
            state.health += scale * value;
            state.health = Mathf.Clamp(state.health, 0, state.source.endurance.baseValue);
        }
    }
    public void Damage(DamageData damage, bool undo)
    {
        int scale = 1;
        if (undo) { scale = -1; }

        if (damage.target is Actor)
        {
            if (((Actor)damage.target) == _self.actor)
            {
                _self.health -= scale * damage.damage;
            } else
            {
                _opponent.health -= scale * damage.damage;
            }
        } else
        {
            Card trg = damage.target as Card;
            if (trg.controller == _self.actor)
            {
                _self.GetCardState(trg).Damage(damage.damage * scale);
            } else
            {
                _opponent.GetCardState(trg).Damage(damage.damage * scale);
            }

        }
    }
    public void Draw(Actor actor, int n, bool undo)
    {
        int scale = 1;
        if (undo) { scale = -1; }
        if (_self.actor == actor) { _self.Draw(scale*n); }
        else { _opponent.Draw(scale*n); }
    }
    public void AddCardToHand(Actor actor, bool undo)
    {
        int scale = 1;
        if (undo) { scale = -1; }
        if (actor == _self.actor) { _self.numCardsInHand += scale; _self.numCardsPlayable += scale; }
        else { _opponent.numCardsInHand += scale; _opponent.numCardsPlayable += scale; }
    }
    public void Status(ITargetable target, StatusEffect.ID id, int stacks, bool undo)
    {
        Debug.Log("(VIRTUAL) Adding status " + id + " to state of " + target.name + " | undo = " + undo);
        int scale = 1;
        if (undo) { scale = -1; }
        if (target is Actor)
        {
            Actor trg = target as Actor;
            if (trg == _self.actor)
            {
                _self.Status(id, stacks * scale);
            } else
            {
                _opponent.Status(id, stacks * scale);
            }
        } else if (target is Card)
        {
            CardState state = GetCardState(target as Card);
            state.Status(id, stacks * scale);
        }
    }
    public void PutCardInPlay(Card card, bool undo)
    {
        
        if (card.controller == _self.actor)
        {
            _self.AddCard(new CardState(card), CardZone.Type.ACTIVE, undo);
            if (card.zone.type == CardZone.Type.HAND) { _self.numCardsInHand--; }
        } else
        {
            _opponent.AddCard(new CardState(card), CardZone.Type.ACTIVE, undo);
            if (card.zone.type == CardZone.Type.HAND) { _opponent.numCardsInHand--; }
        }
    }
    public void RemoveCardFromPlay(Card card, bool undo)
    {
        if (card.controller == _self.actor)
        {
            _self.RemoveCard(card, CardZone.Type.ACTIVE, undo);
        }
        else
        {
            _opponent.RemoveCard(card, CardZone.Type.ACTIVE, undo);
        }
    }
    public float Evaluate()
    {
        float healthDelta = _self.health - _opponent.health;
        float handDelta = _self.numCardsPlayable - _opponent.numCardsPlayable;
        float threatDelta = _self.threat - _opponent.threat;

        float evaluation = 5 * healthDelta + 3 * threatDelta + handDelta;
        if (_self.health <= 0) { evaluation -= 9999; }         //avoid defeat
        if (_opponent.health <= 0) { evaluation += 9999; }     //always choose victory
        return evaluation;
    }
    public float EvaluateSelf()
    {
        float healthDelta = _self.health;
        float handDelta = _self.numCardsPlayable;
        float threatDelta = _self.threat;

        float evaluation = 5 * healthDelta + 3 * threatDelta + handDelta;
        if (_self.health <= 0) { evaluation -= 9999; }         //avoid defeat
        if (_opponent.health <= 0) { evaluation += 9999; }     //always choose victory
        return evaluation;
    }
    public float EvaluateOpponent()
    {
        float healthDelta = _opponent.health;
        float handDelta = _opponent.numCardsPlayable;
        float threatDelta = _opponent.threat;

        float evaluation = 5 * healthDelta + 3 * threatDelta + handDelta;
        if (_self.health <= 0) { evaluation += 9999; }         //avoid defeat
        if (_opponent.health <= 0) { evaluation -= 9999; }     //always choose victory
        return evaluation;
    }
}
