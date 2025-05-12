using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform mainTransform; // The main transform of the layer
        public float scrollSpeed;       // The speed at which the layer scrolls
        public float layerWidth;        // The width of the layer (calculated automatically)
        public Transform leftDuplicate; // Duplicate on the left
        public Transform rightDuplicate; // Duplicate on the right
    }

    [Header("Parallax Layers")]
    [SerializeField] private ParallaxLayer[] layers; // Array of layers

    private void Start()
    {
        // Calculate the width of each layer and create duplicates
        foreach (var layer in layers)
        {
            SpriteRenderer spriteRenderer = layer.mainTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                layer.layerWidth = spriteRenderer.bounds.size.x;

                // Create left and right duplicates
                layer.leftDuplicate = Instantiate(layer.mainTransform, layer.mainTransform.position - new Vector3(layer.layerWidth, 0, 0), Quaternion.identity, layer.mainTransform.parent);
                layer.rightDuplicate = Instantiate(layer.mainTransform, layer.mainTransform.position + new Vector3(layer.layerWidth, 0, 0), Quaternion.identity, layer.mainTransform.parent);
            }
            else
            {
                Debug.LogError($"Layer {layer.mainTransform.name} is missing a SpriteRenderer!");
            }
        }
    }

    private void Update()
    {
        foreach (var layer in layers)
        {
            // Scroll the main layer and its duplicates
            ScrollLayer(layer.mainTransform, layer.scrollSpeed);
            ScrollLayer(layer.leftDuplicate, layer.scrollSpeed);
            ScrollLayer(layer.rightDuplicate, layer.scrollSpeed);

            // Reset the main layer if it moves out of view
            if (layer.mainTransform.position.x <= -layer.layerWidth)
            {
                ResetLayer(layer);
            }
        }
    }

    private void ScrollLayer(Transform layerTransform, float scrollSpeed)
    {
        Vector3 newPosition = layerTransform.position;
        newPosition.x -= scrollSpeed * Time.deltaTime;
        layerTransform.position = newPosition;
    }

    private void ResetLayer(ParallaxLayer layer)
    {
        // Reset the main layer to the right of the right duplicate
        layer.mainTransform.position = layer.rightDuplicate.position + new Vector3(layer.layerWidth, 0, 0);

        // Swap references to maintain the correct order
        Transform temp = layer.leftDuplicate;
        layer.leftDuplicate = layer.mainTransform;
        layer.mainTransform = layer.rightDuplicate;
        layer.rightDuplicate = temp;
    }
}
