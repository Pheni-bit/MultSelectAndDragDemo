using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class TerrainGrid : MonoBehaviour
{
    public float cellSize = 1;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float yOffset = 0.5f;
    public Material cellMaterialValid;
    public Material cellMaterialInvalid;
    public Sprite sprite;
    float baseHeight;
    private GameObject[] _cells;
    public GameObject objToBuild;
    private GameObject tempBuildObj;
    private float[] _heights;
    private float[] _thisCellHeights = new float[4];
    private string validity;
    public void Init(GameObject go)
    {
        objToBuild = go;
        Building building = go.GetComponent<Building>();
        CreateGrid(building.sizeX, building.sizeY);
        CreateBuildingTemplate();
    }
    public void CreateGrid(int gridX, int gridY)
    {
        gridWidth = gridX;
        gridHeight = gridY;
        _cells = new GameObject[gridHeight * gridWidth];
        _heights = new float[(gridHeight + 1) * (gridWidth + 1)];

        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                _cells[z * gridWidth + x] = CreateChild();
            }
        }
    }
    GameObject CreateChild()
    {
        GameObject go = new GameObject
        {
            name = "Grid Cell"
        };
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshFilter>().mesh = CreateMesh();
        return go;
    }
    void CreateBuildingTemplate()
    {
        tempBuildObj = Instantiate(objToBuild, transform.position, Quaternion.identity);
        tempBuildObj.transform.parent = gameObject.transform;
    }

    void Update()
    {
        Debug.Log("running");
        UpdateSize();
        UpdatePosition();
        UpdateHeights();
        UpdateCells();
        UpdateBuildPreview();
        DetectInputForRotation();
    }

    void UpdateSize()
    {
        int newSize = gridHeight * gridWidth;
        int oldSize = _cells.Length;

        if (newSize == oldSize)
            return;

        GameObject[] oldCells = _cells;
        _cells = new GameObject[newSize];

        if (newSize < oldSize)
        {
            for (int i = 0; i < newSize; i++)
            {
                _cells[i] = oldCells[i];
            }

            for (int i = newSize; i < oldSize; i++)
            {
                Destroy(oldCells[i]);
            }
        }
        else if (newSize > oldSize)
        {
            for (int i = 0; i < oldSize; i++)
            {
                _cells[i] = oldCells[i];
            }

            for (int i = oldSize; i < newSize; i++)
            {
                _cells[i] = CreateChild();
            }
        }

        _heights = new float[(gridHeight + 1) * (gridWidth + 1)];
    }

    void UpdatePosition()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));
        Vector3 position = hitInfo.point;

        position.x -= hitInfo.point.x % cellSize + gridWidth * cellSize / 2;
        position.z -= hitInfo.point.z % cellSize + gridHeight * cellSize / 2;
        position.y = 0;

        transform.position = position;
    }

    void UpdateHeights()
    {
        RaycastHit hitInfo;
        Vector3 origin;

        for (int z = 0; z < gridHeight + 1; z++)
        {
            for (int x = 0; x < gridWidth + 1; x++)
            {
                origin = new Vector3(x * cellSize, 200, z * cellSize);
                Physics.Raycast(transform.TransformPoint(origin), Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));

                _heights[z * (gridWidth + 1) + x] = hitInfo.point.y;
            }
        }
    }

    void UpdateCells()
    {
        validity = "true";
        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cell = _cells[z * gridWidth + x];
                MeshRenderer meshRenderer = cell.GetComponent<MeshRenderer>();
                MeshFilter meshFilter = cell.GetComponent<MeshFilter>();
                meshRenderer.material = IsCellValid(x, z) ? cellMaterialValid : cellMaterialInvalid;
                UpdateMesh(meshFilter.mesh, x, z);
                IsCellStillValid(meshRenderer);
            }                                                         
        }
    }
    private void UpdateBuildPreview()
    {
        tempBuildObj.transform.localPosition = new Vector3(gridWidth / 2.0f, (tempBuildObj.transform.localScale.y / 2.0f) + _heights[0], gridHeight / 2.0f);
    }

    void DetectInputForRotation()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            tempBuildObj.transform.Rotate(new Vector3(0, 90, 0));
            int tempInt = gridHeight;
            gridHeight = gridWidth;
            gridWidth = tempInt;
        }
    }
    bool IsCellValid(int x, int z)
    {
        RaycastHit hitInfo;
        Vector3 origin = new Vector3(x * cellSize + cellSize / 2, 200, z * cellSize + cellSize / 2);
        //Physics.Raycast(transform.TransformPoint(origin), Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Buildings"));
        Physics.Raycast(transform.TransformPoint(origin), Vector3.down, out hitInfo, LayerMask.GetMask("Terrain"));;
        Debug.Log(hitInfo.collider);
        if (hitInfo.collider)
        {
            if (hitInfo.collider.gameObject.CompareTag("Floor"))
            {
                return true;
            }
            validity = "The raycast down does not see a gameobject.tag of 'Floor'";
            return false;
        }
        validity = "The raycast down does not see a gameobject'";
        return false;
    }

    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            name = "Grid Cell",
            vertices = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero },
            triangles = new int[] { 0, 1, 2, 2, 1, 3 },
            normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up },
            uv = new Vector2[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) }
        };
        return mesh;
    }

    void UpdateMesh(Mesh mesh, int x, int z)
    {
        mesh.vertices = new Vector3[] {
            MeshVertex(x, z),
            MeshVertex(x, z + 1),
            MeshVertex(x + 1, z),
            MeshVertex(x + 1, z + 1),
        };
        _thisCellHeights[0] = GetHeight(x, z);
        _thisCellHeights[1] = GetHeight(x, z + 1);
        _thisCellHeights[2] = GetHeight(x + 1, z);
        _thisCellHeights[3]= GetHeight(x + 1, z + 1);
    }

    Vector3 MeshVertex(int x, int z)
    {
        return new Vector3(x * cellSize, _heights[z * (gridWidth + 1) + x] + yOffset, z * cellSize);
    }
    float GetHeight(int x, int z)
    {
        Vector3 temVec = new Vector3(x * cellSize, _heights[z * (gridWidth + 1) + x] + yOffset, z * cellSize);
        return temVec.y;
    }
    void IsCellStillValid(MeshRenderer mr)
    {
        for (int f = 0; f < _thisCellHeights.Length - 1; f++)
        {
            if (_thisCellHeights[f] != _thisCellHeights[f + 1])
            {
                validity = "The Terrain is too uneven to place gameobject";
                mr.material = cellMaterialInvalid;
                break;
            }
        }
    }

    public string AttemptToPlaceObject()
    {
        if (tempBuildObj.GetComponent<Building>().HasTheRequiredResources() == false)
            validity = "Does not have the Required Resources!";
        return validity;
    }
    public void PlaceObject()
    {
        GameObject builtObj = Instantiate(objToBuild, tempBuildObj.transform.position, tempBuildObj.transform.rotation);

        builtObj.GetComponent<Building>().Init();
    }

    public void TurnOff()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Destroy(child);
        }
        gameObject.SetActive(false);
    }
    public void BuildingButtonClick(GameObject go)
    {
        TurnOff();
        gameObject.SetActive(true);
        Init(go);
    }
}
