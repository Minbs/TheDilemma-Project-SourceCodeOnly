using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Service;

public class SystemInformationService : MonoService
{
	//public CultureInfo CurrentCultureInfo { get; private set; }

	public override void OnRegistered()
	{
		base.OnRegistered();
		//CurrentCultureInfo = CultureInfo.CurrentCulture;
	}

	public override void OnUnregistered()
	{
		base.OnUnregistered();
	}

	public string GetSystemTime()
	{
		return "";

		//return $"{DateTime.Now.ToString("t", CurrentCultureInfo)}<br>" +
//			$"{DateTime.Now.ToString("d", CurrentCultureInfo)}";
	}
}
