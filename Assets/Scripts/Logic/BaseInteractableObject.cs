using Signals;
using UnityEngine;
using Zenject;

namespace Logic
{
	[DisallowMultipleComponent]
	public abstract class BaseInteractableObject : MonoBehaviour
	{
		[field: SerializeField] public string Name { get; private set; }

		[Inject] private readonly SignalBus _signalBus;

		protected virtual void Start()
		{
			_signalBus.Subscribe<InteractSignal>(OnInteract);
		}

		protected virtual void OnDestroy()
		{
			_signalBus.Unsubscribe<InteractSignal>(OnInteract);
		}

		private void OnInteract(InteractSignal signal)
		{
			if (signal.InteractableObject == this)
			{
				Interact(signal.HitPoint, signal.Character);
			}
		}

		protected abstract void Interact(Vector3 hitPoint, GameObject character);
	}
}