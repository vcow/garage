using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent, RequireComponent(typeof(Canvas))]
	public class InteractableMarkerController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _interactableName;

		[Inject] private SignalBus _signalBus;

		private Vector3 _labelOffset;
		private Transform _interactable;
		private Camera _camera;

		private void Awake()
		{
			var canvas = GetComponent<Canvas>();
			_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			canvas.worldCamera = _camera;
		}

		private void Start()
		{
			_interactableName.gameObject.SetActive(false);

			_signalBus.Subscribe<InteractableHitSignal>(OnInteractableHit);
			_signalBus.Subscribe<InteractableLoseSignal>(OnInteractableLose);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<InteractableHitSignal>(OnInteractableHit);
			_signalBus.Unsubscribe<InteractableLoseSignal>(OnInteractableLose);
		}

		private void Update()
		{
			if (_interactable)
			{
				ValidateLabelPosition();
			}
		}

		private void OnInteractableHit(InteractableHitSignal signal)
		{
			_interactable = signal.InteractableObject.transform;
			_labelOffset = _interactable.gameObject.GetBounds().center - _interactable.transform.position;
			_interactableName.text = signal.InteractableObject.Name;
			_interactableName.gameObject.SetActive(true);
			ValidateLabelPosition();
		}

		private void OnInteractableLose()
		{
			_interactable = null;
			_interactableName.gameObject.SetActive(false);
		}

		private void ValidateLabelPosition()
		{
			Assert.IsNotNull(_interactable);
			var position = _interactable.position + _labelOffset;
			var viewportPoint = _camera.WorldToViewportPoint(position);
			var sizeDelta = ((RectTransform)transform).sizeDelta;
			var timerPosition = new Vector2(viewportPoint.x * sizeDelta.x - sizeDelta.x * 0.5f,
				viewportPoint.y * sizeDelta.y - sizeDelta.y * 0.5f);
			_interactableName.rectTransform.anchoredPosition = timerPosition;
		}

		private void OnValidate()
		{
			Assert.IsNotNull(_interactableName, "_interactableName != null");
		}
	}
}