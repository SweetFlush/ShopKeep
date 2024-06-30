using CodeMonkey.Utils;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }

    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomDir = UtilsClass.GetRandomDir();
        ItemWorld itemWorld = SpawnItemWorld(new Vector3(dropPosition.x + randomDir.x * 10f, 20, dropPosition.z + randomDir.z * 10f), item);
        itemWorld.GetComponent<Rigidbody>().AddForce(randomDir * 5f, ForceMode.Impulse);
        return itemWorld;
    }

    private Item item;
    private SpriteRenderer spriteRenderer;
    private MeshFilter meshFilter;   //3D모델
    private Light light;
    

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshFilter = transform.Find("Model").GetComponent<MeshFilter>();
        light = transform.Find("Light").GetComponent<Light>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
        meshFilter.mesh = item.GetMesh();
        light.color = item.GetColor();
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
