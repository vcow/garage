using Logic;
using Signals;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Zenject;

namespace Character
{
	[DisallowMultipleComponent]
	public sealed class CharacterAimController : MonoBehaviour
	{
		[field: SerializeField] public Rigidbody Rigidbody { get; private set; }
		[SerializeField] private Transform _lookAtTarget;
		[SerializeField] private float _touchDistanse;

		[Inject] private readonly SignalBus _signalBus;

		private Camera _camera;
		private BaseInteractableObject _interactableObject;
		private bool _canInteract;
		private Vector3 _hitPoint;

		private void Awake()
		{
			_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			Assert.IsNotNull(_camera, "Main Camera must have.");
		}

		private void Update()
		{
			var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 10f));
			if (!Physics.Raycast(ray, out var hitInfo, _camera.farClipPlane, LayerMask.GetMask("Interactable", "Obstacle")) ||
			    hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				if (_interactableObject)
				{
					if (_canInteract)
					{
						_canInteract = false;
						_signalBus.TryFire(new CanInteractSignal(null));
					}

					OnLostInteractable();
				}

				return;
			}

			var newInteractableObject = hitInfo.collider.GetComponent<BaseInteractableObject>();
			if (!newInteractableObject)
			{
				Debug.LogError("Interactable object hasn't BaseInteractableObject component.");
				if (!_interactableObject)
				{
					return;
				}
			}
			else
			{
				_hitPoint = hitInfo.point;
				var canInteract = CheckCanInteraction(_hitPoint, newInteractableObject);
				if (canInteract && !_canInteract)
				{
					_signalBus.TryFire(new CanInteractSignal(newInteractableObject));
				}
				else if (!canInteract && _canInteract)
				{
					_signalBus.TryFire(new CanInteractSignal(null));
				}

				_canInteract = canInteract;
			}

			if (_interactableObject)
			{
				if (newInteractableObject == _interactableObject)
				{
					return;
				}

				OnLostInteractable();
			}

			_interactableObject = newInteractableObject;
			if (_interactableObject)
			{
				OnHitInteractable();
			}
		}

		public void OnTapAction(InputAction.CallbackContext context)
		{
			if (context.phase != InputActionPhase.Started || !_canInteract || !_interactableObject)
			{
				return;
			}

			_signalBus.TryFire(new InteractSignal(_interactableObject, gameObject, _hitPoint));
		}

		private bool CheckCanInteraction(Vector3 hitPoint, BaseInteractableObject interactableObject)
		{
			if (!interactableObject)
			{
				return false;
			}

			var l = hitPoint - _camera.transform.position;
			return _touchDistanse * _touchDistanse >= l.sqrMagnitude;
		}

		private void OnLostInteractable()
		{
			Assert.IsNotNull(_interactableObject);
			_interactableObject = null;
			_signalBus.TryFire<InteractableLoseSignal>();
		}

		private void OnHitInteractable()
		{
			Assert.IsNotNull(_interactableObject);
			_signalBus.TryFire(new InteractableHitSignal(_interactableObject));
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_lookAtTarget, "_lookAtTarget != null");
		}
	}
}