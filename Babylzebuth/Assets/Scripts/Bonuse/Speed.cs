using UnityEngine;
using System.Collections;

public class Speed : Bonus
{

	protected override void Action(Transform target)
	{
		PlayerController player = target.GetComponent<PlayerController>();

		StartCoroutine(actionCoroutine(player));
	}

	IEnumerator actionCoroutine(PlayerController target)
	{
		target._maxSpeed *= 2;
		yield return new WaitForSeconds(timeActive);
		//Debug.Log("end");
		target._maxSpeed /= 2;
		
	}

}
