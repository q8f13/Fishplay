using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISocket : MonoBehaviour {
	protected IEmbed _currentItem;
	abstract internal void OnEmbed(IEmbed item);
	abstract internal void OnUnembed(IEmbed item);
	internal delegate void EmbedEvent(bool on);
	internal EmbedEvent OnEmbedTriggered;

	private void Start()
	{
		OnEmbedTriggered = OnTriggeredHandler;
	}

	private void OnTriggeredHandler(bool on)
	{
		throw new NotImplementedException();
	}
}

/* public interface ISocketItem
{
} */

public interface IEmbed
{

}