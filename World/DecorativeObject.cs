using UnityEngine;


public class DecorativeObject : MonoBehaviour
{
    public float movementPercentage = 0.2f;

    [HideInInspector] 
    public float depthOnSpawn;

    private void Update()
    {
        var moveAmountY = PlayerManager.Instance.velocity.y * movementPercentage * Time.deltaTime;
        transform.Translate(0, moveAmountY, 0);
    }

}
