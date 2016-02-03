using UnityEngine;
using System.Collections;
using System;

public abstract class Bonus : MonoBehaviour {
	[SerializeField]
	protected float timeActive = 3;
	protected void OnTriggerEnter(Collider colli)
	{
		if(colli.tag.Substring(0,6) == "Player")
		{
			if (colli.GetComponent<PlayerController>() != null)
			{
				Action(colli.transform);
				GameManager.Instance.BonusSpawned = false;
				GameManager.Instance.resetBonusTimer();
				//Debug.Log(GetType());
				Destroy(this.gameObject, timeActive + 0.5f);
				GetComponent<Renderer>().enabled = false;
				GetComponent<Collider>().enabled = false;
			}
		}
	}

	protected virtual void Action(Transform target) { throw new NotImplementedException(); }
}
