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
    public class AE2961 : BasePetEffect
    {
        private int m_type = 0;
        private int m_count = 0;
        private int m_probability = 0;
        private int m_delay = 0;
        private int m_coldDown = 0;
        private int m_added = 0;
        private int m_currentId;

        public AE2961(
          int count,
          int probability,
          int type,
          int skillId,
          int delay,
          string elementID)
          : base(ePetEffectType.AE2961, elementID)
        {
            this.m_count = count;
            this.m_coldDown = count;
            this.m_probability = probability == -1 ? 10000 : probability;
            this.m_type = type;
            this.m_delay = delay;
            this.m_currentId = skillId;
        }

        public override bool Start(Living living)
        {
            AE2961 ofType = living.PetEffectList.GetOfType(ePetEffectType.AE2961) as AE2961;
            if (ofType == null)
                return base.Start(living);
            ofType.m_probability = this.m_probability > ofType.m_probability ? this.m_probability : ofType.m_probability;
            return true;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.PlayerBuffSkillPet += new PlayerEventHandle(this.Player_PlayerBuffSkillPet);
        }

        private void Player_PlayerBuffSkillPet(Player player)
        {
            if (player.PetEffects.CurrentUseSkill != this.m_currentId || !(player.Game is PVPGame))
                return;
            foreach (Living allEnemyPlayer in player.Game.GetAllEnemyPlayers((Living)player))
                allEnemyPlayer.AddPetEffect((AbstractPetEffect)new CE2961(3, this.m_probability, this.m_type, this.m_currentId, this.m_delay, this.ElementInfo.ID.ToString(), (Living)player), 0);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerBuffSkillPet -= new PlayerEventHandle(this.Player_PlayerBuffSkillPet);
        }
    }
}