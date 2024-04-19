using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSystem
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] GameObject center;
        [SerializeField] GameObject lookingPosition;
        float stageRadius = 14.75f;
        float duration = 60f;
        float time = 0;

        RotateCircle rotInfo;

        void Start()
        {
            rotInfo = new RotateCircle(gameObject, center.transform.position, stageRadius, new AxisAngle(Vector3.right, 0), lookingPosition.transform.position);
        }

        void Update()
        {
            rotInfo.SetPosition(time, 2 * Mathf.PI / duration);
            time += Time.deltaTime;
        }
    }

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

            // 1st : Assume that this.rotation is default, then calcurate the position on the normalized circle.
            float x = Mathf.Sin(angle);
            float y = -Mathf.Cos(angle);
            float z = 0;
            Vector3 positionOnNormalizedCircle = new Vector3(x, y, z);

            // 2nd : Consider this.rotation, then calcurate the position on the circle.
            positionOnNormalizedCircle = Quaternion.AngleAxis(rotation.angle, rotation.axis) * positionOnNormalizedCircle;
            Vector3 positionOnCircle = positionOnNormalizedCircle * this.radius + this.center;

            // 3rd : Calcurate the camera's looking position.
            Vector3 dir = lookingPosition - positionOnCircle;
            Quaternion rot = Quaternion.LookRotation(dir);
            camera.transform.rotation = rot * camera.transform.rotation;
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
