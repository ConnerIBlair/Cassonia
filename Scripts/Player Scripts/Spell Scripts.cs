#region Spell Talbe
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SpellTable : MonoBehaviour, IInteractable
//{
//    public GameObject spellToLearn;

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") && other.TryGetComponent(out CharacterScript player))
//        {
//            player.Interactable = this;
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") && other.TryGetComponent(out CharacterScript player))
//        {
//            if (player.Interactable is SpellTable spellTable && spellTable == this)
//            // This makes sure that if there are multiple interactbles
//            // in a small space that this is the activator being used before closing the interactable.
//            {
//                player.Interactable = null;
//            }
//        }
//    }
//    public void Interact(CharacterScript player)
//    {
//        Debug.Log("New Spell!");
//        player.Spell = spellToLearn.GetComponent<ISpell>();
//    }
//}
#endregion

//public interface IInteractable
//{
//    void Interact(CharacterScript player);
//}

#region Player Script
// Interact is with the dialolgue system
//if (Input.GetButtonDown("Interact"))
//{
//    //Interactable?.Interact(this);// This being the player
//    if (Interactable != null) // Same thing as above code
//    {
//        Interactable.Interact(this);
//    }
//}
//if (Input.GetButtonDown("Cast"))
//{
//    Spell?.Cast(this);
//}
#endregion

#region Spell
//public void Cast(CharacterScript player) // Code commented above is original. Cast() + ISpell interface is new
//{
//    if (canCast && sAnimator.canCast)
//        CastSpell();
//}
#endregion
