using Signals;
using UnityEngine;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public sealed class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<InteractableHitSignal>();
			Container.DeclareSignal<InteractableLoseSignal>();
			Container.DeclareSignal<CanInteractSignal>();
			Container.DeclareSignal<InteractSignal>();
		}
	}
}