using UnityEngine;
using System.Collections;

public class Shield : Bonus {
	protected override void Action(Transform target)
	{
		StartCoroutine(actionCoroutine(target.GetComponent<PlayerController>()));
	}


	IEnumerator actionCoroutine(PlayerController target)
	{
		target.setInvulTime(timeActive);
		
		yield return new WaitForSeconds(timeActive);
	}
}
