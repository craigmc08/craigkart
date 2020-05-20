using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemDisplay : MonoBehaviour
{
    public PItemController controller;
    public Sprite nullSprite;

    [Header("Item Images")]
    public Sprite tripleBoost;
    public Sprite doubleBoost;
    public Sprite singleBoost;

    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

        if (controller == null || controller.heldItem == null) {
            image.sprite = nullSprite;
            return;
        } 

        Item item = controller.heldItem;
        if (item is TripleBoost) {
            TripleBoost boost = (TripleBoost)item;
            if (boost.remaining == 3) image.sprite = tripleBoost;
            else if (boost.remaining == 2) image.sprite = doubleBoost;
            else if (boost.remaining == 1) image.sprite = singleBoost;
            else image.sprite = nullSprite;
        }
    }
}
