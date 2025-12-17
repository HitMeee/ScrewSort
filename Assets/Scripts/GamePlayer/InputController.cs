using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] BoltLogicManager boltLogicManager;

    public float clickCooldown = 0.1f; // Thời gian cooldown giữa các clicks
    private float lastClickTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    public void Init()
    {
        
    }

    void HandleClick()
    {
        // ✅ THÊM: Click cooldown để tránh spam
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BotlBase clickedBolt = hit.collider.GetComponentInParent<BotlBase>();
            if (clickedBolt != null)
            {
                var boltChecker = GamePlayerController.Instance?.gameContaint?.sortScrew?.checker;
                if (boltChecker != null && !boltChecker.CanInteractWithBolt(clickedBolt))
                {
                    return;
                }

                lastClickTime = Time.time;
                boltLogicManager.OnBoltClicked(clickedBolt);
            }
        }
    }

}