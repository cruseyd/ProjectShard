using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage_1 = user.values[0].value;
        int damage_2 = user.values[1].value;

        Ability.Damage(new DamageData(damage_1, Keyword.PIERCING, _user, targets[0]), undo, state);

        ITargetable t2 = RandomOpposing((Card)_user, TargetAnyOpposing(targets[0]));
        if (t2 != null)
        {
            Ability.Damage(new DamageData(damage_2, Keyword.PIERCING, _user, t2), undo, state);
        }
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        int reduction = 0;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL)
            {
                reduction += 1;
            }
        }
        _mod.value = -reduction;
    }
}
public class A_PiercingBolt : CardAbility
{
    private StatModifier _mod;
    public A_PiercingBolt(Card user) : base(user)
    {
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetOpposingThrall());
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage = user.values[0].value;

        Ability.Damage(new DamageData(damage, Keyword.PIERCING, _user, targets[0]), undo, state);
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        int reduction = 0;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL)
            {
                reduction += 1;
            }
        }
        _mod.value = -reduction;
    }
}
public class A_ConcussiveArrow : CardAbility
{
    private StatModifier _mod;
    public A_ConcussiveArrow(Card user) : base(user)
    {
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage_1 = user.values[0].value;

        ITargetable target = (ITargetable)targets[0];
        Ability.Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);
        Ability.Status(target, StatusEffect.ID.DAZE, 1, undo, state);
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        int reduction = 0;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL)
            {
                reduction += 1;
            }
        }
        _mod.value = -reduction;
    }
}
public class A_MasterfulShot : CardAbility
{
    private StatModifier _mod;
    public A_MasterfulShot(Card user) : base(user)
    {
        _user.targetEvents.onRefresh += RefreshHandler;
        playTargets.Add(TargetAnyOpposing());
        _mod = new StatModifier(0, Stat.Name.COST, user);
        user.AddModifier(_mod);
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int damage_1 = user.values[0].value;

        ITargetable target = (ITargetable)targets[0];
        Damage(new DamageData(damage_1, Keyword.PIERCING, _user, target), undo, state);
        Ability.Status(target, StatusEffect.ID.IMPALE, 1, undo, state);
    }
    private void RefreshHandler()
    {
        List<Card> cards = user.controller.active;
        int reduction = 0;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL)
            {
                reduction += 1;
            }
        }
        _mod.value = -reduction;
    }
}
public class A_PackMentality : CardAbility
{
    public A_PackMentality(Card user) : base(user) { }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        int flatValue = user.values[0].value;
        int multiplier = user.values[1].value;
        int numThralls = 0;

        List<Card> cards = _user.controller.active;
        foreach (Card card in cards)
        {
            if (card.type == Card.Type.THRALL) { numThralls++; }
        }

        Ability.AddFocus(user.controller, flatValue + numThralls*multiplier, 0, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
    }
}
public class A_EchoingHowl : CardAbility
{
    public A_EchoingHowl(Card user) : base(user)
    {

    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);

        CardData data = CardIndex.Get("WOLF");
        int numWolves = user.values[0].value;
        for (int ii = 0; ii < numWolves; ii++)
        {
            Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE, undo, state);
        }
        foreach (Card card in user.controller.active)
        {
            if (card.HasKeyword(Keyword.WOLF))
            {
                Ability.AddStatModifier(card, new StatModifier(-1, Stat.Name.UPKEEP, user, Modifier.Duration.END_OF_TURN), undo, state);
            }
        }
    }
}
public class A_AdrenalineRush : CardAbility
{
    public A_AdrenalineRush(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int frenzyGain = user.values[0].value;
        int healthMult = user.values[1].value;
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, frenzyGain, undo, state);
        Ability.Heal(user.controller, user.controller.GetStatus(StatusEffect.ID.FRENZY)*healthMult, undo, state);
    }

    private void CycleHandler(Card card)
    {
        int cycleFrenzyGain = user.values[2].value;
        Ability.Status(user.controller, StatusEffect.ID.FRENZY, cycleFrenzyGain);
    }
}
public class A_Kyrnanos : CardAbility
{
    private TargetTemplate _template;
    public A_Kyrnanos(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.inPlay = true;
        _template.isSelf = true;
        _template.isNot = user;
        _template.keywordAnd.Add(Keyword.BEAST);
        _template.cardType.Add(Card.Type.THRALL);
        user.cardEvents.onEnterPlay += EnterPlayHandler;
    }
    public void EnterPlayHandler(Card self)
    {
        int upkeepValue = user.values[0].value;
        TemplateModifier mod = new TemplateModifier(-upkeepValue, Stat.Name.UPKEEP, user, Modifier.Duration.SOURCE, _template);
        Ability.AddTemplateModifier(mod);
    }
}
public class A_ShadowFangAlpha : CardAbility
{
    private TargetTemplate _template;
    public A_ShadowFangAlpha(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.THRALL);
        _template.inPlay = true;
        _template.isSelf = true;
        _template.keywordAnd.Add(Keyword.WOLF);

        Card card = _user as Card;
        card.cardEvents.onEnterPlay += EnterPlayHandler;
    }
    private void EnterPlayHandler(Card card)
    {
        int power = user.values[0].value;
        TemplateModifier mod = new TemplateModifier(+power, Stat.Name.POWER, user, Modifier.Duration.SOURCE, _template);
        Ability.AddTemplateModifier(mod);
    }
}
public class A_ShadowFangAmbusher : CardAbility
{
    public A_ShadowFangAmbusher(Card user) : base(user)
    {
        user.opponent.actorEvents.onPlayCard += PlayCardHandler;
    }

    private void PlayCardHandler(Card card)
    {
        if (card.type == Card.Type.THRALL && user.inHand)
        {
            int alacrityGain = user.values[0].value;
            Ability.Status(user, StatusEffect.ID.ALACRITY, alacrityGain);
        }
    }
}
public class A_LoyalShadowFang : CardAbility
{
    public A_LoyalShadowFang(Card user) : base(user)
    {
        user.controller.actorEvents.onPlayCard += PlayCardHandler;
    }

    private void PlayCardHandler(Card card)
    {
        if (card.HasKeyword(Keyword.WOLF) && user.inDiscard)
        {
            user.controller.PutInPlay(user);
        }
    }
}
public class A_BlossomingIvyProng : CardAbility
{
    public A_BlossomingIvyProng(Card user) : base(user)
    {
        user.cardEvents.onEnterPlay += EnterPlayHandler;
    }
    void EnterPlayHandler(Card card)
    {
        int health = user.values[0].value;
        Ability.Heal(user.controller, health);
    }
}
public class A_IvyprongSpiritcaller : CardAbility
{
    public A_IvyprongSpiritcaller(Card user) : base(user)
    {
        user.cardEvents.onEnterPlay += EnterPlayHandler;
    }
    private void EnterPlayHandler(Card card)
    {
        Ability.Attune(user.controller, Card.Color.FEN);
    }
}
public class A_RampagingSwordtusk : CardAbility
{
    public A_RampagingSwordtusk(Card user) : base(user)
    {
        activateTargets.Add(TargetOpposingThrall());
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
public class A_TerritorialBriar : CardAbility
{
    public A_TerritorialBriar(Card user) : base(user)
    {
        user.opponent.actorEvents.onPlayCard += OnPlayCardHandler;
        user.opponent.targetEvents.onDeclareAttack += OnDeclareAttackHandler;
    }

    private void OnDeclareAttackHandler(Card source, ITargetable target)
    {
        if (user.inPlay)
        {
            int damage = user.values[0].value;
            Ability.Damage(new DamageData(damage, Keyword.PIERCING, user, source));
        }
    }
    private void OnPlayCardHandler(Card source)
    {
        if (_user.inPlay && source.type == Card.Type.TECHNIQUE)
        {
            int damage = user.values[1].value;
            Ability.Damage(new DamageData(damage, Keyword.PIERCING, _user, source.controller));
        }
    }
}
public class A_MitoticSlime : CardAbility
{
    public A_MitoticSlime(Card user) : base(user)
    {
        user.cardEvents.onDestroy += DestroyHandler;
    }
    private void DestroyHandler(Card card)
    {
        CardData data = CardIndex.Get("SLIMELING");
        int numSlimes = user.values[0].value;
        for (int ii = 0; ii < numSlimes; ii++)
        {
            Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE);
        }
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
        int might = user.values[0].value;
        Ability.Status(user, StatusEffect.ID.MIGHT, might, undo, state);
        Ability.DestroyCard(target, undo, state);
    }

}
public class A_AmorphousDevourer : CardAbility
{
    public A_AmorphousDevourer(Card user) : base(user)
    {
        user.controller.actorEvents.onStartTurn += StartTurnHandler;
        user.cardEvents.onDestroy += DestroyHandler;
    }

    private void StartTurnHandler(Actor actor)
    {
        if (user.inPlay)
        {
            int mightPerTurn = user.values[0].value;
            Ability.Status(user, StatusEffect.ID.MIGHT, mightPerTurn);
        }
    }
    private void DestroyHandler(Card card)
    {
        CardData data = CardIndex.Get("SLIMELING");
        int numSlimes = user.GetStatus(StatusEffect.ID.MIGHT);
        for (int ii = 0; ii < numSlimes; ii++)
        {
            Ability.CreateCard(user.controller, data, user.transform.position, CardZone.Type.ACTIVE);
        }
    }
}
public class A_Empower : CardAbility
{
    private TargetTemplate _template;
    public A_Empower(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.cardType.Add(Card.Type.THRALL);
        _template.inPlay = true;
        playTargets.Add(_template);
    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Card target = targets[0] as Card;
        int mightStacks = user.values[0].value;
        int levels = user.values[1].value;
        StatModifier mod = new StatModifier(levels, Stat.Name.COST, user);
        Ability.Heal(target, 9999, undo, state);
        Ability.Status(target, StatusEffect.ID.MIGHT, mightStacks, undo, state);
        Ability.AddStatModifier(target, mod, undo, state);
    }
}
public class A_Rejuvenate : CardAbility
{
    public A_Rejuvenate(Card user) : base(user)
    {

    }

    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int health = user.values[0].value;
        Ability.Heal(user.controller, health, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
    }
}
public class A_Verdure : CardAbility
{
    public A_Verdure(Card user) : base(user) { }

    public override bool ActivationAvailable()
    {
        return true;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        int upkeep = user.values[0].value;
        foreach (Card card in user.controller.active)
        {
            if (card.type == Card.Type.THRALL)
            {
                Ability.Heal(card, 9999, undo, state);
                Ability.Cleanse(card, undo, state);
                Ability.AddStatModifier(card, new StatModifier(-upkeep, Stat.Name.UPKEEP, user, Modifier.Duration.END_OF_TURN), undo, state);
            }
        }
    }
}

public class A_WillOfTheWild : CardAbility
{
    public A_WillOfTheWild(Card user) : base(user)
    {
        playTargets.Add(TargetFriendlyThrall());
        playTargets.Add(TargetOpposingThrall());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Card aggressor = targets[0] as Card;
        Card target = targets[1] as Card;
        Ability.Fight(aggressor, target, undo, state);
    }
}
public class A_Survivalism : CardAbility
{
    public A_Survivalism(Card user) : base(user)
    {
        playTargets.Add(TargetFriendlyThrall());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Card target = targets[0] as Card;
        Ability.Heal(target, user.values[0].value, undo, state);
        Ability.Draw(user.controller, 1, undo, state);
    }
}
public class A_FirstAid : CardAbility
{
    public A_FirstAid(Card user) : base(user)
    {
        playTargets.Add(TargetAnyFriendly());
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.Heal(targets[0], user.values[0].value, undo, state);
        if (targets[0] is Card)
        {
            Card card = targets[0] as Card;
            StatModifier mod = new StatModifier(-user.values[1].value, Stat.Name.UPKEEP, user, Modifier.Duration.END_OF_TURN);
            Ability.AddStatModifier(card, mod, undo, state);
        }
    }
}
public class A_PrimalStrength : CardAbility
{
    public A_PrimalStrength(Card user) : base(user)
    {
    }
    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        foreach (Card card in user.controller.active)
        {
            if (user.type == Card.Type.THRALL)
            {
                Ability.Cleanse(card, undo, state);
                Ability.Heal(card, 9999, undo, state);
                Ability.Status(card, StatusEffect.ID.MIGHT, user.values[0].value, undo, state);
            }
        }
        Ability.Draw(user.controller, 1, undo, state);
    }
}

public class A_ObstinateOakenwold : CardAbility
{
    public A_ObstinateOakenwold(Card user) : base (user)
    {
        user.targetEvents.onGainHealth += OnHealHandler;
    }

    private void OnHealHandler(int health)
    {
        Ability.Status(user, StatusEffect.ID.ARMOR, user.values[0].value);
    }
}

public class A_WeatheredWillowold : CardAbility
{
    public A_WeatheredWillowold(Card user) : base(user)
    {
        user.targetEvents.onGainHealth += OnHealHandler;
    }

    private void OnHealHandler(int health)
    {
        Ability.Draw(user.controller, 1);
    }
}

public class A_LumberingBanebough : CardAbility
{
    private StatModifier mod;
    public A_LumberingBanebough(Card user) : base(user)
    {
        user.targetEvents.onGainHealth += OnHealHandler;
        user.controller.actorEvents.onEndTurn += OnEndTurnHandler;
        mod = new StatModifier(0, Stat.Name.POWER, user, Modifier.Duration.PERMANENT);
        user.AddModifier(mod);
    }

    private void OnHealHandler(int health)
    {
        mod.value = user.values[0].value;
        user.AddKeywordAbility(KeywordAbility.Key.OVERWHELM);
    }

    private void OnEndTurnHandler(Actor actor)
    {
        mod.value = 0;
        user.RemoveKeywordAbility(KeywordAbility.Key.OVERWHELM);
    }
}

public class A_TitanRedwold : CardAbility
{
    public A_TitanRedwold(Card user) : base(user)
    {
        user.targetEvents.onGainHealth += OnHealHandler;
    }

    private void OnHealHandler(int health)
    {
        Ability.Status(user, StatusEffect.ID.MIGHT, user.values[0].value);
    }
}

public class A_DryadWoodweaver : CardAbility
{
    public A_DryadWoodweaver(Card user) : base(user)
    {
        activateTargets.Add(TargetFriendlyThrall());
    }
    public override bool ActivationAvailable()
    {
        return user.inPlay;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Ability.Status(targets[0], StatusEffect.ID.ARMOR, user.values[0].value, undo, state);

    }
}

public class A_DryadMender : CardAbility
{
    public A_DryadMender(Card user) : base(user)
    {
        activateTargets.Add(TargetFriendlyThrall());
    }
    public override bool ActivationAvailable()
    {
        return user.inPlay;
    }
    protected override void Activate(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Activate(targets, undo, state);
        Ability.Heal(targets[0], user.values[0].value, undo, state);

    }
}
public class A_Eridea : CardAbility
{
    private TargetTemplate _template;
    public A_Eridea(Card user) : base(user)
    {
        _template = new TargetTemplate();
        _template.inPlay = true;
        _template.isSelf = true;
        _template.keywordAnd.Add(Keyword.PLANT);
        _template.cardType.Add(Card.Type.THRALL);

        user.cardEvents.onEnterPlay += EnterPlayHandler;
        user.controller.actorEvents.onStartTurn += StartTurnHandler;
    }
    private void EnterPlayHandler(Card self)
    {
        int upkeepValue = user.values[0].value;
        TemplateModifier mod = new TemplateModifier(-upkeepValue, Stat.Name.UPKEEP, user, Modifier.Duration.SOURCE, _template);
        Ability.AddTemplateModifier(mod);
    }

    private void StartTurnHandler(Actor actor)
    {
        foreach (Card card in user.controller.active)
        {
            Ability.Heal(card, user.values[1].value);
        }
    }
}
public class A_Equanimity : CardAbility
{
    public A_Equanimity(Card user) : base(user)
    {
        user.cardEvents.onCycle += CycleHandler;
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


    protected override void Play(List<ITargetable> targets, bool undo = false, GameState state = null)
    {
        base.Play(targets, undo, state);
        Ability.DestroyCard((Card)targets[0], undo, state);
    }
}







