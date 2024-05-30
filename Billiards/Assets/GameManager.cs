using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum CurrentPlayer
    {
        Player1,
        Player2
    }

    CurrentPlayer currentPlayer;
    bool isWinningShotforPlayer1 = false;
    bool isWinningShotforPlayer2 = false;
    int player1BallsRemaining = 7;
    int player2BallsRemaining = 7;
    bool isWaitingForBallMovementToStop = false;

    [SerializeField] TextMeshProUGUI player1BallsText;
    [SerializeField] TextMeshProUGUI player2BallsText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI messageText;

    [SerializeField] GameObject restartButton;

    [SerializeField] Transform headPosition;

    [SerializeField] Camera cueStickCamera;
    [SerializeField] Camera overheadCamera;

    Camera currentCamera;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCamera = cueStickCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingForBallMovementToStop) //ожидание остановки шаров перед передачей хода
        {

        }
    }

    public void SwitchCameras()
    {
        if (currentCamera == cueStickCamera)
        {
            cueStickCamera.enabled = false;
            overheadCamera.enabled = true;
            currentCamera = overheadCamera;
        }
        else
        {
            cueStickCamera.enabled = true;
            overheadCamera.enabled = false;
            currentCamera = cueStickCamera;
            currentCamera.gameObject.GetComponent<CameraController>().ResetCamera();
        }
    }

    public void RestartTheGame()
    {
        SceneManager.LoadScene(0);
    }

    bool Scratch()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            if (isWinningShotforPlayer1) 
            {
                ScratchOnWinningShot("Игрок 1");
            }
        }
        else
        {
            if(isWinningShotforPlayer2)
            {
                ScratchOnWinningShot("Игрок 2");
                return true;
            }
        }
        NextPlayerTurn();
        return false;
    }

    void EarlyEightBall()
    {
        if (currentPlayer == CurrentPlayer.Player1) 
        {
            Lose("Игрок 1 забил 8 слишком рано и проиграл");
        }
        else
        {
            Lose("Игрок 2 забил 8 слишком рано и проиграл");
        }

    }

    void ScratchOnWinningShot(string player)
    {
        Lose(player + " Забил белый шар и проиграл!");
    }

    void NoMoreBalls(CurrentPlayer player)
    {
        if (player == CurrentPlayer.Player1)
        {
            isWinningShotforPlayer1 = true;
        }
        else
        {
            isWinningShotforPlayer2 = true;
        }
    }

    bool CheckBall(Ball ball)
    {
        if (ball.IsCueBall())
        {
            if (Scratch()) 
            {
                return true; 
            }
            else 
            { 
                return false; 
            }
        }
        else if (ball.IsEightBall())
        {
            if (currentPlayer == CurrentPlayer.Player1)
            {
                if (isWinningShotforPlayer1)
                {
                    Win("Игрок 1");
                    return true;
                }
            }
            else
            {
                if (isWinningShotforPlayer2)
                {
                    Win("Игрок 2");
                    return true;
                }
            }
            EarlyEightBall();
        }
        //логика поведения других шаров кроме белого и восьмерки
        else
        {
            if (ball.IsBallRed())
            {
                player1BallsRemaining--;
                player1BallsText.text = " Игрок 1\r\nОсталось шаров:" + player1BallsRemaining;
                if (player1BallsRemaining <= 0)
                {
                    isWinningShotforPlayer1 = true;
                }
                if (currentPlayer != CurrentPlayer.Player1) 
                {
                    NextPlayerTurn();
                }
            }
            else
            {
                player2BallsRemaining--;
                player2BallsText.text = " Игрок 2\r\nОсталось шаров:" + player2BallsRemaining;
                if (player2BallsRemaining <= 0)
                {
                    isWinningShotforPlayer2 = true;
                }
                if (currentPlayer != CurrentPlayer.Player2)
                {
                    NextPlayerTurn();
                }
            }

        }

        return true;
    }

    void Lose (string message)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        restartButton.SetActive(true);
    }

    void Win(string player)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = player + " победил";
        restartButton.SetActive(true);
    }

    void NextPlayerTurn()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            currentPlayer = CurrentPlayer.Player2;
            currentTurnText.text = "Ход игрока 2";
        }
        else
        {
            currentPlayer = CurrentPlayer.Player1;
            currentTurnText.text = "Ход игрока 1";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (CheckBall(other.gameObject.GetComponent<Ball>())) 
            { 
                Destroy(other.gameObject);
            }
            else
            {
                other.gameObject.transform.position = headPosition.position;
                other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        
    }
}
