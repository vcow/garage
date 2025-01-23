using Logic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GUICaptionController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _counter;

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
				.Subscribe(ints => _counter.text = $"{ints[0]}/{ints[1]}")
				.AddTo(_disposables);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_counter, "_counter != null");
		}
	}
}