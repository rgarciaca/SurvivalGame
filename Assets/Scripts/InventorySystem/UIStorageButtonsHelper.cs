using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStorageButtonsHelper : MonoBehaviour
{
    public Action OnUseButtonClick, OnDropButtonClick;
    public Button useButton, dropButton;


    // Start is called before the first frame update
    void Start()
    {
        useButton.onClick.AddListener(() => OnUseButtonClick?.Invoke());
        dropButton.onClick.AddListener(() => OnDropButtonClick?.Invoke());

    }

    public void HideAllButtons()
    {
        ToggleDropButton(false);
        ToggleUseButton(false);
    }

    public void ToggleUseButton(bool val)
    {
        useButton.interactable = val;
    }

    public void ToggleDropButton(bool val)
    {
        dropButton.interactable = val;
    }
}
