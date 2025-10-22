using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WorkBenchScale : WorkBenchBase
{
	[SerializeField] TMP_Text myScaleScreenText;

	WorkBenchInteraction myWorkBenchContainer;

    private void Awake()
    {
        myWorkBenchContainer = GetComponent<WorkBenchInteraction>();
		if (myWorkBenchContainer == null) Debug.LogError("Work Bench Utility [Scale] requires a WorkbenchObject component, please add it to" + name, this);
		myScaleScreenText.text = "";
    }

	public override void StartWorkBench()
	{
		base.StartWorkBench();
		if (myWorkBenchContainer.CurrentSlotedObject != null)
		{
			ResourceContainerObject container = myWorkBenchContainer.CurrentSlotedObject.gameObject.GetComponent<ResourceContainerObject>();
			if(container != null)
			{
				container.RegisterToQuantityChange(UpdateScaleDisplay);
				UpdateScaleDisplay(container.GetCurrentQuantity());
            }
		}
	}

	public override void StopWorkBench()
	{
		base.StopWorkBench();
        if (myWorkBenchContainer.CurrentSlotedObject != null)
        {
            ResourceContainerObject container = myWorkBenchContainer.CurrentSlotedObject.gameObject.GetComponent<ResourceContainerObject>();
            if (container != null)
            {
                container.DeregisterFromQuantityChange(UpdateScaleDisplay);
            }
        }

		myScaleScreenText.text = "";
	}

	private void UpdateScaleDisplay(float aNewAmount)
	{
		bool showInKG = aNewAmount > 1000f;
        float amountToShow = showInKG? aNewAmount/1000f : aNewAmount;
		string scaleScreenText = amountToShow.ToString() + (showInKG? " kg" : " g");
		myScaleScreenText.text = scaleScreenText;
    }
}
