using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] BoltLogicManager boltLogicManager;

    [Header("Click Settings")]
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
        if (boltLogicManager == null)
        {
            boltLogicManager = FindObjectOfType<BoltLogicManager>();
        }
    }

    void HandleClick()
    {
        // ✅ THÊM: Click cooldown để tránh spam
        if (Time.time - lastClickTime < clickCooldown)
        {
            Debug.Log($"⚠️ Click too fast, ignoring (cooldown: {clickCooldown}s)");
            return;
        }

        // ✅ THÊM: Kiểm tra BoltLogicManager có sẵn sàng không
        if (boltLogicManager == null)
        {
            Debug.LogWarning("❌ BoltLogicManager not found!");
            return;
        }

        // ✅ THÊM: Kiểm tra có đang xử lý animation không
        if (boltLogicManager.IsCurrentlyAnimating())
        {
            Debug.Log("⏳ Animation in progress, adding to queue");
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BotlBase clickedBolt = hit.collider.GetComponentInParent<BotlBase>();
            if (clickedBolt != null)
            {
                // ✅ THÊM: Log để debug
                Debug.Log($"🎯 Clicked on bolt: {clickedBolt.name}");

                // ✅ THÊM: Kiểm tra bolt có thể tương tác không
                var boltChecker = GamePlayerController.Instance?.gameContaint?.sortScrew?.checker;
                if (boltChecker != null && !boltChecker.CanInteractWithBolt(clickedBolt))
                {
                    Debug.Log($"🔒 Bolt {clickedBolt.name} is locked, cannot interact");
                    return;
                }

                lastClickTime = Time.time;
                boltLogicManager.OnBoltClicked(clickedBolt);
            }
        }
    }

    // ✅ THÊM: Public methods for debugging
    public bool CanAcceptClick()
    {
        return Time.time - lastClickTime >= clickCooldown && !boltLogicManager.IsCurrentlyAnimating();
    }

    public float GetTimeSinceLastClick()
    {
        return Time.time - lastClickTime;
    }
}