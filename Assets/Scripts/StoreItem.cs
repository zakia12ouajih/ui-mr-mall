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
    [Header("UI References")]
    public GameObject cardPrefabStore;
    public Transform contentParent;
    public TMP_Dropdown floorDropdown;
    public GameObject scrollViewStores;
    // public GameObject categoryButtons;

    [Header("Category Prefab")]
    // public GameObject categoryPrefab;
    public GameObject categoryPrefab;     // prefab for each category
    public Transform contentCategoryParent; 
    public GameObject scrollViewCategory;     

    

    [Header("Mock Logos")]
    public List<Sprite> logos;

    private GridLayoutGroup grid;
    private List<StoreData> mockStores;

    void Start()
    {
        CreateMockStores();
        // SetupContentParent();
        SetupDropdown();

        // Default: show categories
        ShowSelectedFloor();
    }

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

            Button btn = card.GetComponent<Button>();
            if (btn != null)
            {
                string capturedCategory = category;
                btn.onClick.AddListener(() => ShowStoresByCategory(capturedCategory));
            }
        }
        scrollViewCategory.SetActive(true);
        scrollViewStores.SetActive(false); // hide stores
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
        // Clear categories panel
        for (int i = contentCategoryParent.childCount - 1; i >= 0; i--)
            Destroy(contentCategoryParent.GetChild(i).gameObject);

        // categoryButtons.SetActive(false);
        scrollViewCategory.SetActive(false);
        scrollViewStores.SetActive(true);

        // Filter and spawn stores of selected category
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var store in mockStores)
        {
            if (store.category != category) continue;

            GameObject card = Instantiate(cardPrefabStore, contentParent);
            SetText(card, "verticalText/StoreName", store.storeName);
            SetText(card, "verticalText/CategoryBadge", store.category);
            SetText(card, "verticalText/Tagline", store.tagline);

            if (store.logo != null)
            {
                Image logoImage = card.transform.Find("Logo")?.GetComponent<Image>();
                if (logoImage != null) logoImage.sprite = store.logo;
            }
        }

        // ForceLayoutUpdate();
    }

    void SetupDropdown()
    {
        floorDropdown.ClearOptions();
        List<string> options = new List<string> { "Show Categories", "Floor 1", "Floor 2", "Floor 3" };
        floorDropdown.AddOptions(options);

        // floorDropdown.onValueChanged.AddListener(delegate { ShowSelectedFloor(); });
    }

    void SetupContentParent()
    {
        if (contentParent == null)
        {
            Debug.LogError("Content Parent not assigned!");
            return;
        }

        grid = contentParent.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = contentParent.gameObject.AddComponent<GridLayoutGroup>();

        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;

        // Ensure content width fits all columns

        }

    void CreateMockStores()
    {
        mockStores = new List<StoreData>
        {
            new StoreData{ storeName="NovaTech Hub", category="Electronics", tagline="Next-gen gadgets", floor=1 },
            new StoreData{ storeName="Aura Fashion", category="Fashion", tagline="Style that moves", floor=1 },
            new StoreData{ storeName="GlowUp Beauty", category="Makeup", tagline="Feel good", floor=1 },
            new StoreData{ storeName="FitZone Pro", category="Sports", tagline="Train smarter", floor=1 },

            // FLOOR 2
            new StoreData{ storeName="PixelPlay", category="Electronics", tagline="Level up gaming", floor=2 },
            new StoreData{ storeName="HomeScape", category="Home Decor", tagline="Perfect space", floor=2 },
            new StoreData{ storeName="Elegance Gems", category="Jewelry", tagline="Shine bright", floor=2 },
            new StoreData{ storeName="Urban Wear", category="Fashion", tagline="Street style", floor=2 },

            // FLOOR 3
            new StoreData{ storeName="SoundWave", category="Electronics", tagline="Hear every detail", floor=3 },
            new StoreData{ storeName="DecoDream", category="Home Decor", tagline="Design your vibe", floor=3 },
            new StoreData{ storeName="GoldNest", category="Jewelry", tagline="Luxury pieces", floor=3 },
            new StoreData{ storeName="ActiveLife", category="Sports", tagline="Stay active", floor=3 }
        };
    }

    public void ShowSelectedFloor()
    {
        int index = floorDropdown.value;

        if (index == 0)
        {
            ShowCategories();
            return;
        }

        // Clear categories when showing a floor
        for (int i = contentCategoryParent.childCount - 1; i >= 0; i--)
            Destroy(contentCategoryParent.GetChild(i).gameObject);
        scrollViewCategory.SetActive(false);
        scrollViewStores.SetActive(true);
        SpawnStores(index); // floor 1 = index 1, etc.
    }

    void SpawnStores(int floor)
    {
        // Clear existing cards
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var store in mockStores)
        {
            if (store.floor != floor) continue;

            GameObject card = Instantiate(cardPrefabStore, contentParent);

            SetText(card, "verticalText/StoreName", store.storeName);
            SetText(card, "verticalText/CategoryBadge", store.category);
            SetText(card, "verticalText/Tagline", store.tagline);

            if (store.logo != null)
            {
                Image logoImage = card.transform.Find("Logo")?.GetComponent<Image>();
                if (logoImage != null) logoImage.sprite = store.logo;
            }
        }

        // ForceLayoutUpdate();
    }

    void SetText(GameObject card, string name, string text)
    {
        var tmp = card.transform.Find(name)?.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
    }

    // void ForceLayoutUpdate()
    // {
    //     if (contentParent == null) return;

    //     Canvas.ForceUpdateCanvases();
    //     LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    // }
    
    public void OnGoButtonClicked()
    {
        ShowSelectedFloor();
    }
}