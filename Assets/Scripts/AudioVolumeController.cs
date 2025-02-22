using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] Image musicImage;
    [SerializeField] Image soundsImage;
    private bool musicActive = true;
    private bool soundsActive = true;

    Color faded = new Color(1, 1, 1, 0.5f);

    public void OnMusicButtion() {
        musicActive = !musicActive;
        SoundFXManager.Instance.SetMusic(musicActive);
        if (musicActive) {
            musicImage.color = Color.white;
        } else {
            musicImage.color = faded;
        }

    }

    public void OnSoundsButtion() {
        soundsActive = !soundsActive;
        SoundFXManager.Instance.SetEffects(soundsActive);
        if (soundsActive) {
            soundsImage.color = Color.white;
        } else {
            soundsImage.color = faded;
        }
    }
}
