using UnityEngine;

public enum pieceType {
  None = 0,
  wPawn = 1,
  wBishop = 2,
  wKnight = 3,
  wRook = 4,
  wQueen = 5,
  wKing = 6,
  bPawn = 7,
  bBishop = 8,
  bKnight = 9,
  bRook = 10,
  bQueen = 11,
  bKing = 12
};

public class ChessPiece : MonoBehaviour
{
  public int pieceColor;
  public int currX, currY;
  public pieceType type;

  private Vector3 piecePosition;
  private Vector3 pieceScale;
}
