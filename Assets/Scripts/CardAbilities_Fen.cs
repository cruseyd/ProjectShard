using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_TerritorialBriar : CardAbility
{
    public A_TerritorialBriar(Card user) : base(user)
    {
        ((Card)user).cardEvents.onDraw += DrawHandler;
    }
    public override string Text()
    {
        string txt = "";
        txt += "When an opposing thrall attacks, it first takes 1 PIERCING damage.";
        txt += "When your opponent uses a Technique, they first take 1 PIERCING damage.";
        return txt;
    }

    private void DrawHandler(Card card)
    {
        _user.opponent.actorEvents.onPlayCard += OnPlayCardHandler;
        _user.opponent.targetEvents.onDeclareAttack += OnDeclareAttackHandler;
    }
    private void OnDeclareAttackHandler(Card source, ITargetable target)
    {
        if (_user.inPlay)
        {
            Ability.Damage(new DamageData(1, Keyword.PIERCING, _user, source), false, null);
        }
    }

    private void OnPlayCardHandler(Card source)
    {
        if (_user.inPlay && source.type == Card.Type.TECHNIQUE)
        {
            Damage(new DamageData(1, Keyword.PIERCING, _user, source.controller), false, null);
        }
    }
}

public class A_CarnivorousPitfall : CardAbility
{
    public A_CarnivorousPitfall(Card user) : base(user) { }

    public override string Text()
    {
        return "<b>Activate: </b> Add 2 <b>Pitfall Vine </b> to opponent's deck.";
    }

    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        CardData data = Resources.Load("Cards/Fen/PitfallVine") as CardData;
        _user.opponent.deck.Shuffle(data);
        _user.opponent.deck.Shuffle(data);
    }
}

public class A_PitfallVine : CardAbility
{
    public override string Text()
    {
        return "When you draw this, take 2 Crushing damage. Then, put this into play under your opponent's control. Draw a card.";
    }
    public A_PitfallVine(Card user) : base(user)
    {
        ((Card)user).cardEvents.onDraw += OnDrawHandler;
    }

    private void OnDrawHandler(Card card)
    {
        Actor owner = card.controller;
        Ability.Damage(new DamageData(2, Keyword.CRUSHING, card, card.controller), false, null);
        owner.PutInPlay(card, false);
        owner.Draw();
    }
}

public class A_BlossomingIvyProng : CardAbility
{
    public A_BlossomingIvyProng(Card user) : base(user)
    {
        ((Card)user).cardEvents.onEnterPlay += EnterPlayHandler;
    }
    public override string Text()
    {
        return "When this enters play, gain 1 HEALTH.";
    }

    void EnterPlayHandler(Card card)
    {
        _user.controller.IncrementHealth(1);
    }
}

public class A_RampagingSwordtusk : CardAbility
{
    public A_RampagingSwordtusk(Card user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
    }

    public override string Text()
    {
        return "<b>Activate:</b> Rampaging Swordtusk fights target opposing Thrall.";
    }

    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Fight((Card)targets[0], undo, state);
    }
}

public class A_Equanimity : CardAbility
{
    public A_Equanimity(Card user) : base(user)
    {

    }

    public override string Text()
    {
        string txt = "Gain 3/0 FOCUS.";
        txt += "<b>Passive: </b> Gain 1/0 FOCUS.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        if (_user.playerControlled)
        {
            ((Player)_user.controller).focus.baseValue += 3;
        }

    }

    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (_user.playerControlled)
        {
            ((Player)_user.controller).focus.baseValue += 1;
        }
    }

}

public class A_Ricochet : CardAbility
{
    private int _costMod;
    public A_Ricochet(Card user) : base(user)
    {
        _costMod = 0;
        playTargets.Add(TargetAnyOpposing());
        user.targetEvents.onRefresh += RefreshHandler;
    }

    public override string Text()
    {
        string txt = "Target enemy takes 2 PIERCING damage. Another random enemy takes 1 PIERCING damage.";
        txt += "\nThis card has Cost -2 if you control a Thrall with cost 3 or more.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage_1 = 2;
        int damage_2 = 1;

        ITargetable target = (ITargetable)targets[0];
        Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);

        ITargetable t2 = RandomOpposing((Card)_user, TargetAnyOpposing(targets[0]));
        if (t2 != null)
        {
            Damage(new DamageData(damage_2, Keyword.PIERCING, _user, t2), undo, state);
        }
        

    }
    private void RefreshHandler()
    {

        Card user = _user as Card;
        List<Card> cards = user.controller.active;
        bool doReduce = false;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL && card.cost.value >= 3)
            {
                doReduce = true;
            }
        }
        if (doReduce && _costMod == 0)
        {
            _costMod = -2;
            user.cost.RemoveModifiersFromSource(_user as Object);
            StatModifier mod = new StatModifier(_costMod, Stat.Name.COST, (Card)_user);
            user.AddModifier(mod);
        }
        else if (_costMod != 0 && !doReduce)
        {
            _costMod = 0;
            user.cost.RemoveModifiersFromSource(_user as Object);
        }
    }

}

public class A_ConcussiveShot : CardAbility
{
    int _costMod = 0;
    public A_ConcussiveShot(Card user) : base(user)
    {
        _costMod = 0;
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
    }

    public override string Text()
    {
        string txt = "Target enemy takes 2 PIERCING damage and is Dazed.";
        txt += "\nThis card has Cost -2 if you control a Thrall with cost 3 or more.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage_1 = 0;

        ITargetable target = (ITargetable)targets[0];
        Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);
        target.AddStatus(StatusEffect.ID.DAZE);
    }
    private void RefreshHandler()
    {
        Card user = _user as Card;
        List<Card> cards = user.controller.active;
        bool doReduce = false;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL && card.cost.value >= 3)
            {
                doReduce = true;
            }
        }
        if (doReduce && _costMod == 0)
        {
            _costMod = -2;
            user.cost.RemoveModifiersFromSource(_user as Object);
            StatModifier mod = new StatModifier(_costMod, Stat.Name.COST, (Card)_user);
            user.AddModifier(mod);
        }
        else if (_costMod != 0 && !doReduce)
        {
            _costMod = 0;
            user.cost.RemoveModifiersFromSource(_user as Object);
        }
    }
}

public class A_RangersJudgement : CardAbility
{
    int _costMod = 0;
    public A_RangersJudgement(Card user) : base(user)
    {
        _costMod = 0;
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
    }

    public override string Text()
    {
        string txt = "Target enemy takes 3 PIERCING damage and is Impaled.";
        txt += "\nThis card has Cost -2 if you control a Thrall with cost 3 or more.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage_1 = 3;

        ITargetable target = (ITargetable)targets[0];
        Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);
        target.AddStatus(StatusEffect.ID.IMPALE);
    }
    private void RefreshHandler()
    {
        Card user = _user as Card;
        List<Card> cards = user.controller.active;
        bool doReduce = false;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL && card.cost.value >= 3)
            {
                doReduce = true;
            }
        }
        if (doReduce && _costMod == 0)
        {
            _costMod = -2;
            user.cost.RemoveModifiersFromSource(_user as Object);
            StatModifier mod = new StatModifier(_costMod, Stat.Name.COST, (Card)_user);
            user.AddModifier(mod);
        }
        else if (_costMod != 0 && !doReduce)
        {
            _costMod = 0;
            user.cost.RemoveModifiersFromSource(_user as Object);
        }
    }
}

public class A_GenesisSpring : CardAbility
{
    public A_GenesisSpring(Card user) : base(user)
    { }

    public override string Text()
    {
        string txt = "";
        if (_user.controller is Player)
        {
            txt += "Gain 1/0 FOCUS for each Thrall you control.";
            txt += "\n<b>Passive:</b> Gain 1 HEALTH.";
        } else
        {
            txt += "Gain 1 HEALTH for each Thrall you control.";
        }
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int numThralls = 0;

        List<Card> cards = _user.controller.active;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL) { numThralls++; }
        }

        if (_user.controller is Player)
        {
            Player.instance.focus.baseValue += numThralls;
        }
    }

    protected override void Passive(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        if (_user.playerControlled)
        {
            _user.controller.IncrementHealth(1);
        }
    }
}

public class A_FensBlessing : CardAbility
{
    private TargetTemplate _template;
    public A_FensBlessing(Card user) : base(user)
    {
        _react = true;
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.THRALL);
        _template.inPlay = true;
        playTargets.Add(_template);
    }

    public override string Text()
    {
        string txt = "<b> Reaction: </b> Target Thrall is fully healed, gains +1 POWER and +1 Cost.";
        return txt;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Card target = targets[0] as Card;

        target.IncrementHealth(9999);
        target.power.AddModifier(new StatModifier(1, Stat.Name.POWER, (Card)_user));
        target.cost.AddModifier(new StatModifier(1, Stat.Name.COST, (Card)_user));
    }

}

public class A_ConsumingBlob : CardAbility
{
    public A_ConsumingBlob(Card user) : base(user)
    {
        TargetTemplate template = new TargetTemplate();
        template.cardType.Add(Card.Type.THRALL);
        template.isSelf = true;
        template.isNot = _user;
        template.inPlay = true;

        activateTargets.Add(template);
    }

    public override string Text()
    {
        string txt = "<b>Activate: </b> Destroy another target Thrall you control. This gains ENDURANCE and POWER equal to the target's.";
        return txt;
    }

    public override bool ActivationAvailable()
    {
        List<Card> cards = _user.controller.active;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL) { return true; }
        }
        return false;
    }

    

    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Card user = _user as Card;
        Card target = targets[0] as Card;
        user.endurance.AddModifier(new StatModifier(target.endurance.value, Stat.Name.HEALTH, user));
        user.power.AddModifier(new StatModifier(target.power.value, Stat.Name.POWER, user));
        target.Destroy();
    }

}

public class A_MitoticSlime : CardAbility
{
    public A_MitoticSlime(Card user) : base(user)
    {
        Card card = user as Card;
        card.cardEvents.onDestroy += DestroyHandler;
    }

    public override string Text()
    {
        string txt = "When this is destroyed, create two Slimelings and put them into play.";
        return txt;
    }

    private void DestroyHandler(Card card)
    {
        CardData data = Resources.Load("Cards/Fen/Slimeling") as CardData;
        for (int ii = 0; ii < 2; ii++)
        {
            Card slime = Card.Spawn(data, true, card.transform.position);
            _user.controller.PutInPlay(slime);
        }
    }
}

public class A_PackWolfAlpha : CardAbility
{
    private TargetTemplate _template;
    public A_PackWolfAlpha(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.THRALL);
        _template.inPlay = true;
        _template.isSelf = true;
        _template.keyword.Add(Keyword.BEAST);

        Card card = _user as Card;
        card.cardEvents.onEnterPlay += EnterPlayHandler;
    }

    public override string Text()
    {
        string txt = "Beasts you control have +1 POWER.";
        return txt;
    }

    private void EnterPlayHandler(Card card)
    {
        TemplateModifier mod = new TemplateModifier(+1, Stat.Name.POWER, _template, StatModifier.Duration.SOURCE, (Card)_user);
        Dungeon.AddModifier(mod);
    }
}
public class A_KyrnanosLordOfTheWild : CardAbility
{
    public A_KyrnanosLordOfTheWild(Card user) : base(user)
    {
        ((Card)_user).cardEvents.onEnterPlay += EnterPlayHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
    }

    public override string Text()
    {
        if (_user.playerControlled)
        {
            return "While Kyrnanos: Lord of the Wild is in play, Beasts you control have -1 UPKEEP";
        }
        else
        {
            return "";
        }
    }

    public void EnterPlayHandler(Card self)
    {
        TargetTemplate _template = new TargetTemplate();
        _template.inPlay = true;
        _template.isSelf = true;
        _template.keyword.Add(Keyword.BEAST);
        _template.cardType.Add(Card.Type.THRALL);
        TemplateModifier mod = new TemplateModifier(-1, Stat.Name.UPKEEP, _template, StatModifier.Duration.SOURCE, (Card)_user);
        Dungeon.AddModifier(mod);
    }
}

public class A_Stonesunder : CardAbility
{
    public A_Stonesunder(Card user) : base(user)
    {
        TargetTemplate t = new TargetTemplate();
        t.cardType.Add(Card.Type.THRALL);
        t.isOpposing = true;
        t.inPlay = true;
        t.templateParams.Add(new TemplateParam(TargetTemplate.Param.LEVEL, TargetTemplate.Op.LT, 3));
        playTargets.Add(t);
    }

    public override string Text()
    {
        string txt = "Target opposing Thrall with Cost < 3 takes 1 EARTH damage and is Dazed. \nDraw a card.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Damage(new DamageData(1, Keyword.EARTH, user, targets[0]), undo, state);
        targets[0].AddStatus(StatusEffect.ID.DAZE);
        user.controller.Draw();
    }
}

public class A_Fissure : CardAbility
{
    public A_Fissure(Card user) : base(user)
    {
        TargetTemplate t = new TargetTemplate();
        t.cardType.Add(Card.Type.THRALL);
        t.isOpposing = true;
        t.inPlay = true;
        t.notKeywordAbility.Add(KeywordAbility.Key.NIMBLE);
        t.templateParams.Add(new TemplateParam(TargetTemplate.Param.LEVEL, TargetTemplate.Op.LT, 4));
        playTargets.Add(t);
    }

    public override string Text()
    {
        string txt = "Destroy target opposing non-Nimble Thrall with Cost < 4.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        ((Card)targets[0]).Destroy();
    }
}

public class A_Rejuvenate : CardAbility
{
    public A_Rejuvenate(Card user) : base(user)
    {

    }

    public override string Text()
    {
        return "Gain +3 HEALTH. Draw a card.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        user.controller.IncrementHealth(3);
        user.controller.Draw();
    }
}

public class A_ConsumeAdrenaline : CardAbility
{
    public A_ConsumeAdrenaline(Card user) : base(user)
    {

    }
    public override string Text()
    {
        return "Gain 2 Frenzy. Then, gain +1 HEALTH for each Frenzy you have.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        user.controller.AddStatus(StatusEffect.ID.FRENZY, 2);
        user.controller.IncrementHealth(user.controller.GetStatus(StatusEffect.ID.FRENZY));
    }
}

public class A_HowlOfThePack : CardAbility
{
    public A_HowlOfThePack(Card user) : base(user)
    {

    }
    public override string Text()
    {
        return "Create two <b>Loyal Pack Wolf</b>. Then, +1 HEALTH for each Beast you control.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        CardData data = Resources.Load<CardData>("Cards/Fen/LoyalPackWolf") as CardData;
        for (int ii = 0; ii < 2; ii++)
        {
            Card wolf = Card.Spawn(data, user.playerControlled, Dungeon.GetZone(CardZone.Type.HAND, user.playerControlled).transform.position);
            user.controller.PutInPlay(wolf);
        }

        List<Card> cards = user.controller.GetCardsWithKeyword(Keyword.BEAST, CardZone.Type.ACTIVE);
        user.controller.IncrementHealth(cards.Count);
    }
}