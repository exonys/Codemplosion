using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.TreeSharp;

namespace HighVoltz.Composites
{
    public class ApplyLureAction : Action
    {
        private static readonly Dictionary<uint, string> Lures = new Dictionary<uint, string>
                                                                     {
                                                                         {68049, "Heat-Treated Spinning Lure"},
                                                                         {62673, "Feathered Lure"},
                                                                         {34861, "Sharpened Fish Hook"},
                                                                         {46006, "Glow Worm"},
                                                                         {6533, "Aquadynamic Fish Attractor"},
                                                                         {7307, "Flesh Eating Worm"},
                                                                         {6532, "Bright Baubles"},
                                                                         {6530, "Nightcrawlers"},
                                                                         {6811, "Aquadynamic Fish Lens"},
                                                                         {6529, "Shiny Bauble"},
                                                                         {67404, "Glass Fishing Bobber"},
                                                                     };

        private readonly Stopwatch _lureRecastSW = new Stopwatch();
        private readonly LocalPlayer _me = StyxWoW.Me;

        protected override RunStatus Run(object context)
        {
            if (!_me.IsCasting && !Utils.IsLureOnPole && Applylure())
                return RunStatus.Success;
            return RunStatus.Failure;
        }

        readonly uint[] _fishingHatIds = new uint[]
                                         {
                                             33820, // Weather-Beaten Fishing Hat
                                             88710 , // Nat's Hat
                                         };


        // does nothing if no lures are in bag
        private bool Applylure()
        {
            if (_lureRecastSW.IsRunning && _lureRecastSW.ElapsedMilliseconds < 10000)
                return false;
            _lureRecastSW.Reset();
            _lureRecastSW.Start();
            if (_me.Inventory.Equipped.MainHand != null &&
                _me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole)
                return false;

            //Awesome panda lure
            WoWItem pandalure = StyxWoW.Me.BagItems.FirstOrDefault(r => r.Entry == 85973);
            if (pandalure != null && !_me.HasAura(125167))
            {
                AutoAngler.Instance.Log("Appling awesome panda lure");
                Utils.UseItemByID(85973);
                return true;
            }

            // Fishing Hats
            WoWItem head = _me.Inventory.GetItemBySlot((uint)WoWEquipSlot.Head);
            if (head != null && (head.Entry == 88710 || head.Entry == 33820))
            {
                AutoAngler.Instance.Log("Appling Fishing Hat lure to fishing pole");
                Utils.UseItemByID((int)head.Entry);
                return true;
            }

            foreach (var kv in Lures)
            {
                WoWItem lureInBag = Utils.GetIteminBag(kv.Key);
                if (lureInBag != null && lureInBag.Use())
                {
                    AutoAngler.Instance.Log("Appling {0} to fishing pole", kv.Value);
                    return true;
                }
            }
            return false;
        }
    }
}