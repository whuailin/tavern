using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapEditorEngine : MonoBehaviour {

	public string mapName;

    private enum states {
        none,
        tChoose,
        tMove,
        tRoate,
        tBuild,
        tErase,
        camMove,
        camZoom
    };

    private GameObject curTile;
    private string curFamilyName;
    private int lastSelectIndex;
    private int globalGridSizeX;
    private int globalGridSizeZ;
    private Vector3 gridSize;
    private int curObjID;
    private int curLayer;
    private states optState;

    private GameObject Map;
    private List<GameObject> tcGoes = new List<GameObject>();
    public GameObject grid;

    public GameObject cameraGO;
    public class TileInfo {
        public string catName;
        public string catCollision;
        public List<GameObject> catObjs = new List<GameObject>();
        public List<Texture2D> catObjsPrevs = new List<Texture2D>();
        public List<string> catObjsNames = new List<string>();
        public List<string> catGuidNames = new List<string>();

        public TileInfo(string _catName, List<string> _catObjsNames, string _catCollision, List<GameObject> _catObjs, List<Texture2D> _catObjsPrevs, List<string> _catGuidNames)
        {
            catName = _catName;
            catCollision = _catCollision;
            catObjs = _catObjs;
            catObjsPrevs = _catObjsPrevs;
            catObjsNames = _catObjsNames;
            catGuidNames = _catGuidNames;
        }
    }

    void Start()
    {
        initMapEditorEngine();

        loadTools();
    }

    public void initMapEditorEngine() {
        Map = new GameObject("MAP");
        globalGridSizeZ = 1000;
        globalGridSizeX = 1000;
        gridSize = new Vector3(1000.0f, 0.1f, 1000.0f);

        cameraGO.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        cameraGO.transform.position = new Vector3(500, 14, 490);

        optState = states.none;
    }

    private void loadTools() {
        GameObject _grid = (GameObject)Resources.Load("uteLayer");
        grid = (GameObject)Instantiate(_grid, new Vector3((gridSize.x / 2) + 0.5f, 0.0f, (gridSize.z / 2) + 0.5f), _grid.transform.rotation);
        grid.name = "Grid";
        setGrid(globalGridSizeX, globalGridSizeZ);
    }

    private void setGrid(int x, int z) {
        grid.transform.localScale = new Vector3((float)x, 0.01f, (float)z);
        grid.renderer.material.mainTextureScale = new Vector2((float)x, (float)z);
    }

	// Update is called once per frame
	void Update () {
        switch (optState)
        {
            case states.none:
                break;
            case states.tChoose:
                break;
            case states.tMove:
                break;
            case states.tRoate:
                break;
            case states.tBuild:
                break;
            case states.tErase:
                break;
            case states.camMove:
                break;
            case states.camZoom:
                break;
            default:
                break;
        }
	}

    void foo() {
        Ray builldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit buildHit;

        if (Physics.Raycast(builldRay, out buildHit, 1000))
        {
            if (buildHit.collider)
            {
                GameObject hitObj = buildHit.collider.gameObject;

            } 
        }
    }
}
