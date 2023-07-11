using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thermine.TFWBDBS.Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        [Header("Variables")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;


        //floats
        public float RotationSmoothTime = 0.12f;
        private float _targetRotation = 0f;
        private float _rotationVelocity;
        [SerializeField] private GameObject mainCamera;


        //vectors
        private Vector3 moveDirection;
        private Vector3 moveRotation;
        private Vector3 velocity;

        private GameObject playerMesh;

        [SerializeField] private bool isGrounded;
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpHeight;

        private CharacterController controller;
        private Animator animator;

        #endregion

        #region Methods

        private void Start()
        {
            playerMesh = gameObject.transform.Find("chel.anim").gameObject; // player 3d object in scene
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            Move(); // moving

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(Attack()); // attack target
            }
        }

        private void Move()
        {
            isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask); // checking ground 

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float moveZ = Input.GetAxis("Vertical");
            float moveX = Input.GetAxis("Horizontal");

            moveDirection = new Vector3(moveX * moveSpeed, moveDirection.y, moveZ * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            // =================================================

            // attempt to make a rotation of player's 3d model

            //moveRotation = new Vector3(0f, moveRotation.y, 0f);
            //if(moveX >= 1f)
            //{
            //    moveRotation.y = 90f;
            //}
            //else if (moveX <= -1f)
            //{
            //    moveRotation.y = -90f;
            //}
            //else if (moveZ >= 1f)
            //{
            //    moveRotation.y = 0f;
            //}
            //else if (moveZ <= -1f)
            //{
            //    moveRotation.y = 180f;
            //}
            //playerMesh.transform.rotation = Quaternion.Euler(moveRotation);
            //moveRotation = transform.TransformDirection(moveRotation);

            // ==================================================

            if (isGrounded) // change player's condition
            {
                if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
                {
                    Walk();
                }
                else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
                {
                    Run();
                }
                else if (moveDirection == Vector3.zero)
                {
                    Idle();
                }


                moveDirection *= moveSpeed;

                if (Input.GetKeyDown(KeyCode.Space)) // if "space" pressed than jump
                {

                    StartCoroutine(Jump());

                }
            }



            controller.Move(moveDirection * Time.deltaTime); // apply moving

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime); // apply gravity


            float moveInput = Input.GetAxis("Vertical");
            if (moveInput > 0)
            {
                Flip();
            }

        private void Flip() // rotate player's 3d model when pressed "w', "a", "s", "d"
        {
            //Vector3 rotator = transform.localRotation.eulerAngles;
            //var rotator = new Vector3(0f, -90f, 0f);
            float inputDirectionx = Input.GetAxis("Horizontal");
            float inputDirectionz = Input.GetAxis("Vertical");
            Vector3 inputDirection = new Vector3(playerMesh.transform.position.x, 0.0f, playerMesh.transform.position.y).normalized;
            Vector2 plMeshTransform = new Vector2(playerMesh.transform.position.x, playerMesh.transform.position.z);
            if (plMeshTransform != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirectionx, inputDirectionz) * Mathf.Rad2Deg +
                              mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                playerMesh.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        private void Walk()
        {
            moveSpeed = walkSpeed;
            animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
        }

        private void Run()
        {
            moveSpeed = runSpeed;
            animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
        }

        private void Idle()
        {
            animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
        }

        private IEnumerator Jump()
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Jump Layer"), 1);
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

            yield return new WaitForSeconds(0.9f);

            animator.SetLayerWeight(animator.GetLayerIndex("Jump Layer"), 0);

        }

        private IEnumerator Attack()
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);

            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.9f);
            animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 0);
        }

        #endregion
    }
}

