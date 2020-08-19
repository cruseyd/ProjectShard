using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Icons
{

    public static string power = "<sprite index=0>";
    public static string endurance = "<sprite index=1>";
    public static string upkeep = "<sprite index=2>";
    public static string strength = "<sprite index=3>";
    public static string finesse = "<sprite index=4>";
    public static string perception = "<sprite index=5>";
    public static string focus = "<sprite index=6>";
    public static string health = "<sprite index=7>";

    public static string raiz = "<sprite index=8>";
    public static string iri = "<sprite index=9>";
    public static string fen = "<sprite index=10>";
    public static string lis = "<sprite index=11>";
    public static string ora = "<sprite index=12>";
    public static string vael = "<sprite index=13>";

    public static string slashing = "<sprite index=16>";
    public static string piercing = "<sprite index=17>";
    public static string crushing = "<sprite index=18>";

    public static string fire = "<sprite index=19>";
    public static string water = "<sprite index=20>";
    public static string ice = "<sprite index=21>";
    public static string lightning = "<sprite index=22>";
    public static string wind = "<sprite index=23>";
    public static string earth = "<sprite index=24>";
    public static string light = "<sprite index=25>";
    public static string dark = "<sprite index=26>";

    public static string arcane = "<sprite index=27>";
    public static string mystic = "<sprite index=28>";
    public static string elder = "<sprite index=29>";

    public static string Get(Keyword key)
    {
        switch (key)
        {
            case Keyword.SLASHING: return slashing;
            case Keyword.PIERCING: return piercing;
            case Keyword.CRUSHING: return crushing;
            case Keyword.FIRE: return fire;
            case Keyword.WATER: return water;
            case Keyword.ICE: return ice;
            case Keyword.LIGHTNING: return lightning;
            case Keyword.WIND: return wind;
            case Keyword.EARTH: return earth;
            case Keyword.LIGHT: return light;
            case Keyword.DARK: return dark;
            case Keyword.ARCANE: return arcane;
            case Keyword.MYSTIC: return mystic;
            case Keyword.ELDER: return elder;
            default: return "";
        }
    }

    public static string Parse(string input)
    {
        string output = input;
        output = output.Replace("POWER", power);
        output = output.Replace("HEALTH", health);
        output = output.Replace("UPKEEP", upkeep);

        output = output.Replace("STRENGTH", strength);
        output = output.Replace("FINESSE", finesse);
        output = output.Replace("PERCEPTION", perception);

        output = output.Replace("FOCUS", focus);
        output = output.Replace("HEALTH", health);

        output = output.Replace("RAIZ", raiz);
        output = output.Replace("IRI", iri);
        output = output.Replace("FEN", fen);
        output = output.Replace("LIS", lis);
        output = output.Replace("ORA", ora);
        output = output.Replace("VAEL", vael);

        output = output.Replace("SLASHING", slashing);
        output = output.Replace("PIERCING", piercing);
        output = output.Replace("CRUSHING", crushing);

        output = output.Replace("FIRE", fire);
        output = output.Replace("WATER", water);
        output = output.Replace("ICE", ice);
        output = output.Replace("LIGHTNING", lightning);
        output = output.Replace("WIND", wind);
        output = output.Replace("EARTH", earth);

        output = output.Replace("LIGHT", light);
        output = output.Replace("DARK", dark);

        output = output.Replace("ARCANE", arcane);
        output = output.Replace("MYSTIC", mystic);
        output = output.Replace("ELDER", elder);
        return output;
    }
}
