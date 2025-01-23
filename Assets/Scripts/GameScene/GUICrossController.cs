using Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GUICrossController : MonoBehaviour
	{
		[SerializeField] private Image _cross;
		[SerializeField] private Color _activeColor;
		[SerializeField] private Color _inactiveColor;

		[Inject] private SignalBus _signalBus;

		private void Start()
		{
			_cross.color = _inactiveColor;
			_signalBus.Subscribe<CanInteractSignal>(OnCanInteract);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<CanInteractSignal>(OnCanInteract);
		}

		private void OnCanInteract(CanInteractSignal signal)
		{
			_cross.color = signal.CanInteract ? _activeColor : _inactiveColor;
		}
	}
}