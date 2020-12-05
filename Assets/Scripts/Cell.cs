using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour {
    public TextMeshPro nearText;
    public Material baseM;
    public Material hoverM;
    public Material markedM;
    public GameObject panel;
    public GameObject bombObject;
    public TextMeshPro prob;
    public float weight {
        set { prob.text = (!_shown && value != -1 && value != 0 && !_marked) ? $"{Mathf.Round(value*100)/100}" : ""; }
    }

    int _near = 0;
    bool _bomb = false;
    bool _shown = false;
    bool _marked = false;

    public bool marked { get => _marked; }
    public bool shown { get => _shown; }

    public bool bomb {
        get => _bomb;
        set {
            bombObject.GetComponent<MeshRenderer> ().enabled = value;
            _bomb = value;
        }
    }

    public int near {
        get => _near;
        set {
            nearText.text = (!bomb && value > 0) ? $"{value}" : "";
            _near = value;
        }
    }

    public Vector2 Show () {
        if (marked) return new Vector2 (-2, -2);

        prob.text = "";
        _shown = true;

        this.gameObject.GetComponent<MeshRenderer> ().enabled = false;
        this.gameObject.GetComponent<BoxCollider> ().enabled = false;

        if (_bomb) return new Vector2 (-1, -1);
        else return new Vector2 (float.Parse (this.name.Split ('|') [0]), float.Parse (this.name.Split ('|') [1]));
    }

    public bool Mark () => _marked = !_marked;
    public bool Mark (bool b) => _marked = b;
    public void Hover () => this.gameObject.GetComponent<MeshRenderer> ().material = panel.GetComponent<MeshRenderer> ().material = (marked) ? markedM : hoverM;
    private void Update () => this.gameObject.GetComponent<MeshRenderer> ().material = panel.GetComponent<MeshRenderer> ().material = (marked) ? markedM : baseM;
}