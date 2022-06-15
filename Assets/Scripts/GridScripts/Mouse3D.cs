using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3D : MonoBehaviour    //마우스의 위치를 Vector3 값으로 리턴하기 위하여 생성한 클래스
{
    public static Mouse3D Instance
    {
        get;
        private set;
    }

    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            transform.position = raycastHit.point;
        }
    }

    public static Vector3 GetMouseWorldPosition() => Instance.GetMouseWorldPosition_Instance();

    private Vector3 GetMouseWorldPosition_Instance()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
            return Vector3.zero;
    }
}
