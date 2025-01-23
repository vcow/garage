using Logic;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GUIWinWindow : MonoBehaviour
	{
		[Inject] private readonly IGameLogic _gameLogic;

		private readonly CompositeDisposable _disposables = new();

		private void Start()
		{
			new[]
				{
					_gameLogic.NumGatheredInteractableObjects,
					_gameLogic.NumInteractableObjects
				}
				.CombineLatest()
				.Select(ints => ints[0] >= ints[1])
				.Subscribe(b => gameObject.SetActive(b))
				.AddTo(_disposables);
			gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}
	}
}