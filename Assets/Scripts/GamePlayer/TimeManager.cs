using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    [SerializeField] private GameObject panelLose; // Kéo UI Text vào đây

    [Header("Giao diện")]
    [SerializeField] private TextMeshProUGUI timeText; // Kéo UI Text vào đây

    private float totalTime = 60f;
    private float currentTime;
    private bool isTimerRunning = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 1. MẶC ĐỊNH TẮT KHI MỚI VÀO GAME
        isTimerRunning = false;
        if (timeText != null) 
        {
            timeText.text = ""; // Xóa nội dung
            timeText.gameObject.SetActive(false); // Ẩn luôn object text cho gọn
        }
    }

    // Hàm này được gọi từ LevelController khi bắt đầu tạo level
    public void SetLevelTime(float seconds)
    {
        totalTime = seconds;
        currentTime = seconds;
        
        // 2. BẬT LÊN KHI VÀO LEVEL
        isTimerRunning = true;
        
        if (timeText != null)
        {
            timeText.gameObject.SetActive(true); // Hiện lại text
            UpdateUI(); // Cập nhật ngay số giây ban đầu (ví dụ 60:00)
        }

        Debug.Log($"⏰ Timer đã bật: {totalTime}s");
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateUI();
            }
            else
            {
                currentTime = 0;
                UpdateUI();
                isTimerRunning = false;
                GameOver(false);
            }
        }
    }

    void UpdateUI()
    {
        if (timeText == null) return;

        // Format lại hiển thị cho đẹp (01:05 thay vì 1:5)
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void FinishLevel()
    {
        if (!isTimerRunning) return; // Tránh gọi nhiều lần

        isTimerRunning = false;
        
        // Tính % thời gian còn lại để xét sao
        float percentLeft = (totalTime > 0) ? (currentTime / totalTime) * 100f : 0;
        int stars = CalculateStars(percentLeft);

        Debug.Log($"🏆 Hoàn thành! Dư: {percentLeft:F1}% -> {stars} Sao");
        
        // Gọi UI Win (Ví dụ: UIManager.Instance.ShowWin(stars));
    }
    
    // Hàm lấy số sao hiện tại dựa trên thời gian còn lại
    public int GetCurrentStars()
    {
        if (totalTime <= 0) return 1; // Nếu không có timer, mặc định 1 sao
        
        float percentLeft = (currentTime / totalTime) * 100f;
        return CalculateStars(percentLeft);
    }

    // Hàm tắt timer cưỡng bức (ví dụ khi bấm nút Pause hoặc về Home)
    public void StopAndHideTimer()
    {
        isTimerRunning = false;
        if (timeText != null) timeText.gameObject.SetActive(false);
    }

    int CalculateStars(float percentage)
    {
        if (percentage >= 80f) return 3;       
        if (percentage >= 30f) return 2;       
        return 1;                             
    }

    void GameOver(bool win)
    {
        if (!win)
        {
            panelLose.SetActive(true);
        }
    }
}