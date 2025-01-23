using Logic;

namespace Signals
{
	public class CanInteractSignal
	{
		public BaseInteractableObject InteractableObject { get; }
		public bool CanInteract => InteractableObject;

		public CanInteractSignal(BaseInteractableObject interactableObject)
		{
			InteractableObject = interactableObject;
		}
	}
}