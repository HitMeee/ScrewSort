using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContaint : MonoBehaviour
{
    public LevelController levelController;
    public InputController inputController;
    public BoltLogicManager boltLogicManager;
    public SortScrew sortScrew;
    public void Init()
    {

        levelController.Init();
        inputController.Init();
        boltLogicManager.Init();
        sortScrew.Init();
    }
}
