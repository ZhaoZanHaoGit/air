using Hypertonic.GridPlacement.Enums;
using System;
using UnityEngine;


namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    /// <summary>
    /// This is an example of how you could implement a manager that handles user input for interating
    /// with a grid and performs the relevent actions on the grid manager.
    /// </summary>
    [SerializeField]
    public class GridControlManager_sample : MonoBehaviour
    {
        public delegate void GridControlManagerEvent(GameObject obj);
        public static event GridControlManagerEvent OnObjectPlacedOnGrid;


        [SerializeField]
        public GameObject _cancelPlacementButton;

        
        public GameObject _deleteObjectButton;

        public GameObject _selectedGridObject;




        private void OnEnable()
        {
            Button_ConfirmPlacement.OnConfirmPlacementPressed += HandleConfirmButtonPressed;
            Button_RotateLeft.OnRotateLeftPressed += HandleRotateLeftPressed;
            Button_RotateRight.OnRotateRightPressed += HandleRotateRightPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed += HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacementPressed;
            Button_Delete.OnDeletePressed += HandleDeleteObjectPressed;
            CreateItemBtn.OnOptionSelected += HandleGridObjectOptionSelected;
           UI_TestPanelItemBTN.OnOptionSelected += HandleGridObjectOptionSelected;
            ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;


            _cancelPlacementButton.SetActive(true);
        }

        private void OnDisable()
        {
            Button_ConfirmPlacement.OnConfirmPlacementPressed -= HandleConfirmButtonPressed;
            Button_RotateLeft.OnRotateLeftPressed -= HandleRotateLeftPressed;
            Button_RotateRight.OnRotateRightPressed -= HandleRotateRightPressed;
            Button_ChangeAlignment.OnChangeAlignmentPressed -= HandleChangeAlignmentPressed;
            Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacementPressed;
            Button_Delete.OnDeletePressed -= HandleDeleteObjectPressed;
            CreateItemBtn.OnOptionSelected -= HandleGridObjectOptionSelected;
            UI_TestPanelItemBTN.OnOptionSelected -= HandleGridObjectOptionSelected;
            ExampleGridObject.OnObjectSelected -= HandleExampleGridObjectSelected;
        }

        private void HandleConfirmButtonPressed()
        {
            bool placed = GridManagerAccessor.GridManager.ConfirmPlacement();

            if (placed)
            {
                OnObjectPlacedOnGrid?.Invoke(_selectedGridObject);
                _selectedGridObject = null;
            }
        }

        private void HandleRotateLeftPressed()
        {
            _selectedGridObject.transform.Rotate(new Vector3(0, -90, 0));

            GridManagerAccessor.GridManager.HandleGridObjectRotated();
        }

        private void HandleRotateRightPressed()
        {
            _selectedGridObject.transform.Rotate(new Vector3(0, 90, 0));

            GridManagerAccessor.GridManager.HandleGridObjectRotated();
        }

        private void HandleChangeAlignmentPressed(ObjectAlignment objectAlignment)
        {
            GridManagerAccessor.GridManager.ChangeAlignment(objectAlignment);
        }

        private void HandleCancelPlacementPressed()
        {
            if (_selectedGridObject.GetComponent<Item3D>() && _selectedGridObject.GetComponent<Item3D>().hasCreate)
            { ReturnItemUI(_selectedGridObject); }

            GridManagerAccessor.GridManager.CancelPlacement();
            _selectedGridObject = null;

        }

       public void HandleDeleteObjectPressed()
        {

            ReturnItemUI(_selectedGridObject);
            GridManagerAccessor.GridManager.DeleteObject(_selectedGridObject);
            _selectedGridObject = null;

        }

        private void HandleGridObjectOptionSelected(GameObject gridObject)
        {
            _selectedGridObject = gridObject;

             _cancelPlacementButton.SetActive(true);
             _deleteObjectButton.SetActive(false);
        }

        public void HandleExampleGridObjectSelected(GameObject gridObject)
        {
            _selectedGridObject = gridObject;

            _cancelPlacementButton.SetActive(false);
            _deleteObjectButton.SetActive(true);

            GridManagerAccessor.GridManager.ModifyPlacementOfGridObject(gridObject);
        }
        public void ReturnItemUI(GameObject @object)
        {
            if (@object.GetComponent<Item3D>().itemUI == null)
            { return; }    
            @object.GetComponent<Item3D>().itemUI.itemnum++;
            @object.GetComponent<Item3D>().itemUI.chazhi--;
            @object.GetComponent<Item3D>().itemUI.UpdateUIState();
            // ItemManager.Instance.AddItem(@object.GetComponent<Item3D>().itemSelf, 1);
        }
    }
}