using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    string name { get; }
    void Execute();
    void Execute(GameState state);
    void Undo(GameState state);
    void Show();
}

public class AbilityCommand : ICommand
{
    private ITargetable _source;
    private List<ITargetable> _targets;
    private Ability _ability;
    private Ability.Mode _mode;

    public string name
    {
        get
        {
            if (_targets != null)
            {
                return _source.name + " targets " + _targets[0].name + " with " + _mode.ToString();
            } else
            {
                return _source.name + " uses " + _mode.ToString();
            }
            
        }
    }

    public AbilityCommand(Ability ability, Ability.Mode mode, ITargetable source, List<ITargetable> targets = null)
    {
        _ability = ability;
        _mode = mode;
        _source = source;
        _targets = targets;
    }

    public void Execute()
    {
        _source.Resolve(_mode, _targets);
    }

    public void Execute(GameState state)
    {
        _ability.Try(_mode,_targets, state);
    }

    public void Show()
    {
        _ability.Show(_mode, _targets);
    }

    public void Undo(GameState state)
    {
        _ability.Undo(_mode, _targets, state);
    }
}
