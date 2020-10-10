using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_TerritorialBriar : CardAbility
{
    public A_TerritorialBriar(Card user) : base(user)
    {
        user.opponent.actorEvents.onPlayCard += OnPlayCardHandler;
        user.opponent.targetEvents.onDeclareAttack += OnDeclareAttackHandler;
    }
    public override string Text()
    {
        string txt = "";
        txt += "When an opposing thrall attacks, it first takes 1 PIERCING damage.";
        txt += "When your opponent uses a Technique, they first take 1 PIERCING damage.";
        return txt;
    }

    private void OnDeclareAttackHandler(Card source, ITargetable target)
    {
        if (user.inPlay)
        {
            Ability.Damage(new DamageData(1, Keyword.PIERCING, user, source));
        }
    }
    private void OnPlayCardHandler(Card source)
    {
        if (_user.inPlay && source.type == Card.Type.TECHNIQUE)
        {
            Ability.Damage(new DamageData(1, Keyword.PIERCING, _user, source.controller));
        }
    }
}

public class A_BlossomingIvyProng : CardAbility
{
    public A_BlossomingIvyProng(Card user) : base(user)
    {
        user.cardEvents.onEnterPlay += EnterPlayHandler;
    }
    public override string Text()
    {
        return "When this enters play, gain 2 HEALTH.";
    }
    void EnterPlayHandler(Card card)
    {
        Ability.Heal(user.controller, 2);
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
        Ability.Fight(user, (Card)targets[0], undo, state);
    }
}

public class A_Equanimity : CardAbility
{
    public A_Equanimity(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }

    public override string Text()
    {
        string txt = "Gain 3/0 FOCUS.";
        txt += "<b>Cycle: </b> Gain 1/0 FOCUS.";
        return txt;
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.AddFocus(user.controller, 3, 0, undo, state);
    }

    private void CycleHandler(Card card)
    {
        Ability.AddFocus(user.controller, 1, 0);
    }
}

public class A_Ricochet : CardAbility
{
    private StatModifier _mod;
    public A_Ricochet(Card user) : base(user)
    {
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
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
        Ability.Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);

        ITargetable t2 = RandomOpposing((Card)_user, TargetAnyOpposing(targets[0]));
        if (t2 != null)
        {
            Ability.Damage(new DamageData(damage_2, Keyword.PIERCING, _user, t2), undo, state);
        }
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        bool doReduce = false;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL && card.cost.value >= 3)
            {
                doReduce = true;
            }
        }
        if (doReduce) { _mod.value = -2; }
        else { _mod.value = 0; }
    }
}

public class A_ConcussiveShot : CardAbility
{
    private StatModifier _mod;
    public A_ConcussiveShot(Card user) : base(user)
    {
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
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

        int damage_1 = 2;

        ITargetable target = (ITargetable)targets[0];
        Ability.Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);
        Ability.Status(target, StatusEffect.ID.DAZE, 1, undo, state);
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        bool doReduce = false;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL && card.cost.value >= 3)
            {
                doReduce = true;
            }
        }
        if (doReduce) { _mod.value = -2; }
        else { _mod.value = 0; }
    }
}

public class A_RangersJudgement : CardAbility
{
    private StatModifier _mod;
    public A_RangersJudgement(Card user) : base(user)
    {
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
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
        Ability.Status(target, StatusEffect.ID.IMPALE, 1, undo, state);
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        bool doReduce = false;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL && card.cost.value >= 3)
            {
                doReduce = true;
            }
        }
        if (doReduce) { _mod.value = -2; }
        else { _mod.value = 0; }
    }
}

public class A_GenesisSpring : CardAbility
{
    public A_GenesisSpring(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }

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

        Ability.AddFocus(user.controller, numThralls, 0, undo, state);
    }

    private void CycleHandler(Card card)
    {
        Ability.Heal(user.controller, 1);
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
        Ability.Heal(target, 9999, undo, state);
        Ability.AddStatModifier(target, new StatModifier(1, Stat.Name.POWER, (Card)_user), undo, state);
        Ability.AddStatModifier(target, new StatModifier(1, Stat.Name.COST, (Card)_user), undo, state);
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
        Card target = targets[0] as Card;
        Ability.AddStatModifier(user, new StatModifier(target.endurance.value, Stat.Name.ENDURANCE, user), undo, state);
        Ability.AddStatModifier(user, new StatModifier(target.endurance.value, Stat.Name.POWER, user), undo, state);
        Ability.DestroyCard(target, undo, state);
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
        CardData data = CardIndex.Get("SLIMELING");
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE);
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE);
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
        TemplateModifier mod = new TemplateModifier(+1, Stat.Name.POWER, user, Modifier.Duration.SOURCE, _template);
        Ability.AddTemplateModifier(mod);
    }
}

public class A_KyrnanosLordOfTheWild : CardAbility
{
    private TargetTemplate _template;
    public A_KyrnanosLordOfTheWild(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.inPlay = true;
        _template.isSelf = true;
        _template.keyword.Add(Keyword.BEAST);
        _template.cardType.Add(Card.Type.THRALL);
        user.cardEvents.onEnterPlay += EnterPlayHandler;
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
        TemplateModifier mod = new TemplateModifier(-1, Stat.Name.UPKEEP, user, Modifier.Duration.SOURCE, _template);
        Ability.AddTemplateModifier(mod);
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
        Ability.Damage(new DamageData(1, Keyword.EARTH, user, targets[0]), undo, state);
        Ability.Status(targets[0], StatusEffect.ID.DAZE, 1, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
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
        Ability.DestroyCard((Card)targets[0], undo, state);
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
        Ability.Heal(user.controller, 3, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
    }
}

public class A_ConsumeAdrenaline : CardAbility
{
    public A_ConsumeAdrenaline(Card user) : base(user)
    {

    }
    public override string Text()
    {
        return "Gain 3 Frenzy. Then, gain +1 HEALTH for each Frenzy you have.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, 3, undo, state);
        Ability.Heal(user.controller, user.controller.GetStatus(StatusEffect.ID.FRENZY), undo, state);
    }
}

public class A_HowlOfThePack : CardAbility
{
    public A_HowlOfThePack(Card user) : base(user)
    {

    }
    public override string Text()
    {
        return "Create two <b>Wolf</b>. Then, +2 HEALTH for each Beast you control.";
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        
        CardData data = CardIndex.Get("WOLF");
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE, undo, state);
        Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE, undo, state);

        List<Card> cards = user.controller.GetCardsWithKeyword(Keyword.BEAST, CardZone.Type.ACTIVE);
        Ability.Heal(user.controller, 2*cards.Count, undo, state);
    }
}

public class A_IvyprongSpiritcaller : CardAbility
{
    public A_IvyprongSpiritcaller(Card user) : base(user)
    {
        user.cardEvents.onEnterPlay += EnterPlayHandler;
    }

    public override string Text()
    {
        if (user.controller is Player)
        {
            return "When this enters play, Attune FEN.";
        }
        return "";
    }

    private void EnterPlayHandler(Card card)
    {
        Ability.Attune(user.controller, Card.Color.FEN);
    }
}