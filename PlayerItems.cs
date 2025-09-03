using UnityEngine;
using UnityEngine.UI;

public class PlayerItems : MonoBehaviour
{
    public InventorySlot itemSlot1;
    public InventorySlot itemSlot2;

    private IItem Item1;
    private IItem Item2;

    private GameObject item1;
    private GameObject item2;

    public GameObject thirdButton;

    public GameObject Chain;
    public Image JButton; // Button 1
    public Image KButton; // Button 2
    public Image JButtonChild; // Button 1
    public Image KButtonChild; // Button 2

    private bool use1 = true;
    private bool use2 = true;

    private void Update() // Using Items once they're equipped
    {
        if (Input.GetButtonDown("Item1") && use1)
        {
            item1 = GameObject.Find("/Player/Available_Items/" + itemSlot1.myItem.myItem.codeName + "(Clone)"); // Refernece to the item through InventorySlot
            Item1 = item1.GetComponent<IItem>();
            Item1?.Use(this.GetComponentInParent<PlayerMovement>()); //itemSlot1.myItem.myItem.Use(this.GetComponentInParent<PlayerMovement>());
        }
        if (Input.GetButtonDown("Item2") && use2)
        {
            item2 = GameObject.Find("/Player/Available_Items/" + itemSlot2.myItem.myItem.codeName + "(Clone)");
            Item2 = item2.GetComponent<IItem>();
            Item2?.Use(this.GetComponentInParent<PlayerMovement>());
        }

        if (Input.GetKeyUp(KeyCode.L) && thirdButton.activeSelf == true)
        {
            if (Chain.activeSelf == false)
            {
                if (!item1 || !item2)
                {
                    item1 = GameObject.Find("/Player/Available_Items/" + itemSlot1.myItem.myItem.codeName + "(Clone)");
                    item2 = GameObject.Find("/Player/Available_Items/" + itemSlot2.myItem.myItem.codeName + "(Clone)");
                }

                Chain.SetActive(true);
                if (itemSlot1.myItem.myItem.linkable == true)
                {
                    KButton.color = new Color(1, 1, 1, .75f);
                    KButtonChild.color = new Color(1, 1, 1, .75f);
                    use2 = false;
                    item1.GetComponent<Linkable>().linked = true;
                    item1.GetComponent<Linkable>().linkee = item2;
                }
                else
                {
                    JButton.color = new Color(1, 1, 1, .75f);
                    JButtonChild.color = new Color(1, 1, 1, .75f);
                    use1 = false;
                    item2.GetComponent<Linkable>().linked = true;
                    item2.GetComponent<Linkable>().linkee = item1;
                }
            }
            else
            {
                item1.GetComponent<Linkable>().linked = false;
                item1.GetComponent<Linkable>().linkee = null;
                item2.GetComponent<Linkable>().linked = false;
                item2.GetComponent<Linkable>().linkee = null;
                use1 = true;
                use2 = true;
                Chain.SetActive(false);
                JButton.color = new Color(1, 1, 1, 1);
                JButtonChild.color = new Color(1, 1, 1, 1);
                KButton.color = new Color(1, 1, 1, 1);
                KButtonChild.color = new Color(1, 1, 1, 1);
            }
        }
    }
    private void FixedUpdate()
    {
        if (itemSlot1.myItem && itemSlot2.myItem)
        {
            if (itemSlot1.myItem.myItem.linkable)
            {
                if (itemSlot2.myItem.myItem.linkee)
                {
                    thirdButton.SetActive(true);
                    return;
                }
            }
            if (itemSlot2.myItem.myItem.linkable)
            {
                if (itemSlot1.myItem.myItem.linkee)
                {
                    thirdButton.SetActive(true);
                    return;
                }
            }
        }
        thirdButton.SetActive(false);
    }
}
