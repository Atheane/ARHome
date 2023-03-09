using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Vuforia;
using System;
using System.Globalization;
using System.Collections.Generic;

public class PlaneController : MonoBehaviour
{
    [SerializeField]
    GameObject groundPlane;
    [SerializeField]
    GameObject uiPanel;

    [Header("Current Augmentation Object")]
    [SerializeField] GameObject currentAugmentationObject = null;
    ProductController currentProductController;
    ProductSO currentProductData;
    ProductType currentProductType;

    [Header("Other Augmentation Objects")]
    [SerializeField]
    GameObject[] productTypes;
    [SerializeField]
    List<int> productIds;

    const string GROUND_PLANE_NAME = "Emulator Ground Plane";
    const string FLOOR_NAME = "Floor";

    Camera mainCamera;
    string mFloorName;
  
    bool isActive;
    public bool IsPlaneControllerActive {
        get { return isActive ;}
        private set { isActive = value ;}
    }

    bool hasPlacedProducts;
    public bool HasPlacedProducts {
        get { return productIds.Count > 0; }
    }

    public void OnTouch()
    {
        Debug.Log("----Place----");
        if (!this.IsPlaneControllerActive) return;

        if (this.currentProductController.State == ProductState.PREVIEWED) {
            this.currentAugmentationObject.SendMessage("OnPlaced", this.productIds.Count);
            this.productIds.Add(this.currentProductController.Id);
            return;
        }

        GameObject target = null;
        Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction*10, out var hit)) {
            target = hit.collider.gameObject;
        }
        if (!target.TryGetComponent<ProductController>(out ProductController productController)) return;
        this.currentAugmentationObject = target;
        this.SetProperties(this.currentProductType);

        if (this.currentProductController.State != ProductState.SELECTED) {
            this.currentAugmentationObject.SendMessage("OnSelected");
            this.uiPanel.SetActive(true);
            this.uiPanel.GetComponentInChildren<TMP_Text>().text = $"{this.currentProductData.Title} \n\nPrice: {this.currentProductData.Price}€ \n\nDimensions: {this.currentProductData.Dimensions} \n\nDescription: {this.currentProductData.Description}";
        } else {
            this.currentAugmentationObject.SendMessage("OnDeselected");
            this.uiPanel.SetActive(false);
        }
    }

    /// true if AR Camera raycast encounter a plane.
    // result gives the hit point position
    public void OnAutomaticHitTest(HitTestResult result)
    {
        if (!this.IsPlaneControllerActive) return;

        if (this.currentProductController.State == ProductState.PREVIEWED) {
            this.currentAugmentationObject.transform.position = result.Position;
            return;
        }
    }

    public void EraseAllProducts() {
        foreach (Transform childTransform in this.transform.GetComponentsInChildren<Transform>())
        {
            if(childTransform == this.transform) continue;
            if(childTransform.gameObject.TryGetComponent<ProductController>(out ProductController productController)) {
                Destroy(childTransform.gameObject);
            }
        }
        this.Reset();
        this.isActive = false;
        this.productIds = new List<int>();
        Debug.Log("ALL_PRODUCTS_ERASED");
    }

    void Start()
    {
        this.mainCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
        this.SetupFloor();
        this.isActive = false;
        this.productIds = new List<int>();
    }

    void Reset()
    {
        this.currentAugmentationObject = null;
        this.currentProductController = null;
        this.currentProductData = null;
        this.currentProductType = ProductType.CHAIR;
        this.uiPanel.GetComponentInChildren<TMP_Text>().text = "";
        this.uiPanel.SetActive(false);
    }

    void OnPreview(ProductType productType) {
        if (this.currentAugmentationObject != null && this.currentProductController.State == ProductState.PREVIEWED) {
            Destroy(this.currentAugmentationObject);
        }
        this.isActive = true;
        this.InstantiatePrefab(productType);
        this.SetProperties(productType);
        this.currentAugmentationObject.SendMessage("OnPreviewed", productType);
    }

    // helpers
    void InstantiatePrefab(ProductType productType) {
        this.currentAugmentationObject = Instantiate(
            this.productTypes[(int)productType],
            this.groundPlane.transform.position,
            this.productTypes[(int)productType].transform.rotation
        );
        this.currentAugmentationObject.AddComponent<ProductController>();
        this.currentAugmentationObject.transform.SetParent(this.transform);
    }

    void SetProperties(ProductType productType)
    {
        Debug.Log("SetProperties");
        this.currentProductType = productType;
        this.currentProductController = this.currentAugmentationObject.GetComponent<ProductController>();
        this.currentProductData = this.currentAugmentationObject.GetComponent<ProductData>().Data;
    }

    void SetupFloor()
    {
        if (VuforiaRuntimeUtilities.IsPlayMode())
            mFloorName = GROUND_PLANE_NAME;
        else
        {
            mFloorName = FLOOR_NAME;
            var floor = new GameObject(mFloorName, typeof(BoxCollider));
            floor.transform.SetParent(this.currentAugmentationObject.transform.parent);
            floor.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            floor.transform.localScale = Vector3.one;
            floor.GetComponent<BoxCollider>().size = new Vector3(100f, 0, 100f);
        }
    }

}

   