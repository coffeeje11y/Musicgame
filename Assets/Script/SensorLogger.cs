using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SensorLogger : MonoBehaviour
{
    private List<string> logData = new List<string>();
    private float interval = 1f / 60f; // 60Hz
    private float timer = 0f;
    private string filePath;
    public bool isLogging = false;

    public void StartLogging(string songName)
    {
        // センサーを有効化
        Input.gyro.enabled = true;
        Input.compass.enabled = true;

        // ファイル名（曲名 + 日付時刻）
        string filename = songName + "_sensor_" +
            System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        filePath = Path.Combine(Application.persistentDataPath, filename);

        logData.Clear();
        logData.Add("time,acc_x,acc_y,acc_z,gyro_x,gyro_y,gyro_z,mag_x,mag_y,mag_z");

        isLogging = true;
        Debug.Log("Sensor logging started: " + filePath);
    }

    void Update()
    {
        if (!isLogging) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;

            Vector3 acc = Input.acceleration;
            Vector3 gyro = Input.gyro.rotationRateUnbiased;
            Vector3 mag = Input.compass.rawVector;

            string line = string.Format(
                "{0:F3},{1:F4},{2:F4},{3:F4},{4:F4},{5:F4},{6:F4},{7:F4},{8:F4},{9:F4}",
                Time.time,
                acc.x, acc.y, acc.z,
                gyro.x, gyro.y, gyro.z,
                mag.x, mag.y, mag.z
            );

            logData.Add(line);
        }
    }

    public void StopLogging()
    {
        if (!isLogging) return;
        isLogging = false;

        File.WriteAllLines(filePath, logData.ToArray());
        Debug.Log("Sensor log saved: " + filePath);
    }

    void Start()
    {
        // ゲーム開始時にログ開始
        StartLogging("TestRun");
    }
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopLogging();
        }
    }

}
