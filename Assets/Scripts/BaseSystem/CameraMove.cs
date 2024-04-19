using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] GameObject center;
        [SerializeField] GameObject lookingPosition;

        CalcCircle.RotateCircle rotInfo;
        float time = 0;

        void Start()
        {
            rotInfo = new CalcCircle.RotateCircle(gameObject, center.transform.position, PlayerSO.Entity.CameraRadius, new CalcCircle.AxisAngle(Vector3.right, 15), lookingPosition.transform.position);
        }

        void Update()
        {
            rotInfo.SetPosition(time, 2 * Mathf.PI / PlayerSO.Entity.CameraDuration);
            time += Time.deltaTime;
        }
    }

    namespace CalcCircle
    {
        /// <summary>
        /// Camera moves along this circle.
        /// When time = 0, the start position's x = 0.
        /// Counterclockwise when camera speed > 0.
        /// </summary>
        public class RotateCircle
        {
            public GameObject camera = null;
            public Vector3 center = Vector3.up;
            public float radius = 1;
            public AxisAngle rotation = new AxisAngle(Vector3.right, 0);
            public Vector3 lookingPosition = Vector3.zero;

            public RotateCircle(GameObject camera, Vector3 center, float radius, AxisAngle rotation, Vector3 lookingPosition)
            {
                this.camera = camera;
                this.center = center;
                this.radius = radius;
                this.rotation = rotation;
                this.lookingPosition = lookingPosition;
            }

            /// <summary>
            /// Calcurate the camera's position based on the current time.
            /// </summary>
            /// <param name="time">When time = 0, the start position's x = 0.</param>
            /// <param name="speed">[rad/s].</param>
            public void SetPosition(float time, float speed)
            {
                float angle = time * speed;

                // 1st : Calcurate the position.
                float x = Mathf.Sin(angle);
                float y = 0;
                float z = -Mathf.Cos(angle);
                Vector3 position1 = new Vector3(x, y, z);
                Vector3 position2 = position1 * this.radius;
                Vector3 position3 = Quaternion.AngleAxis(rotation.angle, rotation.axis) * position2;
                Vector3 position4 = position3 + this.center;
                camera.transform.position = position4;

                // 2nd : Look at the center.
                camera.transform.LookAt(lookingPosition);
            }
        }

        /// <summary>
        /// Rotation around the "axis".
        /// Rotate for "angle" (deg).
        /// </summary>
        public struct AxisAngle
        {
            public Vector3 axis;
            public float angle;

            public AxisAngle(Vector3 axis, float angle)
            {
                this.axis = axis;
                this.angle = angle;
            }
        }
    }
}
