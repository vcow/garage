using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Logic
{
	[DisallowMultipleComponent]
	public sealed class GameLogicCarController : MonoInstaller<GameLogicCarController>, IGameLogic
	{
		private readonly CompositeDisposable _disposables = new();
		private readonly IntReactiveProperty _numInteractableObjects = new(0);
		private readonly IntReactiveProperty _numGatheredInteractableObjects = new(0);

		private readonly HashSet<int> _registered = new();
		private readonly HashSet<int> _gathered = new();

		public override void InstallBindings()
		{
			Container.Bind<IGameLogic>().FromInstance(this).AsSingle();
		}

		public IReadOnlyReactiveProperty<int> NumInteractableObjects => _numInteractableObjects;
		public IReadOnlyReactiveProperty<int> NumGatheredInteractableObjects => _numGatheredInteractableObjects;

		public override void Start()
		{
			_disposables.Add(_numInteractableObjects);
			_disposables.Add(_numGatheredInteractableObjects);
		}

		private void OnDestroy()
		{
			_disposables.Dispose();
		}

		public void RegisterInteractableObject(DropInteractableObject interactableObject)
		{
			if (!_registered.Add(interactableObject.InteractableId))
			{
				throw new Exception("Try to register interactable object twice.");
			}

			_numInteractableObjects.Value = _registered.Count;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer != LayerMask.NameToLayer("Interactable"))
			{
				return;
			}

			var interactable = other.GetComponentInParent<DropInteractableObject>();
			if (interactable)
			{
				_gathered.Add(interactable.InteractableId);
				_numGatheredInteractableObjects.Value = _gathered.Count;
				Assert.IsTrue(_gathered.Count <= _registered.Count, "Some interactable objects wasn't registered.");
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.layer != LayerMask.NameToLayer("Interactable"))
			{
				return;
			}

			var interactable = other.GetComponentInParent<DropInteractableObject>();
			if (interactable)
			{
				_gathered.Remove(interactable.InteractableId);
				_numGatheredInteractableObjects.Value = _gathered.Count;
			}
		}
	}
}