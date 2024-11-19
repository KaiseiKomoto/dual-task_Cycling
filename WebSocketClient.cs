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
        // WebSocket�T�[�o�[�̐ݒ�
        ws = new WebSocket("ws://localhost:8080");

        ws.OnMessage += (sender, e) =>
        {
            // WebSocket���烁�b�Z�[�W����M
            Debug.Log("Received message: " + e.Data);

            // ���b�Z�[�W��JSON�Ƃ��ĉ��
            var data = JsonUtility.FromJson<CadenceData>(e.Data);
            cadence = data.cadence;
        };

        ws.Connect();
    }

    void Update()
    {
        // �P�C�f���X�f�[�^�Ɋ�Â��ăI�u�W�F�N�g��z���W���X�V
        if (objectToMove != null)
        {
            float speed = cadence * 0.1f; // �P�C�f���X�Ɋ�Â��ړ����x�̃X�P�[������
            objectToMove.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    void OnDestroy()
    {
        // WebSocket�ڑ������
        if (ws != null)
        {
            ws.Close();
        }
    }

    // JSON�f�[�^����͂��邽�߂̃N���X
    [System.Serializable]
    public class CadenceData
    {
        public float cadence;
    }
}
