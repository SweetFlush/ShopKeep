using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils; //CodeMonkey사이트에서 지원하는 유틸리티 에셋

public class GridSystem<TGridObject> {    //그리드 시스템 구현하기

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;    //Grid 오브젝트 값이 변경되었을때 이벤트 발생

    public class OnGridObjectChangedEventArgs : EventArgs 
    {
        public int x;
        public int z;
    }

    private int width;  //가로
    private int height; //세로
    private float cellSize; //셀 하나의 크기
    private Vector3 originPosition;

    private TGridObject[,] gridArray;   //그리드 배열

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition, Func<GridSystem<TGridObject>, int, int, TGridObject> createGridObject)    //GridSystem 객체 생성
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        //그리드생성

        gridArray = new TGridObject[width, height]; //그리드 배열 생성

        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebugLine = true;  //그리드 격자 보이기
        if (showDebugLine)
        {

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.black, 100f);  //Debug.DrawLine을 이용해 격자를 그린다
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.black, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);
        }

        bool showDebugText = false;  //셀 속성값 보이기
        if (showDebugText)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, cellSize) * .5f, 30 /*글씨크기*/,
                        Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);  //그리드 셀 각각 중간지점에 좌표를 표시하는 Text 생성
                }
            }
            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();    //Grid값이 변경되었으면 적용
            };
        }
    }

    public int GetWidth()   //각각 그리드 속성값을 리턴하는 함수들
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
    
    public void GetXZ(Vector3 worldPosition, out int x, out int z) // 셀 하나의 좌표를 가져오는 함수
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);

    }

    public Vector3 GetWorldPosition(int x, int z)  //x, y좌표에 해당하는 셀 하나의 Vector3값을 리턴하는 함수
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void SetGridObject(int x, int z, TGridObject value)   //x, y좌표값에 해당하는 셀 하나의 값 설정하기
    {
        if(x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            TriggerGridObjectChanged(x, z);
        }
    }

    public void TriggerGridObjectChanged(int x, int z)  //GridObject가 변경되었다는 신호를 보냄
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x , z = z});
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)  //Vector3 좌표값에 해당하는 셀 하나의 값 설정하기
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)   
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, width - 1),
            Mathf.Clamp(gridPosition.y, 0, height - 1)
            );
    }
}
