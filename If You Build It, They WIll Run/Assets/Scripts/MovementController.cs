    #define DEBUG
    //#define DESIRED_DEBUG
    //#define GRAVITY_DEBUG
    #define GROUND_DEBUG
    //#define SPEED_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Movement
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private float WALKING_SPEED = 3f;
        [SerializeField] private float speedSmoothTime = 0.1f;
        [SerializeField] public float SPRINT_MULTIPLIER = 3;
        [SerializeField] public float gravity = 4f, gravity_limit = -16f;


        private CharacterController controller = null;
        private Animator animator = null;
        private Transform mainCameraTransform = null;

        private float speedSmoothVelocity = 0f;
        private float currentSpeed = 0f;
        private static readonly int hashSpeedPercentage = Animator.StringToHash("SpeedPercentage");



        public float rotationSpeed = 0.04f;
        public float jumpForce = 2f;
        private float sprintEnable = 1f;
        private Vector3 desiredMoveDirection = Vector3.zero;
        private Vector3 forward = Vector3.zero;
        private Vector3 right = Vector3.zero;
        private Vector3 vertical = Vector3.zero;
        private float verticalVelocity = 0f;



        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            mainCameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            float movementInputYmagnitude = 0f;
            float movementInputXmagnitude = 0f;
            forward = mainCameraTransform.forward;
            right = mainCameraTransform.right;
            forward.y = 0f;
            forward.y = 0f;

            #if DEBUG
                #if GRAVITY_DEBUG
                    print("verticalVelocity = " + verticalVelocity);
                #endif
                #if DESIRED_DEBUG
                    print("desiredMoveDirection.y pre = " + desiredMoveDirection.y);
                #endif
                #if GROUND_DEBUG
                    print("Grounded = " + controller.isGrounded);
                #endif
            #endif

            if(controller.isGrounded)
            {
                movementInputYmagnitude = movementInput.y;
                movementInputXmagnitude = movementInput.x;
                verticalVelocity = -gravity * Time.deltaTime;

                if(Input.GetKeyDown(KeyCode.Space))
                    verticalVelocity = jumpForce;
                desiredMoveDirection = (forward * movementInputYmagnitude + right * movementInputXmagnitude).normalized;

                if(movementInput.y != 0 || movementInput.x != 0)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationSpeed);
            }
            else
                verticalVelocity -= gravity * Time.deltaTime;
            desiredMoveDirection.y = verticalVelocity;

            if(Input.GetKey(KeyCode.LeftShift))
                sprintEnable = SPRINT_MULTIPLIER;
            else if(Input.GetKeyUp(KeyCode.LeftShift))
                sprintEnable = 1f;                

            float targetSpeed = WALKING_SPEED * movementInput.magnitude * sprintEnable;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
            controller.Move(desiredMoveDirection * currentSpeed * Time.deltaTime);
            animator.SetFloat(hashSpeedPercentage,  (sprintEnable / 2) * movementInput.magnitude, speedSmoothTime, Time.deltaTime);

            #if DEBUG
                #if DESIRED_DEBUG
                    print("desiredMoveDirection.y post = " + desiredMoveDirection.y);
                #endif
                #if SPEED_DEBUG
                    print("targetSpeed = " + targetSpeed);
                #endif
            #endif
        }
    }
}