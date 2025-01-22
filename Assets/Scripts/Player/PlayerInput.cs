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

		private InputActionMap _inputActionMap;
		private InputAction _movement;
		private NavMeshAgent _navMeshAgent;
		private Vector3 _targetDirection;
		private Vector3 _lastDirection;
		private Vector3 _movementVector;
		private float _lerpTime;
		private Transform _transform;

		private void Awake()
		{
			_transform = transform;
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_inputActionMap = _inputActionAsset.FindActionMap("Navigation");
			Assert.IsNotNull(_inputActionAsset, "Can't find action map Navigation.");
			_movement = _inputActionMap.FindAction("Movement");
			Assert.IsNotNull(_movement, "Can't find action map Movement.");
		}

		private void Start()
		{
			Observable.FromEvent<InputAction.CallbackContext>(h => _movement.started += h, h => _movement.started -= h).Subscribe(OnMovementAction).AddTo(_disposables);
			Observable.FromEvent<InputAction.CallbackContext>(h => _movement.canceled += h, h => _movement.canceled -= h).Subscribe(OnMovementAction).AddTo(_disposables);
			Observable.FromEvent<InputAction.CallbackContext>(h => _movement.performed += h, h => _movement.performed -= h).Subscribe(OnMovementAction).AddTo(_disposables);

			_movement.Enable();
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

		private void Update()
		{
			if (_movementVector != _lastDirection)
			{
				_lerpTime = 0;
			}

			_lastDirection = _movementVector;
			var interpolation = Mathf.Clamp01(_lerpTime * _targetLerpSpeed * (1f - _smoothing));
			_targetDirection = Vector3.Lerp(_targetDirection, _movementVector, interpolation);
			_navMeshAgent.Move(_targetDirection * _navMeshAgent.speed * Time.deltaTime);

			var lookDirection = _movementVector;
			if (lookDirection.sqrMagnitude > 0.00001f)
			{
				_transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(lookDirection), interpolation);
			}

			_lerpTime += Time.deltaTime;
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_inputActionAsset, "_inputActionAsset != null");
		}
	}
}