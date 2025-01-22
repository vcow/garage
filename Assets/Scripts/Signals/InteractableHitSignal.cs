using Logic;

namespace Signals
{
	public sealed class InteractableHitSignal
	{
		public BaseInteractableObject InteractableObject { get; }

		public InteractableHitSignal(BaseInteractableObject interactableObject)
		{
			InteractableObject = interactableObject;
		}
	}
}