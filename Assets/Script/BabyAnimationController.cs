using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class BabyAnimationController : MonoBehaviour
    {
        [Tooltip("Reference to the Animator component controlling the baby animations.")]
        public Animator animator;
        [SerializeField] private float crossFadeDuration = 0.25f;

        public enum BabyAnimations
        {
            Crawl,
            CrawlToSit,
            PickBall,
            SitToCrawl
        }

        // Precomputed hash values for each animation state.
        private static readonly Dictionary<BabyAnimations, int> animationHashes = new Dictionary<BabyAnimations, int>()
        {
            { BabyAnimations.Crawl, Animator.StringToHash("Crawl") },
            { BabyAnimations.CrawlToSit, Animator.StringToHash("CrawlToSit") },
            { BabyAnimations.PickBall, Animator.StringToHash("PickBall") },
            { BabyAnimations.SitToCrawl, Animator.StringToHash("SitToCrawl") }
        };
        
        private BabyAnimations currentAnimation = BabyAnimations.CrawlToSit;

        public void PlayCrawl()
        {
            PlayAnimation(BabyAnimations.Crawl);
        }

        public void PlayPickBall()
        {
            PlayAnimation(BabyAnimations.PickBall);
        }

        public void PlaySit()
        {
            PlayAnimation(BabyAnimations.SitToCrawl);
        }
        /// <summary>
        /// Uses crossfade to play the specified animation.
        /// It avoids calling the same animation repeatedly.
        /// </summary>
        /// <param name="animation">The animation state (from the enum) to play.</param>
        public void PlayAnimation(BabyAnimations animation)
        {
            // Check if the requested animation is already playing.
            if (currentAnimation == animation)
            {
                return;
            }

            if (!animationHashes.TryGetValue(animation, out int animHash))
            {
                Debug.LogError("Animation hash not found for: " + animation);
                return;
            }

            // Update the current animation and perform the cross fade.
            currentAnimation = animation;
            Debug.Log(currentAnimation);
            animator.CrossFade(animHash, crossFadeDuration);
        }
    }
}
