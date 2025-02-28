using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Satchel;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

namespace ShadesAnywhere {
    public class ShadesAnywhere: Mod {
        new public string GetName() => "ShadesAnywhere";
        public override string GetVersion() => "1.0.0.1";

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            new ILHook(typeof(HeroController).GetMethod("Die", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), die);
            On.PlayMakerFSM.OnEnable += editFSM;
        }

        private void editFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.name == "Hero Death" && self.FsmName == "Hero Death Anim") {
                StringCompare stringCompare = self.GetValidState("Map Zone").GetFirstActionOfType<StringCompare>();
                stringCompare.equalEvent = stringCompare.notEqualEvent = FsmEvent.GetFsmEvent("FINISHED");
            }
        }

        private void die(ILContext il) {
            ILCursor cursor = new ILCursor(il).Goto(0);
            cursor.GotoNext(i => i.MatchLdstr("DREAM_WORLD"));
            cursor.GotoNext(i => i.Match(OpCodes.Brtrue_S));
            cursor.EmitDelegate<Func<bool, bool>>(j => { return false; });
            cursor.GotoNext(i => i.Match(OpCodes.Brfalse_S));
            cursor.EmitDelegate<Func<bool, bool>>(j => { return false; });
        }
    }
}