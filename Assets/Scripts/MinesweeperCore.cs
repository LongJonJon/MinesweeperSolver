using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperCore : MonoBehaviour {
    public Vector2 boardSize = new Vector2 ();
    public int bombs = 0;
    public GameObject cellObject;
    protected List<List<GameObject>> board;
    public bool firstClick;
    int _gameOver = 0;
    public int gameOver { get => _gameOver; }
    void Start () => Setup ();

    public void Setup () {
        firstClick = true;
        _gameOver = 0;
        board = new List<List<GameObject>> ();

        GameObject p = new GameObject ("board");

        for (int y = 0; y < boardSize.y; y++) {
            List<GameObject> row = new List<GameObject> ();
            for (int x = 0; x < boardSize.x; x++) {
                GameObject s = Instantiate (cellObject, new Vector3 (x, y, 0), new Quaternion ());
                s.transform.SetParent (p.transform, true);
                s.name = $"{x}|{y}";
                row.Add (s);
            }
            board.Add (row);
        }

        p.transform.position = new Vector3 (-boardSize.x / 2 + 0.5f, -boardSize.y / 2 + 0.5f, 0);
    }

    private void FirstClick (Vector2 pointer) {
        firstClick = false;

        List<int> bombSpots = new List<int> ();
        for (var s = 0; s < (int) boardSize.x * (int) boardSize.y; s++) bombSpots.Add (s);
        if (bombs > bombSpots.Count - 9) bombs = bombSpots.Count - 9;

        for (var u = -1; u < 2; u++)
            for (var v = -1; v < 2; v++)
                if (u + pointer.x > -1 && u + pointer.x < boardSize.x && v + pointer.y > -1 && v + pointer.y < boardSize.y)
                    bombSpots.Remove ((int) (boardSize.y * (pointer.x + u) + pointer.y + v));

        for (var b = 0; b < bombs; b++) {
            int position = bombSpots[Mathf.RoundToInt (Random.Range (1, bombSpots.Count) - 1)];

            board[position % (int) boardSize.y][Mathf.FloorToInt (position / boardSize.y)].GetComponent<Cell> ().bomb = true;
            bombSpots.Remove (position);
        }

        for (int y = 0; y < boardSize.y; y++)
            for (int x = 0; x < boardSize.x; x++) {
                int _b = 0;
                for (var u = -1; u < 2; u++)
                    for (var v = -1; v < 2; v++)
                        if (u + x > -1 && u + x < boardSize.x && v + y > -1 && v + y < boardSize.y && !(v == 0 && u == v))
                            _b += (board[y + v][x + u].GetComponent<Cell> ().bomb) ? 1 : 0;
                board[y][x].GetComponent<Cell> ().near = _b;
            }
    }

    public void BombCount () {
        int check = 0;
        int count = 0;
        for (int y = 0; y < boardSize.y; y++)
            for (int x = 0; x < boardSize.x; x++) {
                check += (board[y][x].GetComponent<Cell> ().shown) ? 0 : 1;
                count += (board[y][x].GetComponent<Cell> ().marked) ? 1 : 0;
            }
        if (check == bombs && bombs == count) _gameOver = 1;
    }

    public void Cascade (Vector2 pointer) {
        if (firstClick) FirstClick (pointer);
        if (pointer.x == -2) return;
        if (pointer.x == -1) _gameOver = -1;
        else if (board[(int) pointer.y][(int) pointer.x].GetComponent<Cell> ().near == 0)
            for (var u = -1; u < 2; u++)
                for (var v = -1; v < 2; v++)
                    if (!(v == 0 && u == v) && u + pointer.x > -1 && u + pointer.x < boardSize.x && v + pointer.y > -1 && v + pointer.y < boardSize.y) {
                        Cell cell = board[(int) pointer.y + v][(int) pointer.x + u].GetComponent<Cell> ();
                        if (cell.shown) continue;
                        Cascade (cell.Show ());
                    }
    }

    public void Carve (string name) {
        Vector2 pointer = new Vector2 (float.Parse (name.Split ('|') [0]), float.Parse (name.Split ('|') [1]));
        int bombs = 0;
        for (var u = -1; u < 2; u++)
            for (var v = -1; v < 2; v++)
                if (!(v == 0 && u == v) && u + pointer.x > -1 && u + pointer.x < boardSize.x && v + pointer.y > -1 && v + pointer.y < boardSize.y)
                    bombs += (board[(int) pointer.y + v][(int) pointer.x + u].GetComponent<Cell> ().marked) ? 1 : 0;
        if (bombs >= board[(int) pointer.y][(int) pointer.x].GetComponent<Cell> ().near)
            for (var u = -1; u < 2; u++)
                for (var v = -1; v < 2; v++)
                    if (!(v == 0 && u == v) && u + pointer.x > -1 && u + pointer.x < boardSize.x && v + pointer.y > -1 && v + pointer.y < boardSize.y)
                        Cascade (board[(int) pointer.y + v][(int) pointer.x + u].GetComponent<Cell> ().Show ());
    }

    #region Solvers Functions
    public List<List<float>> GetBoard () {
        List<List<float>> _board = new List<List<float>> ();
        for (int y = 0; y < boardSize.y; y++) {
            List<float> row = new List<float> ();
            for (int x = 0; x < boardSize.x; x++)
                row.Add ((board[y][x].GetComponent<Cell> ().shown) ? board[y][x].GetComponent<Cell> ().near : (board[y][x].GetComponent<Cell> ().marked) ? -2 : -1);
            _board.Add (row);
        }
        return _board;
    }

    public void ShowWeights (List<List<float>> weights) {
        for (int y = 0; y < boardSize.y; y++)
            for (int x = 0; x < boardSize.x; x++) {
                board[y][x].GetComponent<Cell> ().weight = weights[y][x];
            }
    }

    public bool Mark (Vector2 target) => board[(int) target.y][(int) target.x].GetComponent<Cell> ().Mark (true);
    public void Show (Vector2 target) => Cascade (board[(int) target.y][(int) target.x].GetComponent<Cell> ().Show ());

    #endregion
}