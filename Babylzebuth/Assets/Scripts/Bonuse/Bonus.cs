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
			Action(colli.transform);
			GameManager.Instance.BonusSpawned = false;
			Destroy(this.gameObject);
		}
	}

	protected virtual void Action(Transform target) { throw new NotImplementedException(); }
}
