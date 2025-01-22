using UnityEngine;

namespace Logic
{
	[DisallowMultipleComponent]
	public abstract class BaseInteractableObject : MonoBehaviour
	{
		[field: SerializeField] public string Name { get; private set; }

		public abstract void Interact();
	}
}