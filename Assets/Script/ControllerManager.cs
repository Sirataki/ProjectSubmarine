using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour {

    public static ControllerManager Instance { get; private set;}

    private GamePad.Index player1, player2;

    public GamepadState gamePadState1, gamePadState2;

    [SerializeField]
    private TitleController titleController;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private ResultManager resultManeger;

    private bool ready1p = false, ready2p = false;

    public static bool aiFlag = false;

    public GameObject aiText;

    public enum PlayerNum
    {
        Player1 =1,
        Player2,
        Player3,
        Player4
    };

    public PlayerNum playerNum1, playerNum2;

    public enum Scene
    {
        Title,
        Main,
        Result
    }

    public Scene scene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            player1 = (GamePad.Index)playerNum1;
            player2 = (GamePad.Index)playerNum2;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Start()
    {
        if (scene == Scene.Main)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        else if (scene == Scene.Title)
        {
            aiFlag = false;
        }
    }

    private void Update()
    {
        gamePadState1 = GamePad.GetState(player1, gamePadState1);
        gamePadState2 = GamePad.GetState(player2, gamePadState2);

        switch (scene)
        {
            case Scene.Title:
                TitleUpdate();
                break;
            case Scene.Main:
                MainUpdate1p();
                MainUpdate2p();
                break;
            case Scene.Result:
                ResultUpdate();
                break;
        }
    }

    private void TitleUpdate()
    {
        if (gamePadState1.A_down || gamePadState1.B_down || gamePadState1.X_down || gamePadState1.Y_down)
        {
            titleController.Is1pFlag = true;
        }

        if (gamePadState2.A_down || gamePadState2.B_down || gamePadState2.X_down || gamePadState2.Y_down)
        {
            titleController.Is2pFlag = true;
        }

        if (gamePadState2.Start_down && gamePadState2.Back_down && !aiFlag)
        {
            aiFlag = true;
        }
        else if (gamePadState2.Start_down && gamePadState2.Back_down && aiFlag)
        {
            aiFlag = false;
        }
        aiText.SetActive(aiFlag);
    }

    private void MainUpdate1p()
    {
        if (gamePadState1.LeftStickAxis.x < 0)
        {
            gameManager.submarineController.getControllSubmarineX(gamePadState1.LeftStickAxis.x);
        }
        else if (gamePadState1.LeftStickAxis.x > 0)
        {
            gameManager.submarineController.getControllSubmarineX(gamePadState1.LeftStickAxis.x);
        }
        else
        {
            gameManager.submarineController.getControllSubmarineX(gamePadState1.LeftStickAxis.x);
        }

        if (gamePadState1.LeftStickAxis.y < 0)
        {
            gameManager.submarineController.getControllSubmarineY(gamePadState1.LeftStickAxis.y);
        }
        else if (gamePadState1.LeftStickAxis.y > 0)
        {
            gameManager.submarineController.getControllSubmarineY(gamePadState1.LeftStickAxis.y);
        }
        else
        {
            gameManager.submarineController.getControllSubmarineY(gamePadState1.LeftStickAxis.y);
        }

        if (gamePadState1.RightShoulder)
        {
            gameManager.submarineController.FireTorpedo();
        }
    }

    private void MainUpdate2p()
    {
        if(aiFlag){
            return;
        }
        if (gamePadState2.LeftStickAxis.x < 0)
        {
            gameManager.shipController.getControllShip(gamePadState2.LeftStickAxis.x);
        }
        else if (gamePadState2.LeftStickAxis.x > 0)
        {
            gameManager.shipController.getControllShip(gamePadState2.LeftStickAxis.x);
        }
        else
        {
            gameManager.shipController.getControllShip(gamePadState2.LeftStickAxis.x);
        }

        if (gamePadState2.A_down)
        {
            gameManager.shipController.createTrashBoots();
        }
        else if (gamePadState2.B_down)
        {
            gameManager.shipController.createTrashMicroWave();
        }
        else if (gamePadState2.X_down)
        {
            gameManager.shipController.createTrashIceBox();
        }
        else if (gamePadState2.Y_down)
        {
            gameManager.shipController.createTrapNet();
        }
    }

    private void ResultUpdate()
    {
        if (gamePadState1.LeftShoulder && gamePadState1.RightShoulder)
        {
            ready1p = true;
            resultManeger.ready1pFlag = true;
        }

        if (gamePadState2.LeftShoulder && gamePadState2.RightShoulder)
        {
            ready2p = true;
            resultManeger.ready2pFlag = true;
        }

        if (ready1p && ready2p && resultManeger.ReturnToTitle())
        {
            resultManeger.BackFlagProp = false;
            ready1p = false;
            ready2p = false;

            scene = Scene.Title;
            SceneManager.LoadScene("Title");
        }

    }
}
