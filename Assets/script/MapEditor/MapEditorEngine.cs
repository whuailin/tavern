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
	private List<TileInfo> allTiles = new List<TileInfo>();
    private List<GameObject> tcGoes = new List<GameObject>();
	private GameObject lightGO;
    public GameObject grid;
    private GameObject hitObj;

    public GameObject cameraGO;

	public GraphicRaycaster UIRaycaster;

	private float globalYSize = 1.0f; // ???

	//UI
	private Texture2D previewT;

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
					if (!splitedGUIDs[j].ToString().Equals("")) {
						string objPath = UnityEditor.AssetDatabase.GUIDToAssetPath(splitedGUIDs[j]);
						GameObject tObj = (GameObject)UnityEditor.AssetDatabase.LoadMainAssetAtPath(objPath);

						if(tObj) {
							cObjsNames.Add(tObj.name);
							cObjsGuids.Add(splitedGUIDs[j].ToString());

							previewT = UnityEditor.AssetPreview.GetAssetPreview((Object)tObj);

							GameObject temObj = (GameObject)Instantiate(tObj, Vector3.zero, tObj.transform.rotation);
							temObj.name = splitedGUIDs[j].ToString();
							List<GameObject> twoObjs = createColliderToObject(temObj, tObj);
							GameObject behindObj = (GameObject)twoObjs[0];
							behindObj.name = temObj.name;
							tObj = (GameObject) twoObjs[1];
							tObj.transform.parent = behindObj.transform;
							behindObj.layer = 2;
							cObjs.Add(behindObj);


							if(previewT)
							{
								cTexture.Add(previewT);
							}
							else
							{
								cTexture.Add(new Texture2D(20,20));
							}
						}
					}
				}

				if(!cName.Equals("")&&!cCollider.Equals("")&&cObjs.Count>0)
				{
					allTiles.Add(new TileInfo(cName,cObjsNames,cCollider,cObjs,cTexture,cObjsGuids));
				}
				else
				{
					if(cObjs.Count<=0)
					{
						Debug.Log ("Warning: Category ["+cName+"] was ignored because there are no objects inside");
					}
					else
					{
						Debug.Log ("Something is Wrong (CE)");
					}
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

	public List<GameObject> createColliderToObject(GameObject obj, GameObject obj_rs)
	{
		float lowestPointY = 10000.0f;
		float highestPointY = -10000.0f;
		float lowestPointZ = 10000.0f;
		float highestPointZ = -10000.0f;
		float lowestPointX = 10000.0f;
		float highestPointX = -10000.0f;
		float finalYSize = 1.0f;
		float finalZSize = 1.0f;
		float finalXSize = 1.0f;
		float divX = 2.0f;
		float divY = 2.0f;
		float divZ = 2.0f;
		
		Vector3 objScale = obj.transform.localScale;
		obj.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
		
		MeshFilter mfs = (MeshFilter) obj.GetComponent<MeshFilter>();
		MeshFilter[] mfs_arr = (MeshFilter[]) obj.GetComponentsInChildren<MeshFilter>();
		SkinnedMeshRenderer smfs = (SkinnedMeshRenderer) obj.GetComponent(typeof(SkinnedMeshRenderer));
		SkinnedMeshRenderer[] smfs_arr = (SkinnedMeshRenderer[]) obj.GetComponentsInChildren<SkinnedMeshRenderer>();
		Transform[] trms = (Transform[]) obj.GetComponentsInChildren<Transform>();
		
		if(mfs&&mfs.renderer)
		{
			lowestPointY = mfs.renderer.bounds.min.y;
			highestPointY = mfs.renderer.bounds.max.y;
		}
		
		if(mfs_arr.Length>0)
		{
			for(int i=0;i<mfs_arr.Length;i++)
			{
				MeshFilter mf_c = (MeshFilter) mfs_arr[i];
				
				if(mf_c&&mf_c.renderer)
				{
					if(mf_c.renderer.bounds.min.y<lowestPointY)
					{
						lowestPointY = mf_c.renderer.bounds.min.y;
					}
					
					if(mf_c.renderer.bounds.max.y>highestPointY)
					{
						highestPointY = mf_c.renderer.bounds.max.y;
					}
					
					if(mf_c.renderer.bounds.min.x<lowestPointX)
					{
						lowestPointX = mf_c.renderer.bounds.min.x;
					}
					
					if(mf_c.renderer.bounds.max.x>highestPointX)
					{
						highestPointX = mf_c.renderer.bounds.max.x;
					}
					
					if(mf_c.renderer.bounds.min.z<lowestPointZ)
					{
						lowestPointZ = mf_c.renderer.bounds.min.z;
					}
					
					if(mf_c.renderer.bounds.max.z>highestPointZ)
					{
						highestPointZ = mf_c.renderer.bounds.max.z;
					}
				}
			}
		}
		
		if(smfs)
		{
			lowestPointY = smfs.renderer.bounds.min.y;
			highestPointY = smfs.renderer.bounds.max.y;
		}
		
		if(smfs_arr.Length>0)
		{
			for(int i=0;i<smfs_arr.Length;i++)
			{
				SkinnedMeshRenderer smfs_c = (SkinnedMeshRenderer) smfs_arr[i];
				
				if(smfs_c)
				{
					if(smfs_c.renderer.bounds.min.y<lowestPointY)
					{
						lowestPointY = smfs_c.renderer.bounds.min.y;
					}
					
					if(smfs_c.renderer.bounds.max.y>highestPointY)
					{
						highestPointY = smfs_c.renderer.bounds.max.y;
					}
					
					if(smfs_c.renderer.bounds.min.x<lowestPointX)
					{
						lowestPointX = smfs_c.renderer.bounds.min.x;
					}
					
					if(smfs_c.renderer.bounds.max.x>highestPointX)
					{
						highestPointX = smfs_c.renderer.bounds.max.x;
					}
					
					if(smfs_c.renderer.bounds.min.z<lowestPointZ)
					{
						lowestPointZ = smfs_c.renderer.bounds.min.z;
					}
					
					if(smfs_c.renderer.bounds.max.z>highestPointZ)
					{
						highestPointZ = smfs_c.renderer.bounds.max.z;
					}
				}
			}
		}
		
		if(highestPointX - lowestPointX != -20000)
		{
			finalXSize = highestPointX - lowestPointX;
		} else { finalXSize = 1.0f; divX = 1.0f; lowestPointX = 0; Debug.Log ("X Something wrong with "+obj_rs.name); }
		
		if(highestPointY - lowestPointY != -20000)
		{
			finalYSize = highestPointY - lowestPointY;
		} else { finalYSize = globalYSize; divY = 1.0f; lowestPointY = 0; Debug.Log ("Y Something wrong with "+obj_rs.name); }
		
		if(highestPointZ - lowestPointZ != -20000)
		{
			finalZSize = highestPointZ - lowestPointZ;
		} else { finalZSize = 1.0f; divZ = 1.0f; lowestPointZ = 0; Debug.Log ("Z Something wrong with "+obj_rs.name); }
		
		for(int i=0;i<trms.Length;i++)
		{
			GameObject trm_go = (GameObject) ((Transform) trms[i]).gameObject;
			trm_go.layer = 2;
		}
		
		//BoxCollider obj_bc = obj.GetComponent<BoxCollider>();
		
		//if(!obj_bc)
		//{
		GameObject behindGO = new GameObject(obj.name);
		behindGO.AddComponent<BoxCollider>();
		obj.transform.parent = behindGO.transform;
		//}
		
		if(Mathf.Approximately(finalXSize,1.0f) || finalXSize<1.0f)
		{
			if(finalXSize<1.0f)
			{
				divX=1.0f;
				lowestPointX=-1.0f;
			}
			
			finalXSize=1.0f;
		}
		
		if(Mathf.Approximately(finalYSize,1.0f) || finalYSize<0.1f)
		{
			//	finalYSize=1.0f;
			//	divY=1.0f;
			//	lowestPointY=-1.0f;
		}
		
		if(Mathf.Approximately(finalYSize,0.0f)) { finalYSize = 0.01f; divY = 0.1f; lowestPointY = 0.0f; }
		
		if(Mathf.Approximately(finalZSize,1.0f) || finalZSize<1.0f)
		{
			if(finalZSize<1.0f)
			{
				divZ=1.0f;
				lowestPointZ=-1.0f;
			}
			
			finalZSize=1.0f;
		}
		behindGO.transform.localScale = objScale;
		((BoxCollider)behindGO.GetComponent(typeof(BoxCollider))).size = new Vector3(finalXSize,finalYSize,finalZSize);
		((BoxCollider)behindGO.GetComponent(typeof(BoxCollider))).center = new Vector3(finalXSize/divX+lowestPointX,finalYSize/divY+lowestPointY,finalZSize/divZ+lowestPointZ);
		
		
		
		//if(objScale.x<0.99||objScale.x>1.01||objScale.y>1.01||objScale.y<0.99||objScale.z>1.01||objScale.z<0.99)
		//	Debug.Log ("Warning: "+"("+obj.name+") is not using (1,1,1) localScale. This might couse some problems with map editor. We suggest to always use object scale = 1,1,1 and change mesh size instead.");
		
		DisableAllExternalColliders(obj);
		
		List<GameObject> twoGO = new List<GameObject>();
		twoGO.Add(behindGO);
		twoGO.Add(obj);
		
		return twoGO;
		
		//Destroy(obj);
	}

	private void DisableAllExternalColliders(GameObject obj)
	{
		BoxCollider[] boxColls = obj.GetComponentsInChildren<BoxCollider>();
		
		for(int i=0;i<boxColls.Length;i++)
		{
			BoxCollider coll = (BoxCollider) boxColls[i];
			if(coll) coll.enabled = false;
		}
		
		MeshCollider[] mrColls = obj.GetComponentsInChildren<MeshCollider>();
		
		for(int i=0;i<mrColls.Length;i++)
		{
			MeshCollider coll = (MeshCollider) mrColls[i];
			if(coll) coll.enabled = false;
		}
		
		SphereCollider[] spColls = obj.GetComponentsInChildren<SphereCollider>();
		
		for(int i=0;i<spColls.Length;i++)
		{
			SphereCollider coll = (SphereCollider) spColls[i];
			if(coll) coll.enabled = false;
		}
		
		CapsuleCollider[] cpColls = obj.GetComponentsInChildren<CapsuleCollider>();
		
		for(int i=0;i<cpColls.Length;i++)
		{
			CapsuleCollider coll = (CapsuleCollider) cpColls[i];
			if(coll) coll.enabled = false;
		}
	}
}
