using Utils;
using Utils.Service;

public class SceneManageServiceLocator : IUnregistrable
{
	private SceneManageService mServiceInstance = null;

	public void RegisterService(SceneManageService sceneManageService)
	{
		if (sceneManageService == null)
		{
			return;
		}

		mServiceInstance?.OnUnregistered();
		mServiceInstance = sceneManageService;

		Ulog.Log(UlogType.Service, $"{sceneManageService.GetType().Name} Registered!");
		mServiceInstance.OnRegistered();
	}

	public void UnregisterService()
	{
		if (mServiceInstance != null)
		{
			mServiceInstance.OnUnregistered();
			Ulog.Log(UlogType.Service, $"{mServiceInstance.GetType().Name} Unregistered!");
		}
	}

	public T GetServiceOrNull<T>() where T : SceneManageService
	{
		return mServiceInstance as T;
	}

	public bool TryGetService<T>(out T service) where T : SceneManageService
	{
		service = mServiceInstance as T;
		return (mServiceInstance != null && mServiceInstance is T);
	}
}