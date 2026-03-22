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
    public GameObject cardPrefab;
    public Transform contentParent;
    public TMP_Dropdown floorDropdown;
    public GameObject scrollView;
    public GameObject categoryButtons;

    [Header("Grid Settings")]
    public int columns = 3;
    public Vector2 cardSize = new Vector2(200, 200);
    public Vector2 cardSpacing = new Vector2(50, 20);

    [Header("Padding")]
    public int leftPadding = 50;
    public int rightPadding = 50;
    public int topPadding = 20;
    public int bottomPadding = 10;

    [Header("Mock Logos")]
    public List<Sprite> logos;

    private GridLayoutGroup grid;
    private List<StoreData> mockStores;

    void Start()
    {
        CreateMockStores();
        SetupContentParent();
        SetupDropdown();

        // Default: show categories
        ShowSelectedFloor();
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

        grid.cellSize = cardSize;
        grid.spacing = cardSpacing;
        grid.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        // Ensure content width fits all columns
        float minWidth = (cardSize.x * columns) + (cardSpacing.x * (columns - 1)) + leftPadding + rightPadding;
        contentParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);

        ContentSizeFitter fitter = contentParent.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = contentParent.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    void CreateMockStores()
    {
        mockStores = new List<StoreData>
        {
            new StoreData{ storeName="NovaTech Hub", category="Electronics", tagline="Next-gen gadgets", floor=1 },
            new StoreData{ storeName="Aura Fashion", category="Clothing", tagline="Style that moves", floor=1 },
            new StoreData{ storeName="PixelPlay", category="Gaming", tagline="Level up", floor=2 },
            new StoreData{ storeName="HomeScape", category="Home & Decor", tagline="Perfect space", floor=2 },
            new StoreData{ storeName="GlowUp Beauty", category="Beauty & Skincare", tagline="Feel good", floor=3 },
            new StoreData{ storeName="FitZone Pro", category="Fitness", tagline="Train smarter", floor=3 },
            new StoreData{ storeName="QuickBite", category="Food & Snacks", tagline="Instant cravings", floor=1 },
            new StoreData{ storeName="ReadSphere", category="Books", tagline="A universe of stories", floor=2 },
            new StoreData{ storeName="SoundWave", category="Music & Audio", tagline="Hear every detail", floor=3 },
            new StoreData{ storeName="KiddoLand", category="Toys & Kids", tagline="Where fun begins", floor=1 }
        };
    }

    public void ShowSelectedFloor()
    {
        int index = floorDropdown.value;

        // Show categories if first option is selected
        if (index == 0)
        {
            scrollView.SetActive(false);
            categoryButtons.SetActive(true);
            return;
        }

        int selectedFloor = index; // floor 1 = index 1, etc.
        scrollView.SetActive(true);
        categoryButtons.SetActive(false);
        SpawnStores(selectedFloor);
    }

    void SpawnStores(int floor)
    {
        // Clear existing cards
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var store in mockStores)
        {
            if (store.floor != floor) continue;

            GameObject card = Instantiate(cardPrefab, contentParent);

            SetText(card, "StoreName", store.storeName);
            SetText(card, "CategoryBadge", store.category);
            SetText(card, "Tagline", store.tagline);

            if (store.logo != null)
            {
                Image logoImage = card.transform.Find("Logo")?.GetComponent<Image>();
                if (logoImage != null) logoImage.sprite = store.logo;
            }
        }

        ForceLayoutUpdate();
    }

    void SetText(GameObject card, string name, string text)
    {
        var tmp = card.transform.Find(name)?.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
    }

    void ForceLayoutUpdate()
    {
        if (contentParent == null) return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }
    
    public void OnGoButtonClicked()
    {
        ShowSelectedFloor();
    }
}