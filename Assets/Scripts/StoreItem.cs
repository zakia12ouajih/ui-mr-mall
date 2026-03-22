using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [Header("Prefab & Parent")]
    public GameObject cardPrefab;
    public Transform contentParent;

    [Header("Dropdown")]
    public TMP_Dropdown floorDropdown; // Dropdown to select floor

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

    private List<StoreData> mockStores;
    private GridLayoutGroup grid;

    void Start()
    {
        CreateMockStores();
        SetupContentParent();
        ShowSelectedFloor(); // default floor at start
    }

    void SetupContentParent()
    {
        if (contentParent == null)
        {
            Debug.LogError("Content Parent not assigned!");
            return;
        }

        RectTransform contentRect = contentParent.GetComponent<RectTransform>();

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

        // Ensure viewport width can fit all columns
        float minWidth = (cardSize.x * columns) + (cardSpacing.x * (columns - 1)) + leftPadding + rightPadding;
        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);

        // Content Size Fitter
        ContentSizeFitter fitter = contentParent.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = contentParent.gameObject.AddComponent<ContentSizeFitter>();

        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ScrollRect setup
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.content = contentRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            // Ensure viewport has a RectMask2D for clipping
            RectMask2D mask = scrollRect.viewport.GetComponent<RectMask2D>();
            if (mask == null) scrollRect.viewport.gameObject.AddComponent<RectMask2D>();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
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
        if (floorDropdown == null)
        {
            Debug.LogWarning("Dropdown not assigned, defaulting to floor 1");
            SpawnStores(1);
            return;
        }

        int floor = floorDropdown.value + 1; // Dropdown index starts at 0
        SpawnStores(floor);
    }

    void SpawnStores(int selectedFloor)
    {
        if (contentParent == null) return;

        // Clear existing
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // Spawn stores for selected floor
        foreach (var store in mockStores)
        {
            if (store.floor != selectedFloor) continue;

            GameObject card = Instantiate(cardPrefab, contentParent);

            SetTextComponent(card, "StoreName", store.storeName);
            SetTextComponent(card, "CategoryBadge", store.category);
            SetTextComponent(card, "Tagline", store.tagline);

            if (store.logo != null)
            {
                Image logoImage = card.transform.Find("Logo")?.GetComponent<Image>();
                if (logoImage != null) logoImage.sprite = store.logo;
            }
        }

        ForceLayoutUpdate();
    }

    void SetTextComponent(GameObject card, string name, string text)
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
}