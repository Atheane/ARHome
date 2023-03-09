using TMPro;
using UnityEngine;
using Vuforia;




public class UIController : MonoBehaviour
{
    [SerializeField]
    bool isCarrouselActive;
    [SerializeField]
    GameObject buttonMenu;
    [SerializeField]
    GameObject buttonErase;
    [SerializeField]
    GameObject currentProducImage;
    [SerializeField]
    Sprite[] productImages;

    RectTransform productImageRectTransform;
    Vector2 startSwipePosition;
    Vector2 direction;
    GameObject arPlane;
    Camera mainCamera;
    ProductType currentProduct;

    static Rect RECT_DIMENSION;

    // buttonMenu onClick
    public void ShowCarrousel() {
        this.isCarrouselActive = true;
        this.buttonMenu.SetActive(false);
        this.buttonErase.SetActive(false);
        this.currentProducImage.SetActive(true);
        this.arPlane.SendMessage("OnPreview", this.currentProduct);
    }


    public void HideCarrousel() {
        Debug.Log("HideCarrousel");
        this.isCarrouselActive = false;
        this.buttonMenu.SetActive(true);
        this.buttonErase.SetActive(this.arPlane.GetComponent<PlaneController>().HasPlacedProducts);
        this.currentProducImage.SetActive(false);
    }

    public void HideEraseButton() {
        this.buttonErase.SetActive(false);
    }

    public void OnCarrouselTouched() {
        if (this.direction.x > 100) {
            Debug.Log($"SWIPE_RIGHT direction.x: {direction.x}");
            if ((int)this.currentProduct > 0) {
                this.currentProduct -= 1;
            }
            this.ShowPreview2D();
            this.arPlane.SendMessage("OnPreview", this.currentProduct);
        } else if (this.direction.x < -100) {
            Debug.Log($"SWIPE_LEFT direction.x: {this.direction.x}");
            if ((int)this.currentProduct < this.productImages.Length - 1) {
                this.currentProduct += 1;
            }   
            this.ShowPreview2D();
            this.arPlane.SendMessage("OnPreview", this.currentProduct);
        }
    }

    void Start()
    {
        this.arPlane = GameObject.FindWithTag("Plane");
        this.currentProduct = ProductType.CHAIR;
        this.productImageRectTransform = this.currentProducImage.GetComponent<RectTransform>();
        RECT_DIMENSION = RectTransformUtility.PixelAdjustRect(this.productImageRectTransform, this.GetComponent<Canvas>());
        this.mainCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
        this.HideCarrousel();
    }

    void Update() {

        // 1. Check if only one finger. If no or more than one finder, no UI action
        if (Input.touchCount != 1) {
            return;
        }

        // 2. Check if the carrousel is touched
        Touch touch = Input.GetTouch(0);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.productImageRectTransform, touch.position, this.mainCamera, out Vector2 localPoint); 
        if (localPoint.y > RECT_DIMENSION.height + RECT_DIMENSION.y) {
            return;
        }
        // At this point, we know that the image is touched with one finger only

        // 3. Detect if swipe
        switch (touch.phase)
        {
            case TouchPhase.Began:
                this.startSwipePosition = touch.position;
                break;
            case TouchPhase.Moved:
                direction = touch.position - this.startSwipePosition;
                break;
        }

    }

    void ShowPreview2D() {
        this.currentProducImage.GetComponent<UnityEngine.UI.Image>().sprite = this.productImages[(int)this.currentProduct];
    }
}
