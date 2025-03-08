using UnityEngine;

public class SimpleRoom : MonoBehaviour
{
    public Vector3 roomSize = new Vector3(10, 5, 10);
    public Material wallMaterial;
    public Material floorMaterial;

    private void Start()
    {
        CreateWall(new Vector3(0, 0, roomSize.z / 2), new Vector3(roomSize.x, roomSize.y, 0.1f)); // Front Wall
        CreateWall(new Vector3(0, 0, -roomSize.z / 2), new Vector3(roomSize.x, roomSize.y, 0.1f)); // Back Wall
        CreateWall(new Vector3(roomSize.x / 2, 0, 0), new Vector3(0.1f, roomSize.y, roomSize.z)); // Right Wall
        CreateWall(new Vector3(-roomSize.x / 2, 0, 0), new Vector3(0.1f, roomSize.y, roomSize.z)); // Left Wall
        CreateFloorOrCeiling(new Vector3(0, -roomSize.y / 2, 0), new Vector3(roomSize.x, 0.1f, roomSize.z), floorMaterial); // Floor
        CreateFloorOrCeiling(new Vector3(0, roomSize.y / 2, 0), new Vector3(roomSize.x, 0.1f, roomSize.z), wallMaterial); // Ceiling
    }

    private void CreateWall(Vector3 position, Vector3 scale)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = wallMaterial;
        wall.AddComponent<BoxCollider>();
        wall.name = "Wall";
    }

    private static void CreateFloorOrCeiling(Vector3 position, Vector3 scale, Material material)
    {
        var surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        surface.transform.position = position;
        surface.transform.localScale = scale;
        surface.GetComponent<Renderer>().material = material;
        surface.AddComponent<BoxCollider>();
        surface.name = "Floor";
    }
}
