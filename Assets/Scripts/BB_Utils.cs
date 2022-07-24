using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BB_Utils
{
    [System.Flags] public enum ScoringType {
        None = 0,
        Headshot = 1 << 0,
        LongRange = 1 << 1,
        Style = 1 << 2,
    }

    public enum ComboScoringFunction {
        SummationEquation,
        LinearEquation,
    }

    public enum GameState {
        Undefined,
        Paused,
        Running,
        Dead,
        Complete,
    }

    public static string HELP_TEXT = "[WASD]/Arrow Keys for Movement\n" +
                                     "[Space] for Jump\n" +
                                     "Mouse for Camera Movement\n" +
                                     "[Left Click] to Throw Ball\n" +
                                     "[Right Click] to Drop Ball\n" +
                                     "[Left Shift] to Sprint\n" +
                                     "[Left Ctrl/Command] to Crouch";

    public static int BoolToInt(bool boolean) {
        return boolean ? 1 : 0;
    }
    public static bool IntToBool(int integer) {
        return integer != 0;
    }

    public static bool ToggleCursor(bool toggleOn) {
        if (toggleOn) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        return !toggleOn;
    }

    public static bool OnlyWhiteSpace(string str) {
        foreach(char c in str)
            if (c != ' ') return false;
        return true;
        
    }

    public static bool HasFlag(ScoringType parent, ScoringType flag) {
        return (parent & flag) == flag;
    }

    public static float HorizontalDistance(Vector3 v1, Vector3 v2) {
        return Vector3.Distance(new Vector3(v1.x, 0f, v1.z), new Vector3(v2.x, 0f, v2.z));
    }

    public static int SummationEquation(int x, int scalar) {
        return scalar * (x * (x + 1) / 2); // summation formula: i=1 Sigma n (i) = n(n+1)/2
    }

    public static int SquareEquation(int x, int scalar) {
        return scalar * (x ^ 2);
    }

    public static int LinearEquation(int x, int scalar) {
        return scalar * x;
    }

    public static int SumOfComboFunction(ComboScoringFunction fn, int currentCombo, int scalar) {
        if (currentCombo <= 0)
            return 0;
        else {
            if(fn == ComboScoringFunction.LinearEquation) {
                int sum = 0;
                for (int i = 1; i <= currentCombo; i++) {
                    sum += LinearEquation(i, scalar);
                }
                return sum;
            }
            else if(fn == ComboScoringFunction.SummationEquation) {
                int sum = 0;
                for (int i = 1; i <= currentCombo; i++) {
                    sum += SummationEquation(i, scalar);
                }
                return sum;
            }
            else {
                return 0;
            }
        }
    }

    // https://www.reddit.com/r/Unity3D/comments/36pzid/shuffling_list_without_destroying_it/
    public static void Shuffle<T>(this IList<T> list, bool indexMinAdvance = true) {
        for(int i = 0; i < list.Count; i++) {
            var temp = list[i];
            int lowerBound;
            if (indexMinAdvance)
                lowerBound = i;
            else
                lowerBound = 0;
            int randIdx = Random.Range(lowerBound, list.Count);
            list[i] = list[randIdx];
            list[randIdx] = temp;
        }
    }
}
