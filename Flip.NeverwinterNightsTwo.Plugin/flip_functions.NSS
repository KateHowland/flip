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