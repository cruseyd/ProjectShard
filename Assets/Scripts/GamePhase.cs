using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePhase
{
    private static GamePhase _enemyPhase;
    private static GamePhase _playerPhase;
    private static GamePhase _draftPhase;

    public static GamePhase player
    {
        get
        {
            if (_playerPhase == null)
            {
                _playerPhase = new PlayerTurnPhase();
            }
            return _playerPhase;
        }
    }
    public static GamePhase enemy
    {
        get
        {
            if (_enemyPhase == null)
            {
                _enemyPhase = new EnemyTurnPhase();
            }
            return _enemyPhase;
        }
    }

    public static GamePhase draft
    {
        get
        {
            if (_draftPhase == null)
            {
                _draftPhase = new EnemyTurnPhase();
            }
            return _draftPhase;
        }
    }

    private bool _priority;
    public bool priority
    {
        get { return _priority; }
        set
        {
            bool doRefresh = (_priority != value);
            _priority = value;
            if (doRefresh)
            {
                GameEvents.current.Refresh();
            }
        }
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Confirm();
}

public class PlayerTurnPhase : GamePhase
{
    public override void Enter()
    {
        priority = true;
        Player.instance.StartTurn();
    }
    public override void Exit()
    {
        Player.instance.EndTurn();
        GameEvents.current.EndTurn(Player.instance);
        Player.instance.actorEvents.EndTurn();
        Player.instance.actorEvents.PostTurn();
        priority = false;
    }
    public override void Confirm()
    {
        Dungeon.phase = GamePhase.enemy;
    }
}
public class EnemyTurnPhase : GamePhase
{
    private ICommand _currentMove;
    private bool _interrupt = false;
    public override void Enter()
    {
        priority = false;
        Dungeon.EnableConfirmButton(true);
        Dungeon.SetParticleUnderlay(false);
        GameEvents.current.StartTurn(Enemy.instance);
        Enemy.instance.actorEvents.StartTurn();
        Enemy.instance.actorEvents.BeginTurn();

        _currentMove = Enemy.instance.GetComponent<AIModule>().ChooseMove(AIStrategy.SIMULATE);
        if (_currentMove == null)
        {
            Dungeon.SetConfirmButtonText("End Enemy Turn");
        } else
        {
            _currentMove.Show();
            priority = true;
            Dungeon.SetConfirmButtonText("Accept");
        }
    }
    public override void Exit()
    {
        GameEvents.current.EndTurn(Enemy.instance);
        Enemy.instance.actorEvents.EndTurn();
        Enemy.instance.actorEvents.PostTurn();
        // end enemy turn
        Enemy.instance.DrawRandom();
    }
    public override void Confirm()
    {
        priority = false;
        if (_currentMove == null)
        {
            priority = false;
            Dungeon.phase = GamePhase.player;
        } else
        {
            Dungeon.instance.StartCoroutine(doNextMove());
        }
    }

    private IEnumerator doNextMove()
    {
        if (!_interrupt)
        {
            _currentMove.Execute();
        } else
        {
            _interrupt = false;
        }
        
        _currentMove = Enemy.instance.GetComponent<AIModule>().ChooseMove(AIStrategy.SIMULATE);
        yield return new WaitForSeconds(GameData.instance.cardAnimationRate);
        if (_currentMove != null)
        {
            _currentMove.Show();
            priority = true;
            GameEvents.current.Refresh();
        }
        else {
            Dungeon.ClearTargeter();
            priority = true;
            Dungeon.SetConfirmButtonText("End Enemy Turn");
        }
    }

    public void Interrupt()
    {
        _interrupt = true;
        Enemy.instance.Discard(Dungeon.magnifiedCard);
        Dungeon.ClearTargeter();
    }
}

public class DraftPhase : GamePhase
{

    private List<Drafter> _drafters;
    private Queue<DraftPack> _packs;

    public override void Confirm()
    {
        Next();
    }

    public override void Enter()
    {
        _drafters = new List<Drafter>();
        _packs = new Queue<DraftPack>();
        // create List of Drafters
        // Initialize non-player drafter AI
        // generate DraftPacks
        // display first pack
        // load Player deck into drafter 0
    }

    public override void Exit()
    {
        // update Player deck
        // configure AI based on AI drafted cards
        // create next encounter
        // update Enemy deck
    }

    private void Next()
    {
        // process picks for all AI drafters
        // rotate DraftPack contents

    }
}
