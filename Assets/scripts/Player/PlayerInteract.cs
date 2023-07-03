using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{

    [SerializeField] UnityEvent OnInteracted;

    private void OnInteract(InputValue value)
    {
        
        Interact();
    }
    private void Interact()
    {
        OnInteracted?.Invoke();
    }
    private void InteractRange()
    {


    }
    private void OnState(InputValue value)
    {
        PopUpUI ui = GameManager.Resource.Load<PopUpUI>("UI/StateUI");
        GameManager.UI.ShowPopUpUI(ui);
    }


}
