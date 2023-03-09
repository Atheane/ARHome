using UnityEngine;

[CreateAssetMenu(fileName="productSO", menuName="ARHome/productSO", order=1)]
public class ProductSO : ScriptableObject
{
    [SerializeField]
    private string title = "Chaise style scandinave";
    public string Title {
        get { return title ;}
        set { title = value ;}
    }
    [SerializeField]
    private float price = 22.99f;
    public float Price {
        get { return price ;}
        set { price = value ;}
    }
    [SerializeField]
    private string dimensions = "L50 x P54.5 x H80 cm";
    public string Dimensions {
        get { return dimensions ;}
        set { dimensions = value ;}
    }
    [SerializeField]
    private string description = "La jolie harmonie entre le bois clair et la couleur blanche de leur assise et de leur dossier fait de ces chaises de cuisine ou salle à manger de beaux sièges nordiques au style délicat.";
    public string Description {
        get { return description ;}
        set { description = value ;}
    }
}
