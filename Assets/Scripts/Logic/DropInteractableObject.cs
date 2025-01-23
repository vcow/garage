using Character;
using UnityEngine;
using Zenject;

namespace Logic
{
	public sealed class DropInteractableObject : BaseInteractableObject
	{
		[SerializeField] private float _spring = 1000;
		[SerializeField] private float _breakForce = 5000;
		[SerializeField] private float _damper = 200;

		[Inject] private readonly IGameLogic _gameLogic;

		private static int _ctr;

		private ConfigurableJoint _joint;

		public int InteractableId { get; } = _ctr++;

		protected override void Start()
		{
			_gameLogic.RegisterInteractableObject(this);
			base.Start();
		}

		protected override void Interact(Vector3 hitPoint, GameObject character)
		{
			if (_joint)
			{
				Destroy(_joint);
				return;
			}

			var connectPositionLocal = hitPoint - transform.position;
			var characterAimController = character.GetComponent<CharacterAimController>();
			_joint = gameObject.AddComponent<ConfigurableJoint>();
			_joint.anchor = connectPositionLocal;
			characterAimController.Rigidbody.transform.position = hitPoint;
			_joint.connectedBody = characterAimController.Rigidbody;
			_joint.breakForce = _breakForce;
			_joint.xMotion = ConfigurableJointMotion.Limited;
			_joint.yMotion = ConfigurableJointMotion.Limited;
			_joint.zMotion = ConfigurableJointMotion.Limited;
			_joint.angularXMotion = ConfigurableJointMotion.Limited;
			_joint.angularYMotion = ConfigurableJointMotion.Limited;
			_joint.angularZMotion = ConfigurableJointMotion.Limited;
			_joint.angularXLimitSpring = new SoftJointLimitSpring { spring = _spring, damper = _damper };
			_joint.highAngularXLimit = new SoftJointLimit { limit = 1f };
			_joint.lowAngularXLimit = new SoftJointLimit { limit = 0.5f };
		}
	}
}