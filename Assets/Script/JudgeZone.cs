using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JudgeZone : MonoBehaviour
{
    [Header("基本設定")]
    public int laneIndex = 0;                 // この JudgeZone のレーン番号 (0～4)
    public KeyCode inputKey = KeyCode.F;      // PC用キー入力
    public float judgeRangeZ = 30.0f;         // 判定範囲

    [Header("参照設定")]
    public JudgeTextController judgeTextController;
    public GameManager gameManager;

    [Header("UI")]
    public TMP_Text scoreCountText;
    public TMP_Text comboCountText;

    private int score = 0;
    private int combo = 0;
    private List<GameObject> notesInZone = new();

    void Update()
    {
        CheckMissedNotes();

        bool inputDetected = false;

        // --- PC用キー入力 ---
        if (Input.GetKeyDown(inputKey))
        {
            inputDetected = true;
        }

        // --- スマホ用タップ入力 ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // タップしたのが自分自身の collisionbox なら判定する
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        inputDetected = true;
                    }
                }
            }
        }

        if (inputDetected)
        {
            JudgeNotes();
        }
    }

    private void CheckMissedNotes()
    {
        for (int i = notesInZone.Count - 1; i >= 0; i--)
        {
            GameObject note = notesInZone[i];
            if (note == null)
            {
                notesInZone.RemoveAt(i);
                continue;
            }

            if (note.transform.position.z < transform.position.z - judgeRangeZ)
            {
                ShowJudge("Bad");
                combo = 0;
                UpdateComboText();
                Destroy(note);
                notesInZone.RemoveAt(i);
            }
        }
    }

    private void JudgeNotes()
    {
        bool hitNote = false;

        for (int i = 0; i < notesInZone.Count; i++)
        {
            GameObject note = notesInZone[i];
            if (note != null)
            {
                float dz = Mathf.Abs(note.transform.position.z - transform.position.z);
                if (dz <= judgeRangeZ)
                {
                    hitNote = true;
                    Destroy(note);
                    notesInZone.RemoveAt(i);
                    break;
                }
            }
        }

        if (hitNote)
        {
            score += 100;
            combo++;
            UpdateScoreText();
            UpdateComboText();

            if (gameManager != null) gameManager.AddScore(100);
            ShowJudge("Good");
        }
        else
        {
            combo = 0;
            UpdateComboText();

            if (gameManager != null) gameManager.ResetCombo();
            ShowJudge("Bad");
        }
    }

    private void ShowJudge(string result)
    {
        if (judgeTextController != null)
        {
            judgeTextController.ShowJudge(result);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] JudgeTextController が未設定です");
        }
    }

    private void UpdateScoreText()
    {
        if (scoreCountText != null)
        {
            scoreCountText.text = $"{score}";
        }
    }

    private void UpdateComboText()
    {
        if (comboCountText != null)
        {
            comboCountText.text = combo > 0 ? $" {combo}" : "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            notesInZone.Add(other.gameObject);
            Debug.Log($"[{gameObject.name}] Note 入場: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            notesInZone.Remove(other.gameObject);
            Debug.Log($"[{gameObject.name}] Note 退出: {other.name}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(transform.localScale.x, transform.localScale.y, judgeRangeZ * 2));
    }
}
