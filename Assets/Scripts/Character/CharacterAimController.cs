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
		[SerializeField] private Transform _lookAtTarget;
		[SerializeField] private float _touchDistanse;

		[Inject] private readonly SignalBus _signalBus;

		private Camera _camera;
		private BaseInteractableObject _interactableObject;

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
					OnLostInteractable();
				}

				return;
			}

			if (_interactableObject)
			{
				OnLostInteractable();
			}

			_interactableObject = hitInfo.collider.GetComponent<BaseInteractableObject>();
			if (_interactableObject)
			{
				OnHitInteractable();
			}
			else
			{
				Debug.LogError("Interactable object hasn't BaseInteractableObject component.");
			}
		}

		public void OnTapAction(InputAction.CallbackContext context)
		{
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