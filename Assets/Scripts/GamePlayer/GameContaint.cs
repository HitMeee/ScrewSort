using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContaint : MonoBehaviour
{
    public LevelController levelController;
    public InputController inputController;
    public void Init()
    {
        levelController.Init();
        inputController.Init();
    }
}
