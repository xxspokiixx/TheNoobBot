/*Kill mob near totem
VALUE HARDCODED Modify to use Extra! TODO
 */

WoWUnit unit = ObjectManager.GetNearestWoWUnit(ObjectManager.GetWoWUnitByEntry(questObjective.Entry, questObjective.IsDead), questObjective.IgnoreNotSelectable, questObjective.IgnoreBlackList,
	questObjective.AllowPlayerControlled);
Point pos = ObjectManager.Me.Position; /* Initialize or getting an error */
int q = QuestID; /* not used but otherwise getting warning QuestID not used */
uint baseAddress = 0;
uint baseAddressTotem = 0;


if (unit.IsValid && !nManagerSetting.IsBlackListedZone(unit.Position) && !nManagerSetting.IsBlackListed(unit.Guid))
{
			
	if (unit.IsValid && unit.NotAttackable)
	{	
		nManagerSetting.AddBlackList(unit.Guid, 30*10000); //Blacklist NotAttackable units
		return false;
	}
	
	nManager.Wow.Helpers.Quest.GetSetIgnoreFight = true; 
		
	if(!nManager.Wow.ObjectManager.ObjectManager.GetHostileUnitAttackingPlayer().Exists(x=> x.GetBaseAddress == unit.GetBaseAddress))
	{
		//Go Toward the unit until we aggro it
		baseAddress = MovementManager.FindTarget(unit, CombatClass.GetAggroRange);
	
		//Pre Select Target
		if (unit.IsValid && unit.Position.DistanceTo(ObjectManager.Me.Position) <= 80 && ObjectManager.Target.Guid != unit.Guid)
		{
			Interact.InteractWith(unit.GetBaseAddress);
		}
		
		
		Thread.Sleep(500);
		
		if (baseAddress <= 0)
			return false;
		if (baseAddress > 0 && (unit.IsValid && ((unit.GetDistance > questObjective.Range) && !ObjectManager.Me.InCombat)))
			return false;
			
		MountTask.DismountMount();
	}
	
	WoWUnit totem = ObjectManager.GetNearestWoWUnit(ObjectManager.GetWoWUnitByEntry(25987, questObjective.IsDead),questObjective.IgnoreNotSelectable, questObjective.IgnoreBlackList,
	questObjective.AllowPlayerControlled);
	
	//Place totem if its not there
	if(!totem.IsValid)
	{
		if ((ObjectManager.Me.Position.DistanceTo(unit.Position) >= 20) || ItemsManager.GetItemCount(questObjective.UseItemId) <= 0 || ItemsManager.IsItemOnCooldown(questObjective.UseItemId) || !ItemsManager.IsItemUsable(questObjective.UseItemId))
		return false;

		ItemsManager.UseItem(ItemsManager.GetItemNameById(questObjective.UseItemId));
	}
	
	//The mob is aggro, go back to the totem to be in range of it		
	baseAddressTotem = MovementManager.FindTarget(totem, 5f);
	
	if (baseAddressTotem <= 0)
		return false;
	if (baseAddressTotem > 0 && (totem.IsValid && totem.GetDistance > questObjective.Range))
		return false;

	MovementManager.StopMove();
	MountTask.DismountMount();
	nManager.Wow.Helpers.Quest.GetSetIgnoreFight = false;
	
	return false; //Let the bot kill the mob(s)
	
}
	/* Move to Zone/Hotspot */
else if (!MovementManager.InMovement)
{
	nManager.Wow.Helpers.Quest.GetSetIgnoreFight = false;
	if (questObjective.PathHotspots[nManager.Helpful.Math.NearestPointOfListPoints(questObjective.PathHotspots, ObjectManager.Me.Position)].DistanceTo(ObjectManager.Me.Position) > 5)
	{
		nManager.Wow.Helpers.Quest.TravelToQuestZone(questObjective.PathHotspots[nManager.Helpful.Math.NearestPointOfListPoints(questObjective.PathHotspots, ObjectManager.Me.Position)],
			ref questObjective.TravelToQuestZone, questObjective.ContinentId, questObjective.ForceTravelToQuestZone);
		MovementManager.Go(PathFinder.FindPath(questObjective.PathHotspots[nManager.Helpful.Math.NearestPointOfListPoints(questObjective.PathHotspots, ObjectManager.Me.Position)]));
	}
	else
	{
		MovementManager.GoLoop(questObjective.PathHotspots);
	}
}