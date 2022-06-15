using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils; //CodeMonkey����Ʈ���� �����ϴ� ��ƿ��Ƽ ����

public class GridSystem<TGridObject> {    //�׸��� �ý��� �����ϱ�

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;    //Grid ������Ʈ ���� ����Ǿ����� �̺�Ʈ �߻�

    public class OnGridObjectChangedEventArgs : EventArgs 
    {
        public int x;
        public int z;
    }

    private int width;  //����
    private int height; //����
    private float cellSize; //�� �ϳ��� ũ��
    private Vector3 originPosition;

    private TGridObject[,] gridArray;   //�׸��� �迭

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition, Func<GridSystem<TGridObject>, int, int, TGridObject> createGridObject)    //GridSystem ��ü ����
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        //�׸������

        gridArray = new TGridObject[width, height]; //�׸��� �迭 ����

        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebugLine = true;  //�׸��� ���� ���̱�
        if (showDebugLine)
        {

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.black, 100f);  //Debug.DrawLine�� �̿��� ���ڸ� �׸���
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.black, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);
        }

        bool showDebugText = false;  //�� �Ӽ��� ���̱�
        if (showDebugText)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, cellSize) * .5f, 30 /*�۾�ũ��*/,
                        Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);  //�׸��� �� ���� �߰������� ��ǥ�� ǥ���ϴ� Text ����
                }
            }
            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();    //Grid���� ����Ǿ����� ����
            };
        }
    }

    public int GetWidth()   //���� �׸��� �Ӽ����� �����ϴ� �Լ���
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
    
    public void GetXZ(Vector3 worldPosition, out int x, out int z) // �� �ϳ��� ��ǥ�� �������� �Լ�
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);

    }

    public Vector3 GetWorldPosition(int x, int z)  //x, y��ǥ�� �ش��ϴ� �� �ϳ��� Vector3���� �����ϴ� �Լ�
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void SetGridObject(int x, int z, TGridObject value)   //x, y��ǥ���� �ش��ϴ� �� �ϳ��� �� �����ϱ�
    {
        if(x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            TriggerGridObjectChanged(x, z);
        }
    }

    public void TriggerGridObjectChanged(int x, int z)  //GridObject�� ����Ǿ��ٴ� ��ȣ�� ����
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x , z = z});
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)  //Vector3 ��ǥ���� �ش��ϴ� �� �ϳ��� �� �����ϱ�
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
