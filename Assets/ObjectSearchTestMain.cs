using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSearchTestMain : MonoBehaviour
{
    [Range(1, 512)]
    public int GenerateNum;
    [Range(1, 256)]
    public int SearchLoopNum;
    public GameObject Prefab;
    public Transform CharacterParent;
    List<Character> CharacterList = new List<Character>();
    List<Character> CharacterCandidateList;
    IEnumerable<Character> CharacterCandidateEnum;
    StringBuilder sb = new StringBuilder();

    long GCTotalMemory;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < GenerateNum; i++)
        {
            var chara = Instantiate(Prefab).GetComponent<Character>();
            chara.transform.SetParent(CharacterParent, true);
            chara.groupColor = i < GenerateNum / 2 ? Character.GroupColor.Red : Character.GroupColor.Blue;
            chara.transform.position = new Vector3(
                Random.Range(-250, 250),
                0,
                Random.Range(-250, 250)
            );
            chara.gameObject.name = "Character:" + i.ToString();
            CharacterList.Add(chara);
        }
    }
    void Preprocess(out long totalMemory)
    {
        sb.Length = 0;
        CharacterCandidateList = null;
        CharacterCandidateEnum = null;
        System.GC.Collect();
        StopWatchUtility.instance.Reset();
        StopWatchUtility.instance.Start();
        totalMemory = System.GC.GetTotalMemory(false);
    }
    void Endprocess(out long totalMemory)
    {
        totalMemory = System.GC.GetTotalMemory(false);
        StopWatchUtility.instance.Stop();
    }
    void Result()
    {
        if (CharacterCandidateList != null)
        {
            Debug.LogFormat("Result:{0}", CharacterCandidateList.Count);
            foreach (var character in CharacterCandidateList)
            {
                sb.AppendLine(character.name);
            }
            Debug.Log(sb.ToString());
        }
        if (CharacterCandidateEnum != null)
        {
            Debug.LogFormat("Result:{0}", CharacterCandidateEnum.Count());
            foreach (var character in CharacterCandidateEnum)
            {
                sb.AppendLine(character.name);
            }
            Debug.Log(sb.ToString());
        }
    }
    /// <summary>
    /// Linqを用いた要素の抽出
    /// </summary>
    public void OnLinqSearch()
    {
        long before = 0;
        Preprocess(out before);
        for (int search = 0; search < SearchLoopNum; search++)
        {
            CharacterCandidateEnum = null;
            CharacterCandidateEnum = (from x in CharacterList
                                      where x.groupColor.Equals(Character.GroupColor.Red)
                                      select x);
        }

        Result();
        Endprocess(out GCTotalMemory);

        Debug.LogFormat("OnLinqSearch Time:{0}msec GC:{1}byte", StopWatchUtility.instance.ElapsedMilliseconds, GCTotalMemory - before);
    }
    /// <summary>
    /// for文を用いた要素の抽出
    /// </summary>
    public void OnForLoopSearch()
    {
        long before = 0;
        Preprocess(out before);

        for (int search = 0; search < SearchLoopNum; search++)
        {
            CharacterCandidateList = null;
            CharacterCandidateList = new List<Character>();
            for (int i = 0; i < CharacterList.Count; i++)
            {
                var x = CharacterList[i];
                if (x.groupColor.Equals(Character.GroupColor.Red))
                {
                    CharacterCandidateList.Add(x);
                }
            }
        }

        Result();
        Endprocess(out GCTotalMemory);

        Debug.LogFormat("OnForLoopSearch Time:{0}msec GC:{1}byte", StopWatchUtility.instance.ElapsedMilliseconds, GCTotalMemory - before);
    }
    /// <summary>
    /// foreach文を用いた要素の抽出
    /// </summary>
    public void OnForeachSearch()
    {
        long before = 0;
        Preprocess(out before);

        for (int search = 0; search < SearchLoopNum; search++)
        {
            CharacterCandidateList = null;
            CharacterCandidateList = new List<Character>();
            foreach (var x in CharacterList)
            {
                if (x.groupColor.Equals(Character.GroupColor.Red))
                {
                    CharacterCandidateList.Add(x);
                }
            }
        }

        Result();
        Endprocess(out GCTotalMemory);

        Debug.LogFormat("OnForeachSearch Time:{0}msec GC:{1}byte", StopWatchUtility.instance.ElapsedMilliseconds, GCTotalMemory - before);
    }
}

public static class StopWatchUtility
{
    public static System.Diagnostics.Stopwatch instance = new System.Diagnostics.Stopwatch();
}