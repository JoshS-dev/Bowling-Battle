using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using TMPro;
using QuantumTek.QuantumUI;

using static BB_Utils;

public class OptionsHandler : MonoBehaviour
{
    private SceneHandler _sh;
    private PostProcessingController _ppc;
    private PlayerMovement _pm;
    LeaderboardManager _lbm;

    [SerializeField]
    Toggle fullscreenToggle;
    [SerializeField]
    TMP_Dropdown resolutionDropdown;
    [SerializeField]
    QUI_OptionList qualityOptions;
    [SerializeField]
    Toggle bloomToggle, vignetteToggle, filmGToggle, chromAbbToggle;
    [SerializeField]
    Slider mouseSensSlider;
    [SerializeField]
    Toggle musicMuteToggle, sfxMuteToggle;
    [SerializeField]
    Slider musicSlider, sfxSlider;
    [SerializeField]
    AudioMixer parentMixer;

    Resolution[] resolutions;

    private bool DODEBUG = false;

    void Awake(){
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();
        _ppc = GameObject.Find("/Global Volume").GetComponent<PostProcessingController>();
        _pm = GameObject.Find("/Player").GetComponent<PlayerMovement>();
        _lbm = GameObject.Find("/SceneHandler").GetComponent<LeaderboardManager>();

        //int currentResIdx = 0;
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();
        foreach(var res in resolutions) {
            resOptions.Add(res.width + " x " + res.height);
        }
        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.RefreshShownValue();

        qualityOptions.SetOption(2);

        if (!(PlayerPrefs.HasKey("fullscreen") && PlayerPrefs.HasKey("resolution") && PlayerPrefs.HasKey("quality") &&
              PlayerPrefs.HasKey("bloom") && PlayerPrefs.HasKey("vignette") && PlayerPrefs.HasKey("film_grain") && PlayerPrefs.HasKey("chrom_abb") &&
              PlayerPrefs.HasKey("sensitivity") && PlayerPrefs.HasKey("music_mute") && PlayerPrefs.HasKey("music_lvl") &&
              PlayerPrefs.HasKey("sfx_mute") && PlayerPrefs.HasKey("sfx_lvl"))) {
                SetDefaults();
                if (DODEBUG)
                    Debug.Log("NO KEYS");
        }
        else {
            if (DODEBUG)
                Debug.Log("KEYS RECOVERED");
            SetAll(PlayerPrefs.GetInt("fullscreen"), PlayerPrefs.GetInt("resolution"), PlayerPrefs.GetInt("quality"),
                   PlayerPrefs.GetInt("bloom"), PlayerPrefs.GetInt("vignette"), PlayerPrefs.GetInt("film_grain"), PlayerPrefs.GetInt("chrom_abb"),
                   PlayerPrefs.GetFloat("sensitivity"), PlayerPrefs.GetInt("music_mute"), PlayerPrefs.GetFloat("music_lvl"),
                   PlayerPrefs.GetInt("sfx_mute"), PlayerPrefs.GetFloat("sfx_lvl"));
        }
    }

    void Start() {
        SetMusicMute(IntToBool(PlayerPrefs.GetInt("music_mute")));
        SetSFXMute(IntToBool(PlayerPrefs.GetInt("sfx_mute")));
    }

    // Update is called once per frame
    void Update(){
    }

    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", BoolToInt(isFullscreen));
        if (DODEBUG)
            Debug.Log("FULLSCREEN PRESS: " + isFullscreen);
    }

    public void SetResolution(int resIdx) {
        Resolution newRes = resolutions[resIdx];
        Screen.SetResolution(newRes.width, newRes.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", resIdx);
        if (DODEBUG)
            Debug.Log("RESOLUTION PRESS: " + newRes);
    }

    public void SetQuality() {
        QualitySettings.SetQualityLevel(qualityOptions.optionIndex);
        PlayerPrefs.SetInt("quality", qualityOptions.optionIndex);
        if (DODEBUG)
            Debug.Log("QUALITY PRESS: " + QualitySettings.GetQualityLevel());
    }

    public void SetBloom(bool doBloom) {
        _ppc.SetBloom(doBloom);
        PlayerPrefs.SetInt("bloom", BoolToInt(doBloom));
        if (DODEBUG)
            Debug.Log("BLOOM PRESS: " + doBloom);
    }

    public void SetVignette(bool doVign) {
        _ppc.SetVignette(doVign);
        PlayerPrefs.SetInt("vignette", BoolToInt(doVign));
        if (DODEBUG)
            Debug.Log("VIGNETTE PRESS: " + doVign);
    }

    public void SetFilmGrain(bool doGrain) {
        _ppc.SetFilmGrain(doGrain);
        PlayerPrefs.SetInt("film_grain", BoolToInt(doGrain));
        if (DODEBUG)
            Debug.Log("FILM GRAIN PRESS: " + doGrain);
    }

    public void SetChromAbb(bool doChrAbb) {
        _ppc.SetChromAbb(doChrAbb);
        PlayerPrefs.SetInt("chrom_abb", BoolToInt(doChrAbb));
        if (DODEBUG)
            Debug.Log("CHROM ABB PRESS: " + doChrAbb);
    }

    public void SetSensitivity(float sensLevel) {
        _pm.mouseSensitivity = sensLevel;
        PlayerPrefs.SetFloat("sensitivity", sensLevel);
        if (DODEBUG)
            Debug.Log("SENS CHANGE: " + sensLevel);
    }

    private float VolumeConvert01(float volume) {
        if(volume > 0) {
            return Mathf.Log10(volume) * 20f;
        }
        return -80f;
    }

    public void SetMusicVol(float musicLevel) {
        if(!musicMuteToggle.isOn)
            parentMixer.SetFloat("Music_Vol", VolumeConvert01(musicLevel));
        PlayerPrefs.SetFloat("music_lvl", musicLevel);
        if (DODEBUG)
            Debug.Log("MUSIC CHANGE: " + musicLevel);
    }

    public void SetSFXVol(float sfxLevel) {
        if(!sfxMuteToggle.isOn)
            parentMixer.SetFloat("SFX_Vol", VolumeConvert01(sfxLevel));
        PlayerPrefs.SetFloat("sfx_lvl", sfxLevel);
        if (DODEBUG)
            Debug.Log("SFX CHANGE: " + sfxLevel);
    }

    public void SetMusicMute(bool doMute) {
        if (doMute)
            parentMixer.SetFloat("Music_Vol", -80f); 
        else
            SetMusicVol(musicSlider.value);
        PlayerPrefs.SetInt("music_mute", BoolToInt(doMute));
        if (DODEBUG)
            Debug.Log("MUSIC MUTE: " + doMute);
    }

    public void SetSFXMute(bool doMute) {
        if (doMute)
            parentMixer.SetFloat("SFX_Vol", -80f);
        else
            SetSFXVol(sfxSlider.value);
        PlayerPrefs.SetInt("sfx_mute", BoolToInt(doMute));
        if (DODEBUG)
            Debug.Log("SFX MUTE: " + doMute);
    }

    public void SetDefaults() {
        SetAll(1, -1, 2, 1, 1, 1, 1, 4.5f, 0, 0.5f, 0, 0.5f);

        SetAll(PlayerPrefs.GetInt("fullscreen"), PlayerPrefs.GetInt("resolution"), PlayerPrefs.GetInt("quality"),
                   PlayerPrefs.GetInt("bloom"), PlayerPrefs.GetInt("vignette"), PlayerPrefs.GetInt("film_grain"), PlayerPrefs.GetInt("chrom_abb"),
                   PlayerPrefs.GetFloat("sensitivity"), PlayerPrefs.GetInt("music_mute"), PlayerPrefs.GetFloat("music_lvl"),
                   PlayerPrefs.GetInt("sfx_mute"), PlayerPrefs.GetFloat("sfx_lvl"));
    }

    public void SetAll(int fullscreen, int resIdx, int quality, int bloom, int vignette, int filmGrain, int chromAbb,
                       float sensitivity, int musicMute, float musicLvl, int sfxMute, float sfxLvl) {
        SetFullscreen(IntToBool(fullscreen));
        fullscreenToggle.isOn = IntToBool(fullscreen);

        if (resIdx == -1) { //default, therefore 'current res'
            resIdx = 0;
            int idx = 0;
            foreach (var res in resolutions) {
                if ((res.height == Screen.currentResolution.height) && (res.width == Screen.currentResolution.width)) {
                    resIdx = idx;
                    break;
                }
                idx++;
            }
        }
        SetResolution(resIdx);
        resolutionDropdown.value = resIdx;
        resolutionDropdown.RefreshShownValue();

        qualityOptions.SetOption(quality);
        SetQuality();

        SetBloom(IntToBool(bloom));
        bloomToggle.isOn = IntToBool(bloom);

        SetVignette(IntToBool(vignette));
        vignetteToggle.isOn = IntToBool(vignette);

        SetFilmGrain(IntToBool(filmGrain));
        filmGToggle.isOn = IntToBool(filmGrain);

        SetChromAbb(IntToBool(chromAbb));
        chromAbbToggle.isOn = IntToBool(chromAbb);

        SetSensitivity(sensitivity);
        mouseSensSlider.value = sensitivity;

        //StartCoroutine(WaitForSounds(musicLvl, !IntToBool(musicMute), sfxLvl, !IntToBool(sfxMute)));
        
        musicSlider.value = musicLvl;
        SetMusicMute(IntToBool(musicMute));
        musicMuteToggle.isOn = IntToBool(musicMute);
        
        sfxSlider.value = sfxLvl;
        SetSFXMute(IntToBool(sfxMute));
        sfxMuteToggle.isOn = IntToBool(sfxMute);
        
    } 
    /*
    IEnumerator WaitForSounds(float musicLvl, bool musicMute, float sfxLvl, bool sfxMute) {
        Debug.Log("music mute: " + musicMute);
        musicSlider.value = musicLvl;
        sfxSlider.value = sfxLvl;
        yield return new WaitForEndOfFrame();
        SetMusicMute(musicMute);
        musicMuteToggle.isOn = musicMute;
        SetSFXMute(sfxMute);
        sfxMuteToggle.isOn = sfxMute;
    }
    */
    public void SaveOptions() {
        PlayerPrefs.Save();
    }

    public void ResetHighScores() {
        _lbm.DeleteLeaderboard();
    }
}
