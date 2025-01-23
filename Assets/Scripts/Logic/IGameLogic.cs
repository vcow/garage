using UniRx;

namespace Logic
{
	public interface IGameLogic
	{
		IReadOnlyReactiveProperty<int> NumInteractableObjects { get; }
		IReadOnlyReactiveProperty<int> NumGatheredInteractableObjects { get; }
		void RegisterInteractableObject(DropInteractableObject interactableObject);
	}
}