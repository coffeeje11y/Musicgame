using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

        LoadChart();
        StartCoroutine(SpawnNotesCoroutine());
    }

    void LoadChart()
    {
        string path = Path.Combine(Application.streamingAssetsPath, chartFileName);
        string json = File.ReadAllText(path);

        Chart chart = JsonConvert.DeserializeObject<Chart>(json);
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
        Debug.Log($"[AddNote] num={note.num}, block={note.block}, lpb={note.LPB}, time={(note.num / (float)note.LPB) * (60f / chart.BPM)}");

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
                Instantiate(notePrefab, spawnPoints[block].position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"ブロック番号 {block + 1} が spawnPoints の範囲外です。現在の長さ: {spawnPoints.Length}");
            }
        }
    }
}
