using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Scripts.UI
{
	public class CanvasAdjuster : MonoBehaviour
	{
		private CanvasScaler mCanvasScaler;

		public void Start()
		{
			mCanvasScaler = GetComponent<CanvasScaler>();

			if (mCanvasScaler == null)
			{
				Ulog.LogError(this, $"There is no CanvasScaler to adjust!");
			}
		}

		public void Update()
		{
			mCanvasScaler.scaleFactor = (Screen.height / 270) * 0.5f;
		}
	}
}
