using Logic;
using UnityEngine;

namespace Signals
{
	public class InteractSignal
	{
		public BaseInteractableObject InteractableObject { get; }
		public GameObject Character { get; }
		public Vector3 HitPoint { get; }

		public InteractSignal(BaseInteractableObject interactableObject, GameObject character, Vector3 hitPoint)
		{
			InteractableObject = interactableObject;
			Character = character;
			HitPoint = hitPoint;
		}
	}
}