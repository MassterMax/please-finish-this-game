using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFxObjectPrefab;  // because we want to spawn prefabs with audio
    [SerializeField] AudioSource bgMusic;

    public static SoundFXManager Instance { get; private set; }

    private const float DEFAULT_EFFECTS_VOLUME = 0.3f;
    private const float DEFAULT_MUSIC_VOLUME = 0.2f;
    private float effectsVolume;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetMusic(true);
        SetEffects(true);
    }

    // if just want to play clip
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform)
    {
        PlaySoundFXClip(audioClip, spawnTransform, audioClip.length);

    }

    // if want to play clip until newClipLength
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float newClipLength)
    {
        AudioSource audioSource = Instantiate(soundFxObjectPrefab, spawnTransform.position, Quaternion.identity);
        //assign the audioClip
        audioSource.clip = audioClip;
        //assign volume
        audioSource.volume = effectsVolume;
        //play sound
        audioSource.Play();
        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, newClipLength);

    }

    public void SetMusic(bool state) {
        if (state) {
            bgMusic.volume = DEFAULT_MUSIC_VOLUME;
        } else {
            bgMusic.volume = 0;
        }
    }

    public void SetEffects(bool state) {
        if (state) {
            effectsVolume = DEFAULT_EFFECTS_VOLUME;
        } else {
            effectsVolume = 0;
        }
    }

    public float GetEffectsVolume()
    {
        return effectsVolume;
    }

    public void StopBGMusic() {
        bgMusic.Pause();
    }

    public void ResumeBGMusic() {
        bgMusic.UnPause();
    }
}
