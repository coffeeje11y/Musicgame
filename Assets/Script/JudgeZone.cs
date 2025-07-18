using System.Collections.Generic;
using UnityEngine;
using TMPro;  // ← 追加

public class JudgeZone : MonoBehaviour
{
    public KeyCode inputKey = KeyCode.F;
    public JudgeTextController judgeTextController;
    public GameManager gameManager;  // GameManager を参照するための変数

    // スコア・コンボ用
    public TMP_Text scoreCountText;
    public TMP_Text comboCountText;
    private int score = 0;
    private int combo = 0;

    private List<GameObject> notesInZone = new();
    public float judgeRangeZ = 30.0f;

    void Update()
    {
        // ノーツがあるのに押されなかった場合のチェック
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

        // 入力処理
        if (Input.GetKeyDown(inputKey))
        {
            bool hitNote = false;

            for (int i = 0; i < notesInZone.Count; i++)
            {
                GameObject note = notesInZone[i];
                if (note != null)
                {
                    float dz = Mathf.Abs(note.transform.position.z - transform.position.z);
                    Debug.Log($"{gameObject.name} 判定差 = {dz}");
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
                if (gameManager != null)
                {
                    gameManager.AddScore(100);
                }
                ShowJudge("Good");
            }
            else
            {
                if (gameManager != null)
                {
                    gameManager.ResetCombo();
                }
                ShowJudge("Bad");
            }
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
            Debug.LogWarning("JudgeTextController がアタッチされていません！");
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
            Debug.Log($"{gameObject.name}: Note 入った {other.name}");
            notesInZone.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            notesInZone.Remove(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, judgeRangeZ * 2));
    }
}
