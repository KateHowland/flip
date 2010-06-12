int CreatureHasItemEquipped(object oCreature, object oItem)
{
	int iLoop;

	// Loop through <span class="highlight">equipped</span> items
	for(iLoop = 0; iLoop <= NUM_INVENTORY_SLOTS; iLoop++)
	{
	    object oEquippedItem = GetItemInSlot(iLoop, oCreature);
	    if (oItem == oEquippedItem) {
			return 1;
		} 
	}
	
	return 0;
}


string GetSafeResRef(string sTag)
{	
	object o = GetObjectByTag(sTag,0);
	
	// If the object exists, use its resref:
	if (o != OBJECT_INVALID) {
		return GetResRef(o);
	}
	
	// If the object doesn't exist, use the given tag as a resref:
	else {
		return sTag;
	}
}


object GetObjectInArea(string sTag, object oTargetArea)
{
	int nNth = 0;

	do {
		object oObject = GetObjectByTag(sTag,nNth);
		
		if (oObject == OBJECT_INVALID) {
			return OBJECT_INVALID; // couldn't find anything, so give up
		}
		else {
			object oObjectArea = GetArea(oObject);
			
			if (oObjectArea != OBJECT_INVALID && oObjectArea == oTargetArea) {
				return oObject;
			}	
		}
		
		nNth = nNth+1;
	}
	while (TRUE);

	return OBJECT_INVALID;
}


object GetAreaFromTag(string sTag)
{
	object oArea = GetFirstArea();
	while (oArea != OBJECT_INVALID)
	{
		if (GetTag(oArea) == sTag) {
			return oArea;
		}
		else {
			oArea = GetNextArea();
		}
	}
	return OBJECT_INVALID;
}