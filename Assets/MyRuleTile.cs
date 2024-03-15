using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Neighbour
{
    public const int This = 1;
    public const int NotThis = 2;
    public const int Empty = 3;
    public const int NotEmpty = 4;
}

[CreateAssetMenu]
public class MyRuleTile : RuleTile<Neighbour> {    

    public override bool RuleMatch(int neighbor, TileBase other) {
        if (other is RuleOverrideTile ot)
            other = ot.m_InstanceTile;

        switch (neighbor) {
            case Neighbour.This: return other == this;
            case Neighbour.NotThis: return other != this;
            case Neighbour.Empty: return other == null;
            case Neighbour.NotEmpty: return other != null;
        }
        return true;
    }
}
