using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePhase
{
    private static GamePhase _enemyPhase;
    private static GamePhase _playerPhase;

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

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Confirm();
}

public class PlayerTurnPhase : GamePhase
{
    public override void Enter()
    {
        Player.instance.burnAvailable = true;
        Player.instance.focus.baseValue = Player.instance.maxFocus.value;
        Player.instance.Redraw();

        Dungeon.SetConfirmButtonText("End Turn");
        Dungeon.EnableConfirmButton(true);
        Dungeon.SetParticleUnderlay(true);

        Player.instance.events.StartTurn();
    }
    public override void Exit()
    {
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
        
        Dungeon.EnableConfirmButton(true);
        Dungeon.SetParticleUnderlay(false);
        Enemy.instance.events.StartTurn();

        _currentMove = Enemy.instance.GetComponent<AIModule>().ChooseMove();
        if (_currentMove == null)
        {
            Dungeon.SetConfirmButtonText("End Enemy Turn");
        } else
        {
            _currentMove.Show();
            Dungeon.SetConfirmButtonText("Accept");
        }

    }
    public override void Exit()
    {
        // end enemy turn
        Enemy.instance.DrawRandom();
    }
    public override void Confirm()
    {
        if (_currentMove == null)
        {
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
        
        _currentMove = Enemy.instance.GetComponent<AIModule>().ChooseMove();
        yield return new WaitForSeconds(Dungeon.gameParams.cardAnimationRate);
        if (_currentMove != null)
        {
            _currentMove.Show();
            GameEvents.current.Refresh();
        }
        else {
            Targeter.Clear();
            Dungeon.SetConfirmButtonText("End Enemy Turn");
        }
    }

    public void Interrupt()
    {
        _interrupt = true;
        Enemy.instance.Discard(Dungeon.magnifiedCard);
        Targeter.Clear();
    }
}

