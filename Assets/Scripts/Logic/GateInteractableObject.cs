using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
	public sealed class GateInteractableObject : BaseInteractableObject
	{
		private const float OpenDuration = 1f;
		private const float CloseDuration = 1f;

		[SerializeField] private float _closeAngle;
		[SerializeField] private float _openAngle;
		[SerializeField] private GateInteractableObject _parent;
		[SerializeField] private NavMeshObstacle[] _obstacles;

		private readonly CompositeDisposable _disposables = new();
		private readonly BoolReactiveProperty _isOpened = new(false);
		private Tween _tween;

		public IReadOnlyReactiveProperty<bool> IsOpened => _isOpened;

		protected override void Start()
		{
			base.Start();
			if (_parent)
			{
				_parent.IsOpened.Subscribe(parentIsOpened =>
					{
						if (parentIsOpened && !_isOpened.Value)
						{
							Interact(null);
						}
					})
					.AddTo(_disposables);
			}
		}

		protected override void Interact(GameObject _)
		{
			if (_parent)
			{
				if (_isOpened.Value && _parent.IsOpened.Value)
				{
					return;
				}
			}

			_isOpened.Value = !_isOpened.Value;
			float ang, duration;
			if (_isOpened.Value)
			{
				ang = _openAngle;
				duration = OpenDuration;
				foreach (var obstacle in _obstacles)
				{
					obstacle.gameObject.SetActive(false);
				}
			}
			else
			{
				ang = _closeAngle;
				duration = CloseDuration;
				foreach (var obstacle in _obstacles)
				{
					obstacle.gameObject.SetActive(true);
				}
			}

			_tween?.Kill();
			_tween = transform.DOLocalRotate(new Vector3(0f, ang, 0f), duration).SetEase(Ease.InOutQuad)
				.OnComplete(() => _tween = null);
		}

		protected override void OnDestroy()
		{
			_disposables.Dispose();
			_tween?.Kill();
			base.OnDestroy();
		}
	}
}