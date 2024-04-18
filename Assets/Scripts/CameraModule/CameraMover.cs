using System;
using TankModule;
using UnityEngine;

namespace CameraModule
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private float dampTime = 0.2f;
        [SerializeField] private float screenEdgeBuffer = 4f;
        [SerializeField] private float minSize = 6.5f;

        private Transform[] _targets;
        private float _zoomSpeed;
        private Vector3 _moveVelocity;
        private Vector3 _destinationPosition;
        
        public void SetTargets(Tank[] tanks)
        {
            _targets = new Transform[tanks.Length];
            for (int i = 0; i < tanks.Length; i++)
            {
                _targets[i] = tanks[i].GetTransform();
            }
        }
        
        public void SetStartPosition()
        {
            FindAveragePosition ();

            transform.position = _destinationPosition;
            camera.orthographicSize = FindRequiredSize ();
        }

        private void FixedUpdate()
        {
            Move();
            Zoom();
        }

        private void Move()
        {
            FindAveragePosition();
            transform.position =
                Vector3.SmoothDamp(transform.position, _destinationPosition, ref _moveVelocity, dampTime);
        }

        private void FindAveragePosition()
        {
            Vector3 averagePosition = new Vector3 ();
            int targetsNumber = 0;

            for (int i = 0; i < _targets.Length; i++)
            {
                if (!_targets[i].gameObject.activeSelf)
                    continue;

                averagePosition += _targets[i].position;
                targetsNumber++;
            }

            if (targetsNumber > 0)
                averagePosition /= targetsNumber;

            averagePosition.y = transform.position.y;
            _destinationPosition = averagePosition;
        }

        private void Zoom()
        {
            float requiredSize = FindRequiredSize();
            camera.orthographicSize = Mathf.SmoothDamp (camera.orthographicSize, requiredSize, ref _zoomSpeed, dampTime);
        }
        
        private float FindRequiredSize ()
        {
            Vector3 desiredLocalPos = transform.InverseTransformPoint(_destinationPosition);
            float size = 0f;

            for (int i = 0; i < _targets.Length; i++)
            {
                if (!_targets[i].gameObject.activeSelf)
                    continue;

                Vector3 targetLocalPos = transform.InverseTransformPoint(_targets[i].position);
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
                
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / camera.aspect);
            }

            size += screenEdgeBuffer;
            size = Mathf.Max (size, minSize);

            return size;
        }
    }
}