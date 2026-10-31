using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;
using Game.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game.logic.Game.Logic.PetSkills
{
    public class CE2961 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_added = 0;
        private int m_currentId;
        private Living m_source;

        public CE2961(
          int count,
          int probability,
          int type,
          int skillId,
          int delay,
          string elementID,
          Living source)
          : base(ePetEffectType.CE2961, elementID)
        {
            this.m_count = count;
            this.m_coldDown = count;
            this.m_probability = probability == -1 ? 10000 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
            this.m_source = source;
        }

        public override bool Start(Living living)
        {
            CE2961 ofType = living.PetEffectList.GetOfType(ePetEffectType.CE2961) as CE2961;
            if (ofType == null)
                return base.Start(living);
            ofType.m_probability = this.m_probability > ofType.m_probability ? this.m_probability : ofType.m_probability;
            return true;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginNextTurn += new LivingEventHandle(this.Player_BeginNextTurn);
            player.BeginSelfTurn += new LivingEventHandle(this.Player_BeginSelfTurn);
            player.PlayerClearBuffSkillPet += new PlayerEventHandle(this.Player_PlayerClearBuffSkillPet);
        }

        private void Player_PlayerClearBuffSkillPet(Player player)
        {
            this.Stop();
        }

        private void Player_BeginNextTurn(Living living)
        {
        }

        private void Player_BeginSelfTurn(Living living)
        {
            --this.m_count;
            if (this.m_count < 0)
            {
                this.Stop();
            }
            else
            {
                this.m_added = 8 * living.MaxBlood / 100;
                living.AddRemoveEnergy(20);
                living.AddBlood(-this.m_added, 1);
                if (living.Blood <= 0)
                {
                    living.Die();
                    if (this.m_source != null && this.m_source is Player)
                        (this.m_source as Player).PlayerDetail.OnKillingLiving((AbstractGame)this.m_source.Game, 2, living.Id, living.IsLiving, this.m_added);
                }
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginNextTurn -= new LivingEventHandle(this.Player_BeginNextTurn);
            player.BeginSelfTurn -= new LivingEventHandle(this.Player_BeginSelfTurn);
        }
    }
}

