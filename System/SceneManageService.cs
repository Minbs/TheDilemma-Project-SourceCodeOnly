using System.Collections.Generic;
using Utils;
using Utils.Service;

public abstract class SceneManageService : MonoService
{
	public MonoManageObject[] mManageable;
	private List<IUnregistrable> mServiceLocators = new List<IUnregistrable>();
	private List<IServiceable> mServices = new List<IServiceable>();

	private bool mIsFinalized = false;
	private bool mIsInitialized = false;

	protected virtual void Awake()
	{
		GlobalServiceLocator.SceneManageService.RegisterService(this);

		OnRemovedByMono += (e) =>
		{
			GlobalServiceLocator.SceneManageService.UnregisterService();
		};

	}

	/// <summary>모든 서비스를 바인딩합니다.</summary>
	protected abstract void bindAllService();

	#if UNITY_EDITOR

	public virtual void OnValidate()
	{
		mManageable = FindObjectsOfType<MonoManageObject>();
	}

	#endif

	public override void OnRegistered()
	{
		base.OnRegistered();
		bindAllService();

		// Initialize
		if (mIsInitialized)
		{
			return;
		}

		mIsInitialized = true;

		foreach (var obj in mManageable)
		{
			obj?.OnInitialize();
		}

		foreach (var service in mServices)
		{
			service.OnRegistered();
		}
	}

	public override void OnUnregistered()
	{
		// Finalize

		if (mIsFinalized)
		{
			return;
		}

		mIsFinalized = true;

		for (int i = mServices.Count - 1; i >= 0; i--)
		{
			mServices[i]?.OnUnregistered();
		}

		for (int i = mServiceLocators.Count - 1; i >= 0; i--)
		{
			mServiceLocators[i].UnregisterService();
		}

		for (int i = mManageable.Length - 1; i >= 0; i--)
		{
			mManageable[i]?.OnFinalize();
		}

		base.OnUnregistered();
	}

	/// <summary>Scene 서비스 전환시 내부 서비스 해제를 위해 서비스를 바인딩합니다.</summary>
	protected void bindServiceLocator(IUnregistrable service)
	{
		mServiceLocators.Add(service);
	}

	/// <summary>Scene에서 사용할 서비스를 등록합니다. 자동으로 등록 및 해제가 이루어집니다.</summary>
	protected void bindService(IServiceable service)
	{
		mServices.Add(service);
	}
}
