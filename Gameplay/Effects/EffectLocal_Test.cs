using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay
{
	public class EffectLocal_Test : MonoBehaviour
	{
		public void OnEnable()
		{
			StartCoroutine(release());
		}

		public IEnumerator release()
		{
			yield return new WaitForSeconds(0.5f);
			gameObject.SetActive(false);
		}
	}
}
