using UnityEngine;


public class SelfDestruct : MonoBehaviour
{
	public float lifeSpan;

	void Start()
	{
		Destroy(gameObject, lifeSpan);
	}
}
