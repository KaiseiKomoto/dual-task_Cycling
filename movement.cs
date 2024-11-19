using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    public Text problemText;
    public Text resultText;
    public GameObject canvasParent; // Canvasが含まれている親オブジェクト（ゲームオブジェクト）
    public GameObject objectToFollow; // オブジェクト (プレイヤーなど)


    //public float canvasOffsetX = -478.011f; // x軸のオフセット
    //public float canvasOffsetY = -296.562f; // y軸のオフセット
    public float canvasOffsetZ = 5f; // オブジェクトの前に配置する距離

    private int number1;
    private int number2;
    private int number3;
    private int number4;
    private string currentProblem;
    private string correctAnswer;
    private string[] operations = { "+", "-", "*", "/" };
    private string rightOperation;
    private string correctOperation;

    private float threshold = 20f; // z座標の閾値
    private float lastCanvasVisibleZ = 0f; // 最後にCanvasが表示されたz座標
    private bool canvasVisible = true; // Canvasの表示状態

    void Start()
    {
        HideCanvas();
        GenerateNewProblem();
        lastCanvasVisibleZ = objectToFollow.transform.position.z;
    }

    void Update()
    {
        // ゲームオブジェクトをオブジェクトに追従させる
        Vector3 newCanvasPosition = objectToFollow.transform.position;
        newCanvasPosition.z += canvasOffsetZ; // オブジェクトの前方に配置
        canvasParent.transform.position = newCanvasPosition; // Canvasを含むゲームオブジェクトの位置を更新

        // z座標に応じてCanvasを表示/非表示
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
            // ランダムな数値を生成します。
            number1 = Random.Range(1, 51);
            number2 = Random.Range(1, 51);
            number3 = Random.Range(1, 51);
            number4 = Random.Range(1, 51);

            // 右側の演算子をランダムに選択します。
            rightOperation = operations[Random.Range(0, operations.Length)];

            // 右側の式を計算します。
            int rhs = Calculate(number3, number4, rightOperation);

            // ランダムな不等号を選択します。
            string[] inequalities = { "<", ">", "<=", ">=" };
            string inequality = inequalities[Random.Range(0, inequalities.Length)];

            // 問題文を作成します。
            currentProblem = $"{number1} ? {number2} {inequality} {number3} {rightOperation} {number4}";
            resultText.text = "";

            // 各操作を試して正しい答えが一つだけ存在することを確認します。
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

        // 問題文を表示します。
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
        // 左側の式を計算します。
        int lhs = Calculate(number1, number2, selectedOption);

        // 右側の式を計算します。
        int rhs = Calculate(number3, number4, rightOperation);

        string inequality = currentProblem.Split(' ')[3];

        // 正しいかどうかをチェックします。
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

        // 結果を表示します。
        if (isCorrect && selectedOption == correctOperation)
        {
            resultText.text = "正解！";
        }
        else
        {
            resultText.text = "不正解。";
        }

        // 解答後にCanvasを非表示にする
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
                return b != 0 ? a / b : 0; // 0除算回避
            default:
                return 0;
        }
    }

    void ShowCanvas()
    {
        canvasParent.SetActive(true); // Canvasを含むゲームオブジェクト全体を表示
        canvasVisible = true;
    }

    void HideCanvas()
    {
        canvasParent.SetActive(false); // Canvasを含むゲームオブジェクト全体を非表示
        canvasVisible = false;
    }
}
