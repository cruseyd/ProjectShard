using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameParams", fileName = "NewGameParams")]
public class GameParams : ScriptableObject
{
    public Color VIOLET;
    public Color RED;
    public Color GOLD;
    public Color GREEN;
    public Color BLUE;
    public Color INDIGO;
    public Color TAN;

    public int playerHandSize;
    public int enemyDrawMin;
    public int enemyDrawMax;
    public float hoverDelay = 0.5f;
    public float cardAnimationRate = 0.25f;

    /*
    public Color GetColor(Card.Color color)
    {
        switch (color)
        {
            case Card.Color.VIOLET: return VIOLET;
            case Card.Color.RED: return RED;
            case Card.Color.GOLD: return GOLD;
            case Card.Color.GREEN: return GREEN;
            case Card.Color.BLUE: return BLUE;
            case Card.Color.INDIGO: return INDIGO;
            case Card.Color.TAN: return TAN;
            default: return Color.white;
        }
    }
    */
}
