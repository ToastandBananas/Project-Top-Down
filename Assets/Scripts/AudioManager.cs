using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {

    public string soundName;
    public AudioClip clip;

    [Range(0f, 2f)] public float volume = 1f;
    [Range(0f, 2f)] public float pitch = 1f;

    [Range(0f, 0.25f)] public float randomVolumeAddOn = 0.25f;
    [Range(0f, 0.25f)] public float randomPitchAddOn = 0.25f;
    
    float maxSoundDist = 20f;
    float percent;
    float distToPlayer;

    public bool loop = false;

    AudioSource source;

    public void SetSource (AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play(Vector3 soundPosition)
    {
        source.volume = volume * (1 + Random.Range(-randomVolumeAddOn, randomVolumeAddOn));
        source.pitch = pitch * (1 + Random.Range(-randomPitchAddOn, randomPitchAddOn));
        
        distToPlayer = Vector2.Distance(soundPosition, PlayerMovement.instance.transform.position);
        if (distToPlayer <= maxSoundDist)
        {
            percent = 1 - (distToPlayer / maxSoundDist);
            source.volume *= percent;

            source.Play();
        }
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
    public List<Sound[]> allSounds = new List<Sound[]>();

    [Header("Ambient Sounds")]
    public Sound[] windSounds;
    
    [Header("Inventory Sounds")]
    public Sound[] defaultPickUpSounds;
    public Sound[] clothingPickUpSounds;
    public Sound[] armorPickUpSounds;
    public Sound[] sharpWeaponPickUpSounds;
    public Sound[] bluntWeaponPickUpSounds;
    public Sound[] ringPickUpSounds;
    public Sound[] goldSounds;
    public Sound[] eatFoodSounds;
    public Sound[] drinkSounds;

    [Header("Human Sounds")]
    public Sound[] humanMaleGruntSounds;
    public Sound[] humanMaleDeathSounds;

    [Header("Footsteps")]
    public Sound[] footstepsStandard;
    public Sound[] footstepsStone;

    [Header("Door Sounds")]
    public Sound[] openDoorSounds;
    public Sound[] closeDoorSounds;

    [Header("Container Sounds")]
    public Sound[] chestSounds;
    public Sound[] searchBodySounds;

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

    PlayerMovement player;

    void Awake()
    {
        #region Singleton
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
        #endregion

        allSounds.Add(windSounds);
        allSounds.Add(defaultPickUpSounds);
        allSounds.Add(clothingPickUpSounds);
        allSounds.Add(armorPickUpSounds);
        allSounds.Add(sharpWeaponPickUpSounds);
        allSounds.Add(bluntWeaponPickUpSounds);
        allSounds.Add(ringPickUpSounds);
        allSounds.Add(goldSounds);
        allSounds.Add(eatFoodSounds);
        allSounds.Add(drinkSounds);
        allSounds.Add(humanMaleGruntSounds);
        allSounds.Add(humanMaleDeathSounds);
        allSounds.Add(footstepsStandard);
        allSounds.Add(footstepsStone);
        allSounds.Add(openDoorSounds);
        allSounds.Add(closeDoorSounds);
        allSounds.Add(chestSounds);
        allSounds.Add(searchBodySounds);
        allSounds.Add(arrowHitFleshSounds);
        allSounds.Add(arrowHitArmorSounds);
        allSounds.Add(arrowHitWallSounds);
        allSounds.Add(bowDrawSounds);
        allSounds.Add(bowReleaseSounds);
        allSounds.Add(swordSlashSounds);
        allSounds.Add(swordStabSounds);
        allSounds.Add(swordSlashFleshSounds);
        allSounds.Add(swordSlashArmorSounds);
        allSounds.Add(swordStabFleshSounds);
        allSounds.Add(swordStabArmorSounds);
        allSounds.Add(bluntHitFleshSounds);
        allSounds.Add(bluntHitArmorSounds);
    }

    void Start()
    {
        player = PlayerMovement.instance;

        foreach (Sound[] soundArray in allSounds)
        {
            for (int i = 0; i < soundArray.Length; i++)
            {
                GameObject _go = new GameObject("Sound_" + i + " " + soundArray[i].soundName);
                _go.transform.SetParent(transform);
                soundArray[i].SetSource(_go.AddComponent<AudioSource>());
                _go.GetComponent<AudioSource>().outputAudioMixerGroup = masterAudioMixerGroup;
            }
        }

        PlayAmbienceSound();
    }

    public void PlaySound(Sound[] soundArray, string _soundName, Vector3 soundPosition)
    {
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[i].soundName == _soundName)
            {
                soundArray[i].Play(soundPosition);
                return;
            }
        }

        // No sound with _soundName
        Debug.LogWarning("AudioManager: Sound not found in list: " + _soundName);
    }

    public void PlayRandomSound(Sound[] soundArray, Vector3 soundPosition)
    {
        int randomIndex = Random.Range(0, soundArray.Length);
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[randomIndex] == soundArray[i])
            {
                PlaySound(soundArray, soundArray[i].soundName, soundPosition);
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

    void PlayAmbienceSound()
    {
        int randomIndex = Random.Range(0, windSounds.Length);
        for (int i = 0; i < windSounds.Length; i++)
        {
            if (windSounds[randomIndex] == windSounds[i])
            {
                PlaySound(windSounds, windSounds[i].soundName, player.transform.position);
                Invoke("PlayAmbienceSound", windSounds[i].clip.length);
                return;
            }
        }
    }

    public void PlayPickUpItemSound(Item item)
    {
        bool soundFound = false;

        if (item.itemType == ItemType.Weapon)
        {
            Equipment equipment = (Equipment)item;
            if (equipment.weaponType == WeaponType.Sword)
            {
                PlayRandomSound(sharpWeaponPickUpSounds, player.transform.position);
                soundFound = true;
            }
            else if (equipment.weaponType == WeaponType.Mace || equipment.weaponType == WeaponType.Staff || equipment.weaponType == WeaponType.Spear || equipment.weaponType == WeaponType.Axe)
            {
                PlayRandomSound(bluntWeaponPickUpSounds, player.transform.position);
                soundFound = true;
            }
        }

        if (item.itemType == ItemType.Armor)
        {
            Equipment equipment = (Equipment)item;
            if (equipment.armorType == ArmorType.Shirt || equipment.armorType == ArmorType.Pants || equipment.armorType == ArmorType.Belt)
            {
                PlayRandomSound(clothingPickUpSounds, player.transform.position);
                soundFound = true;
            }
            else
            {
                PlayRandomSound(armorPickUpSounds, player.transform.position);
                soundFound = true;
            }
        }

        if (item.itemType == ItemType.Consumable)
        {
            Consumable consumable = (Consumable)item;
            if (consumable.consumableType == ConsumableType.Drink)
            {
                PlayRandomSound(drinkSounds, player.transform.position);
                soundFound = true;
            }
        }

        if (soundFound == false)
        {
            PlayRandomSound(defaultPickUpSounds, player.transform.position);
            soundFound = true;
        }
    }

    public void PlayPickUpGoldSound(int goldAmount)
    {
        if (goldAmount <= 10)
            PlaySound(goldSounds, goldSounds[0].soundName, player.transform.position);
        else if (goldAmount > 10 && goldAmount <= 50)
        {
            int randomNum = Random.Range(1, 3);
            if (randomNum == 1)
                PlaySound(goldSounds, goldSounds[1].soundName, player.transform.position);
            else
                PlaySound(goldSounds, goldSounds[2].soundName, player.transform.position);
        }
        else if (goldAmount > 50 && goldAmount <= 100)
            PlaySound(goldSounds, goldSounds[3].soundName, player.transform.position);
        else
            PlaySound(goldSounds, goldSounds[4].soundName, player.transform.position);
    }
}
