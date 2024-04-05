using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Service;

public class TestMonoService : MonoService
{
	public override void OnRegistered()
	{
		base.OnRegistered();
		Ulog.Log(this, "OnRegistered");
	}

	public override void OnUnregistered()
	{
		Ulog.Log(this, "OnUnregistered");
		base.OnUnregistered();
	}
}
