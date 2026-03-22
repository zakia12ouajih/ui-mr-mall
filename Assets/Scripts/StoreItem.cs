using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class StoreData
{
    public string storeName;
    public string category;
    public string tagline;
    public Sprite logo;
}

public class StoreItem : MonoBehaviour
{
    [Header("Prefab & Parent")]
    public GameObject cardPrefab;
    public Transform contentParent;
    
    [Header("Mock Logos")]
    public List<Sprite> logos;
    
    [Header("Grid Settings")]
    public int columns = 3;
    public Vector2 cardSize = new Vector2(200, 200);
    public Vector2 cardSpacing = new Vector2(50, 20);
    
    [Header("Padding")]
    public int leftPadding = 50;
    public int rightPadding = 50;
    public int topPadding = 10;
    public int bottomPadding = 10;
    
    private List<StoreData> mockStores;
    private GridLayoutGroup grid;

    void Start()
    {
        CreateMockStores();
        StartCoroutine(InitializeStoresProperly());
    }

    IEnumerator InitializeStoresProperly()
    {
        // Wait until Unity finishes layout
        yield return new WaitForEndOfFrame();

        SetupContentParent();
        SpawnStores();

        // Let Unity apply layout
        yield return new WaitForEndOfFrame();

        ForceLayoutUpdate();

        // FINAL enforcement (this makes columns stick)
        yield return new WaitForEndOfFrame();

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            contentParent.GetComponent<RectTransform>()
        );

        Debug.Log("FINAL columns forced: " + grid.constraintCount);
    }

    void SetupContentParent()
    {
        if (contentParent == null)
        {
            Debug.LogError("Content Parent not assigned!");
            return;
        }

        RectTransform contentRect = contentParent.GetComponent<RectTransform>();

        // Get or create GridLayoutGroup
        grid = contentParent.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = contentParent.gameObject.AddComponent<GridLayoutGroup>();
        }

        // Apply settings
        grid.cellSize = cardSize;
        grid.spacing = cardSpacing;
        grid.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        // Ensure enough width for 3 columns
        float minContentWidth = (cardSize.x * columns) +
                                (cardSpacing.x * (columns - 1)) +
                                leftPadding + rightPadding;

        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minContentWidth);

        // Content Size Fitter
        ContentSizeFitter fitter = contentParent.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = contentParent.gameObject.AddComponent<ContentSizeFitter>();
        }

        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ScrollRect setup
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.content = contentRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
        }

        // Check for parent layout conflicts
        LayoutGroup parentLayout = contentParent.GetComponentInParent<LayoutGroup>();
        if (parentLayout != null && parentLayout != grid)
        {
            Debug.LogWarning("Parent LayoutGroup may override Grid!");
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    void CreateMockStores()
    {
        mockStores = new List<StoreData>
        {
            new StoreData{ storeName="NovaTech Hub", category="Electronics", tagline="Next-gen gadgets at your fingertips"},
            new StoreData{ storeName="Aura Fashion", category="Clothing", tagline="Style that moves with you" },
            new StoreData{ storeName="PixelPlay", category="Gaming", tagline="Level up your experience" },
            new StoreData{ storeName="HomeScape", category="Home & Decor", tagline="Design your perfect space"},
            new StoreData{ storeName="GlowUp Beauty", category="Beauty & Skincare", tagline="Feel good, look better" },
            new StoreData{ storeName="FitZone Pro", category="Fitness", tagline="Train smarter, not harder" },
            new StoreData{ storeName="QuickBite", category="Food & Snacks", tagline="Instant cravings, satisfied" },
            new StoreData{ storeName="ReadSphere", category="Books", tagline="A universe of stories" },
            new StoreData{ storeName="SoundWave", category="Music & Audio", tagline="Hear every detail" },
            new StoreData{ storeName="KiddoLand", category="Toys & Kids", tagline="Where fun begins"}
        };
    }

    void SpawnStores()
    {
        if (contentParent == null) return;

        // Clear existing
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentParent.GetChild(i).gameObject);
        }

        // Spawn
        for (int i = 0; i < mockStores.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, contentParent);
            card.name = $"StoreCard_{i}_{mockStores[i].storeName}";

            SetTextComponent(card, "StoreName", mockStores[i].storeName);
            SetTextComponent(card, "CategoryBadge", mockStores[i].category);
            SetTextComponent(card, "Tagline", mockStores[i].tagline);

            if (i < logos.Count && logos[i] != null)
            {
                Image logoImage = card.transform.Find("Logo")?.GetComponent<Image>();
                if (logoImage != null)
                {
                    logoImage.sprite = logos[i];
                }
            }
        }
    }

    void SetTextComponent(GameObject card, string componentName, string text)
    {
        TextMeshProUGUI textComponent = card.transform.Find(componentName)?.GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            textComponent = card.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }

    void ForceLayoutUpdate()
    {
        if (contentParent == null) return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            contentParent.GetComponent<RectTransform>()
        );
    }
}