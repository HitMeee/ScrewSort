using System.Collections.Generic;
using UnityEngine;

public class AddBolt : MonoBehaviour
{
    [Header("🔧 Thêm Bolt Rỗng")]
    [SerializeField] private LevelController levelController;
    [SerializeField] private BotlBase boltPrefab;

    void Start()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (boltPrefab == null)
            boltPrefab = FindObjectOfType<BotlBase>();
    }

    // ✅ Phương thức nút UI
    public void ButtonAddBolt()
    {
        AddEmptyBolt();
    }

    // ✅ ĐƠN GIẢN: Thêm bolt vào vị trí PostCreateBolts tiếp theo
    public void AddEmptyBolt()
    {
        if (levelController == null || boltPrefab == null) return;

        var allBolts = levelController.GetAllBolts();
        var postCreateBolts = levelController.PostCreateBolts;

        // Kiểm tra xem có thể thêm nhiều hơn không
        if (allBolts.Count >= postCreateBolts.Count)
        {
            Debug.Log("❌ Không còn vị trí nào để thêm bolt!");
            return;
        }

        // ✅ Lưu trạng thái trước khi thêm
        var backStep = levelController.GetBackStep();
        if (backStep != null)
        {
            backStep.SaveCurrentState();
        }

        // ✅ Lấy vị trí tiếp theo từ PostCreateBolts
        Vector3 position = postCreateBolts[allBolts.Count].position;

        // ✅ Tạo bolt rỗng
        BotlBase newBolt = Instantiate(boltPrefab, position, Quaternion.identity);
        newBolt.Init(new List<int>()); // Bolt rỗng
        newBolt.name = "EmptyBolt_" + (allBolts.Count + 1);

        // ✅ Thêm vào danh sách
        allBolts.Add(newBolt);

        Debug.Log($"✅ Đã thêm bolt tại vị trí {allBolts.Count - 1}: {position}");
    }

    // Getter đơn giản
    public bool CanAddBolt()
    {
        if (levelController == null) return false;
        var allBolts = levelController.GetAllBolts();
        var postCreateBolts = levelController.PostCreateBolts;
        return allBolts.Count < postCreateBolts.Count;
    }
}