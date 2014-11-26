#pragma strict

//This script allows you to create equipment effects that will be called either OnEquip or WhileEquipped. This is usefull for magic effects and stat handling.

@script AddComponentMenu ("Inventory/Items/Equipment Effect")
@script RequireComponent(Item)

private var effectActive = false;

public var dmg = 0;

function Update () 
{
	if (effectActive == true)
	{
	}
}

function EquipmentEffectToggle (effectIs : boolean)
{
	if (effectIs == true)
	{
		effectActive = true;
		
		Debug.LogWarning("Remember to insert code for the EquipmentEffect script you have attached to " + transform.name + ".");
		
		dmg += 20;
		
	}
	else
	{
		effectActive = false;
		
		dmg -= 20;
	}
}