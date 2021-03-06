﻿namespace InvokerReborn.Abilities
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Threading;

    using InvokerReborn.Interfaces;

    internal class ForgeSpirit : InvokerComboAbility
    {
        private readonly Ability _exort;

        private readonly Ability _quas;

        public ForgeSpirit(Hero me)
            : this(me, () => 100)
        {
        }

        public ForgeSpirit(Hero me, Func<int> extraDelay)
            : base(me, extraDelay)
        {
            this.Ability = me.FindSpell("invoker_forge_spirit");

            this._quas = me.Spellbook.SpellQ;
            this._exort = me.Spellbook.SpellE;
        }

        public override SequenceEntryID ID => SequenceEntryID.ForgeSpirit;

        public override bool IsSkilled
            => (this.Owner.Spellbook.SpellQ.Level > 0) && (this.Owner.Spellbook.SpellE.Level > 0);

        public override async Task ExecuteAsync(Unit target, CancellationToken tk = default(CancellationToken))
        {
            var invokeDelay = await this.UseInvokeAbilityAsync(target, tk);
            await Await.Delay(Math.Max(0, this.ExtraDelay() - invokeDelay), tk);
            this.Ability.UseAbility();

            DelayAction.Add(
                250,
                () =>
                    ObjectManager.GetEntitiesFast<Unit>()
                                 .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit)
                                 .ToList()
                                 .ForEach(x => x.Attack(target)));
        }

        public override async Task<int> InvokeAbility(
            bool useCooldown,
            CancellationToken tk = default(CancellationToken))
        {
            // E E Q
            return await this.InvokeAbility(new[] { this._exort, this._exort, this._quas }, useCooldown, tk);
        }
    }
}