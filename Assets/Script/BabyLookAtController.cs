using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Script
{
    public class BabyLookAtController : MonoBehaviour
    {
        [Header("Head Settings")]
        [Tooltip("Reference to the baby head bone that should look at the tapped point.")]
        public Transform headBone;
        [Tooltip("Speed multiplier for head rotation.")]
        public float headRotationSpeed = 5f;
        [Tooltip("Duration (in seconds) for the head rotation to complete.")]
        public float rotationDuration = 1f;
        [Tooltip("Time to hold the tapped look direction before returning to default rotation.")]
        public float holdDuration = 1f;

        public Animator animator;
        private Camera _mainCamera;
        // Store the default head rotation (the rotation the head should return to)
        private Quaternion _defaultHeadRotation;

        private void Start()
        {
            _mainCamera = Camera.main;
            if (headBone)
            {
                _defaultHeadRotation = headBone.rotation;
            }
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += HandleFingerDown;
        }

        private void OnDisable()
        {
            Touch.onFingerDown -= HandleFingerDown;
            EnhancedTouchSupport.Disable();
        }

        private void HandleFingerDown(Finger finger)
        {
            // Create a ray from the camera using the finger's screen position.
            Ray ray = _mainCamera.ScreenPointToRay(finger.screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            Debug.Log($"{hit.transform.gameObject.name}");
            // Check if the hit collider belongs to the baby (bone-based colliders should be children).
            if (!hit.transform.IsChildOf(transform)) return;
            Debug.Log($"{hit.transform.gameObject.name} is child of baby");
            StartCoroutine(RotateHeadTowards(hit.point));
        }

        /// <summary>
        /// Smoothly rotates the headBone so it looks at the targetPoint,
        /// then after holding the look direction, rotates back to its default rotation.
        /// </summary>
        /// <param name="targetPoint">The world-space point to look at.</param>
        private IEnumerator RotateHeadTowards(Vector3 targetPoint)
        {
            if (!headBone)
            {
                Debug.LogWarning("HeadBone reference is not set.");
                yield break;
            }

            // Disable animator to prevent it from overriding our manual rotation.
            animator.enabled = false;

            // Use the stored default rotation.
            Quaternion originalRotation = _defaultHeadRotation;

            Vector3 direction = targetPoint - headBone.position;
            if (direction == Vector3.zero)
            {
                animator.enabled = true;
                yield break;
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Rotate head to look at the target point.
            var elapsed = 0f;
            while (elapsed < rotationDuration)
            {
                elapsed += Time.deltaTime * headRotationSpeed;
                headBone.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsed / rotationDuration);
                yield return null;
            }
            headBone.rotation = targetRotation;

            // Hold the target-look direction.
            yield return new WaitForSeconds(holdDuration);

            // Rotate head back to its default rotation.
            elapsed = 0f;
            while (elapsed < rotationDuration)
            {
                elapsed += Time.deltaTime * headRotationSpeed;
                headBone.rotation = Quaternion.Slerp(targetRotation, originalRotation, elapsed / rotationDuration);
                yield return null;
            }
            headBone.rotation = originalRotation;

            // Re-enable animator.
            animator.enabled = true;
        }

        public void MoveCameraTo(Transform target)
        {
            StartCoroutine(MoveCameraTo(target, 0.4f));
            // _mainCamera.transform.position = target.position;
            // _mainCamera.transform.rotation = target.rotation;
        }

        private IEnumerator MoveCameraTo(Transform target, float duration)
        {
            Vector3 startPos = _mainCamera.transform.position;
            Quaternion startRot = _mainCamera.transform.rotation;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _mainCamera.transform.position = Vector3.Lerp(startPos, target.position, t);
                _mainCamera.transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);
                yield return null;
            }
        
            // Ensure the final position and rotation exactly match the target.
            _mainCamera.transform.position = target.position;
            _mainCamera.transform.rotation = target.rotation;
        }
    }
}
