using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text  titleText;
   //  [SerializeField] private TMP_Text priceText;
   //  [SerializeField] private TMP_Text descriptionText;
   //  [SerializeField] private Image productImage;

    private ProductDTO currentProduct;

    // void Awake()
    // {
        
    //     if (titleText == null)
    //     {
    //         titleText = GetComponentInChildren<TextMeshProUGUI>();
    //         if (titleText != null)
    //         {
    //             Debug.Log("ProductDisplay: auto-found titleText on " + titleText.gameObject.name);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("ProductDisplay: No TextMeshProUGUI found in children");
    //         }
    //     }
    // }
    void Awake()
{
    if (titleText == null)
        titleText = GetComponentInChildren<TMP_Text>();
}

    void OnEnable()
    {
        Debug.Log("ProductDisplay enabled on: " + gameObject.name);
    }

    public void SetProduct(ProductDTO product)
    {
        currentProduct = product;
        Debug.Log("SetProduct called with: " + (product != null ? product.title : "NULL"));
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (currentProduct == null)
        {
            Debug.LogWarning("currentProduct is null!");
            return;
        }

        Debug.Log("UpdateDisplay for: " + currentProduct.title);

        // Set text fields
        if (titleText != null)
        {
            titleText.text = currentProduct.title;
            Debug.Log("Title set: " + currentProduct.title);
        }
        else
        {
            Debug.LogWarning("titleText is not assigned!");
        }
         Debug.Log("titleText reference = " + titleText);

      //   if (priceText != null)
      //       priceText.text = $"{currentProduct.currency} {currentProduct.price}";
      //   else
      //       Debug.LogWarning("priceText is not assigned!");

      //   if (descriptionText != null)
      //       descriptionText.text = currentProduct.description;
      //   else
      //       Debug.LogWarning("descriptionText is not assigned!");

        // Set product image if available
      //   if (productImage != null && currentProduct.imageUrl != null && currentProduct.imageUrl.Count > 0)
      //   {
      //       string imageUrl = currentProduct.imageUrl[0].url;
      //       Debug.Log("Loading image from: " + imageUrl);
      //   }
      //   else if (productImage == null)
      //   {
      //       Debug.LogWarning("productImage is not assigned!");
      //   }
    }
}
