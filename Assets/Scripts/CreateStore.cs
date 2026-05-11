using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreSceneManager : MonoBehaviour
{
    string storeId;

    List<ProductDTO> storeProducts = new();

    [Header("Scene References")]
    public Rack rack;

    [Header("Prefabs")]
    public GameObject productPrefab;

    public GameObject rackPrefab;
    public Transform racksParent;
    public int rackCapacity = 3;

    void Start()
    {
        storeId = PlayerPrefs.GetString("selected_store_id");

        Debug.Log("SELECTED STORE: " + storeId);

        StartCoroutine(LoadStoreProducts());
    }

    IEnumerator LoadStoreProducts()
    {
        yield return StartCoroutine(ApiManager.Instance.Get(
            "/products",
            (response) =>
            {
                ProductDTO[] data =
                    StoreItem.JsonHelper.FromJson<ProductDTO>(response);

                storeProducts = data
                    .Where(p =>
                        p.store != null &&
                        p.store.id == storeId)
                    .ToList();

                Debug.Log("STORE PRODUCTS: " + storeProducts.Count);
            },
            (error) => Debug.LogError(error)
        ));

        // load media
        for (int i = 0; i < storeProducts.Count; i++)
        {
            int index = i;

            yield return StartCoroutine(ApiManager.Instance.Get(
                $"/products/{storeProducts[index].id}/media",
                (mediaResponse) =>
                {
                    ProductMediaDTO[] media =
                        StoreItem.JsonHelper.FromJson<ProductMediaDTO>(mediaResponse);

                    storeProducts[index].imageUrl = media.ToList();
                },
                (error) => Debug.LogError(error)
            ));
        }

        ShowProducts();
    }

    void ShowProducts()
    {
        // clear old racks
        for (int i = racksParent.childCount - 1; i >= 0; i--)
        {
            Destroy(racksParent.GetChild(i).gameObject);
        }

        int productIndex = 0;
        int rackCount =
            Mathf.CeilToInt((float)storeProducts.Count / rackCapacity);

        float rackSpacingY = 2.5f;

        for (int i = 0; i < rackCount; i++)
        {
            GameObject rackObj =
                Instantiate(rackPrefab, racksParent, false);

            rackObj.transform.localPosition =
                new Vector3(0f, i * rackSpacingY, 0f);

            Rack rack =
                rackObj.GetComponent<Rack>();

            for (int j = 0; j < rackCapacity; j++)
            {
                if (productIndex >= storeProducts.Count)
                    return;

                CreateProductCard(
                    storeProducts[productIndex],
                    rack.content
                );

                productIndex++;
            }
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

    

    void CreateProductCard(ProductDTO data, Transform parent)
    {
        GameObject go =
            Instantiate(productPrefab, parent, false);

        SetText(go, "ProductName", data.title);

        SetText(
            go,
            "price",
            data.price.ToString("0.00") +
            " " +
            data.currency
        );

        Image img =
            go.transform.Find("Logo")
            ?.GetComponent<Image>();

        if (
            data.imageUrl != null &&
            data.imageUrl.Count > 0
        )
        {
            var imgUrl = data.imageUrl
                .FirstOrDefault(x => x.type == "IMAGE")
                ?.url;

            if (!string.IsNullOrEmpty(imgUrl))
            {
                StartCoroutine(
                    LoadImage(
                        "http://localhost:5089" + imgUrl,
                        img
                    )
                );
            }
        }
    }

    IEnumerator LoadImage(string url, Image target)
{
    using (
        UnityEngine.Networking.UnityWebRequest request =
        UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url)
    )
    {
        yield return request.SendWebRequest();

        if (
            request.result ==
            UnityEngine.Networking.UnityWebRequest.Result.Success
        )
        {
            Texture2D texture =
                ((UnityEngine.Networking.DownloadHandlerTexture)
                request.downloadHandler).texture;

            target.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        else
        {
            Debug.LogError("IMAGE LOAD FAILED: " + url);
        }
    }
}

    
}