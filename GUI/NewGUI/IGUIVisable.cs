using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGUIVisable
{
	public void Show(Action callback = null);
	public void Hide(Action callback = null);
}