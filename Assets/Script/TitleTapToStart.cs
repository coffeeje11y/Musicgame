using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleTapToStart : MonoBehaviour
{
    // タップまたはクリックで呼ばれる
    void Update()
    {
        // スマホ・タブレット用（1本指タップ）
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene("SubjectInputScene");
        }

        // PC用（マウスクリック）
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("SubjectInputScene");
        }
    }
}
