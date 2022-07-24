using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BB_Utils;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> musicTracks = new List<AudioClip>();

    AudioSource _as;

    [SerializeField]
    int currentTrack;
    
    // Start is called before the first frame update
    void Start()
    {
        _as = GetComponent<AudioSource>();

        musicTracks.Shuffle(false);
        currentTrack = -1; // Increments before first Play()
    }

    // Update is called once per frame
    void Update()
    {
        _as.volume = 1f;
        if (!_as.isPlaying) {
            currentTrack++;
            if (currentTrack > musicTracks.Count - 1)
                currentTrack = 0;
            _as.clip = musicTracks[currentTrack];
            _as.Play();
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            _as.Stop(); // will run previous code and go to the next song
        }
    }
}
