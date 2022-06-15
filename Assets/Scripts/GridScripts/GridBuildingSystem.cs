using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;    //������ �ϴ� ������ ����� ��� �߻�
    //public event EventHandler OnObjectPlaced;       //������ ���������� �߻�
    public Player player;

    private GridSystem<GridObject> grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;

    private int index;  //���� ��ȣ
    private int tableIndex;
    private int weaponHangerIndex;

    private void Awake()    //Awake�ÿ� �׸��� ����
    {
        Instance = this;

        int gridWidth = 10;
        int gridHeight = 15;
        float cellSize = 10f;
        grid = new GridSystem<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(10, 0, 10),
            (GridSystem<GridObject> g, int x, int z) => new GridObject(g, x, z));   //Grid ��ü ����

        placedObjectTypeSO = placedObjectTypeSOList[0];
        DeselectObjectType();
        tableIndex = 0;
        weaponHangerIndex = 0;
    }

    public class GridObject    //Grid�� �����Ǹ� ������ ���� GridObject�� ������
    {
        private GridSystem<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;



        public GridObject(GridSystem<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
            placedObject = null;
        }

        public override string ToString()
        {
            return x + " " + z + "\n" + placedObject;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) //��Ŭ���ϸ� ������ ������ �Ǽ��ȴ�
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);
            
            //��ġ �������� ����
            bool canBuild = true;   //true���̸� ������ ��ġ��
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);
            foreach (Vector2Int gridPosition in gridPositionList)    //���� ���� ũ�� �� �ϳ��ϳ����� ��ġ �������� ���� ����
            {
                if(!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {   //��ġ�� �Ұ����ϸ�
                    canBuild = false;   //canBuild���� false�� �ٲپ� ��ġ �Ұ���
                    break;
                }
            }

            if(canBuild)    //�Ǽ��� �����ϸ�
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) +
                    new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();    //ȸ���������� pivot�� ���� ȸ���� ��ü�� �ܵ�ġ ���� �������� ȸ������ �ʰ� ����

                //���콺�� �ִ� ���� testTransform��ü�� �����Ѵ�
                PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO, index);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);    //�ش� �Ǽ� �� �ϳ��ϳ����� �Ӽ��� �ο�
                }

                switch (index)   // 0:Table, 1:WeaponHanger, 2:PotionStorage
                {                // ���̺��� ���������� ���� ������ +3, �����ġ��� ���������� +1
                    case 0:
                        tableIndex++;
                        player.numOfTable++;
                        break;
                    case 1:
                        weaponHangerIndex++;
                        player.numOfWeaponHanger++;
                        break;
                    case 2:
                        break;
                }

                player.RemoveItem();
                DeselectObjectType();   //���������� �Ǽ������� deselect

            } else
            {
                UtilsClass.CreateWorldTextPopup("Cannot build Here!", Mouse3D.GetMouseWorldPosition()); //�Ǽ� �Ұ����� ���� ��� �ش� ��ġ�� ��� ���
            }
        }

        /*if(Input.GetMouseButtonDown(1)) //��Ŭ���ϸ� ������ ���ŵȴ�
        {
            GridObject gridObject = grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
            PlacedObject placedObject = gridObject.GetPlacedObject();
            if(placedObject != null)
            {
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList(); //���� ���� �� ����Ʈ �о����

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();    //������ �� ���� Clear
                }
            }
            
        }*/

        if(Input.GetKeyDown(KeyCode.R)) //RŰ�� ������ ���� ���� ��ȯ
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
            UtilsClass.CreateWorldTextPopup("" + dir, Mouse3D.GetMouseWorldPosition());
        }
        
        //���� �����ϴ°�
        /*
         * if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }; 
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); };
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); };
        */
    }

    public void SelectItem(Item.ItemType itemType)  //������ ����
    {
        switch (itemType)   // 0:Table, 1:WeaponHanger, 2:PotionStorage
        {
            case Item.ItemType.Table:
                index = 0;
                break;
            case Item.ItemType.WeaponHanger:
                index = 1;
                break;
            case Item.ItemType.PotionStorage:
                index = 2;
                break;
        }

        placedObjectTypeSO = placedObjectTypeSOList[index];
        RefreshSelectedObjectType();   
    }

    private void DeselectObjectType()
    {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() 
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()   //BuildingGhost�� ǥ�õ� ��ġ�� ����
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if(placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        } else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    public int GetTableIndex()
    {
        return tableIndex;
    }

    public int GetWeaponHangerIndex()
    {
        return weaponHangerIndex;
    }
}
