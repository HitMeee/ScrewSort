 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerController : MonoBehaviour
{
    public static GamePlayerController Instance;

    public GameScene gameScene;
    public GameContaint gameContaint;
    

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameScene.Init();
        gameContaint.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
