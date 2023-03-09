using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public enum ProductState {
    UNSHOWN,
    PREVIEWED,
    PLACED,
    SELECTED,
    DESELECTED
}

public enum ProductType {
    CHAIR,
    COFFEETABLE,
    SOFA,
    TABLE,
    TV
}

public class ProductController : MonoBehaviour
{
    [Header("Product Properties")]
    [SerializeField] ProductType type;
    [SerializeField] int id;
    [SerializeField] Material standardMaterial;
    [SerializeField] Material transparentMaterial;
    [SerializeField] Material highlightMaterial;
    GameObject translationIndicator;
    GameObject rotationIndicator;
    MeshRenderer renderer;

    [Header("Product State")]
    [SerializeField] ProductState state = ProductState.UNSHOWN;

    public ProductState State {
        get {
            return state;
        }
        private set {
            state = value;
            Debug.Log($"ProductController - set State @{this.state}");
        }
    }

    public int Id {
        get {
            return id;
        }
        private set {
            if (id != null && id != 0) {
                throw new Exception($"Product Id already set @{id}");
            }
            id = value;
            Debug.Log($"ProductController - set Id @{id}");
        }
    }

    public ProductType Type {
        get {
            return type;
        }
        set {
            if (type != null) {
                throw new Exception($"Product Type already set @{type}");
            }
            type = value;
            Debug.Log($"ProductController - set Type @{type}");
        }
    }

    void OnPreviewed(ProductType type) {
        Debug.Log("---OnPreviewed----");
        this.state = ProductState.PREVIEWED;
        this.type = type;
        TextInfo myTI = new CultureInfo("fr-FR",false).TextInfo;
        string materialName = myTI.ToTitleCase(myTI.ToLower(type.ToString()));
        this.standardMaterial = Resources.Load<Material>($"Materials/{materialName}Material");
        this.transparentMaterial = Resources.Load<Material>($"Materials/{materialName}MaterialTransparent");
        this.highlightMaterial = Resources.Load<Material>($"Materials/{materialName}MaterialHighLight");
        this.renderer = this.gameObject.GetComponent<MeshRenderer>();
        this.renderer.enabled = true;
        this.renderer.materials = new[] { this.transparentMaterial };
    }

    void OnPlaced(int id) {
        Debug.Log("---OnPlaced----");
        this.id = id;
        this.state = ProductState.PLACED;
        this.renderer.materials = new[] { this.standardMaterial };
    }

    void OnSelected() {
        Debug.Log("---OnSelected----");
        this.state = ProductState.SELECTED;
        this.renderer.materials = new[] { this.highlightMaterial };
    }

    void OnDeselected() {
        Debug.Log("---OnDeselected----");
        this.state = ProductState.DESELECTED;
        this.renderer.materials = new[] { this.standardMaterial };
    }
}
