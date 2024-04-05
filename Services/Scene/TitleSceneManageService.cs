using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using Utils.Service;

public class TitleSceneManageService : SceneManageService
{
	[field : SerializeField]
	public TitleGuiService TitleGuiService { get; private set; }

	protected override void bindAllService()
	{
		bindService(TitleGuiService);
	}
}
