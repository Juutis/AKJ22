using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    public static SoundManager main;

    [SerializeField]
    private AudioSource music;
    private float originalVolume;
    private bool musicMuted = false;
    private bool sfxMuted = false;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
            originalVolume = music.volume;
        }
    }

    private AudioSource loopingSound;

    [SerializeField]
    private List<GameSound> sounds;
    public void PlaySound(GameSoundType soundType)
    {
        if (sfxMuted)
        {
            return;
        }
        if (soundType == GameSoundType.None)
        {
            return;
        }
        GameSound gameSound = sounds.Where(sound => sound.Type == soundType).FirstOrDefault();
        if (gameSound != null)
        {
            AudioSource audio = gameSound.Get();
            if (audio != null)
            {
                //audio.pitch = 1f;
                if (gameSound.RandomizePitch)
                {
                    audio.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
                }
                audio.Play();
            }
        }
    }

    public void PlayLoop(GameSoundType soundType)
    {
        if (sfxMuted)
        {
            return;
        }
        if (loopingSound)
        {
            loopingSound.Play();
            return;
        }
        if (soundType == GameSoundType.None)
        {
            return;
        }
        GameSound gameSound = sounds.Where(sound => sound.Type == soundType).FirstOrDefault();
        if (gameSound != null)
        {
            AudioSource audio = gameSound.Get();
            if (audio != null)
            {
                if (!audio.isPlaying)
                {
                    audio.pitch = 1f;
                    audio.loop = true;
                    audio.Play();
                }
                loopingSound = audio;
            }
        }
    }
    public void StopLoop(GameSoundType soundType)
    {
        if (loopingSound != null)
        {
            loopingSound.Pause();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            sfxMuted = !sfxMuted;
        }
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            musicMuted = !musicMuted;
            music.volume = musicMuted ? 0f : originalVolume;
        }
    }

}


public enum GameSoundType
{
    None,
    KillGobboSmall,
    KillGobboMedium,
    KillGobboBig,
    ShootProjectile,
    ChainLightningLoop,
    Select,
    PickupXp,
    GobboHello,
    EnemyHit,
    WeaponEffectDeep,
    Fireball,
    LevelUp,
    Hurt

}


[System.Serializable]
public class GameSound
{
    [field: SerializeField]
    public GameSoundType Type { get; private set; }

    [field: SerializeField]
    private List<AudioSource> sounds;

    [SerializeField]
    private bool randomizePitch;
    public bool RandomizePitch { get { return randomizePitch; } }

    private List<GameSoundPool> soundPools = new List<GameSoundPool>();
    private bool initialized = false;


    public AudioSource Get()
    {
        if (!initialized)
        {
            initialize();
        }

        if (sounds == null || sounds.Count == 0)
        {
            return null;
        }
        return soundPools[UnityEngine.Random.Range(0, soundPools.Count)].getAvailable();
    }

    private void initialize()
    {
        soundPools = sounds.Select(it => new GameSoundPool(it)).ToList();
        initialized = true;
    }


    private class GameSoundPool
    {
        private AudioSource originalAudioSource;
        private List<AudioSource> audioSources = new List<AudioSource>();

        public GameSoundPool(AudioSource audioSource)
        {
            originalAudioSource = audioSource;
            addNewToPool();
        }

        public AudioSource getAvailable()
        {
            var src = audioSources.Where(it => it.isPlaying == false).FirstOrDefault();
            if (src == null)
            {
                src = addNewToPool();
            }
            return src;
        }

        private AudioSource addNewToPool()
        {
            if (originalAudioSource == null)
            {

            }
            AudioSource newSource = GameObject.Instantiate(originalAudioSource, originalAudioSource.transform.parent);
            audioSources.Add(newSource);
            return newSource;
        }
    }
}