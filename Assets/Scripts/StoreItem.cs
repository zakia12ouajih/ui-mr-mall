using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

[System.Serializable]
public class ProductMediaDTO
{
    public string id;
    public string type;
    public string url;
}

[System.Serializable]
public class ProductDTO
{
    public string id;
    public string title;
    public string description;
    public float price;
    public string currency;
    public int stock;
    public string status;

    public StoreDTO store;
    public CategoryDTO category;

    public List<ProductMediaDTO> imageUrl;
}


[System.Serializable]
public class CategoryDTO
{
    public string id;
    public string name;
    public string imageUrl;
    public string description;
}


[System.Serializable]
public class StoreDTO
{
    public string id;
    public string name;
    public string slug;
    public string categoryId;
    public string description;
}

public class StoreItem : MonoBehaviour
{
    // app state for "back" btn
    enum AppState
    {
        Welcome,
        Categories,
        FloorView,
        CategoryStores,
        SearchResults 
    }

    AppState currentState;

    [Header("UI References")]
    public GameObject cardPrefabStore;
    public Transform contentParent;
    public TMP_Dropdown floorDropdown;
    public GameObject scrollViewStores;
    public Button goButton; // Reference to Go button
    
    [Header("Search UI")]
    public TMP_InputField searchInput;
    public Button searchButton; // Add reference to search button
    public Button clearSearchButton; // Optional: add clear button

    [Header("Category Prefab")]
    public GameObject categoryPrefab;     // prefab for each category
    public Transform contentCategoryParent; 
    public GameObject scrollViewCategory; 

    [Header("Product Prefab")]
    public GameObject productPrefab;  
    public Transform contentProductParent;
    public GameObject scrollViewProduct;
    
    [Header("Mock Logos")]
    public List<Sprite> logos;

    private int selectedFloorValue; // Store the currently selected dropdown value
    
    // Store currently selected category to maintain state
    private string currentCategory;
    private string lastSearchQuery; // Store last search query

    private Dictionary<string, Sprite> iconMap;

    void Start()
    {
        
        // SetupDropdown();
        // SetupUI();

        StartCoroutine(InitApp());
    }
    IEnumerator InitApp()
    {
        yield return StartCoroutine(LoadCategories());
        yield return StartCoroutine(LoadStores());
        yield return StartCoroutine(LoadProducts());

        // later:
        // yield return StartCoroutine(LoadCart());
        // yield return StartCoroutine(LoadWishlist());

        ShowCategories(); 
    }

    // void SetupUI()
    // {
    //     if (goButton != null)
    //     {
    //         goButton.onClick.RemoveAllListeners();
    //         goButton.onClick.AddListener(OnGoButtonClicked);
    //     }

    //     if (searchButton != null)
    //     {
    //         searchButton.onClick.RemoveAllListeners();
    //         searchButton.onClick.AddListener(OnSearchButtonClicked);
    //     }

    //     if (searchInput != null)
    //     {
    //         searchInput.onSubmit.AddListener((value) => OnSearchButtonClicked());
    //     }
    // }
    
    // products 
    

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    void SetText(GameObject card, string path, string text)
    {
        Transform target = card.transform.Find(path);
        if (target != null)
        {
            TextMeshProUGUI tmp = target.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = text;
            else Debug.LogWarning($"TextMeshProUGUI component not found at path: {path}");
        }
        else
        {
            Debug.LogWarning($"Transform not found at path: {path}");
        }
    }

// =================== categories ======================================
    List<CategoryDTO> categoriesData = new List<CategoryDTO>();

    IEnumerator LoadCategories()
    {
        Debug.Log("ApiManager: " + ApiManager.Instance);
        yield return StartCoroutine(ApiManager.Instance.Get("/categories",
            (response) =>
            {
                CategoryDTO[] data = JsonHelper.FromJson<CategoryDTO>(response);

                categoriesData = data.ToList();

                Debug.Log("Categories loaded: " + categoriesData.Count);
            },
            (error) => Debug.LogError(error)
        ));
        
    }
    
    void ShowCategories()
    {
        for (int i = contentCategoryParent.childCount - 1; i >= 0; i--)
            Destroy(contentCategoryParent.GetChild(i).gameObject);

        foreach (var category in categoriesData)
        {
            GameObject card = Instantiate(categoryPrefab, contentCategoryParent);

            SetText(card, "CategoryName", category.name);
            SetText(card, "CategoryPreview", category.description);

            Image iconImage = card.transform.Find("icon")?.GetComponent<Image>();
            if (iconImage != null && !string.IsNullOrEmpty(category.imageUrl))
            {
                string fullUrl = "http://localhost:5089" + category.imageUrl;
                StartCoroutine(LoadImage(fullUrl, iconImage));
            }

            Button btn = card.transform.Find("viewProducts")?.GetComponent<Button>();

            if (btn != null)
            {
                string capturedCategory = category.id;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Debug.Log("CLICKED CATEGORY: " + capturedCategory);
                    ShowProductsByCategory(capturedCategory);
                });
            }
            else
            {
                Debug.LogError("viewProduct button not found in prefab");
            }
        }

        scrollViewCategory.SetActive(true);
        scrollViewStores.SetActive(false);
        scrollViewProduct.SetActive(false);

        currentState = AppState.Categories;
    }

// ================================ Store ============================================
    List<StoreDTO> storesData = new List<StoreDTO>();

    IEnumerator LoadStores()
    {
        yield return StartCoroutine(ApiManager.Instance.Get("/stores",
            (response) =>
            {
                StoreDTO[] raw = JsonHelper.FromJson<StoreDTO>(response);

                var list = raw != null ? raw.ToList() : new List<StoreDTO>();

                storesData = list.Select(s => new StoreDTO
                {
                    id = s.id,
                    name = s.name,
                    // description = s.description
                }).ToList();

                Debug.Log($"Stores loaded: {storesData.Count}");
            },
            (error) => Debug.LogError("LOAD STORES ERROR: " + error)
        ));
    }

    void ShowStores()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var store in storesData)
        {
            GameObject card = Instantiate(cardPrefabStore, contentParent);

            SetText(card, "StoreName", store.name);
            SetText(card, "StoreDescription", store.description);

            Button btn = card.GetComponentInChildren<Button>();
            if (btn != null)
            {
                string captured = store.id;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Debug.Log("Clicked store: " + captured);
                });
            }
        }

        scrollViewStores.SetActive(true);
        scrollViewCategory.SetActive(false);
    }

    IEnumerator LoadImage(string url, Image target)
    {
        using (UnityEngine.Networking.UnityWebRequest request =
            UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture =
                    ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;

                target.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
            else
            {
                Debug.LogError("Image load failed: " + url);
            }
        }
    }

// ====================== Product ======================================

    List<ProductDTO> productsData = new List<ProductDTO>();
    IEnumerator LoadProducts()
    {
        yield return StartCoroutine(ApiManager.Instance.Get("/products",
            (response) =>
            {
                Debug.Log("RAW RESPONSE: " + response);
                ProductDTO[] data = JsonHelper.FromJson<ProductDTO>(response);

                productsData = data.ToList();

                Debug.Log("Products loaded: " + productsData.Count);
                foreach (var p in productsData)
                {
                    Debug.Log("PRODUCT: " + p.title +
                        " | category: " + p.category?.id +
                        " | name: " + p.category?.name);
                }            },
            (error) => Debug.LogError("LOAD PRODUCTS ERROR: " + error)
        ));
        for (int i = 0; i < productsData.Count; i++)
        {
            string productId = productsData[i].id;

            yield return StartCoroutine(ApiManager.Instance.Get($"/products/{productId}/media",
                (mediaResponse) =>
                {
                    ProductMediaDTO[] media =
                        JsonHelper.FromJson<ProductMediaDTO>(mediaResponse);

                    productsData[i].imageUrl = media.ToList();
                },
                (error) => Debug.LogError(error)
            ));
        }
        Debug.Log("Products + media fully loaded");
    }

    void ShowProductsByCategory(string categoryId)
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        var filtered = productsData
        .Where(p => p.category != null && p.category.id == categoryId)
        .ToList();

        foreach (var p in productsData)
        {
            Debug.Log($"PRODUCT: {p.title} | CATEGORY: {p.category?.id} | NAME: {p.category?.name}");
        }

        foreach (var product in filtered)
        {
            Debug.Log("MATCHED: " + product.title + " -> " + product.category.id);
            CreateProductCard(product);
        }

        scrollViewStores.SetActive(false);
        scrollViewCategory.SetActive(false);
        scrollViewProduct.SetActive(true);

        currentState = AppState.SearchResults;
    }

    void CreateProductCard(ProductDTO data)
    {
        GameObject go = Instantiate(productPrefab, contentProductParent);

        TMP_Text[] texts = go.GetComponentsInChildren<TMP_Text>();
        Image img = go.transform.Find("Logo")?.GetComponent<Image>();
        if (texts.Length >= 3)
        {
            texts[0].text = data.title;
            texts[1].text = data.store != null ? data.store.name : "Unknown store";
            texts[2].text = data.price.ToString("0.00") + " " + data.currency;
        }

        // image (if you already stored URL list)
        Debug.Log("IMAGE COUNT: " + (data.imageUrl?.Count ?? 0));        if (data.imageUrl != null && data.imageUrl.Count > 0)
        {
            var imgUrl = data.imageUrl
                .FirstOrDefault(x => x.type == "IMAGE")?.url;

            if (!string.IsNullOrEmpty(imgUrl))
            {
                string fullUrl = "http://localhost:5089" + imgUrl;
                StartCoroutine(LoadImage(fullUrl, img));
            }
        }
    }
    
    
    
    
    
    public void OnBackButtonClicked()
    {
        switch (currentState)
        {
            case AppState.SearchResults:
            case AppState.CategoryStores:
            case AppState.FloorView:
                // If we are in search results, category stores, or floor view, go back to categories
                ShowCategories();
                break;

            case AppState.Categories:
            case AppState.Welcome:
            default:
                // If we are already on categories or at root, go back to welcome scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("WelcomeScene");
                break;
        }
    }
    
    
    
    

    
    
    

    


}