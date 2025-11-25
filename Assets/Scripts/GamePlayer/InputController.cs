using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] BoltLogicManager boltLogicManager;

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BotlBase clickedBolt = hit.collider.GetComponentInParent<BotlBase>();
            if (clickedBolt != null)
            {
                boltLogicManager.OnBoltClicked(clickedBolt);
            }
        }
    }
}
