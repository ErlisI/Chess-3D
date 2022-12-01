using UnityEngine;

public class ChessBoard : MonoBehaviour
{

   //Tells the unity engine to save/restore it's state to/from disk
  [Header("Art")]
  [SerializeField] private Material tileMat;
  [SerializeField] private float tileSize = 1.0f;
  [SerializeField] private float Yoffset = 0.2f;
  [SerializeField] private Vector3 boardCenter = Vector3.zero;

  [Header("Prefabs & Materials")]
  [SerializeField] private GameObject[] prefabs;
  //[SerializeField] private Material[] pieceColorMaterial;

  //Game Logic
  private GameObject[,] tiles;
  private Camera currCam;
  private Vector2Int currHover;
  private Vector3 bounds;
  private ChessPiece[,] chessPieces;

  void Awake() {
    generateTiles(tileSize, 8, 8);
    spawnPieces();
    positionPieces();
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
  //Spawning all the chess pieces
  private void spawnPieces() {

    chessPieces = new ChessPiece[8, 8]; //creating 64 pieces but most of them will be NULL

    //Spawning the white pieces
    chessPieces[0,0] = spawnPiece(pieceType.wRook);
    chessPieces[1,0] = spawnPiece(pieceType.wKnight);
    chessPieces[2,0] = spawnPiece(pieceType.wBishop);
    chessPieces[3,0] = spawnPiece(pieceType.wKing);
    chessPieces[4,0] = spawnPiece(pieceType.wQueen);
    chessPieces[5,0] = spawnPiece(pieceType.wBishop);
    chessPieces[6,0] = spawnPiece(pieceType.wKnight);
    chessPieces[7,0] = spawnPiece(pieceType.wRook);

    for(int i = 0; i < 8; i++)
      chessPieces[i,1] = spawnPiece(pieceType.wPawn);

    //Spawning the black pieces
    chessPieces[0,7] = spawnPiece(pieceType.bRook);
    chessPieces[1,7] = spawnPiece(pieceType.bKnight);
    chessPieces[2,7] = spawnPiece(pieceType.bBishop);
    chessPieces[3,7] = spawnPiece(pieceType.bQueen);
    chessPieces[4,7] = spawnPiece(pieceType.bKing);
    chessPieces[5,7] = spawnPiece(pieceType.bBishop);
    chessPieces[6,7] = spawnPiece(pieceType.bKnight);
    chessPieces[7,7] = spawnPiece(pieceType.bRook);

    for(int i = 0; i < 8; i++)
      chessPieces[i,6] = spawnPiece(pieceType.bPawn);

  }
  //Spawning one chess piece
  private ChessPiece spawnPiece(pieceType type) {
    ChessPiece cP = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
    cP.type = type;
    return cP;
  }

  private void positionPieces() {
    for(int i = 0; i < 8; i++)
      for(int j = 0; j < 8; j++)
        if(chessPieces[i,j] != null)
          positionPiece(i, j, true);
  }

  private void positionPiece(int pieceX, int pieceY, bool f = false) { //defaluting f (force) to false so when we move we dont just teleport but actually move
    chessPieces[pieceX, pieceY].currX = pieceX;
    chessPieces[pieceX, pieceY].currY = pieceY;
    chessPieces[pieceX, pieceY].transform.position = new Vector3(pieceX * tileSize, Yoffset, pieceY * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2); //sets it to the tile center
  }
}











