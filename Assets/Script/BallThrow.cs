using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Script
{
    public class BallThrow : MonoBehaviour
    {
        private Vector2 _startPos, _endPos, _direction; // Touch start, end, and swipe direction
        private float _touchTimeStart, _touchTimeFinish, _timeInterval; // For calculating swipe duration

        [SerializeField] private float throwForceInXandY = 1f;
        [SerializeField] private float throwForceInZ = 50f;

        private Rigidbody _rb;
        [SerializeField] private bool ballThrown;
        [SerializeField] private Transform ballPosition;

        #region Singleton

        public static BallThrow instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning("There is an instance of Baby state manager");
                Destroy(gameObject);
            }
        }

        #endregion

        private void OnEnable()
        {
            // Enable Enhanced Touch support
            EnhancedTouchSupport.Enable();

            // Subscribe to finger events
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += OnFingerUp;
        }

        private void OnDisable()
        {
            // Unsubscribe from finger events
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerUp -= OnFingerUp;

            // Disable Enhanced Touch support
            EnhancedTouchSupport.Disable();
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Reset()
        {
            _rb.isKinematic = true;
            StartCoroutine(SmoothReset());
        }

        /// <summary>
        ///     Resets the scene by reloading the currently active scene.
        /// </summary>
        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator SmoothReset()
        {
            const float duration = 1f; // Duration of the smooth transition (in seconds)
            var elapsed = 0f;
            Vector3 startPos = transform.position;
            Vector3 targetPos = ballPosition.position;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            ballThrown = false;


            // Ensure the position is exactly set at the end
            transform.position = targetPos;
        }


        private void OnFingerDown(Finger finger)
        {
            // Optionally, only consider the primary finger (index 0)
            if (finger.index != 0) return;

            Debug.Log("Touch Started");
            _touchTimeStart = Time.time;
            _startPos = finger.screenPosition;
        }

        private void OnFingerUp(Finger finger)
        {
            if (finger.index != 0) return;

            Debug.Log("Touch Ended");

            if (ballThrown)
                return;

            _touchTimeFinish = Time.time;
            _timeInterval = _touchTimeFinish - _touchTimeStart;
            _endPos = finger.screenPosition;
            _direction = _startPos - _endPos;

            // Ignore if no movement was detected
            if (_direction == Vector2.zero)
                return;

            // Calculate swipe speed: distance divided by time (avoid division by zero)
            float swipeSpeed = Mathf.Clamp(_direction.magnitude / Mathf.Max(_timeInterval, 0.1f),0,500);
            
            // Optionally, output swipe speed for debugging
            Debug.Log($"Swipe Speed: {swipeSpeed}");

            _rb.isKinematic = false;
            _rb.AddForce(-_direction.x * throwForceInXandY,
                -_direction.y * throwForceInXandY,
                throwForceInZ / _timeInterval);
            ballThrown = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground")) BabyStateManager.instance.ActivateBallChase();
        }
    }
}
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class SwipeScript : MonoBehaviour
// {
//     private Vector2 _startPos, _endPos, _direction; // touch start position, touch end position, swipe direction
//     private float _touchTimeStart, _touchTimeFinish, _timeInterval; // to calculate swipe time to control throw force in Z direction
//
//     // ReSharper disable once IdentifierTypo
//     [SerializeField] private float throwForceInXandY = 1f;
//     [SerializeField] private float throwForceInZ = 50f;
//
//     private Rigidbody _rb;
//     [SerializeField] private bool ballThrown;
//     [SerializeField] private Transform ballPosition;
//
//     private PlayerInputActions _playerInput;
//     private InputAction _touchAction;
//
//     private void Awake()
//     {
//         _playerInput = new();
//     }
//
//     private void OnEnable()
//     {
//         _touchAction = _playerInput.Touch.TouchPress;
//         _touchAction.Enable();
//         _touchAction.started += OnTouchStart;
//         _touchAction.performed += OnTouchEnd;
//     }
//
//     private void OnDisable()
//     {
//         _touchAction.started -= OnTouchStart;
//         _touchAction.performed -= OnTouchEnd;
//         _touchAction.Disable();
//     }
//
//     private void Start()
//     {
//         _rb = GetComponent<Rigidbody>();
//     }
//
//     public void Reset()
//     {
//         transform.position = ballPosition.position;
//         ballThrown = false;
//         _rb.isKinematic = true;
//     }
//
//     private void OnTouchStart(InputAction.CallbackContext context)
//     {
//         Debug.Log("Touch Started");
//         _touchTimeStart = Time.time;
//         _startPos = context.ReadValue<Vector2>();
//     }
//
//     private void OnTouchEnd(InputAction.CallbackContext context)
//     {
//         Debug.Log("Touch Ended");
//
//         if (ballThrown) return;
//
//         _touchTimeFinish = Time.time;
//         _timeInterval = _touchTimeFinish - _touchTimeStart;
//         _endPos = context.ReadValue<Vector2>();
//         _direction = _startPos - _endPos;
//
//         if (_direction == Vector2.zero) return;
//
//         _rb.isKinematic = false;
//         _rb.AddForce(-_direction.x * throwForceInXandY, -_direction.y * throwForceInXandY, throwForceInZ / _timeInterval);
//         ballThrown = true;
//     }
// }

// using UnityEngine;
//
// public class SwipeScript : MonoBehaviour {
//
// 	Vector2 startPos, endPos, direction; // touch start position, touch end position, swipe direction
// 	float touchTimeStart, touchTimeFinish, timeInterval; // to calculate swipe time to sontrol throw force in Z direction
//
// 	[SerializeField] private float throwForceInXandY = 1f; // to control throw force in X and Y directions
//
// 	[SerializeField] private float throwForceInZ = 50f; // to control throw force in Z direction
//
// 	private Rigidbody _rb;
// 	[SerializeField] private bool ballThrown;
// 	[SerializeField] private Transform ballPosition;
// 	private void Start()
// 	{
// 		_rb = GetComponent<Rigidbody> ();
// 	}
//
// 	public void Reset()
// 	{
// 		transform.position = ballPosition.position;
// 		ballThrown = false;
// 		_rb.isKinematic = true;
// 	}
//
// 	// Update is called once per frame
// 	private void Update () {
//
// 		// if you touch the screen
// 		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
//
// 			// getting touch position and marking time when you touch the screen
// 			touchTimeStart = Time.time;
// 			startPos = Input.GetTouch (0).position;
// 		}
//
// 		// if you release your finger
// 		if (Input.touchCount <= 0 || Input.GetTouch(0).phase != TouchPhase.Ended || ballThrown) return;
// 		Debug.Log("ball thrown");
// 		// marking time when you release it
// 		touchTimeFinish = Time.time;
//
// 		// calculate swipe time interval 
// 		timeInterval = touchTimeFinish - touchTimeStart;
//
// 		// getting release finger position
// 		endPos = Input.GetTouch (0).position;
//
// 		// calculating swipe direction in 2D space
// 		direction = startPos - endPos;
// 		Debug.Log(direction);
// 		
// 		if(direction == Vector2.zero) return;
// 		
// 		// add force to balls rigidbody in 3D space depending on swipe time, direction and throw forces
// 		_rb.isKinematic = false;
// 		_rb.AddForce (- direction.x * throwForceInXandY, - direction.y * throwForceInXandY, throwForceInZ / timeInterval);
// 		ballThrown = true;
// 	}
// }