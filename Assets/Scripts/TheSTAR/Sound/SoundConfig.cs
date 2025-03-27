using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSTAR.Sound
{
    [CreateAssetMenu(menuName = "Data/Sound", fileName = "SoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        [SerializeField] private List<SoundData> soundDatas = new List<SoundData>();
        [SerializeField] private List<MusicData> musicDatas = new List<MusicData>();

        [Space]
        [SerializeField] private float timeClear = 1; 
        [SerializeField] private float timeMusicChange = 1;

        public float TimeClear => timeClear;
        public float TimeMusicChange => timeMusicChange;

        public MusicData GetData(MusicType type)
        {
            return musicDatas.Find(info => info.Type == type);
        }

        public SoundData GetData(SoundType type)
        {
            return soundDatas.Find(info => info.Type == type);
        }
        
        [Serializable]
        public class SoundData
        {
            [SerializeField] private AudioClip clip;
            [SerializeField] private SoundType type;
            [SerializeField] [Range(0, 1)] private float volume = 1;
            /// <summary>
            /// Может ли одновременно воспроизводиться больше 1 экземпляра звука
            /// </summary>
            [SerializeField] private bool canMultiply = false;
            [SerializeField] private bool loop = false;
            
            public AudioClip Clip => clip;
            public float Volume => volume;
            public bool CanMultiply => canMultiply;
            public bool Loop => loop;
            public SoundType Type => type;
        }

        [Serializable]
        public class MusicData
        {
            [SerializeField] private AudioClip clip;
            [SerializeField] private MusicType type;
            [SerializeField] [Range(0, 1)] private float volume = 1;
            [SerializeField] private bool loop = true;
            [SerializeField] private string name;

            public AudioClip Clip => clip;
            public float Volume => volume;
            public bool Loop => loop;
            public MusicType Type => type;
            public string Name => name;
        }
    }

    public enum SoundType
    {
        ButtonClick,
        ButtonClickWet,
        StreetSound,
        ClickLaptop,
        Oshibka,
        Oplata_korsini,
        Dostavka,
        Gotovka,
        snd_pickup_item_v1,
        snd_pickup_item_v4,
        Jarim_myaso,
        friturnitsa,
        timer_bell_m1tycbno,
        water_pour_liquid_into_styrofoam_cup_coffee_zy_fi2nu,
        coffee_bean_bag_1,
        level_up,
        Step_0,
        Cash_Register_2,
        BeepSound,
        Bumajka,
        Coins_1,
        Step_1,
        Step_2,
        Step_3,
        WheelTick,
        DailyReward
    }

    public enum MusicType
    {
        Theme_0,
        Theme_1,
        Theme_2,
        Theme_3,
        Theme_4,
        Theme_5,
    }
}