using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Controller2D : MonoBehaviour
    {
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

        public bool grounded;
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
            public Vector2 moveAmountOld;
            public int faceDir;

            public void Reset()
            {
                left = right = false;
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
            grounded = collisions.below;
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

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        //public void Move(Vector2 moveAmount)
        //{
        //    Move(moveAmount, Vector2.zero, false);
        //}


        //public void Move(Vector2 moveAmount, bool standingOnPlatform)
        //{
        //    Move(moveAmount, Vector2.zero, standingOnPlatform);
        //}

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

            HorizontalCollisions(ref moveAmount);
            VerticalCollisions(ref moveAmount);

           // if (suspendGravity || grounded) moveAmount.y = 0;

            if (float.IsNaN(moveAmount.x) || float.IsNaN(moveAmount.y))
                return;
            else
                transform.Translate(moveAmount);
        }


        void HorizontalCollisions(ref Vector2 moveAmount)
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

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit)
                {
                    if (hit.distance == 0) continue;

                    if ((hit.collider.gameObject.CompareTag("Solid") ||
                        hit.collider.gameObject.CompareTag("Destructible") ||
                        hit.collider.gameObject.CompareTag("Damage"))
                        && PlayerManager.Instance.isSuperDashing) return;

                    //float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    //if (i == 0 && slopeAngle <= maxClimbAngle)
                    //{
                    //    if (collisions.descendingSlope)
                    //    {
                    //        collisions.descendingSlope = false;
                    //        moveAmount = collisions.moveAmountOld;
                    //    }
                    //    float distanceToSlopeStart = 0;
                    //    if (slopeAngle != collisions.slopeAngleOld)
                    //    {
                    //        distanceToSlopeStart = hit.distance - skinWidth;
                    //        moveAmount.x -= distanceToSlopeStart * directionX;
                    //    }
                    //    ClimbSlope(ref moveAmount, slopeAngle);
                    //    moveAmount.x += distanceToSlopeStart * directionX;
                    //}

                    //if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                    //{
                    //    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    //    rayLength = hit.distance;

                    //    collisions.left = directionX == -1;
                    //    collisions.right = directionX == 1;
                    //}

                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    if ((hit.collider.gameObject.CompareTag("Solid") ||
                        hit.collider.gameObject.CompareTag("Destructible") ||
                        hit.collider.gameObject.CompareTag("Damage"))
                        && PlayerManager.Instance.isSuperDashing) return;

                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    collisions.below = true;
                    PlayerManager.Instance.velocityY = 0;
                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
                else
                {
                    collisions.below = false;
                }
            }

        }

    }


}