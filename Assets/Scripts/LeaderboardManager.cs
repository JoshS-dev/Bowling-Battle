using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;

public class LeaderboardManager : MonoBehaviour
{
    public Leaderboard lb = new Leaderboard();
    private readonly BinaryFormatter bf = new BinaryFormatter();

    private string FILEPATH;

    [DllImport("__Internal")]
    private static extern void SyncFiles();
    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);

    // Start is called before the first frame update
    void Start(){ // LOAD LEADERBOARD
        FILEPATH = Application.persistentDataPath + "/Leaderboard.dat";
        FileStream fs;
        try {
            if (File.Exists(FILEPATH)) {
                fs = File.Open(FILEPATH, FileMode.Open);
                lb = bf.Deserialize(fs) as Leaderboard;
                fs.Close();
            }
        }
        catch(Exception e) {
            PlatformSafeMessage("Failed to load: " + e.Message);
        }

        //UpdateLeaderboard("JSJ", 100, 5);
        //UpdateLeaderboard("Tommy", 69, 3);
        //UpdateLeaderboard("BopBip", 0, 0);
        //DeleteLeaderboard();
        /*
        Debug.Log(lb.filledSpaces);
        foreach ((string, int, int) score in lb.topTen)
            Debug.Log(score); 
        */
    }

    public void UpdateLeaderboard(string newscoreName, int newscoreScore, int newscoreWave, int desiredIndex = -2) {
        // Search top ten to see if newscore can fit, and find where it goes
        if(desiredIndex == -2) {
            desiredIndex = FitInLeaderboard(newscoreScore);
        }
        if (desiredIndex == -1)
            return;


        for(int i = Mathf.Min(lb.filledSpaces - 1, 9 - 1); i >= desiredIndex; i--) {
            lb.topTen[i + 1] = lb.topTen[i];
        }
        lb.topTen[desiredIndex] = (newscoreName, newscoreScore, newscoreWave);
        if (lb.filledSpaces != 10)
            lb.filledSpaces++;

        SaveLeaderboard();
    }

    public int FitInLeaderboard(int score) {
        if (lb.filledSpaces == 0)
            return 0;

        for (int i = 0; i < lb.filledSpaces; i++) {
            if (score > lb.topTen[i].Item2)
                return i;
        }
        
        if (lb.filledSpaces != 10) // if score isnt higher than any other, but leaderboard not full, add to end
            return lb.filledSpaces;
        
        return -1; // no space in the leaderboard
    }

    private void SaveLeaderboard() {
        FileStream fs;
        try {
            if (File.Exists(FILEPATH)) {
                File.WriteAllText(FILEPATH, string.Empty);
                fs = File.Open(FILEPATH, FileMode.Open);
            }
            else {
                fs = File.Create(FILEPATH);
            }
            bf.Serialize(fs, lb);
            fs.Close();

            if (Application.platform == RuntimePlatform.WebGLPlayer)
                SyncFiles();
        }
        catch(Exception e) {
            PlatformSafeMessage("Failed to save: " + e.Message);
        }
    }

    public void DeleteLeaderboard() {
        File.Delete(FILEPATH);
        lb = new Leaderboard();
    }

    private static void PlatformSafeMessage(string message) {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            WindowAlert(message);
        }
        else {
            Debug.Log(message);
        }
    }
}
