using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperSolver : MonoBehaviour {
    public float delay = 0;
    public bool running = false;
    float time = 0;
    void Update () {
        if (!running) return;

        MinesweeperCore mc = this.GetComponent<MinesweeperCore> ();
        if (mc.gameOver != 0) {
            running = false;
            return;
        }

        if (mc.firstClick) mc.Cascade (new Vector2 (Mathf.Round (Random.Range (0, mc.boardSize.x - 1)), Mathf.Round (Random.Range (0, mc.boardSize.y - 1))));

        time += Time.deltaTime;
        if (time >= delay) {
            time -= delay;
            Step ();
        }
    }

    void Step () {
        MinesweeperCore MC = this.GetComponent<MinesweeperCore> ();
        List<List<float>> board = MC.GetBoard ();
        Vector2 boardSize = new Vector2 (board[0].Count, board.Count);

        bool _stuck = true;
        for (var y = 0; y < boardSize.y; y++)
            for (var x = 0; x < boardSize.x; x++) {
                float _unseen = 0;
                float _known = 0;
                for (var u = -1; u < 2; u++)
                    for (var v = -1; v < 2; v++)
                        if (!(v == 0 && u == v) && u + x > -1 && u + x < boardSize.x && v + y > -1 && v + y < boardSize.y) {
                            _unseen += (board[y + v][x + u] == -1) ? 1 : 0;
                            _known += (board[y + v][x + u] == -2) ? 1 : 0;
                        }
                if (board[y][x] > 0)
                    for (var u = -1; u < 2; u++)
                        for (var v = -1; v < 2; v++)
                            if (!(v == 0 && u == v) && u + x > -1 && u + x < boardSize.x && v + y > -1 && v + y < boardSize.y) {
                                if (board[y][x] - _known == _unseen && board[y + v][x + u] == -1) {
                                    MC.Mark (new Vector2 (x + u, y + v));
                                    MC.BombCount ();
                                    _stuck = false;
                                } else if (board[y][x] - _known == 0) MC.Carve ($"{x + u}|{y + v}");
                            }
            }
        if (!_stuck) return;

        // put in big function

        // It needs to find all the linked numbers put them in a 2d reference array, 
        // and a weights array, then create an array for possible outcomes fill it with binary and check if it is a possibility option in the network
        // if it is increment the count and add 1 to each spot in the weights array that matches after it goes from 0 to 2^x then the weight of each would be it's weights array index / count

        board = MC.GetBoard ();
        List<List<float>> weights = new List<List<float>> ();
        for (var y = 0; y < boardSize.y; y++) {
            List<float> row = new List<float> ();
            for (var x = 0; x < boardSize.x; x++)
                row.Add (0);
            weights.Add (row);
        }

        for (var y = 0; y < boardSize.y; y++)
            for (var x = 0; x < boardSize.x; x++)
                for (var u = -1; u < 2; u++)
                    for (var v = -1; v < 2; v++)
                        if (!(v == 0 && u == v) && u + x > -1 && u + x < boardSize.x && v + y > -1 && v + y < boardSize.y && board[y + v][x + u] == -1 && board[y][x] >= 0)
                            weights[y + v][x + u] += 1;
        MC.ShowWeights (weights);

        List<Vector3> pos = new List<Vector3> ();

        for (var y = 0; y < boardSize.y; y++)
            for (var x = 0; x < boardSize.x; x++)
                if (weights[y][x] > 0) {
                    pos.Add (new Vector3 (x, y, 0));
                }

        List<List<float>> network = new List<List<float>> ();

    }
}