using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace Serialize
{

    /// <summary>
    /// テーブルの管理クラス
    /// </summary>
    [System.Serializable]
    public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
    {
        [SerializeField]
        private List<Type> list;
        private Dictionary<TKey, TValue> table;


        public Dictionary<TKey, TValue> GetTable()
        {
            if (table == null)
            {
                table = ConvertListToDictionary(list);
            }
            return table;
        }

        /// <summary>
        /// Editor Only
        /// </summary>
        public List<Type> GetList()
        {
            return list;
        }

        static Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            foreach (KeyAndValue<TKey, TValue> pair in list)
            {
                dic.Add(pair.Name, pair.AudioSource);
            }
            return dic;
        }
    }

    /// <summary>
    /// シリアル化できる、KeyValuePair
    /// </summary>
    [System.Serializable]
    public class KeyAndValue<TKey, TValue>
    {
        public TKey Name;
        public TValue AudioSource;

        public KeyAndValue(TKey key, TValue value)
        {
            Name = key;
            AudioSource = value;
        }
        public KeyAndValue(KeyValuePair<TKey, TValue> pair)
        {
            Name = pair.Key;
            AudioSource = pair.Value;
        }


    }
}

[System.Serializable]
public class SampleTable : Serialize.TableBase<string, AudioClip, SamplePair>
{


}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class SamplePair : Serialize.KeyAndValue<string, AudioClip>
{

    public SamplePair(string key, AudioClip value) : base(key, value)
    {

    }
}

public class SoundManager : MonoBehaviour
{
    //Inspectorに表示できるデータテーブル
    public SampleTable soundList;
    private Dictionary<string, AudioClip> soundDictionary;

    void Awake()
    {
        //内容を列挙
        foreach (KeyValuePair<string, AudioClip> pair in soundList.GetTable())
        {
            Debug.Log("Name : " + pair.Key + "  Source : " + pair.Value);
        }
    }

    private void Start()
    {
        soundDictionary = soundList.GetTable();
    }

    public AudioClip GetAudioClip(string name)
    {
        return soundDictionary[name];
    }


}