using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class Leaderboard
{
    // NAME , SCORE* , WAVE     *sorted by
    // index 0 = #1, index 9 = #10
    public (string, int, int)[] topTen = new (string, int, int)[10];
    public int filledSpaces;
}
