using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using static BB_Utils;

public class PostProcessingController : MonoBehaviour
{
    Volume _vol;

    Bloom bloom;

    Vignette vignette;

    FilmGrain filmGrain;

    ChromaticAberration chromAbb;
    [SerializeField]
    float defaultChromAbb = 0.25f;
    [SerializeField]
    float heightenedChromAbb = 0.99f;

    DepthOfField depthOfField;

    SceneHandler _sh;

    // Start is called before the first frame update
    void Awake()
    {
        _vol = gameObject.GetComponent<Volume>();
        _sh = GameObject.Find("/SceneHandler").GetComponent<SceneHandler>();

        if (_vol.profile.TryGet(out Bloom b_tmp))
            bloom = b_tmp;
        if (_vol.profile.TryGet(out Vignette v_tmp))
            vignette = v_tmp;
        if (_vol.profile.TryGet(out FilmGrain fg_tmp))
            filmGrain = fg_tmp;
        if (_vol.profile.TryGet(out ChromaticAberration ca_tmp))
            chromAbb = ca_tmp;
        if (_vol.profile.TryGet(out DepthOfField dof_tmp))
            depthOfField = dof_tmp;
    }

    // Update is called once per frame
    void Update()
    {
        if (_sh.currentState == GameState.Paused)
            depthOfField.active = true;
        else
            depthOfField.active = false;

        if (_sh.currentState == GameState.Complete || _sh.currentState == GameState.Dead) {
            chromAbb.intensity.Override(heightenedChromAbb);
        }
        else if(_sh.currentState == GameState.Running)
            chromAbb.intensity.Override(defaultChromAbb); 
    }

    public void SetBloom(bool doBloom) {
        bloom.active = doBloom;
    }

    public void SetVignette(bool doVign) {
        vignette.active = doVign;
    }

    public void SetFilmGrain(bool doGrain) {
        filmGrain.active = doGrain;
    }

    public void SetChromAbb(bool doChrAbb) {
        chromAbb.active = doChrAbb;
    }
}
