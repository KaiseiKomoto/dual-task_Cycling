using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;
    public GameObject objectToMove;
    private float cadence = 0.0f;

    void Start()
    {
        // WebSocketサーバーの設定
        ws = new WebSocket("ws://localhost:8080");

        ws.OnMessage += (sender, e) =>
        {
            // WebSocketからメッセージを受信
            Debug.Log("Received message: " + e.Data);

            // メッセージをJSONとして解析
            var data = JsonUtility.FromJson<CadenceData>(e.Data);
            cadence = data.cadence;
        };

        ws.Connect();
    }

    void Update()
    {
        // ケイデンスデータに基づいてオブジェクトのz座標を更新
        if (objectToMove != null)
        {
            float speed = cadence * 0.1f; // ケイデンスに基づく移動速度のスケール調整
            objectToMove.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    void OnDestroy()
    {
        // WebSocket接続を閉じる
        if (ws != null)
        {
            ws.Close();
        }
    }

    // JSONデータを解析するためのクラス
    [System.Serializable]
    public class CadenceData
    {
        public float cadence;
    }
}
