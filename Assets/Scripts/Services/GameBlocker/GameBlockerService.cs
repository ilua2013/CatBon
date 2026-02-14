using System;
using UnityEngine;

namespace Services.GameBlocker
{
    public class GameBlockerService : IGameBlockerService
    {
        private readonly Setting setting;

        public GameBlockerService(Setting setting)
        {
            this.setting = setting;
        }
        
        public void Block()
        {
            var games = setting.Games;

            foreach (var game in games)
            {
                game.Button.interactable = false;
                game.Lock.gameObject.SetActive(true);
            }
        }

        public void Unblock()
        {
            var games = setting.Games;

            foreach (var game in games)
            {
                game.Button.interactable = true;
                game.Lock.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public class Setting
        {
            [field: SerializeField] public BlockedGameData[] Games { get; private set; }
        }
    }
}