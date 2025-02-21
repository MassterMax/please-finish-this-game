using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFxObjectPrefab;  // because we want to spawn prefabs with audio
    [SerializeField] AudioSource bgMusic;

    public static SoundFXManager Instance { get; private set; }

    private float effectsVolume = 0.2f;

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

    public void SetBGMusicVolume(float volume)
    {
        bgMusic.volume = volume;
    }

    public void SetEffectsVolume(float volume)
    {
        effectsVolume = volume;
    }

    public void StopBGMusic() {
        bgMusic.Pause();
    }

    public void ResumeBGMusic() {
        bgMusic.UnPause();
    }
}
