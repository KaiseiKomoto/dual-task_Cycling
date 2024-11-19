using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    public Text problemText;
    public Text resultText;
    public GameObject canvasParent; // Canvas���܂܂�Ă���e�I�u�W�F�N�g�i�Q�[���I�u�W�F�N�g�j
    public GameObject objectToFollow; // �I�u�W�F�N�g (�v���C���[�Ȃ�)


    //public float canvasOffsetX = -478.011f; // x���̃I�t�Z�b�g
    //public float canvasOffsetY = -296.562f; // y���̃I�t�Z�b�g
    public float canvasOffsetZ = 5f; // �I�u�W�F�N�g�̑O�ɔz�u���鋗��

    private int number1;
    private int number2;
    private int number3;
    private int number4;
    private string currentProblem;
    private string correctAnswer;
    private string[] operations = { "+", "-", "*", "/" };
    private string rightOperation;
    private string correctOperation;

    private float threshold = 20f; // z���W��臒l
    private float lastCanvasVisibleZ = 0f; // �Ō��Canvas���\�����ꂽz���W
    private bool canvasVisible = true; // Canvas�̕\�����

    void Start()
    {
        HideCanvas();
        GenerateNewProblem();
        lastCanvasVisibleZ = objectToFollow.transform.position.z;
    }

    void Update()
    {
        // �Q�[���I�u�W�F�N�g���I�u�W�F�N�g�ɒǏ]������
        Vector3 newCanvasPosition = objectToFollow.transform.position;
        newCanvasPosition.z += canvasOffsetZ; // �I�u�W�F�N�g�̑O���ɔz�u
        canvasParent.transform.position = newCanvasPosition; // Canvas���܂ރQ�[���I�u�W�F�N�g�̈ʒu���X�V

        // z���W�ɉ�����Canvas��\��/��\��
        float currentZPosition = objectToFollow.transform.position.z;
        if (!canvasVisible && Mathf.Abs(currentZPosition - lastCanvasVisibleZ) >= threshold)
        {
            ShowCanvas();
            lastCanvasVisibleZ = currentZPosition;
        }
    }

    void GenerateNewProblem()
    {
        bool problemHasUniqueSolution = false;

        while (!problemHasUniqueSolution)
        {
            // �����_���Ȑ��l�𐶐����܂��B
            number1 = Random.Range(1, 51);
            number2 = Random.Range(1, 51);
            number3 = Random.Range(1, 51);
            number4 = Random.Range(1, 51);

            // �E���̉��Z�q�������_���ɑI�����܂��B
            rightOperation = operations[Random.Range(0, operations.Length)];

            // �E���̎����v�Z���܂��B
            int rhs = Calculate(number3, number4, rightOperation);

            // �����_���ȕs������I�����܂��B
            string[] inequalities = { "<", ">", "<=", ">=" };
            string inequality = inequalities[Random.Range(0, inequalities.Length)];

            // ��蕶���쐬���܂��B
            currentProblem = $"{number1} ? {number2} {inequality} {number3} {rightOperation} {number4}";
            resultText.text = "";

            // �e����������Đ�������������������݂��邱�Ƃ��m�F���܂��B
            int correctCount = 0;
            string uniqueCorrectOperation = "";

            foreach (string operation in operations)
            {
                int lhs = Calculate(number1, number2, operation);

                bool isCorrect = false;
                switch (inequality)
                {
                    case "<":
                        isCorrect = lhs < rhs;
                        break;
                    case ">":
                        isCorrect = lhs > rhs;
                        break;
                    case "<=":
                        isCorrect = lhs <= rhs;
                        break;
                    case ">=":
                        isCorrect = lhs >= rhs;
                        break;
                }

                if (isCorrect)
                {
                    correctCount++;
                    uniqueCorrectOperation = operation;
                }
            }

            if (correctCount == 1)
            {
                problemHasUniqueSolution = true;
                correctOperation = uniqueCorrectOperation;
            }
        }

        // ��蕶��\�����܂��B
        problemText.text = "Question: \n" + currentProblem;
    }

    public void OnClickA()
    {
        CheckAnswer("+");
    }

    public void OnClickB()
    {
        CheckAnswer("-");
    }

    public void OnClickC()
    {
        CheckAnswer("*");
    }

    public void OnClickD()
    {
        CheckAnswer("/");
    }

    public void CheckAnswer(string selectedOption)
    {
        // �����̎����v�Z���܂��B
        int lhs = Calculate(number1, number2, selectedOption);

        // �E���̎����v�Z���܂��B
        int rhs = Calculate(number3, number4, rightOperation);

        string inequality = currentProblem.Split(' ')[3];

        // ���������ǂ������`�F�b�N���܂��B
        bool isCorrect = false;
        switch (inequality)
        {
            case "<":
                isCorrect = lhs < rhs;
                break;
            case ">":
                isCorrect = lhs > rhs;
                break;
            case "<=":
                isCorrect = lhs <= rhs;
                break;
            case ">=":
                isCorrect = lhs >= rhs;
                break;
        }

        // ���ʂ�\�����܂��B
        if (isCorrect && selectedOption == correctOperation)
        {
            resultText.text = "�����I";
        }
        else
        {
            resultText.text = "�s�����B";
        }

        // �𓚌��Canvas���\���ɂ���
        HideCanvas();
    }

    int Calculate(int a, int b, string operation)
    {
        switch (operation)
        {
            case "+":
                return a + b;
            case "-":
                return a - b;
            case "*":
                return a * b;
            case "/":
                return b != 0 ? a / b : 0; // 0���Z���
            default:
                return 0;
        }
    }

    void ShowCanvas()
    {
        canvasParent.SetActive(true); // Canvas���܂ރQ�[���I�u�W�F�N�g�S�̂�\��
        canvasVisible = true;
    }

    void HideCanvas()
    {
        canvasParent.SetActive(false); // Canvas���܂ރQ�[���I�u�W�F�N�g�S�̂��\��
        canvasVisible = false;
    }
}
