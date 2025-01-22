using ModestTree;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
	[DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
	public sealed class PlayerInput : MonoBehaviour
	{
		private readonly CompositeDisposable _disposables = new();

		[SerializeField] private InputActionAsset _inputActionAsset;
		[SerializeField, Range(0f, 0.99f)] private float _smoothing = 0.25f;
		[SerializeField] private float _targetLerpSpeed = 1f;
		[SerializeField] private Transform _lookAtTarget;
		[SerializeField] private float _rotationSpeed = 1f;

		private InputActionMap _inputActionMap;
		private InputAction _movement;
		private InputAction _rotation;
		private NavMeshAgent _navMeshAgent;
		private Vector3 _targetDirection;
		private Vector3 _lastDirection;
		private Vector3 _movementVector;
		private Vector3 _rotationDelta;
		private float _lerpTime;
		private Camera _camera;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();

			_inputActionMap = _inputActionAsset.FindActionMap("Navigation");
			Assert.IsNotNull(_inputActionAsset, "Can't find action map Navigation.");

			_movement = _inputActionMap.FindAction("Movement");
			Assert.IsNotNull(_movement, "Can't find action map Movement.");

			_rotation = _inputActionMap.FindAction("Rotation");
			Assert.IsNotNull(_rotation, "Can't find action map Rotation.");

			_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			Assert.IsNotNull(_camera, "Main Camera must have.");
		}

		private void Start()
		{
			Observable.FromEvent<InputAction.CallbackContext>(h => _movement.started += h, h => _movement.started -= h).Subscribe(OnMovementAction).AddTo(_disposables);
			Observable.FromEvent<InputAction.CallbackContext>(h => _movement.canceled += h, h => _movement.canceled -= h).Subscribe(OnMovementAction).AddTo(_disposables);
			Observable.FromEvent<InputAction.CallbackContext>(h => _movement.performed += h, h => _movement.performed -= h).Subscribe(OnMovementAction).AddTo(_disposables);

			Observable.FromEvent<InputAction.CallbackContext>(h => _rotation.started += h, h => _rotation.started -= h).Subscribe(OnRotationAction).AddTo(_disposables);
			Observable.FromEvent<InputAction.CallbackContext>(h => _rotation.canceled += h, h => _rotation.canceled -= h).Subscribe(OnRotationAction).AddTo(_disposables);
			Observable.FromEvent<InputAction.CallbackContext>(h => _rotation.performed += h, h => _rotation.performed -= h).Subscribe(OnRotationAction).AddTo(_disposables);

			_movement.Enable();
			_rotation.Enable();
			_inputActionMap.Enable();
			_inputActionAsset.Enable();
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		private void OnMovementAction(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<Vector2>();
			_movementVector = Vector3.Normalize(new Vector3(value.x, 0f, value.y));
		}

		private void OnRotationAction(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<Vector2>();
			_rotationDelta = _rotationSpeed * new Vector3(-value.y, value.x, 0);
		}

		private void Update()
		{
			var tookAtTransform = _lookAtTarget.transform;
			tookAtTransform.eulerAngles += _rotationDelta;
			var lookAtRotation = tookAtTransform.rotation.eulerAngles.y;
			var rotatedMovementVector = Quaternion.AngleAxis(lookAtRotation, Vector3.up) * _movementVector;
			
			// if (_movementVector != _lastDirection)
			// {
				// _lerpTime = 0;
			// }

			_lastDirection = rotatedMovementVector;
			var interpolation = Mathf.Clamp01(_lerpTime * _targetLerpSpeed * (1f - _smoothing));
			_targetDirection = Vector3.Lerp(_targetDirection, rotatedMovementVector, interpolation);
			_navMeshAgent.Move(_targetDirection * (_navMeshAgent.speed * Time.deltaTime));
			_lerpTime += Time.deltaTime;
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_inputActionAsset, "_inputActionAsset != null");
			Assert.IsNotNull(_lookAtTarget, "_lookAtTarget != null");
		}
	}
}