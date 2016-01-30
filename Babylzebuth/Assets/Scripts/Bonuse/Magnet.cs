using UnityEngine;
using System.Collections;

public class Magnet : Bonus
{
	protected override void Action(Transform target)
	{
		StartCoroutine(focusCoroutine(target.position));
	}

	IEnumerator focusCoroutine(Vector3 target)
	{
		GameObject[] babies = GameObject.FindGameObjectsWithTag("Babies");
		for (int i = 0; i < babies.Length; ++i)
		{
			babies[i].GetComponent<Baby>().FocusTarget(target);
		}

		yield return new WaitForSeconds(this.timeActive);

		for (int i = 0; i < babies.Length; ++i)
		{
			babies[i].GetComponent<Baby>().FocusTarget(babies[i].GetComponent<Baby>().originalTargetPos);
		}
	}

}
