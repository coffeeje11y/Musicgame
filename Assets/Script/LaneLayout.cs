using UnityEngine;

public class LaneLayout : MonoBehaviour
{
    public Camera mainCamera;
    public Transform[] lanes; // 左から順に

    // 判定ラインのZ位置
    public float judgeZ = 5f;

    void Start()
    {
        int laneCount = lanes.Length;

        // 画面左右端（Viewport）
        Vector3 left =
            mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, judgeZ));
        Vector3 right =
            mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, judgeZ));

        float totalWidth = right.x - left.x;
        float laneWidth = totalWidth / laneCount;

        for (int i = 0; i < laneCount; i++)
        {
            float x = left.x + laneWidth * (i + 0.5f);

            lanes[i].position = new Vector3(
                x,
                lanes[i].position.y,
                lanes[i].position.z
            );

            // 見た目の幅を合わせる
            lanes[i].localScale = new Vector3(laneWidth, 1f, 1f);
        }
    }
}
