using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ============================================ RED CARDS ============================================
public class A_Cinder : CardAbility
{
    public A_Cinder(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 2;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.FIRE, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        return "Cinder inflicts 2 FIRE damage to any target.";
    }
}
public class A_Singe : CardAbility
{
    public A_Singe(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        if (state != null)
        {

        }
        else
        {
            int n = user.opponent.GetStatus(StatusEffect.ID.BURN);
            if (n > 0) { user.opponent.AddStatus(StatusEffect.ID.BURN, 1); }
            else { user.opponent.AddStatus(StatusEffect.ID.BURN, 2); }
        }
    }

    public override string Text()
    {
        return "If your opponent is not Burned, they gain 2 Burn. Otherwise, they gain 1 Burn.";
    }
}
public class A_Blitz : CardAbility
{
    public A_Blitz(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        if (state != null)
        {
            if (undo)
            {
                state.DrawCards(user.controller, -1);
            }
            else
            {
                state.DrawCards(user.controller, 1);
            }
        }
        else
        {
            int n = 0;
            user.controller.Draw();
            if (user.playerControlled)
            {
                foreach (Card card in user.controller.hand)
                {
                    if (card.type == Card.Type.TECHNIQUE) { n += 1; }
                }
                ((Player)user.controller).focus.baseValue += n;
            }
        }

    }
    /*
    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (((Card)_user).playerControlled)
        {
            Player.instance.focus.baseValue += 1;
        }
    }
    */
    public override string Text()
    {
        string txt = "Draw a card.";
        if (((Card)_user).playerControlled)
        {
            txt += " Gain 1/0 FOCUS for each Technique in your hand.";
            txt += "\n<b>Passive:</b> Gain 1/0 FOCUS.";
        }
        return txt;
    }
    private void CycleHandler(Card card)
    {
        if (user.playerControlled)
        {
            Player.instance.focus.baseValue += 1;
        }
    }
}
public class A_Slash : CardAbility
{
    public A_Slash(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 2;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        string txt = "Target enemy takes 2 SLASHING damage.";
        return txt;
    }
}

public class A_ViciousRend : CardAbility
{
    public A_ViciousRend(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 3;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        string txt = "Target enemy takes 3 SLASHING damage.";
        return txt;
    }
}

public class A_MightyCleave : CardAbility
{
    public A_MightyCleave(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 4;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }

    public override string Text()
    {
        string txt = "Target enemy takes 4 SLASHING damage.";
        return txt;
    }
}

public class A_Flurry : CardAbility
{
    private TargetTemplate _template;
    public A_Flurry(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.TECHNIQUE);

        Card usr = user as Card;
        usr.cardEvents.onDraw += DrawHandler;
        
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = _user as Card;
        base.Play(targets, undo, state);
        int damage = 1;

        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }
    public override string Text()
    {
        return "Target takes 1 SLASHING damage. If you play another Technique this turn, return this card to your hand.";
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += PlayedCardHandler;
    }
    private void PlayedCardHandler(Card card)
    {
        Card user = _user as Card;
        if (user.playedThisTurn && card.Compare(_template, _user.controller))
        {
            user.controller.AddToHand(user);
        }
    }
}

public class A_WildSwing : CardAbility
{
    public A_WildSwing(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 2;
        ITargetable t1 = (ITargetable)targets[0];
        ITargetable t2 = RandomOpposing(user, TargetAnyOpposing(t1));

        DamageData d1 = new DamageData(damage, Keyword.SLASHING, _user, t1);
        Ability.Damage(d1, undo, state);
        if (t2 != null)
        {
            DamageData d2 = new DamageData(damage, Keyword.SLASHING, _user, t2);
            Ability.Damage(d2, undo, state);
        }
    }

    public override string Text()
    {
        return "Wild Swing inflicts 2 SLASHING damage to any target and to another random opposing target.";
    }
}

public class A_SeeingRed : CardAbility
{
    public A_SeeingRed(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        _user.controller.AddStatus(StatusEffect.ID.FRENZY, 1);
        _user.controller.Draw();
    }

    public override string Text()
    {
        return "Gain 1 Frenzy. Draw a card.";
    }
}

public class A_Ireblade : CardAbility
{
    private StatModifier _mod;
    public A_Ireblade(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
        user.targetEvents.onRefresh += RefreshHandler;
        user.cardEvents.onCycle += CycleHandler;
        _mod = new StatModifier(0, Stat.Name.COST, user);
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        int damage = 3;
        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.SLASHING, _user, target);
        Ability.Damage(data, undo, state);
    }
    /*
    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        _user.controller.AddStatus(StatusEffect.ID.FRENZY);
    }
    */
    private void CycleHandler(Card card)
    {
        user.controller.AddStatus(StatusEffect.ID.FRENZY);
    }
    public override string Text()
    {
        string txt = "Target enemy takes 3 SLASHING damage. Ireblade has -1 Cost for each stack of Frenzy you have.";
        txt += "\n<b>Passive:</b> Gain 1 Frenzy.";
        return txt;
    }

    private void RefreshHandler()
    {
        _mod.value = user.controller.GetStatus(StatusEffect.ID.FRENZY);
    }
}

public class A_InfernoDjinn : CardAbility
{
    private TargetTemplate _template;
    public A_InfernoDjinn(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.keyword.Add(Keyword.FIRE);
        _template.cardType.Add(Card.Type.SPELL);
        
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }
    public override string Text()
    {
        return "When you play a FIRE Spell, this gets +1 POWER.";
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += OnPlayCardHandler;
    }
    private void OnPlayCardHandler(Card card)
    {
        if (_user.inPlay && card.Compare(_template, _user.controller))
        {
            Card user = _user as Card;
            user.AddModifier(new StatModifier(1, Stat.Name.POWER, user, StatModifier.Duration.PERMANENT));
        }
    }
}

public class A_IntoTheFray : CardAbility
{
    public A_IntoTheFray(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        _user.controller.AddStatus(StatusEffect.ID.FRENZY, 3);
    }

    public override string Text()
    {
        return "Gain 3 Frenzy.";
    }
}


public class A_Sharpen : CardAbility
{
    public A_Sharpen(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        _user.controller.targetEvents.onDealRawDamage += RawDamageHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
        _user.controller.Draw();
    }
    public override string Text()
    {
        return "Increase all SLASHING damage you deal this turn by 1. Draw a card.";
    }

    void RawDamageHandler(DamageData data)
    {
        if (data.type == Keyword.SLASHING) { data.damage += 1; }
    }

    void EndTurnHandler(Actor actor)
    {
        actor.targetEvents.onDealRawDamage -= RawDamageHandler;
        actor.actorEvents.onEndTurn -= EndTurnHandler;
    }
}

public class A_Spinetail : CardAbility
{
    public A_Spinetail(Card user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
    }
    public override string Text()
    {
        return "<b>Activate: </b> Impale target opposing Thrall.";
    }

    public override bool ActivationAvailable()
    {
        return true;
    }

    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        targets[0].AddStatus(StatusEffect.ID.IMPALE, 1);
    }
}

public class A_Emberfall : CardAbility
{
    public A_Emberfall(Card user) : base(user)
    {

    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        StartChannel();
        CardData data = Resources.Load("Cards/Raiz/Emberstrike") as CardData;
        for (int ii = 0; ii < 2; ii++)
        {
            _user.controller.AddToHand(Card.Spawn(data, _user.playerControlled, _user.transform.position));
        }
    }

    protected override void Channel()
    {
        base.Channel();
        CardData data = Resources.Load("Cards/Raiz/Emberstrike") as CardData;
        for (int ii = 0; ii < 2; ii++)
        {
            _user.controller.AddToHand(Card.Spawn(data, _user.playerControlled, _user.transform.position));
        }
    }

    public override string Text()
    {
        string txt = "Add 2 <b>Emberstrike</b> with to your hand.";
        txt += "\n<b>Channel: </b> Add 2 <b>Emberstrike</b> to your hand.";
        return txt;
    }
}

public class A_Emberstrike : CardAbility
{
    public A_Emberstrike(Card user) : base(user)
    {
        playTargets.Add(TargetAnyOpposing());
    }
    public override string Text()
    {
        return "Target enemy takes 1 FIRE damage and gains 1 Burn.";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = _user as Card;
        base.Play(targets, undo, state);
        int damage = 1;

        ITargetable target = (ITargetable)targets[0];
        DamageData data = new DamageData(damage, Keyword.FIRE, _user, target);
        Ability.Damage(data, undo, state);
        target.AddStatus(StatusEffect.ID.BURN);
    }
}

public class A_BlazingVortex : CardAbility
{
    public A_BlazingVortex(Card user) : base(user)
    {
    }
    public override string Text()
    {
        return "All players and Thralls take 2 FIRE damage.";
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = _user as Card;
        base.Play(targets, undo, state);
        int damage = 2;

        List<ITargetable> trg = new List<ITargetable>();
        trg.Add(Enemy.instance);
        foreach (Card card in Enemy.instance.active)
        {
            if (card.type == Card.Type.THRALL)
            {
                trg.Add(card);
            }
        }
        foreach (Card card in Player.instance.active)
        {
            if (card.type == Card.Type.THRALL)
            {
                trg.Add(card);
            }
        }
        trg.Add(Player.instance);

        foreach (ITargetable target in trg)
        {
            DamageData data = new DamageData(damage, Keyword.FIRE, _user, target);
            Ability.Damage(data, undo, state);
        }
    }
}

public class A_HyrocChampion : CardAbility
{
    int _upkeepMod;
    public A_HyrocChampion(Card user) : base(user)
    {
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        base.Play(targets, undo, state);
    }
    public override string Text()
    {
        string txt = "When you play a Technique, Hyroc Champion gets -1 UPKEEP.";
        return txt;
    }

    private void DrawHandler(Card card)
    {
        _user.controller.actorEvents.onPlayCard += PlayCardHandler;
        _user.controller.actorEvents.onEndTurn += EndTurnHandler;
        _upkeepMod = 0;
    }
    private void PlayCardHandler(Card card)
    {
        Card user = _user as Card;
        if (card.type == Card.Type.TECHNIQUE)
        {
            user.upkeep.RemoveModifiersFromSource(_user as Object);
            _upkeepMod--;
            StatModifier mod = new StatModifier(_upkeepMod, Stat.Name.UPKEEP, user);
            user.AddModifier(mod);
        }
    }

    private void EndTurnHandler(Actor actor)
    {
        Card user = _user as Card;
        _upkeepMod = 0;
        user.upkeep.RemoveModifiersFromSource(_user as Object);
    }
}

public class A_Ignite : CardAbility
{
    public A_Ignite(Card user) : base(user) { }
    public override string Text()
    {
        string txt = "Gain 1 Firebrand. Draw a card.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        user.controller.AddStatus(StatusEffect.ID.FIREBRAND);
        user.controller.Draw();
    }
}

public class A_FlamingFlourish : CardAbility
{
    public A_FlamingFlourish(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    public override string Text()
    {
        string txt = "Draw a card. Gain 1 Firebrand for each Ability in your hand.";
        txt += "<b>Passive: </b> Gain 1 Firebrand.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        Debug.Assert(_user is Card);
        Card user = (Card)_user;
        base.Play(targets, undo, state);
        user.controller.Draw();
        List<Card> cards = user.controller.hand;
        int n = 0;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.TECHNIQUE) { n++; }
        }
        user.controller.AddStatus(StatusEffect.ID.FIREBRAND, n);
    }
    
    private void CycleHandler(Card card)
    {
        _user.controller.AddStatus(StatusEffect.ID.FIREBRAND);
    }
    /*
    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        _user.controller.AddStatus(StatusEffect.ID.FIREBRAND);
    }
    */
}


public class A_FeralWarcat : CardAbility
{
    public A_FeralWarcat(Card user) : base(user)
    {
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }
    public override string Text()
    {
        return "When you target an enemy with an Technique, Feral Warcat deals 1 SLASHING damage to that target.";
    }

    private void DrawHandler(Card card)
    {
        _user.controller.targetEvents.onDeclareTarget += DeclareTargetHandler;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    private void DeclareTargetHandler(ITargetable source, ITargetable target)
    {
        if (source is Card && ((Card)_user).inPlay)
        {
            Card src = (Card)source;
            if (src.type == Card.Type.TECHNIQUE)
            {
                Ability.Damage(new DamageData(1, Keyword.SLASHING, _user, target), false, null);
            }
        }
    }

}
