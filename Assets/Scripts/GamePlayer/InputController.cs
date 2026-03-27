using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] BoltLogicManager boltLogicManager;

    public float clickCooldown = 0.05f; // Cooldown ngắn để responsive nhưng vẫn tránh spam
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
        // ✅ Click cooldown để tránh spam quá nhanh
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        // ✅ CHO PHÉP CLICK VÀO QUEUE - Animation sẽ chạy tuần tự trong BoltLogicManager

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // ✅ Lấy bolt parent từ object được click (có thể là bolt hoặc screw)
            BotlBase clickedBolt = hit.collider.GetComponentInParent<BotlBase>();
            
            // ✅ KIỂM TRA: Nếu không lấy được bolt parent, thử lấy từ transform parent
            if (clickedBolt == null)
            {
                var screw = hit.collider.GetComponent<ScrewBase>();
                if (screw != null && screw.transform.parent != null)
                {
                    clickedBolt = screw.transform.parent.GetComponent<BotlBase>();
                }
            }
            
            if (clickedBolt != null)
            {
                var boltChecker = GamePlayerController.Instance?.gameContaint?.sortScrew?.checker;
                if (boltChecker != null && !boltChecker.CanInteractWithBolt(clickedBolt))
                {
                    return;
                }

                lastClickTime = Time.time;
                
                // ✅ Gửi click vào queue - BoltLogicManager sẽ xử lý tuần tự
                boltLogicManager.OnBoltClicked(clickedBolt);
            }
        }
    }

}