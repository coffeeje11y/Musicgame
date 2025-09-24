using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public string chartFileName = "Rogue_Circuit.json";

    private List<(float time, int block)> scheduledNotes = new();

    void Start()
    {
        Debug.Log($"[NoteSpawner] 実行中: {gameObject.name}");
        Debug.Log($"[NoteSpawner] notePrefab: {(notePrefab != null ? notePrefab.name : "null")}");
        Debug.Log($"[NoteSpawner] spawnPoints.Length: {spawnPoints.Length}");

        StartCoroutine(LoadChartAndStart());
    }

    IEnumerator LoadChartAndStart()
    {
        yield return StartCoroutine(LoadChart());
        if (scheduledNotes.Count > 0)
        {
            StartCoroutine(SpawnNotesCoroutine());
        }
        else
        {
            Debug.LogError("[NoteSpawner] ノーツデータが読み込まれませんでした！");
        }
    }

    IEnumerator LoadChart()
    {
        string path = Path.Combine(Application.streamingAssetsPath, chartFileName);
        string json = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log($"[NoteSpawner] Android で JSON 読み込み: {path}");
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[NoteSpawner] JSON 読み込み失敗: {www.error}");
            yield break;
        }
        json = www.downloadHandler.text;
#else
        // PC / Editor
        if (!File.Exists(path))
        {
            Debug.LogError($"[NoteSpawner] ファイルが見つかりません: {path}");
            yield break;
        }
        json = File.ReadAllText(path);
        yield return null;
#endif
        if (!string.IsNullOrEmpty(json) && json[0] == '\uFEFF')
        {
            Debug.LogWarning("[NoteSpawner] JSON 先頭にBOMがあったので削除しました");
            json = json.Substring(1);
        }
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("[NoteSpawner] JSON が空です");
            yield break;
        }

        Chart chart = JsonConvert.DeserializeObject<Chart>(json);
        if (chart == null || chart.notes == null)
        {
            Debug.LogError("[NoteSpawner] Chart データが不正です");
            yield break;
        }

        scheduledNotes.Clear();
        foreach (var note in chart.notes)
        {
            AddNoteRecursive(note, chart);
        }

        scheduledNotes.Sort((a, b) => a.time.CompareTo(b.time));
        Debug.Log($"[NoteSpawner] scheduledNotes count = {scheduledNotes.Count}");
    }

    // ネストされたノートも含めて再帰的にスケジューリング
    void AddNoteRecursive(NoteData note, Chart chart)
    {
        float sec = (note.num / (float)note.LPB) * (60f / chart.BPM);
        scheduledNotes.Add((sec + chart.offset, note.block - 1));

        if (note.notes != null && note.notes.Count > 0)
        {
            foreach (var child in note.notes)
            {
                AddNoteRecursive(child, chart);
            }
        }
    }

    IEnumerator SpawnNotesCoroutine()
    {
        float startTime = Time.time;

        foreach (var (spawnTime, block) in scheduledNotes)
        {
            float delay = spawnTime - (Time.time - startTime);
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            if (block >= 0 && block < spawnPoints.Length)
            {
                // ノーツ生成（親を設定して位置ずれを防ぐ）
                GameObject note = Instantiate(notePrefab, spawnPoints[block].position, Quaternion.identity);
                note.transform.SetParent(spawnPoints[block]);
                Debug.Log($"[NoteSpawner] ノート生成: time={spawnTime:F2}, block={block}");
            }
            else
            {
                Debug.LogWarning($"ブロック番号 {block + 1} が spawnPoints の範囲外です。現在の長さ: {spawnPoints.Length}");
            }
        }
    }
}
