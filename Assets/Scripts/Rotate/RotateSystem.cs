using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectTeam
{
    public class RotateSystem : MonoBehaviour
    {
        // ���S�_
        [SerializeField] private Vector3 _center = Vector3.zero;

        // ��]��
        [SerializeField] private Vector3 _axis = Vector3.up;

        // �~�^������
        [SerializeField] private float _period = 2;

        private void Update()
        {
            // ���S�_center�̎�����A��axis�ŁAperiod�����ŉ~�^��
            transform.RotateAround(
                _center,
                _axis,
                360 / _period * Time.deltaTime
            );
        }
    }
}
    // Start is called before the first frame update
    
