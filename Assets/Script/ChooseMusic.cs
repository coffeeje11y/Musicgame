using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseMusic : MonoBehaviour
{
    // タップまたはクリックで呼ばれる
    void Update()
    {
        // スマホ・タブレット用（1本指タップ）
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene("GameScene");
        }

        // PC用（マウスクリック）
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
