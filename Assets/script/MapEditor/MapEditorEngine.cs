using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
	private bool canBuild;

    private GameObject Map;
	private GameObject tilesContaier;
    private List<GameObject> tcGoes = new List<GameObject>();
	private GameObject lightGO;
    public GameObject grid;
    private GameObject hitObj;

    public GameObject cameraGO;

	public GraphicRaycaster UIRaycaster;

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

		loadTiles ();
        loadTools();

		optState = states.tMove;
    }

	void loadTiles(){
		TextAsset _allTileInfo = (TextAsset)Resources.Load ("uteForEditor/uteCategoryInfo");
		string allTileInfo = _allTileInfo.text;
		string[] allInfoByCat = (string[]) allTileInfo.Split ('|');

		for (int i = 0; i < allInfoByCat.Length; i++) {
			if (!allInfoByCat[i].ToString().Equals("")) {
				string[] splitedInfo = (string[])allInfoByCat[i].ToString().Split('$');
				string[] splitedGUIDs = (string[])splitedInfo[2].ToString().Split(':');

				string cName = splitedInfo[0].ToString();
				string cCollider = splitedInfo[1].ToString();
				string cType = splitedInfo[3].ToString();

				List<GameObject> cObjs = new List<GameObject>();
				List<Texture2D> cTexture = new List<Texture2D>();
				List<string> cObjsNames = new List<string>();
				List<string> cObjsGuids = new List<string>();

				for (int j = 0; j < splitedGUIDs.Length; j++) {
					
				}
			}
		}
	}


    public void initMapEditorEngine() {
        Map = new GameObject("MAP");
		tilesContaier = new GameObject ("TileContainer");
		lightGO = (GameObject) Instantiate((GameObject) Resources.Load("uteForEditor/uteMapLight"));
		lightGO.name = "MapLight";
		
        globalGridSizeZ = 1000;
        globalGridSizeX = 1000;
        gridSize = new Vector3(1000.0f, 0.1f, 1000.0f);

        cameraGO.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        cameraGO.transform.position = new Vector3(500, 14, 490);

        optState = states.none;
		GameObject gameObj = (GameObject)Resources.Load("box3");
        curTile = (GameObject)Instantiate(gameObj, new Vector3(0.0f, 0.0f, 0.0f), gameObj.transform.rotation);
    }

    private void loadTools() {
		GameObject _grid = (GameObject)Resources.Load("uteForEditor/uteLayer");
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
                procTMove();
                break;
            case states.tRoate:
                break;
            case states.tBuild:
				procBuild();
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

    void procTMove ()
		{
				if (EventSystem.current.IsPointerOverGameObject ()) {
						Debug.Log ("当前触摸在UI上");
				} else {
						RaycastHit buildHit = getHitObj ();
						if (buildHit.collider) {
				
								hitObj = buildHit.collider.gameObject;			
								Debug.Log (hitObj.name);
				
								Vector3 calPos = new Vector3 (Mathf.Round (buildHit.point.x), 0.01f, Mathf.Round (buildHit.point.z));
				
								curTile.transform.position = calPos;
								canBuild = true;
								if (Input.GetKeyDown (KeyCode.Mouse0) && canBuild) {
										optState = states.tBuild;
								}
						} else {
								Debug.Log ("not hit obj");
						}
						Debug.Log ("当前没有触摸在UI上");
				}
        
		}

    private RaycastHit getHitObj()
    {
        Ray buildRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit buildHit;

        if (Physics.Raycast(buildRay, out buildHit, 1000))
        {
            if (buildHit.collider)
            {
                return buildHit;
            }
            else
            {
                return new RaycastHit();
            }
        }
        else
        {
            return new RaycastHit();
        }
    }

	void procBuild(){
		GameObject newObj = null;
		newObj = (GameObject)Instantiate (curTile, curTile.transform.position, curTile.transform.rotation);
		newObj.layer = 0;
		Destroy (newObj.rigidbody);
		newObj.collider.isTrigger = false;

		newObj.transform.parent = tilesContaier.transform;

		canBuild = false;
		optState = states.tMove;
	}
}
