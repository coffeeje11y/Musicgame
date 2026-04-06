using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SubjectInputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField participantIdInput;

    public void StartGame()
    {
        string id = participantIdInput.text.Trim();

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("被験者番号が入力されていません。");
            return;
        }

        // 保存（次のシーンでも利用できる）
        PlayerPrefs.SetString("ParticipantID", id);
        PlayerPrefs.Save();

        // 選曲画面へ遷移
        SceneManager.LoadScene("ChooseMusic");
    }
}
