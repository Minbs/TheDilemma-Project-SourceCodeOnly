using System;

public interface IGUIAnimationControl
{
	public void PlayShow(Action callback = null);
	public void PlayHide(Action callback = null);
}

