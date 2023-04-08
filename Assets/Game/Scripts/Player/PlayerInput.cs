using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Settings")]
        [Min(0f)] public float speedMax = 4f;

        [Header("Touch")]
        public Vector3 touchScreen = Vector3.zero;
        public Vector3 touchBeganScreen = Vector3.zero;

        [Header("Input")]
        public Vector3 move = Vector3.zero;

        [Header("Events")]
        public UnityEvent<Vector3> onTouchBegan = new UnityEvent<Vector3>();
        public UnityEvent<Vector3> onTouchMoved = new UnityEvent<Vector3>();
        public UnityEvent<Vector3> onTouchEndOrCancel = new UnityEvent<Vector3>();

        [Header("References")]
        public PlayerController player;
        public Camera cam;

        private void UpdateInputEditor()
        {
            move.x = Input.GetAxis("Horizontal") * Mathf.Abs(Input.GetAxisRaw("Horizontal"));
            move.y = 0f;
            move.z = Input.GetAxis("Vertical") * Mathf.Abs(Input.GetAxisRaw("Vertical"));
        }

        private int UpdateInputTouch()
        {
            var touchCount = Input.touchCount;

            if (touchCount == 0) return 0;

            var touch = Input.GetTouch(0);

            touchScreen = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchBeganScreen = touchScreen;
                    onTouchBegan.Invoke(touchScreen);
                    break;
                case TouchPhase.Moved:
                    move = touchScreen - touchBeganScreen;
                    move = new Vector3(move.x, 0f, move.y);
                    onTouchMoved.Invoke(touchScreen);
                    break;
                case TouchPhase.Ended or TouchPhase.Canceled:
                    onTouchEndOrCancel.Invoke(touchScreen);
                    break;
            }

            return touchCount;
        }

        private Vector3 GetWorldDirection(Vector3 localDirection)
        {
            var direction = cam.transform.TransformDirection(localDirection);
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            return direction;
        }

        private void UpdateInput()
        {
            var inputCount = UpdateInputTouch();

#if UNITY_EDITOR
            if (inputCount == 0) UpdateInputEditor();
#endif
        }

        private void UpdatePlayer()
        {
            var direction = GetWorldDirection(move);

            player.moving = direction != Vector3.zero;
            
            player.Move(direction.normalized * (speedMax * Time.deltaTime));

            if (player.moving) player.View(direction);
        }

        private void OnEnable()
        {
            if (cam == null) cam = Camera.main;
        }

        private void Update()
        {
            UpdateInput();

            UpdatePlayer();
        }
    }
}