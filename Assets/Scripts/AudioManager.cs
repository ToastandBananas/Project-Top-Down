using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {

    public string soundName;
    public AudioClip clip;

    [Range(0f, 1.5f)] public float volume = 1f;
    [Range(0f, 1.5f)] public float pitch = 1f;

    [Range(0f, 0.25f)] public float randomVolumeAddOn = 0.25f;
    [Range(0f, 0.25f)] public float randomPitchAddOn = 0.25f;

    public bool loop = false;

    private AudioSource source;

    public void SetSource (AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolumeAddOn, randomVolumeAddOn));
        source.pitch = pitch * (1 + Random.Range(-randomPitchAddOn, randomPitchAddOn));

        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

}

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [Header("Master Audio Mixer:")]
    public AudioMixerGroup masterAudioMixerGroup;

    [Header("All Sounds")]
    public List<Sound[]> sounds = new List<Sound[]>();
    
    [Header("Inventory Sounds")]
    public Sound[] inventorySounds;
    public Sound[] goldSounds;
    public Sound[] eatFoodSounds;
    public Sound[] drinkSounds;

    [Header("Footsteps")]
    public Sound[] footstepsStandard;
    public Sound[] footstepsStone;

    [Header("Bow and Arrow Sounds")]
    public Sound[] arrowHitFleshSounds;
    public Sound[] arrowHitArmorSounds;
    public Sound[] arrowHitWallSounds;
    public Sound[] bowDrawSounds;
    public Sound[] bowReleaseSounds;
    
    [Header("Sword Sounds")]
    public Sound[] swordSlashSounds;
    public Sound[] swordStabSounds;
    public Sound[] swordSlashFleshSounds;
    public Sound[] swordSlashArmorSounds;
    public Sound[] swordStabFleshSounds;
    public Sound[] swordStabArmorSounds;

    [Header("Blunt Weapon Sounds")]
    public Sound[] bluntHitFleshSounds;
    public Sound[] bluntHitArmorSounds;

    void Awake()
    {
        sounds.Add(inventorySounds);
        sounds.Add(goldSounds);
        sounds.Add(eatFoodSounds);
        sounds.Add(drinkSounds);
        sounds.Add(footstepsStandard);
        sounds.Add(footstepsStone);
        sounds.Add(arrowHitFleshSounds);
        sounds.Add(arrowHitArmorSounds);
        sounds.Add(arrowHitWallSounds);
        sounds.Add(bowDrawSounds);
        sounds.Add(bowReleaseSounds);
        sounds.Add(swordSlashSounds);
        sounds.Add(swordStabSounds);
        sounds.Add(swordSlashFleshSounds);
        sounds.Add(swordSlashArmorSounds);
        sounds.Add(swordStabFleshSounds);
        sounds.Add(swordStabArmorSounds);
        sounds.Add(bluntHitFleshSounds);
        sounds.Add(bluntHitArmorSounds);

        if (instance != null)
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        foreach (Sound[] soundArray in sounds)
        {
            for (int i = 0; i < soundArray.Length; i++)
            {
                GameObject _go = new GameObject("Sound_" + i + " " + soundArray[i].soundName);
                _go.transform.SetParent(transform);
                soundArray[i].SetSource(_go.AddComponent<AudioSource>());
                _go.GetComponent<AudioSource>().outputAudioMixerGroup = masterAudioMixerGroup;
            }
        }

        // PlaySound("Music");
    }

    public void PlaySound(Sound[] soundArray, string _soundName)
    {
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[i].soundName == _soundName)
            {
                soundArray[i].Play();
                return;
            }
        }

        // No sound with _soundName
        Debug.LogWarning("AudioManager: Sound not found in list: " + _soundName);
    }

    public void PlayRandomSound(Sound[] soundArray)
    {
        int randomIndex = Random.Range(0, soundArray.Length);
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[randomIndex] == soundArray[i])
            {
                PlaySound(soundArray, soundArray[i].soundName);
                return;
            }
        }
    }

    public void StopSound(Sound[] soundArray, string _soundName)
    {
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[i].soundName == _soundName)
            {
                soundArray[i].Stop();
                return;
            }
        }

        // No sound with _soundName
        Debug.LogWarning("AudioManager: Sound not found in list: " + _soundName);
    }

    public void PlayPickUpGoldSound(int goldAmount)
    {
        if (goldAmount <= 10)
            PlaySound(goldSounds, goldSounds[0].soundName);
        else if (goldAmount > 10 && goldAmount <= 50)
        {
            int randomNum = Random.Range(1, 3);
            if (randomNum == 1)
                PlaySound(goldSounds, goldSounds[1].soundName);
            else
                PlaySound(goldSounds, goldSounds[2].soundName);
        }
        else if (goldAmount > 50 && goldAmount <= 100)
            PlaySound(goldSounds, goldSounds[3].soundName);
        else
            PlaySound(goldSounds, goldSounds[4].soundName);
    }
}
