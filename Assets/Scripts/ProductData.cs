using UnityEngine;

public class ProductData : MonoBehaviour
{
    [SerializeField]
    private ProductSO data;

    public ProductSO Data {
        get {
            return data;
        }
    }
}
