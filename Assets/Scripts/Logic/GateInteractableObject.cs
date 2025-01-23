using DG.Tweening;
using UnityEngine;

namespace Logic
{
	public sealed class GateInteractableObject : BaseInteractableObject
	{
		private const float OpenDuration = 1f;
		private const float CloseDuration = 1f;

		[SerializeField] private float _closeAngle;
		[SerializeField] private float _openAngle;

		private Tween _tween;

		public bool IsOpened { get; private set; }

		protected override void Interact(GameObject _)
		{
			var ang = IsOpened ? _closeAngle : _openAngle;
			var duration = IsOpened ? CloseDuration : OpenDuration;
			_tween?.Kill();
			_tween = transform.DOLocalRotate(new Vector3(0f, ang, 0f), duration).SetEase(Ease.InOutQuad)
				.OnComplete(() => _tween = null);
			IsOpened = !IsOpened;
		}

		private void OnDestroy()
		{
			_tween?.Kill();
		}
	}
}