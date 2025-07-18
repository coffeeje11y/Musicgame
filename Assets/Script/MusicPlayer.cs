using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // AudioSourceをアタッチ or 取得
        audioSource = gameObject.AddComponent<AudioSource>();

        // Resourcesからmp3を読み込む（AudioClipに変換）
        AudioClip clip = Resources.Load<AudioClip>("Rogue_Circuit");
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("音声ファイルが見つかりません！Resources/Rogue_Circuit.mp3 を確認してください");
        }
    }
}
