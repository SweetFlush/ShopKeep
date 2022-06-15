using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static int tableIndex = 0;
    public static int weaponHangerIndex = 0;
    public static Table table;

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO, int index)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.gameObject.layer = 7;

        switch (index)
        {
            case 0: //���̺��̴�
                tableIndex = GridBuildingSystem.Instance.GetTableIndex();
                placedObject.gameObject.name = tableIndex.ToString();   //�̸��� �޾ƿ���
                placedObject.gameObject.AddComponent<Table>();          //table��ũ��Ʈ �޴´�
                break;
            case 1:
                weaponHangerIndex = GridBuildingSystem.Instance.GetWeaponHangerIndex();
                placedObject.gameObject.name = weaponHangerIndex.ToString();   //�̸��� �޾ƿ���
                placedObject.gameObject.AddComponent<WeaponHanger>();          //table��ũ��Ʈ �޴´�
                break;
            case 2: placedObject.gameObject.name = "PotionStorage";
                break;
        }

        placedObject.gameObject.transform.Find("Area").gameObject.SetActive(false);
        placedObject.gameObject.transform.Find("Anchor").gameObject.SetActive(true);

        return placedObject;
    }   //��ġ�� ������ �Ӽ����� �޾ƿ´�

    private PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;
    private List<Item> itemList;

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
