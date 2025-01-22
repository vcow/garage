using UnityEngine;
using Zenject;

[DisallowMultipleComponent]
public sealed class ProjectInstaller : MonoInstaller<ProjectInstaller>
{
	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);
	}
}