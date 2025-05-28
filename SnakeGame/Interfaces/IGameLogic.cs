using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Interfaces
{
    public interface IGameLogic
    {
        void StartGame();
        void PauseGame();
        void RestartGame();
        void Update();
    }
}
