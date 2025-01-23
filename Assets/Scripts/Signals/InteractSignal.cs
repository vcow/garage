using Logic;
using UnityEngine;

namespace Signals
{
	public class InteractSignal
	{
		public BaseInteractableObject InteractableObject { get; }
		public GameObject Character { get; }

		public InteractSignal(BaseInteractableObject interactableObject, GameObject character)
		{
			InteractableObject = interactableObject;
			Character = character;
		}
	}
}