using ModestTree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Character
{
	[DisallowMultipleComponent, RequireComponent(typeof(NavMeshAgent))]
	public sealed class CharacterController : MonoBehaviour
	{
		[SerializeField] private Transform _lookAtTarget;
		[SerializeField] private float _rotationSpeed = 1f;

		private NavMeshAgent _navMeshAgent;
		private Vector3 _targetDirection;
		private Vector3 _movementVector;
		private Vector3 _rotationDelta;
		private Camera _camera;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			Assert.IsNotNull(_camera, "Main Camera must have.");
		}

		public void OnMovementAction(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<Vector2>();
			_movementVector = Vector3.Normalize(new Vector3(value.x, 0f, value.y));
		}

		public void OnRotationAction(InputAction.CallbackContext context)
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
			
			_navMeshAgent.Move(rotatedMovementVector * (_navMeshAgent.speed * Time.deltaTime));
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_lookAtTarget, "_lookAtTarget != null");
		}
	}
}