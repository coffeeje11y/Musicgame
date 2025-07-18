using TMPro;
using UnityEngine;

public class JudgeTextController : MonoBehaviour
{
    // 表示対象の TextMeshProUGUI をアサインする
    public TextMeshProUGUI judgeText;

    // 判定結果を表示する
    public void ShowJudge(string result)
    {
        if (judgeText == null)
        {
            Debug.LogWarning("judgeText が設定されていません！");
            return;
        }

        judgeText.text = result;
        judgeText.gameObject.SetActive(true);

        // 0.5秒後に非表示にする
        CancelInvoke();
        Invoke(nameof(HideJudge), 0.5f);
    }

    // テキストを非表示にする
    private void HideJudge()
    {
        judgeText.gameObject.SetActive(false);
    }
}
