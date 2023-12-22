using UnityEngine;


public class ObjectTester : MonoBehaviour
{
	public GameObject ObjectToCreate;
	public Transform target;

    [Header("Options")]
	public KeyCode testButton = KeyCode.Keypad5;
    public bool destroyAfterCreating;
    public float destroyTime = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(testButton))
        {
            CreateObject();
        }
    }

    void CreateObject()
    {
        var newObject = Instantiate(ObjectToCreate, target.position, Quaternion.identity);

        if (destroyAfterCreating)
        {
            Destroy(newObject, destroyTime);
        }
    }

}
