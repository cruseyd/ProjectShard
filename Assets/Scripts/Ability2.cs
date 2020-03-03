using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability2
{
    public enum Mode
    {
        ACTIVATE,
        PLAY,
        PASSIVE,
        ATTACK,
        INFUSE,
        CREATE
    }
    public class Instance
    {
        private ITargetable _user;
        private Ability2 _ability;

        public void Use(Mode mode, List<ITargetable> targets)
        {

            switch (mode)
            {
                case Mode.PLAY: _ability.Play((Card)_user, targets); break;
                case Mode.ACTIVATE: _ability.Activate((Card)_user, targets); break;
                case Mode.ATTACK: Ability2.Attack((Card)_user, (IDamageable)targets[0]); break;
                case Mode.PASSIVE: _ability.Passive((Card)_user, targets); break;
                case Mode.CREATE: _ability.Create((Card)_user, targets); break;
                default: break;
            }
        }
        public TargetTemplate GetQuery(Mode mode, int n)
        {
            if (n > NumTargets(mode)) { return null; }
            switch (mode)
            {
                case Mode.ACTIVATE:
                    if (_ability.activateTargets != null && _ability.activateTargets.Count > n)
                    {
                        return _ability.activateTargets[n];
                    }
                    return null;
                case Mode.PLAY:
                    if (_ability.playTargets != null && _ability.playTargets.Count > n)
                    {
                        return _ability.playTargets[n];
                    }
                    return null;
                case Mode.ATTACK:
                    TargetTemplate t = new TargetTemplate();
                    t.isOpposing = true;
                    t.isAttackable = true;
                    t.inPlay = true;
                    return t;
                default: return null;
            }

        }
        
        public void Try(Mode mode, List<ITargetable> targets, GameState state)
        {
            switch (mode)
            {
                case Mode.PLAY: _ability.Play((Card)_user, targets, false, state); break;
                case Mode.ACTIVATE: _ability.Activate((Card)_user, targets, false, state); break;
                case Mode.ATTACK: Attack((Card)_user, (IDamageable)targets[0], false, state); break;
                case Mode.PASSIVE: _ability.Passive((Card)_user, targets, false, state); break;
                default: break;
            }
        }
        public void Undo(Mode mode, List<ITargetable> targets, GameState state)
        {
            switch (mode)
            {
                case Mode.PLAY: _ability.Play((Card)_user, targets, true, state); break;
                case Mode.ACTIVATE: _ability.Activate((Card)_user, targets, true, state); break;
                case Mode.ATTACK: Attack((Card)_user, (IDamageable)targets[0], true, state); break;
                case Mode.PASSIVE: _ability.Passive((Card)_user, targets, true, state); break;
                default: break;
            }
        }
        public void Show(Mode mode, List<ITargetable> targets)
        {
            Targeter.Clear();
            switch (mode)
            {
                case Mode.PLAY:
                    Dungeon.MoveCard((Card)_user, CardZone.MAGNIFY);
                    ((Card)_user).FaceUp(true, true);
                    if (targets != null && targets.Count > 0)
                    {
                        Targeter.ShowTarget(Dungeon.GetZone(CardZone.MAGNIFY).transform.position, targets[0].transform.position);
                    }
                    break;
                case Mode.ATTACK:
                    Targeter.ShowTarget(_user, targets[0]);
                    break;
                case Mode.ACTIVATE:
                    if (_user is Card)
                    {
                        ((Card)_user).particles.Glow(true);
                    }
                    if (targets != null && targets.Count > 0)
                    {
                        Targeter.ShowTarget(_user, targets[0]);
                    }
                    break;
                default: return;
            }
        }
        public int NumTargets(Mode type)
        {
            switch (type)
            {
                case Mode.PLAY:
                    if (_ability.playTargets == null) { return 0; }
                    else { return _ability.playTargets.Count; }
                case Mode.ACTIVATE:
                    if (_ability.activateTargets == null) { return 0; }
                    else { return _ability.activateTargets.Count; }
                case Mode.ATTACK: return 1;
                case Mode.PASSIVE: return 0;
                default: return 0;
            }
        }
       

    }
    

    private static Dictionary<string, Component> _index;
    public static Dictionary<string, Component> index
    {
        get
        {
            if (_index == null)
            {
                _index = new Dictionary<string, AbilityComponent>();
            }
            return _index;
        }
    }

    public List<TargetTemplate> activateTargets;
    public List<TargetTemplate> playTargets;

    public string Text(Card source) { return "NO TEXT"; }
    protected virtual void Create(Card source, List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected virtual void Activate(Card source, List<ITargetable> targets, bool undo = false, GameState state = null) { }
    protected virtual void Play(Card source, List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        // at the bare minimum, all thralls are put into play, changing the boardstate
        if (state != null)
        {
            if (source.type == Card.Type.THRALL)
            {
                if (undo) { state.RemoveCard(source); }
                else { state.AddCard(source); }
            }
        }
    }
    protected virtual void Passive(Card source, List<ITargetable> targets, bool undo = false, GameState state = null) { }
    
    protected static void Attack(Card source, IDamageable target, bool undo = false, GameState state = null)
    {
        DamageData sourceDamage = new DamageData(source.power.value, source.damageType, source, target);
        DamageData targetDamage = null;
        if (target is Card)
        {
            Card targetCard = (Card)target;
            targetDamage = new DamageData(targetCard.power.value, targetCard.damageType, targetCard, source);
        }

        target.Damage(sourceDamage);
        source.Damage(targetDamage);
        target.ResolveDamage(sourceDamage);
        source.ResolveDamage(targetDamage);

        if (state != null)
        {
            if (undo) { source.attackAvailable = true; }
            else { source.attackAvailable = false; }
        }
        else { source.attackAvailable = false; }
    }
    public static void Infuse(Player player, Card target, bool undo = false, GameState state = null)
    {
        if (target == null) { return; }

        if (target.type == Card.Type.THRALL && player.focus.value > 0)
        {
            player.focus.baseValue -= 1;
            target.allegiance.baseValue += 1;
        }
    }

    
    public static void Damage(DamageData data, bool undo, GameState state)
    {
        if (state != null)
        {
            state.ApplyDamage(data, undo);
        }
        else
        {
            data.target.Damage(data);
            data.target.ResolveDamage(data);
        }
    }
    public void TargetAnyOpposing()
    {
        if (playTargets == null)
        {
            playTargets = new List<TargetTemplate>();
        }
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        playTargets.Add(t);
    }
    public void TargetOpposingThrall()
    {
        if (playTargets == null)
        {
            playTargets = new List<TargetTemplate>();
        }
        TargetTemplate t = new TargetTemplate();
        t.isOpposing = true;
        t.inPlay = true;
        t.cardType = Card.Type.THRALL;
        playTargets.Add(t);
    }
    public static TargetTemplate RandomOpposingTarget()
    {
        TargetTemplate t = new TargetTemplate();
        t.isDamageable = true;
        t.isOpposing = true;
        t.inPlay = true;
        return t;
    }
    public static List<ITargetable> ValidTargets(Card source, TargetTemplate template)
    {
        List<ITargetable> validTargets = new List<ITargetable>();
        validTargets.Add(source.opponent);
        foreach (Card card in source.opponent.active)
        {
            if (card.Compare(template, source.owner))
            {
                validTargets.Add(card);
            }
        }
        return validTargets;
    }
    public static ITargetable RandomValidTarget(Card source, TargetTemplate template)
    {
        List<ITargetable> valid = ValidTargets(source, template);
        return valid[Random.Range(0, valid.Count)];
    }

    public static ITargetable RandomOtherTarget(Card source, ITargetable ignore, TargetTemplate template)
    {
        List<ITargetable> valid = ValidTargets(source, template);
        if (valid.Count > 1)
        {
            ITargetable target = null;
            while (target == null)
            {
                ITargetable temp = valid[Random.Range(0, valid.Count)];
                if (temp != ignore) { target = temp; }
            }
            return target;
        }
        return null;
    }

}
