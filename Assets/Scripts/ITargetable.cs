using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IMonoBehaviour
{
    string name { get; }
    Transform transform { get; }
    GameObject gameObject { get; }


}

public interface ITargetable : IMonoBehaviour
{
    Actor Controller();
    void AddTarget(ITargetable target);
    void FindTargets(Ability.Mode mode, int n, bool show = false);
    List<ICommand> FindMoves();
    void MarkTarget(TargetTemplate query, ITargetable source, bool show);
    bool Compare(TargetTemplate query, Actor self);
    TargetTemplate GetQuery(Ability.Mode mode, int n);
    bool Resolve(Ability.Mode mode, List<ITargetable> targets);

    void AddStatus(StatusName id, int stacks = 1);
    void RemoveStatus(StatusName id, int stacks = 1);
    int GetStatus(StatusName id);
}
