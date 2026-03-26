using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class StoreData
{
    public string storeName;
    public string category;
    public string tagline;
    public Sprite logo;
    public int floor; // 1, 2, 3...
}

public class StoreItem : MonoBehaviour
{
    // app state for "back" btn
    enum AppState
    {
        Welcome,
        Categories,
        FloorView,
        CategoryStores
    }

    AppState currentState;

    [Header("UI References")]
    public GameObject cardPrefabStore;
    public Transform contentParent;
    public TMP_Dropdown floorDropdown;
    public GameObject scrollViewStores;
    public Button goButton; // Add reference to Go button

    [Header("Category Prefab")]
    public GameObject categoryPrefab;     // prefab for each category
    public Transform contentCategoryParent; 
    public GameObject scrollViewCategory;     
    
    [Header("Mock Logos")]
    public List<Sprite> logos;

    private List<StoreData> mockStores;
    private int selectedFloorValue; // Store the currently selected dropdown value
    
    // Store currently selected category to maintain state
    private string currentCategory;

    void Start()
    {
        CreateMockStores();
        SetupDropdown();
        
        // Setup Go button
        if (goButton != null)
        {
            goButton.onClick.RemoveAllListeners();
            goButton.onClick.AddListener(OnGoButtonClicked);
        }
        else
        {
            Debug.LogWarning("Go Button not assigned in inspector!");
        }
        
        // Default: show categories
        ShowCategories();
    }
    
    // ----------------- Category: list, show category function, 
    private List<string> categories = new List<string>
    {
        "Makeup", "Home Decor", "Jewelry", "Sports", "Fashion", "Electronics"
    };
    
    void ShowCategories()
    {
        // Clear existing category cards
        for (int i = contentCategoryParent.childCount - 1; i >= 0; i--)
            Destroy(contentCategoryParent.GetChild(i).gameObject);

        foreach (var category in categories)
        {
            GameObject card = Instantiate(categoryPrefab, contentCategoryParent);

            SetText(card, "CategoryName", category);
            SetText(card, "CategoryPreview", GetCategoryPreview(category));

            // IMPORTANT: Find the button properly
            Button btn = card.GetComponentInChildren<Button>();
            if (btn == null)
            {
                // Try to find by name if not found in children
                btn = card.transform.Find("viewCategory")?.GetComponent<Button>();
            }
            
            if (btn != null)
            {
                // Capture the variable in the loop
                string capturedCategory = category;
                btn.onClick.RemoveAllListeners(); // Clear any existing listeners
                btn.onClick.AddListener(() => ShowStoresByCategory(capturedCategory));
                Debug.Log($"Added listener for category: {capturedCategory}");
            }
            else
            {
                Debug.LogWarning($"viewCategory button not found in category prefab for category: {category}");
            }
        }
        
        scrollViewCategory.SetActive(true);
        scrollViewStores.SetActive(false); // hide stores
        currentState = AppState.Categories;
    }
    
    string GetCategoryPreview(string category)
    {
        List<string> names = new List<string>();

        foreach (var store in mockStores)
        {
            if (store.category == category)
                names.Add(store.storeName);
        }

        if (names.Count == 0) return "";

        // Case 1: only 1 store
        if (names.Count == 1)
            return names[0];

        // Case 2: 2 or more stores
        string line1 = names[0];
        string line2 = names[1];

        if (names.Count > 2)
        {
            int remaining = names.Count - 2;
            line2 += " +" + remaining;
        }

        return line1 + "\n" + line2;
    }
    
    public void ShowStoresByCategory(string category)
    {
        // Store the current category
        currentCategory = category;
        
        // Clear categories panel
        for (int i = contentCategoryParent.childCount - 1; i >= 0; i--)
            Destroy(contentCategoryParent.GetChild(i).gameObject);

        scrollViewCategory.SetActive(false);
        scrollViewStores.SetActive(true);

        // Clear and spawn stores of selected category
        ClearStores();
        SpawnStoresByCategory(category);
        currentState = AppState.CategoryStores;
    }
    
    void SpawnStoresByCategory(string category)
    {
        foreach (var store in mockStores)
        {
            if (store.category != category) continue;
            CreateStoreCard(store);
        }
    }
    
    void SpawnStoresByFloor(int floor)
    {
        foreach (var store in mockStores)
        {
            if (store.floor != floor) continue;
            CreateStoreCard(store);
        }
    }
    
    void CreateStoreCard(StoreData store)
    {
        GameObject card = Instantiate(cardPrefabStore, contentParent);
        
        SetText(card, "verticalText/StoreName", store.storeName);
        SetText(card, "verticalText/CategoryBadge", store.category);
        SetText(card, "verticalText/Tagline", store.tagline);

        if (store.logo != null)
        {
            Image logoImage = card.transform.Find("Logo")?.GetComponent<Image>();
            if (logoImage != null) logoImage.sprite = store.logo;
        }
        
        // Capture store for button click
        Button btn = card.GetComponent<Button>();
        if (btn != null)
        {
            StoreData capturedStore = store;
            btn.onClick.RemoveAllListeners(); // Clear any existing listeners
            btn.onClick.AddListener(() => ShowStoreDetails(capturedStore));
        }
        else
        {
            Debug.LogWarning($"Button component not found on store card for: {store.storeName}");
        }
    }
    
    void ClearStores()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);
    }

    void SetupDropdown()
    {
        floorDropdown.ClearOptions();
        List<string> options = new List<string> { "Show Categories", "Floor 1", "Floor 2", "Floor 3" };
        floorDropdown.AddOptions(options);
        
        // Just store the initial value
        selectedFloorValue = floorDropdown.value;
        
        // Optional: Add visual feedback when dropdown changes without triggering action
        floorDropdown.onValueChanged.AddListener((value) => {
            selectedFloorValue = value;
            Debug.Log($"Dropdown selection changed to: {options[value]} - Click Go to apply");
        });
    }

    void CreateMockStores()
    {
        mockStores = new List<StoreData>
        {
            // FLOOR 1
            new StoreData{ storeName="NovaTech Hub", category="Electronics", tagline="Next-gen gadgets", floor=1, logo=GetLogo(0) },
            new StoreData{ storeName="Aura Fashion", category="Fashion", tagline="Style that moves", floor=1, logo=GetLogo(1) },
            new StoreData{ storeName="GlowUp Beauty", category="Makeup", tagline="Feel good", floor=1, logo=GetLogo(2) },
            new StoreData{ storeName="FitZone Pro", category="Sports", tagline="Train smarter", floor=1, logo=GetLogo(3) },

            // FLOOR 2
            new StoreData{ storeName="PixelPlay", category="Electronics", tagline="Level up gaming", floor=2, logo=GetLogo(0) },
            new StoreData{ storeName="HomeScape", category="Home Decor", tagline="Perfect space", floor=2, logo=GetLogo(1) },
            new StoreData{ storeName="Elegance Gems", category="Jewelry", tagline="Shine bright", floor=2, logo=GetLogo(2) },
            new StoreData{ storeName="Urban Wear", category="Fashion", tagline="Street style", floor=2, logo=GetLogo(3) },

            // FLOOR 3
            new StoreData{ storeName="SoundWave", category="Electronics", tagline="Hear every detail", floor=3, logo=GetLogo(0) },
            new StoreData{ storeName="DecoDream", category="Home Decor", tagline="Design your vibe", floor=3, logo=GetLogo(1) },
            new StoreData{ storeName="GoldNest", category="Jewelry", tagline="Luxury pieces", floor=3, logo=GetLogo(2) },
            new StoreData{ storeName="ActiveLife", category="Sports", tagline="Stay active", floor=3, logo=GetLogo(3) }
        };
    }
    
    Sprite GetLogo(int index)
    {
        if (logos != null && index < logos.Count)
            return logos[index];
        return null;
    }

    public void OnGoButtonClicked()
    {
        // This is triggered by the Go button
        Debug.Log($"Go button clicked! Selected dropdown value: {selectedFloorValue}");
        
        // Process the current dropdown selection
        if (selectedFloorValue == 0)
        {
            // Show categories
            ShowCategories();
        }
        else
        {
            // Show stores for selected floor
            // Clear categories when showing a floor
            for (int i = contentCategoryParent.childCount - 1; i >= 0; i--)
                Destroy(contentCategoryParent.GetChild(i).gameObject);
            
            scrollViewCategory.SetActive(false);
            scrollViewStores.SetActive(true);
            
            // Clear and spawn stores for selected floor
            ClearStores();
            SpawnStoresByFloor(selectedFloorValue); // floor 1 = index 1, etc.
            currentState = AppState.FloorView;
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
    
    void ShowStoreDetails(StoreData store)
    {
        Debug.Log($"Clicked store: {store.storeName} (Category: {store.category}, Floor: {store.floor})");
        // Here you can add code to show a details panel, load a scene, etc.
    }
    public void OnBackButtonClicked()
    {
        switch (currentState)
        {
            case AppState.CategoryStores:
            case AppState.FloorView:
                // If we are inside a category or floor view, go back to categories
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