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
		float elapsedTime = 0;
		GameObject[] babies = GameObject.FindGameObjectsWithTag("Babies");

		while (elapsedTime < this.timeActive)
		{
			elapsedTime += Time.deltaTime;
			for (int i = 0; i < babies.Length; ++i)
			{
				if (Vector3.Distance(target, babies[i].transform.position) < 5)
					babies[i].GetComponent<Baby>().FocusTarget(target);
			}
			yield return new WaitForEndOfFrame();
		}

		for (int i = 0; i < babies.Length; ++i)
		{
			babies[i].GetComponent<Baby>().FocusTarget(babies[i].GetComponent<Baby>().originalTargetPos);
		}
	}

}
