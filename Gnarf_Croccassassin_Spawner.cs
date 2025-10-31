using XRL.UI;

namespace XRL.World.Parts
{
    public class Gnarf_Croccassassin_Spawner : IPart
    {
        public int SpawnChance = 10;
        public string Spawn = "Gnarf_Croccassassin";
        public bool WillJoinParty = true;
        public override bool WantEvent(int ID, int Cascade)
        {
            return base.WantEvent(ID, Cascade)
                || ID == TakenEvent.ID
            ;
        }

        static string CheckedProperty = nameof(Gnarf_Croccassassin_Spawner) + "_Checked";
        public override bool HandleEvent(TakenEvent E)
        {
            if (E.Actor.IsInActiveZone())
            {
                if (ParentObject.GetIntProperty(CheckedProperty) != 0)
                {
                    return base.HandleEvent(E);
                }
                ParentObject.SetIntProperty(CheckedProperty, 1);
                for (int count = ParentObject.Count, index = 0; index < count; index++)
                {
                    if (Options.GetOptionBool("Gnarf_Croccassassins_Oops") || SpawnChance.in100())
                    {
                        GameObject toReplace = ParentObject;
                        if (ParentObject.Count > 1)
                        {
                            toReplace = ParentObject.RemoveOne();
                        }
                        GameObject replaceWith = null;
                        GameObjectFactory.ProcessSpecification(Spawn, AfterObjectCreated: obj =>
                        {
                            if (E.Actor.CurrentCell.GetConnectedSpawnLocation() is Cell cell)
                            {
                                if (replaceWith == null) replaceWith = obj;
                                cell.AddObject(obj, Forced: true, Silent: true);
                            }
                        });

                        if (replaceWith != null)
                        {
                            var verb = replaceWith.GetxTag("TakeReplace", "Verb", "uncoil");
                            var extra = replaceWith.GetxTag("TakeReplace", "Extra", $"from {replaceWith.its} dormant form ready to attack");
                            XDidY(replaceWith, verb, extra, ColorAsGoodFor: replaceWith, EndMark: "!", UsePopup: true);
                            E.RequestInterfaceExit();
                            toReplace.Destroy(Silent: true);
                        }
                    }
                }
                if (!ParentObject.IsValid()) return false;
            }

            return base.HandleEvent(E);
        }
    
    }
}