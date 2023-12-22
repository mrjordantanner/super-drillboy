using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts
{
    // Based on Controller2D by Sebastian Lague
    public class Controller2D : MonoBehaviour
    {
        [Header("Settings")]
        public LayerMask collisionMask;

        public const float skinWidth = .05f;
        const float dstBetweenRays = .25f;
        [HideInInspector]
        public int horizontalRayCount, verticalRayCount;
        [HideInInspector]
        public float horizontalRaySpacing, verticalRaySpacing;
        [HideInInspector]
        public new BoxCollider2D collider;
        public RaycastOrigins raycastOrigins;

        [Header("States")]
        public bool isGrounded;
        public int facing;
        public Vector2 moveAmt;

        public CollisionInfo collisions;
        [HideInInspector]
        public Vector2 playerInput;

        [HideInInspector]
        public RaycastHit2D hit;

        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        public struct CollisionInfo
        {
            public bool left, right, above, below;
            public bool climbingSlope;
            public bool descendingSlope;
            public float slopeAngle, slopeAngleOld;
            public Vector2 moveAmountOld;
            public int faceDir;

            public void Reset()
            {
                left = right = false;

                climbingSlope = false;
                descendingSlope = false;

                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
            }
        }

        public void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
        }

        public void Start()
        {
            CalculateRaySpacing();
            collisions.faceDir = 1;
        }

        private void Update()
        {
            facing = collisions.faceDir;
            isGrounded = collisions.below;

            PlayerManager.Instance.collisionsLeft = collisions.left;
            PlayerManager.Instance.collisionsRight = collisions.right;
            PlayerManager.Instance.collisionsBelow = collisions.below;
        }


        public void UpdateRaycastOrigins()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public void CalculateRaySpacing()
        {
            if (collider == null) return;
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            //horizontalRayCount = 4;
            //verticalRayCount = 3;
            horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        public void Move(Vector2 moveAmount, Vector2 input)
        {
            UpdateRaycastOrigins();

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;

            moveAmt = moveAmount;

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HandleHorizontalCollisions(ref moveAmount);
            HandleVerticalCollisions(ref moveAmount);

            if (float.IsNaN(moveAmount.x) || float.IsNaN(moveAmount.y)) return;

            transform.Translate(moveAmount);
        }

        void HandleHorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * rayLength, Color.white);

                if (hit)
                {
                    if (hit.distance == 0) continue;

                    horizontalRaycastHit = hit;

                    if (PlayerManager.Instance.isBoosting && BoostCanPassThrough(hit)) return;

                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    Debug.DrawRay(rayOrigin, Vector2.right * rayLength, Color.red);

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }

        //void HandleVerticalCollisions(ref Vector2 moveAmount)
        //{
        //    float directionY = -1f;
        //    float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        //    var atLeastOneRayHitGround = false;

        //    for (int i = 0; i < verticalRayCount; i++)
        //    {
        //        Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
        //        rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
        //        hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

        //        Vector2 slideDirection;
        //        float slideDistance;

        //        // One or more vertical raycasts hit a collider
        //        if (hit)
        //        {
        //            PlayerManager.Instance.verticalRaycastHit = true;

        //            if (PlayerManager.Instance.isBoosting && SuperDashCheck(hit)) return;

        //            var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        //            PlayerManager.Instance.slopeAngle = slopeAngle;

        //            // If slope is "slideable" and slide has not been canceled
        //            if (SlopeIsSlideable(slopeAngle) && !PlayerManager.Instance.slideWasCanceled)
        //            {
        //                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.green);

        //                PlayerManager.Instance.isSliding = true;

        //                slideDirection = new Vector2(hit.normal.x, -hit.normal.y);
        //                slideDistance = PlayerManager.Instance.slopeSlideSpeed * Time.deltaTime;
        //                moveAmount = slideDirection * slideDistance;

        //                PlayerManager.Instance.slideDirection = slideDirection;
        //                PlayerManager.Instance.slideDistance = slideDistance;
        //            }
        //            else if (SlopeIsSlideable(slopeAngle))
        //            {
        //                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.blue);
        //            }
        //            // Ground is flat, slope is too steep to be slideable, or slide was canceled
        //            else
        //            {
        //                Debug.DrawRay(rayOrigin, Vector2.up * moveAmount.y, Color.red);

        //                PlayerManager.Instance.isSliding = false;
        //                PlayerManager.Instance.slideWasCanceled = false;

        //                atLeastOneRayHitGround = true;
        //                collisions.above = directionY == 1;
        //                moveAmount.y = (hit.distance - skinWidth) * directionY;
        //            }
        //        }
        //        // Vertical raycasts did not hit a collider
        //        else
        //        {
        //            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.white);

        //            PlayerManager.Instance.verticalRaycastHit = false;
        //            PlayerManager.Instance.isSliding = false;

        //            if (PlayerManager.Instance.slideWasCanceled)
        //            {
        //                moveAmount = Vector2.zero;
        //                PlayerManager.Instance.isSliding = false;
        //                PlayerManager.Instance.slideWasCanceled = false;
        //            }
        //        }
        //    }

        //    collisions.below = atLeastOneRayHitGround;
        //}

        [HideInInspector]
        public RaycastHit2D verticalRaycastHit = new();

        [HideInInspector]
        public RaycastHit2D horizontalRaycastHit = new();

        void HandleVerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = -1f;
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            var atLeastOneRayHitGround = false;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Vector2 slideDirection;
                float slideDistance;

                // A vertical raycast hit a collider
                if (hit)
                {
                    verticalRaycastHit = hit;
                    Debug.DrawRay(rayOrigin, Vector2.up * moveAmount.y, Color.magenta);

                    if (PlayerManager.Instance.isBoosting && BoostCanPassThrough(hit)) return;

                    var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    PlayerManager.Instance.slopeAngle = slopeAngle;

                    // If slope is "slideable" and slide has not been canceled
                    if (SlopeIsSlideable(slopeAngle))// && !PlayerManager.Instance.slideWasCanceled)
                    {
                        Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.green);
                        PlayerManager.Instance.isSliding = true;

                        slideDirection = new(hit.normal.x, -hit.normal.y);

                        slideDistance = PlayerManager.Instance.slopeSlideSpeed * Time.deltaTime;
                        moveAmount = slideDirection * slideDistance;

                        PlayerManager.Instance.slideDirection = slideDirection;
                        PlayerManager.Instance.slideDistance = slideDistance;
                    }

                    // Ground is flat, slope is too steep to be slideable, or slide was canceled
                    else
                    {
                        PlayerManager.Instance.isSliding = false;
                        //PlayerManager.Instance.slideWasCanceled = false;
                        PlayerManager.Instance.velocity.y = 0;
                        atLeastOneRayHitGround = true;
                        collisions.above = directionY == 1;

                        moveAmount.y = (hit.distance - skinWidth) * directionY;

                        Debug.DrawRay(rayOrigin, Vector2.up * moveAmount.y, Color.red);
                    }
                }
                // Vertical raycast did not hit a collider
                else
                {
                    Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.white);

                    PlayerManager.Instance.isSliding = false;
                    //PlayerManager.Instance.slideWasCanceled = false;
                }
            }

            collisions.below = atLeastOneRayHitGround;
        }

        bool BoostCanPassThrough(RaycastHit2D hit)
        {
            return hit.collider.gameObject.CompareTag("Solid") ||
                        hit.collider.gameObject.CompareTag("Destructible") ||
                        hit.collider.gameObject.CompareTag("Damage") ||
                        hit.collider.gameObject.CompareTag("Nitro");
        }

        bool SlopeIsSlideable(float slopeAngle)
        {
            return slopeAngle > 0 && slopeAngle <= PlayerManager.Instance.maxSlopeAngle;
        }
    }


}