using UnityEngine;

public class ChessBoard : MonoBehaviour
{

   //tells the unity engine to save/restore it's state to/from disk
  [Header("Art")]
  [SerializeField] private Material tileMat;
  [SerializeField] private float tileSize = 1.0f;
  [SerializeField] private float Yoffset = 0.2f;
  [SerializeField] private Vector3 boardCenter = Vector3.zero;

  //Game Logic
  private GameObject[,] tiles;
  private Camera currCam;
  private Vector2Int currHover;
  private Vector3 bounds;

  void Awake() {
    generateTiles(tileSize, 8, 8);
  }

  private void Update() {

    //checking if we have a current camera
    if(!currCam){
      currCam = Camera.main;
      return;
    }

    //crating ray-cast
    RaycastHit clickInfo;
    Ray ray = currCam.ScreenPointToRay(Input.mousePosition);

    if(Physics.Raycast(ray, out clickInfo, 100, LayerMask.GetMask("Tile"))) {
      Vector2Int clickPos = getTileIndex(clickInfo.transform.gameObject);

      //when we are not hovering on any tiles
      if(currHover == -Vector2Int.one){
        currHover = clickPos;
        tiles[clickPos.x, clickPos.y].layer = LayerMask.NameToLayer("Hovered");
      }

      if(currHover != -Vector2Int.one){

        tiles[currHover.x, currHover.y].layer = LayerMask.NameToLayer("Tile");
        currHover = clickPos;
        tiles[clickPos.x, clickPos.y].layer = LayerMask.NameToLayer("Hovered");
      }

    }else {
      if(currHover != -Vector2Int.one){

        tiles[currHover.x, currHover.y].layer = LayerMask.NameToLayer("Tile");
        currHover = -Vector2Int.one;
      }
    }
  }

  //generates all the tiles of the board
  private void generateTiles(float tileSize, int tilesX, int tilesY){

    Yoffset += transform.position.y;
    bounds = new Vector3((tilesX / 2) * tileSize, 0, (tilesX / 2) * tileSize) + boardCenter;

    tiles = new GameObject[tilesX, tilesY];

    for(int i = 0; i < tilesX; i++)
      for(int j = 0; j < tilesY; j++)
        tiles[i,j] = generateTile(tileSize, i, j); //generates all the tiles one by one
  }

  //function to generate a tile
  private GameObject generateTile(float tileSize, int tileX, int tileY){


    GameObject tileObj = new GameObject(string.Format("X:{0}, Y:{1}", tileX, tileY));
    tileObj.transform.parent = transform;

    //mesh object
    Mesh mesh = new Mesh();
    tileObj.AddComponent<MeshFilter>().mesh = mesh;
    tileObj.AddComponent<MeshRenderer>().material = tileMat;

    //generating the geometry of the tile
    Vector3[] vertices = new Vector3[4];
    vertices[0] = new Vector3(tileX * tileSize, Yoffset, tileY * tileSize) - bounds;
    vertices[1] = new Vector3(tileX * tileSize, Yoffset, (tileY + 1) * tileSize) - bounds;
    vertices[2] = new Vector3((tileX + 1) * tileSize, Yoffset, tileY * tileSize) - bounds;
    vertices[3] = new Vector3((tileX + 1) * tileSize, Yoffset, (tileY + 1) * tileSize) - bounds;

    int[] triangles = new int[] {0, 1, 2, 1, 3, 2}; //to render the geometry with the right order

    mesh.vertices = vertices;
    mesh.triangles = triangles;

    tileObj.layer = LayerMask.NameToLayer("Tile"); //assing the layer name to "Tile"

    //collider for the ray-cast
    tileObj.AddComponent<BoxCollider>();

    mesh.RecalculateNormals(); //calculating proper lighting

    return tileObj;
  }

  private Vector2Int getTileIndex(GameObject clickInfo) {
    for(int i = 0; i < 8; i++)
      for(int j = 0; j < 8; j++)
        if(tiles[i,j] == clickInfo)
          return new Vector2Int(i, j);

    return -Vector2Int.one; // returns -1 -1 *ERROR*
  }
}
