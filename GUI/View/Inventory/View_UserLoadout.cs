using Utils;
using UnityEngine;
using UnityEngine.UI;
using Utils.ViewModel;
using Sirenix.OdinInspector;

public class View_UserLoadout : GUINavigationView
{
    [SerializeField] private ButtonViewModel Btn_Exit = new(nameof(Btn_Exit));
    
    public override void NaigationViewSetup(GUINavigation navigation)
    {
        base.NaigationViewSetup(navigation);

        Btn_Exit.Initialize(this);
        Btn_Exit.BindAction(Exit);
    }

    private void Exit()
    {
		mNavigation.TryPop(out var topView);
    }

    public void SetupByCharacterInfomation()
	{

	}
}
