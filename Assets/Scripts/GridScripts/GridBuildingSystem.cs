using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;    //짓고자 하는 가구가 변경된 경우 발생
    //public event EventHandler OnObjectPlaced;       //가구가 지어졌을때 발생
    public Player player;

    private GridSystem<GridObject> grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;

    private int index;  //가구 번호
    private int tableIndex;
    private int weaponHangerIndex;

    private void Awake()    //Awake시에 그리드 생성
    {
        Instance = this;

        int gridWidth = 10;
        int gridHeight = 15;
        float cellSize = 10f;
        grid = new GridSystem<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(10, 0, 10),
            (GridSystem<GridObject> g, int x, int z) => new GridObject(g, x, z));   //Grid 객체 생성

        placedObjectTypeSO = placedObjectTypeSOList[0];
        DeselectObjectType();
        tableIndex = 0;
        weaponHangerIndex = 0;
    }

    public class GridObject    //Grid가 생성되면 각각의 셀은 GridObject를 가진다
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
        if(Input.GetMouseButtonDown(0)) //왼클릭하면 선택한 가구가 건설된다
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);
            
            //배치 가능한지 점검
            bool canBuild = true;   //true값이면 가구가 배치됨
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);
            foreach (Vector2Int gridPosition in gridPositionList)    //가구 점유 크기 셀 하나하나마다 배치 가능한지 여부 조사
            {
                if(!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {   //배치가 불가능하면
                    canBuild = false;   //canBuild값을 false로 바꾸어 배치 불가능
                    break;
                }
            }

            if(canBuild)    //건설이 가능하면
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) +
                    new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();    //회전시켰을때 pivot을 따라 회전한 객체가 외도치 않은 방향으로 회전되지 않게 설정

                //마우스가 있는 셀에 testTransform객체를 생성한다
                PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO, index);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);    //해당 건설 셀 하나하나마다 속성값 부여
                }

                switch (index)   // 0:Table, 1:WeaponHanger, 2:PotionStorage
                {                // 테이블이 지어졌으면 포션 진열량 +3, 무기거치대면 무기진열량 +1
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
                DeselectObjectType();   //성공적으로 건설했으니 deselect

            } else
            {
                UtilsClass.CreateWorldTextPopup("Cannot build Here!", Mouse3D.GetMouseWorldPosition()); //건설 불가능한 곳일 경우 해당 위치에 경고문 출력
            }
        }

        /*if(Input.GetMouseButtonDown(1)) //우클릭하면 가구가 제거된다
        {
            GridObject gridObject = grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
            PlacedObject placedObject = gridObject.GetPlacedObject();
            if(placedObject != null)
            {
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList(); //점유 중인 셀 리스트 읽어오기

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();    //점유된 셀 각각 Clear
                }
            }
            
        }*/

        if(Input.GetKeyDown(KeyCode.R)) //R키를 누르면 가구 방향 전환
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
            UtilsClass.CreateWorldTextPopup("" + dir, Mouse3D.GetMouseWorldPosition());
        }
        
        //가구 선택하는거
        /*
         * if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }; 
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); };
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); };
        */
    }

    public void SelectItem(Item.ItemType itemType)  //아이템 선택
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

    public Vector3 GetMouseWorldSnappedPosition()   //BuildingGhost가 표시될 위치를 리턴
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
