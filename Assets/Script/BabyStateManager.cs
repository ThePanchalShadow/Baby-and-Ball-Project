using System;
using UnityEngine;

namespace Script
{
    public class BabyStateManager : MonoBehaviour
    {
        public enum BabyState { Idle, Chase, PickBall, Deliver, ReturnToIdle }
        public BabyState currentState = BabyState.Idle;

        [Header("References")]
        [Tooltip("Reference to the ball transform")]
        public Transform ball;
        private Rigidbody ballRigidbody;
        [Tooltip("Reference to the player's location transform where the ball is to be delivered")]
        public Transform ballSubmitLocation;
        [Tooltip("Reference to the baby's hand transform where the ball will be attached")]
        public Transform hand;
        [Tooltip("Reference to the baby animation controller")]
        public BabyAnimationController babyAnimationController;

        [Header("Movement Settings")]
        [Tooltip("Distance within which the baby can catch the ball")]
        public float catchRange = 1f;
        [Tooltip("Acceleration factor for crawling speed (proportional to distance)")]
        public float acceleration = 1f;
        [Tooltip("Maximum crawl speed")]
        public float maxSpeed = 5f;

        [Header("Rotation Settings")]
        [Tooltip("Rotation speed for smoothly turning towards the movement direction")]
        public float rotationSpeed = 5f;

        // Store the baby's original starting position (idle spot)
        private Vector3 originalPosition;

        #region Singleton
        public static BabyStateManager instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.LogWarning("There is an instance of BabyStateManager already.");
                Destroy(gameObject);
            }
        }
        #endregion

        private void Start()
        {
            originalPosition = transform.position;
            currentState = BabyState.Idle;
            ballRigidbody = ball.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            switch (currentState)
            {
                case BabyState.Idle:
                    // When idle, have the baby rotate to face the player.
                    if (ballSubmitLocation != null)
                    {
                        Vector3 directionToPlayer = ballSubmitLocation.position - transform.position;
                        SmoothRotate(directionToPlayer);
                    }
                    break;
                case BabyState.Chase:
                    ChaseBall();
                    break;
                case BabyState.PickBall:
                    // Handled immediately in PickUpBall()
                    break;
                case BabyState.Deliver:
                    DeliverBall();
                    break;
                case BabyState.ReturnToIdle:
                    ReturnHome();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Call this function (e.g., from a button press or event) to initiate the ball chase.
        /// </summary>
        public void ActivateBallChase()
        {
            if (currentState != BabyState.Idle)
                return;

            currentState = BabyState.Chase;
            babyAnimationController.PlayAnimation(BabyAnimationController.BabyAnimations.Crawl);
        }

        /// <summary>
        /// Moves the baby towards the ball. When the baby is within catchRange, it transitions to picking up the ball.
        /// </summary>
        private void ChaseBall()
        {
            babyAnimationController.PlayAnimation(BabyAnimationController.BabyAnimations.Crawl);
            
            if (!MoveBabyTowards(ball.position, catchRange)) return;
            currentState = BabyState.PickBall;
            PickUpBall();
        }

        /// <summary>
        /// Handles picking up the ball: plays the pick ball animation and attaches the ball to the baby's hand.
        /// Then transitions to the Deliver state.
        /// </summary>
        private void PickUpBall()
        {
            babyAnimationController.PlayAnimation(BabyAnimationController.BabyAnimations.PickBall);

            if (ball && hand)
            {
                ballRigidbody.isKinematic = true;
                ball.SetParent(hand);
                ball.localPosition = Vector3.zero;
            }

            currentState = BabyState.Deliver;
        }

        /// <summary>
        /// Moves the baby towards the player's location to deliver the ball.
        /// Once close enough, detaches the ball and transitions to ReturnToIdle.
        /// </summary>
        private void DeliverBall()
        {
            babyAnimationController.PlayAnimation(BabyAnimationController.BabyAnimations.Crawl);
            if (!MoveBabyTowards(ballSubmitLocation.position, 1f)) return;
            
            if (ball)
            {
                ball.SetParent(null);
                BallThrow.instance.Reset();
            }
            currentState = BabyState.ReturnToIdle;
        }

        /// <summary>
        /// Moves the baby back to its original idle position.
        /// When it arrives, plays the crawl-to-sit animation and sets state to Idle.
        /// </summary>
        private void ReturnHome()
        {
            babyAnimationController.PlayAnimation(BabyAnimationController.BabyAnimations.Crawl);
            
            if (!MoveBabyTowards(originalPosition, 2f)) return;
            babyAnimationController.PlayAnimation(BabyAnimationController.BabyAnimations.CrawlToSit);
            currentState = BabyState.Idle;
        }

        /// <summary>
        /// Moves the baby towards a target position.
        /// Returns true when the baby is within the specified threshold of the target.
        /// </summary>
        /// <param name="target">The target position.</param>
        /// <param name="threshold">How close is considered "arrived".</param>
        /// <returns>True if arrived at the target, otherwise false.</returns>
        private bool MoveBabyTowards(Vector3 target, float threshold)
        {
            Vector3 direction = target - transform.position;
            float distance = direction.magnitude;
            float speed = Mathf.Clamp(acceleration * distance, 0, maxSpeed);
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            SmoothRotate(direction);
            return distance <= threshold;
        }

        /// <summary>
        /// Smoothly rotates the baby towards the given direction.
        /// </summary>
        /// <param name="direction">The direction to rotate towards.</param>
        private void SmoothRotate(Vector3 direction)
        {
            if (direction == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
