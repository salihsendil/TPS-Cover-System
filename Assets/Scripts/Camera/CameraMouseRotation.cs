using UnityEngine;

public class CameraMouseRotation : MonoBehaviour
{
    private float _mouseX;
    private float _mouseY;

    [SerializeField] private Transform _followTarget;
    [SerializeField] float _followTargetYOffset = 1.5f;
    [SerializeField] float _followTargetZOffset = -2f;

    private float _yaw = 0f;
    private float _pitch = 10f;
    [SerializeField] private Vector2 _pitchLimit = new Vector2(-30f, 60f);
    [SerializeField] private float _rotationSpeed = 3f;

    private void Update()
    {
        if (_followTarget == null)
        {
            Debug.LogWarning("Follow Target is empty");
            return;
        }

        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");

        _yaw = _yaw + _mouseX * _rotationSpeed;
        _pitch = _pitch - _mouseY * _rotationSpeed;

        _pitch = Mathf.Clamp(_pitch, _pitchLimit.x, _pitchLimit.y);

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        transform.position = _followTarget.position + transform.forward * _followTargetZOffset + Vector3.up * _followTargetYOffset;

    }

}
