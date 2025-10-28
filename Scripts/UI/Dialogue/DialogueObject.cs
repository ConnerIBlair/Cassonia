using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;

    public string[] Dialogue => dialogue; // Gets the dialogue we wrote and prevents the original from being changed while having a way of changing it still
                                          // called a getter

    public bool HasResponses => Responses != null && Responses.Length > 0; // This is a getter and it NEEDS to begin with a capital letter

    public Response[] Responses => responses;
}
