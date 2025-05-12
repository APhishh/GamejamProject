using UnityEngine;

public class ParallaxGameBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform mainTransform; // The main transform of the layer
        public float parallaxSpeedX;    // Horizontal parallax speed
        public float parallaxSpeedY;    // Vertical parallax speed
        public float layerWidth;        // The width of the layer (calculated automatically)
        public float layerHeight;       // The height of the layer (calculated automatically)
        public bool hasVerticalDuplicates; // Whether this layer has vertical duplicates
        public Transform[] duplicates; // Array to hold duplicates (up, down, left, right, and corners)
    }

    [Header("Parallax Layers")]
    [SerializeField] private ParallaxLayer[] layers; // Array of layers

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform; // Reference to the camera

    private Vector3 previousCameraPosition;

    private void Start()
    {
        // Store the initial camera position
        previousCameraPosition = cameraTransform.position;

        // Calculate the dimensions of each layer and create duplicates
        foreach (var layer in layers)
        {
            SpriteRenderer spriteRenderer = layer.mainTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                layer.layerWidth = spriteRenderer.bounds.size.x;
                layer.layerHeight = spriteRenderer.bounds.size.y;

                // Create duplicates based on the layer's settings
                layer.duplicates = new Transform[layer.hasVerticalDuplicates ? 8 : 4];

                // Horizontal duplicates (left and right)
                layer.duplicates[0] = Instantiate(layer.mainTransform, layer.mainTransform.position - new Vector3(layer.layerWidth, 0, 0), Quaternion.identity, layer.mainTransform.parent); // Left
                layer.duplicates[1] = Instantiate(layer.mainTransform, layer.mainTransform.position + new Vector3(layer.layerWidth, 0, 0), Quaternion.identity, layer.mainTransform.parent); // Right

                if (layer.hasVerticalDuplicates)
                {
                    // Vertical duplicates (up and down)
                    layer.duplicates[2] = Instantiate(layer.mainTransform, layer.mainTransform.position + new Vector3(0, layer.layerHeight, 0), Quaternion.identity, layer.mainTransform.parent); // Up
                    layer.duplicates[3] = Instantiate(layer.mainTransform, layer.mainTransform.position - new Vector3(0, layer.layerHeight, 0), Quaternion.identity, layer.mainTransform.parent); // Down

                    // Corner duplicates
                    layer.duplicates[4] = Instantiate(layer.mainTransform, layer.mainTransform.position + new Vector3(layer.layerWidth, layer.layerHeight, 0), Quaternion.identity, layer.mainTransform.parent); // Top-Right
                    layer.duplicates[5] = Instantiate(layer.mainTransform, layer.mainTransform.position - new Vector3(layer.layerWidth, layer.layerHeight, 0), Quaternion.identity, layer.mainTransform.parent); // Bottom-Left
                    layer.duplicates[6] = Instantiate(layer.mainTransform, layer.mainTransform.position + new Vector3(layer.layerWidth, -layer.layerHeight, 0), Quaternion.identity, layer.mainTransform.parent); // Bottom-Right
                    layer.duplicates[7] = Instantiate(layer.mainTransform, layer.mainTransform.position - new Vector3(layer.layerWidth, -layer.layerHeight, 0), Quaternion.identity, layer.mainTransform.parent); // Top-Left
                }

                // Ensure duplicates inherit the same sorting order or Z-position
                foreach (var duplicate in layer.duplicates)
                {
                    if (duplicate != null)
                    {
                        duplicate.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;
                        duplicate.position = new Vector3(duplicate.position.x, duplicate.position.y, layer.mainTransform.position.z);
                    }
                }
            }
            else
            {
                Debug.LogError($"Layer {layer.mainTransform.name} is missing a SpriteRenderer!");
            }
        }
    }

    private void Update()
    {
        Vector3 cameraDelta = cameraTransform.position - previousCameraPosition;

        foreach (var layer in layers)
        {
            // Apply parallax effect to the main layer
            Vector3 newPosition = layer.mainTransform.position;
            newPosition.x += cameraDelta.x * layer.parallaxSpeedX;
            newPosition.y += cameraDelta.y * layer.parallaxSpeedY;
            layer.mainTransform.position = newPosition;

            // Apply parallax effect to duplicates
            foreach (var duplicate in layer.duplicates)
            {
                if (duplicate != null)
                {
                    newPosition = duplicate.position;
                    newPosition.x += cameraDelta.x * layer.parallaxSpeedX;
                    newPosition.y += cameraDelta.y * layer.parallaxSpeedY;
                    duplicate.position = newPosition;
                }
            }

            // Reset the main layer and duplicates if they move out of view
            ResetLayerIfOutOfView(layer);
        }

        // Update the previous camera position
        previousCameraPosition = cameraTransform.position;
    }

    private void ResetLayerIfOutOfView(ParallaxLayer layer)
    {
        // Check if the main layer has moved out of view horizontally or vertically
        if (layer.mainTransform.position.x <= cameraTransform.position.x - layer.layerWidth)
        {
            ShiftLayerHorizontally(layer, isMovingRight: false);
        }
        else if (layer.mainTransform.position.x >= cameraTransform.position.x + layer.layerWidth)
        {
            ShiftLayerHorizontally(layer, isMovingRight: true);
        }

        if (layer.hasVerticalDuplicates)
        {
            if (layer.mainTransform.position.y <= cameraTransform.position.y - layer.layerHeight)
            {
                ShiftLayerVertically(layer, isMovingUp: false);
            }
            else if (layer.mainTransform.position.y >= cameraTransform.position.y + layer.layerHeight)
            {
                ShiftLayerVertically(layer, isMovingUp: true);
            }
        }
    }

    private void ShiftLayerHorizontally(ParallaxLayer layer, bool isMovingRight)
    {
        float offset = isMovingRight ? layer.layerWidth : -layer.layerWidth;

        // Shift the main layer and duplicates horizontally
        layer.mainTransform.position += new Vector3(offset, 0, 0);
        foreach (var duplicate in layer.duplicates)
        {
            if (duplicate != null)
            {
                duplicate.position += new Vector3(offset, 0, 0);
            }
        }
    }

    private void ShiftLayerVertically(ParallaxLayer layer, bool isMovingUp)
    {
        float offset = isMovingUp ? layer.layerHeight : -layer.layerHeight;

        // Shift the main layer and duplicates vertically
        layer.mainTransform.position += new Vector3(0, offset, 0);
        foreach (var duplicate in layer.duplicates)
        {
            if (duplicate != null)
            {
                duplicate.position += new Vector3(0, offset, 0);
            }
        }
    }
}
