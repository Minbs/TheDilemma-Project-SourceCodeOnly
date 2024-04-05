using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MonoGUI;
public class TestItemInfo : MonoBehaviour
{
	[field:SerializeField]
	public Sprite Item { private set; get; }

	[field: SerializeField]
	public string NameEntrykey { private set; get; }
	[field: SerializeField]
	public string InfoEntryKey { private set; get; }
}
